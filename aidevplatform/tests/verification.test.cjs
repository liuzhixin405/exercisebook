const { test } = require('node:test');
const assert = require('node:assert/strict');
const { once } = require('node:events');
const { readFileSync } = require('node:fs');
const { createRequire } = require('node:module');
const { EventEmitter } = require('node:events');
const { PassThrough } = require('node:stream');
const http = require('node:http');
const { createVerifier, createServer, validateRequest, validateConfig, dockerCommand } = require('../tools/verification/server.cjs');
const tar = createRequire(require.resolve('../tools/verification/package.json'))('tar-stream');

const example = JSON.parse(readFileSync(require.resolve('../tools/verification/profiles.example.json'), 'utf8'));
const configuration = () => ({ ...structuredClone(example), enabled: true });
const request = () => ({ runId: '11111111-1111-4111-8111-111111111111', taskId: '22222222-2222-4222-8222-222222222222',
    changeId: '33333333-3333-4333-8333-333333333333', technology: 'Go CLI',
    files: [{ path: 'go.mod', content: 'module greeting\n\ngo 1.22\n' },
        { path: 'src/main.go', content: 'package main\n// $(untrusted) <script>not commands</script>\n' }],
    requiredChecks: ['build', 'test'] });

function dockerFixture(options = {}) {
    const calls = [];
    const execute = async (args, context = {}) => {
        calls.push({ args, ...context });
        if (args[0] === 'info') return { code: 0, stdout: JSON.stringify({ OSType: options.os || 'linux', SecurityOptions: options.security || ['name=seccomp,profile=builtin'] }) };
        if (args[0] === 'image') return { code: 0, stdout: JSON.stringify({ Id: example.profiles[0].image,
            Os: 'linux', Config: { Volumes: options.volumes || null } }) };
        if (options.failAt === args[0] || options.failAt === 'upload' && context.input)
            return { code: 1, stderr: 'fixture setup failure' };
        if (args[0] === 'exec' && args.includes('/usr/local/go/bin/go')) {
            if (options.check) return options.check(args, context);
            return { code: options.code ?? 0, stdout: '\x1b[31mfixture output\x1b[0m' };
        }
        if (args[0] === 'rm' && options.cleanupError) return { code: 1, stderr: 'fixture cleanup failure' };
        return { code: 0, stdout: '' };
    };
    return { execute, calls };
}

async function extractArchive(bytes) {
    const archive = tar.extract();
    const files = [];
    archive.on('entry', (header, stream, next) => {
        const chunks = [];
        stream.on('data', chunk => chunks.push(chunk));
        stream.on('end', () => { files.push({ header, content: Buffer.concat(chunks).toString('utf8') }); next(); });
    });
    const complete = once(archive, 'finish'); archive.end(bytes); await complete;
    return files;
}

test('disabled verifier never calls Docker and reports no fabricated success', async () => {
    const docker = dockerFixture();
    await assert.rejects(createVerifier(example, docker.execute)(request(), AbortSignal.timeout(1000)), error => error.status === 503);
    assert.equal(docker.calls.length, 0);
});

test('source validation rejects traversal, reserved paths, duplicates and injected commands before execution', () => {
    for (const path of ['../escape', '/absolute', 'C:/escape', '.git/config', '.env', '_platform/data', 'CON.go', 'nested\\file', 'trailing.']) {
        const input = request(); input.files[0].path = path;
        assert.throws(() => validateRequest(input), error => error.status === 400, path);
    }
    for (const files of [[{ path: 'a', content: '' }, { path: 'A', content: '' }],
        [{ path: 'a', content: '' }, { path: 'a/b', content: '' }],
        [{ path: 'a', content: '\0' }], [{ path: 'a', content: 'x'.repeat(40001) }]]) {
        assert.throws(() => validateRequest({ ...request(), files }), error => error.status === 400);
    }
    assert.throws(() => validateRequest({ ...request(), command: ['powershell', 'untrusted'] }), error => error.status === 400);
    assert.throws(() => validateRequest({ ...request(), requiredChecks: ['build', 'BUILD'] }), error => error.status === 400);
    assert.throws(() => validateRequest({ ...request(), runId: 'not-a-guid' }), error => error.status === 400);
});

test('profile configuration requires pinned images and administrator-owned command arrays', () => {
    const config = configuration(); config.profiles[0].image = 'golang:latest';
    assert.throws(() => validateConfig(config), error => error.status === 503);
    config.profiles[0].image = example.profiles[0].image; config.profiles[0].checks.build = 'go build .';
    assert.throws(() => validateConfig(config), error => error.status === 503);
    config.profiles[0].checks.build = ['go', 'build', '.'];
    assert.throws(() => validateConfig(config), error => error.status === 503);
    for (const host of ['tcp://localhost:2375', 'ssh://example.com', 'unix://remote/socket', 'npipe:////./pipe/bad?query'])
        assert.throws(() => dockerCommand(host), error => error.status === 503);
});

test('unsupported or ambiguous technology profiles return not-run without invoking Docker', async () => {
    for (const mutate of [input => { input.files[0].path = 'Cargo.toml'; }, input => { input.requiredChecks = ['architecture']; }]) {
        const docker = dockerFixture(); const input = request(); mutate(input);
        const report = await createVerifier(configuration(), docker.execute)(input, AbortSignal.timeout(1000));
        assert.ok(report.checks.every(check => check.status === 'not-run')); assert.equal(docker.calls.length, 0);
    }
    const config = configuration(); config.profiles.push(structuredClone(config.profiles[0]));
    const docker = dockerFixture();
    const report = await createVerifier(config, docker.execute)(request(), AbortSignal.timeout(1000));
    assert.ok(report.checks.every(check => check.status === 'not-run')); assert.equal(docker.calls.length, 0);
});

test('each check uses a separate constrained container and streams source as tar without host mounts', async () => {
    const docker = dockerFixture(); const input = request();
    const report = await createVerifier(configuration(), docker.execute)(input, AbortSignal.timeout(1000));
    assert.equal(report.runId, input.runId); assert.equal(report.changeId, input.changeId); assert.equal(report.taskId, input.taskId);
    assert.ok(report.checks.every(check => check.status === 'passed'));
    assert.ok(report.checks.every(check => !check.detail.includes('\x1b')));
    const creates = docker.calls.filter(call => call.args[0] === 'create');
    assert.equal(creates.length, 2);
    assert.notEqual(creates[0].args[2], creates[1].args[2]);
    for (const { args } of creates) {
        for (const option of ['--read-only', '--no-healthcheck']) assert.ok(args.includes(option));
        assert.ok(args.includes('/workspace:rw,noexec,nosuid,nodev,size=2m,uid=0,gid=0,mode=0755'));
        for (const [name, value] of [['--network', 'none'], ['--cap-drop', 'ALL'], ['--user', '10001:10001'],
            ['--pids-limit', '64'], ['--memory', '256m'], ['--memory-swap', '256m'], ['--cpus', '1'], ['--pull', 'never']])
            assert.equal(args[args.indexOf(name) + 1], value);
        assert.ok(!args.some(value => ['--mount', '--volume', '-v', '--privileged', '--publish'].includes(value)));
        assert.equal(args.at(-1), '65');
    }
    const uploads = docker.calls.filter(call => call.input);
    assert.equal(uploads.length, 2);
    assert.ok(uploads.every(call => call.args[call.args.indexOf('--user') + 1] === '0:0'));
    const checks = docker.calls.filter(call => call.args.includes('/usr/local/go/bin/go'));
    assert.ok(checks.every(call => !call.args.includes('--user')));
    const files = await extractArchive(uploads[0].input);
    assert.deepEqual(files.map(file => ({ path: file.header.name, content: file.content })), input.files);
    assert.ok(files.every(file => file.header.type === 'file' && file.header.mode === 0o444));
    assert.equal(docker.calls.filter(call => call.args[0] === 'rm').length, 2);
});

test('failed check prevents subsequent execution and still cleans its container', async () => {
    const docker = dockerFixture({ code: 1 });
    const report = await createVerifier(configuration(), docker.execute)(request(), AbortSignal.timeout(1000));
    assert.deepEqual(report.checks.map(check => check.status), ['failed', 'not-run']);
    assert.equal(docker.calls.filter(call => call.args[0] === 'create').length, 1);
    assert.equal(docker.calls.filter(call => call.args[0] === 'rm').length, 1);
});

test('cancelled checks and output errors cannot pass and cleanup uses a fresh cancellation budget', async () => {
    const controller = new AbortController();
    const docker = dockerFixture({ check: async () => { controller.abort(); throw new Error('cancelled'); } });
    const report = await createVerifier(configuration(), docker.execute)(request(), controller.signal);
    assert.deepEqual(report.checks.map(check => check.status), ['failed', 'not-run']);
    const cleanup = docker.calls.find(call => call.args[0] === 'rm');
    assert.equal(cleanup.signal.aborted, false);
    assert.notEqual(cleanup.signal, controller.signal);
});

test('Docker errors, Windows engines, missing seccomp and image volumes fail closed before running source', async () => {
    for (const options of [{ os: 'windows' }, { security: ['name=apparmor'] },
        { security: ['name=seccomp,profile=unconfined'] }, { volumes: { '/data': {} } }]) {
        const docker = dockerFixture(options);
        await assert.rejects(createVerifier(configuration(), docker.execute)(request(), AbortSignal.timeout(1000)), error => error.status === 503);
        assert.ok(!docker.calls.some(call => call.args[0] === 'create'));
    }
    await assert.rejects(createVerifier(configuration(), async () => { throw new Error('ENOENT'); })(request(), AbortSignal.timeout(1000)), error => error.status === 503);
});

test('failed container cleanup invalidates an otherwise passing result', async () => {
    const docker = dockerFixture({ cleanupError: true });
    await assert.rejects(createVerifier(configuration(), docker.execute)(request(), AbortSignal.timeout(1000)), error => error.status === 503);
});

test('failed container creation, start or source upload always attempts independent cleanup', async () => {
    for (const failAt of ['create', 'start', 'upload']) {
        const docker = dockerFixture({ failAt });
        await assert.rejects(createVerifier(configuration(), docker.execute)(request(), AbortSignal.timeout(1000)), error => error.status === 503);
        assert.equal(docker.calls.filter(call => call.args[0] === 'rm').length, 1);
        assert.ok(!docker.calls.some(call => call.args.includes('/usr/local/go/bin/go')));
    }
});

test('missing process exit status cannot produce a passing report', async () => {
    const docker = dockerFixture({ check: async () => ({ code: null, stdout: 'interrupted' }) });
    const report = await createVerifier(configuration(), docker.execute)(request(), AbortSignal.timeout(1000));
    assert.deepEqual(report.checks.map(check => check.status), ['failed', 'not-run']);
});

test('diagnostics are bounded and Docker setup errors do not leak internal output', async () => {
    const docker = dockerFixture({ check: async () => ({ code: 1, stderr: 'x'.repeat(10000) }) });
    const report = await createVerifier(configuration(), docker.execute)(request(), AbortSignal.timeout(1000));
    assert.ok(report.checks[0].detail.length <= 1800);
    await assert.rejects(createVerifier(configuration(), async () => ({ code: 1, stderr: 'private daemon information' }))
        (request(), AbortSignal.timeout(1000)), error => error.status === 503 && !error.message.includes('private'));
});

async function withServer(config, execute, action) {
    const server = createServer(config, execute); server.listen(0, '127.0.0.1'); await once(server, 'listening');
    const url = `http://127.0.0.1:${server.address().port}`;
    try { await action(url, server); }
    finally { server.closeAllConnections(); await new Promise(resolveClose => server.close(resolveClose)); }
}

function post(url, input = request(), headers = {}) {
    return fetch(`${url}/verify`, { method: 'POST', headers: { 'Content-Type': 'application/json', ...headers }, body: JSON.stringify(input) });
}

test('HTTP protocol rejects browser calls, foreign hosts, invalid JSON and oversized inputs', async () => {
    const docker = dockerFixture();
    await withServer(configuration(), docker.execute, async url => {
        for (const headers of [{ Origin: url }, { 'Sec-Fetch-Site': 'same-origin' }])
            assert.equal((await post(url, request(), headers)).status, 403, JSON.stringify(headers));
        const foreignHost = await new Promise((resolveStatus, reject) => {
            const client = http.request(`${url}/verify`, { method: 'POST', headers: { Host: 'example.com', 'Content-Type': 'application/json' } },
                response => { response.resume(); resolveStatus(response.statusCode); });
            client.on('error', reject); client.end(JSON.stringify(request()));
        });
        assert.equal(foreignHost, 403);
        assert.equal((await post(url, request(), { 'Content-Type': 'text/plain' })).status, 415);
        assert.equal((await post(url, { ...request(), technology: 'x'.repeat(300000) })).status, 413);
        const malformed = await fetch(`${url}/verify`, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: '{' });
        assert.equal(malformed.status, 400);
        const report = await post(url); assert.equal(report.status, 200);
        assert.equal((await report.json()).checks[0].status, 'passed');
    });
});

test('health does not claim runtime readiness and disabled HTTP service returns unavailable', async () => {
    const docker = dockerFixture();
    await withServer(example, docker.execute, async url => {
        const health = await fetch(`${url}/health`); assert.equal(health.status, 503);
        assert.equal((await health.json()).runtimeChecked, false);
        assert.equal((await post(url)).status, 503);
    });
    assert.equal(docker.calls.length, 0);
});

test('concurrent HTTP verification returns busy without sharing running containers', async () => {
    let release;
    let entered;
    const gate = new Promise(resolveGate => { release = resolveGate; });
    const started = new Promise(resolveStarted => { entered = resolveStarted; });
    const docker = dockerFixture({ check: async () => { entered(); await gate; return { code: 0, stdout: 'fixture' }; } });
    await withServer(configuration(), docker.execute, async url => {
        const first = post(url); await started;
        try { assert.equal((await post(url)).status, 409); }
        finally { release(); }
        assert.equal((await first).status, 200);
        assert.equal((await post(url)).status, 200);
    });
});

test('body deadline disconnects an incomplete request and releases the busy slot', async context => {
    const docker = dockerFixture();
    await withServer(configuration(), docker.execute, async (url, server) => {
        context.mock.timers.enable({ apis: ['setTimeout'] });
        const arrived = once(server, 'request');
        const client = http.request(`${url}/verify`, { method: 'POST', headers: { 'Content-Type': 'application/json' } });
        const disconnected = once(client, 'error');
        client.flushHeaders(); client.write('{'); await arrived;
        context.mock.timers.tick(55001); await disconnected;
        context.mock.timers.reset();
        const response = await post(url);
        assert.equal(response.status, 200);
        assert.equal((await response.json()).checks[0].status, 'passed');
    });
});

test('WSL transport uses fixed argument arrays, preserves binary stdin and rejects shell-like distributions', async () => {
    for (const distribution of ['-root', 'Ubuntu;whoami', '../Ubuntu', 'Ubuntu test', null])
        assert.throws(() => dockerCommand('unix:///var/run/docker.sock', distribution), error => error.status === 503);
    assert.throws(() => dockerCommand('npipe:////./pipe/dockerDesktopLinuxEngine', 'Ubuntu'), error => error.status === 503);
    const calls = [];
    const bytes = Buffer.from([0, 255, 13, 10, 128]);
    let uploaded;
    const command = dockerCommand('unix:///var/run/docker.sock', 'Ubuntu', (executable, args, options) => {
        calls.push({ executable, args, options });
        const child = new EventEmitter();
        child.stdout = new PassThrough(); child.stderr = new PassThrough(); child.stdin = new PassThrough();
        const chunks = [];
        child.stdin.on('data', chunk => chunks.push(chunk));
        child.stdin.on('finish', () => {
            uploaded = Buffer.concat(chunks); child.stdout.end('fixture output'); child.emit('close', 0);
        });
        return child;
    });
    const result = await command(['exec', '-i', 'container', '/bin/tar', '-xf', '-'], { input: bytes });
    assert.deepEqual(uploaded, bytes);
    assert.deepEqual(result, { code: 0, stdout: 'fixture output', stderr: '' });
    assert.equal(calls[0].executable, 'wsl.exe');
    assert.deepEqual(calls[0].args, ['--distribution', 'Ubuntu', '--exec', 'docker', '--host',
        'unix:///var/run/docker.sock', 'exec', '-i', 'container', '/bin/tar', '-xf', '-']);
    assert.equal(calls[0].options.shell, false);
    assert.ok(Object.keys(calls[0].options.env).every(name => ['PATH', 'Path', 'SystemRoot', 'WINDIR', 'TEMP', 'TMP'].includes(name)));
});