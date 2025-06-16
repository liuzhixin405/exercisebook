console.log("Script loaded and starting initialization.");

import { 
    createNewProject as apiCreateNewProject, 
    listFiles as apiListFiles, 
    uploadFiles as apiUploadFiles,
    deleteFile as apiDeleteFile,
    deleteAllFiles as apiDeleteAllFiles,
    readFileContent as apiReadFileContent,
    askQuestionStream as apiAskQuestionStream,
    checkHealth as apiCheckHealth
} from './api.js';

let eventSource = null; // No longer used for main streaming, but kept for clarity if needed elsewhere
let isStreaming = false;
let selectedFiles = new Set(); // Stores full paths of selected files
let currentProjectPath = ''; // Stores the current project path, relative to the backend's base path
let globalFileTree = {}; // Stores the entire file tree for the current project
let currentReader = null; // Store the reader to abort it on stopStream

// UI elements (defined globally for easier access in event handlers)
const questionInput = document.getElementById('questionInput');
const askBtn = document.getElementById('askBtn');
const stopBtn = document.getElementById('stopBtn');
const clearBtn = document.getElementById('clearBtn');
const healthBtn = document.getElementById('healthBtn');
const deepModeCheckbox = document.getElementById('deepModeCheckbox');
const fileInput = document.getElementById('fileInput');
const projectPathDisplay = document.getElementById('project-path');
const fileList = document.getElementById('file-list');
const goUpBtn = document.getElementById('goUpBtn');

function showLoading(show, message = '处理中...') {
    const loadingDiv = document.getElementById('loading');
    if (show) {
        loadingDiv.innerText = message;
        loadingDiv.style.display = 'block';
    } else {
        loadingDiv.style.display = 'none';
    }
}

function updateProjectPathDisplay() {
    projectPathDisplay.textContent = currentProjectPath || '/';
}

function renderFileList(fileTree, currentPath) {
    fileList.innerHTML = '';
    const pathParts = currentPath.split('/').filter(p => p);
    let currentNode = fileTree;
    pathParts.forEach(part => {
        // Ensure currentNode and its children are valid before searching
        if (currentNode && currentNode.children) {
            currentNode = currentNode.children.find(child => child.name === part && child.type === 'directory');
        } else {
            currentNode = null; // Path part not found, break out
        }
    });

    if (currentNode && currentNode.children) {
        // Sort directories first, then files
        const sortedChildren = [...currentNode.children].sort((a, b) => {
            if (a.type === 'directory' && b.type !== 'directory') return -1;
            if (a.type !== 'directory' && b.type === 'directory') return 1;
            return a.name.localeCompare(b.name);
        });

        sortedChildren.forEach(item => {
            const li = document.createElement('li');
            li.className = 'file-list-item';
            li.dataset.path = item.path;
            li.dataset.type = item.type;

            const checkbox = document.createElement('input');
            checkbox.type = 'checkbox';
            checkbox.checked = selectedFiles.has(item.path);
            checkbox.onchange = (e) => toggleFileSelection(item.path, item.type === 'directory', e.target.checked);
            li.appendChild(checkbox);

            const icon = document.createElement('span');
            icon.className = item.type === 'directory' ? 'folder-icon' : 'file-icon';
            icon.innerHTML = item.type === 'directory' ? '&#128193;' : '&#128196;'; // Folder or file emoji
            li.appendChild(icon);

            const text = document.createElement('span');
            text.textContent = item.name;
            li.appendChild(text);

            if (item.type === 'directory') {
                li.ondblclick = () => navigateFolder(item.path);
            } else {
                const viewBtn = document.createElement('button');
                viewBtn.textContent = '查看';
                viewBtn.style.marginLeft = 'auto';
                viewBtn.onclick = (e) => {
                    e.stopPropagation(); // Prevent li click when button is clicked
                    apiReadFileContent(item.path, showLoading);
                };
                li.appendChild(viewBtn);

                const deleteBtn = document.createElement('button');
                deleteBtn.textContent = '删除';
                deleteBtn.style.marginLeft = '5px';
                deleteBtn.onclick = (e) => {
                    e.stopPropagation(); // Prevent li click when button is clicked
                    apiDeleteFile(item.path, {value: currentProjectPath}, apiListFiles, showLoading, {value: globalFileTree}, updateProjectPathDisplay, renderFileList);
                };
                li.appendChild(deleteBtn);
            }
            fileList.appendChild(li);
        });
    }
}

function navigateFolder(folderPath) {
    currentProjectPath = folderPath;
    apiListFiles(currentProjectPath, showLoading, {value: globalFileTree}, {value: currentProjectPath}, updateProjectPathDisplay, renderFileList);
}

function createNewFolder() {
    const folderName = prompt("请输入新文件夹的名称 (相对当前路径):");
    if (folderName) {
        const fullPath = currentProjectPath ? `${currentProjectPath}/${folderName}` : folderName;
        apiCreateNewProject(fullPath, {value: currentProjectPath}, apiListFiles, showLoading, {value: globalFileTree}, updateProjectPathDisplay, renderFileList);
    }
}

// NOTE: This function's `createNewFolder` call should be an API call, not directly defined here.
// The `createNewFolder` in api.js expects the full path as an argument.
// This function needs to be updated to pass the full path correctly.
// For now, I'm defining a placeholder in api.js and calling that.
// The actual `createNewFolder` in api.js needs to be implemented.
// As per the original plan, the `createNewProject` function (which creates a top-level folder)
// is handled by the API, so this `createNewFolder` will also be an API call.
async function apiCreateNewFolder(folderPath, showLoadingFunc, currentProjectPathRef, listFilesFunc, globalFileTreeRef, updateProjectPathDisplayFunc, renderFileListFunc) {
    showLoadingFunc(true, '正在创建文件夹...');
    try {
        const response = await fetch(`${backendBaseUrl}/api/file/create-folder`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ folderName: folderPath })
        });
        if (response.ok) {
            alert(`文件夹 "${folderPath}" 创建成功！`);
            await listFilesFunc(currentProjectPathRef.value, showLoadingFunc, globalFileTreeRef, currentProjectPathRef, updateProjectPathDisplayFunc, renderFileListFunc); // Refresh file list
        } else {
            const errorText = await response.text();
            alert(`创建文件夹失败: ${errorText}`);
        }
    } catch (error) {
        console.error("创建文件夹时出错:", error);
        alert("创建文件夹时发生网络错误或服务器错误。");
    } finally {
        showLoadingFunc(false);
    }
}

function handleFileSelection(event) {
    const files = event.target.files;
    if (files.length > 0) {
        console.log(`选择了 ${files.length} 个文件/文件夹`);
        // 这里只是为了调试，实际上传在 uploadFiles 函数中
    }
}

function toggleFileSelection(filePath, isDirectory, isSelected) {
    if (isSelected) {
        selectedFiles.add(filePath);
    } else {
        selectedFiles.delete(filePath);
    }
    console.log("当前选中文件:", Array.from(selectedFiles));
}

// Event Listeners
document.addEventListener('DOMContentLoaded', async () => {
    // Buttons
    askBtn.onclick = () => apiAskQuestionStream(questionInput.value.trim(), selectedFiles, deepModeCheckbox.checked, showLoading, {value: isStreaming}, {value: currentReader}, askBtn, stopBtn, questionInput);
    stopBtn.onclick = () => {
        if (currentReader) {
            currentReader.cancel();
            console.log("请求已中止。");
            showLoading(false);
            isStreaming = false;
            askBtn.disabled = false;
            stopBtn.disabled = true;
            questionInput.disabled = false;
            currentReader = null;
        }
    };
    clearBtn.onclick = () => {
        document.getElementById('response').innerHTML = '等待提问...';
        questionInput.value = '';
        selectedFiles.clear(); // Clear selected files
        // Uncheck all file checkboxes
        document.querySelectorAll('#file-list input[type="checkbox"]').forEach(checkbox => {
            checkbox.checked = false;
        });
    };
    healthBtn.onclick = () => apiCheckHealth(showLoading);

    // File Management Buttons
    document.querySelector('.file-actions button:nth-child(1)').onclick = () => { // New Project Button
        const projectName = prompt("请输入新项目的名称:");
        if (projectName) {
            apiCreateNewProject(projectName, {value: currentProjectPath}, apiListFiles, showLoading, {value: globalFileTree}, updateProjectPathDisplay, renderFileList);
        }
    };
    document.querySelector('.file-actions button:nth-child(3)').onclick = () => apiUploadFiles(fileInput.files, {value: currentProjectPath}, apiListFiles, showLoading, {value: globalFileTree}, updateProjectPathDisplay, renderFileList); // Upload Button
    document.querySelector('#file-management-container > button').onclick = () => apiDeleteAllFiles({value: currentProjectPath}, {value: selectedFiles}, apiListFiles, showLoading, {value: globalFileTree}, updateProjectPathDisplay, renderFileList); // Delete All Button
    goUpBtn.onclick = () => {
        const pathParts = currentProjectPath.split('/').filter(p => p);
        if (pathParts.length > 0) {
            pathParts.pop(); // Remove the last part
            currentProjectPath = pathParts.join('/');
            apiListFiles(currentProjectPath, showLoading, {value: globalFileTree}, {value: currentProjectPath}, updateProjectPathDisplay, renderFileList);
        } else {
            alert('已经在根目录。');
        }
    };
    
    // Initial file listing
    await apiListFiles(currentProjectPath, showLoading, {value: globalFileTree}, {value: currentProjectPath}, updateProjectPathDisplay, renderFileList);
}); 