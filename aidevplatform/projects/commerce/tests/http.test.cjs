'use strict';
const { test } = require('node:test');
const assert = require('node:assert/strict');
const { once } = require('node:events');
const { randomUUID } = require('node:crypto');
const { createApp } = require('../server.cjs');

async function fixture(action) {
    const { app, store } = createApp({ database: ':memory:' });
    const server = app.listen(0, '127.0.0.1'); await once(server, 'listening');
    const base = `http://127.0.0.1:${server.address().port}`;
    function client() {
        let cookie = ''; let csrf = '';
        return async (path, body, method = body ? 'POST' : 'GET', headers = {}) => {
            const response = await fetch(base + path, { method, headers: { Cookie: cookie, 'Content-Type': 'application/json', 'X-CSRF-Token': csrf, ...headers },
                body: body === undefined ? undefined : JSON.stringify(body) });
            if (response.headers.get('set-cookie')) cookie = response.headers.get('set-cookie').split(';')[0];
            const result = await response.json(); if (result.csrf) csrf = result.csrf;
            return { status: response.status, body: result, headers: response.headers };
        };
    }
    try { await action({ client, store, base }); }
    finally { server.closeAllConnections(); await new Promise(resolveClose => server.close(resolveClose)); store.close(); }
}
async function login(call, admin = false) {
    await call('/api/session');
    return call('/api/auth/login', { email: admin ? 'admin@mori.local' : 'buyer@mori.local', password: admin ? 'Admin123!' : 'Shopper123!' });
}
const delivery = () => ({ key: randomUUID(), name: '测试顾客', phone: '13800000000', address: '测试地址六号，不实际配送', note: '' });

test('storefront, scripts, stylesheet, icons and favicon are served under the local CSP', async () => fixture(async ({ base }) => {
    for (const path of ['/', '/app.js', '/style.css', '/vendor/lucide.js', '/favicon.svg']) {
        const response = await fetch(base + path);
        assert.equal(response.status, 200, path);
        assert.ok(response.headers.get('content-security-policy'));
        const text = await response.text(); assert.ok(text.length > 100, path);
        if (path === '/') { assert.match(text, /href="\/favicon.svg"/); assert.doesNotMatch(text, /href="data:/); }
    }
    const catalog = await (await fetch(base + '/api/products')).json();
    for (const product of catalog.items) {
        const response = await fetch(`${base}/images/${product.image}.jpg`);
        assert.equal(response.status, 200, product.name);
        assert.match(response.headers.get('content-type'), /image\/jpeg/);
        assert.ok((await response.arrayBuffer()).byteLength > 1000);
    }
}));

test('catalog is public, cookies are HttpOnly and unsafe requests require CSRF', async () => fixture(async ({ client, base }) => {
    const call = client(); const session = await call('/api/session');
    assert.equal(session.body.user, null); assert.match(session.headers.get('set-cookie'), /HttpOnly/);
    assert.match(session.headers.get('set-cookie'), /SameSite=Strict/);
    assert.equal((await call('/api/products')).body.items.length, 8);
    assert.equal((await call('/api/cart')).status, 401);
    const unsafe = await fetch(base + '/api/auth/login', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: '{}' });
    assert.equal(unsafe.status, 403);
    assert.equal((await call('/api/auth/login', {}, 'POST', { Origin: 'https://foreign.example' })).status, 403);
}));

test('register logs in a customer, forbids role injection, normalizes email and rejects duplicates', async () => fixture(async ({ client, store }) => {
    const call = client(); await call('/api/session');
    const input = { name: '新顾客', email: 'NEW@EXAMPLE.COM', password: 'Secure123!' };
    assert.equal((await call('/api/auth/register', { ...input, role: 'admin' })).status, 400);
    const created = await call('/api/auth/register', input);
    assert.equal(created.status, 201); assert.equal(created.body.user.role, 'customer');
    assert.equal(created.body.user.email, 'new@example.com'); assert.equal(created.body.user.password, undefined);
    assert.notEqual(store.database.prepare('SELECT password FROM users WHERE email=?').get('new@example.com').password, input.password);
    assert.equal((await call('/api/auth/register', input)).status, 409);
    assert.equal((await call('/api/auth/logout', {})).status, 200);
    assert.equal((await call('/api/cart')).status, 401);
}));

test('customers cannot read or write the administration APIs', async () => fixture(async ({ client }) => {
    const call = client(); assert.equal((await login(call)).status, 200);
    for (const route of ['/api/admin/overview', '/api/admin/products', '/api/admin/orders']) assert.equal((await call(route)).status, 403);
    assert.equal((await call('/api/admin/products', {})).status, 403);
    assert.equal((await call('/api/auth/login', { email: 'buyer@mori.local', password: 'wrongpass' })).status, 401);
}));

test('real HTTP purchase survives repeated checkout and moves through demo payment, shipment and receipt', async () => fixture(async ({ client, store }) => {
    const buyer = client(); const admin = client(); await login(buyer); await login(admin, true);
    assert.equal((await buyer('/api/cart', { productId: 1, quantity: 1 }, 'PUT')).status, 200);
    const input = delivery();
    assert.equal((await buyer('/api/checkout', { ...input, total: 1 })).status, 400);
    const created = await buyer('/api/checkout', input); assert.equal(created.status, 201);
    const order = created.body.order; assert.equal(order.total, 32900);
    assert.equal((await buyer('/api/checkout', input)).body.order.id, order.id);
    assert.equal(store.product(1).stock, 23);
    assert.equal((await buyer(`/api/orders/${order.id}/actions`, { action: 'ship' })).status, 400);
    assert.equal((await buyer(`/api/orders/${order.id}/actions`, { action: 'pay' })).body.order.status, 'paid');
    assert.equal((await admin('/api/admin/orders')).body.orders[0].number, order.number);
    assert.equal((await admin(`/api/admin/orders/${order.id}/actions`, { action: 'ship', tracking: 'SF-DEMO-12345' })).body.order.status, 'shipped');
    assert.equal((await buyer(`/api/orders/${order.id}/actions`, { action: 'complete' })).body.order.status, 'completed');
}));

test('another customer cannot discover or mutate a private order', async () => fixture(async ({ client }) => {
    const buyer = client(); await login(buyer); await buyer('/api/cart', { productId: 4, quantity: 1 }, 'PUT');
    const order = (await buyer('/api/checkout', delivery())).body.order;
    const other = client(); await other('/api/session'); await other('/api/auth/register', { name: '另一顾客', email: 'other@example.com', password: 'Secure123!' });
    assert.equal((await other('/api/orders')).body.orders.length, 0);
    assert.equal((await other(`/api/orders/${order.id}/actions`, { action: 'cancel' })).status, 404);
}));

test('concurrent buyers cannot oversell the last stock and failed checkout retains its cart', async () => fixture(async ({ client, store }) => {
    const first = client(); const second = client(); await login(first);
    await second('/api/session'); await second('/api/auth/register', { name: '并发顾客', email: 'concurrent@example.com', password: 'Secure123!' });
    await first('/api/cart', { productId: 7, quantity: 4 }, 'PUT');
    await second('/api/cart', { productId: 7, quantity: 4 }, 'PUT');
    const results = await Promise.all([first('/api/checkout', delivery()), second('/api/checkout', delivery())]);
    assert.deepEqual(results.map(result => result.status).sort(), [201, 409]);
    assert.equal(store.product(7).stock, 0);
    assert.equal(store.database.prepare('SELECT COUNT(*) AS count FROM orders').get().count, 1);
    const remaining = await Promise.all([first('/api/cart'), second('/api/cart')]);
    assert.deepEqual(remaining.map(result => result.body.items.length).sort(), [0, 1]);
}));

test('administrator creates, edits and archives a product with stale-write protection', async () => fixture(async ({ client }) => {
    const call = client(); await login(call, true);
    const input = { name: '测试新品', category: '居家生活', description: '这是一个用于真实接口验收的测试商品', price: 10000, stock: 3,
        image: 'photo-1503602642458-232111445657', active: true };
    const created = await call('/api/admin/products', input); assert.equal(created.status, 201);
    const product = created.body.product;
    assert.equal((await call(`/api/admin/products/${product.id}`, { ...input, stock: 5, version: product.version }, 'PUT')).body.product.stock, 5);
    assert.equal((await call(`/api/admin/products/${product.id}`, { ...input, version: product.version }, 'PUT')).status, 409);
    await call(`/api/admin/products/${product.id}`, { ...input, active: false, version: product.version + 1 }, 'PUT');
    assert.equal((await call(`/api/products/${product.id}`)).status, 404);
}));

test('malformed inputs and unknown routes fail without exposing stack traces or SQL', async () => fixture(async ({ client }) => {
    const call = client(); await login(call);
    for (const input of [{ productId: 1, quantity: -1 }, { productId: 1, quantity: 1, price: 1 }, { productId: '1', quantity: 1 }])
        assert.equal((await call('/api/cart', input, 'PUT')).status, 400);
    const unknown = await call('/api/not-a-route'); assert.equal(unknown.status, 404);
    assert.doesNotMatch(JSON.stringify(unknown.body), /SELECT|stack|node_modules/);
    assert.equal((await call('/api/products?sort=untrusted')).status, 400);
}));