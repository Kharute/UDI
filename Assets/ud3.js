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

    const query = 'SELECT USER_ID, LOGIN_COUNT_MONTH, LOGIN_ISFIRST, LOGIN_DATE FROM USER_LOGIN WHERE user_name = ? AND password = ?';
    db.query(query, [username, password], (err, results) => {
        if (err) {
            return res.json({ success: false, message: 'Database query error to login' });
        }

        if (results.length > 0) {
            const userId = results[0].USER_ID;
            let loginCountMonth = results[0].LOGIN_COUNT_MONTH;
            let loginIsFirst = false;
            const lastLoginDate = results[0].LOGIN_DATE ? new Date(results[0].LOGIN_DATE) : null;
            const currentTime = new Date();
            const currentHour = currentTime.getHours();

            const updateLoginTimeQuery = 'UPDATE USER_LOGIN SET LOGIN_TIME = ?, LOGIN_DATE = ? WHERE USER_ID = ?';
            db.beginTransaction(transactionErr => {
                if (transactionErr) {
                    return res.json({ success: false, message: 'Failed to start transaction' });
                }

                db.query(updateLoginTimeQuery, [currentTime, currentTime, userId], (updateTimeErr, updateTimeResults) => {
                    if (updateTimeErr) {
                        return db.rollback(() => {
                            res.json({ success: false, message: 'Failed to update login time' });
                        });
                    }

                    // Check if the month has changed
                    if (lastLoginDate && (lastLoginDate.getMonth() !== currentTime.getMonth() || lastLoginDate.getFullYear() !== currentTime.getFullYear())) {
                        loginCountMonth = 0;
                    }

                    if (currentHour >= 6 && (!lastLoginDate || lastLoginDate.toDateString() !== currentTime.toDateString())) {
                        const incrementLoginCountQuery = 'UPDATE USER_LOGIN SET LOGIN_COUNT_MONTH = ?, LOGIN_ISFIRST = 1 WHERE USER_ID = ?';
                        db.query(incrementLoginCountQuery, [loginCountMonth + 1, userId], (incrementCountErr, incrementCountResults) => {
                            if (incrementCountErr) {
                                return db.rollback(() => {
                                    res.json({ success: false, message: 'Failed to increment login count' });
                                });
                            }
                            loginCountMonth += 1;
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
                                        loginCountMonth: loginCountMonth,
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
                                    loginCountMonth: loginCountMonth,
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

app.post('/createUserDetails', (req, res) => {
    const { userId } = req.body;

    const checkUserDetailsQuery = 'SELECT * FROM USER_DETAILS WHERE USER_ID = ?';
    const checkUserGoodsQuery = 'SELECT * FROM USER_ITEM_GOODS WHERE USER_ID = ?';

    db.beginTransaction(transactionErr => {
        if (transactionErr) {
            return res.json({ success: false, message: 'Failed to start transaction' });
        }

        db.query(checkUserDetailsQuery, [userId], (checkUserDetailsErr, checkUserDetailsResults) => {
            if (checkUserDetailsErr) {
                return db.rollback(() => {
                    res.json({ success: false, message: 'Database query error to create detail Query' });
                });
            }

            db.query(checkUserGoodsQuery, [userId], (checkUserGoodsErr, checkUserGoodsResults) => {
                if (checkUserGoodsErr) {
                    return db.rollback(() => {
                        res.json({ success: false, message: 'Database query error create goods Query' });
                    });
                }

                const userDetailsExists = checkUserDetailsResults.length > 0;
                const userGoodsExists = checkUserGoodsResults.length > 0;

                const insertUserDetailsQuery = 'INSERT INTO USER_DETAILS (USER_ID, NICKNAME, LEVEL, EXPERIENCE, INVENTORY) VALUES (?, "NONAME", 1, 0, "")';
                const insertUserGoodsQuery = 'INSERT INTO USER_ITEM_GOODS (USER_ID, GOLD, JEWEL, TICKET_WEAPON, TICKET_ARMOR) VALUES (?, 0, 0, 0, 0)';

                const queries = [];

                if (!userDetailsExists) {
                    queries.push({
                        query: insertUserDetailsQuery,
                        params: [userId]
                    });
                }

                if (!userGoodsExists) {
                    queries.push({
                        query: insertUserGoodsQuery,
                        params: [userId]
                    });
                }

                if (queries.length === 0) {
                    return db.commit(commitErr => {
                        if (commitErr) {
                            return db.rollback(() => {
                                res.json({ success: false, message: 'Failed to commit transaction' });
                            });
                        }
                        res.json({ success: true, message: 'User details and item goods already exist' });
                    });
                }

                const executeQueries = (index = 0) => {
                    if (index >= queries.length) {
                        return db.commit(commitErr => {
                            if (commitErr) {
                                return db.rollback(() => {
                                    res.json({ success: false, message: 'Failed to commit transaction' });
                                });
                            }
                            res.json({ success: true, message: 'User details and/or item goods created successfully' });
                        });
                    }

                    const { query, params } = queries[index];
                    db.query(query, params, (err, results) => {
                        if (err) {
                            return db.rollback(() => {
                                res.json({ success: false, message: 'Failed to execute query' });
                            });
                        }
                        executeQueries(index + 1);
                    });
                };

                executeQueries();
            });
        });
    });
});

const detailsColumns = ['LEVEL', 'EXPERIENCE', 'SKILL_POINT'];

app.post('/updateUserDetails', (req, res) => {
    const { userId, column, value } = req.body;

    if (!detailsColumns.includes(column)) {
        return res.status(400).json({ error: 'Invalid column name' });
    }

    const checkQuery = 'SELECT * FROM USER_DETAILS WHERE USER_ID = ?';
    db.query(checkQuery, [userId], (checkErr, checkResults) => {
        if (checkErr) {
            return res.json({ success: false, message: 'Database query error update detail Query' });
        }

        if (checkResults.length === 0) {
            return res.json({ success: false, message: 'User not found' });
        }

        const query = `UPDATE USER_DETAILS SET ${mysql.escapeId(column)} = ? WHERE USER_ID = ?`;
        db.query(query, [value, userId], (err, result) => {
            if (err) {
                return res.json({ success: false, message: 'Failed to update user details' });
            }
            res.json({ message: 'Update successful', result });
        });
    });
});

const goodsColumns = ['GOLD', 'JEWEL', 'TICKET_WEAPON', 'TICKET_ARMOR'];

app.post('/updateUserGoods', (req, res) => {
    const { userId, column, value } = req.body;

    if (!goodsColumns.includes(column)) {
        return res.status(400).json({ error: 'Invalid column name' });
    }

    const checkQuery = 'SELECT * FROM USER_ITEM_GOODS WHERE USER_ID = ?';
    db.query(checkQuery, [userId], (checkErr, checkResults) => {
        if (checkErr) {
            return res.json({ success: false, message: 'Database query error update goods Query' });
        }

        if (checkResults.length === 0) {
            return res.json({ success: false, message: 'User not found' });
        }

        const query = `UPDATE USER_ITEM_GOODS SET ${mysql.escapeId(column)} = ? WHERE USER_ID = ?`;
        db.query(query, [value, userId], (err, result) => {
            if (err) {
                return res.json({ success: false, message: 'Failed to update user goods' });
            }
            res.json({ message: 'Update successful', result });
        });
    });
});

app.listen(port, () => {
    console.log(`Server running on http://localhost:${port}`);
});
