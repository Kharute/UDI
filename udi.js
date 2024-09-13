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
    host: 'localhost',
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
            id: parseInt(w.$.weapon_id),
            name: w.$.WeaponName
        }));
        console.log('Loaded weaponList:', weaponList);
    });
});

app.post('/login', (req, res) => {
    console.log('Received request:', req.body);
    const username = req.body.username;
    const password = req.body.password;

    if (!username || !password) {
        return res.json({ success: false, message: 'Please provide username and password' });
    }

    const query = 'SELECT user_id, login_count_month, login_isfirst, login_date, login_time FROM user_login WHERE user_name = ? AND password = ?';
    db.query(query, [username, password], (err, results) => {
        if (err) {
            console.error('Database query error:', err);
            return res.json({ success: false, message: 'Database query error to login' });
        }

        if (results.length > 0) {
            const userId = results[0].user_id;
            let loginCountMonth = results[0].login_count_month;
            let loginIsFirst = login_isfirst;
            const lastLoginDate = results[0].login_date ? new Date(results[0].login_date) : null;
            const currentTime = login_time;
            const currentHour = currentTime.getHours();

            db.beginTransaction(transactionErr => {
                if (transactionErr) {
                    console.error('Transaction error:', transactionErr);
                    return res.json({ success: false, message: 'Failed to start transaction' });
                }

                const updateLoginTimeQuery = 'UPDATE user_login SET login_time = ?, login_date = ? WHERE user_id = ?';
                db.query(updateLoginTimeQuery, [currentTime, currentTime, userId], (updateTimeErr) => {
                    if (updateTimeErr) {
                        console.error('Update login time error:', updateTimeErr);
                        return db.rollback(() => {
                            res.json({ success: false, message: 'Failed to update login time' });
                        });
                    }

                    // Check if the month has changed
                    if (lastLoginDate && (lastLoginDate.getMonth() !== currentTime.getMonth() || lastLoginDate.getFullYear() !== currentTime.getFullYear())) {
                        loginCountMonth = 0;
                    }

                    if (currentHour >= 6 && (!lastLoginDate || lastLoginDate.toDateString() !== currentTime.toDateString())) {
                        const incrementLoginCountQuery = 'UPDATE user_login SET login_count_month = ?, login_isfirst = 1 WHERE user_id = ?';
                        db.query(incrementLoginCountQuery, [loginCountMonth + 1, userId], (incrementCountErr) => {
                            if (incrementCountErr) {
                                console.error('Increment login count error:', incrementCountErr);
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
                    console.log('Processing user details for user_id:', userId);
                    const checkUserDetailsQuery = 'SELECT * FROM user_details WHERE user_id = ?';
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
                            const insertUserDetailsQuery = 'INSERT INTO user_details (user_id, nickname, level, experience, skill_point, inventory) VALUES (?, "NONAME", 1, 0, 0, "")';
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
                    const checkUserGoodsQuery = 'SELECT * FROM user_item_goods WHERE user_id = ?';
                    db.query(checkUserGoodsQuery, [userId], (checkUserGoodsErr, userGoodsResults) => {
                        if (checkUserGoodsErr) {
                            console.error('Error checking user goods:', checkUserGoodsErr);
                            return db.rollback(() => {
                                res.json({ success: false, message: 'Failed to check user goods' });
                            });
                        }

                        if (userGoodsResults.length === 0) {
                            const insertUserGoodsQuery = 'INSERT INTO user_item_goods (user_id, gold, jewel, ticket_weapon, ticket_armor) VALUES (?, 0, 0, 0, 0)';
                            db.query(insertUserGoodsQuery, [userId], (insertUserGoodsErr) => {
                                if (insertUserGoodsErr) {
                                    console.error('Error inserting user goods:', insertUserGoodsErr);
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
                    const detailsQuery = 'SELECT user_id, nickname, level, experience, skill_point, inventory FROM user_details WHERE user_id = ?';
                    db.query(detailsQuery, [userId], (detailsErr, detailsResults) => {
                        if (detailsErr) {
                            console.error('Error retrieving user details:', detailsErr);
                            return db.rollback(() => {
                                res.json({ success: false, message: 'Failed to retrieve user details' });
                            });
                        }

                        const goodsQuery = 'SELECT * FROM user_item_goods WHERE user_id = ?';
                        db.query(goodsQuery, [userId], (goodsErr, goodsResults) => {
                            if (goodsErr) {
                                console.error('Error retrieving user goods:', goodsErr);
                                return db.rollback(() => {
                                    res.json({ success: false, message: 'Failed to retrieve user item goods' });
                                });
                            }

                            db.commit(commitErr => {
                                if (commitErr) {
                                    console.error('Error committing transaction:', commitErr);
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
    console.log('Received request:', req.body);
    const { userId } = req.body;

    const checkUserDetailsQuery = 'SELECT * FROM user_details WHERE user_id = ?';
    const checkUserGoodsQuery = 'SELECT * FROM user_item_goods WHERE user_id = ?';

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

                const insertUserDetailsQuery = 'INSERT INTO user_details (user_id, nickname, level, experience, inventory) VALUES (?, "NONAME", 1, 0, "")';
                const insertUserGoodsQuery = 'INSERT INTO user_item_goods (user_id, gold, jewel, ticket_weapon, ticket_armor) VALUES (?, 100, 100, 1, 1)';

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

const detailsColumns = ['level', 'experience', 'skill_point'];

app.post('/updateUserDetails', (req, res) => {
    console.log('Received request:', req.body);
    const { userId, column, value } = req.body;

    if (!detailsColumns.includes(column)) {
        return res.status(400).json({ error: 'Invalid column name' });
    }

    const checkQuery = 'SELECT * FROM user_details WHERE user_id = ?';
    db.query(checkQuery, [userId], (checkErr, checkResults) => {
        if (checkErr) {
            return res.json({ success: false, message: 'Database query error update detail Query' });
        }

        if (checkResults.length === 0) {
            return res.json({ success: false, message: 'User not found' });
        }

        const query = `UPDATE user_details SET ${mysql.escapeId(column)} = ? WHERE user_id = ?`;
        db.query(query, [value, userId], (err, result) => {
            if (err) {
                return res.json({ success: false, message: 'Failed to update user details' });
            }
            res.json({ message: 'Update successful', result });
        });
    });
});

const goodsColumns = ['gold', 'jewel', 'ticket_weapon', 'ticket_armor'];

app.post('/updateUserGoods', (req, res) => {
    console.log('Received request:', req.body);
    const { userId, column, value } = req.body;

    if (!goodsColumns.includes(column)) {
        return res.status(400).json({ error: 'Invalid column name' });
    }

    const checkQuery = 'SELECT * FROM user_item_goods WHERE user_id = ?';
    db.query(checkQuery, [userId], (checkErr, checkResults) => {
        if (checkErr) {
            return res.json({ success: false, message: 'Database query error update goods Query' });
        }

        if (checkResults.length === 0) {
            return res.json({ success: false, message: 'User not found' });
        }

        const query = `UPDATE user_item_goods SET ${mysql.escapeId(column)} = ? WHERE user_id = ?`;
        db.query(query, [value, userId], (err, result) => {
            if (err) {
                return res.json({ success: false, message: 'Failed to update user goods' });
            }
            res.json({ message: 'Update successful', result });
        });
    });
});

app.post('/loadWeaponData', (req, res) => {
    console.log('Received request:', req.body);
    const userId = req.body.userId;
    let sql = 'SELECT weapon_id, weapon_count FROM user_item_weapon WHERE user_id = ?';
    db.query(sql, [userId], (err, result) => {
      if (err) {
        res.status(500).send('Error fetching data from database');
        return;
      }
      res.json(result);
    });
});

app.post('/uploadWeaponData', async (req, res) => {
    console.log('Received request:', req.body);

    const { USER_ID, weapons } = req.body;

    db.beginTransaction(async transactionErr => {
        if (transactionErr) {
            console.error('Failed to start transaction:', transactionErr);
            return res.status(500).json({ error: 'Failed to start transaction' });
        }

        try {
            for (const weapon of weapons) {
                const { weapon_id, weapon_count } = weapon;
                const query = `
                    INSERT INTO user_item_weapon (user_id, weapon_id, weapon_count)
                    VALUES (?, ?, ?)
                    ON DUPLICATE KEY UPDATE weapon_count = VALUES(weapon_count)
                `;

                await new Promise((resolve, reject) => {
                    db.query(query, [user_id, weapon_id, weapon_count], (err, result) => {
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
    console.log('Received request:', req.body);
    
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
        db.query(`SELECT weapon_count FROM user_item_weapon WHERE user_id = ? AND weapon_id = ?`, [userId, weaponId], (err, rows) => {
            if (err) {
                console.error('Error querying database:', err);
                return;
            }
            if (rows.length > 0) {
                db.query(`UPDATE user_item_weapon SET weapon_count = weapon_count + ? WHERE user_id = ? AND weapon_id = ?`, [weaponCount, userId, weaponId], err => {
                    if (err) {
                        console.error('Error updating database:', err);
                    }
                });
            } else {
                db.query(`INSERT INTO user_item_weapon (user_id, weapon_id, weapon_count) VALUES (?, ?, ?)`, [userId, weaponId, weaponCount], err => {
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
