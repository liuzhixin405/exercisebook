(() => {
    'use strict';
    const root = document.querySelector('.dev-workspace');
    if (!root) return;
    const errorBox = document.getElementById('dev-error');
    const progress = document.getElementById('dev-progress');
    const dataElement = document.getElementById('dev-task-data');
    const task = dataElement ? JSON.parse(dataElement.value) : null;
    const lines = value => String(value || '').split(/\r?\n/).map(line => line.trim()).filter(Boolean);
    let requestController = null;

    function selectTab(name) {
        for (const tab of root.querySelectorAll('[data-dev-tab]')) {
            const active = tab.dataset.devTab === name;
            tab.setAttribute('aria-selected', String(active));
            tab.tabIndex = active ? 0 : -1;
        }
        for (const panel of root.querySelectorAll('[role=tabpanel]')) panel.hidden = panel.id !== `panel-${name}`;
    }
    const tabs = Array.from(root.querySelectorAll('[data-dev-tab]'));
    tabs.forEach((tab, index) => {
        tab.addEventListener('click', () => selectTab(tab.dataset.devTab));
        tab.addEventListener('keydown', event => {
            if (!['ArrowLeft', 'ArrowRight', 'Home', 'End'].includes(event.key)) return;
            event.preventDefault();
            const target = event.key === 'Home' ? 0 : event.key === 'End' ? tabs.length - 1 :
                (index + (event.key === 'ArrowRight' ? 1 : -1) + tabs.length) % tabs.length;
            selectTab(tabs[target].dataset.devTab); tabs[target].focus();
        });
    });
    const requested = location.hash.slice(1);
    if (tabs.length) selectTab(tabs.some(tab => tab.dataset.devTab === requested) ? requested : 'requirements');

    document.getElementById('context-files')?.addEventListener('change', event => {
        const list = document.getElementById('context-list');
        list.replaceChildren();
        for (const file of event.target.files) {
            const item = document.createElement('li'); item.textContent = `${file.name} (${file.size} B)`; list.append(item);
        }
        document.getElementById('context-count').textContent = `${event.target.files.length} 个文件`;
    });
    document.getElementById('dev-cancel')?.addEventListener('click', () => requestController?.abort());

    for (const form of root.querySelectorAll('[data-dev-form]')) form.addEventListener('submit', async event => {
        event.preventDefault();
        if (requestController || !form.reportValidity()) return;
        const fields = new FormData(form);
        const mode = form.dataset.devForm;
        const buttons = Array.from(root.querySelectorAll('form button')).map(button => ({ button, disabled: button.disabled }));
        let timer;
        let saved;
        try {
            errorBox.hidden = true;
            let payload;
            if (mode === 'create') {
                const selected = Array.from(document.getElementById('context-files').files);
                if (selected.length > 8 || selected.some(file => file.size > 64000)) throw new Error('上下文最多 8 个文本文件，单文件不超过 64 KB。');
                const contextFiles = [];
                for (const file of selected) contextFiles.push({ path: file.webkitRelativePath || file.name, content: await file.text() });
                if (contextFiles.reduce((total, file) => total + file.content.length, 0) > 16000) throw new Error('上下文总计不能超过 16000 个字符。');
                payload = { title: fields.get('title'), technology: fields.get('technology'), requirement: fields.get('requirement'), standardId: fields.get('standardId'), contextFiles };
            } else if (mode === 'standard') {
                payload = { name: fields.get('name'), version: fields.get('version'), instructions: fields.get('instructions'),
                    requiredFiles: lines(fields.get('requiredFiles')), forbiddenTerms: lines(fields.get('forbiddenTerms')),
                    requiredChecks: lines(fields.get('requiredChecks')) };
            } else if (mode === 'confirm') {
                payload = { expectedRevision: task.revision, draft: { ...task.draft,
                    specification: fields.get('specification'), acceptanceCriteria: lines(fields.get('acceptanceCriteria')),
                    plannedFiles: lines(fields.get('plannedFiles')) }, answers: fields.getAll('answer') };
            } else {
                payload = { expectedRevision: task.revision, feedback: fields.get('feedback') || '',
                    decision: fields.get('decision') || '', riskAcknowledged: fields.has('riskAcknowledged'),
                    generationMode: mode === 'generate-project' ? 'project' : fields.get('generationMode') || 'change',
                    targetPath: fields.get('targetPath') || '' };
            }
            requestController = new AbortController();
            const cancel = document.getElementById('dev-cancel');
            if (cancel) cancel.hidden = false;
            buttons.forEach(({ button }) => { button.disabled = true; });
            progress.textContent = mode === 'verify' ? '隔离验证器处理中…' : ['clarify', 'generate', 'generate-project'].includes(mode) ? 'AI 处理中…' : '正在保存…';
            const url = new URL(location.href); url.hash = '';
            if (task) url.searchParams.set('operation', mode === 'generate-project' ? 'generate' : mode);
            const projectPaths = mode === 'generate-project' ? [...new Set([...task.draft.plannedFiles, ...task.standard.requiredFiles].map(path => path.toLowerCase()))] : [];
            const countMissing = snapshot => {
                const present = new Set((snapshot.changes.at(-1)?.files || snapshot.contextFiles).map(file => file.path.toLowerCase()));
                return projectPaths.filter(path => !present.has(path)).length;
            };
            let remaining = mode === 'generate-project' ? countMissing(task) : 0;
            do {
                timer = setTimeout(() => requestController?.abort(), 195000);
                const response = await fetch(url, { method: 'POST', headers: { 'Content-Type': 'application/json',
                    RequestVerificationToken: fields.get('__RequestVerificationToken') }, body: JSON.stringify(payload), signal: requestController.signal });
                clearTimeout(timer);
                if (!response.ok) {
                    const problem = await response.json().catch(() => ({}));
                    throw new Error(problem.error || `操作未成功 (${response.status})，请刷新确认状态后重试。`);
                }
                saved = await response.json();
                if (mode === 'generate-project') {
                    const nextRemaining = countMissing(saved);
                    if (nextRemaining >= remaining) throw new Error('本批未推进文件计划，已停止后续调用。');
                    remaining = nextRemaining;
                    payload.expectedRevision = saved.revision;
                    progress.textContent = `项目文件已保存 ${projectPaths.length - remaining} / ${projectPaths.length}`;
                }
            } while (mode === 'generate-project' && remaining > 0);
            if (mode === 'standard') location.reload();
            else {
                const destination = `/Development/Task/${saved.id}#${['materialize', 'generate-project'].includes(mode) ? 'project' : mode === 'verify' ? 'checks' : mode === 'generate' || mode === 'confirm' ? 'changes' : mode === 'review' ? 'versions' : 'requirements'}`;
                if (task) {
                    history.replaceState(null, '', destination);
                    location.reload();
                } else location.assign(destination);
            }
        } catch (error) {
            errorBox.textContent = error.name === 'AbortError' ? '操作取消或超时，请刷新确认服务端状态后重试。' : error.message;
            if (mode === 'generate-project' && saved) errorBox.textContent += ' 已完成批次已保存，请刷新后继续。';
            errorBox.hidden = false; progress.textContent = '';
        } finally {
            clearTimeout(timer); requestController = null;
            buttons.forEach(({ button, disabled }) => { button.disabled = disabled; });
            const cancel = document.getElementById('dev-cancel'); if (cancel) cancel.hidden = true;
        }
    });

    if (!task?.changes.length) return;
    const selector = document.getElementById('dev-change-select');
    function drawVersion() {
        const index = Number(selector.value);
        const change = task.changes[index];
        const baseline = index ? task.changes[index - 1].files : task.contextFiles;
        document.getElementById('dev-change-summary').textContent = change.summary;
        const list = document.getElementById('dev-file-list'); list.replaceChildren();
        function showFile(file, button) {
            for (const item of list.children) item.setAttribute('aria-pressed', String(item === button));
            document.getElementById('dev-file-name').textContent = file.path;
            document.getElementById('dev-source-before').textContent = baseline.find(item => item.path === file.path)?.content ?? '（新增文件）';
            document.getElementById('dev-source-after').textContent = file.content;
        }
        for (const file of change.files) {
            const previous = baseline.find(item => item.path === file.path);
            const button = document.createElement('button'); button.type = 'button';
            button.textContent = `${previous ? previous.content === file.content ? '=' : '~' : '+'} ${file.path}`;
            button.title = file.path; button.addEventListener('click', () => showFile(file, button)); list.append(button);
        }
        if (change.files.length) showFile(change.files[0], list.firstElementChild);
        const checks = document.getElementById('dev-checks'); checks.replaceChildren();
        const labels = { passed: '通过', failed: '未通过', 'not-run': '未运行', manual: '需人工核对' };
        const required = task.standard.requiredChecks || [];
        const reports = change.verifications || [];
        const report = reports.at(-1);
        const external = required.map(name => ({ ...(report?.checks.find(check => check.name === name) ||
            { status: 'not-run', detail: '该版本尚无有效外部验证报告。' }), name: `外部验证 / ${name}` }));
        document.getElementById('dev-verification-summary').textContent = required.length ?
            `必需验证 ${required.length} 项 · 已记录 ${reports.length}/5 次` : '此规范未要求外部验证';
        for (const check of [...change.checks, ...external]) {
            const row = document.createElement('div'); row.className = 'dev-check';
            const status = document.createElement('strong'); status.textContent = labels[check.status] || check.status;
            status.className = `check-${['passed', 'failed', 'not-run', 'manual'].includes(check.status) ? check.status : 'manual'}`;
            const text = document.createElement('div'); const name = document.createElement('b'); name.textContent = check.name;
            const detail = document.createElement('p'); detail.textContent = check.detail; text.append(name, detail); row.append(status, text); checks.append(row);
        }
        const review = document.getElementById('dev-review-form');
        if (review) {
            review.hidden = index !== task.changes.length - 1;
            review.querySelector('[value=accepted]').disabled = change.checks.some(check => check.status === 'failed') ||
                external.some(check => check.status !== 'passed');
        }
        const verify = document.getElementById('dev-verify-form');
        if (verify) verify.hidden = index !== task.changes.length - 1;
        const history = document.getElementById('dev-verification-history'); history.replaceChildren();
        if (reports.length) {
            const heading = document.createElement('h3'); heading.textContent = '外部验证记录'; history.append(heading);
            for (const item of [...reports].reverse()) {
                const details = document.createElement('details'); details.className = 'dev-standard';
                const summary = document.createElement('summary');
                summary.textContent = `${new Date(item.receivedAt).toLocaleString()} · ${item.runId}`;
                const list = document.createElement('ul');
                for (const check of item.checks) {
                    const entry = document.createElement('li');
                    entry.textContent = `${check.name}: ${labels[check.status] || check.status} · ${check.detail}`;
                    list.append(entry);
                }
                details.append(summary, list); history.append(details);
            }
        }
    }
    selector.addEventListener('change', drawVersion); drawVersion();
})();