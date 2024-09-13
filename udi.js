const express = require('express');
const bodyParser = require('body-parser');
const fs = require('fs');
const xml2js = require('xml2js');
const mysql = require('mysql');
const path = require('path');

const app = express();
const ip = '15.165.15.237';
const port = 7777;

// MySQL 연결 설정
const db = mysql.createConnection({
    host: ip,
    user: 'root',
    password: '1234',
    database: 'UDI'
});

db.connect((err) => {
    if (err) {
        throw err;
    }
    console.log('MySQL connected...');
});

app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: true }));

const xmlFilePath = path.join(__dirname, 'WeaponInfo.xml'); //dirname부분 체크
let weaponList = [];

// XML 파일 읽기
fs.readFile(xmlFilePath, (err, data) => {
    if (err) {
        console.error('Error reading XML file:', err);
        return;
    }
    xml2js.parseString(data, { explicitArray: false }, (err, result) => {
        if (err) {
            console.error('Error parsing XML:', err);
            return;
        }
        // 무기 데이터를 배열로 변환
        weaponList = result.WeaponInfo.dataCategory.data.map(w => ({
            id: parseInt(w.$.WeaponID),
            name: w.$.WeaponName
        }));
        console.log('Loaded weaponList:', weaponList);
    });
});

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

            db.beginTransaction(transactionErr => {
                if (transactionErr) {
                    return res.json({ success: false, message: 'Failed to start transaction' });
                }

                const updateLoginTimeQuery = 'UPDATE USER_LOGIN SET LOGIN_TIME = ?, LOGIN_DATE = ? WHERE USER_ID = ?';
                db.query(updateLoginTimeQuery, [currentTime, currentTime, userId], (updateTimeErr) => {
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
                        db.query(incrementLoginCountQuery, [loginCountMonth + 1, userId], (incrementCountErr) => {
                            if (incrementCountErr) {
                                return db.rollback(() => {
                                    res.json({ success: false, message: 'Failed to increment login count' });
                                });
                            }
                            loginCountMonth += 1;
                            loginIsFirst = true;
                            processUserDetails();
                        });
                    } else {
                        processUserDetails();
                    }
                });

                function processUserDetails() {
                    console.log('Processing user details for USER_ID:', userId);
                    const checkUserDetailsQuery = 'SELECT * FROM USER_DETAILS WHERE USER_ID = ?';
                    db.query(checkUserDetailsQuery, [userId], (checkUserDetailsErr, userDetailsResults) => {
                        if (checkUserDetailsErr) {
                            console.error('Error checking user details:', checkUserDetailsErr);
                            return db.rollback(() => {
                                res.json({ success: false, message: 'Failed to check user details' });
                            });
                        }
                
                        console.log('User details check result:', userDetailsResults);
                
                        if (userDetailsResults.length === 0) {
                            console.log('Inserting new user details');
                            const insertUserDetailsQuery = 'INSERT INTO USER_DETAILS (USER_ID, NICKNAME, LEVEL, EXPERIENCE, SKILL_POINT, INVENTORY) VALUES (?, "NONAME", 1, 0, 0, "")';
                            db.query(insertUserDetailsQuery, [userId], (insertUserDetailsErr) => {
                                if (insertUserDetailsErr) {
                                    console.error('Error inserting user details:', insertUserDetailsErr);
                                    return db.rollback(() => {
                                        res.json({ success: false, message: 'Failed to create user details' });
                                    });
                                }
                                console.log('User details inserted successfully');
                                processUserGoods();
                            });
                        } else {
                            console.log('User details already exist');
                            processUserGoods();
                        }
                    });
                }
                function processUserGoods() {
                    const checkUserGoodsQuery = 'SELECT * FROM USER_ITEM_GOODS WHERE USER_ID = ?';
                    db.query(checkUserGoodsQuery, [userId], (checkUserGoodsErr, userGoodsResults) => {
                        if (checkUserGoodsErr) {
                            return db.rollback(() => {
                                res.json({ success: false, message: 'Failed to check user goods' });
                            });
                        }

                        if (userGoodsResults.length === 0) {
                            const insertUserGoodsQuery = 'INSERT INTO USER_ITEM_GOODS (USER_ID, GOLD, JEWEL, TICKET_WEAPON, TICKET_ARMOR) VALUES (?, 0, 0, 0, 0)';
                            db.query(insertUserGoodsQuery, [userId], (insertUserGoodsErr) => {
                                if (insertUserGoodsErr) {
                                    return db.rollback(() => {
                                        res.json({ success: false, message: 'Failed to create user goods' });
                                    });
                                }
                                finalizeLogin();
                            });
                        } else {
                            finalizeLogin();
                        }
                    });
                }

                function finalizeLogin() {
                    const detailsQuery = 'SELECT USER_ID, NICKNAME, LEVEL, EXPERIENCE, SKILL_POINT, INVENTORY FROM USER_DETAILS WHERE USER_ID = ?';
                    db.query(detailsQuery, [userId], (detailsErr, detailsResults) => {
                        if (detailsErr) {
                            return db.rollback(() => {
                                res.json({ success: false, message: 'Failed to retrieve user details' });
                            });
                        }

                        const goodsQuery = 'SELECT * FROM USER_ITEM_GOODS WHERE USER_ID = ?';
                        db.query(goodsQuery, [userId], (goodsErr, goodsResults) => {
                            if (goodsErr) {
                                return db.rollback(() => {
                                    res.json({ success: false, message: 'Failed to retrieve user item goods' });
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
                                    userGoods: goodsResults[0],
                                    loginCountMonth: loginCountMonth,
                                    loginIsFirst: loginIsFirst
                                });
                            });
                        });
                    });
                }
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
                const insertUserGoodsQuery = 'INSERT INTO USER_ITEM_GOODS (USER_ID, GOLD, JEWEL, TICKET_WEAPON, TICKET_ARMOR) VALUES (?, 100, 100, 1, 1)';

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

app.post('/loadWeaponData', (req, res) => {
    const userId = req.body.userId;
    let sql = 'SELECT WeaponID, WeaponCount FROM user_item_weapon WHERE USER_ID = ?';
    db.query(sql, [userId], (err, result) => {
      if (err) {
        res.status(500).send('Error fetching data from database');
        return;
      }
      res.json(result);
    });
});

app.post('/uploadWeaponData', async (req, res) => {
    const { USER_ID, weapons } = req.body;

    db.beginTransaction(async transactionErr => {
        if (transactionErr) {
            console.error('Failed to start transaction:', transactionErr);
            return res.status(500).json({ error: 'Failed to start transaction' });
        }

        try {
            for (const weapon of weapons) {
                const { WeaponID, WeaponCount } = weapon;
                const query = `
                    INSERT INTO user_item_weapon (USER_ID, WeaponID, WeaponCount)
                    VALUES (?, ?, ?)
                    ON DUPLICATE KEY UPDATE WeaponCount = VALUES(WeaponCount)
                `;

                await new Promise((resolve, reject) => {
                    db.query(query, [USER_ID, WeaponID, WeaponCount], (err, result) => {
                        if (err) {
                            return reject(err);
                        }
                        resolve(result);
                    });
                });
            }

            db.commit(commitErr => {
                if (commitErr) {
                    console.error('Failed to commit transaction:', commitErr);
                    return db.rollback(() => {
                        res.status(500).json({ error: 'Failed to commit transaction' });
                    });
                }
                res.status(200).json({ message: 'Data updated successfully' });
            });
        } catch (error) {
            console.error('Failed to update data:', error);
            db.rollback(() => {
                res.status(500).json({ error: 'Failed to update data', details: error.message });
            });
        }
    });
});

app.post('/gacha', (req, res) => {
    const userId = req.body.userId;
    const count = req.body.count;
    const result = {};

    for (let i = 0; i < count; i++) {
        const randomWeapon = weaponList[Math.floor(Math.random() * weaponList.length)];
        if (result[randomWeapon.id]) {
            result[randomWeapon.id]++;
        } else {
            result[randomWeapon.id] = 1;
        }
    }

    // 데이터베이스에 저장
    for (const [weaponId, weaponCount] of Object.entries(result)) {
        db.query(`SELECT WeaponCount FROM user_item_weapon WHERE USER_ID = ? AND WeaponID = ?`, [userId, weaponId], (err, rows) => {
            if (err) {
                console.error('Error querying database:', err);
                return;
            }
            if (rows.length > 0) {
                db.query(`UPDATE user_item_weapon SET WeaponCount = WeaponCount + ? WHERE USER_ID = ? AND WeaponID = ?`, [weaponCount, userId, weaponId], err => {
                    if (err) {
                        console.error('Error updating database:', err);
                    }
                });
            } else {
                db.query(`INSERT INTO user_item_weapon (USER_ID, WeaponID, WeaponCount) VALUES (?, ?, ?)`, [userId, weaponId, weaponCount], err => {
                    if (err) {
                        console.error('Error inserting into database:', err);
                    }
                });
            }
        });
    }

    res.json(result);
});

app.listen(port, () => {
    console.log(`Server running on http://${ip}:${port}`);
});
