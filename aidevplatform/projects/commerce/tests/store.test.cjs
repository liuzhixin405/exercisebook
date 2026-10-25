'use strict';
const { test } = require('node:test');
const assert = require('node:assert/strict');
const { mkdtempSync, rmSync } = require('node:fs');
const { tmpdir } = require('node:os');
const { join } = require('node:path');
const { createStore } = require('../store.cjs');

function fixture() {
    const store = createStore();
    for (const id of ['buyer', 'other']) store.database.prepare('INSERT INTO users(id,email,name,password) VALUES(?,?,?,?)').run(id, `${id}@test.local`, id, 'unused-test-hash');
    return store;
}
const shipping = key => ({ key, name: '测试顾客', phone: '13800000000', address: '测试地址，不用于配送', note: '' });

test('catalog filters, searches, sorts and excludes inactive products', () => {
    const store = fixture();
    try {
        assert.equal(store.products().length, 8);
        assert.equal(store.products({ query: '无线' }).length, 1);
        assert.equal(store.products({ category: '居家生活' }).length, 3);
        assert.equal(store.products({ sort: 'price-asc' })[0].id, 4);
        store.saveProduct(1, { ...store.product(1), active: false });
        assert.equal(store.products().length, 7);
        assert.throws(() => store.product(1), error => error.status === 404);
    } finally { store.close(); }
});

test('checkout calculates server prices, ships free above threshold and is idempotent', () => {
    const store = fixture();
    try {
        store.setCart('buyer', 1, 2);
        const created = store.checkout('buyer', { ...shipping('same-key'), total: 1 });
        assert.equal(created.total, 65800); assert.equal(created.shipping, 0);
        assert.equal(store.product(1).stock, 22); assert.equal(store.cart('buyer').items.length, 0);
        assert.equal(store.checkout('buyer', shipping('same-key')).id, created.id);
        assert.equal(store.product(1).stock, 22); assert.equal(store.orders('buyer').length, 1);
        assert.throws(() => store.order('other', created.id), error => error.status === 404);
    } finally { store.close(); }
});

test('insufficient stock rolls back the entire order, inventory and cart', () => {
    const store = fixture();
    try {
        store.setCart('buyer', 1, 2); store.setCart('buyer', 7, 4);
        store.setCart('other', 7, 4); store.checkout('other', shipping('other'));
        assert.throws(() => store.checkout('buyer', shipping('buyer')), error => error.status === 409);
        assert.equal(store.product(1).stock, 24); assert.equal(store.orders('buyer').length, 0);
        assert.equal(store.cart('buyer').items.length, 2);
    } finally { store.close(); }
});

test('cancel restores inventory once and shipping below threshold is charged', () => {
    const store = fixture();
    try {
        store.setCart('buyer', 4, 1); const created = store.checkout('buyer', shipping('cancel'));
        assert.equal(created.total, 10100); assert.equal(created.shipping, 1200);
        assert.equal(store.transition('buyer', created.id, 'cancel').status, 'cancelled');
        assert.equal(store.product(4).stock, 36);
        assert.throws(() => store.transition('buyer', created.id, 'cancel'), error => error.status === 409);
        assert.throws(() => store.transition('buyer', created.id, 'pay'), error => error.status === 409);
        assert.equal(store.product(4).stock, 36);
    } finally { store.close(); }
});

test('payment, shipment and receipt enforce ownership, role and state transitions', () => {
    const store = fixture();
    try {
        store.setCart('buyer', 1, 1); const created = store.checkout('buyer', shipping('delivery'));
        assert.throws(() => store.transition('buyer', created.id, 'ship'), error => error.status === 409);
        assert.throws(() => store.transition('other', created.id, 'pay'), error => error.status === 404);
        store.transition('buyer', created.id, 'pay');
        assert.throws(() => store.transition('admin', created.id, 'ship', true), error => error.status === 400);
        store.transition('admin', created.id, 'ship', true, 'DEMO123456');
        const completed = store.transition('buyer', created.id, 'complete');
        assert.equal(completed.status, 'completed'); assert.equal(completed.tracking, 'DEMO123456');
        assert.equal(store.statistics().revenue, 32900);
    } finally { store.close(); }
});

test('refund requires customer request and administrator approval, restoring inventory once', () => {
    const store = fixture();
    try {
        store.setCart('buyer', 2, 1); const created = store.checkout('buyer', shipping('refund'));
        store.transition('buyer', created.id, 'pay');
        assert.throws(() => store.transition('buyer', created.id, 'refund'), error => error.status === 409);
        store.transition('buyer', created.id, 'request-refund');
        store.transition('admin', created.id, 'refund', true);
        assert.equal(store.product(2).stock, 18); assert.equal(store.statistics().revenue, 0);
        assert.throws(() => store.transition('admin', created.id, 'refund', true), error => error.status === 409);
    } finally { store.close(); }
});

test('archived products cannot be ordered and optimistic editing prevents inventory overwrites', () => {
    const store = fixture();
    try {
        const stale = store.product(1); store.setCart('buyer', 1, 1); store.checkout('buyer', shipping('version'));
        assert.throws(() => store.saveProduct(1, stale), error => error.status === 409);
        store.setCart('buyer', 2, 1); store.saveProduct(2, { ...store.product(2), active: false });
        assert.throws(() => store.checkout('buyer', shipping('archived')), error => error.status === 409);
        assert.throws(() => store.setCart('buyer', 3, -1), error => error.status === 400);
    } finally { store.close(); }
});

test('orders, customers and inventory survive closing and reopening the database', () => {
    const directory = mkdtempSync(join(tmpdir(), 'mori-store-')); const file = join(directory, 'shop.sqlite');
    let store = createStore(file);
    try {
        store.database.prepare('INSERT INTO users(id,email,name,password) VALUES(?,?,?,?)').run('buyer', 'persist@test.local', '顾客', 'test-hash');
        store.setCart('buyer', 3, 1); const created = store.checkout('buyer', shipping('persistent'));
        store.close(); store = createStore(file);
        assert.equal(store.order('buyer', created.id).number, created.number); assert.equal(store.product(3).stock, 11);
        assert.equal(store.products().length, 8);
    } finally { store.close(); rmSync(directory, { recursive: true, force: true }); }
});