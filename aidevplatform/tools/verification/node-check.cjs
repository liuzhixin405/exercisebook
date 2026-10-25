'use strict';

const { readdirSync, readFileSync } = require('node:fs');
const { join } = require('node:path');
const { spawnSync } = require('node:child_process');
const { wrap } = require('node:module');
const { Script } = require('node:vm');

const required = ['package.json', 'README.md', 'src/database.cjs', 'src/security.cjs', 'src/catalog.cjs',
    'src/cart.cjs', 'src/orders.cjs', 'src/admin.cjs', 'src/server.cjs', 'public/index.html',
    'public/style.css', 'public/app.js', 'public/admin.js', 'tests/store.test.cjs', 'tests/http.test.cjs', 'scripts/check.cjs'];

function inventory(directory = '.') {
    return readdirSync(directory, { withFileTypes: true }).flatMap(entry => {
        const path = directory === '.' ? entry.name : join(directory, entry.name);
        if (entry.isSymbolicLink()) throw new Error('Symbolic links are not supported.');
        return entry.isDirectory() ? inventory(path) : [path];
    });
}

function execute(arguments_, timeout) {
    const result = spawnSync(process.execPath, arguments_, { shell: false, stdio: 'inherit', timeout });
    if (result.error || result.status !== 0) process.exit(result.status || 1);
}

try {
    const mode = process.argv[2];
    if (!['build', 'test'].includes(mode)) throw new Error('Unknown fixed check.');
    const files = inventory();
    if (mode === 'build') {
        if (files.some(path => path.endsWith('.mjs'))) throw new Error('ES modules require a separate reviewed syntax profile.');
        for (const path of files.filter(path => /\.(cjs|js)$/.test(path)).sort()) {
            const source = readFileSync(path, 'utf8').replace(/^\uFEFF/, '').replace(/^#![^\r\n]*/, '');
            new Script(path.startsWith('public/') ? source : wrap(source), { filename: path });
        }
        const manifest = JSON.parse(readFileSync('package.json', 'utf8'));
        const dependencies = manifest.dependencies || {};
        if (dependencies.express !== '5.1.0' || Object.keys(dependencies).some(name => name !== 'express') ||
            Object.keys(manifest.devDependencies || {}).length || Object.keys(manifest.optionalDependencies || {}).length)
            throw new Error('This reviewed offline profile supports exactly express 5.1.0 and Node built-ins; other dependencies require a separate reviewed image.');
        const missing = required.filter(path => !files.includes(path));
        if (missing.length) throw new Error(`Incomplete project: ${missing.join(', ')}`);
        console.log('Build check: all project JavaScript parses, the pinned dependency contract matches, and all 16 planned files exist.');
    } else {
        for (const path of ['tests/store.test.cjs', 'tests/http.test.cjs'])
            if (!files.includes(path)) throw new Error(`Missing required test: ${path}`);
        execute(['--test', '--test-concurrency=1', 'tests/store.test.cjs', 'tests/http.test.cjs'], 40000);
    }
} catch (error) {
    console.error(error.message);
    process.exitCode = 1;
}