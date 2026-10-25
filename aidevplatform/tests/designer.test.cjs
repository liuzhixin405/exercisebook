const { test } = require('node:test');
const assert = require('node:assert/strict');
const { readFileSync } = require('node:fs');
const { join } = require('node:path');
const { runInNewContext } = require('node:vm');
const { webcrypto } = require('node:crypto');

function createDesigner() {
    const elements = new Map();
    const storage = new Map();
    const container = { dataset: { applicationId: 'app-a', pageId: 'page-a' } };
    const context = {
        crypto: webcrypto,
        document: {
            addEventListener() {},
            querySelector: () => container,
            getElementById: id => {
                if (!elements.has(id)) elements.set(id, { clientWidth: 600 });
                return elements.get(id);
            }
        },
        localStorage: {
            setItem: (key, value) => storage.set(key, value),
            getItem: key => storage.get(key) ?? null
        }
    };
    const source = readFileSync(join(__dirname, '../src/AiDevPlatform.Web/wwwroot/js/designer.js'), 'utf8');
    const Designer = runInNewContext(`${source}\nPageDesigner;`, context);
    const designer = Object.create(Designer.prototype);
    Object.assign(designer, { components: [], history: [], historyIndex: -1, renderAllComponents() {} });
    return { designer, elements, storage, container };
}

test('attribute values cannot break out of property inputs', () => {
    const { designer } = createDesigner();
    const component = designer.createComponent('text', 0, 0);
    component.properties.content = '\" autofocus onfocus=alert(1) <script>';
    const markup = designer.getComponentSpecificProperties(component);
    assert.ok(markup.includes('&quot; autofocus onfocus=alert(1) &lt;script&gt;'));
    assert.ok(!markup.includes('value="" autofocus'));
});

test('AI components have unique IDs and nonoverlapping deterministic geometry', () => {
    const { designer } = createDesigner();
    const components = designer.buildDraftComponents({ components: [
        { type: 'text', text: 'Name', inputType: 'text' },
        { type: 'input', text: 'Enter name', inputType: 'text' },
        { type: 'button', text: 'Save', inputType: 'text' }
    ] });
    assert.equal(new Set(components.map(component => component.id)).size, 3);
    assert.ok(components[1].y >= components[0].y + components[0].height);
    assert.ok(components[2].y >= components[1].y + components[1].height);
    assert.equal(components[1].properties.placeholder, 'Enter name');
    assert.ok(components.every(component => component.x + component.width <= 600));
});

test('undo returns to an empty canvas and redo restores the draft', () => {
    const { designer } = createDesigner();
    designer.saveToHistory();
    designer.components.push(designer.createComponent('input', 24, 24));
    designer.saveToHistory();
    designer.undo();
    assert.equal(designer.components.length, 0);
    designer.redo();
    assert.equal(designer.components.length, 1);
    designer.undo();
    designer.components.push(designer.createComponent('text', 24, 24));
    designer.saveToHistory();
    designer.redo();
    assert.equal(designer.components[0].type, 'text');
});

test('draft survives save and restore and is isolated by page ID', () => {
    const { designer, container } = createDesigner();
    designer.components.push(designer.createComponent('input', 24, 60));
    designer.savePage();
    designer.components = [];
    designer.loadPage();
    assert.equal(designer.components.length, 1);
    assert.equal(designer.components[0].y, 60);
    const firstKey = designer.storageKey();
    container.dataset.pageId = 'page-b';
    assert.notEqual(designer.storageKey(), firstKey);
});

test('corrupt draft is not rendered or overwritten', () => {
    const { designer, storage } = createDesigner();
    storage.set(designer.storageKey(), '{not json');
    designer.loadPage();
    assert.equal(designer.components.length, 0);
    assert.equal(storage.get(designer.storageKey()), '{not json');
});

test('stored scripts and arbitrary styles are not restored', () => {
    const { designer, storage } = createDesigner();
    storage.set(designer.storageKey(), JSON.stringify({ version: 1, components: [{
        type: 'text', x: 0, y: 0, width: 100, height: 50,
        properties: { content: '<script>alert(1)</script>', onclick: 'alert(1)' },
        styles: { backgroundImage: 'url(https://example.com/track)' }
    }] }));
    designer.loadPage();
    assert.equal(designer.components.length, 1);
    assert.equal(designer.components[0].properties.onclick, undefined);
    assert.equal(designer.components[0].styles.backgroundImage, undefined);
});

for (const mode of ['clarify', 'generate']) test(`development task actions reload server state even when the destination hash is unchanged: ${mode}`, async () => {
    let submit;
    let reloads = 0;
    let destination;
    const form = { dataset: { devForm: mode }, reportValidity: () => true,
        addEventListener: (name, handler) => { if (name === 'submit') submit = handler; } };
    const elements = { 'dev-error': {}, 'dev-progress': {},
        'dev-task-data': { value: JSON.stringify({ id: 'task-a', revision: 1, changes: [] }) } };
    const root = { querySelectorAll: selector => selector === '[data-dev-form]' ? [form] : [] };
    const location = { href: 'http://localhost/Development/Task/task-a#requirements', hash: '#requirements',
        reload: () => { reloads++; }, assign: () => assert.fail('hash navigation does not reload the task') };
    const source = readFileSync(join(__dirname, '../src/AiDevPlatform.Web/wwwroot/js/development.js'), 'utf8');
    runInNewContext(source, {
        document: { querySelector: () => root, getElementById: id => elements[id] }, location,
        history: { replaceState: (state, title, url) => { destination = url; } },
        URL, AbortController, FormData: class {
            get(name) { return ({ generationMode: 'repair', targetPath: 'main.cjs', feedback: 'Fix the reported defect' })[name] || ''; }
            has() { return false; }
        },
        setTimeout: () => 1, clearTimeout() {},
        fetch: async (url, request) => {
            assert.equal(url.searchParams.get('operation'), mode);
            assert.equal(JSON.parse(request.body).expectedRevision, 1);
            if (mode === 'generate') {
                assert.equal(JSON.parse(request.body).generationMode, 'repair');
                assert.equal(JSON.parse(request.body).targetPath, 'main.cjs');
                assert.equal(JSON.parse(request.body).feedback, 'Fix the reported defect');
            }
            return { ok: true, json: async () => ({ id: 'task-a', revision: 2 }) };
        }
    });
    await submit({ preventDefault() {} });
    assert.equal(destination, `/Development/Task/task-a#${mode === 'generate' ? 'changes' : 'requirements'}`);
    assert.equal(reloads, 1);
    assert.equal(elements['dev-error'].hidden, true);
});

test('verification UI blocks incomplete gates and displays historical reports as text', () => {
    for (const status of [null, 'not-run', 'failed', 'passed']) {
        const element = () => {
            const node = { children: [], handlers: {}, textContent: '', value: '',
                append(...items) { this.children.push(...items); },
                replaceChildren() { this.children = []; },
                setAttribute(name, value) { this[name] = value; },
                addEventListener(name, handler) { this.handlers[name] = handler; },
                get firstElementChild() { return this.children[0]; } };
            Object.defineProperty(node, 'innerHTML', { set() { assert.fail('report content must not be parsed as HTML'); } });
            return node;
        };
        const nodes = new Map();
        const get = id => { if (!nodes.has(id)) nodes.set(id, element()); return nodes.get(id); };
        const report = value => ({ runId: 'fixture-run', receivedAt: '2026-10-25T00:00:00Z', checks:
            ['build', 'test'].map(name => ({ name, status: value, detail: '<script>untrusted log</script>' })) });
        const change = verifications => ({ summary: 'Source version', files: [{ path: 'README.md', content: '<script>source</script>' }],
            checks: [{ name: 'Boundary', status: 'passed', detail: 'Valid' }], verifications });
        get('dev-task-data').value = JSON.stringify({ standard: { requiredChecks: ['build', 'test'] }, contextFiles: [],
            changes: [change([report('passed')]), change(status ? [report(status)] : [])] });
        get('dev-change-select').value = '1';
        const accepted = {};
        get('dev-review-form').querySelector = () => accepted;
        const source = readFileSync(join(__dirname, '../src/AiDevPlatform.Web/wwwroot/js/development.js'), 'utf8');
        runInNewContext(source, { document: { querySelector: () => ({ querySelectorAll: () => [] }), getElementById: get, createElement: element },
            location: { hash: 'checks' } });
        assert.equal(accepted.disabled, status !== 'passed');
        assert.equal(get('dev-source-after').textContent, '<script>source</script>');
        assert.equal(get('dev-checks').children.length, 3);
        if (status) {
            const log = get('dev-verification-history').children[1].children[1].children[0];
            assert.ok(log.textContent.includes('<script>untrusted log</script>'));
        }
        get('dev-change-select').value = '0';
        get('dev-change-select').handlers.change();
        assert.equal(get('dev-review-form').hidden, true);
        assert.equal(get('dev-verify-form').hidden, true);
        get('dev-change-select').value = '1';
        get('dev-change-select').handlers.change();
        assert.equal(get('dev-review-form').hidden, false);
        assert.equal(accepted.disabled, status !== 'passed');
    }
});

for (const failSecondBatch of [false, true]) test(`project UI chains saved revisions and ${failSecondBatch ? 'preserves completed batches on failure' : 'finishes on the project tab'}`, async () => {
    let submit;
    let reloads = 0;
    let destination;
    const requests = [];
    const plannedFiles = ['README.md', 'package.json', 'src/main.cjs', 'tests/main.test.cjs'];
    const form = { dataset: { devForm: 'generate-project' }, reportValidity: () => true,
        addEventListener: (name, handler) => { if (name === 'submit') submit = handler; } };
    const elements = { 'dev-error': {}, 'dev-progress': {}, 'dev-task-data': { value: JSON.stringify({ id: 'project-task',
        revision: 1, changes: [], contextFiles: [], draft: { plannedFiles }, standard: { requiredFiles: ['README.md'] } }) } };
    const root = { querySelectorAll: selector => selector === '[data-dev-form]' ? [form] : [] };
    runInNewContext(readFileSync(join(__dirname, '../src/AiDevPlatform.Web/wwwroot/js/development.js'), 'utf8'), {
        document: { querySelector: () => root, getElementById: id => elements[id] },
        location: { href: 'http://localhost/Development/Task/project-task#project', hash: '#project',
            reload: () => { reloads++; }, assign: () => assert.fail('same-task project generation must reload') },
        history: { replaceState: (state, title, url) => { destination = url; } },
        URL, AbortController, FormData: class { get() { return ''; } has() { return false; } },
        setTimeout: () => 1, clearTimeout() {},
        fetch: async (url, request) => {
            assert.equal(url.searchParams.get('operation'), 'generate');
            requests.push(JSON.parse(request.body));
            if (failSecondBatch && requests.length === 2)
                return { ok: false, status: 502, json: async () => ({ error: 'Model batch rejected' }) };
            return { ok: true, json: async () => ({ id: 'project-task', revision: requests.length + 1,
                contextFiles: [], changes: [{ files: plannedFiles.slice(0, requests.length * 2).map(path => ({ path, content: 'fixture only' })) }] }) };
        }
    });
    await submit({ preventDefault() {} });
    assert.deepEqual(requests.map(request => request.expectedRevision), [1, 2]);
    assert.ok(requests.every(request => request.generationMode === 'project'));
    if (failSecondBatch) {
        assert.equal(reloads, 0);
        assert.equal(elements['dev-error'].hidden, false);
        assert.match(elements['dev-error'].textContent, /已完成批次已保存/);
    } else {
        assert.equal(reloads, 1);
        assert.equal(destination, '/Development/Task/project-task#project');
    }
});