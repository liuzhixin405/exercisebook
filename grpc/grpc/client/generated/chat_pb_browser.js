// Browser-compatible version of chat_pb.js
// This file adapts the CommonJS generated code for browser use

(function () {
    'use strict';

    // Mock the CommonJS require system for browser
    const jspb = window.jspb || window.proto.google.protobuf;
    const goog = window.goog || {};

    if (!window.proto) {
        window.proto = {};
    }
    if (!window.proto.chat) {
        window.proto.chat = {};
    }

    // RealtimePushRequest class
    window.proto.chat.RealtimePushRequest = function (opt_data) {
        jspb.Message.initialize(this, opt_data, 0, -1, null, null);
    };

    // Inherit from jspb.Message
    if (jspb.Message) {
        window.proto.chat.RealtimePushRequest.prototype = Object.create(jspb.Message.prototype);
        window.proto.chat.RealtimePushRequest.prototype.constructor = window.proto.chat.RealtimePushRequest;
    }

    // RealtimePushRequest methods
    window.proto.chat.RealtimePushRequest.prototype.getClientId = function () {
        return jspb.Message.getFieldWithDefault(this, 1, "");
    };

    window.proto.chat.RealtimePushRequest.prototype.setClientId = function (value) {
        return jspb.Message.setProto3StringField(this, 1, value);
    };

    window.proto.chat.RealtimePushRequest.prototype.getTimestamp = function () {
        return jspb.Message.getFieldWithDefault(this, 2, 0);
    };

    window.proto.chat.RealtimePushRequest.prototype.setTimestamp = function (value) {
        return jspb.Message.setProto3IntField(this, 2, value);
    };

    // RealtimePushRequest serialization
    window.proto.chat.RealtimePushRequest.prototype.serializeBinary = function () {
        const writer = new jspb.BinaryWriter();
        window.proto.chat.RealtimePushRequest.serializeBinaryToWriter(this, writer);
        return writer.getResultBuffer();
    };

    window.proto.chat.RealtimePushRequest.serializeBinaryToWriter = function (message, writer) {
        const f = message.getClientId();
        if (f.length > 0) {
            writer.writeString(1, f);
        }
        const f2 = message.getTimestamp();
        if (f2 !== 0) {
            writer.writeInt64(2, f2);
        }
    };

    window.proto.chat.RealtimePushRequest.deserializeBinary = function (bytes) {
        const reader = new jspb.BinaryReader(bytes);
        const msg = new window.proto.chat.RealtimePushRequest();
        return window.proto.chat.RealtimePushRequest.deserializeBinaryFromReader(msg, reader);
    };

    window.proto.chat.RealtimePushRequest.deserializeBinaryFromReader = function (msg, reader) {
        while (reader.nextField()) {
            if (reader.isEndGroup()) {
                break;
            }
            const field = reader.getFieldNumber();
            switch (field) {
                case 1:
                    const value = reader.readString();
                    msg.setClientId(value);
                    break;
                case 2:
                    const value2 = reader.readInt64();
                    msg.setTimestamp(value2);
                    break;
                default:
                    reader.skipField();
                    break;
            }
        }
        return msg;
    };

    // RealtimePushResponse class
    window.proto.chat.RealtimePushResponse = function (opt_data) {
        jspb.Message.initialize(this, opt_data, 0, -1, null, null);
    };

    // Inherit from jspb.Message
    if (jspb.Message) {
        window.proto.chat.RealtimePushResponse.prototype = Object.create(jspb.Message.prototype);
        window.proto.chat.RealtimePushResponse.prototype.constructor = window.proto.chat.RealtimePushResponse;
    }

    // RealtimePushResponse methods
    window.proto.chat.RealtimePushResponse.prototype.getData = function () {
        return jspb.Message.getFieldWithDefault(this, 1, "");
    };

    window.proto.chat.RealtimePushResponse.prototype.setData = function (value) {
        return jspb.Message.setProto3StringField(this, 1, value);
    };

    window.proto.chat.RealtimePushResponse.prototype.getTimestamp = function () {
        return jspb.Message.getFieldWithDefault(this, 2, 0);
    };

    window.proto.chat.RealtimePushResponse.prototype.setTimestamp = function (value) {
        return jspb.Message.setProto3IntField(this, 2, value);
    };

    window.proto.chat.RealtimePushResponse.prototype.getDataType = function () {
        return jspb.Message.getFieldWithDefault(this, 3, "");
    };

    window.proto.chat.RealtimePushResponse.prototype.setDataType = function (value) {
        return jspb.Message.setProto3StringField(this, 3, value);
    };

    // RealtimePushResponse serialization
    window.proto.chat.RealtimePushResponse.prototype.serializeBinary = function () {
        const writer = new jspb.BinaryWriter();
        window.proto.chat.RealtimePushResponse.serializeBinaryToWriter(this, writer);
        return writer.getResultBuffer();
    };

    window.proto.chat.RealtimePushResponse.serializeBinaryToWriter = function (message, writer) {
        const f = message.getData();
        if (f.length > 0) {
            writer.writeString(1, f);
        }
        const f2 = message.getTimestamp();
        if (f2 !== 0) {
            writer.writeInt64(2, f2);
        }
        const f3 = message.getDataType();
        if (f3.length > 0) {
            writer.writeString(3, f3);
        }
    };

    window.proto.chat.RealtimePushResponse.deserializeBinary = function (bytes) {
        const reader = new jspb.BinaryReader(bytes);
        const msg = new window.proto.chat.RealtimePushResponse();
        return window.proto.chat.RealtimePushResponse.deserializeBinaryFromReader(msg, reader);
    };

    window.proto.chat.RealtimePushResponse.deserializeBinaryFromReader = function (msg, reader) {
        while (reader.nextField()) {
            if (reader.isEndGroup()) {
                break;
            }
            const field = reader.getFieldNumber();
            switch (field) {
                case 1:
                    const value = reader.readString();
                    msg.setData(value);
                    break;
                case 2:
                    const value2 = reader.readInt64();
                    msg.setTimestamp(value2);
                    break;
                case 3:
                    const value3 = reader.readString();
                    msg.setDataType(value3);
                    break;
                default:
                    reader.skipField();
                    break;
            }
        }
        return msg;
    };

    console.log('chat_pb_browser.js loaded successfully');
})();