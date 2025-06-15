// OllamaContext7Api/wwwroot/api.js

const backendBaseUrl = ''; // If your backend is at a different origin, specify it here.

export async function createNewProject(projectName, currentProjectPathRef, listFilesFunc, showLoadingFunc, globalFileTreeRef, updateProjectPathDisplayFunc, renderFileListFunc) {
    console.log("createNewProject called");
    if (projectName) {
        try {
            const response = await fetch(`${backendBaseUrl}/api/file/create-folder`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ folderName: projectName })
            });
            if (response.ok) {
                alert(`项目 "${projectName}" 创建成功！`);
                currentProjectPathRef.value = projectName; // Update current project
                await listFilesFunc(currentProjectPathRef.value, showLoadingFunc, globalFileTreeRef, currentProjectPathRef, updateProjectPathDisplayFunc, renderFileListFunc); // List files in the new project
            } else {
                const errorText = await response.text();
                alert(`创建项目失败: ${errorText}`);
            }
        } catch (error) {
            console.error("创建项目时出错:", error);
            alert("创建项目时发生网络错误或服务器错误。");
        }
    }
}

export async function listFiles(path = '', showLoadingFunc, globalFileTreeRef, currentProjectPathRef, updateProjectPathDisplayFunc, renderFileListFunc) {
    console.log(`listFiles called for path: ${path}`);
    showLoadingFunc(true, '正在加载文件...');
    try {
        const response = await fetch(`${backendBaseUrl}/api/file/list?path=${encodeURIComponent(path)}`);
        if (response.ok) {
            const files = await response.json();
            globalFileTreeRef.value = files; // Directly store the FileExplorerItem object
            currentProjectPathRef.value = path; // Update current path
            updateProjectPathDisplayFunc();
            renderFileListFunc(globalFileTreeRef.value, currentProjectPathRef.value); // Render from the tree
        } else {
            const errorText = await response.text();
            console.error("获取文件列表失败:", errorText);
            alert(`获取文件列表失败: ${errorText}`);
        }
    } catch (error) {
        console.error("获取文件列表时出错:", error);
        alert("获取文件列表时发生网络错误或服务器错误。");
    } finally {
        showLoadingFunc(false);
    }
}

export async function uploadFiles(filesToUpload, currentProjectPathRef, listFilesFunc, showLoadingFunc, globalFileTreeRef, updateProjectPathDisplayFunc, renderFileListFunc, buildFileTreeFunc) {
    console.log("uploadFiles called");
    if (filesToUpload.length === 0) {
        alert("请先选择文件或文件夹进行上传。");
        return;
    }

    showLoadingFunc(true, '正在上传文件...');
    try {
        const formData = new FormData();
        let filesCount = 0;
        for (let i = 0; i < filesToUpload.length; i++) {
            const file = filesToUpload[i];
            const relativePath = file.webkitRelativePath || file.name;
            const targetPath = currentProjectPathRef.value ? `${currentProjectPathRef.value}/${relativePath}` : relativePath;
            formData.append('files', file, targetPath);
            filesCount++;
        }

        if (filesCount === 0) {
            alert("没有文件可供上传。");
            showLoadingFunc(false);
            return;
        }

        const response = await fetch(`${backendBaseUrl}/api/file/upload`, {
            method: 'POST',
            body: formData
        });

        if (response.ok) {
            alert(`成功上传 ${filesCount} 个文件！`);
            await listFilesFunc(currentProjectPathRef.value, showLoadingFunc, globalFileTreeRef, currentProjectPathRef, updateProjectPathDisplayFunc, renderFileListFunc); // Refresh file list
        } else {
            const errorText = await response.text();
            alert(`上传文件失败: ${errorText}`);
        }
    } catch (error) {
        console.error("上传文件时出错:", error);
        alert("上传文件时发生网络错误或服务器错误。");
    } finally {
        showLoadingFunc(false);
    }
}

export async function deleteFile(filePath, currentProjectPathRef, listFilesFunc, showLoadingFunc, globalFileTreeRef, updateProjectPathDisplayFunc, renderFileListFunc, buildFileTreeFunc) {
    if (!confirm(`确定要删除文件 "${filePath}" 吗？`)) {
        return;
    }
    showLoadingFunc(true, '正在删除文件...');
    try {
        const response = await fetch(`${backendBaseUrl}/api/file/delete?filePath=${encodeURIComponent(filePath)}`, {
            method: 'DELETE'
        });
        if (response.ok) {
            alert(`文件 "${filePath}" 已删除。`);
            await listFilesFunc(currentProjectPathRef.value, showLoadingFunc, globalFileTreeRef, currentProjectPathRef, updateProjectPathDisplayFunc, renderFileListFunc); // Refresh file list
        } else {
            const errorText = await response.text();
            alert(`删除文件失败: ${errorText}`);
        }
    } catch (error) {
        console.error("删除文件时出错:", error);
        alert("删除文件时发生网络错误或服务器错误。");
    } finally {
        showLoadingFunc(false);
    }
}

export async function deleteAllFiles(currentProjectPathRef, selectedFilesRef, listFilesFunc, showLoadingFunc, globalFileTreeRef, updateProjectPathDisplayFunc, renderFileListFunc, buildFileTreeFunc) {
    if (!confirm(`确定要删除当前项目 "${currentProjectPathRef.value}" 下的所有文件吗？`)) {
        return;
    }
    showLoadingFunc(true, '正在删除所有文件...');
    try {
        const response = await fetch(`${backendBaseUrl}/api/file/delete-all?path=${encodeURIComponent(currentProjectPathRef.value)}`, {
            method: 'DELETE'
        });
        if (response.ok) {
            alert(`当前项目 "${currentProjectPathRef.value}" 下的所有文件已删除。`);
            selectedFilesRef.value.clear(); // Clear selected files as well
            await listFilesFunc(currentProjectPathRef.value, showLoadingFunc, globalFileTreeRef, currentProjectPathRef, updateProjectPathDisplayFunc, renderFileListFunc); // Refresh file list
        } else {
            const errorText = await response.text();
            alert(`删除所有文件失败: ${errorText}`);
        }
    } catch (error) {
        console.error("删除所有文件时出错:", error);
        alert("删除所有文件时发生网络错误或服务器错误。");
    } finally {
        showLoadingFunc(false);
    }
}

export async function readFileContent(filePath, showLoadingFunc) {
    showLoadingFunc(true, `正在读取文件 "${filePath}"...`);
    try {
        const response = await fetch(`${backendBaseUrl}/api/file/content?filePath=${encodeURIComponent(filePath)}`);
        if (response.ok) {
            const content = await response.text();
            alert(`文件 "${filePath}" 的内容：\n\n${content.substring(0, 500)}... (仅显示前500字符)`);
        } else {
            const errorText = await response.text();
            alert(`读取文件内容失败: ${errorText}`);
        }
    } catch (error) {
        console.error("读取文件内容时出错:", error);
        alert("读取文件内容时发生网络错误或服务器错误。");
    } finally {
        showLoadingFunc(false);
    }
}

export async function askQuestionStream(question, selectedFiles, isDeepMode, showLoadingFunc, isStreamingRef, currentReaderRef, askBtn, stopBtn, questionInput) {
    console.log(`askQuestionStream called with isDeepMode: ${isDeepMode}`);
    if (!question) {
        alert('请输入你的问题！');
        return;
    }

    document.getElementById('response').innerHTML = '等待处理...';
    showLoadingFunc(true);

    isStreamingRef.value = true;
    askBtn.disabled = true; // Corrected: should be true
    stopBtn.disabled = false;
    questionInput.disabled = true;

    try {
        const requestBody = {
            question: question,
            relatedFiles: Array.from(selectedFiles),
            isDeepMode: isDeepMode
        };
        console.log("请求体:", requestBody);

        const response = await fetch(`${backendBaseUrl}/api/question/ask-stream`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(requestBody)
        });

        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(`HTTP 错误: ${response.status} - ${errorText}`);
        }

        const reader = response.body.getReader();
        currentReaderRef.value = reader;
        const decoder = new TextDecoder('utf-8');
        let buffer = '';
        let firstChunk = true;

        while (true) {
            const { done, value } = await reader.read();
            if (done) {
                console.log("流式传输完成。");
                break;
            }

            buffer += decoder.decode(value, { stream: true });

            const events = buffer.split('\n\n');
            buffer = events.pop();

            for (const event of events) {
                if (event.startsWith('event: data')) {
                    const data = event.substring(event.indexOf('data:') + 5).trim();
                    try {
                        const json = JSON.parse(data);
                        if (firstChunk) {
                            document.getElementById('response').innerHTML = '';
                            firstChunk = false;
                        }
                        document.getElementById('response').innerHTML += json.content;
                        document.getElementById('response').scrollTop = document.getElementById('response').scrollHeight;
                    } catch (e) {
                        console.error('解析 JSON 失败:', e, '数据:', data);
                    }
                } else if (event.startsWith('event: start')) {
                    console.log('流式传输开始。');
                } else if (event.startsWith('event: end')) {
                    console.log('流式传输结束。');
                } else if (event.startsWith('event: error')) {
                    const data = event.substring(event.indexOf('data:') + 5).trim();
                    try {
                        const json = JSON.parse(data);
                        document.getElementById('response').innerHTML += `\n\n错误: ${json.error || json.message}`;
                        console.error('服务器错误:', json);
                    } catch (e) {
                        console.error('解析错误 JSON 失败:', e, '数据:', data);
                    }
                }
            }
        }
    } catch (error) {
        console.error('流式提问时发生错误:', error);
        if (error.name === 'AbortError') {
            console.log('Fetch request aborted by user.');
        } else {
            document.getElementById('response').innerHTML += `\n\n错误: ${error.message}`;
        }
    } finally {
        isStreamingRef.value = false;
        askBtn.disabled = false;
        stopBtn.disabled = true;
        questionInput.disabled = false;
        showLoadingFunc(false);
        currentReaderRef.value = null;
    }
}

export async function checkHealth(showLoadingFunc) {
    showLoadingFunc(true, '正在检查 AI 服务健康状态...');
    try {
        const response = await fetch(`${backendBaseUrl}/api/question/health`);
        const data = await response.json();
        if (response.ok) {
            alert(`AI 服务状态: ${data.status} (时间: ${new Date(data.timestamp).toLocaleString()})`);
        } else {
            alert(`AI 服务健康检查失败: ${data.error || '未知错误'}`);
        }
    } catch (error) {
        console.error("健康检查时出错:", error);
        alert("健康检查时发生网络错误或服务器错误。");
    } finally {
        showLoadingFunc(false);
    }
} 