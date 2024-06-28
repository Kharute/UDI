const express = require('express');
const bodyParser = require('body-parser');
const mysql = require('mysql');
const app = express();
const port = 3000;

// MySQL 연결 설정
const db = mysql.createConnection({
    host: 'localhost',
    user: 'root',
    password: '1234',
    database: 'UD3'
});

db.connect((err) => {
    if (err) {
        throw err;
    }
    console.log('MySQL connected...');
});

app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: true }));
app.post('/login', (req, res) => {
    const username = req.body.username;
    const password = req.body.password;

    if (!username || !password) {
        return res.json({ success: false, message: 'Please provide username and password' });
    }

    const query = 'SELECT USER_ID, LOGIN_COUNT_MONTH, LOGIN_ISFIRST FROM USER_LOGIN WHERE username = ? AND password = ?';
    db.query(query, [username, password], (err, results) => {
        if (err) {
            return res.json({ success: false, message: 'Database query error' });
        }

        if (results.length > 0) {
            const userId = results[0].USER_ID;
            const loginCountMonth = results[0].LOGIN_COUNT_MONTH;
            let loginIsFirst = false;
            const currentTime = new Date();
            const currentHour = currentTime.getHours();

            // 로그인 시간 업데이트 쿼리
            const updateLoginTimeQuery = 'UPDATE USER_LOGIN SET LOGIN_TIME = ? WHERE USER_ID = ?';
            
            // 트랜잭션 시작
            db.beginTransaction(transactionErr => {
                if (transactionErr) {
                    return res.json({ success: false, message: 'Failed to start transaction' });
                }

                // 로그인 시간 업데이트
                db.query(updateLoginTimeQuery, [currentTime, userId], (updateTimeErr, updateTimeResults) => {
                    if (updateTimeErr) {
                        return db.rollback(() => {
                            res.json({ success: false, message: 'Failed to update login time' });
                        });
                    }

                    let newLoginCountMonth = loginCountMonth;

                    // 오전 6시 기준으로 로그인 회수 증가
                    if (currentHour >= 6) {
                        const incrementLoginCountQuery = 'UPDATE USER_LOGIN SET LOGIN_COUNT_MONTH = LOGIN_COUNT_MONTH + 1, LOGIN_ISFIRST = 1 WHERE USER_ID = ?';
                        db.query(incrementLoginCountQuery, [userId], (incrementCountErr, incrementCountResults) => {
                            if (incrementCountErr) {
                                return db.rollback(() => {
                                    res.json({ success: false, message: 'Failed to increment login count' });
                                });
                            }
                            newLoginCountMonth += 1;
                            loginIsFirst = true;

                            const detailsQuery = 'SELECT * FROM USER_DETAILS WHERE USER_ID = ?';
                            db.query(detailsQuery, [userId], (detailsErr, detailsResults) => {
                                if (detailsErr) {
                                    return db.rollback(() => {
                                        res.json({ success: false, message: 'Failed to retrieve user details' });
                                    });
                                }

                                db.commit(commitErr => {
                                    if (commitErr) {
                                        return db.rollback(() => {
                                            res.json({ success: false, message: 'Failed to commit transaction' });
                                        });
                                    }

                                    res.json({ 
                                        success: true, 
                                        message: 'Login successful, login time updated, and login count incremented', 
                                        userDetails: detailsResults[0],
                                        loginCountMonth: newLoginCountMonth,
                                        loginIsFirst: loginIsFirst 
                                    });
                                });
                            });
                        });
                    } else {
                        const detailsQuery = 'SELECT * FROM USER_DETAILS WHERE USER_ID = ?';
                        db.query(detailsQuery, [userId], (detailsErr, detailsResults) => {
                            if (detailsErr) {
                                return db.rollback(() => {
                                    res.json({ success: false, message: 'Failed to retrieve user details' });
                                });
                            }

                            db.commit(commitErr => {
                                if (commitErr) {
                                    return db.rollback(() => {
                                        res.json({ success: false, message: 'Failed to commit transaction' });
                                    });
                                }

                                res.json({ 
                                    success: true, 
                                    message: 'Login successful and login time updated', 
                                    userDetails: detailsResults[0],
                                    loginCountMonth: newLoginCountMonth,
                                    loginIsFirst: loginIsFirst 
                                });
                            });
                        });
                    }
                });
            });
        } else {
            res.json({ success: false, message: 'Invalid username or password' });
        }
    });
});

// 새로운 사용자 세부 정보 생성 또는 조회 엔드포인트
app.post('/createUserDetails', (req, res) => {
    const { userId } = req.body;

    const checkQuery = 'SELECT * FROM USER_DETAILS WHERE USER_ID = ?';
    db.query(checkQuery, [userId], (checkErr, checkResults) => {
        if (checkErr) {
            return res.json({ success: false, message: 'Database query error' });
        }

        if (checkResults.length > 0) {
            // 유저 정보가 이미 존재할 경우
            res.json({ success: true, message: 'User details found', userDetails: checkResults[0] });
        } else {
            // 유저 정보가 없을 경우
            const insertQuery = 'INSERT INTO USER_DETAILS (USER_ID, NICKNAME, LEVEL, EXPERIENCE, GOLD, JEWEL, INVENTORY) VALUES (?, "NONAME", 1, 0, 0, 0, "")';
            db.query(insertQuery, [userId], (insertErr, insertResults) => {
                if (insertErr) {
                    return res.json({ success: false, message: 'Failed to create user details' });
                }

                const newDetailsQuery = 'SELECT * FROM USER_DETAILS WHERE USER_ID = ?';
                db.query(newDetailsQuery, [userId], (newDetailsErr, newDetailsResults) => {
                    if (newDetailsErr) {
                        return res.json({ success: false, message: 'Failed to retrieve newly created user details' });
                    }

                    res.json({ success: true, message: 'User details created successfully', userDetails: newDetailsResults[0] });
                });
            });
        }
    });
});

// 허용된 컬럼 이름을 설정 (SQL 인젝션 방지용)
const allowedColumns = ['LEVEL', 'EXPERIENCE', 'GOLD', 'JEWEL']; // 실제 사용 중인 컬럼명 리스트

// 업데이트 엔드포인트
app.post('/updateUserDetails', (req, res) => {
    const { userId, column, value } = req.body;

    // 컬럼이 허용된 목록에 있는지 확인
    if (!allowedColumns.includes(column)) {
        return res.status(400).json({ error: 'Invalid column name' });
    }

    const checkQuery = 'SELECT * FROM USER_DETAILS WHERE USER_ID = ?';
    db.query(checkQuery, [userId], (checkErr, checkResults) => {
        if (checkErr) {
            return res.json({ success: false, message: 'Database query error' });
        
        } else {
            const query = `UPDATE USER_DETAILS SET ?? = ? WHERE USER_ID = ?`; // SQL 업데이트 쿼리

            db.query(query, [column, value, userId], (err, result) => {
                if (err) {
                    return res.json({ success: false, message: 'Failed to update user details' });
                }
                res.json({ message: 'Update successful', result });
            });
        }
    });
});

// 
app.post('/updateUserGoods', (req, res) => {
    const { userId, column, value } = req.body;

    // 컬럼이 허용된 목록에 있는지 확인
    if (!allowedColumns.includes(column)) {
        return res.status(400).json({ error: 'Invalid column name' });
    }

    const checkQuery = 'SELECT * FROM USER_ITEM_GOODS WHERE USER_ID = ?';
    db.query(checkQuery, [userId], (checkErr, checkResults) => {
        if (checkErr) {
            return res.json({ success: false, message: 'Database query error' });
        
        } else {
            const query = `UPDATE USER_ITEM_GOODS SET ?? = ? WHERE USER_ID = ?`; // SQL 업데이트 쿼리

            db.query(query, [column, value, userId], (err, result) => {
                if (err) {
                    return res.json({ success: false, message: 'Failed to update user details' });
                }
                res.json({ message: 'Update successful', result });
            });
        }
    });
});

app.listen(port, () => {
    console.log(`Server running on http://localhost:${port}`);
});
