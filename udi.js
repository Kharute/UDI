const express = require('express');
const bodyParser = require('body-parser');
const fs = require('fs').promises;
const xml2js = require('xml2js');
const mysql = require('mysql2/promise');
const path = require('path');
const winston = require('winston');

const app = express();
const ip = '15.165.15.237';
const port = 7777;

// Winston 로거 설정
const logger = winston.createLogger({
    level: 'info',
    format: winston.format.combine(
        winston.format.timestamp(),
        winston.format.json()
    ),
    transports: [
        new winston.transports.Console(), // 콘솔 출력 추가
        new winston.transports.File({ filename: 'error.log', level: 'error' }),
        new winston.transports.File({ filename: 'combined.log' })
    ]
});

// MySQL 연결 설정
const dbConfig = {
    host: 'localhost',
    user: 'root',
    password: '1234',
    database: 'UDI',
    connectionLimit: 10,
    connectTimeout: 10000, // 10초
};

let dbConnect;

async function connectToDatabase() {
    try {
        dbConnect = await mysql.createPool(dbConfig);
        
        logger.info('MySQL connected...');
    } catch (err) {
        logger.error('Error connecting to MySQL:', err);
        setTimeout(connectToDatabase, 5000); // 5초 후 재연결 시도
    }
}

app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: true }));

const xmlFilePath = path.join(__dirname, 'WeaponInfo.xml');
let weaponList = [];

async function loadXMLFile() {
    try {
        const data = await fs.readFile(xmlFilePath);
        const result = await xml2js.parseStringPromise(data, { explicitArray: false });
        weaponList = result.WeaponInfo.dataCategory.data.map(w => ({
            id: parseInt(w.$.weapon_id),
            name: w.$.WeaponName
        }));
        logger.info('Loaded weaponList:', weaponList);
    } catch (err) {
        logger.error('Error loading XML file:', err);
        setTimeout(loadXMLFile, 5000); // 5초 후 재시도
    }
}

loadXMLFile();
connectToDatabase();

// 로그인 처리
app.post('/login', async (req, res) => {
    const { username, password } = req.body;

    if (!username || !password) {
        return res.status(400).json({ success: false, message: 'Please provide username and password' });
    }

    let db;
    try {
        db = await dbConnect.getConnection();
        const [results] = await db.query('SELECT user_id, login_count_month, login_isfirst, login_date, login_time FROM user_login WHERE user_name = ? AND password = ?', [username, password]);

        if (results.length === 0) {
            return res.json({ success: false, message: 'Invalid username or password' });
        }

        const userId = results[0].user_id;
        let { login_count_month: loginCountMonth, login_isfirst: loginIsFirst, login_date: lastLoginDate } = results[0];
        const currentTime = new Date();

        await db.beginTransaction();

        try {
            await db.query('UPDATE user_login SET login_time = ?, login_date = ? WHERE user_id = ?', [currentTime, currentTime, userId]);

            if (lastLoginDate && (lastLoginDate.getMonth() !== currentTime.getMonth() || lastLoginDate.getFullYear() !== currentTime.getFullYear())) {
                loginCountMonth = 0;
            }

            const currentHour = currentTime.getHours();
            if (currentHour >= 6 && (!lastLoginDate || lastLoginDate.toDateString() !== currentTime.toDateString())) {
                [{ affectedRows }] = await db.query('UPDATE user_login SET login_count_month = ?, login_isfirst = 1 WHERE user_id = ?', [loginCountMonth + 1, userId]);
                if (affectedRows > 0) {
                    loginCountMonth += 1;
                    loginIsFirst = true;
                }
            }

            const [userDetails] = await db.query('SELECT * FROM user_details WHERE user_id = ?', [userId]);
            if (userDetails.length === 0) {
                await db.query('INSERT INTO user_details (user_id, nickname, level, experience, skill_point, inventory) VALUES (?, "NONAME", 1, 0, 0, "")', [userId]);
            }

            const [userGoods] = await db.query('SELECT * FROM user_item_goods WHERE user_id = ?', [userId]);
            if (userGoods.length === 0) {
                await db.query('INSERT INTO user_item_goods (user_id, gold, jewel, ticket_weapon, ticket_armor) VALUES (?, 0, 0, 0, 0)', [userId]);
            }

            const [finalUserDetails] = await db.query('SELECT user_id, nickname, level, experience, skill_point, inventory FROM user_details WHERE user_id = ?', [userId]);
            const [finalUserGoods] = await db.query('SELECT * FROM user_item_goods WHERE user_id = ?', [userId]);

            await db.commit();

            res.json({
                success: true,
                message: 'Login successful',
                userDetails: finalUserDetails[0],
                userGoods: finalUserGoods[0],
                loginCountMonth,
                loginIsFirst
            });
        } catch (error) {
            await db.rollback();
            throw error;
        }
    } catch (error) {
        logger.error('Error in login route:', error);
        res.status(500).json({ success: false, message: 'Internal server error during login' });
    }
});

// 사용자 상세 정보 생성
app.post('/createUserDetails', async (req, res) => {
    const { userId } = req.body;

    let db;
    try {
        db = await dbConnect.getConnection();
        await db.beginTransaction();

        const [userDetails] = await db.query('SELECT * FROM user_details WHERE user_id = ?', [userId]);
        const [userGoods] = await db.query('SELECT * FROM user_item_goods WHERE user_id = ?', [userId]);

        if (userDetails.length === 0) {
            await db.query('INSERT INTO user_details (user_id, nickname, level, experience, inventory) VALUES (?, "NONAME", 1, 0, "")', [userId]);
        }

        if (userGoods.length === 0) {
            await db.query('INSERT INTO user_item_goods (user_id, gold, jewel, ticket_weapon, ticket_armor) VALUES (?, 100, 100, 1, 1)', [userId]);
        }

        await db.commit();
        res.json({ success: true, message: 'User details and item goods created or updated successfully' });
    } catch (error) {
        await db.rollback();
        logger.error('Error in createUserDetails:', error);
        res.status(500).json({ success: false, message: 'Internal server error' });
    }
});

// 사용자 상세 정보 업데이트
const detailsColumns = ['level', 'experience', 'skill_point'];

app.post('/updateUserDetails', async (req, res) => {
    const { userId, column, value } = req.body;

    if (!detailsColumns.includes(column)) {
        return res.status(400).json({ success: false, message: 'Invalid column name' });
    }
    
    let db;
    try {
        db = await dbConnect.getConnection();
        const [results] = await db.query('SELECT * FROM user_details WHERE user_id = ?', [userId]);
        if (results.length === 0) {
            return res.json({ success: false, message: 'User not found' });
        }

        const [result] = await db.query(`UPDATE user_details SET ${mysql.escapeId(column)} = ? WHERE user_id = ?`, [value, userId]);
        res.json({ success: true, message: 'Update successful', result });
    } catch (error) {
        logger.error('Error in updateUserDetails:', error);
        res.status(500).json({ success: false, message: 'Internal server error' });
    }
});

// 사용자 아이템 업데이트
const goodsColumns = ['gold', 'jewel', 'ticket_weapon', 'ticket_armor'];

app.post('/updateUserGoods', async (req, res) => {
    const { userId, column, value } = req.body;

    if (!goodsColumns.includes(column)) {
        return res.status(400).json({ success: false, message: 'Invalid column name' });
    }

    let db;
    try {
        db = await dbConnect.getConnection();
        const [results] = await db.query('SELECT * FROM user_item_goods WHERE user_id = ?', [userId]);
        if (results.length === 0) {
            return res.json({ success: false, message: 'User not found' });
        }

        const [result] = await db.query(`UPDATE user_item_goods SET ${mysql.escapeId(column)} = ? WHERE user_id = ?`, [value, userId]);
        res.json({ success: true, message: 'Update successful', result });
    } catch (error) {
        logger.error('Error in updateUserGoods:', error);
        res.status(500).json({ success: false, message: 'Internal server error' });
    }
});

// 무기 데이터 로드
app.post('/loadWeaponData', async (req, res) => {
    const { userId } = req.body;
    let db;
    try {
        db = await dbConnect.getConnection();
        const [results] = await db.query('SELECT weapon_id, weapon_count FROM user_item_weapon WHERE user_id = ?', [userId]);
        res.json(results);
    } catch (error) {
        logger.error('Error in loadWeaponData:', error);
        res.status(500).json({ success: false, message: 'Internal server error' });
    }
});

// 무기 데이터 업로드
app.post('/uploadWeaponData', async (req, res) => {
    const { user_id, weapons } = req.body;
    let db;
    try {
        db = await dbConnect.getConnection();
        await db.beginTransaction();

        for (const weapon of weapons) {
            const { weapon_id, weapon_count } = weapon;
            await db.query(`
                INSERT INTO user_item_weapon (user_id, weapon_id, weapon_count)
                VALUES (?, ?, ?)
                ON DUPLICATE KEY UPDATE weapon_count = VALUES(weapon_count)
            `, [user_id, weapon_id, weapon_count]);
        }

        await db.commit();
        res.json({ success: true, message: 'Data updated successfully' });
    } catch (error) {
        await db.rollback();
        logger.error('Error in uploadWeaponData:', error);
        res.status(500).json({ success: false, message: 'Internal server error' });
    }
});

// 가챠 시스템
app.post('/gacha', async (req, res) => {
    const { userId, count } = req.body;
    const result = {};
    let db;
    try {
        db = await dbConnect.getConnection();
        for (let i = 0; i < count; i++) {
            const randomWeapon = weaponList[Math.floor(Math.random() * weaponList.length)];
            result[randomWeapon.id] = (result[randomWeapon.id] || 0) + 1;
        }

        await db.beginTransaction();

        for (const [weaponId, weaponCount] of Object.entries(result)) {
            await db.query(`
                INSERT INTO user_item_weapon (user_id, weapon_id, weapon_count)
                VALUES (?, ?, ?)
                ON DUPLICATE KEY UPDATE weapon_count = weapon_count + ?
            `, [userId, weaponId, weaponCount, weaponCount]);
        }

        await db.commit();
        res.json({ success: true, result });
    } catch (error) {
        await db.rollback();
        logger.error('Error in gacha:', error);
        res.status(500).json({ success: false, message: 'Internal server error' });
    }
});

// 서버 시작
async function startServer() {
    try {
        await connectToDatabase();
        app.listen(port, () => {
            logger.info(`Server running on http://${ip}:${port}`);
        });
    } catch (err) {
        logger.error('Error starting server:', err);
        setTimeout(startServer, 5000); // 5초 후 서버 재시작 시도
    }
}

startServer();

// 예기치 않은 예외 처리
process.on('uncaughtException', (err) => {
    logger.error('Uncaught Exception:', err);
});

process.on('unhandledRejection', (reason, promise) => {
    logger.error('Unhandled Rejection at:', promise, 'reason:', reason);
});