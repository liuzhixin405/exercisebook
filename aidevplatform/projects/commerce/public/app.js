'use strict';
const root = document.getElementById('app');
const modal = document.getElementById('modal');
const modalContent = document.getElementById('modal-content');
const imageChoices = [
    ['photo-1505740420928-5e560c06d30e', '耳机'], ['photo-1507473885765-e6ed057f782c', '台灯'],
    ['photo-1523275335684-37898b6baf30', '腕表'], ['photo-1602143407151-7111542de6e8', '水杯'],
    ['photo-1548036328-c9fa89d128fa', '手袋'], ['photo-1503602642458-232111445657', '椅子'],
    ['photo-1608043152269-423dbba4e7e1', '音箱'], ['photo-1495474472287-4d71bcdd2085', '咖啡']
];
const labels = { pending: '待付款', paid: '待发货', shipped: '待收货', completed: '已完成', cancelled: '已取消', 'refund-requested': '退款审核中', refunded: '已退款' };
const state = { user: null, csrf: '', products: [], categories: [], cart: { items: [], subtotal: 0, shipping: 0, total: 0 },
    category: '', query: '', sort: 'featured', orders: [], adminProducts: [], adminOrders: [], orderFilter: '',
    adminOrderFilter: '', adminQuery: '', authMode: 'login', afterLogin: null, checkoutKey: null };
let navigation = 0;
let toastTimer;
let confirmAction;

function escape(value) { return String(value ?? '').replace(/[&<>"']/g, character => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' })[character]); }
const money = value => new Intl.NumberFormat('zh-CN', { style: 'currency', currency: 'CNY' }).format((value || 0) / 100);
const icon = name => `<i data-lucide="${name}" aria-hidden="true"></i>`;
const image = id => `/images/${encodeURIComponent(id)}.jpg`;
const date = value => new Date(value).toLocaleString('zh-CN', { month: '2-digit', day: '2-digit', hour: '2-digit', minute: '2-digit' });
const badge = status => `<span class="status-badge status-${escape(status)}">${escape(labels[status] || status)}</span>`;
function icons() { lucide.createIcons({ attrs: { 'stroke-width': 1.6 } }); }
function toast(message) {
    const element = document.getElementById('toast'); element.textContent = message; element.hidden = false;
    clearTimeout(toastTimer); toastTimer = setTimeout(() => { element.hidden = true; }, 4500);
}
async function api(path, { method = 'GET', data } = {}) {
    const response = await fetch(`/api${path}`, { method, credentials: 'same-origin',
        headers: { 'Content-Type': 'application/json', 'X-CSRF-Token': state.csrf }, body: data === undefined ? undefined : JSON.stringify(data) });
    const result = await response.json();
    if (!response.ok) { const error = new Error(result.error || '操作失败，请稍后重试'); error.status = response.status; throw error; }
    if (result.csrf) state.csrf = result.csrf;
    return result;
}
function guestItems() {
    try {
        const saved = JSON.parse(localStorage.getItem('mori-guest-cart') || '[]');
        return Array.isArray(saved) ? saved.filter(item => Number.isInteger(item.productId) && item.productId > 0 && Number.isInteger(item.quantity) && item.quantity > 0 && item.quantity <= 20)
            .filter((item, index, items) => items.findIndex(other => other.productId === item.productId) === index).slice(0, 50) : [];
    } catch { return []; }
}
function saveGuest(items) { localStorage.setItem('mori-guest-cart', JSON.stringify(items)); }
async function loadCart() {
    if (state.user) state.cart = await api('/cart');
    else {
        const items = guestItems().map(item => ({ ...state.products.find(product => product.id === item.productId), quantity: item.quantity })).filter(item => item.id);
        const subtotal = items.reduce((sum, item) => sum + item.price * item.quantity, 0);
        const shipping = items.length && subtotal < 29900 ? 1200 : 0;
        state.cart = { items, subtotal, shipping, total: subtotal + shipping };
    }
    updateHeader();
}
function updateHeader() {
    document.getElementById('account-label').textContent = state.user?.name || '登录';
    document.getElementById('account-button').title = state.user ? '账户与退出登录' : '登录或注册';
    const count = state.cart.items.reduce((sum, item) => sum + item.quantity, 0);
    const counter = document.getElementById('cart-count'); counter.textContent = count; counter.hidden = !count;
    const route = location.hash.slice(1) || '/';
    document.querySelectorAll('[data-nav]').forEach(link => link.classList.toggle('active', link.dataset.nav === (route.startsWith('/orders') ? 'orders' : route === '/' || route.startsWith('/product') ? 'shop' : '')));
}
async function changeQuantity(id, quantity) {
    if (state.user) state.cart = await api('/cart', { method: 'PUT', data: { productId: id, quantity } });
    else {
        const items = guestItems().filter(item => item.productId !== id);
        const product = state.products.find(item => item.id === id);
        if (quantity > 0) {
            if (!product || quantity > product.stock || quantity > 20) throw new Error('数量超过库存或单件限购 20 件');
            items.push({ productId: id, quantity });
        }
        saveGuest(items); await loadCart();
    }
    state.checkoutKey = null; updateHeader();
}
function empty(title, description, link = '#/', button = '去逛商店', name = 'shopping-bag') {
    return `<div class="empty-state">${icon(name)}<h2>${escape(title)}</h2><p>${escape(description)}</p><a class="button primary" href="${link}">${button}${icon('arrow-right')}</a></div>`;
}
function pageHeading(eyebrow, title, trailing = '') {
    return `<header class="page-heading"><div><span class="eyebrow">${eyebrow}</span><h1>${title}</h1></div>${trailing}</header>`;
}
function totals(button = '') {
    return `<aside class="order-summary"><h2>订单摘要</h2><dl><div><dt>商品合计</dt><dd>${money(state.cart.subtotal)}</dd></div><div><dt>配送运费</dt><dd>${state.cart.shipping ? money(state.cart.shipping) : '免运费'}</dd></div></dl>
        ${state.cart.shipping ? `<p class="muted small">再购 ${money(29900 - state.cart.subtotal)} 即可包邮</p>` : ''}
        <div class="summary-total"><span>应付金额</span><strong>${money(state.cart.total)}</strong></div>${button}<p class="payment-note">${icon('shield-check')}演示交易，不产生真实扣款</p></aside>`;
}
function productCard(product) {
    return `<article class="product-card"><a class="product-photo" href="#/product/${product.id}"><img src="${image(product.image)}" alt="${escape(product.name)}" loading="lazy" width="600" height="600">${product.stock <= 5 ? `<span class="photo-label">${product.stock ? '仅剩 ' + product.stock + ' 件' : '暂时售罄'}</span>` : ''}</a>
        <div class="product-meta"><span>${escape(product.category)}</span><span>精选 · ${String(product.id).padStart(2, '0')}</span></div><a class="product-name" href="#/product/${product.id}">${escape(product.name)}</a><div class="product-bottom"><strong>${money(product.price)}</strong><button class="icon-button add-button" data-action="add" data-id="${product.id}" ${!product.stock ? 'disabled' : ''} title="加入购物车" aria-label="将${escape(product.name)}加入购物车">${icon('plus')}</button></div></article>`;
}
function drawCatalog() {
    const container = document.getElementById('catalog-grid'); if (!container) return;
    let products = state.products.filter(product => (!state.category || product.category === state.category) && `${product.name}${product.description}`.toLowerCase().includes(state.query.toLowerCase()));
    if (state.sort === 'price-asc') products.sort((first, second) => first.price - second.price);
    if (state.sort === 'price-desc') products.sort((first, second) => second.price - first.price);
    if (state.sort === 'newest') products.sort((first, second) => second.id - first.id);
    container.innerHTML = products.length ? products.map(productCard).join('') : empty('没有找到商品', '试试其他关键词或分类', '#/', '重置筛选', 'search');
    document.getElementById('product-count').textContent = `${products.length} 件好物`;
    document.querySelectorAll('[data-category]').forEach(button => { const selected = button.dataset.category === state.category; button.classList.toggle('selected', selected); button.setAttribute('aria-selected', selected); });
    icons();
}
function catalog() {
    return `<section class="shop-intro"><div><span class="eyebrow">THE EVERYDAY COLLECTION / 2026</span><h1>MORI 生活商店<span>日常，自有好物。</span></h1><p>从居家到出行，为生活留一点喜欢。</p></div><div class="intro-note"><span>01 — 08</span><p>生活里的小选择<br>也是自己的风格</p></div></section>
        <section class="catalog"><div class="catalog-toolbar"><div class="category-tabs" role="tablist" aria-label="商品分类">${['', ...state.categories].map(category => `<button role="tab" data-action="category" data-category="${escape(category)}" aria-selected="${category === state.category}">${escape(category || '全部商品')}</button>`).join('')}</div><div class="catalog-controls"><label class="search-field">${icon('search')}<input id="catalog-search" type="search" maxlength="100" placeholder="搜索心仪好物" aria-label="搜索商品" value="${escape(state.query)}"></label><select id="catalog-sort" aria-label="商品排序">${[['featured', '精选排序'], ['price-asc', '价格从低到高'], ['price-desc', '价格从高到低'], ['newest', '最新上架']].map(([value, text]) => `<option value="${value}" ${state.sort === value ? 'selected' : ''}>${text}</option>`).join('')}</select></div></div><div class="catalog-caption"><span id="product-count"></span><span>少一点将就，多一点喜欢</span></div><div id="catalog-grid" class="product-grid"></div></section>`;
}
function productDetail(product) {
    return `<a class="back-link" href="#/">${icon('arrow-left')}返回商店</a><section class="product-detail"><div class="detail-photo"><img src="${image(product.image, 1000)}" alt="${escape(product.name)}" width="1000" height="1000"></div><div class="detail-info"><span class="eyebrow">${escape(product.category)} / MORI SELECT</span><h1>${escape(product.name)}</h1><div class="detail-price">${money(product.price)}</div><p class="detail-description">${escape(product.description)}</p><div class="detail-stock">${icon('package')} ${product.stock ? `现货 ${product.stock} 件` : '暂时售罄'}</div><div class="purchase-controls"><label class="quantity-label">数量<input type="number" id="detail-quantity" min="1" max="${Math.min(20, product.stock) || 1}" value="1" aria-label="购买数量"></label><button class="button primary" data-action="add" data-id="${product.id}" data-detail="true" ${!product.stock ? 'disabled' : ''}>${icon('shopping-bag')}加入购物车</button></div><dl class="detail-benefits"><div><dt>${icon('truck')}配送</dt><dd>满 ¥299 包邮，未满运费 ¥12</dd></div><div><dt>${icon('rotate-ccw')}售后</dt><dd>未付款可取消，已付款未发货可申请退款</dd></div><div><dt>${icon('credit-card')}付款</dt><dd>本店使用演示支付，不会真实扣款</dd></div></dl></div></section>`;
}
function cartPage() {
    if (!state.cart.items.length) return pageHeading('YOUR BAG', '购物车') + empty('购物车还是空的', '去发现下一件喜欢的好物。');
    return pageHeading('YOUR BAG', `购物车 <span class="heading-count">${state.cart.items.length}</span>`, '<a href="#/" class="text-link">继续购物 ' + icon('arrow-right') + '</a>') +
        `<div class="checkout-layout"><section class="cart-items">${state.cart.items.map(item => `<article class="cart-row"><a href="#/product/${item.id}"><img src="${image(item.image, 240)}" width="100" height="100" alt="${escape(item.name)}"></a><div class="cart-item-info"><span class="muted small">${escape(item.category)}</span><a href="#/product/${item.id}">${escape(item.name)}</a><span>${money(item.price)}</span>${!item.active || item.stock < item.quantity ? '<span class="error-text small">库存已变化，请调整数量或移除</span>' : ''}</div><div class="quantity-stepper"><button data-action="quantity" data-id="${item.id}" data-quantity="${item.quantity - 1}" title="减少数量" aria-label="减少${escape(item.name)}数量">${icon('minus')}</button><span>${item.quantity}</span><button data-action="quantity" data-id="${item.id}" data-quantity="${item.quantity + 1}" ${item.quantity >= Math.min(item.stock, 20) ? 'disabled' : ''} title="增加数量" aria-label="增加${escape(item.name)}数量">${icon('plus')}</button></div><strong class="line-total">${money(item.price * item.quantity)}</strong><button class="icon-button" data-action="quantity" data-id="${item.id}" data-quantity="0" title="移除商品" aria-label="移除${escape(item.name)}">${icon('trash-2')}</button></article>`).join('')}</section>${totals('<a class="button primary full-width" href="#/checkout">前往结算' + icon('arrow-right') + '</a>')}</div>`;
}
function checkoutPage() {
    state.checkoutKey ||= crypto.randomUUID();
    if (!state.cart.items.length) return empty('没有待结算的商品', '先将商品加入购物车。');
    return pageHeading('CHECKOUT', '确认订单', '<a href="#/cart" class="text-link">返回购物车</a>') + `<div class="checkout-layout"><form id="checkout-form" data-form="checkout" class="checkout-form"><h2><span class="step-number">01</span>收货信息</h2><div class="form-row"><label>收货人<input name="name" required minlength="2" maxlength="40" autocomplete="name" value="${escape(state.user.name)}"></label><label>联系电话<input name="phone" type="tel" required minlength="7" maxlength="20" autocomplete="tel" placeholder="用于接收配送信息"></label></div><label>完整收货地址<textarea name="address" required minlength="6" maxlength="300" rows="3" autocomplete="street-address" placeholder="省 / 市 / 区 / 街道门牌号"></textarea></label><label>订单备注 <span class="muted">（选填）</span><input name="note" maxlength="500" placeholder="其他需要我们知道的事情"></label><h2 class="section-heading"><span class="step-number">02</span>确认商品</h2>${state.cart.items.map(item => `<div class="checkout-item"><img src="${image(item.image, 160)}" alt="${escape(item.name)}" width="56" height="56"><div>${escape(item.name)}<span class="muted small">数量 ${item.quantity}</span></div><strong>${money(item.price * item.quantity)}</strong></div>`).join('')}<p class="checkout-notice">${icon('info')}提交后将生成待付款订单，下一步可选择演示付款。</p><p class="form-error" role="alert" hidden></p></form>${totals('<button class="button primary full-width" type="submit" form="checkout-form">提交订单' + icon('arrow-right') + '</button>')}</div>`;
}
function orderButtons(order, admin = false) {
    if (admin) return order.status === 'paid' ? `<button class="button small-button primary" data-action="ship" data-id="${order.id}">${icon('truck')}发货</button>` : order.status === 'refund-requested' ? `<button class="button small-button" data-action="order" data-id="${order.id}" data-operation="reject-refund" data-admin="true">拒绝退款</button><button class="button small-button primary" data-action="order" data-id="${order.id}" data-operation="refund" data-admin="true">同意退款</button>` : '';
    const buttons = { pending: [['cancel', '取消订单', 'x', ''], ['pay', '演示付款', 'credit-card', 'primary']], paid: [['request-refund', '申请退款', 'rotate-ccw', '']], shipped: [['complete', '确认收货', 'check', 'primary']] }[order.status] || [];
    return buttons.map(([action, text, glyph, primary]) => `<button class="button small-button ${primary}" data-action="order" data-id="${order.id}" data-operation="${action}">${icon(glyph)}${text}</button>`).join('');
}
function ordersPage() {
    const orders = state.orders.filter(order => !state.orderFilter || order.status === state.orderFilter);
    return pageHeading('MY ORDERS', '我的订单', '<a href="#/" class="text-link">继续购物 ' + icon('arrow-right') + '</a>') +
        `<div class="filter-tabs" role="tablist" aria-label="订单状态">${[['', '全部'], ['pending', '待付款'], ['paid', '待发货'], ['shipped', '待收货'], ['completed', '已完成'], ['refund-requested', '退款审核'], ['refunded', '已退款'], ['cancelled', '已取消']].map(([value, text]) => `<button role="tab" aria-selected="${state.orderFilter === value}" class="${state.orderFilter === value ? 'selected' : ''}" data-action="order-filter" data-filter="${value}">${text}</button>`).join('')}</div>` +
        (orders.length ? `<div class="orders-list">${orders.map(order => `<article class="order-card"><header><div><strong>${escape(order.number)}</strong><time>${date(order.created_at)}</time></div>${badge(order.status)}</header><div class="order-lines">${order.items.map(item => `<div class="order-line"><img src="${image(item.image, 160)}" width="64" height="64" alt="${escape(item.name)}"><span>${escape(item.name)}<small>${money(item.price)} × ${item.quantity}</small></span><strong>${money(item.price * item.quantity)}</strong></div>`).join('')}</div><div class="order-extra"><details><summary>收货信息${icon('chevron-down')}</summary><p>${escape(order.name)} · ${escape(order.phone)}</p><p>${escape(order.address)}</p>${order.note ? `<p>备注：${escape(order.note)}</p>` : ''}</details>${order.tracking ? `<span class="tracking">${icon('truck')}物流单号 ${escape(order.tracking)}</span>` : ''}</div><footer><span>含运费 ${money(order.shipping)} · 合计 <strong>${money(order.total)}</strong></span><div class="button-row">${orderButtons(order)}</div></footer></article>`).join('')}</div>` : empty('暂无相关订单', '喜欢的好物，随时等你带回家。'));
}
function adminShell(route, content) {
    return `<div class="admin-layout"><aside class="admin-sidebar"><span class="eyebrow">MORI BUSINESS</span><h2>商家工作台</h2><nav class="admin-nav" aria-label="商家导航">${[['/admin', '经营概览', 'layout-dashboard'], ['/admin/products', '商品管理', 'package'], ['/admin/orders', '订单管理', 'receipt-text']].map(([path, name, glyph]) => `<a href="#${path}" class="${path === route ? 'active' : ''}">${icon(glyph)}${name}</a>`).join('')}<a href="#/">${icon('arrow-left')}返回商城</a></nav><div class="admin-identity">${icon('shield-check')}商家权限</div></aside><div class="admin-content">${content}</div></div>`;
}
function adminOrdersTable(orders, compact = false) {
    return `<div class="table-scroll"><table><thead><tr><th>订单 / 下单时间</th><th>收货人</th><th>金额</th><th>状态</th><th class="align-right">操作</th></tr></thead><tbody>${orders.map(order => `<tr><td><strong class="table-order">${escape(order.number)}</strong><span class="cell-secondary">${date(order.created_at)} · ${order.items.reduce((sum, item) => sum + item.quantity, 0)} 件商品</span></td><td>${escape(order.name)}<span class="cell-secondary">${escape(order.phone)}</span></td><td class="nowrap">${money(order.total)}</td><td>${badge(order.status)}</td><td><div class="table-actions">${!compact ? orderButtons(order, true) : ''}<button class="icon-button" data-action="order-detail" data-id="${order.id}" title="查看订单" aria-label="查看订单${escape(order.number)}">${icon('eye')}</button></div></td></tr>`).join('') || '<tr><td colspan="5" class="table-empty">暂无订单</td></tr>'}</tbody></table></div>`;
}
function dashboard(overview) {
    return pageHeading('STORE OVERVIEW', '经营概览', `<span class="muted small">${new Date().toLocaleDateString('zh-CN')}</span>`) +
        `<div class="metric-grid">${[['演示成交额', money(overview.revenue), 'wallet'], ['累计订单', overview.orders, 'receipt-text'], ['待发货', overview.awaitingShipment || 0, 'truck'], ['在售商品', overview.products, 'package']].map(([name, value, glyph]) => `<div class="metric"><div><span>${name}</span>${icon(glyph)}</div><strong>${value}</strong></div>`).join('')}</div>
        <section class="admin-section"><div class="section-bar"><h2>最近订单</h2><a href="#/admin/orders" class="text-link">全部订单 ${icon('arrow-right')}</a></div>${adminOrdersTable(state.adminOrders.slice(0, 6), true)}</section><section class="admin-section"><div class="section-bar"><h2>库存提醒 <span class="heading-count">${overview.lowStock.length}</span></h2><a href="#/admin/products" class="text-link">管理商品 ${icon('arrow-right')}</a></div>${overview.lowStock.length ? overview.lowStock.map(product => `<div class="stock-alert"><img src="${image(product.image, 120)}" width="44" height="44" alt="${escape(product.name)}"><span>${escape(product.name)}</span><span class="stock-warning">剩余 ${product.stock} 件</span><button class="icon-button" data-action="edit-product" data-id="${product.id}" title="编辑库存" aria-label="编辑${escape(product.name)}库存">${icon('square-pen')}</button></div>`).join('') : '<p class="muted">目前库存充足。</p>'}</section>`;
}
function adminProducts() {
    return pageHeading('PRODUCTS', '商品管理', `<button class="button primary" data-action="new-product">${icon('plus')}新增商品</button>`) +
        `<div class="section-bar"><span class="muted">${state.adminProducts.length} 件商品 · ${state.adminProducts.filter(product => product.active).length} 件在售</span><label class="search-field">${icon('search')}<input id="admin-product-search" type="search" placeholder="搜索商品" aria-label="搜索后台商品"></label></div><div class="table-scroll"><table><thead><tr><th>商品</th><th>分类</th><th>售价</th><th>库存</th><th>状态</th><th class="align-right">操作</th></tr></thead><tbody id="admin-product-rows">${productRows(state.adminProducts)}</tbody></table></div>`;
}
function productRows(products) {
    return products.map(product => `<tr><td><div class="table-product"><img src="${image(product.image, 160)}" width="48" height="48" alt="${escape(product.name)}"><div><strong>${escape(product.name)}</strong><span class="cell-secondary">SKU ${String(product.id).padStart(4, '0')}</span></div></div></td><td>${escape(product.category)}</td><td class="nowrap">${money(product.price)}</td><td><span class="${product.stock <= 5 ? 'stock-warning' : ''}">${product.stock}</span></td><td><span class="status-badge ${product.active ? 'status-paid' : 'status-cancelled'}">${product.active ? '在售' : '已下架'}</span></td><td><div class="table-actions"><button class="icon-button" data-action="edit-product" data-id="${product.id}" title="编辑商品" aria-label="编辑${escape(product.name)}">${icon('square-pen')}</button><button class="icon-button" data-action="toggle-product" data-id="${product.id}" title="${product.active ? '下架' : '上架'}商品" aria-label="${product.active ? '下架' : '上架'}${escape(product.name)}">${icon(product.active ? 'eye-off' : 'eye')}</button></div></td></tr>`).join('') || '<tr><td colspan="6" class="table-empty">没有匹配商品</td></tr>';
}
function adminOrders() {
    const orders = state.adminOrders.filter(order => (!state.adminOrderFilter || order.status === state.adminOrderFilter) && `${order.number}${order.name}${order.phone}`.toLowerCase().includes(state.adminQuery.toLowerCase()));
    return pageHeading('ORDERS', '订单管理', `<span class="muted small">最近 ${state.adminOrders.length} 笔订单</span>`) +
        `<div class="section-bar order-filters"><select id="admin-order-filter" aria-label="后台订单状态"><option value="">全部状态</option>${Object.entries(labels).map(([value, name]) => `<option value="${value}" ${state.adminOrderFilter === value ? 'selected' : ''}>${name}</option>`).join('')}</select><label class="search-field">${icon('search')}<input id="admin-order-search" type="search" placeholder="订单号 / 收货人" aria-label="搜索后台订单" value="${escape(state.adminQuery)}"></label></div><div id="admin-orders-table">${adminOrdersTable(orders)}</div>`;
}

async function render() {
    const ticket = ++navigation; const route = location.hash.slice(1) || '/';
    updateHeader(); document.title = `${route.startsWith('/admin') ? '商家工作台' : route === '/cart' ? '购物车' : route === '/orders' ? '我的订单' : 'MORI 生活商店'} · MORI`;
    root.classList.toggle('admin-page', route.startsWith('/admin'));
    try {
        let content;
        if (route.startsWith('/admin')) {
            if (!state.user) {
                root.innerHTML = empty('登录商家账号', '请使用有商家权限的账号登录。', '#/', '返回商店', 'shield-check');
                openAuth(() => render()); icons(); return;
            }
            if (state.user.role !== 'admin') content = empty('需要商家权限', '当前账号是顾客账号，请切换为商家账号。', '#/', '返回商店', 'shield-alert');
            else {
                const [products, orders] = await Promise.all([api('/admin/products'), api('/admin/orders')]);
                state.adminProducts = products.items; state.adminOrders = orders.orders;
                content = adminShell(route, route === '/admin/products' ? adminProducts() : route === '/admin/orders' ? adminOrders() : dashboard(await api('/admin/overview')));
            }
        } else if (route.startsWith('/product/')) content = productDetail((await api(`/products/${Number(route.split('/')[2])}`)).product);
        else if (route === '/cart') { await loadCart(); content = cartPage(); }
        else if (route === '/checkout') {
            await loadCart();
            if (!state.user) { root.innerHTML = cartPage(); icons(); openAuth(() => render()); return; }
            content = checkoutPage();
        } else if (route === '/orders') {
            if (!state.user) { root.innerHTML = empty('登录查看订单', '登录后查看你的购买记录。', '#/', '返回商店', 'receipt-text'); icons(); openAuth(() => render()); return; }
            state.orders = (await api('/orders')).orders; content = ordersPage();
        } else content = catalog();
        if (ticket !== navigation) return;
        root.innerHTML = content; if (route === '/' || !['/cart', '/checkout', '/orders'].includes(route) && !route.startsWith('/admin') && !route.startsWith('/product/')) drawCatalog(); icons();
    } catch (error) {
        if (ticket !== navigation) return;
        root.innerHTML = `<div class="empty-state">${icon('circle-alert')}<h2>暂时无法打开</h2><p>${escape(error.message)}</p><button class="button" data-action="retry">${icon('rotate-cw')}重试</button><a href="#/" class="text-link">返回商店</a></div>`; icons();
    }
}

function showModal(content) { modalContent.innerHTML = content; if (!modal.open) modal.showModal(); icons(); }
function closeModal() { modal.close(); modalContent.innerHTML = ''; confirmAction = null; }
const modalHeader = title => `<header class="modal-header"><h2 id="modal-title">${title}</h2><button type="button" class="icon-button" data-action="close-modal" aria-label="关闭" title="关闭">${icon('x')}</button></header>`;
function openAuth(after = null, mode = 'login') {
    state.afterLogin = after; state.authMode = mode;
    showModal(`${modalHeader(mode === 'login' ? '欢迎回到 MORI' : '开启你的 MORI 日常')}<div class="auth-tabs"><button class="${mode === 'login' ? 'selected' : ''}" data-action="auth-mode" data-mode="login">登录</button><button class="${mode === 'register' ? 'selected' : ''}" data-action="auth-mode" data-mode="register">注册</button></div><form data-form="auth" class="modal-form">${mode === 'register' ? '<label>昵称<input name="name" required minlength="2" maxlength="40" autocomplete="name" placeholder="你的称呼"></label>' : ''}<label>邮箱<input name="email" type="email" required maxlength="120" autocomplete="email" placeholder="you@example.com"></label><label>密码<input name="password" type="password" required minlength="8" maxlength="72" autocomplete="${mode === 'login' ? 'current-password' : 'new-password'}" placeholder="至少 8 个字符"></label><p class="form-error" role="alert" hidden></p><button class="button primary full-width" type="submit">${mode === 'login' ? '登录' : '创建账号'}${icon('arrow-right')}</button></form>`);
}
function confirmDialog(title, message, action, label = '确认') {
    confirmAction = action;
    showModal(`${modalHeader(title)}<p class="modal-message">${escape(message)}</p><div class="modal-actions"><button class="button" data-action="close-modal">再想想</button><button class="button primary" data-action="confirm">${escape(label)}</button></div>`);
}
function productInput(product) {
    return { name: product.name, category: product.category, description: product.description, price: product.price,
        stock: product.stock, image: product.image, active: Boolean(product.active), version: product.version };
}
function productEditor(product = null) {
    showModal(`${modalHeader(product ? '编辑商品' : '新增商品')}<form class="modal-form" data-form="product" data-id="${product?.id || ''}" data-version="${product?.version || ''}"><label>商品名称<input name="name" required minlength="2" maxlength="80" value="${escape(product?.name || '')}"></label><div class="form-row"><label>分类<input name="category" list="product-categories" required maxlength="40" value="${escape(product?.category || '居家生活')}"><datalist id="product-categories"><option value="居家生活"><option value="数码配件"><option value="出行随身"></datalist></label><label>商品状态<select name="active"><option value="true" ${!product || product.active ? 'selected' : ''}>在售</option><option value="false" ${product && !product.active ? 'selected' : ''}>下架</option></select></label></div><div class="form-row"><label>售价（元）<input name="price" type="number" min="0.01" max="1000000" step="0.01" required value="${product ? (product.price / 100).toFixed(2) : ''}"></label><label>库存<input name="stock" type="number" min="0" max="1000000" step="1" required value="${product?.stock ?? 10}"></label></div><label>商品描述<textarea name="description" rows="3" minlength="5" maxlength="2000" required>${escape(product?.description || '')}</textarea></label><label>商品图片<select name="image">${imageChoices.map(([id, name]) => `<option value="${id}" ${product?.image === id ? 'selected' : ''}>${name}</option>`).join('')}</select></label><p class="form-error" role="alert" hidden></p><button class="button primary full-width" type="submit">${icon('check')}保存商品</button></form>`);
}
async function busy(button, action) {
    if (button?.disabled) return;
    if (button) button.disabled = true;
    try { await action(); }
    catch (error) { toast(error.message); }
    finally { if (button?.isConnected) button.disabled = false; }
}

document.addEventListener('click', async event => {
    const button = event.target.closest('[data-action]'); if (!button) return;
    const action = button.dataset.action;
    if (action === 'close-modal') return closeModal();
    if (action === 'account') {
        if (!state.user) return openAuth();
        return showModal(`${modalHeader('我的账户')}<div class="account-detail">${icon('user-round')}<h3>${escape(state.user.name)}</h3><p>${escape(state.user.email)}</p><span class="status-badge status-paid">${state.user.role === 'admin' ? '商家账号' : '顾客账号'}</span></div><div class="modal-actions"><button class="button" data-action="logout">${icon('log-out')}退出登录</button><a href="#/${state.user.role === 'admin' ? 'admin' : 'orders'}" class="button primary" data-action="close-modal">${state.user.role === 'admin' ? '进入后台' : '我的订单'}</a></div>`);
    }
    if (action === 'auth-mode') return openAuth(state.afterLogin, button.dataset.mode);
    if (action === 'category') { state.category = button.dataset.category; drawCatalog(); return; }
    if (action === 'order-filter') { state.orderFilter = button.dataset.filter; root.innerHTML = ordersPage(); icons(); return; }
    if (action === 'new-product') return productEditor();
    if (action === 'edit-product') return productEditor(state.adminProducts.find(product => product.id === Number(button.dataset.id)));
    if (action === 'ship') return showModal(`${modalHeader('订单发货')}<form class="modal-form" data-form="ship" data-id="${button.dataset.id}"><label>物流公司及单号<input name="tracking" required minlength="3" maxlength="80" placeholder="例如：顺丰 SF123456789"></label><p class="form-error" role="alert" hidden></p><button class="button primary full-width">${icon('truck')}确认发货</button></form>`);
    if (action === 'order-detail') {
        const order = state.adminOrders.find(item => item.id === button.dataset.id);
        return showModal(`${modalHeader('订单详情')}${badge(order.status)}<p class="muted small">${escape(order.number)}</p><div class="order-lines">${order.items.map(item => `<div class="order-line"><img src="${image(item.image, 160)}" alt="${escape(item.name)}" width="56" height="56"><span>${escape(item.name)}<small>数量 ${item.quantity}</small></span><strong>${money(item.price * item.quantity)}</strong></div>`).join('')}</div><p>${escape(order.name)} · ${escape(order.phone)}</p><p>${escape(order.address)}</p>${order.note ? `<p>备注：${escape(order.note)}</p>` : ''}${order.tracking ? `<p>物流：${escape(order.tracking)}</p>` : ''}<p class="summary-total">合计<strong>${money(order.total)}</strong></p><div class="modal-actions">${orderButtons(order, true)}</div>`);
    }
    if (action === 'toggle-product') {
        const product = state.adminProducts.find(item => item.id === Number(button.dataset.id));
        return confirmDialog(product.active ? '下架商品' : '上架商品', `${product.name} 将${product.active ? '停止销售，已有订单不受影响' : '重新对顾客展示'}。`, async () => {
            await api(`/admin/products/${product.id}`, { method: 'PUT', data: { ...productInput(product), active: !product.active } });
            closeModal(); await refreshProducts(); await render(); toast('商品状态已更新');
        });
    }
    if (action === 'order') {
        const operation = button.dataset.operation; const admin = button.dataset.admin === 'true';
        const messages = { pay: ['演示付款', '本次为演示支付，不会扣款。确认后订单进入待发货状态。'], cancel: ['取消订单', '取消后将释放库存，此操作不可撤销。'], complete: ['确认收货', '请确认已经收到商品。'], 'request-refund': ['申请退款', '申请后由商家审核，审核通过后恢复库存；本系统不会发起真实资金退款。'], refund: ['同意退款', '确认通过退款申请并恢复商品库存？不会发生真实资金变动。'], 'reject-refund': ['拒绝退款', '订单将恢复为待发货状态。'] };
        return confirmDialog(...messages[operation], async () => {
            await api(`${admin ? '/admin' : ''}/orders/${button.dataset.id}/actions`, { method: 'POST', data: { action: operation } });
            closeModal(); await refreshProducts(); await loadCart(); await render(); toast('订单状态已更新');
        }, operation === 'pay' ? '确认演示付款' : '确认');
    }
    await busy(button, async () => {
        if (action === 'confirm') { const execute = confirmAction; if (execute) await execute(); }
        if (action === 'retry') await render();
        if (action === 'add') {
            const id = Number(button.dataset.id); const existing = state.cart.items.find(item => item.id === id)?.quantity || 0;
            const extra = button.dataset.detail ? Number(document.getElementById('detail-quantity').value) : 1;
            if (!Number.isInteger(extra) || extra < 1 || existing + extra > 20) throw new Error('每件商品最多选购 20 件');
            await changeQuantity(id, existing + extra); toast('已加入购物车');
        }
        if (action === 'quantity') { await changeQuantity(Number(button.dataset.id), Number(button.dataset.quantity)); await render(); }
        if (action === 'logout') {
            await api('/auth/logout', { method: 'POST', data: {} });
            state.user = null; state.csrf = (await api('/session')).csrf; closeModal(); await loadCart();
            if (location.hash === '#/') await render(); else location.hash = '#/'; toast('已退出登录');
        }
    });
});
document.addEventListener('input', event => {
    if (event.target.id === 'catalog-search') { state.query = event.target.value; drawCatalog(); }
    if (event.target.id === 'admin-product-search') {
        document.getElementById('admin-product-rows').innerHTML = productRows(state.adminProducts.filter(product => product.name.includes(event.target.value))); icons();
    }
    if (event.target.id === 'admin-order-search') { state.adminQuery = event.target.value; drawAdminOrders(); }
});
document.addEventListener('change', event => {
    if (event.target.id === 'catalog-sort') { state.sort = event.target.value; drawCatalog(); }
    if (event.target.id === 'admin-order-filter') { state.adminOrderFilter = event.target.value; drawAdminOrders(); }
});
function drawAdminOrders() {
    const items = state.adminOrders.filter(order => (!state.adminOrderFilter || order.status === state.adminOrderFilter) && `${order.number}${order.name}${order.phone}`.toLowerCase().includes(state.adminQuery.toLowerCase()));
    document.getElementById('admin-orders-table').innerHTML = adminOrdersTable(items); icons();
}
async function refreshProducts() {
    const catalog = await api('/products'); state.products = catalog.items; state.categories = catalog.categories;
}
document.addEventListener('submit', async event => {
    const form = event.target.closest('[data-form]'); if (!form) return;
    event.preventDefault(); if (form.dataset.busy) return;
    const data = Object.fromEntries(new FormData(form));
    const buttons = [...form.querySelectorAll('button[type="submit"],button:not([type])'), ...document.querySelectorAll(`button[form="${form.id || 'unused'}"]`)];
    const errorElement = form.querySelector('.form-error'); errorElement.hidden = true;
    form.dataset.busy = 'true'; buttons.forEach(button => { button.disabled = true; });
    try {
        if (form.dataset.form === 'auth') {
            const result = await api(`/auth/${state.authMode}`, { method: 'POST', data }); state.user = result.user;
            await loadCart(); const guest = guestItems();
            for (const item of guest) {
                try {
                    const quantity = (state.cart.items.find(product => product.id === item.productId)?.quantity || 0) + item.quantity;
                    await changeQuantity(item.productId, quantity);
                    saveGuest(guestItems().filter(product => product.productId !== item.productId));
                } catch (error) { toast(`部分访客商品未合并：${error.message}`); }
            }
            const after = state.afterLogin; state.afterLogin = null; closeModal(); updateHeader();
            if (after) await after(); else await render();
            toast('登录成功');
        }
        if (form.dataset.form === 'checkout') {
            await api('/checkout', { method: 'POST', data: { ...data, key: state.checkoutKey } });
            state.checkoutKey = null; await refreshProducts(); await loadCart(); location.hash = '#/orders'; toast('订单已提交，请完成演示付款');
        }
        if (form.dataset.form === 'ship') {
            await api(`/admin/orders/${form.dataset.id}/actions`, { method: 'POST', data: { action: 'ship', tracking: data.tracking } });
            closeModal(); await render(); toast('已发货，顾客可以查看物流单号');
        }
        if (form.dataset.form === 'product') {
            const payload = { ...data, price: Math.round(Number(data.price) * 100), stock: Number(data.stock), active: data.active === 'true' };
            if (form.dataset.id) payload.version = Number(form.dataset.version);
            await api(`/admin/products${form.dataset.id ? '/' + form.dataset.id : ''}`, { method: form.dataset.id ? 'PUT' : 'POST', data: payload });
            closeModal(); await refreshProducts(); await render(); toast('商品已保存');
        }
    } catch (error) { if (errorElement.isConnected) { errorElement.textContent = error.message; errorElement.hidden = false; } else toast(error.message); }
    finally { delete form.dataset.busy; buttons.forEach(button => { if (button.isConnected) button.disabled = false; }); }
});
modal.addEventListener('click', event => { if (event.target === modal) closeModal(); });
window.addEventListener('hashchange', () => { if (modal.open) closeModal(); window.scrollTo({ top: 0 }); render(); });
(async () => {
    try {
        const session = await api('/session'); state.user = session.user;
        await refreshProducts(); await loadCart(); await render();
    } catch (error) {
        root.innerHTML = `<div class="empty-state"><h1>商店暂时未能加载</h1><p>${escape(error.message)}</p><button class="button" data-action="retry">重试</button></div>`;
    }
})();