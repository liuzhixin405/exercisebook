// gRPC-Web Chat Client Implementation

class RealtimePushClient {
    constructor() {
        this.client = null;
        this.isConnected = false;
        this.serverUrl = 'https://localhost:5201';
        
        // Streaming related properties
        this.currentStream = null;
        this.streamMessageCount = 0;
        this.streamStartTime = null;
        
        this.initializeUI();
    }

    initializeUI() {
        const streamButton = document.getElementById('streamButton');
        const stopStreamButton = document.getElementById('stopStreamButton');
        const clearButton = document.getElementById('clearButton');

        streamButton.addEventListener('click', () => this.startStreamingChat());
        stopStreamButton.addEventListener('click', () => this.stopStreaming());
        clearButton.addEventListener('click', () => this.clearMessages());

        // Initialize connection status
        this.updateConnectionStatus(false, '正在初始化...');

        // Try to connect when the page loads
        this.connect();
    }



    connect() {
        try {
            // Initialize the gRPC-Web client using the generated protobuf classes
            console.log('正在初始化实时推送客户端...');
            
            // Check if the required dependencies are available
            if (typeof jspb === 'undefined') {
                throw new Error('google-protobuf 库未加载');
            }
            
            if (typeof grpc === 'undefined' || !grpc.web) {
                console.warn('grpc-web 库未完全加载，等待重试...');
                setTimeout(() => this.connect(), 1000);
                return;
            }
            
            if (typeof proto === 'undefined' || !proto.chat || !proto.chat.ChatServiceClient) {
                throw new Error('gRPC 生成的客户端代码未加载');
            }

            // Create the gRPC-Web client
            this.client = new proto.chat.ChatServiceClient(this.serverUrl, null, {
                format: 'text',
                withCredentials: false
            });
            
            console.log('实时推送客户端创建成功');
            this.updateConnectionStatus(true, '已连接');
            this.addMessage('系统', '🚀 实时推送客户端已就绪', 'system');
            
        } catch (error) {
            console.error('连接初始化失败:', error);
            this.updateConnectionStatus(false, '初始化失败');
            this.addMessage('系统', '初始化失败: ' + this.getErrorMessage(error), 'error');
        }
    }



    updateConnectionStatus(connected, message = '') {
        const statusDiv = document.getElementById('status');
        const sendButton = document.getElementById('sendButton');
        const streamButton = document.getElementById('streamButton');
        
        this.isConnected = connected;
        
        if (connected) {
            statusDiv.textContent = '状态: 已连接' + (message ? ' - ' + message : '');
            statusDiv.className = 'status connected';
            streamButton.disabled = false;
        } else {
            statusDiv.textContent = '状态: 未连接' + (message ? ' - ' + message : '');
            statusDiv.className = 'status disconnected';
            streamButton.disabled = true;
        }
    }



    addMessage(sender, content, type) {
        const chatContainer = document.getElementById('chatContainer');
        const messageDiv = document.createElement('div');
        messageDiv.className = `message ${type}`;
        
        const timestamp = new Date().toLocaleTimeString();
        messageDiv.innerHTML = `
            <div><strong>${sender}</strong> <small>${timestamp}</small></div>
            <div>${content}</div>
        `;

        chatContainer.appendChild(messageDiv);
        chatContainer.scrollTop = chatContainer.scrollHeight;


    }

    clearMessages() {
        const chatContainer = document.getElementById('chatContainer');
        chatContainer.innerHTML = '';
        this.addMessage('系统', '消息历史已清空', 'system');
    }

    startStreamingChat() {
        if (!this.isConnected) {
            this.addMessage('系统', '未连接到服务器，无法启动实时推送', 'error');
            return;
        }

        if (!this.client) {
            this.addMessage('系统', 'gRPC客户端未初始化', 'error');
            return;
        }

        // Check if already streaming
        if (this.currentStream) {
            this.addMessage('系统', '实时推送已在运行中', 'system');
            return;
        }

        try {
            // Create a RealtimePushRequest for starting the stream
            const pushRequest = new proto.chat.RealtimePushRequest();
            pushRequest.setClientId('web-client-' + Date.now());
            pushRequest.setTimestamp(Math.floor(Date.now() / 1000));

            console.log('启动实时推送:', {
                clientId: pushRequest.getClientId(),
                timestamp: pushRequest.getTimestamp()
            });

            // Add metadata for streaming
            const metadata = {
                'x-user-agent': 'grpc-web-realtime-client'
            };

            // Start the streaming
            console.log('🚀 Calling client.startRealtimePush...');
            const stream = this.client.startRealtimePush(pushRequest, metadata);
            
            if (!stream) {
                throw new Error('无法创建实时推送连接');
            }

            console.log('✅ Stream created successfully:', stream);

            // Store the stream reference
            this.currentStream = stream;
            this.streamMessageCount = 0;
            this.streamStartTime = Date.now();

            // Update UI to show streaming is active
            this.updateStreamingUI(true);

            stream.on('data', (response) => {
                console.log('📡 收到实时数据:', response);
                if (response && typeof response.getData === 'function') {
                    this.streamMessageCount++;
                    
                    console.log(`✅ 处理第 ${this.streamMessageCount} 条实时数据:`, response.getData());
                    
                    // Add message with special styling for real-time data
                    this.addRealtimeMessage(
                        `[${response.getDataType()}] ${response.getData()}`, 
                        this.streamMessageCount
                    );
                    
                    // Update statistics
                    this.updateStreamStats();
                } else {
                    console.warn('❌ 收到无效的实时响应:', response);
                }
            });

            stream.on('error', (error) => {
                console.error('实时推送错误:', error);
                this.addMessage('系统', '实时推送错误: ' + this.getErrorMessage(error), 'error');
                this.stopStreaming();
            });

            stream.on('end', () => {
                console.log('实时推送结束');
                this.addMessage('系统', '实时推送已结束', 'system');
                this.stopStreaming();
            });

            stream.on('status', (status) => {
                console.log('实时推送状态:', status);
                if (status.code !== 0) {
                    this.addMessage('系统', `实时推送状态错误: ${status.details}`, 'error');
                }
            });

            this.addMessage('系统', '🚀 实时数据推送已启动', 'system');
            
        } catch (error) {
            console.error('启动实时推送失败:', error);
            this.addMessage('系统', '启动实时推送失败: ' + this.getErrorMessage(error), 'error');
        }
    }

    stopStreaming() {
        if (this.currentStream) {
            try {
                // Note: gRPC-Web doesn't have a standard cancel method
                // The stream will be closed when the component is destroyed
                this.currentStream = null;
                this.updateStreamingUI(false);
                this.addMessage('系统', '⏹️ 实时推送已停止', 'system');
            } catch (error) {
                console.error('停止实时推送时出错:', error);
            }
        }
    }

    updateStreamingUI(isStreaming) {
        const streamButton = document.getElementById('streamButton');
        const stopButton = document.getElementById('stopStreamButton');
        
        if (isStreaming) {
            streamButton.style.display = 'none';
            if (stopButton) {
                stopButton.style.display = 'inline-block';
            }
        } else {
            streamButton.style.display = 'inline-block';
            if (stopButton) {
                stopButton.style.display = 'none';
            }
        }
    }

    addRealtimeMessage(content, count) {
        const chatContainer = document.getElementById('chatContainer');
        const messageDiv = document.createElement('div');
        messageDiv.className = 'message realtime';
        
        const timestamp = new Date().toLocaleTimeString();
        messageDiv.innerHTML = `
            <div class="realtime-header">
                <strong>📡 实时数据 #${count}</strong> 
                <small>${timestamp}</small>
            </div>
            <div class="realtime-content">${content}</div>
        `;

        chatContainer.appendChild(messageDiv);
        
        // Auto-scroll to bottom
        chatContainer.scrollTop = chatContainer.scrollHeight;

        // Keep only last 100 messages to prevent memory issues
        const messages = chatContainer.querySelectorAll('.message');
        if (messages.length > 100) {
            for (let i = 0; i < messages.length - 100; i++) {
                messages[i].remove();
            }
        }
    }

    updateStreamStats() {
        // Update or create stats display
        let statsDiv = document.getElementById('streamStats');
        if (!statsDiv) {
            statsDiv = document.createElement('div');
            statsDiv.id = 'streamStats';
            statsDiv.className = 'stream-stats';
            
            const statusDiv = document.getElementById('status');
            statusDiv.parentNode.insertBefore(statsDiv, statusDiv.nextSibling);
        }
        
        const uptime = this.currentStream ? Math.floor((Date.now() - this.streamStartTime) / 1000) : 0;
        statsDiv.innerHTML = `
            📊 实时统计: 已接收 <strong>${this.streamMessageCount}</strong> 条数据 | 
            运行时间: <strong>${uptime}</strong> 秒 | 
            平均速率: <strong>${(this.streamMessageCount / Math.max(uptime, 1)).toFixed(1)}</strong> 条/秒
        `;
    }

    getErrorMessage(error) {
        if (!error) return '未知错误';
        
        // Handle gRPC-Web specific errors
        if (error.code !== undefined) {
            const grpcErrorCodes = {
                0: 'OK',
                1: 'CANCELLED - 操作被取消',
                2: 'UNKNOWN - 未知错误',
                3: 'INVALID_ARGUMENT - 无效参数',
                4: 'DEADLINE_EXCEEDED - 请求超时',
                5: 'NOT_FOUND - 未找到',
                6: 'ALREADY_EXISTS - 已存在',
                7: 'PERMISSION_DENIED - 权限被拒绝',
                8: 'RESOURCE_EXHAUSTED - 资源耗尽',
                9: 'FAILED_PRECONDITION - 前置条件失败',
                10: 'ABORTED - 操作被中止',
                11: 'OUT_OF_RANGE - 超出范围',
                12: 'UNIMPLEMENTED - 未实现',
                13: 'INTERNAL - 内部错误',
                14: 'UNAVAILABLE - 服务不可用',
                15: 'DATA_LOSS - 数据丢失',
                16: 'UNAUTHENTICATED - 未认证'
            };
            
            const codeDescription = grpcErrorCodes[error.code] || `未知错误代码: ${error.code}`;
            return `gRPC错误: ${codeDescription}`;
        }
        
        if (error.message) {
            return error.message;
        }
        
        if (typeof error === 'string') {
            return error;
        }
        
        // Handle network errors
        if (error.name === 'TypeError' && error.message.includes('fetch')) {
            return '网络连接错误，请检查服务器是否运行';
        }
        
        return '未知错误: ' + JSON.stringify(error);
    }

    isConnectionError(error) {
        if (!error) return false;
        
        // Check gRPC error codes that indicate connection issues
        if (error.code !== undefined) {
            return error.code === 14 || // UNAVAILABLE
                   error.code === 4 ||  // DEADLINE_EXCEEDED
                   error.code === 1;    // CANCELLED
        }
        
        const errorMessage = this.getErrorMessage(error).toLowerCase();
        return errorMessage.includes('network') || 
               errorMessage.includes('connection') || 
               errorMessage.includes('timeout') ||
               errorMessage.includes('unavailable') ||
               errorMessage.includes('fetch') ||
               errorMessage.includes('cors') ||
               errorMessage.includes('refused');
    }
}

// Initialize the realtime push client when the page loads
document.addEventListener('DOMContentLoaded', () => {
    window.realtimePushClient = new RealtimePushClient();
});

// Export for potential use in other modules
if (typeof module !== 'undefined' && module.exports) {
    module.exports = RealtimePushClient;
}