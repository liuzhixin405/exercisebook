'use strict';
const express = require('express');
const helmet = require('helmet');
const { rateLimit } = require('express-rate-limit');
const bcrypt = require('bcryptjs');
const cookie = require('cookie');
const { z, ZodError } = require('zod');
const { randomBytes, randomUUID, createHash } = require('node:crypto');
const { join, resolve } = require('node:path');
const { createStore, ShopError } = require('./store.cjs');

const publicUser = user => user ? { id: user.id, name: user.name, email: user.email, role: user.role } : null;
const hash = token => createHash('sha256').update(token).digest('hex');
const identifier = z.coerce.number().int().positive();
const password = z.string().min(8, '密码至少 8 个字符').refine(value => Buffer.byteLength(value) <= 72, '密码过长');
const loginSchema = z.object({ email: z.email().max(120).transform(value => value.toLowerCase()), password }).strict();
const productSchema = z.object({
    name: z.string().trim().min(2).max(80), category: z.string().trim().min(1).max(40),
    description: z.string().trim().min(5).max(2000), price: z.number().int().min(1).max(100000000),
    stock: z.number().int().min(0).max(1000000), image: z.string().regex(/^photo-\d{13}-[a-f0-9]{12}$/),
    active: z.boolean(), version: z.number().int().positive().optional()
}).strict();

function createApp({ database = join(__dirname, 'data', 'commerce.sqlite'), demoAccounts = true } = {}) {
    const store = createStore(database);
    if (demoAccounts) {
        for (const [email, name, secret, role] of [
            ['buyer@mori.local', '林小满', 'Shopper123!', 'customer'],
            ['admin@mori.local', 'MORI 店长', 'Admin123!', 'admin']
        ]) if (!store.database.prepare('SELECT id FROM users WHERE email=?').get(email))
            store.database.prepare('INSERT INTO users(id,email,name,password,role) VALUES(?,?,?,?,?)')
                .run(randomUUID(), email, name, bcrypt.hashSync(secret, 10), role);
    }
    const app = express();
    app.disable('x-powered-by');
    app.use(helmet({ contentSecurityPolicy: { directives: {
        defaultSrc: ["'self'"], scriptSrc: ["'self'"], styleSrc: ["'self'", 'https://fonts.googleapis.com'],
        fontSrc: ["'self'", 'https://fonts.gstatic.com'], imgSrc: ["'self'", 'https://images.unsplash.com'],
        connectSrc: ["'self'"], objectSrc: ["'none'"], frameAncestors: ["'none'"],
        upgradeInsecureRequests: null
    } }, strictTransportSecurity: false }));
    app.use((request, response, next) => {
        if (!/^(localhost|127\.0\.0\.1|\[::1\])(?::\d+)?$/.test(request.headers.host || ''))
            return response.status(403).json({ error: '此实例仅供本机访问' });
        if (request.headers.origin && request.headers.origin !== `http://${request.headers.host}`)
            return response.status(403).json({ error: '不允许跨站请求' });
        next();
    });
    app.use('/api', rateLimit({ windowMs: 60000, limit: 240, standardHeaders: 'draft-8', legacyHeaders: false,
        message: { error: '请求过于频繁，请稍后再试' } }));
    app.use('/api', (request, response, next) => {
        response.setHeader('Cache-Control', 'no-store');
        if (!['GET', 'HEAD'].includes(request.method) && !request.is('application/json'))
            return response.status(415).json({ error: '仅接受 JSON 请求' });
        next();
    });
    app.use(express.json({ limit: '32kb' }));

    function issueSession(response, userId = null) {
        const token = randomBytes(32).toString('hex'); const csrf = randomBytes(32).toString('hex');
        store.database.prepare('DELETE FROM sessions WHERE expires<?').run(Date.now());
        store.database.prepare('INSERT INTO sessions(hash,user_id,csrf,expires) VALUES(?,?,?,?)')
            .run(hash(token), userId, csrf, Date.now() + 7 * 86400000);
        response.cookie('mori_session', token, { httpOnly: true, sameSite: 'strict', maxAge: 7 * 86400000, path: '/' });
        return { hash: hash(token), user_id: userId, csrf };
    }
    app.use('/api', (request, response, next) => {
        const token = cookie.parse(request.headers.cookie || '').mori_session || '';
        request.session = token && store.database.prepare('SELECT * FROM sessions WHERE hash=? AND expires>?').get(hash(token), Date.now());
        if (!request.session) request.session = issueSession(response);
        request.user = request.session.user_id && store.database.prepare('SELECT id,email,name,role FROM users WHERE id=?').get(request.session.user_id);
        if (!['GET', 'HEAD'].includes(request.method) && request.headers['x-csrf-token'] !== request.session.csrf)
            return response.status(403).json({ error: '安全令牌已失效，请刷新后重试' });
        next();
    });
    const authenticated = (request, response, next) => request.user ? next() : response.status(401).json({ error: '请先登录' });
    const administrator = (request, response, next) => request.user.role === 'admin' ? next() : response.status(403).json({ error: '需要商家权限' });
    const rotate = (request, response, user) => {
        store.database.prepare('DELETE FROM sessions WHERE hash=?').run(request.session.hash);
        const session = issueSession(response, user.id);
        return { user: publicUser(user), csrf: session.csrf };
    };

    app.get('/api/session', (request, response) => response.json({ user: publicUser(request.user), csrf: request.session.csrf, paymentMode: 'demo' }));
    app.get('/api/products', (request, response) => {
        const input = z.object({ query: z.string().max(100).optional(), category: z.string().max(40).optional(),
            sort: z.enum(['featured', 'price-asc', 'price-desc', 'newest']).optional() }).strict().parse(request.query);
        response.json({ items: store.products(input), categories: store.database.prepare('SELECT DISTINCT category FROM products WHERE active=1 ORDER BY category').all().map(item => item.category) });
    });
    app.get('/api/products/:id', (request, response) => response.json({ product: store.product(identifier.parse(request.params.id)) }));
    const authLimit = rateLimit({ windowMs: 600000, limit: 20, standardHeaders: 'draft-8', legacyHeaders: false,
        message: { error: '登录尝试过多，请稍后再试' } });
    app.post('/api/auth/register', authLimit, (request, response) => {
        const input = loginSchema.extend({ name: z.string().trim().min(2, '昵称至少 2 个字符').max(40) }).parse(request.body);
        if (store.database.prepare('SELECT id FROM users WHERE email=?').get(input.email)) throw new ShopError(409, '该邮箱已注册');
        const user = { id: randomUUID(), name: input.name, email: input.email, role: 'customer' };
        store.database.prepare('INSERT INTO users(id,email,name,password,role) VALUES(?,?,?,?,?)')
            .run(user.id, user.email, user.name, bcrypt.hashSync(input.password, 10), user.role);
        response.status(201).json(rotate(request, response, user));
    });
    app.post('/api/auth/login', authLimit, (request, response) => {
        const input = loginSchema.parse(request.body);
        const user = store.database.prepare('SELECT * FROM users WHERE email=?').get(input.email);
        const valid = bcrypt.compareSync(input.password, user?.password || '$2b$10$0RP6lS6jfhD0.L6U63qZ0O0QpCsdSg9nO5oEz4QUdV4yj3l3mPzWu');
        if (!user || !valid) throw new ShopError(401, '邮箱或密码错误');
        response.json(rotate(request, response, user));
    });
    app.post('/api/auth/logout', (request, response) => {
        store.database.prepare('DELETE FROM sessions WHERE hash=?').run(request.session.hash);
        response.clearCookie('mori_session', { path: '/', httpOnly: true, sameSite: 'strict' });
        response.json({ ok: true });
    });
    app.get('/api/cart', authenticated, (request, response) => response.json(store.cart(request.user.id)));
    app.put('/api/cart', authenticated, (request, response) => {
        const input = z.object({ productId: z.number().int().positive(), quantity: z.number().int().min(0).max(20) }).strict().parse(request.body);
        response.json(store.setCart(request.user.id, input.productId, input.quantity));
    });
    app.post('/api/checkout', authenticated, (request, response) => {
        const input = z.object({ key: z.uuid(), name: z.string().trim().min(2).max(40),
            phone: z.string().trim().regex(/^[0-9+ ()-]{7,20}$/, '请填写有效联系电话'),
            address: z.string().trim().min(6, '请填写完整收货地址').max(300), note: z.string().trim().max(500).default('') }).strict().parse(request.body);
        response.status(201).json({ order: store.checkout(request.user.id, input) });
    });
    app.get('/api/orders', authenticated, (request, response) => response.json({ orders: store.orders(request.user.id) }));
    app.post('/api/orders/:id/actions', authenticated, (request, response) => {
        const input = z.object({ action: z.enum(['pay', 'cancel', 'complete', 'request-refund']) }).strict().parse(request.body);
        response.json({ order: store.transition(request.user.id, z.uuid().parse(request.params.id), input.action) });
    });
    app.use('/api/admin', authenticated, administrator);
    app.get('/api/admin/overview', (request, response) => response.json(store.statistics()));
    app.get('/api/admin/products', (request, response) => response.json({ items: store.products({ admin: true }) }));
    app.post('/api/admin/products', (request, response) => response.status(201).json({ product: store.saveProduct(null, productSchema.parse(request.body)) }));
    app.put('/api/admin/products/:id', (request, response) => {
        const input = productSchema.extend({ version: z.number().int().positive() }).parse(request.body);
        response.json({ product: store.saveProduct(identifier.parse(request.params.id), input) });
    });
    app.get('/api/admin/orders', (request, response) => response.json({ orders: store.orders(request.user.id, true) }));
    app.post('/api/admin/orders/:id/actions', (request, response) => {
        const input = z.object({ action: z.enum(['ship', 'refund', 'reject-refund']), tracking: z.string().trim().max(80).default('') }).strict().parse(request.body);
        response.json({ order: store.transition(request.user.id, z.uuid().parse(request.params.id), input.action, true, input.tracking) });
    });
    app.use('/api', (request, response) => response.status(404).json({ error: '接口不存在' }));
    app.get('/vendor/lucide.js', (request, response) => response.sendFile(require.resolve('lucide/dist/umd/lucide.js')));
    app.use(express.static(join(__dirname, 'public'), { etag: true, maxAge: 0 }));
    app.use((error, request, response, next) => {
        if (response.headersSent) return next(error);
        if (error instanceof ZodError) return response.status(400).json({ error: `输入无效：${error.issues[0]?.message || '请检查表单'}` });
        if (error instanceof ShopError) return response.status(error.status).json({ error: error.message });
        if (error.type === 'entity.too.large') return response.status(413).json({ error: '请求内容过大' });
        if (error instanceof SyntaxError && 'body' in error) return response.status(400).json({ error: 'JSON 格式错误' });
        console.error('Commerce request failed:', error.name);
        response.status(500).json({ error: '操作未完成，请刷新后重试' });
    });
    return { app, store };
}

if (require.main === module) {
    const port = Number(process.env.PORT || 54930);
    if (!Number.isInteger(port) || port < 1024 || port > 65535) throw new Error('Invalid PORT');
    const { app, store } = createApp({ database: resolve(process.env.MORI_DATABASE || join(__dirname, 'data', 'commerce.sqlite')),
        demoAccounts: process.env.MORI_DEMO_ACCOUNTS !== 'false' });
    const server = app.listen(port, '127.0.0.1', () => console.log(`MORI shop: http://127.0.0.1:${port} | SQLite persisted | DEMO payment only`));
    const close = () => server.close(() => { store.close(); process.exit(0); });
    process.on('SIGINT', close); process.on('SIGTERM', close);
}
module.exports = { createApp };