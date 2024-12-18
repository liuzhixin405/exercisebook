(function (global, factory) {
	typeof exports === 'object' && typeof module !== 'undefined' ? factory(exports) :
	typeof define === 'function' && define.amd ? define(['exports'], factory) :
	(factory((global.Datafeeds = {})));
}(this, (function (exports) { 'use strict';


/*! *****************************************************************************
Copyright (c) Microsoft Corporation. All rights reserved.
Licensed under the Apache License, Version 2.0 (the "License"); you may not use
this file except in compliance with the License. You may obtain a copy of the
License at http://www.apache.org/licenses/LICENSE-2.0

THIS CODE IS PROVIDED ON AN *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED
WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
MERCHANTABLITY OR NON-INFRINGEMENT.

See the Apache Version 2.0 License for specific language governing permissions
and limitations under the License.
***************************************************************************** */
/* global Reflect, Promise */

var extendStatics = Object.setPrototypeOf ||
    ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
    function (d, b) { for (var p in b) { if (b.hasOwnProperty(p)) { d[p] = b[p]; } } };

function __extends(d, b) {
    extendStatics(d, b);
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
}

/**
 * If you want to enable logs from datafeed set it to `true`
 */
var isLoggingEnabled = false;
function logMessage(message) {
    if (isLoggingEnabled) {
        var now = new Date();
//        console.log(now.toLocaleTimeString() + "." + now.getMilliseconds() + "> " + message);
    }
}
function getErrorMessage(error) {
    if (error === undefined) {
        return '';
    }
    else if (typeof error === 'string') {
        return error;
    }
    return error.message;
}

var HistoryProvider = /** @class */ (function () {
    function HistoryProvider(datafeedUrl, requester) {
//        this._subscribers = {};
        this.historyData = null;
        this.historyResolution = null;
        this._datafeedUrl = datafeedUrl;
        this._requester = requester;
        this.lastUpdateTime = 0;
    }
    
    function dealData(_this, response, resolve, reject) {
        var _type = Object.prototype.toString.call(response.data);
        if( _type === "[object Array]") {
            var _data = response.data;
            var _response = {
                t: [],
                o: [],
                c: [],
                h: [],
                l: [],
                v: [],
                s: 'ok'
            };

            if(_data.length) {
				
				if (_data.length > 2) {
					_this.historyData = null;
				}
            	
            	_this.lastUpdateTime = (new Date()).getTime();
            	
                if(!_this.historyData) {
                    // 无历史数据时
//                    console.log('无历史数据时');
                    _this.historyData = _data;
                    
                    if(_this.historyResolution === '1D' || _this.historyResolution === '1W' || _this.historyResolution === '1M'){
                        _data.forEach(function (v, i, arr) {
                            _response.t.push(v.time);
                            _response.o.push(v.first);
                            _response.c.push(v.last);
                            _response.h.push(v.high);
                            _response.l.push(v.low);
                            _response.v.push(v.vol);
                        });
                    } else {
                    	var correctTime = parseInt(_this.historyResolution * 60);
                        _data.forEach(function (v, i, arr) {
                            _response.t.push(v.time - correctTime);
                            _response.o.push(v.first);
                            _response.c.push(v.last);
                            _response.h.push(v.high);
                            _response.l.push(v.low);
                            _response.v.push(v.vol);
                        });
                    }
                    
                } else {
                    // 有历史数据时 对不断加载中的优化
//                	console.log("有历史数据时");
                    var oldObj = _this.historyData[_this.historyData.length - 1];
                    var newObj = _data[_data.length - 1];
                    var _i = 1;
//                    for(var k in oldObj) {
//                    	console.log(oldObj[k] + ", " + newObj[k]);
//                        if(oldObj[k] !== newObj[k]) {
//                            _i++;
//                        }
//                    }
                    
                    if(_i) {
//                       console.log('有历史数据时, 不同的')
                       if (oldObj.time == newObj.time) {
                    	   _this.historyData[_this.historyData.length - 1] = _data[_data.length - 1];
                       } else {
                    	   _this.historyData[_this.historyData.length] = _data[_data.length - 1];
                       }
                       
                       _data = _this.historyData;

                        if(_this.historyResolution === '1D' || _this.historyResolution === '1W' || _this.historyResolution === '1M'){
                            // 日线处理时间为utc时间
                            _data.forEach(function (v, i, arr) {
                                _response.t.push(v.time);
                                _response.o.push(v.first);
                                _response.c.push(v.last);
                                _response.h.push(v.high);
                                _response.l.push(v.low);
                                _response.v.push(v.vol);
                            });
                        } else {
                        	var correctTime = parseInt(_this.historyResolution * 60);
                            _data.forEach(function (v, i, arr) {
                                _response.t.push(v.time - correctTime);
                                _response.o.push(v.first);
                                _response.c.push(v.last);
                                _response.h.push(v.high);
                                _response.l.push(v.low);
                                _response.v.push(v.vol);
                            });
                        }

                    } else {
//                        console.log('有历史数据时, 同的')
                        _response = {
                            s: 'no_data'
                        }
                    }

                }


            } else {
                _response = {
                    s: 'no_data'
                }
            }
        } else {
            _response = {
                s: 'no_data'
            }
        }

	    var response = _response;
	
	    if (response.s !== 'ok' && response.s !== 'no_data') {
	        reject(response.errmsg);
	        return;
	    }
	    
	    var bars = [];
	    var meta = {
	        noData: false,
	    };
	    if (response.s === 'no_data') {
	        meta.noData = true;
	        meta.nextTime = response.nextTime;
	    } else {
	        var volumePresent = response.v !== undefined;
	        var ohlPresent = response.o !== undefined;
	        for (var i = 0; i < response.t.length; ++i) {
	            var barValue = {
	                time: response.t[i] * 1000,
	                close: Number(response.c[i]),
	                open: Number(response.c[i]),
	                high: Number(response.c[i]),
	                low: Number(response.c[i]),
	            };
	            if (ohlPresent) {
	                barValue.open = Number(response.o[i]);
	                barValue.high = Number(response.h[i]);
	                barValue.low = Number(response.l[i]);
	            }
	            if (volumePresent) {
	                barValue.volume = Number(response.v[i]);
	            }
	            bars.push(barValue);
	        }
	    }
	    
	    resolve({
	        bars: bars,
	        meta: meta,
	    });
    }

  
    HistoryProvider.prototype.getBars = function (symbolInfo, resolution, rangeStartDate, rangeEndDate) {
        var _this = this;
        var _resolution;
        // 对分辨率的处理

//        console.log("resolution:" + resolution);
        if (resolution === '1M') {
            _resolution = '1month';
        } else if (resolution === '1W') {
            _resolution = '1week';
        } else if (resolution === '1D') {
            _resolution = '1day';
        } else if (Number(resolution) > 30) {
            _resolution = Number(resolution) / 60 + 'hour';
        } else {
            _resolution = resolution + 'min';
        }
        
        // 对偶尔白屏的优化
        if(!this.historyResolution) {
            this.historyResolution = resolution; 
        } else {
            if(this.historyResolution !== resolution) {
//                console.log("分辨率改变了")
                this.historyData = null;
            }
            this.historyResolution = resolution;
        }

//        var formdata = new FormData();
//        formdata.append("symbol", symbolInfo.ticker);
//        formdata.append("type", _resolution);
//        
//        console.log(formdata);
//        
//        var requestParams = {
//             method: 'GET',
//             params: formdata
//        };
        
        var tradeType = $("#tradePath").val();
        if ($("#tradePath").val() == "vtrade") {
        	tradeType = "vcontract";
        } else if ($("#tradePath").val() == "spotTrade") {
        	tradeType = "spot";
        } else if ($("#tradePath").val() == "dtrade") {
        	tradeType = "delivery";
        } else if ($("#tradePath").val() == "trade") {
        	tradeType = "contract";
        }
        
        var isLast = "0";
        var nowTime = (new Date()).getTime();
        if(_this.historyData && (nowTime - _this.lastUpdateTime < 1800000)) {
            isLast = "1";
        }
        
        var requestParams = {
            symbol: symbolInfo.ticker,
            type: tradeType,
            interval: _resolution,
            last: isLast,
        };
        return new Promise(function (resolve, reject) {
        	var newSubStr = "kline." + requestParams.type + '.' + requestParams.symbol + "." + requestParams.interval;
        	if (klineSubStr != newSubStr || requestParams.last == "0") {
        		if (klineSubStr != newSubStr) {
        			unsubKline();
        			subKline(newSubStr);
        		}
	            _this._requester.sendRequest(_this._datafeedUrl, '', requestParams) 
	                .then(function (response) {
	                    
	                	dealData(_this, response, resolve, reject);
	                    
	            }).catch(function (reason) {
	                var reasonString = getErrorMessage(reason);
	                console.warn("HistoryProvider: getBars() failed, error=" + reasonString);
	                reject(reasonString);
	            });
	            
        	} else {
        		if (klineData && klineData.ch == newSubStr) {
            		dealData(_this, klineData.data, resolve, reject);
            	} else { 
	        		 _this._requester.sendRequest(_this._datafeedUrl, '', requestParams) 
		                .then(function (response) {
		                	
		                	dealData(_this, response, resolve, reject);
	
		            }).catch(function (reason) {
		                var reasonString = getErrorMessage(reason);
		                console.warn("HistoryProvider: getBars() failed, error=" + reasonString);
		                reject(reasonString);
		            });
            	}
        	}
            
        });
        
    };
    
    HistoryProvider.prototype.subscribeBars = function (symbolInfo, resolution, newDataCallback, listenerGuid) {

        if (this._subscribers.hasOwnProperty(listenerGuid)) {
            logMessage("DataPulseProvider: already has subscriber with id=" + listenerGuid);
            return;
        }
        this._subscribers[listenerGuid] = {
            lastBarTime: null,
            listener: newDataCallback,
            resolution: resolution,
            symbolInfo: symbolInfo,
        };
        logMessage("DataPulseProvider: subscribed for #" + listenerGuid + " - {" + symbolInfo.name + ", " + resolution + "}");
    };
    HistoryProvider.prototype.unsubscribeBars = function (listenerGuid) {
        delete this._subscribers[listenerGuid];
        logMessage("DataPulseProvider: unsubscribed for #" + listenerGuid);
    };
    
    
    
    return HistoryProvider;
}());

var DataPulseProvider = /** @class */ (function () {
    function DataPulseProvider(historyProvider, updateFrequency) {
        this._subscribers = {};
        this._requestsPending = 0;
        this._historyProvider = historyProvider;
        this._updateFrequency = updateFrequency;
    	this._updateInterval = setInterval(this._updateData.bind(this), this._updateFrequency);
    }
    DataPulseProvider.prototype.stopUpdate = function() {
    	clearInterval(this._updateInterval);
    	this._historyProvider.historyData = null;
    }
    
    DataPulseProvider.prototype.startUpdate = function() {
    	if (this._updateInterval) {
    		this.stopUpdate();
    	}
    	this._updateData();
    	this._updateInterval = setInterval(this._updateData.bind(this), this._updateFrequency);
    }
    DataPulseProvider.prototype.updateData = function() {
    	this._updateData();
    }
    DataPulseProvider.prototype.subscribeBars = function (symbolInfo, resolution, newDataCallback, listenerGuid) {

        if (this._subscribers.hasOwnProperty(listenerGuid)) {
            logMessage("DataPulseProvider: already has subscriber with id=" + listenerGuid);
            return;
        }
        this._subscribers[listenerGuid] = {
            lastBarTime: null,
            listener: newDataCallback,
            resolution: resolution,
            symbolInfo: symbolInfo,
        };
        logMessage("DataPulseProvider: subscribed for #" + listenerGuid + " - {" + symbolInfo.name + ", " + resolution + "}");
    };
    DataPulseProvider.prototype.unsubscribeBars = function (listenerGuid) {
        delete this._subscribers[listenerGuid];
        logMessage("DataPulseProvider: unsubscribed for #" + listenerGuid);
    };
    
    DataPulseProvider.prototype._updateData = function () {
        var this$1 = this;
        var _this = this;
        if (this._requestsPending > 0) {
            return;
        }
        this._requestsPending = 0;
        var _loop_1 = function (listenerGuid) {
            this_1._requestsPending += 1;
            this_1._updateDataForSubscriber(listenerGuid)
                .then(function () {
                _this._requestsPending -= 1;
                logMessage("DataPulseProvider: data for #" + listenerGuid + " updated successfully, pending=" + _this._requestsPending);
            })
                .catch(function (reason) {
                _this._requestsPending -= 1;
                logMessage("DataPulseProvider: data for #" + listenerGuid + " updated with error=" + getErrorMessage(reason) + ", pending=" + _this._requestsPending);
            });
        };
        var this_1 = this;
        for (var listenerGuid in this$1._subscribers) {
            _loop_1(listenerGuid);
        }
    };
    DataPulseProvider.prototype._updateDataForSubscriber = function (listenerGuid) {
        var _this = this;
        var subscriptionRecord = this._subscribers[listenerGuid];
        var rangeEndTime = parseInt((Date.now() / 1000).toString());
        // BEWARE: please note we really need 2 bars, not the only last one
        // see the explanation below. `10` is the `large enough` value to work around holidays
        var rangeStartTime = rangeEndTime - periodLengthSeconds(subscriptionRecord.resolution, 10);
        return this._historyProvider.getBars(subscriptionRecord.symbolInfo, subscriptionRecord.resolution, rangeStartTime, rangeEndTime)
            .then(function (result) {
            _this._onSubscriberDataReceived(listenerGuid, result);
        });
    };
    DataPulseProvider.prototype._onSubscriberDataReceived = function (listenerGuid, result) {
        // means the subscription was cancelled while waiting for data
        if (!this._subscribers.hasOwnProperty(listenerGuid)) {
            logMessage("DataPulseProvider: Data comes for already unsubscribed subscription #" + listenerGuid);
            return;
        }
        var bars = result.bars;
        if (bars.length === 0) {
            return;
        }
        var lastBar = bars[bars.length - 1];
        var subscriptionRecord = this._subscribers[listenerGuid];
        if (subscriptionRecord.lastBarTime !== null && lastBar.time < subscriptionRecord.lastBarTime) {
            return;
        }
        var isNewBar = subscriptionRecord.lastBarTime !== null && lastBar.time > subscriptionRecord.lastBarTime;
        // Pulse updating may miss some trades data (ie, if pulse period = 10 secods and new bar is started 5 seconds later after the last update, the
        // old bar's last 5 seconds trades will be lost). Thus, at fist we should broadcast old bar updates when it's ready.
        if (isNewBar) {
            if (bars.length < 2) {
                throw new Error('Not enough bars in history for proper pulse update. Need at least 2.');
            }
            var previousBar = bars[bars.length - 2];
            subscriptionRecord.listener(previousBar);
        }
        subscriptionRecord.lastBarTime = lastBar.time;
        subscriptionRecord.listener(lastBar);
    };
    return DataPulseProvider;
}());
function periodLengthSeconds(resolution, requiredPeriodsCount) {
    var daysCount = 0;
    if (resolution === 'D' || resolution === '1D') {
        daysCount = requiredPeriodsCount;
    }
    else if (resolution === 'M' || resolution === '1M') {
        daysCount = 31 * requiredPeriodsCount;
    }
    else if (resolution === 'W' || resolution === '1W') {
        daysCount = 7 * requiredPeriodsCount;
    }
    else {
        daysCount = requiredPeriodsCount * parseInt(resolution) / (24 * 60);
    }
    return daysCount * 24 * 60 * 60;
}

var QuotesPulseProvider = /** @class */ (function () {
    function QuotesPulseProvider(quotesProvider) {
        this._subscribers = {};
        this._requestsPending = 0;
        this._quotesProvider = quotesProvider;
        setInterval(this._updateQuotes.bind(this, 1 /* Fast */), 10000 /* Fast */);
        setInterval(this._updateQuotes.bind(this, 0 /* General */), 60000 /* General */);
    }
    QuotesPulseProvider.prototype.subscribeQuotes = function (symbols, fastSymbols, onRealtimeCallback, listenerGuid) {
        this._subscribers[listenerGuid] = {
            symbols: symbols,
            fastSymbols: fastSymbols,
            listener: onRealtimeCallback,
        };
        logMessage("QuotesPulseProvider: subscribed quotes with #" + listenerGuid);
    };
    QuotesPulseProvider.prototype.unsubscribeQuotes = function (listenerGuid) {
        delete this._subscribers[listenerGuid];
        logMessage("QuotesPulseProvider: unsubscribed quotes with #" + listenerGuid);
    };
    QuotesPulseProvider.prototype._updateQuotes = function (updateType) {
        var this$1 = this;

        var _this = this;
        if (this._requestsPending > 0) {
            return;
        }
        var _loop_1 = function (listenerGuid) {
            this_1._requestsPending++;
            var subscriptionRecord = this_1._subscribers[listenerGuid];
            this_1._quotesProvider.getQuotes(updateType === 1 /* Fast */ ? subscriptionRecord.fastSymbols : subscriptionRecord.symbols)
                .then(function (data) {
                _this._requestsPending--;
                if (!_this._subscribers.hasOwnProperty(listenerGuid)) {
                    return;
                }
                subscriptionRecord.listener(data);
                logMessage("QuotesPulseProvider: data for #" + listenerGuid + " (" + updateType + ") updated successfully, pending=" + _this._requestsPending);
            })
                .catch(function (reason) {
                _this._requestsPending--;
                logMessage("QuotesPulseProvider: data for #" + listenerGuid + " (" + updateType + ") updated with error=" + getErrorMessage(reason) + ", pending=" + _this._requestsPending);
            });
        };
        var this_1 = this;
        for (var listenerGuid in this$1._subscribers) {
            _loop_1(listenerGuid);
        }
    };
    return QuotesPulseProvider;
}());

function extractField(data, field, arrayIndex) {
    var value = data[field];
    return Array.isArray(value) ? value[arrayIndex] : value;
}

/**
 * This class implements interaction with UDF-compatible datafeed.
 * See UDF protocol reference at https://github.com/tradingview/charting_library/wiki/UDF
 */
var UDFCompatibleDatafeedBase = /** @class */ (function () {
    function UDFCompatibleDatafeedBase(datafeedURL, quotesProvider, requester, updateFrequency) {
        if (updateFrequency === void 0) { updateFrequency = 10 * 1000; }
        this._configuration = defaultConfiguration();
        this._datafeedURL = datafeedURL;
        this._requester = requester;
        this._historyProvider = new HistoryProvider(datafeedURL, this._requester);
        this._quotesProvider = quotesProvider;
        this._dataPulseProvider = new DataPulseProvider(this._historyProvider, updateFrequency);
        this._quotesPulseProvider = new QuotesPulseProvider(this._quotesProvider);
    }
    UDFCompatibleDatafeedBase.prototype.onReady = function (callback) {
        var config = {
            'supports_search': true,
            'supports_group_request': false,
            'supported_resolutions': ['1', '3', '5', '15', '30', '60', '120', '240', '360', '720', '1D', '1W', '1M'],
            'supports_marks': false,
            'supports_time': false
        };
        setTimeout(function () {
            callback(config);
        });
    };
    UDFCompatibleDatafeedBase.prototype.startUpdate = function() {
    	this._dataPulseProvider.startUpdate();
    };
    UDFCompatibleDatafeedBase.prototype.updateData = function() {
    	this._dataPulseProvider.updateData();
    };
    UDFCompatibleDatafeedBase.prototype.stopUpdate = function() {
    	this._dataPulseProvider.stopUpdate();
    };
    UDFCompatibleDatafeedBase.prototype.getQuotes = function (symbols, onDataCallback, onErrorCallback) {
        this._quotesProvider.getQuotes(symbols).then(onDataCallback).catch(onErrorCallback);
    };
    UDFCompatibleDatafeedBase.prototype.subscribeQuotes = function (symbols, fastSymbols, onRealtimeCallback, listenerGuid) {
        this._quotesPulseProvider.subscribeQuotes(symbols, fastSymbols, onRealtimeCallback, listenerGuid);
    };
    UDFCompatibleDatafeedBase.prototype.unsubscribeQuotes = function (listenerGuid) {
        this._quotesPulseProvider.unsubscribeQuotes(listenerGuid);
    };
    UDFCompatibleDatafeedBase.prototype.calculateHistoryDepth = function (resolution, resolutionBack, intervalBack) {
        return undefined;
    };
    UDFCompatibleDatafeedBase.prototype.getMarks = function (symbolInfo, startDate, endDate, onDataCallback, resolution) {
        if (!this._configuration.supports_marks) {
            return;
        }
        var requestParams = {
            symbol: symbolInfo.ticker || '',
            from: startDate,
            to: endDate,
            resolution: resolution,
        };
        this._send('marks', requestParams)
            .then(function (response) {
            if (!Array.isArray(response)) {
                var result = [];
                for (var i = 0; i < response.id.length; ++i) {
                    result.push({
                        id: extractField(response, 'id', i),
                        time: extractField(response, 'time', i),
                        color: extractField(response, 'color', i),
                        text: extractField(response, 'text', i),
                        label: extractField(response, 'label', i),
                        labelFontColor: extractField(response, 'labelFontColor', i),
                        minSize: extractField(response, 'minSize', i),
                    });
                }
                response = result;
            }
            onDataCallback(response);
        })
            .catch(function (error) {
            logMessage("UdfCompatibleDatafeed: Request marks failed: " + getErrorMessage(error));
            onDataCallback([]);
        });
    };
    UDFCompatibleDatafeedBase.prototype.getTimescaleMarks = function (symbolInfo, startDate, endDate, onDataCallback, resolution) {
        if (!this._configuration.supports_timescale_marks) {
            return;
        }
        var requestParams = {
            symbol: symbolInfo.ticker || '',
            from: startDate,
            to: endDate,
            resolution: resolution,
        };
        this._send('timescale_marks', requestParams)
            .then(function (response) {
            if (!Array.isArray(response)) {
                var result = [];
                for (var i = 0; i < response.id.length; ++i) {
                    result.push({
                        id: extractField(response, 'id', i),
                        time: extractField(response, 'time', i),
                        color: extractField(response, 'color', i),
                        label: extractField(response, 'label', i),
                        tooltip: extractField(response, 'tooltip', i),
                    });
                }
                response = result;
            }
            onDataCallback(response);
        })
            .catch(function (error) {
            logMessage("UdfCompatibleDatafeed: Request timescale marks failed: " + getErrorMessage(error));
            onDataCallback([]);
        });
    };
    UDFCompatibleDatafeedBase.prototype.getServerTime = function (callback) {
        if (!this._configuration.supports_time) {
            return;
        }
        this._send('time')
            .then(function (response) {
            var time = parseInt(response);
            if (!isNaN(time)) {
                callback(time);
            }
        })
            .catch(function (error) {
            logMessage("UdfCompatibleDatafeed: Fail to load server time, error=" + getErrorMessage(error));
        });
    };
    UDFCompatibleDatafeedBase.prototype.searchSymbols = function (userInput, exchange, symbolType, onResult) {
    };
    UDFCompatibleDatafeedBase.prototype.resolveSymbol = function (symbolName, onResolve, onError) {
        var _this = this;
        var symbolInfo = {
            name: symbolName,
            full_name: symbolName,
            ticker: symbolName,
            description: symbolName,
            type: '数字货币',
            session: '24x7',
            exchange: '',
            listed_exchange: '',
            timezone: 'Asia/Shanghai',
            pricescale: Math.pow(10, 8),
            minmov: 1,
            pointvalue: 1,
            has_intraday: true,
            supported_resolutions: ['1', '5', '15', '30', '60', '120', '240', '360', '720', '1D', '1W', '1M'],
            intraday_multipliers: ['1', '5', '15', '30', '60', '120', '240', '360', '720'],
            has_daily: true,
            has_weekly_and_monthly: true,
            has_empty_bars: false,
            has_no_volume: false,
        };
        
//        var formdata = new FormData();
//        formdata.append("symbol", symbolName);
//        formdata.append("type", '1min');
        
//        var requestParams = {
//             method: 'POST',
//             params: formdata
//        };
        
        var tradeType = $("#tradePath").val();
        if ($("#tradePath").val() == "vtrade") {
        	tradeType = "vcontract";
        } else if ($("#tradePath").val() == "spotTrade") {
        	tradeType = "spot";
        } else if ($("#tradePath").val() == "dtrade") {
        	tradeType = "delivery";
        } else if ($("#tradePath").val() == "trade") {
        	tradeType = "contract";
        }
        
        var requestParams = {
            symbol: symbolName,
            type: tradeType,
            interval: 'default'
        };
        
        this._requester.sendRequest(this._historyProvider._datafeedUrl, '', requestParams) 
            .then(function (response) {
                symbolInfo.pricescale = Math.pow(10, Number(response.digits)) || Math.pow(10, 8);
                onResolve(symbolInfo);
            });
        
    };
    UDFCompatibleDatafeedBase.prototype.getBars = function (symbolInfo, resolution, rangeStartDate, rangeEndDate, onResult, onError) {
        
        this._historyProvider.getBars(symbolInfo, resolution, rangeStartDate, rangeEndDate)
            .then(function (result) {
            onResult(result.bars, result.meta);
        })
            .catch(onError);
    };
    UDFCompatibleDatafeedBase.prototype.subscribeBars = function (symbolInfo, resolution, onTick, listenerGuid, onResetCacheNeededCallback) {
        this._dataPulseProvider.subscribeBars(symbolInfo, resolution, onTick, listenerGuid);
    };
    UDFCompatibleDatafeedBase.prototype.unsubscribeBars = function (listenerGuid) {
        this._dataPulseProvider.unsubscribeBars(listenerGuid);
    };
    UDFCompatibleDatafeedBase.prototype._requestConfiguration = function () {
        return this._send('config')
            .catch(function (reason) {
            logMessage("UdfCompatibleDatafeed: Cannot get datafeed configuration - use default, error=" + getErrorMessage(reason));
            return null;
        });
    };
    UDFCompatibleDatafeedBase.prototype._send = function (urlPath, params) {
        return this._requester.sendRequest(this._datafeedURL, urlPath, params);
    };
    return UDFCompatibleDatafeedBase;
}());
function defaultConfiguration() {
    return {
        supports_search: false,
        supports_group_request: true,
        supported_resolutions: ['1', '5', '15', '30', '60', '120', '240', '360', '720', '1D', '1W', '1M'],
        supports_marks: false,
        supports_timescale_marks: false,
    };
}

var QuotesProvider = /** @class */ (function () {
    function QuotesProvider(datafeedUrl, requester) {
        this._datafeedUrl = datafeedUrl;
        this._requester = requester;
    }
    QuotesProvider.prototype.getQuotes = function (symbols) {
        var _this = this;
        return new Promise(function (resolve, reject) {
            _this._requester.sendRequest(_this._datafeedUrl, 'quotes', { symbols: symbols })
                .then(function (response) {
                if (response.s === 'ok') {
                    resolve(response.d);
                }
                else {
                    reject(response.errmsg);
                }
            })
                .catch(function (error) {
                var errorMessage = getErrorMessage(error);
                logMessage("QuotesProvider: getQuotes failed, error=" + errorMessage);
                reject("network error: " + errorMessage);
            });
        });
    };
    return QuotesProvider;
}());

var Requester = /** @class */ (function () {
    function Requester(headers) {
        if (headers) {
            this._headers = headers;
        }
    }
    Requester.prototype.sendRequest = function (datafeedUrl, urlPath, params) {
        if (params !== undefined && params.method !== 'POST') {
            var paramKeys = Object.keys(params);
            if (paramKeys.length !== 0) {
                urlPath += '?';
            }
            urlPath += paramKeys.map(function (key) {
                return encodeURIComponent(key) + "=" + encodeURIComponent(params[key].toString());
            }).join('&');
        }
        logMessage('New request: ' + urlPath);
        // Send user cookies if the URL is on the same origin as the calling script.
        var options = { credentials: 'same-origin' };
        if (this._headers !== undefined) {
            options.headers = this._headers;
        }
        if (params && params.method === 'POST') {
            options.method = params.method;
            
            options.body = params.body;
        }
        
//        console.log(options)
//        options.headers = {
//   　　　　 		'Content-Type': 'application/x-www-form-urlencoded'
//        };
      
        return fetch(datafeedUrl + urlPath, options)
            .then(function (response) {
                var _response = response.json();
                return _response;
            })
            .then(function (responseTest) { return responseTest; });
    };
    return Requester;
}());

var UDFCompatibleDatafeed = /** @class */ (function (_super) {
    __extends(UDFCompatibleDatafeed, _super);
    function UDFCompatibleDatafeed(datafeedURL, updateFrequency) {
        if (updateFrequency === void 0) { updateFrequency = 10 * 1000; }
        var _this = this;
        var requester = new Requester();
        var quotesProvider = new QuotesProvider(datafeedURL, requester);
        _this = _super.call(this, datafeedURL, quotesProvider, requester, updateFrequency) || this;
        return _this;
    }
    return UDFCompatibleDatafeed;
}(UDFCompatibleDatafeedBase));

exports.UDFCompatibleDatafeed = UDFCompatibleDatafeed;

Object.defineProperty(exports, '__esModule', { value: true });

})));
