// Browser-compatible version of chat_grpc_web_pb.js
// This file adapts the CommonJS generated code for browser use

(function () {
    'use strict';

    // Ensure proto namespace exists
    if (!window.proto) {
        window.proto = {};
    }
    if (!window.proto.chat) {
        window.proto.chat = {};
    }

    // ChatServiceClient class
    window.proto.chat.ChatServiceClient = function (hostname, credentials, options) {
        if (!options) options = {};
        options['format'] = options['format'] || 'text';

        // Use our custom gRPC-Web shim
        if (window.grpc && window.grpc.web && window.grpc.web.GrpcWebClientBase) {
            window.grpc.web.GrpcWebClientBase.call(this, options);
        }

        this.hostname_ = hostname;
        this.credentials_ = credentials;
        this.options_ = options;
    };

    // Inherit from GrpcWebClientBase if available
    if (window.grpc && window.grpc.web && window.grpc.web.GrpcWebClientBase) {
        window.proto.chat.ChatServiceClient.prototype = Object.create(window.grpc.web.GrpcWebClientBase.prototype);
        window.proto.chat.ChatServiceClient.prototype.constructor = window.proto.chat.ChatServiceClient;
    }

    // Method descriptor for StartRealtimePush
    const methodDescriptor_ChatService_StartRealtimePush = new window.grpc.web.MethodDescriptor(
        '/chat.ChatService/StartRealtimePush',
        window.grpc.web.MethodType.SERVER_STREAMING,
        window.proto.chat.RealtimePushRequest,
        window.proto.chat.RealtimePushResponse,
        function (request) {
            return request.serializeBinary();
        },
        function (bytes) {
            return window.proto.chat.RealtimePushResponse.deserializeBinary(bytes);
        }
    );

    // StartRealtimePush method
    window.proto.chat.ChatServiceClient.prototype.startRealtimePush = function (request, metadata) {
        const url = this.hostname_ + '/chat.ChatService/StartRealtimePush';

        console.log('🚀 Starting realtime push with URL:', url);
        console.log('🚀 Request:', {
            clientId: request.getClientId(),
            timestamp: request.getTimestamp()
        });

        return this.serverStreaming(
            url,
            request,
            metadata || {},
            methodDescriptor_ChatService_StartRealtimePush
        );
    };

    console.log('chat_grpc_web_pb_browser.js loaded successfully');
})();