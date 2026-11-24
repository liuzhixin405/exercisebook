// 没有框架、没有构建：浏览器只和 Console 的 /api 说话，从不直接碰 Admin（Admin key 在服务端）。
// 所有来自服务器的文字都用 textContent 放进页面，不拼 HTML，配合 CSP 杜绝 XSS。
const $ = (id) => document.getElementById(id);
let me = null;
let timer = null;

function el(tag, props = {}, ...children) {
  const node = document.createElement(tag);
  for (const [key, value] of Object.entries(props)) {
    if (key === 'class') node.className = value;
    else if (key.startsWith('on')) node.addEventListener(key.slice(2), value);
    else node[key] = value;
  }
  node.append(...children);
  return node;
}

function say(text, error = false) {
  $('message').textContent = text;
  $('message').className = error ? 'error' : '';
}

async function api(method, path, body) {
  const response = await window.fetch(`/api/${path}`, {
    method,
    headers: body === undefined ? {} : { 'Content-Type': 'application/json' },
    body: body === undefined ? undefined : JSON.stringify(body),
  });
  const text = await response.text();
  let data = null;
  try {
    data = text ? JSON.parse(text) : null;
  } catch {
    data = text;
  }

  if (response.status === 401 && path !== 'login') {
    showLogin();
    throw new Error('请先登录');
  }
  if (!response.ok) throw new Error(typeof data === 'string' && data ? data : `${response.status} ${response.statusText}`);
  return data;
}

// ---- 登录 / 页面切换 ----
function showLogin() {
  me = null;
  clearInterval(timer);
  for (const id of ['tabs', 'who', 'logout', 'sources', 'keys', 'health']) $(id).hidden = true;
  $('login').hidden = false;
}

async function enter(user) {
  me = user;
  $('login').hidden = true;
  $('who').textContent = `${me.name}（${me.role}）`;
  for (const id of ['tabs', 'who', 'logout']) $(id).hidden = false;
  document.querySelectorAll('.admin-only').forEach((node) => (node.hidden = me.role !== 'admin'));
  openTab('sources');
}

function openTab(name) {
  clearInterval(timer);
  say('');
  for (const id of ['sources', 'keys', 'health']) $(id).hidden = id !== name;
  document.querySelectorAll('#tabs button').forEach((b) => b.classList.toggle('active', b.dataset.tab === name));
  const load = { sources: loadSources, keys: loadKeys, health: loadHealth }[name];
  load().catch((e) => say(e.message, true));
  if (name === 'health') timer = setInterval(() => loadHealth().catch(() => {}), 3000);
}

// ---- 源 ----
async function loadSources() {
  const sources = await api('GET', 'sources');
  $('source-rows').replaceChildren(
    ...sources.map((s) =>
      el('tr', {},
        el('td', { class: s.retired ? 'retired' : '' }, s.name),
        el('td', {}, s.connector),
        el('td', { class: s.retired ? 'bad' : 'ok' }, s.retired ? '已退役' : '运行中'),
        el('td', {}, String(s.version)),
        el('td', {}, s.config.devices.join(', ')),
        el('td', {}, String(s.config.intervalMs)),
        el('td', {}, String(s.config.maxPerSecond)),
        el('td', {}, ...(me.role === 'admin' && !s.retired
          ? [el('button', { onclick: () => startEdit(s) }, '编辑'), ' ', el('button', { onclick: () => retire(s.name) }, '退役')]
          : [])),
      ),
    ),
  );
}

let editing = null;
function startEdit(source) {
  editing = source.name;
  $('edit-name').textContent = source.name;
  const form = $('edit-form');
  form.devices.value = source.config.devices.join(',');
  form.intervalMs.value = source.config.intervalMs;
  form.maxPerSecond.value = source.config.maxPerSecond;
  form.hidden = false;
}

async function retire(name) {
  if (!confirm(`退役 ${name}？退役后 Source 会停止发布，名字也不能再被注册。`)) return;
  await api('DELETE', `sources/${encodeURIComponent(name)}`);
  say(`${name} 已退役`);
  await loadSources();
}

// ---- API key ----
async function loadKeys() {
  const admin = me.role === 'admin';
  $('keys-denied').hidden = admin;
  $('keys-admin').hidden = !admin;
  if (!admin) return;

  const keys = await api('GET', 'keys');
  $('key-rows').replaceChildren(
    ...keys.map((k) =>
      el('tr', {},
        el('td', {}, k.id),
        el('td', {}, k.owner),
        el('td', { class: k.revoked ? 'bad' : 'ok' }, k.revoked ? '已吊销' : '有效'),
        el('td', {}, ...(k.revoked ? [] : [el('button', { onclick: () => revoke(k.id) }, '吊销')])),
      ),
    ),
  );
}

async function revoke(id) {
  if (!confirm(`吊销 ${id}？`)) return;
  await api('DELETE', `keys/${encodeURIComponent(id)}`);
  say(`${id} 已吊销`);
  await loadKeys();
}

// ---- 健康 ----
async function loadHealth() {
  const items = await api('GET', 'health');
  $('health-rows').replaceChildren(
    ...items.map((i) =>
      el('tr', {},
        el('td', {}, i.name),
        el('td', {}, i.url),
        el('td', { class: i.ready ? 'ok' : 'bad' }, i.ready ? '就绪' : '未就绪'),
        el('td', {}, i.detail),
      ),
    ),
  );
}

// ---- 事件 ----
const guarded = (fn) => async (event) => {
  event.preventDefault();
  try {
    await fn(event.target);
  } catch (e) {
    say(e.message, true);
  }
};

$('login-form').addEventListener('submit', guarded(async (form) => {
  try {
    await api('POST', 'login', { name: form.name.value, password: form.password.value });
  } catch {
    throw new Error('用户名或密码不对');
  }
  form.password.value = '';
  const user = await api('GET', 'me');
  await enter({ name: user.name, role: user.role });
}));

$('logout').addEventListener('click', async () => {
  await api('POST', 'logout');
  showLogin();
});
document.querySelectorAll('#tabs button').forEach((b) => b.addEventListener('click', () => openTab(b.dataset.tab)));

$('source-form').addEventListener('submit', guarded(async (form) => {
  await api('POST', 'sources', { name: form.name.value, connector: form.connector.value });
  say(`${form.name.value} 已注册`);
  form.name.value = '';
  await loadSources();
}));

$('edit-form').addEventListener('submit', guarded(async (form) => {
  await api('PUT', `sources/${encodeURIComponent(editing)}/config`, {
    devices: form.devices.value.split(',').map((d) => d.trim()).filter(Boolean),
    intervalMs: Number(form.intervalMs.value),
    maxPerSecond: Number(form.maxPerSecond.value),
  });
  say(`${editing} 的配置已保存`);
  form.hidden = true;
  await loadSources();
}));
$('edit-cancel').addEventListener('click', () => ($('edit-form').hidden = true));

$('key-form').addEventListener('submit', guarded(async (form) => {
  const created = await api('POST', 'keys', { owner: form.owner.value });
  form.owner.value = '';
  // 明文只此一次：关掉就再也看不到了
  $('new-key').textContent = `新 key（只显示这一次，请立刻保存）：${created.key}`;
  $('new-key').hidden = false;
  await loadKeys();
}));

// ---- 启动：已有登录 Cookie 就直接进，否则显示登录页 ----
window.fetch('/api/me').then(async (response) => {
  if (response.ok) {
    const user = await response.json();
    await enter({ name: user.name, role: user.role });
  } else {
    showLogin();
  }
});
