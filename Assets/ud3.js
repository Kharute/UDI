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

// 로그인 엔드포인트
app.post('/login', (req, res) => {
    const username = req.body.username;
    const password = req.body.password;

    if (!username || !password) {
        return res.json({ success: false, message: 'Please provide username and password' });
    }

    const query = 'SELECT USER_ID FROM USER_LOGIN WHERE username = ? AND password = ?';
    db.query(query, [username, password], (err, results) => {
        if (err) {
            return res.json({ success: false, message: 'Database query error' });
        }

        if (results.length > 0) {
            const userId = results[0].USER_ID;
            const updateQuery = 'UPDATE USER_LOGIN SET LOGIN_TIME = ? WHERE USER_ID = ?';
            const currentTime = new Date();

            db.query(updateQuery, [currentTime, userId], (updateErr, updateResults) => {
                if (updateErr) {
                    return res.json({ success: false, message: 'Failed to update login time' });
                }

                const detailsQuery = 'SELECT * FROM USER_DETAILS WHERE USER_ID = ?';
                db.query(detailsQuery, [userId], (detailsErr, detailsResults) => {
                    if (detailsErr) {
                        return res.json({ success: false, message: 'Failed to retrieve user details' });
                    }

                    res.json({ 
                        success: true, 
                        message: 'Login successful and login time updated', 
                        userDetails: detailsResults[0] 
                    });
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

app.listen(port, () => {
    console.log(`Server running on http://localhost:${port}`);
});