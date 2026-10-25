'use strict';

const http = require('node:http');
const { readFile } = require('node:fs/promises');
const { resolve } = require('node:path');
const { randomUUID } = require('node:crypto');
const { spawn } = require('node:child_process');
const tar = require('tar-stream');

class VerificationError extends Error {
    constructor(status, message) { super(message); this.status = status; }
}

function validatePath(value) {
    if (typeof value !== 'string' || !value.length || value.length > 180 || /[\x00-\x1f\x7f\\:*?"<>|]/.test(value))
        throw new VerificationError(400, 'Invalid source path.');
    for (const segment of value.split('/')) {
        if (!segment || segment === '.' || segment === '..' || /[. ]$/.test(segment) ||
            /^(\.git|_platform)$/i.test(segment) || /^\.env/i.test(segment) && segment.toLowerCase() !== '.env.example' ||
            /^(CON|PRN|AUX|NUL|COM[0-9]|LPT[0-9])(?:\.|$)/i.test(segment))
            throw new VerificationError(400, 'Reserved or unsafe source path.');
    }
}

function validateRequest(input) {
    const fields = ['runId', 'taskId', 'changeId', 'technology', 'files', 'requiredChecks'];
    if (!input || typeof input !== 'object' || Object.keys(input).some(key => !fields.includes(key)) ||
        !['runId', 'taskId', 'changeId'].every(key => typeof input[key] === 'string' &&
            /^[0-9a-f]{8}(?:-[0-9a-f]{4}){3}-[0-9a-f]{12}$/i.test(input[key]) && !/^0{8}-0{4}-0{4}-0{4}-0{12}$/.test(input[key])) ||
        typeof input.technology !== 'string' || !input.technology.trim() || input.technology.length > 200 ||
        !Array.isArray(input.files) || input.files.length < 1 || input.files.length > 24 ||
        !Array.isArray(input.requiredChecks) || input.requiredChecks.length < 1 || input.requiredChecks.length > 12 ||
        input.requiredChecks.some(name => typeof name !== 'string' || !name.trim() || name.length > 80) ||
        new Set(input.requiredChecks.map(name => name.toLowerCase())).size !== input.requiredChecks.length)
        throw new VerificationError(400, 'Invalid verification contract.');
    const names = new Set();
    let size = 0;
    for (const file of input.files) {
        if (!file || typeof file.content !== 'string' || file.content.length > 40000 || file.content.includes('\0'))
            throw new VerificationError(400, 'Invalid source content.');
        validatePath(file.path);
        const name = file.path.toLowerCase();
        if (names.has(name)) throw new VerificationError(400, 'Duplicate source path.');
        names.add(name); size += file.content.length;
    }
    if (size > 120000 || [...names].some(name => [...names].some(other => other.startsWith(`${name}/`))))
        throw new VerificationError(400, 'Source size or directory conflict.');
}

function validateConfig(config) {
    if (!config || typeof config.enabled !== 'boolean' || !Array.isArray(config.profiles) || config.profiles.length > 20)
        throw new VerificationError(503, 'Invalid verifier configuration.');
    for (const profile of config.profiles) {
        if (!profile || typeof profile.name !== 'string' || !/^[a-z0-9-]{1,60}$/.test(profile.name) ||
            !/^sha256:[0-9a-f]{64}$/.test(profile.image) || !Array.isArray(profile.matchFiles) || !profile.matchFiles.length ||
            !Array.isArray(profile.excludeFiles) || !profile.checks || typeof profile.checks !== 'object' ||
            !Object.keys(profile.checks).length || Object.keys(profile.checks).length > 12)
            throw new VerificationError(503, 'Profiles must use pinned local image IDs and fixed commands.');
        for (const name of [...profile.matchFiles, ...profile.excludeFiles]) validatePath(name);
        for (const [name, command] of Object.entries(profile.checks)) {
            if (!name.trim() || name.length > 80 || !Array.isArray(command) || !command.length || command.length > 32 ||
                command.some(part => typeof part !== 'string' || part.length > 500 || /[\x00-\x1f]/.test(part)) ||
                !/^\/[A-Za-z0-9_./-]+$/.test(command[0]))
                throw new VerificationError(503, 'Check commands must be fixed executable argument arrays.');
        }
        if (!profile.environment || typeof profile.environment !== 'object' || Array.isArray(profile.environment) ||
            Object.keys(profile.environment).length > 20 || Object.entries(profile.environment).some(([name, value]) =>
                !/^[A-Z][A-Z0-9_]*$/.test(name) || typeof value !== 'string' || value.length > 500 || /[\x00-\x1f]/.test(value)))
            throw new VerificationError(503, 'Invalid fixed environment.');
    }
}

function dockerCommand(host, wslDistribution = '', launch = spawn) {
    const localPipe = host.startsWith('npipe:////./pipe/') && /^[A-Za-z0-9_-]+$/.test(host.slice('npipe:////./pipe/'.length));
    const localSocket = host.startsWith('unix:///') && !/[\x00-\x20?#]/.test(host);
    if (!localPipe && !localSocket) throw new VerificationError(503, 'A local Docker socket is required.');
    if (typeof wslDistribution !== 'string' || wslDistribution &&
        (!/^[A-Za-z0-9][A-Za-z0-9_.-]{0,63}$/.test(wslDistribution) || !localSocket))
        throw new VerificationError(503, 'WSL requires a fixed distribution and local Unix Docker socket.');
    return (args, { signal, input } = {}) => new Promise((resolveCommand, reject) => {
        const environment = Object.fromEntries(['PATH', 'Path', 'SystemRoot', 'WINDIR', 'TEMP', 'TMP']
            .filter(name => process.env[name]).map(name => [name, process.env[name]]));
        const executable = wslDistribution ? 'wsl.exe' : 'docker';
        const prefix = wslDistribution ? ['--distribution', wslDistribution, '--exec', 'docker'] : [];
        const child = launch(executable, [...prefix, '--host', host, ...args], { shell: false, windowsHide: true,
            env: environment, signal, stdio: ['pipe', 'pipe', 'pipe'] });
        const output = { stdout: [], stderr: [] };
        let size = 0;
        let exceeded = false;
        for (const name of ['stdout', 'stderr']) child[name].on('data', chunk => {
            size += chunk.length;
            if (size > 65536) { exceeded = true; child.kill(); }
            else output[name].push(chunk);
        });
        child.stdin.on('error', () => {});
        child.on('error', reject);
        child.on('close', code => {
            if (exceeded) return reject(new Error('Output limit exceeded.'));
            resolveCommand({ code, stdout: Buffer.concat(output.stdout).toString('utf8'), stderr: Buffer.concat(output.stderr).toString('utf8') });
        });
        child.stdin.end(input);
    });
}

async function sourceArchive(files) {
    const pack = tar.pack();
    const chunks = [];
    const complete = new Promise((resolveArchive, reject) => {
        pack.on('data', chunk => chunks.push(chunk)); pack.on('error', reject);
        pack.on('end', () => resolveArchive(Buffer.concat(chunks)));
    });
    for (const file of files) pack.entry({ name: file.path, type: 'file', mode: 0o444, uid: 0, gid: 0 }, file.content);
    pack.finalize();
    return complete;
}

function diagnostic(result) {
    const text = `${result.stdout || ''}\n${result.stderr || ''}`
        .replace(/\x1b\[[0-?]*[ -/]*[@-~]/g, '').replace(/[\x00-\x08\x0b-\x1f\x7f]/g, '').trim();
    return `Exit ${result.code}. ${text || 'No diagnostic output.'}`.slice(0, 1800);
}

function createVerifier(config, executeDocker) {
    validateConfig(config);
    return async (input, signal) => {
        validateRequest(input);
        if (!config.enabled) throw new VerificationError(503, 'Verifier disabled. No source executed.');
        const report = { runId: input.runId, taskId: input.taskId, changeId: input.changeId, checks: [] };
        const profiles = config.profiles.filter(profile => profile.matchFiles.every(name => input.files.some(file => file.path === name)) &&
            !profile.excludeFiles.some(name => input.files.some(file => file.path === name)));
        if (profiles.length !== 1 || input.requiredChecks.some(name => !Object.hasOwn(profiles[0].checks, name))) {
            report.checks = input.requiredChecks.map(name => ({ name, status: 'not-run', detail: 'No unambiguous trusted profile for all required checks.' }));
            return report;
        }
        const profile = profiles[0];
        const command = async (args, options = {}) => {
            const result = await executeDocker(args, { signal, ...options });
            if (result.code !== 0) throw new VerificationError(503, 'Docker setup or cleanup failed. No passing report issued.');
            return result;
        };
        try {
            const info = JSON.parse((await command(['info', '--format', '{{json .}}'])).stdout);
            const image = JSON.parse((await command(['image', 'inspect', '--format', '{{json .}}', profile.image])).stdout);
            if (info.OSType !== 'linux' || !info.SecurityOptions?.includes('name=seccomp,profile=builtin') ||
                image.Id !== profile.image || image.Os !== 'linux' || Object.keys(image.Config?.Volumes || {}).length)
                throw new VerificationError(503, 'Linux, seccomp and a reviewed image without declared volumes are required.');
            const archive = await sourceArchive(input.files);
            let stopped = false;
            for (const name of input.requiredChecks) {
                if (stopped || signal.aborted) {
                    report.checks.push({ name, status: 'not-run', detail: 'A previous check failed or the verification budget expired.' });
                    continue;
                }
                const container = `lowcode-verify-${randomUUID()}`;
                try {
                    await command(['create', '--name', container, '--pull', 'never', '--network', 'none', '--read-only',
                        '--cap-drop', 'ALL', '--security-opt', 'no-new-privileges', '--user', '10001:10001',
                        '--pids-limit', '64', '--memory', '256m', '--memory-swap', '256m', '--cpus', '1',
                        '--ulimit', 'nofile=128:128', '--log-driver', 'none', '--no-healthcheck', '--restart', 'no',
                        '--stop-timeout', '1', '--tmpfs', '/tmp:rw,nosuid,nodev,size=128m,mode=1777',
                        '--tmpfs', '/workspace:rw,noexec,nosuid,nodev,size=2m,uid=0,gid=0,mode=0755',
                        '--workdir', '/workspace', '--entrypoint', '/bin/sleep', profile.image, '65']);
                    await command(['start', container]);
                    await command(['exec', '--user', '0:0', '-i', container, '/bin/tar', '-xf', '-', '-C', '/workspace', '--no-same-owner'], { input: archive });
                    const environment = Object.entries(profile.environment).flatMap(([key, value]) => ['--env', `${key}=${value}`]);
                    let result;
                    try { result = await executeDocker(['exec', '--workdir', '/workspace', ...environment, container, ...profile.checks[name]], { signal }); }
                    catch { result = { code: -1, stderr: 'Check cancelled, timed out, unavailable or exceeded output limit.' }; }
                    stopped = result.code !== 0;
                    report.checks.push({ name, status: stopped ? 'failed' : 'passed', detail: diagnostic(result) });
                } finally {
                    await command(['rm', '--force', '--volumes', container], { signal: AbortSignal.timeout(20000) });
                }
            }
            return report;
        } catch (error) {
            if (error instanceof VerificationError) throw error;
            throw new VerificationError(503, 'Docker unavailable or verification interrupted. No passing report issued.');
        }
    };
}

function createServer(config, executeDocker) {
    const verify = createVerifier(config, executeDocker);
    let busy = false;
    return http.createServer(async (request, response) => {
        const send = (status, value) => {
            if (!response.destroyed) response.writeHead(status, { 'Content-Type': 'application/json', 'Cache-Control': 'no-store' }).end(JSON.stringify(value));
        };
        try {
            const address = request.socket.remoteAddress;
            const host = new URL(`http://${request.headers.host || ''}`).hostname;
            if (!['127.0.0.1', '::1', '::ffff:127.0.0.1'].includes(address) || !['localhost', '127.0.0.1', '[::1]'].includes(host) ||
                request.headers.origin !== undefined || request.headers['sec-fetch-site'] !== undefined)
                return send(403, { error: 'Local server requests only.' });
            if (request.method === 'GET' && request.url === '/health')
                return send(config.enabled ? 200 : 503, { status: config.enabled ? 'configured' : 'disabled', runtimeChecked: false, backend: 'docker' });
            if (request.method !== 'POST' || request.url !== '/verify') return send(404, { error: 'Not found.' });
            if (!/^application\/json(?:;|$)/i.test(request.headers['content-type'] || '')) return send(415, { error: 'JSON required.' });
            if (Number(request.headers['content-length']) > 262144) {
                request.resume();
                response.setHeader('Connection', 'close');
                return send(413, { error: 'Request too large.' });
            }
            if (busy) return send(409, { error: 'Verifier busy.' });
            busy = true;
            const controller = new AbortController();
            const timer = setTimeout(() => controller.abort(), 55000);
            const cancel = () => { if (!response.writableEnded) controller.abort(); };
            const stopReading = () => { if (!request.complete) request.destroy(); };
            controller.signal.addEventListener('abort', stopReading, { once: true });
            response.on('close', cancel);
            try {
                const chunks = [];
                let size = 0;
                for await (const chunk of request) {
                    size += chunk.length;
                    if (size > 262144) throw new VerificationError(413, 'Request too large.');
                    chunks.push(chunk);
                }
                let input;
                try { input = JSON.parse(Buffer.concat(chunks).toString('utf8')); }
                catch { throw new VerificationError(400, 'Invalid JSON.'); }
                send(200, await verify(input, controller.signal));
            } finally {
                clearTimeout(timer); response.off('close', cancel);
                controller.signal.removeEventListener('abort', stopReading); busy = false;
            }
        } catch (error) { send(error instanceof VerificationError ? error.status : 503,
            { error: error instanceof VerificationError ? error.message : 'Verifier unavailable.' }); }
    });
}

if (require.main === module) {
    (async () => {
        const file = process.env.VERIFIER_CONFIG || resolve(__dirname, 'profiles.example.json');
        const config = JSON.parse(await readFile(file, 'utf8'));
        if (!Number.isInteger(config.port) || config.port < 1024 || config.port > 65535) throw new Error('Invalid port.');
        const distribution = process.env.VERIFIER_WSL_DISTRIBUTION || '';
        const host = process.env.VERIFIER_DOCKER_HOST || (process.platform === 'win32' && !distribution ?
            'npipe:////./pipe/dockerDesktopLinuxEngine' : 'unix:///var/run/docker.sock');
        const server = createServer(config, dockerCommand(host, distribution));
        server.on('error', () => { console.error('Verifier failed to listen.'); process.exitCode = 1; });
        server.listen(config.port, '127.0.0.1', () => console.log(`Verifier ${config.enabled ? 'configured' : 'disabled'} at http://127.0.0.1:${config.port}; runtime not yet checked.`));
    })().catch(() => { console.error('Verifier configuration invalid or unavailable.'); process.exitCode = 1; });
}

module.exports = { createVerifier, createServer, validateRequest, validateConfig, dockerCommand };