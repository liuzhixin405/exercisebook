// gRPC-Web compatibility shim
// This provides the minimal grpc-web functionality needed for the client

(function() {
    'use strict';

    // Create grpc namespace if it doesn't exist
    if (typeof window.grpc === 'undefined') {
        window.grpc = {};
    }

    if (typeof window.grpc.web === 'undefined') {
        window.grpc.web = {};
    }

    // Method types
    window.grpc.web.MethodType = {
        UNARY: 'unary',
        SERVER_STREAMING: 'server_streaming',
        CLIENT_STREAMING: 'client_streaming',
        BIDIRECTIONAL_STREAMING: 'bidirectional_streaming'
    };

    // Method descriptor
    window.grpc.web.MethodDescriptor = function(path, methodType, requestType, responseType, requestSerializeFn, responseDeserializeFn) {
        this.path = path;
        this.methodType = methodType;
        this.requestType = requestType;
        this.responseType = responseType;
        this.requestSerializeFn = requestSerializeFn;
        this.responseDeserializeFn = responseDeserializeFn;
    };

    // Base client
    window.grpc.web.GrpcWebClientBase = function(options) {
        this.options = options || {};
        this.format = this.options.format || 'text';
    };

    // RPC call method
    window.grpc.web.GrpcWebClientBase.prototype.rpcCall = function(url, request, metadata, methodDescriptor, callback) {
        const self = this;
        
        try {
            // Serialize the request
            const serializedRequest = methodDescriptor.requestSerializeFn(request);
            
            // Create proper gRPC-Web frame
            const frameHeader = new Uint8Array(5);
            frameHeader[0] = 0; // Compression flag (0 = no compression)
            
            // Message length (big-endian 32-bit)
            const messageLength = serializedRequest.length;
            frameHeader[1] = (messageLength >>> 24) & 0xFF;
            frameHeader[2] = (messageLength >>> 16) & 0xFF;
            frameHeader[3] = (messageLength >>> 8) & 0xFF;
            frameHeader[4] = messageLength & 0xFF;
            
            // Combine frame header and message
            const framedMessage = new Uint8Array(5 + messageLength);
            framedMessage.set(frameHeader, 0);
            framedMessage.set(serializedRequest, 5);
            
            // Convert to base64 for grpc-web-text format
            const base64Request = btoa(String.fromCharCode.apply(null, framedMessage));
            
            // Create fetch request with proper headers
            const headers = {
                'Content-Type': 'application/grpc-web-text',
                'X-Grpc-Web': '1',
                'Accept': 'application/grpc-web-text'
            };
            
            // Add metadata without overriding Content-Type
            if (metadata) {
                Object.keys(metadata).forEach(key => {
                    if (key.toLowerCase() !== 'content-type') {
                        headers[key] = metadata[key];
                    }
                });
            }
            
            const fetchOptions = {
                method: 'POST',
                headers: headers,
                body: base64Request
            };

            fetch(url, fetchOptions)
                .then(response => {
                    if (!response.ok) {
                        throw new Error(`HTTP ${response.status}: ${response.statusText}`);
                    }
                    return response.text();
                })
                .then(base64Response => {
                    try {
                        console.log('Raw base64 response:', base64Response);
                        
                        // Split the response to separate message from trailers
                        // gRPC-Web format: [message][trailers]
                        // Look for the trailer marker (usually starts with 0x80)
                        let messageBase64 = base64Response;
                        
                        // Find the end of the message part (before trailers)
                        // Trailers typically start with 0x80 which is 'gA' in base64
                        const trailerIndex = base64Response.indexOf('gAAAA');
                        if (trailerIndex > 0) {
                            messageBase64 = base64Response.substring(0, trailerIndex);
                            console.log('Message part:', messageBase64);
                            console.log('Trailer part:', base64Response.substring(trailerIndex));
                        }
                        
                        // Clean up base64 string
                        const cleanBase64 = messageBase64.replace(/[^A-Za-z0-9+/=]/g, '');
                        console.log('Cleaned base64:', cleanBase64);
                        
                        // Decode base64 response
                        const binaryString = atob(cleanBase64);
                        const responseBytes = new Uint8Array(binaryString.length);
                        for (let i = 0; i < binaryString.length; i++) {
                            responseBytes[i] = binaryString.charCodeAt(i);
                        }
                        
                        console.log('Response bytes length:', responseBytes.length);
                        console.log('Response bytes (first 20):', Array.from(responseBytes.slice(0, 20)));
                        
                        // Skip the gRPC frame header (5 bytes) and get the message
                        if (responseBytes.length < 5) {
                            throw new Error('Invalid gRPC response: too short');
                        }
                        
                        // Read frame header
                        const compressionFlag = responseBytes[0];
                        const messageLength = (responseBytes[1] << 24) | (responseBytes[2] << 16) | (responseBytes[3] << 8) | responseBytes[4];
                        
                        console.log('Frame info - compression:', compressionFlag, 'length:', messageLength);
                        
                        const messageBytes = responseBytes.slice(5, 5 + messageLength);
                        console.log('Message bytes length:', messageBytes.length);
                        console.log('Message bytes:', Array.from(messageBytes));
                        
                        const response = methodDescriptor.responseDeserializeFn(messageBytes);
                        callback(null, response);
                    } catch (parseError) {
                        console.error('Parse error:', parseError);
                        callback(parseError, null);
                    }
                })
                .catch(error => {
                    callback(error, null);
                });
                
        } catch (error) {
            setTimeout(() => callback(error, null), 0);
        }
    };

    // Server streaming method
    window.grpc.web.GrpcWebClientBase.prototype.serverStreaming = function(url, request, metadata, methodDescriptor) {
        const self = this;
        
        // Create a simple event emitter for the stream
        const stream = {
            listeners: {},
            
            on: function(event, callback) {
                if (!this.listeners[event]) {
                    this.listeners[event] = [];
                }
                this.listeners[event].push(callback);
            },
            
            emit: function(event, data) {
                if (this.listeners[event]) {
                    this.listeners[event].forEach(callback => callback(data));
                }
            }
        };

        try {
            // Serialize the request
            const serializedRequest = methodDescriptor.requestSerializeFn(request);
            
            // Create proper gRPC-Web frame
            const frameHeader = new Uint8Array(5);
            frameHeader[0] = 0; // Compression flag
            
            const messageLength = serializedRequest.length;
            frameHeader[1] = (messageLength >>> 24) & 0xFF;
            frameHeader[2] = (messageLength >>> 16) & 0xFF;
            frameHeader[3] = (messageLength >>> 8) & 0xFF;
            frameHeader[4] = messageLength & 0xFF;
            
            const framedMessage = new Uint8Array(5 + messageLength);
            framedMessage.set(frameHeader, 0);
            framedMessage.set(serializedRequest, 5);
            
            const base64Request = btoa(String.fromCharCode.apply(null, framedMessage));
            
            const headers = {
                'Content-Type': 'application/grpc-web-text',
                'X-Grpc-Web': '1',
                'Accept': 'application/grpc-web-text'
            };
            
            // Add metadata without overriding Content-Type
            if (metadata) {
                Object.keys(metadata).forEach(key => {
                    if (key.toLowerCase() !== 'content-type') {
                        headers[key] = metadata[key];
                    }
                });
            }
            
            const fetchOptions = {
                method: 'POST',
                headers: headers,
                body: base64Request
            };

            fetch(url, fetchOptions)
                .then(response => {
                    if (!response.ok) {
                        throw new Error(`HTTP ${response.status}: ${response.statusText}`);
                    }
                    
                    console.log('Starting to read streaming response...');
                    
                    // 使用ReadableStream来读取gRPC-Web流式响应
                    const reader = response.body.getReader();
                    const decoder = new TextDecoder();
                    let buffer = '';
                    let messageCount = 0;
                    
                    function readStreamChunk() {
                        return reader.read().then(({ done, value }) => {
                            if (done) {
                                console.log('📡 Stream reading completed, total messages processed:', messageCount);
                                // 处理剩余缓冲区数据
                                if (buffer.length > 0) {
                                    console.log('📦 Processing remaining buffer on stream end');
                                    processStreamBuffer();
                                }
                                stream.emit('end');
                                return;
                            }
                            
                            // 将新数据添加到缓冲区
                            const chunk = decoder.decode(value, { stream: true });
                            buffer += chunk;
                            console.log('📦 Received stream chunk:', chunk.length, 'chars, buffer total:', buffer.length);
                            console.log('📦 Chunk content:', chunk.substring(0, 100) + (chunk.length > 100 ? '...' : ''));
                            
                            // 处理缓冲区中的完整消息
                            processStreamBuffer();
                            
                            // 继续读取
                            return readStreamChunk();
                        }).catch(error => {
                            console.error('❌ Stream reading error:', error);
                            stream.emit('error', error);
                        });
                    }
                    
                    function processStreamBuffer() {
                        console.log('🔍 Processing buffer, length:', buffer.length);
                        
                        // gRPC-Web流式响应处理
                        // 需要正确处理base64编码的gRPC帧
                        
                        while (buffer.length > 0) {
                            try {
                                // 查找完整的base64块
                                // gRPC-Web消息通常以特定模式开始，我们需要找到完整的消息边界
                                
                                // 首先尝试解码整个缓冲区
                                let messageBase64 = buffer;
                                
                                // 检查是否包含trailer标记（通常以0x80开头，base64中是'gA'）
                                const trailerMarkers = ['gAAAA', 'gAAA', 'gAA', 'gA'];
                                let trailerIndex = -1;
                                
                                for (const marker of trailerMarkers) {
                                    const index = messageBase64.indexOf(marker);
                                    if (index > 0) {
                                        trailerIndex = index;
                                        break;
                                    }
                                }
                                
                                if (trailerIndex > 0) {
                                    messageBase64 = messageBase64.substring(0, trailerIndex);
                                    console.log('📦 Found trailer at index:', trailerIndex);
                                    console.log('📦 Message part:', messageBase64);
                                }
                                
                                // 清理base64字符串，确保只包含有效字符
                                const cleanBase64 = messageBase64.replace(/[^A-Za-z0-9+/=]/g, '');
                                
                                // 确保base64字符串长度是4的倍数（添加必要的填充）
                                let paddedBase64 = cleanBase64;
                                const padding = paddedBase64.length % 4;
                                if (padding > 0) {
                                    paddedBase64 += '='.repeat(4 - padding);
                                }
                                
                                console.log('📦 Original base64 length:', messageBase64.length);
                                console.log('📦 Cleaned base64 length:', cleanBase64.length);
                                console.log('📦 Padded base64 length:', paddedBase64.length);
                                
                                if (paddedBase64.length === 0) {
                                    console.log('❌ Empty base64 after cleaning');
                                    buffer = ''; // 清空缓冲区
                                    break;
                                }
                                
                                // 尝试解码base64
                                const binaryString = atob(paddedBase64);
                                const responseBytes = new Uint8Array(binaryString.length);
                                for (let i = 0; i < binaryString.length; i++) {
                                    responseBytes[i] = binaryString.charCodeAt(i);
                                }
                                
                                console.log('📦 Decoded bytes length:', responseBytes.length);
                                console.log('📦 First 10 bytes:', Array.from(responseBytes.slice(0, 10)));
                                
                                // 检查是否有足够的数据来读取gRPC帧头
                                if (responseBytes.length >= 5) {
                                    const compressionFlag = responseBytes[0];
                                    const frameMsgLength = (responseBytes[1] << 24) | (responseBytes[2] << 16) | (responseBytes[3] << 8) | responseBytes[4];
                                    
                                    console.log(`📡 Stream frame: compression=${compressionFlag}, length=${frameMsgLength}, total=${responseBytes.length}`);
                                    
                                    // 检查是否有完整的消息数据
                                    if (responseBytes.length >= 5 + frameMsgLength && frameMsgLength > 0) {
                                        const messageBytes = responseBytes.slice(5, 5 + frameMsgLength);
                                        console.log('📦 Message bytes:', Array.from(messageBytes));
                                        
                                        try {
                                            const response = methodDescriptor.responseDeserializeFn(messageBytes);
                                            messageCount++;
                                            console.log(`✅ Successfully parsed message #${messageCount}, emitting data`);
                                            stream.emit('data', response);
                                            
                                            // 处理完成后，移除已处理的数据
                                            if (trailerIndex > 0) {
                                                buffer = buffer.substring(trailerIndex);
                                                console.log('📦 Moved buffer past trailer, remaining length:', buffer.length);
                                            } else {
                                                buffer = ''; // 清空缓冲区
                                                console.log('📦 Cleared buffer completely');
                                            }
                                            
                                        } catch (deserializeError) {
                                            console.error('❌ Deserialization error:', deserializeError);
                                            console.error('❌ Message bytes that failed:', Array.from(messageBytes.slice(0, 20)));
                                            buffer = ''; // 清空缓冲区避免无限循环
                                            break;
                                        }
                                    } else {
                                        console.log('❌ Incomplete frame data or invalid length');
                                        // 如果数据不完整，等待更多数据
                                        if (buffer.length < 200) { // 避免无限等待
                                            break;
                                        } else {
                                            // 如果缓冲区太大但仍然无法解析，清空它
                                            buffer = '';
                                            break;
                                        }
                                    }
                                } else {
                                    console.log('❌ Frame too short, waiting for more data');
                                    break;
                                }
                                
                            } catch (parseError) {
                                console.error('❌ Error processing stream message:', parseError);
                                // 出错时清空缓冲区，避免无限循环
                                buffer = '';
                                break;
                            }
                        }
                        
                        console.log('🔍 Remaining buffer length:', buffer.length);
                    }
                    
                    // 开始读取流
                    return readStreamChunk();
                })
                .catch(error => {
                    console.error('Stream fetch error:', error);
                    stream.emit('error', error);
                });
                
        } catch (error) {
            setTimeout(() => stream.emit('error', error), 0);
        }

        return stream;
    };

    // Unary call method (for promise-based client)
    window.grpc.web.GrpcWebClientBase.prototype.unaryCall = function(url, request, metadata, methodDescriptor) {
        const self = this;
        
        return new Promise((resolve, reject) => {
            this.rpcCall(url, request, metadata, methodDescriptor, (error, response) => {
                if (error) {
                    reject(error);
                } else {
                    resolve(response);
                }
            });
        });
    };

    console.log('gRPC-Web shim loaded successfully');
})();