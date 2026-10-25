'use strict';
const { DatabaseSync } = require('node:sqlite');
const { mkdirSync } = require('node:fs');
const { dirname } = require('node:path');
const { randomUUID, randomBytes } = require('node:crypto');

class ShopError extends Error {
    constructor(status, message) { super(message); this.status = status; }
}

function createStore(filename = ':memory:') {
    if (filename !== ':memory:') mkdirSync(dirname(filename), { recursive: true });
    const database = new DatabaseSync(filename);
    database.exec(`PRAGMA foreign_keys=ON; PRAGMA journal_mode=WAL; PRAGMA busy_timeout=5000;
        CREATE TABLE IF NOT EXISTS users (
            id TEXT PRIMARY KEY, email TEXT NOT NULL UNIQUE, name TEXT NOT NULL,
            password TEXT NOT NULL, role TEXT NOT NULL CHECK(role IN ('customer','admin')) DEFAULT 'customer');
        CREATE TABLE IF NOT EXISTS sessions (
            hash TEXT PRIMARY KEY, user_id TEXT REFERENCES users(id), csrf TEXT NOT NULL, expires INTEGER NOT NULL);
        CREATE TABLE IF NOT EXISTS products (
            id INTEGER PRIMARY KEY, name TEXT NOT NULL, category TEXT NOT NULL, description TEXT NOT NULL,
            price INTEGER NOT NULL CHECK(price BETWEEN 1 AND 100000000),
            stock INTEGER NOT NULL CHECK(stock BETWEEN 0 AND 1000000), image TEXT NOT NULL,
            active INTEGER NOT NULL DEFAULT 1 CHECK(active IN (0,1)), version INTEGER NOT NULL DEFAULT 1);
        CREATE TABLE IF NOT EXISTS cart (
            user_id TEXT NOT NULL REFERENCES users(id), product_id INTEGER NOT NULL REFERENCES products(id),
            quantity INTEGER NOT NULL CHECK(quantity BETWEEN 1 AND 20), PRIMARY KEY(user_id,product_id));
        CREATE TABLE IF NOT EXISTS orders (
            id TEXT PRIMARY KEY, number TEXT NOT NULL UNIQUE, user_id TEXT NOT NULL REFERENCES users(id),
            request_key TEXT NOT NULL, status TEXT NOT NULL CHECK(status IN
                ('pending','paid','shipped','completed','cancelled','refund-requested','refunded')),
            subtotal INTEGER NOT NULL, shipping INTEGER NOT NULL, total INTEGER NOT NULL,
            name TEXT NOT NULL, phone TEXT NOT NULL, address TEXT NOT NULL, note TEXT NOT NULL,
            tracking TEXT NOT NULL DEFAULT '', created_at TEXT NOT NULL,
            UNIQUE(user_id,request_key));
        CREATE TABLE IF NOT EXISTS order_items (
            order_id TEXT NOT NULL REFERENCES orders(id), product_id INTEGER NOT NULL REFERENCES products(id),
            name TEXT NOT NULL, image TEXT NOT NULL, price INTEGER NOT NULL, quantity INTEGER NOT NULL,
            PRIMARY KEY(order_id,product_id));`);

    const transaction = action => {
        database.exec('BEGIN IMMEDIATE');
        try { const result = action(); database.exec('COMMIT'); return result; }
        catch (error) { database.exec('ROLLBACK'); throw error; }
    };
    const seed = [
        ['轻量无线耳机', '数码配件', '柔软耳垫与轻量头梁，让音乐成为日常的留白。含收纳袋与充电线。', 32900, 24, 'photo-1505740420928-5e560c06d30e'],
        ['暖光阅读台灯', '居家生活', '柔和暖光，金属灯身。为书桌、床头与安静的夜晚留一盏灯。', 21900, 18, 'photo-1507473885765-e6ed057f782c'],
        ['简约石英腕表', '出行随身', '清晰表盘，舒适表带。日常防泼水，记录每个认真生活的时刻。', 45900, 12, 'photo-1523275335684-37898b6baf30'],
        ['轻巧随行水杯', '出行随身', '清透杯身，便携提环。适合通勤与轻运动，容量约 500mL。', 8900, 36, 'photo-1602143407151-7111542de6e8'],
        ['城市通勤手袋', '出行随身', '简洁轮廓与实用分区，装下日常随身物品。尺寸请以实物为准。', 26900, 16, 'photo-1548036328-c9fa89d128fa'],
        ['原木休闲椅', '居家生活', '温润木色与利落线条，适合餐桌与阅读角。单椅售卖，需简单组装。', 69900, 8, 'photo-1503602642458-232111445657'],
        ['便携蓝牙音箱', '数码配件', '小巧随身，蓝牙连接。室内聆听与短途出行都能轻松相伴。', 29900, 4, 'photo-1608043152269-423dbba4e7e1'],
        ['日常手冲咖啡组合', '居家生活', '一份属于自己的咖啡时光。含咖啡豆与手冲配件，适合日常入门。', 15900, 22, 'photo-1495474472287-4d71bcdd2085']
    ];
    transaction(() => {
        if (!database.prepare('SELECT COUNT(*) AS count FROM products').get().count) {
            const insert = database.prepare('INSERT INTO products(name,category,description,price,stock,image) VALUES(?,?,?,?,?,?)');
            for (const product of seed) insert.run(...product);
        }
        database.prepare('UPDATE products SET image=?,version=version+1 WHERE image=?')
            .run('photo-1505740420928-5e560c06d30e', 'photo-1505742675381-3190a3d95177');
    });

    function product(id, includeInactive = false) {
        const found = database.prepare('SELECT * FROM products WHERE id=?').get(id);
        if (!found || !found.active && !includeInactive) throw new ShopError(404, '商品不存在或已下架');
        return found;
    }
    function products({ query = '', category = '', sort = 'featured', admin = false } = {}) {
        const order = { featured: 'id ASC', 'price-asc': 'price ASC,id ASC', 'price-desc': 'price DESC,id ASC', newest: 'id DESC' }[sort] || 'id ASC';
        return database.prepare(`SELECT * FROM products WHERE (?=1 OR active=1)
            AND (?='' OR category=?) AND (?='' OR instr(lower(name||description),lower(?))>0) ORDER BY ${order}`)
            .all(Number(admin), category, category, query, query);
    }
    function cart(userId) {
        const items = database.prepare(`SELECT products.*,cart.quantity FROM cart JOIN products ON products.id=cart.product_id
            WHERE user_id=? ORDER BY products.id`).all(userId);
        const subtotal = items.reduce((total, item) => total + item.price * item.quantity, 0);
        const shipping = items.length && subtotal < 29900 ? 1200 : 0;
        return { items, subtotal, shipping, total: subtotal + shipping };
    }
    function setCart(userId, productId, quantity) {
        if (!Number.isInteger(quantity) || quantity < 0 || quantity > 20) throw new ShopError(400, '每件商品限购 20 件');
        if (!quantity) database.prepare('DELETE FROM cart WHERE user_id=? AND product_id=?').run(userId, productId);
        else {
            const item = product(productId);
            if (item.stock < quantity) throw new ShopError(409, '商品库存不足');
            if (cart(userId).items.length >= 50 && !cart(userId).items.some(entry => entry.id === productId)) throw new ShopError(400, '购物车商品种类已达上限');
            database.prepare(`INSERT INTO cart(user_id,product_id,quantity) VALUES(?,?,?)
                ON CONFLICT(user_id,product_id) DO UPDATE SET quantity=excluded.quantity`).run(userId, productId, quantity);
        }
        return cart(userId);
    }
    function order(userId, id, admin = false) {
        const found = database.prepare('SELECT * FROM orders WHERE id=? AND (?=1 OR user_id=?)').get(id, Number(admin), userId);
        if (!found) throw new ShopError(404, '订单不存在');
        return { ...found, items: database.prepare('SELECT * FROM order_items WHERE order_id=?').all(id) };
    }
    function orders(userId, admin = false) {
        return database.prepare('SELECT id FROM orders WHERE (?=1 OR user_id=?) ORDER BY created_at DESC,id DESC LIMIT 200')
            .all(Number(admin), userId).map(entry => order(userId, entry.id, admin));
    }
    function checkout(userId, input) {
        return transaction(() => {
            const existing = database.prepare('SELECT id FROM orders WHERE user_id=? AND request_key=?').get(userId, input.key);
            if (existing) return order(userId, existing.id);
            const basket = cart(userId);
            if (!basket.items.length) throw new ShopError(409, '购物车为空');
            for (const item of basket.items) {
                if (!item.active || item.stock < item.quantity) throw new ShopError(409, `${item.name} 库存不足或已下架，请调整购物车`);
                database.prepare('UPDATE products SET stock=stock-?,version=version+1 WHERE id=?').run(item.quantity, item.id);
            }
            const id = randomUUID();
            const number = `MO${new Date().toISOString().slice(0, 10).replaceAll('-', '')}${randomBytes(4).toString('hex').toUpperCase()}`;
            database.prepare(`INSERT INTO orders(id,number,user_id,request_key,status,subtotal,shipping,total,name,phone,address,note,created_at)
                VALUES(?,?,?,?,'pending',?,?,?,?,?,?,?,?)`).run(id, number, userId, input.key, basket.subtotal, basket.shipping,
                    basket.total, input.name, input.phone, input.address, input.note || '', new Date().toISOString());
            const insert = database.prepare('INSERT INTO order_items(order_id,product_id,name,image,price,quantity) VALUES(?,?,?,?,?,?)');
            for (const item of basket.items) insert.run(id, item.id, item.name, item.image, item.price, item.quantity);
            database.prepare('DELETE FROM cart WHERE user_id=?').run(userId);
            return order(userId, id);
        });
    }
    function transition(userId, id, action, admin = false, tracking = '') {
        return transaction(() => {
            const current = order(userId, id, admin);
            const rules = admin ? { ship: ['paid', 'shipped'], refund: ['refund-requested', 'refunded'], 'reject-refund': ['refund-requested', 'paid'] } :
                { pay: ['pending', 'paid'], cancel: ['pending', 'cancelled'], complete: ['shipped', 'completed'], 'request-refund': ['paid', 'refund-requested'] };
            const rule = rules[action];
            if (!rule || current.status !== rule[0]) throw new ShopError(409, '当前订单状态不支持此操作');
            if (action === 'ship' && !tracking.trim()) throw new ShopError(400, '请填写物流单号');
            if (action === 'cancel' || action === 'refund') {
                for (const item of current.items) database.prepare('UPDATE products SET stock=stock+?,version=version+1 WHERE id=?').run(item.quantity, item.product_id);
            }
            database.prepare('UPDATE orders SET status=?,tracking=? WHERE id=?').run(rule[1], action === 'ship' ? tracking : current.tracking, id);
            return order(userId, id, admin);
        });
    }
    function saveProduct(id, input) {
        if (!Number.isInteger(input.price) || input.price < 1 || input.price > 100000000 ||
            !Number.isInteger(input.stock) || input.stock < 0 || input.stock > 1000000) throw new ShopError(400, '价格或库存无效');
        if (!id) {
            const result = database.prepare('INSERT INTO products(name,category,description,price,stock,image,active) VALUES(?,?,?,?,?,?,?)')
                .run(input.name, input.category, input.description, input.price, input.stock, input.image, Number(input.active));
            return product(Number(result.lastInsertRowid), true);
        }
        const updated = database.prepare(`UPDATE products SET name=?,category=?,description=?,price=?,stock=?,image=?,active=?,version=version+1
            WHERE id=? AND version=?`).run(input.name, input.category, input.description, input.price, input.stock, input.image, Number(input.active), id, input.version);
        if (!updated.changes) throw new ShopError(409, '商品已被更新，请刷新后重试');
        return product(id, true);
    }
    function statistics() {
        return { ...database.prepare(`SELECT COUNT(*) AS orders,
            COALESCE(SUM(CASE WHEN status IN ('paid','shipped','completed') THEN total ELSE 0 END),0) AS revenue,
            SUM(CASE WHEN status='paid' THEN 1 ELSE 0 END) AS awaitingShipment,
            SUM(CASE WHEN status='refund-requested' THEN 1 ELSE 0 END) AS refunds FROM orders`).get(),
            products: database.prepare('SELECT COUNT(*) AS count FROM products WHERE active=1').get().count,
            lowStock: products({ admin: true }).filter(item => item.active && item.stock <= 5) };
    }
    return { database, product, products, cart, setCart, order, orders, checkout, transition, saveProduct, statistics,
        close: () => database.close() };
}
module.exports = { createStore, ShopError };