// 页面设计器 JavaScript
class PageDesigner {
    constructor() {
        this.selectedComponent = null;
        this.components = [];
        this.history = [];
        this.historyIndex = -1;

        this.init();
    }

    init() {
        this.setupEventListeners();
        this.setupDragAndDrop();
        this.loadPage();
        this.saveToHistory();
    }

    setupEventListeners() {
        // 保存按钮
        document.getElementById('saveBtn')?.addEventListener('click', () => this.savePage());

        // 预览按钮
        document.getElementById('previewBtn')?.addEventListener('click', () => this.previewPage());

        // 发布按钮
        document.getElementById('publishBtn')?.addEventListener('click', () => this.publishPage());

        // 工具栏按钮
        document.getElementById('undoBtn')?.addEventListener('click', () => this.undo());
        document.getElementById('redoBtn')?.addEventListener('click', () => this.redo());
        document.getElementById('copyBtn')?.addEventListener('click', () => this.copy());
        document.getElementById('pasteBtn')?.addEventListener('click', () => this.paste());
        document.getElementById('deleteBtn')?.addEventListener('click', () => this.delete());
        document.getElementById('aiForm')?.addEventListener('submit', (event) => {
            event.preventDefault();
            this.generateDraft();
        });
        document.getElementById('aiCancelBtn')?.addEventListener('click', () => this.aiRequest?.abort());
        document.getElementById('aiApplyBtn')?.addEventListener('click', () => this.applyDraft());
        document.getElementById('aiPreviewBtn')?.addEventListener('click', () => {
            if (this.pendingDraft) this.previewPage(this.buildDraftComponents(this.pendingDraft));
        });
        document.getElementById('closePreviewBtn')?.addEventListener('click', () => document.getElementById('previewDialog').close());
    }

    setupDragAndDrop() {
        const canvasArea = document.getElementById('canvasArea');
        const componentItems = document.querySelectorAll('.component-item');

        // 设置组件拖拽
        componentItems.forEach(item => {
            item.addEventListener('dragstart', (e) => {
                e.dataTransfer.setData('text/plain', item.dataset.component);
                e.dataTransfer.effectAllowed = 'copy';
            });
        });

        // 设置画布拖放
        canvasArea.addEventListener('dragover', (e) => {
            e.preventDefault();
            e.dataTransfer.dropEffect = 'copy';
            canvasArea.classList.add('drag-over');
        });

        canvasArea.addEventListener('dragleave', (e) => {
            canvasArea.classList.remove('drag-over');
        });

        canvasArea.addEventListener('drop', (e) => {
            e.preventDefault();
            canvasArea.classList.remove('drag-over');

            const componentType = e.dataTransfer.getData('text/plain');
            this.addComponent(componentType, e.offsetX, e.offsetY);
        });
    }

    addComponent(type, x, y) {
        const component = this.createComponent(type, x, y);
        this.components.push(component);
        this.renderComponent(component);
        this.saveToHistory();
    }

    createComponent(type, x, y) {
        const id = 'comp_' + crypto.randomUUID();
        const component = {
            id: id,
            type: type,
            x: x,
            y: y,
            width: type === 'input' ? 240 : 160,
            height: 48,
            properties: this.getDefaultProperties(type),
            styles: this.getDefaultStyles(type)
        };
        return component;
    }

    getDefaultProperties(type) {
        const defaults = {
            text: { content: '文本内容', fontSize: 14, color: '#000000' },
            button: { text: '按钮', type: 'button', color: '#007bff' },
            input: { placeholder: '请输入...', type: 'text' },
            image: { src: '', alt: '图片' },
            container: { padding: 10, backgroundColor: '#ffffff' },
            row: { direction: 'horizontal', gap: 10 },
            column: { width: '100%', flex: 1 }
        };
        return defaults[type] || {};
    }

    getDefaultStyles(type) {
        const defaults = {
            text: { fontSize: '14px', color: '#000000', fontFamily: 'Arial' },
            button: {
                backgroundColor: '#007bff',
                color: '#ffffff',
                border: 'none',
                padding: '8px 16px',
                borderRadius: '4px',
                cursor: 'pointer'
            },
            input: {
                padding: '8px',
                border: '1px solid #ccc',
                borderRadius: '4px',
                width: '200px'
            },
            image: { maxWidth: '100%', height: 'auto' },
            container: {
                padding: '10px',
                backgroundColor: '#ffffff',
                border: '1px solid #ddd',
                borderRadius: '4px'
            },
            row: { display: 'flex', gap: '10px', flexDirection: 'row' },
            column: { flex: 1, padding: '5px' }
        };
        return defaults[type] || {};
    }

    renderComponent(component, canvasArea = document.getElementById('canvasArea'), interactive = true) {
        const dropZone = canvasArea.querySelector('.canvas-drop-zone');

        if (dropZone) {
            dropZone.style.display = 'none';
        }

        const element = document.createElement('div');
        element.className = interactive ? 'designer-component' : 'preview-component';
        if (interactive) element.id = component.id;
        element.style.position = 'absolute';
        element.style.left = component.x + 'px';
        element.style.top = component.y + 'px';
        element.style.width = component.width + 'px';
        element.style.height = component.height + 'px';
        element.style.border = '1px solid #ccc';
        element.style.cursor = interactive ? 'move' : 'default';
        element.style.userSelect = 'none';

        // 应用样式
        Object.assign(element.style, component.styles);

        // 设置内容
        this.setComponentContent(element, component);

        // 添加事件监听器
        if (interactive) element.addEventListener('click', (e) => {
            e.stopPropagation();
            this.selectComponent(component);
        });

        if (interactive) element.addEventListener('mousedown', (e) => {
            this.startDrag(component, e);
        });

        canvasArea.appendChild(element);
    }

    setComponentContent(element, component) {
        element.replaceChildren();
        switch (component.type) {
            case 'text':
                element.textContent = component.properties.content;
                break;
            case 'button':
                const button = document.createElement('button');
                button.textContent = component.properties.text;
                button.type = 'button';
                button.className = 'component-button';
                element.appendChild(button);
                break;
            case 'input':
                const input = document.createElement('input');
                input.type = ['text', 'email', 'password', 'number', 'date', 'tel'].includes(component.properties.type) ? component.properties.type : 'text';
                input.placeholder = component.properties.placeholder;
                input.setAttribute('aria-label', component.properties.placeholder || '输入框');
                input.className = 'component-input';
                element.appendChild(input);
                break;
            case 'image':
                const image = document.createElement('img');
                const source = new URL(component.properties.src || '/favicon.ico', window.location.origin);
                if (['http:', 'https:'].includes(source.protocol)) image.src = source.href;
                image.alt = component.properties.alt;
                image.style.maxWidth = '100%';
                image.style.maxHeight = '100%';
                element.appendChild(image);
                break;
            case 'container':
                element.className += ' container-component';
                break;
            case 'row':
                element.className += ' row-component';
                break;
            case 'column':
                element.className += ' column-component';
                break;
        }
    }

    selectComponent(component) {
        // 移除之前的选中状态
        document.querySelectorAll('.designer-component').forEach(el => {
            el.classList.remove('selected');
        });

        // 选中当前组件
        const element = document.getElementById(component.id);
        if (element) {
            element.classList.add('selected');
            this.selectedComponent = component;
            this.showProperties(component);
        }
    }

    showProperties(component) {
        const propertiesContent = document.getElementById('propertiesContent');
        propertiesContent.innerHTML = `
            <h4>${this.getComponentDisplayName(component.type)}</h4>
            <div class="property-group">
                <label>位置 X:</label>
                <input type="number" id="propX" value="${component.x}" onchange="designer.updateProperty('x', this.value)">
            </div>
            <div class="property-group">
                <label>位置 Y:</label>
                <input type="number" id="propY" value="${component.y}" onchange="designer.updateProperty('y', this.value)">
            </div>
            <div class="property-group">
                <label>宽度:</label>
                <input type="number" id="propWidth" value="${component.width}" onchange="designer.updateProperty('width', this.value)">
            </div>
            <div class="property-group">
                <label>高度:</label>
                <input type="number" id="propHeight" value="${component.height}" onchange="designer.updateProperty('height', this.value)">
            </div>
            ${this.getComponentSpecificProperties(component)}
        `;
    }

    getComponentDisplayName(type) {
        const names = {
            text: '文本',
            button: '按钮',
            input: '输入框',
            image: '图片',
            container: '容器',
            row: '行',
            column: '列'
        };
        return names[type] || type;
    }

    getComponentSpecificProperties(component) {
        const properties = Object.fromEntries(Object.entries(component.properties).map(([key, value]) => [key, this.escapeAttribute(value)]));
        switch (component.type) {
            case 'text':
                return `
                    <div class="property-group">
                        <label>内容:</label>
                        <input type="text" id="propContent" value="${properties.content}" onchange="designer.updateComponentProperty('content', this.value)">
                    </div>
                    <div class="property-group">
                        <label>字体大小:</label>
                        <input type="number" id="propFontSize" value="${properties.fontSize}" onchange="designer.updateComponentProperty('fontSize', this.value)">
                    </div>
                    <div class="property-group">
                        <label>颜色:</label>
                        <input type="color" id="propColor" value="${properties.color}" onchange="designer.updateComponentProperty('color', this.value)">
                    </div>
                `;
            case 'button':
                return `
                    <div class="property-group">
                        <label>文本:</label>
                        <input type="text" id="propText" value="${properties.text}" onchange="designer.updateComponentProperty('text', this.value)">
                    </div>
                    <div class="property-group">
                        <label>类型:</label>
                        <select id="propType" onchange="designer.updateComponentProperty('type', this.value)">
                            <option value="button" ${component.properties.type === 'button' ? 'selected' : ''}>按钮</option>
                            <option value="submit" ${component.properties.type === 'submit' ? 'selected' : ''}>提交</option>
                            <option value="reset" ${component.properties.type === 'reset' ? 'selected' : ''}>重置</option>
                        </select>
                    </div>
                `;
            case 'input':
                return `
                    <div class="property-group">
                        <label>占位符:</label>
                        <input type="text" id="propPlaceholder" value="${properties.placeholder}" onchange="designer.updateComponentProperty('placeholder', this.value)">
                    </div>
                    <div class="property-group">
                        <label>类型:</label>
                        <select id="propInputType" onchange="designer.updateComponentProperty('type', this.value)">
                            <option value="text" ${component.properties.type === 'text' ? 'selected' : ''}>文本</option>
                            <option value="email" ${component.properties.type === 'email' ? 'selected' : ''}>邮箱</option>
                            <option value="password" ${component.properties.type === 'password' ? 'selected' : ''}>密码</option>
                            <option value="number" ${component.properties.type === 'number' ? 'selected' : ''}>数字</option>
                            <option value="date" ${component.properties.type === 'date' ? 'selected' : ''}>日期</option>
                            <option value="tel" ${component.properties.type === 'tel' ? 'selected' : ''}>电话</option>
                        </select>
                    </div>
                `;
            case 'image':
                return `
                    <div class="property-group">
                        <label>图片地址:</label>
                        <input type="url" id="propSrc" value="${properties.src}" onchange="designer.updateComponentProperty('src', this.value)">
                    </div>
                    <div class="property-group">
                        <label>替代文本:</label>
                        <input type="text" id="propAlt" value="${properties.alt}" onchange="designer.updateComponentProperty('alt', this.value)">
                    </div>
                `;
            default:
                return '';
        }
    }

    updateProperty(property, value) {
        const parsed = Number(value);
        if (this.selectedComponent && Number.isFinite(parsed)) {
            this.selectedComponent[property] = Math.max(property === 'width' || property === 'height' ? 30 : 0, Math.min(2400, parsed));
            this.updateComponentElement();
            this.saveToHistory();
        }
    }

    updateComponentProperty(property, value) {
        if (this.selectedComponent) {
            this.selectedComponent.properties[property] = value;
            if (property === 'fontSize') this.selectedComponent.styles.fontSize = Math.max(10, Math.min(48, Number(value) || 14)) + 'px';
            if (property === 'color') this.selectedComponent.styles.color = value;
            this.updateComponentElement();
            this.saveToHistory();
        }
    }

    updateComponentElement() {
        if (this.selectedComponent) {
            const element = document.getElementById(this.selectedComponent.id);
            if (element) {
                element.style.left = this.selectedComponent.x + 'px';
                element.style.top = this.selectedComponent.y + 'px';
                element.style.width = this.selectedComponent.width + 'px';
                element.style.height = this.selectedComponent.height + 'px';
                Object.assign(element.style, this.selectedComponent.styles);

                // 更新内容
                this.setComponentContent(element, this.selectedComponent);
            }
        }
    }

    startDrag(component, e) {
        const startX = e.clientX;
        const startY = e.clientY;
        const startLeft = component.x;
        const startTop = component.y;

        const handleMouseMove = (e) => {
            const deltaX = e.clientX - startX;
            const deltaY = e.clientY - startY;

            component.x = Math.max(0, startLeft + deltaX);
            component.y = Math.max(0, startTop + deltaY);

            const element = document.getElementById(component.id);
            if (element) {
                element.style.left = component.x + 'px';
                element.style.top = component.y + 'px';
            }
        };

        const handleMouseUp = () => {
            document.removeEventListener('mousemove', handleMouseMove);
            document.removeEventListener('mouseup', handleMouseUp);
            this.saveToHistory();
        };

        document.addEventListener('mousemove', handleMouseMove);
        document.addEventListener('mouseup', handleMouseUp);
    }

    saveToHistory() {
        this.historyIndex++;
        this.history = this.history.slice(0, this.historyIndex);
        this.history.push(JSON.parse(JSON.stringify(this.components)));
        if (this.history.length > 50) {
            this.history.shift();
            this.historyIndex--;
        }
        document.getElementById('undoBtn').disabled = this.historyIndex <= 0;
        document.getElementById('redoBtn').disabled = true;
    }

    undo() {
        if (this.historyIndex > 0) {
            this.historyIndex--;
            this.components = JSON.parse(JSON.stringify(this.history[this.historyIndex]));
            this.renderAllComponents();
            this.updateHistoryButtons();
        }
    }

    redo() {
        if (this.historyIndex < this.history.length - 1) {
            this.historyIndex++;
            this.components = JSON.parse(JSON.stringify(this.history[this.historyIndex]));
            this.renderAllComponents();
            this.updateHistoryButtons();
        }
    }

    renderAllComponents() {
        this.selectedComponent = null;
        document.getElementById('propertiesContent').textContent = '选择一个组件来编辑其属性';
        const canvasArea = document.getElementById('canvasArea');
        canvasArea.innerHTML = '<div class="canvas-drop-zone"><p>拖拽组件到此处开始设计页面</p></div>';

        this.components.forEach(component => {
            this.renderComponent(component);
        });
    }

    copy() {
        if (this.selectedComponent) {
            this.copiedComponent = JSON.parse(JSON.stringify(this.selectedComponent));
        }
    }

    paste() {
        if (this.copiedComponent) {
            const newComponent = JSON.parse(JSON.stringify(this.copiedComponent));
            newComponent.id = 'comp_' + crypto.randomUUID();
            newComponent.x += 20;
            newComponent.y += 20;

            this.components.push(newComponent);
            this.renderComponent(newComponent);
            this.saveToHistory();
        }
    }

    delete() {
        if (this.selectedComponent) {
            const index = this.components.findIndex(c => c.id === this.selectedComponent.id);
            if (index > -1) {
                this.components.splice(index, 1);
                const element = document.getElementById(this.selectedComponent.id);
                if (element) {
                    element.remove();
                }
                this.selectedComponent = null;
                document.getElementById('propertiesContent').innerHTML = '<p>选择一个组件来编辑其属性</p>';
                this.saveToHistory();
                this.renderAllComponents();
            }
        }
    }

    savePage() {
        try {
            localStorage.setItem(this.storageKey(), JSON.stringify({ version: 1, components: this.components }));
            this.showStatus('草稿已保存至当前浏览器。');
        } catch {
            this.showStatus('草稿保存失败，请检查浏览器存储空间或权限。');
        }
    }

    previewPage(components = this.components) {
        const canvas = document.getElementById('previewCanvas');
        canvas.replaceChildren();
        canvas.style.width = Math.max(320, ...components.map(component => component.x + component.width + 24)) + 'px';
        canvas.style.height = Math.max(400, ...components.map(component => component.y + component.height + 24)) + 'px';
        components.forEach(component => this.renderComponent(component, canvas, false));
        document.getElementById('previewDialog').showModal();
    }

    storageKey() {
        const container = document.querySelector('.designer-container');
        return `lowcode:draft:${container.dataset.applicationId || 'local'}:${container.dataset.pageId || 'new'}`;
    }

    loadPage() {
        try {
            const saved = JSON.parse(localStorage.getItem(this.storageKey()) || 'null');
            if (!saved) return;
            if (saved.version !== 1 || !Array.isArray(saved.components) || saved.components.length > 200) throw new Error('Invalid draft');
            const types = ['text', 'button', 'input', 'image', 'container', 'row', 'column'];
            this.components = saved.components.map(component => {
                if (!component || !types.includes(component.type) || !['x', 'y', 'width', 'height'].every(key => Number.isFinite(component[key]) && component[key] >= 0 && component[key] <= 10000)) throw new Error('Invalid component');
                const restored = this.createComponent(component.type, component.x, component.y);
                restored.width = component.width;
                restored.height = component.height;
                Object.keys(restored.properties).forEach(key => {
                    if (typeof component.properties?.[key] === 'string' || typeof component.properties?.[key] === 'number') restored.properties[key] = component.properties[key];
                });
                if (Number.isFinite(Number(restored.properties.fontSize))) restored.styles.fontSize = Math.max(10, Math.min(48, Number(restored.properties.fontSize))) + 'px';
                if (/^#[0-9a-f]{6}$/i.test(restored.properties.color || '')) restored.styles.color = restored.properties.color;
                return restored;
            });
            this.renderAllComponents();
            this.showStatus('已恢复当前浏览器的草稿。');
        } catch {
            this.components = [];
            this.showStatus('无法恢复草稿，原存储内容未修改。');
        }
    }

    showStatus(message) {
        document.getElementById('designerStatus').textContent = message;
    }

    escapeAttribute(value) {
        return String(value ?? '').replace(/[&<>"']/g, character => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' })[character]);
    }

    updateHistoryButtons() {
        document.getElementById('undoBtn').disabled = this.historyIndex <= 0;
        document.getElementById('redoBtn').disabled = this.historyIndex >= this.history.length - 1;
    }

    async generateDraft() {
        if (this.aiRequest) return;
        const form = document.getElementById('aiForm');
        const status = document.getElementById('aiStatus');
        this.pendingDraft = null;
        document.getElementById('aiDraft').hidden = true;
        document.getElementById('aiGenerateBtn').disabled = true;
        document.getElementById('aiCancelBtn').disabled = false;
        form.setAttribute('aria-busy', 'true');
        this.aiRequest = new AbortController();
        const timeout = setTimeout(() => this.aiRequest?.abort(), 100000);
        status.textContent = '正在生成…';
        try {
            const response = await fetch(form.action, { method: 'POST', body: new FormData(form), signal: this.aiRequest.signal });
            const result = await response.json().catch(() => null);
            if (!response.ok) throw new Error(result?.error || `请求失败 (${response.status})，请刷新页面后重试。`);
            if (!result || typeof result.title !== 'string' || !Array.isArray(result.components) || !result.components.length || result.components.length > 18 ||
                result.components.some(component => !component || !['text', 'input', 'button'].includes(component.type) || typeof component.text !== 'string' || !component.text.trim() || component.text.length > 160 || !['text', 'email', 'number', 'date', 'tel'].includes(component.inputType))) throw new Error('模型输出无效。');
            this.pendingDraft = result;
            document.getElementById('aiDraftTitle').textContent = result.title;
            const summary = document.getElementById('aiDraftSummary');
            summary.replaceChildren();
            result.components.forEach(component => {
                const item = document.createElement('li');
                item.textContent = `${this.getComponentDisplayName(component.type)}：${component.text}`;
                summary.appendChild(item);
            });
            document.getElementById('aiDraft').hidden = false;
            status.textContent = `草稿已就绪，${result.components.length} 个组件。`;
        } catch (error) {
            status.textContent = error.name === 'AbortError' ? '生成已取消或超时。' : error.message;
        } finally {
            clearTimeout(timeout);
            this.aiRequest = null;
            form.removeAttribute('aria-busy');
            document.getElementById('aiGenerateBtn').disabled = false;
            document.getElementById('aiCancelBtn').disabled = true;
        }
    }

    buildDraftComponents(draft) {
        const width = Math.max(240, Math.min(520, document.getElementById('canvasArea').clientWidth - 56));
        let offset = 24;
        return draft.components.map(item => {
            const component = this.createComponent(item.type, 24, offset);
            component.width = width;
            component.height = item.type === 'text' ? 64 : 44;
            if (item.type === 'text') component.properties.content = item.text;
            if (item.type === 'button') component.properties.text = item.text;
            if (item.type === 'input') {
                component.properties.placeholder = item.text;
                component.properties.type = item.inputType;
            }
            offset += component.height + 12;
            return component;
        });
    }

    applyDraft() {
        if (!this.pendingDraft) return;
        if (this.components.length && !window.confirm('替换当前画布？可使用撤销恢复。')) return;
        this.components = this.buildDraftComponents(this.pendingDraft);
        this.renderAllComponents();
        this.saveToHistory();
        this.showStatus('AI 草稿已应用，尚未保存。');
    }

    getLayoutData() {
        return {
            width: 1200,
            height: 800,
            backgroundColor: '#ffffff'
        };
    }
}

// 初始化设计器
let designer;
document.addEventListener('DOMContentLoaded', () => {
    designer = new PageDesigner();
});
