const { test, before, after } = require('node:test');
const assert = require('node:assert/strict');
const { readFileSync, mkdtempSync, rmSync, copyFileSync, symlinkSync, readdirSync } = require('node:fs');
const { tmpdir } = require('node:os');
const path = require('node:path');
const http = require('node:http');
const { spawn } = require('node:child_process');
const { once } = require('node:events');
const { createHash } = require('node:crypto');

const root = path.resolve(__dirname, '..');
const buildConfiguration = process.env.LOWCODE_TEST_CONFIGURATION || 'Debug';
if (!['Debug', 'Release'].includes(buildConfiguration)) throw new Error('LOWCODE_TEST_CONFIGURATION must be Debug or Release');
const children = [];
const calls = [];
const draft = { title: 'Contact', components: [{ type: 'input', text: 'Name', inputType: 'text' }] };
let upstreamStatus = 200;
let upstreamBody = JSON.stringify(draft);
let disconnect = false;
let server;
let webUrl;
let apiUrl;
let managementUrl;
let cookie;
let token;
const developmentDirectory = mkdtempSync(path.join(tmpdir(), 'lowcode-development-'));
let modelReply;
let modelFinish = 'stop';
let verifierReply;
let verifierStatus = 200;
let strictSequence = 0;
const requirementDraft = {
    specification: 'Create a greeting CLI with input validation.', questions: ['What is the default name?'],
    assumptions: ['Local use only'], acceptanceCriteria: ['A greeting is printed'], plannedFiles: ['main.exs', 'README.md']
};

function startService(project, environment = {}) {
    const directory = path.join(root, 'src', project);
    const child = spawn('dotnet', [path.join(directory, 'bin', buildConfiguration, 'net8.0', `${project}.dll`),
        '--urls', 'http://127.0.0.1:0'], {
        cwd: directory,
        env: { ...process.env, ASPNETCORE_ENVIRONMENT: 'Development', DOTNET_ENVIRONMENT: 'Development',
            Serilog__MinimumLevel__Default: 'Information',
            Serilog__MinimumLevel__Override__Microsoft: 'Information', ...environment },
        windowsHide: true,
        stdio: ['ignore', 'pipe', 'pipe']
    });
    children.push(child);
    return new Promise((resolve, reject) => {
        const timer = setTimeout(() => reject(new Error(`${project} startup timed out; build both projects in ${buildConfiguration} first`)), 30000);
        let output = '';
        child.stdout.on('data', chunk => {
            output = (output + chunk.toString()).slice(-16000);
            const match = output.match(/Now listening on: (http:\/\/127\.0\.0\.1:\d+)/);
            if (match) {
                clearTimeout(timer);
                resolve(match[1]);
            }
        });
        child.stderr.on('data', () => {});
        child.once('error', error => { clearTimeout(timer); reject(error); });
        child.once('exit', code => {
            clearTimeout(timer);
            reject(new Error(`${project} exited with code ${code}`));
        });
    });
}

before(async () => {
    server = http.createServer(async (request, response) => {
        let body = '';
        for await (const chunk of request) body += chunk;
        calls.push({ url: request.url, headers: request.headers, body: body ? JSON.parse(body) : null });
        if (request.url === '/verify') {
            const report = await verifierReply(JSON.parse(body));
            response.writeHead(verifierStatus, { 'Content-Type': 'application/json', Location: '/should-not-follow' });
            return response.end(typeof report === 'string' ? report : JSON.stringify(report));
        }
        if (request.url === '/v1/chat/completions') {
            response.writeHead(200, { 'Content-Type': 'application/json' });
            return response.end(JSON.stringify({ choices: [{ finish_reason: modelFinish,
                message: { content: typeof modelReply === 'string' ? modelReply : JSON.stringify(modelReply) } }] }));
        }
        if (disconnect) return request.socket.destroy();
        response.writeHead(upstreamStatus, { 'Content-Type': 'application/json' });
        response.end(upstreamBody);
    });
    server.listen(0, '127.0.0.1');
    await once(server, 'listening');
    apiUrl = await startService('AiDevPlatform.API', { LocalAI__Enabled: 'true',
        LocalAI__BaseUrl: `http://127.0.0.1:${server.address().port}/v1/`,
        Verification__Enabled: 'true', Verification__Endpoint: `http://127.0.0.1:${server.address().port}/verify`,
        DevelopmentWorkspace__DataPath: developmentDirectory });
    webUrl = await startService('AiDevPlatform.Web', {
        Api__BaseUrl: `http://127.0.0.1:${server.address().port}/`,
        UiFeatures__AiDraftEnabled: 'true'
    });
    const page = await fetch(`${webUrl}/Designer`);
    cookie = page.headers.get('set-cookie').split(';')[0];
    const html = await page.text();
    token = html.match(/name="__RequestVerificationToken"[^>]*value="([^"]+)"/)[1];
    managementUrl = await startService('AiDevPlatform.Web', { Api__BaseUrl: `${apiUrl}/` });
});

after(async () => {
    if (server) {
        server.closeAllConnections();
        await new Promise(resolve => server.close(resolve));
    }
    await Promise.all(children.map(async child => {
        if (child.exitCode === null && child.signalCode === null) {
            const exited = once(child, 'exit');
            child.kill();
            await exited;
        }
    }));
    rmSync(developmentDirectory, { recursive: true, force: true });
});

function postWeb(prompt = 'Contact form', withToken = true) {
    const body = new URLSearchParams({ prompt });
    if (withToken) body.set('__RequestVerificationToken', token);
    return fetch(`${webUrl}/Designer?handler=Generate`, {
        method: 'POST', headers: { Cookie: cookie }, body
    });
}

function postApi(body, headers = {}, route = '/api/ai/page-drafts') {
    return new Promise((resolve, reject) => {
        const request = http.request(`${apiUrl}${route}`, {
            method: 'POST', headers: { 'Content-Type': 'application/json', 'X-LowCode-Client': 'Web', ...headers }
        }, response => {
            response.resume();
            resolve({ status: response.statusCode });
        });
        request.on('error', reject);
        request.end(typeof body === 'string' ? body : JSON.stringify(body));
    });
}

test('Web runtime contains shared contracts but no API, Core or Infrastructure dependency', () => {
    const dependencies = JSON.parse(readFileSync(path.join(root,
        `src/AiDevPlatform.Web/bin/${buildConfiguration}/net8.0/AiDevPlatform.Web.deps.json`), 'utf8'));
    const libraries = Object.keys(dependencies.libraries);
    assert.ok(libraries.some(name => name.startsWith('AiDevPlatform.Shared/')));
    assert.ok(!libraries.some(name => /^AiDevPlatform\.(API|Core|Infrastructure)\//.test(name)));
});

test('Web forwards JSON contract to API with a server-side client header', async () => {
    const response = await postWeb();
    assert.equal(response.status, 200);
    assert.deepEqual(await response.json(), draft);
    const call = calls.at(-1);
    assert.equal(call.url, '/api/ai/page-drafts');
    assert.deepEqual(call.body, { prompt: 'Contact form' });
    assert.equal(call.headers['x-lowcode-client'], 'Web');
    assert.equal(call.headers.origin, undefined);
    assert.equal(call.headers.cookie, undefined);
    assert.match(call.headers['content-type'], /^application\/json/);
});

test('Web preserves expected API failures without exposing upstream content', async () => {
    upstreamBody = JSON.stringify({ error: 'private upstream details' });
    try {
        for (const status of [400, 403, 409, 504, 500]) {
            upstreamStatus = status;
            const response = await postWeb();
            assert.equal(response.status, status === 500 ? 502 : status);
            const body = await response.json();
            assert.ok(body.error);
            assert.doesNotMatch(body.error, /private upstream details/);
        }
    } finally {
        upstreamStatus = 200;
        upstreamBody = JSON.stringify(draft);
    }
});

test('Web maps malformed API JSON and disconnected upstream to 502', async () => {
    try {
        upstreamBody = 'not json';
        assert.equal((await postWeb()).status, 502);
        disconnect = true;
        assert.equal((await postWeb()).status, 502);
    } finally {
        disconnect = false;
        upstreamBody = JSON.stringify(draft);
    }
});

test('Web rejects missing antiforgery and invalid prompts before calling API', async () => {
    const count = calls.length;
    assert.equal((await postWeb('Contact form', false)).status, 400);
    assert.equal((await postWeb('')).status, 400);
    assert.equal((await postWeb('a'.repeat(2001))).status, 400);
    assert.equal(calls.length, count);
});

test('API rejects browser origins, fetch metadata, missing client header and foreign Host', async () => {
    for (const headers of [
        { Origin: 'http://example.com' },
        { Origin: apiUrl },
        { 'Sec-Fetch-Site': 'same-origin' },
        { 'X-LowCode-Client': '' },
        { Host: 'example.com' }
    ]) {
        assert.equal((await postApi({ prompt: 'Contact form' }, headers)).status, 403, JSON.stringify(headers));
    }
});

test('API validates JSON, content type and prompt before calling model', async () => {
    for (const body of [{ prompt: '' }, { prompt: 'a'.repeat(2001) }, {}, 'null', '{']) {
        assert.equal((await postApi(body)).status, 400);
    }
    assert.equal((await postApi({ prompt: 'Contact form' }, { 'Content-Type': 'text/plain' })).status, 415);
});

function applicationRequest(method, route = '', body) {
    return fetch(`${apiUrl}/api/applications${route}`, {
        method, headers: { 'Content-Type': 'application/json', 'X-LowCode-Client': 'Web' },
        body: body === undefined ? undefined : JSON.stringify(body)
    });
}

test('applications can be created, listed, read and edited through the API', async () => {
    const input = { name: 'Customer Portal', code: 'customer_portal', description: 'CRM', version: '1.0.0' };
    const created = await applicationRequest('POST', '', { ...input, ownerId: '00000000-0000-0000-0000-000000000000' });
    assert.equal(created.status, 201);
    const application = await created.json();
    assert.equal(application.code, 'CUSTOMER_PORTAL');
    assert.ok(created.headers.get('location').endsWith(application.id));
    assert.ok((await (await applicationRequest('GET')).json()).some(item => item.id === application.id));
    assert.equal((await (await applicationRequest('GET', `/${application.id}`)).json()).name, input.name);
    const updated = await applicationRequest('PUT', `/${application.id}`, { ...input, name: 'Customer Workspace', version: '1.1.0' });
    assert.equal(updated.status, 200);
    const restored = await (await applicationRequest('GET', `/${application.id}`)).json();
    assert.equal(restored.name, 'Customer Workspace');
    assert.equal(restored.version, '1.1.0');
    assert.ok(restored.updatedAt);
    const duplicate = await applicationRequest('POST', '', { ...input, code: 'CUSTOMER_portal' });
    assert.equal(duplicate.status, 409);
});

test('concurrent creation of the same application code succeeds only once', async () => {
    const input = { name: 'Concurrent App', code: 'concurrent_app', version: '1.0.0' };
    const responses = await Promise.all([applicationRequest('POST', '', input), applicationRequest('POST', '', input)]);
    assert.deepEqual(responses.map(response => response.status).sort(), [201, 409]);
});

test('application API rejects invalid data, unknown IDs and browser-origin writes', async () => {
    const input = { name: 'Valid App', code: 'valid_app', version: '1.0.0' };
    for (const body of [{ ...input, name: ' ' }, { ...input, code: 'bad code' },
        { ...input, description: 'a'.repeat(1001) }, { ...input, version: 'a'.repeat(21) }]) {
        assert.equal((await applicationRequest('POST', '', body)).status, 400);
    }
    const missing = '/00000000-0000-0000-0000-000000000099';
    assert.equal((await applicationRequest('GET', missing)).status, 404);
    assert.equal((await applicationRequest('PUT', missing, input)).status, 404);
    for (const headers of [{ Origin: 'http://example.com' }, { 'Sec-Fetch-Site': 'same-origin' },
        { Host: 'example.com' }, { 'X-LowCode-Client': '' }]) {
        assert.equal((await postApi(input, headers, '/api/applications')).status, 403);
    }
});

async function formSession(url) {
    const response = await fetch(url);
    const html = await response.text();
    return {
        cookie: response.headers.get('set-cookie').split(';')[0],
        token: html.match(/name="__RequestVerificationToken"[^>]*value="([^"]+)"/)[1]
    };
}

function submitApplication(url, session, fields, withToken = true) {
    const body = new URLSearchParams(Object.entries(fields).map(([key, value]) => [`Input.${key}`, value]));
    if (withToken) body.set('__RequestVerificationToken', session.token);
    return fetch(url, { method: 'POST', redirect: 'manual', headers: { Cookie: session.cookie }, body });
}

test('Web menu, create/edit forms and application-scoped designer work end to end', async () => {
    const home = await (await fetch(managementUrl)).text();
    assert.match(home, /href="\/Applications"/);
    const createUrl = `${managementUrl}/Applications/Edit`;
    const session = await formSession(createUrl);
    const fields = { Name: 'Web Customer App', Code: 'web_customer_app', Description: 'From Web', Version: '1.0.0' };
    const created = await submitApplication(createUrl, session, fields);
    assert.equal(created.status, 302);
    assert.equal(created.headers.get('location'), '/Applications');
    const applications = await (await applicationRequest('GET')).json();
    const application = applications.find(item => item.code === 'WEB_CUSTOMER_APP');
    assert.ok(application);
    const list = await (await fetch(`${managementUrl}/Applications`)).text();
    assert.ok(list.includes(`/Applications/Edit/${application.id}`));
    assert.ok(list.includes(`/Designer?applicationId=${application.id}`));
    const editUrl = `${managementUrl}/Applications/Edit/${application.id}`;
    const editing = await formSession(editUrl);
    const edited = await submitApplication(editUrl, editing, { ...fields, Name: 'Updated <script>alert(1)</script>', Version: '2.0.0' });
    assert.equal(edited.status, 302);
    const restored = await (await applicationRequest('GET', `/${application.id}`)).json();
    assert.equal(restored.version, '2.0.0');
    const filtered = await (await fetch(`${managementUrl}/Applications?searchTerm=WEB_CUSTOMER_APP`)).text();
    assert.ok(filtered.includes('Updated &lt;script&gt;alert(1)&lt;/script&gt;'));
    assert.ok(!filtered.includes('Customer Workspace'));
    const designer = await fetch(`${managementUrl}/Designer?applicationId=${application.id.toUpperCase()}`);
    assert.equal(designer.status, 200);
    const markup = await designer.text();
    assert.ok(markup.includes(`data-application-id="${application.id}"`));
    assert.ok(markup.includes('Updated &lt;script&gt;alert(1)&lt;/script&gt;'));
    const missing = '00000000-0000-0000-0000-000000000099';
    assert.equal((await fetch(`${managementUrl}/Applications/Edit/${missing}`)).status, 404);
    assert.equal((await fetch(`${managementUrl}/Designer?applicationId=${missing}`)).status, 404);
    assert.equal((await fetch(`${managementUrl}/Designer?applicationId=invalid`)).status, 404);
});

test('Web application forms reject invalid submissions and handle API failure without losing input', async () => {
    const url = `${managementUrl}/Applications/Edit`;
    const session = await formSession(url);
    const fields = { Name: 'Keep this name', Code: 'web_customer_app', Description: '', Version: '1.0.0' };
    assert.equal((await submitApplication(url, session, fields, false)).status, 400);
    assert.equal((await submitApplication(url, session, { ...fields, Name: '' })).status, 400);
    const duplicate = await submitApplication(url, session, fields);
    assert.equal(duplicate.status, 409);
    assert.match(await duplicate.text(), /field-validation-error/);
    const offlineUrl = `${webUrl}/Applications/Edit`;
    const offline = await formSession(offlineUrl);
    upstreamStatus = 500;
    upstreamBody = JSON.stringify({ error: 'private backend failure' });
    try {
        const failure = await submitApplication(offlineUrl, offline, fields);
        assert.equal(failure.status, 502);
        const html = await failure.text();
        assert.ok(html.includes('value="Keep this name"'));
        assert.ok(!html.includes('private backend failure'));
        assert.equal((await fetch(`${webUrl}/Applications`)).status, 502);
    } finally {
        upstreamStatus = 200;
        upstreamBody = JSON.stringify(draft);
    }
});

function developmentRequest(route, body, base = apiUrl) {
    return fetch(`${base}/api/development/${route}`, { method: body === undefined ? 'GET' : 'POST',
        headers: { 'Content-Type': 'application/json', 'X-LowCode-Client': 'Web' },
        body: body === undefined ? undefined : JSON.stringify(body) });
}

test('development workflow pins standards, accepts arbitrary languages and enforces confirmation/revision checks', async () => {
    const standards = await (await developmentRequest('standards')).json();
    const task = await (await developmentRequest('tasks', { title: 'Elixir greeting', technology: 'Elixir / arbitrary CLI',
        requirement: 'Print a greeting with input validation', standardId: standards[0].id, contextFiles: [] })).json();
    assert.equal(task.technology, 'Elixir / arbitrary CLI');
    assert.equal(task.standard.instructions, standards[0].instructions);
    const route = `tasks/${task.id}`;
    assert.equal((await developmentRequest(`${route}/generate`, { expectedRevision: task.revision })).status, 409);
    modelReply = requirementDraft;
    const clarified = await (await developmentRequest(`${route}/clarify`, { expectedRevision: task.revision })).json();
    assert.equal(clarified.draft.questions.length, 1);
    assert.equal((await developmentRequest(`${route}/confirm`, { expectedRevision: clarified.revision, draft: clarified.draft, answers: [] })).status, 400);
    const confirmed = await (await developmentRequest(`${route}/confirm`, { expectedRevision: clarified.revision,
        draft: clarified.draft, answers: ['Use world'] })).json();
    assert.equal(confirmed.confirmed, true);
    assert.equal((await developmentRequest(`${route}/generate`, { expectedRevision: clarified.revision })).status, 409);
    const standard = { name: standards[0].name, version: '2.0.0', instructions: 'Different naming policy', requiredFiles: ['README.md'], forbiddenTerms: [] };
    assert.equal((await developmentRequest('standards', standard)).status, 200);
    assert.equal((await developmentRequest('standards', standard)).status, 409);
    const restored = await (await developmentRequest(route)).json();
    assert.equal(restored.standard.version, standards[0].version);
    assert.equal(restored.standard.instructions, standards[0].instructions);
});

test('development workflow blocks violations, preserves versions and exports only reviewed snapshots', async () => {
    const tasks = await (await developmentRequest('tasks')).json();
    let task = await (await developmentRequest(`tasks/${tasks.find(item => item.title === 'Elixir greeting').id}`)).json();
    const route = `tasks/${task.id}`;
    modelReply = { summary: 'Add greeting implementation', files: [{ path: 'main.exs', content: 'IO.puts("hello")\n' }] };
    task = await (await developmentRequest(`${route}/generate`, { expectedRevision: task.revision })).json();
    assert.ok(task.changes[0].checks.some(check => check.status === 'failed'));
    assert.ok(task.changes[0].checks.some(check => check.status === 'not-run'));
    assert.equal((await developmentRequest(`${route}/review`, { expectedRevision: task.revision, decision: 'accepted',
        feedback: 'Looks fine', riskAcknowledged: true })).status, 409);
    assert.equal((await developmentRequest(`${route}/changes/${task.changes[0].id}/download`)).status, 409);
    modelReply = { summary: 'Supply README and fix greeting default', files: [
        { path: 'main.exs', content: 'IO.puts("hello world")\n' }, { path: 'README.md', content: 'Run elixir main.exs. Local use only.\n' }] };
    task = await (await developmentRequest(`${route}/generate`, { expectedRevision: task.revision, feedback: 'Add required README' })).json();
    assert.equal(task.changes.length, 2);
    assert.equal(task.changes[0].files.length, 1);
    assert.ok(!task.changes[1].checks.some(check => check.status === 'failed'));
    assert.equal((await developmentRequest(`${route}/review`, { expectedRevision: task.revision, decision: 'accepted', feedback: 'Reviewed' })).status, 409);
    task = await (await developmentRequest(`${route}/review`, { expectedRevision: task.revision, decision: 'accepted',
        feedback: 'Reviewed dependency and interface impact; checks not run acknowledged', riskAcknowledged: true })).json();
    const archive = await developmentRequest(`${route}/changes/${task.changes[1].id}/download`);
    assert.equal(archive.status, 200);
    assert.equal(archive.headers.get('content-type'), 'application/zip');
    const bytes = Buffer.from(await archive.arrayBuffer());
    assert.equal(bytes.readUInt32LE(0), 0x04034b50);
    assert.ok(bytes.includes(Buffer.from('_platform/delivery.json')));
    assert.ok(bytes.includes(Buffer.from('main.exs')));
    assert.equal((await developmentRequest(`${route}/review`, { expectedRevision: task.revision, decision: 'accepted',
        feedback: 'Overwrite decision', riskAcknowledged: true })).status, 409);
    const restarted = await startService('AiDevPlatform.API', { DevelopmentWorkspace__DataPath: developmentDirectory });
    const persisted = await (await developmentRequest(route, undefined, restarted)).json();
    assert.equal(persisted.changes[1].review, 'accepted');
    assert.equal(persisted.standard.version, task.standard.version);
});

test('development rejects unsafe source paths, malformed/truncated model output and browser calls', async () => {
    const standards = await (await developmentRequest('standards')).json();
    const input = { title: 'Rust test', technology: 'Rust', requirement: 'Small CLI', standardId: standards[0].id,
        contextFiles: [{ path: '../escape.rs', content: 'text' }] };
    assert.equal((await developmentRequest('tasks', input)).status, 400);
    const task = await (await developmentRequest('tasks', { ...input, contextFiles: [] })).json();
    const route = `tasks/${task.id}`;
    modelReply = 'invalid JSON';
    assert.equal((await developmentRequest(`${route}/clarify`, { expectedRevision: task.revision })).status, 502);
    modelReply = requirementDraft; modelFinish = 'length';
    assert.equal((await developmentRequest(`${route}/clarify`, { expectedRevision: task.revision })).status, 502);
    modelFinish = 'stop';
    let current = await (await developmentRequest(`${route}/clarify`, { expectedRevision: task.revision })).json();
    current = await (await developmentRequest(`${route}/confirm`, { expectedRevision: current.revision, draft: current.draft, answers: ['world'] })).json();
    for (const path of ['../escape.rs', '/absolute.rs', 'C:/escape.rs', '.git/config', '.env', '_platform/metadata.json', 'CON.rs']) {
        modelReply = { summary: 'Unsafe path', files: [{ path, content: 'text' }] };
        assert.equal((await developmentRequest(`${route}/generate`, { expectedRevision: current.revision })).status, 502, path);
    }
    assert.equal((await developmentRequest(route)).status, 200);
    for (const headers of [{ Origin: 'http://localhost' }, { 'Sec-Fetch-Site': 'same-origin' }, { 'X-LowCode-Client': '' }]) {
        assert.equal((await fetch(`${apiUrl}/api/development/tasks`, { headers: { 'X-LowCode-Client': 'Web', ...headers } })).status, 403);
    }
});

function verificationReport(request, status = 'passed') {
    return { runId: request.runId, taskId: request.taskId, changeId: request.changeId,
        checks: request.requiredChecks.map(name => ({ name, status, detail: `Fixture ${status}: ${name}; no generated code executed` })) };
}

async function strictTask() {
    const standard = await (await developmentRequest('standards', { name: 'Required CI', version: String(++strictSequence),
        instructions: 'Build and tests are mandatory', requiredFiles: ['README.md'], forbiddenTerms: [], requiredChecks: ['build', 'test'] })).json();
    let task = await (await developmentRequest('tasks', { title: 'Verified CLI', technology: 'Elixir', requirement: 'Greeting CLI',
        standardId: standard.id, contextFiles: [] })).json();
    const route = `tasks/${task.id}`;
    modelReply = requirementDraft;
    task = await (await developmentRequest(`${route}/clarify`, { expectedRevision: task.revision })).json();
    task = await (await developmentRequest(`${route}/confirm`, { expectedRevision: task.revision, draft: task.draft, answers: ['world'] })).json();
    modelReply = { summary: 'Greeting CLI', files: [{ path: 'main.exs', content: 'IO.puts("hello")' }, { path: 'README.md', content: 'Example CLI' }] };
    return (await developmentRequest(`${route}/generate`, { expectedRevision: task.revision })).json();
}

test('required verification cannot be bypassed by risk acknowledgement or reused after source changes', async () => {
    let task = await strictTask();
    const route = `tasks/${task.id}`;
    const accept = () => developmentRequest(`${route}/review`, { expectedRevision: task.revision, decision: 'accepted',
        feedback: 'Reviewed', riskAcknowledged: true, verifications: [{ checks: [{ name: 'build', status: 'passed' }] }] });
    assert.equal((await accept()).status, 409);
    for (const status of ['failed', 'not-run']) {
        verifierReply = request => verificationReport(request, status);
        const response = await developmentRequest(`${route}/verify`, { expectedRevision: task.revision });
        assert.equal(response.status, 200);
        task = await response.json();
        assert.equal((await accept()).status, 409);
    }
    verifierReply = request => verificationReport(request);
    for (let attempt = 0; attempt < 2; attempt++)
        task = await (await developmentRequest(`${route}/verify`, { expectedRevision: task.revision })).json();
    task = await (await developmentRequest(`${route}/verify`, { expectedRevision: task.revision })).json();
    const call = calls.at(-1);
    assert.equal(call.url, '/verify');
    assert.equal(call.body.changeId, task.changes[0].id);
    assert.deepEqual(call.body.requiredChecks, ['build', 'test']);
    assert.equal(call.body.files.length, 2);
    assert.equal(call.headers.cookie, undefined);
    assert.equal(call.headers.origin, undefined);
    assert.ok(task.changes[0].verifications.at(-1).receivedAt);
    assert.equal(task.changes[0].verifications.length, 5);
    assert.equal((await developmentRequest(`${route}/verify`, { expectedRevision: task.revision })).status, 409);
    const accepted = await accept(); assert.equal(accepted.status, 200); task = await accepted.json();
    assert.equal((await developmentRequest(`${route}/changes/${task.changes[0].id}/download`)).status, 200);
    const oldChange = task.changes[0].id;
    modelReply = { summary: 'Changed greeting', files: [{ path: 'main.exs', content: 'IO.puts("updated")' }] };
    task = await (await developmentRequest(`${route}/generate`, { expectedRevision: task.revision })).json();
    const modelInput = JSON.parse(calls.filter(call => call.url === '/v1/chat/completions').at(-1).body.messages[1].content);
    assert.equal(modelInput.reviewFeedback, 'Reviewed');
    assert.equal(modelInput.verificationReport.checks[0].status, 'passed');
    assert.deepEqual(task.changes[1].verifications, []);
    assert.equal((await accept()).status, 409);
    assert.equal((await developmentRequest(`${route}/changes/${task.changes[1].id}/download`)).status, 409);
    assert.equal((await developmentRequest(`${route}/changes/${oldChange}/download`)).status, 200);
});

test('platform materializes its own model snapshot with provenance without running code or weakening delivery gates', async () => {
    let task = await strictTask();
    const route = `tasks/${task.id}`;
    const change = task.changes.at(-1);
    assert.ok(change.generation.requestId);
    assert.ok(change.generation.model);
    assert.equal((await developmentRequest(`${route}/materialize`, { expectedRevision: task.revision })).status, 409);
    const response = await developmentRequest(`${route}/materialize`, { expectedRevision: task.revision, riskAcknowledged: true,
        directoryPath: '../untrusted-destination', workspace: { runtimeStatus: 'passed' } });
    assert.equal(response.status, 200);
    task = await response.json();
    const workspace = task.changes.at(-1).workspace;
    assert.equal(workspace.taskId, task.id); assert.equal(workspace.changeId, change.id);
    assert.equal(workspace.runtimeStatus, 'not-run');
    assert.ok(path.resolve(workspace.directoryPath).startsWith(path.join(developmentDirectory, 'projects') + path.sep));
    for (const file of change.files) {
        const actual = readFileSync(path.join(workspace.directoryPath, file.path), 'utf8');
        assert.equal(actual, file.content);
        assert.equal(workspace.files.find(item => item.path === file.path).sha256, createHash('sha256').update(actual).digest('hex'));
    }
    const manifest = JSON.parse(readFileSync(path.join(workspace.directoryPath, '_platform/project.json'), 'utf8'));
    assert.equal(manifest.purpose, 'inspection-only');
    assert.equal(manifest.generation.requestId, change.generation.requestId);
    assert.equal(manifest.review, 'pending');
    assert.equal((await developmentRequest(`${route}/materialize`, { expectedRevision: task.revision, riskAcknowledged: true })).status, 409);
    assert.equal((await developmentRequest(`${route}/changes/${change.id}/download`)).status, 409);
    const restarted = await startService('AiDevPlatform.API', { DevelopmentWorkspace__DataPath: developmentDirectory });
    const persisted = await (await developmentRequest(route, undefined, restarted)).json();
    assert.equal(persisted.changes.at(-1).workspace.directoryPath, workspace.directoryPath);
    modelReply = { summary: 'New model revision', files: [{ path: 'main.exs', content: 'IO.puts("new revision")' }] };
    task = await (await developmentRequest(`${route}/generate`, { expectedRevision: task.revision })).json();
    assert.equal(task.changes.at(-1).workspace, null);
    assert.equal(readFileSync(path.join(workspace.directoryPath, 'main.exs'), 'utf8'), 'IO.puts("hello")');
    const linkedDirectory = mkdtempSync(path.join(tmpdir(), 'lowcode-linked-'));
    const outside = mkdtempSync(path.join(tmpdir(), 'lowcode-outside-'));
    try {
        copyFileSync(path.join(developmentDirectory, `${task.id}.task.json`), path.join(linkedDirectory, `${task.id}.task.json`));
        symlinkSync(outside, path.join(linkedDirectory, 'projects'), process.platform === 'win32' ? 'junction' : 'dir');
        const linked = await startService('AiDevPlatform.API', { DevelopmentWorkspace__DataPath: linkedDirectory });
        assert.equal((await developmentRequest(`${route}/materialize`, { expectedRevision: task.revision, riskAcknowledged: true }, linked)).status, 503);
        assert.deepEqual(readdirSync(outside), []);
    } finally {
        rmSync(linkedDirectory, { recursive: true, force: true });
        rmSync(outside, { recursive: true, force: true });
    }
    task = await (await developmentRequest(`${route}/materialize`, { expectedRevision: task.revision, riskAcknowledged: true })).json();
    const newer = task.changes.at(-1).workspace;
    assert.notEqual(newer.directoryPath, workspace.directoryPath);
    assert.equal(newer.files.find(file => file.path === 'README.md').origin, `model:${change.generation.requestId}`);
    assert.equal(newer.files.find(file => file.path === 'main.exs').origin, `model:${task.changes.at(-1).generation.requestId}`);
    const page = await fetch(`${managementUrl}/Development/Task/${task.id}`);
    assert.equal(page.status, 200);
    const html = await page.text();
    assert.ok(html.includes('panel-project'));
    assert.ok(html.includes(newer.directoryPath));
    assert.ok(html.includes('未运行'));
});

test('project generation resumes only missing planned files and rejects off-plan or incomplete AI batches', async () => {
    const standard = (await (await developmentRequest('standards')).json())[0];
    let task = await (await developmentRequest('tasks', { title: 'Complete project batches', technology: 'Node.js',
        requirement: 'A complete tested CLI', standardId: standard.id, contextFiles: [] })).json();
    const route = `tasks/${task.id}`;
    const plannedFiles = ['README.md', 'package.json', 'src/main.js', 'tests/main.test.js'];
    modelReply = { ...requirementDraft, plannedFiles };
    task = await (await developmentRequest(`${route}/clarify`, { expectedRevision: task.revision })).json();
    task = await (await developmentRequest(`${route}/confirm`, { expectedRevision: task.revision, draft: task.draft, answers: ['world'] })).json();
    const batch = () => developmentRequest(`${route}/generate`, { expectedRevision: task.revision, generationMode: 'project' });
    modelReply = { summary: 'Project foundation', files: [{ path: 'README.md', content: 'Example' }] };
    task = await (await batch()).json();
    assert.deepEqual(JSON.parse(calls.at(-1).body.messages[1].content).selectedFiles, plannedFiles.slice(0, 1));
    const schema = calls.at(-1).body.response_format.schema;
    assert.equal(schema.properties.files.minItems, 1);
    assert.equal(schema.properties.files.maxItems, 1);
    assert.deepEqual(schema.properties.files.items.properties.path.enum, plannedFiles.slice(0, 1));
    assert.equal(task.changes.at(-1).checks.find(check => check.name === '项目计划').status, 'failed');
    modelReply = { summary: 'Off-plan attempt', files: [{ path: 'README.md', content: 'Overwrite' }, { path: 'src/main.js', content: 'text' }] };
    const rejected = await batch();
    assert.equal(rejected.status, 502);
    assert.equal((await rejected.json()).code, 'model-batch-mismatch');
    modelReply = { summary: 'Incomplete batch', files: [] };
    assert.equal((await batch()).status, 502);
    const unchanged = await (await developmentRequest(route)).json();
    assert.equal(unchanged.revision, task.revision);
    for (let index = 1; index < plannedFiles.length; index++) {
        modelReply = { summary: 'Next complete file', files: [{ path: plannedFiles[index], content: 'fixture source, not execution evidence' }] };
        const response = await batch();
        assert.equal(response.status, 200);
        task = await response.json();
        const input = JSON.parse(calls.at(-1).body.messages[1].content);
        assert.deepEqual(input.selectedFiles, [plannedFiles[index]]);
        assert.equal(input.baseline.length, index);
    }
    assert.equal(task.changes.length, 4);
    assert.equal(task.changes.at(-1).files.length, 4);
    assert.equal(task.changes.at(-1).checks.find(check => check.name === '项目计划').status, 'passed');
    assert.equal((await batch()).status, 409);
    assert.equal((await developmentRequest(`${route}/generate`, { expectedRevision: task.revision, generationMode: 'run-on-host' })).status, 400);
    assert.equal((await developmentRequest(`${route}/generate`, { expectedRevision: task.revision - 1, generationMode: 'project' })).status, 409);
});

test('targeted repair carries rejection feedback, replaces only its target and resets delivery evidence', async () => {
    let task = await strictTask();
    const route = `tasks/${task.id}`;
    const original = task.changes.at(-1);
    task = await (await developmentRequest(`${route}/review`, { expectedRevision: task.revision,
        decision: 'changes-requested', feedback: 'README omits the real error path; preserve tests' })).json();
    const repair = targetPath => developmentRequest(`${route}/generate`, { expectedRevision: task.revision,
        generationMode: 'repair', targetPath, feedback: 'Explain the actual error path without weakening checks' });
    for (const target of ['../escape', 'missing.exs']) assert.equal((await repair(target)).status, 400);
    assert.equal((await developmentRequest(`${route}/generate`, { expectedRevision: task.revision,
        generationMode: 'repair', targetPath: 'README.md' })).status, 400);
    modelReply = { summary: 'Wrong target', files: [{ path: 'main.exs', content: 'Wrong' }] };
    assert.equal((await repair('README.md')).status, 502);
    assert.equal((await (await developmentRequest(route)).json()).revision, task.revision);
    modelReply = { summary: 'Explain the error path', files: [{ path: 'README.md', content: 'Actual error behavior documented' }] };
    task = await (await repair('README.md')).json();
    const input = JSON.parse(calls.at(-1).body.messages[1].content);
    assert.deepEqual(input.selectedFiles, ['README.md']);
    assert.equal(input.baseline[0].path, 'README.md');
    assert.equal(input.reviewFeedback, 'README omits the real error path; preserve tests');
    assert.equal(input.feedback, 'Explain the actual error path without weakening checks');
    const repaired = task.changes.at(-1);
    assert.equal(repaired.generationTargetPath, 'README.md');
    assert.equal(repaired.generationFeedback, input.feedback);
    assert.equal(repaired.files.find(file => file.path === 'main.exs').content, original.files.find(file => file.path === 'main.exs').content);
    assert.equal(repaired.review, 'pending');
    assert.deepEqual(repaired.verifications, []);
    assert.equal(repaired.workspace, null);
    assert.notEqual(repaired.generation.requestId, original.generation.requestId);
    assert.equal((await developmentRequest(`${route}/changes/${repaired.id}/download`)).status, 409);
    task = await (await developmentRequest(`${route}/materialize`, { expectedRevision: task.revision, riskAcknowledged: true })).json();
    const manifest = JSON.parse(readFileSync(path.join(task.changes.at(-1).workspace.directoryPath, '_platform/project.json'), 'utf8'));
    assert.equal(manifest.generationTargetPath, 'README.md');
    assert.equal(manifest.generationFeedback, repaired.generationFeedback);
    const page = await fetch(`${managementUrl}/Development/Task/${task.id}`);
    assert.ok((await page.text()).includes('name="targetPath"'));
    modelReply = { summary: 'Second repair', files: [{ path: 'README.md', content: 'More accurate documentation' }] };
    task = await (await repair('README.md')).json();
    assert.equal(JSON.parse(calls.at(-1).body.messages[1].content).reviewFeedback, 'README omits the real error path; preserve tests');
});

test('verifier rejects malformed, incomplete, duplicate, foreign and replayed reports without changing task state', async () => {
    const task = await strictTask();
    const route = `tasks/${task.id}`;
    let captured;
    const invalid = [
        request => ({ ...verificationReport(request), runId: '00000000-0000-0000-0000-000000000000' }),
        request => ({ ...verificationReport(request), taskId: '00000000-0000-0000-0000-000000000000' }),
        request => ({ ...verificationReport(request), changeId: '00000000-0000-0000-0000-000000000000' }),
        request => ({ ...verificationReport(request), checks: null }),
        request => ({ ...verificationReport(request), checks: [null, null] }),
        request => ({ ...verificationReport(request), checks: verificationReport(request).checks.slice(0, 1) }),
        request => ({ ...verificationReport(request), checks: [verificationReport(request).checks[0], verificationReport(request).checks[0]] }),
        request => ({ ...verificationReport(request), checks: [{ name: 'build', status: 'success', detail: 'Not valid' }, verificationReport(request).checks[1]] }),
        () => 'invalid json', () => 'null', () => 'x'.repeat(70000)
    ];
    for (const reply of invalid) {
        verifierReply = reply;
        assert.equal((await developmentRequest(`${route}/verify`, { expectedRevision: task.revision })).status, 502);
    }
    verifierReply = request => { captured = verificationReport(request); return captured; };
    verifierStatus = 302;
    try { assert.equal((await developmentRequest(`${route}/verify`, { expectedRevision: task.revision })).status, 502); }
    finally { verifierStatus = 200; }
    verifierReply = () => captured;
    assert.equal((await developmentRequest(`${route}/verify`, { expectedRevision: task.revision })).status, 502);
    const restored = await (await developmentRequest(route)).json();
    assert.equal(restored.revision, task.revision);
    assert.deepEqual(restored.changes[0].verifications, []);
    for (const requiredChecks of [null, ['build', ' BUILD '], [' '], ['x'.repeat(81)]]) {
        assert.equal((await developmentRequest('standards', { name: 'Invalid', version: '1', instructions: 'rules', requiredChecks })).status, 400);
    }
});

test('unconfigured or unsafe verification endpoints fail closed without executing or forwarding', async () => {
    const task = await strictTask();
    const route = `tasks/${task.id}/verify`;
    for (const configuration of [
        { Verification__Enabled: 'false' },
        { Verification__Enabled: 'true', Verification__Endpoint: 'http://example.com/verify' },
        { Verification__Enabled: 'true', Verification__Endpoint: `http://127.0.0.1:${server.address().port}/verify?override=true` }
    ]) {
        const service = await startService('AiDevPlatform.API', { DevelopmentWorkspace__DataPath: developmentDirectory, ...configuration });
        const count = calls.filter(call => call.url === '/verify').length;
        assert.equal((await developmentRequest(route, { expectedRevision: task.revision }, service)).status, 503);
        assert.equal(calls.filter(call => call.url === '/verify').length, count);
    }
});

test('verification finishing after a newer source revision cannot attach stale evidence', async () => {
    let task = await strictTask();
    const route = `tasks/${task.id}`;
    let release;
    let entered;
    const gate = new Promise(resolve => { release = resolve; });
    const started = new Promise(resolve => { entered = resolve; });
    verifierReply = async request => { entered(); await gate; return verificationReport(request); };
    const pending = developmentRequest(`${route}/verify`, { expectedRevision: task.revision });
    await started;
    try {
        modelReply = { summary: 'Newer source', files: [{ path: 'main.exs', content: 'IO.puts("new")' }] };
        task = await (await developmentRequest(`${route}/generate`, { expectedRevision: task.revision })).json();
    } finally { release(); }
    assert.equal((await pending).status, 409);
    const restored = await (await developmentRequest(route)).json();
    assert.equal(restored.revision, task.revision);
    assert.ok(restored.changes.every(change => change.verifications.length === 0));
});

test('Web development workspace forwards protected actions and renders generated source only as text', async () => {
    const redirect = await fetch(managementUrl, { redirect: 'manual' });
    assert.equal(redirect.status, 302);
    assert.equal(redirect.headers.get('location'), '/Development');
    const indexUrl = `${managementUrl}/Development`;
    const home = await (await fetch(indexUrl)).text();
    assert.ok(home.includes('href="/Development/Standards"'));
    assert.ok(!home.includes('href="/Designer"'));
    const session = await formSession(indexUrl);
    const standards = await (await developmentRequest('standards')).json();
    const input = { title: 'UI <script>alert(1)</script>', technology: 'Haskell', requirement: 'Small greeting CLI',
        standardId: standards.find(item => item.version === '1.0.0').id, contextFiles: [] };
    const post = (url, state, payload, withToken = true) => fetch(url, {
        method: 'POST', headers: { Cookie: state.cookie, 'Content-Type': 'application/json',
            ...(withToken ? { RequestVerificationToken: state.token } : {}) }, body: JSON.stringify(payload)
    });
    assert.equal((await post(indexUrl, session, input, false)).status, 400);
    const created = await post(indexUrl, session, input);
    assert.equal(created.status, 200);
    let task = await created.json();
    const taskUrl = `${managementUrl}/Development/Task/${task.id}`;
    const taskSession = await formSession(taskUrl);
    modelReply = requirementDraft;
    task = await (await post(`${taskUrl}?operation=clarify`, taskSession, { expectedRevision: task.revision })).json();
    task = await (await post(`${taskUrl}?operation=confirm`, taskSession, {
        expectedRevision: task.revision, draft: task.draft, answers: ['world'] })).json();
    modelReply = { summary: 'Add real files', files: [
        { path: 'Main.hs', content: 'main = putStrLn "Hello world"\n' },
        { path: 'README.md', content: 'Use runhaskell Main.hs' },
        { path: 'example.html', content: '<script>window.sourceExecuted=true</script>' }] };
    const generated = await post(`${taskUrl}?operation=generate`, taskSession, { expectedRevision: task.revision });
    assert.equal(generated.status, 200);
    task = await generated.json();
    const page = await (await fetch(taskUrl)).text();
    assert.ok(page.includes('UI &lt;script&gt;alert(1)&lt;/script&gt;'));
    assert.ok(page.includes('dev-source-after'));
    assert.ok(!page.includes('<script>window.sourceExecuted=true</script>'));
    assert.equal((await post(`${taskUrl}?operation=execute`, taskSession, {})).status, 400);
    assert.equal((await fetch(`${taskUrl}?handler=Download&changeId=${task.changes[0].id}`)).status, 409);
});

test('Web protects verifier submission with antiforgery and renders version-bound verification records', async () => {
    const task = await strictTask();
    const url = `${managementUrl}/Development/Task/${task.id}`;
    const session = await formSession(url);
    verifierReply = request => verificationReport(request, 'failed');
    const payload = { expectedRevision: task.revision, report: { checks: [{ name: 'build', status: 'passed' }] } };
    const post = withToken => fetch(`${url}?operation=verify`, { method: 'POST',
        headers: { Cookie: session.cookie, 'Content-Type': 'application/json', ...(withToken ? { RequestVerificationToken: session.token } : {}) },
        body: JSON.stringify(payload) });
    assert.equal((await post(false)).status, 400);
    const verified = await post(true); assert.equal(verified.status, 200);
    const saved = await verified.json();
    assert.equal(saved.changes[0].verifications[0].checks[0].status, 'failed');
    const html = await (await fetch(url)).text();
    assert.ok(html.includes('dev-verify-form'));
    assert.ok(html.includes('dev-verification-history'));
    assert.ok(html.includes(saved.changes[0].verifications[0].runId));
});

test('API stays forbidden in Production even when LocalAI is enabled', async () => {
    const productionUrl = await startService('AiDevPlatform.API', {
        ASPNETCORE_ENVIRONMENT: 'Production', DOTNET_ENVIRONMENT: 'Production', LocalAI__Enabled: 'true'
    });
    const response = await fetch(`${productionUrl}/api/ai/page-drafts`, {
        method: 'POST', headers: { 'Content-Type': 'application/json', 'X-LowCode-Client': 'Web' },
        body: JSON.stringify({ prompt: 'Contact form' })
    });
    assert.equal(response.status, 403);
    const applications = await fetch(`${productionUrl}/api/applications`, {
        headers: { 'X-LowCode-Client': 'Web' }
    });
    assert.equal(applications.status, 403);
    assert.equal((await developmentRequest('tasks', undefined, productionUrl)).status, 403);
    assert.equal((await developmentRequest('tasks/11111111-1111-4111-8111-111111111111/verify', { expectedRevision: 1 }, productionUrl)).status, 403);
    const productionWeb = await startService('AiDevPlatform.Web', {
        ASPNETCORE_ENVIRONMENT: 'Production', DOTNET_ENVIRONMENT: 'Production', Api__BaseUrl: `${apiUrl}/`
    });
    assert.equal((await fetch(`${productionWeb}/Applications`)).status, 403);
    assert.equal((await fetch(`${productionWeb}/Applications/Edit`)).status, 403);
    assert.equal((await fetch(`${productionWeb}/Development`)).status, 403);
    assert.equal((await fetch(`${productionWeb}/Development/Standards`)).status, 403);
});