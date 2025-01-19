function Fractional(n, bit) {
	// 数字转为字符串
	n = n.toString();

	// 获取小数点位置
	var point = n.indexOf('.');

	if (point < 0) {
		return n + ".000000".substring(0, bit + (bit == 0 ? 0 : 1));
	} else {
		var vleng = point + bit + (bit == 0 ? 0 : 1);
		if (n.length > vleng) {

			return n.substring(0, vleng);
		}
		else{
			n=n+"00000";
			return n.substring(0, vleng);
		}

	}
	return n;
}

function getLocalTime(nS) {
	var date = new Date(parseInt(nS) * 1000);
	var hour = date.getHours() + "";
	if (hour.length < 2) {
		hour = "0" + hour;
	}
	var minutes = date.getMinutes() + "";
	if (minutes.length < 2) {
		minutes = "0" + minutes;
	}
	var seconds = date.getSeconds() + "";
	if (seconds.length < 2) {
		seconds = "0" + seconds;
	}
	return hour + ":" + minutes + ":" + seconds;
}


String.prototype.replaceAll = function(s1,s2){ 
	var ret = this.replace(new RegExp(s1,"gm"),s2);
	return ret;
}

var prevLastPrice = 0;
var showSuspend = false;
var jgws=-1;
var slws=$("amountDigits").val();
function updateCommonData(data) {
	if (data == null) return;
	
	var coinPairName = $('#coinPairName').val();
	if (data.suspend && !showSuspend) {
		showSuspend = true;
		$(".suspend-div").show();
		$(".normal-trade-div").hide();
		tipAlert(coinPairName.toUpperCase() + MSG['suspend1']);
	} else if (!data.suspend) {
		showSuspend = false;
		$(".suspend-div").hide();
		$(".normal-trade-div").show();
	}
	
	var digits = data.digits;
	jgws=digits;
	slws=data.amountDigits;
	$("#todayHighCoin").html(data.todayHigh);
	$("#todayLowCoin").html(data.todayLow);
	$("#tradesCoin").html(data.trades + " USDT");
	
	if (data.lastPrice && data.lastPrice != prevLastPrice) {
		var lastPriceType = "buy";
		if (data.lastPrice < prevLastPrice) {
			lastPriceType = "sell";
		}
		prevLastPrice = data.lastPrice;
		if (klineData) {
			var name = $($("#chart_toolbar_periods_vert li a.selectsed")[0]).parent().attr("name");
			klineData.data.data[klineData.data.data.length - 1].last = prevLastPrice;
			if (klineViewType == 0 && typeof(dealKlineData) == "function") {
				dealKlineData(klineData.data.data, name);
			}
			
			if (klineViewType == 1 && typeof(updateTradingView) == "function") {
				updateTradingView();
			}
		}
		
		$("#lastPriceFull").html("<span class='" + lastPriceType + "'>" + data.lastPrice + "</span>");
		
		if (currFirstPrice > 0) {
			var fudu = (data.lastPrice - currFirstPrice) / currFirstPrice * 100;
			var fuduClass = "sell";
			if (fudu >= 0) {
				fuduClass = "buy";
			}

			isLastPriceChange = true;
			$($(".contract-list-li.later").children(".contract-list-cell-price")[0]).html("<span class='" + fuduClass + "'>" + data.lastPrice + "</span>");
			$($(".contract-list-li.later").children(".contract-list-cell-fudu")[0]).html("<span class='" + fuduClass + "'>" + fudu.toFixed(2) + "%</span>");
			
			$("#lastPrice").html("<span class='" + fuduClass + "'>" + data.lastPrice + "</span>");
		}
	}
	
	if (data.lastPrice) {
		document.title = parseFloat(data.lastPrice) + coinPairName.toUpperCase() + MSG['systeam1'];
	}

}

function updateDepth(data) {
	if (jgws < 0) return;
	
	var selltop5List = data.asks;
	var buytop5List = data.bids;
	
	var price = "";
	var sumNum = "";
	var countNum = "";
	var i = 0;
	var j = 0;
	var sellSize = selltop5List.length;
	var buySize = buytop5List.length;
	var maxCount = 0;
	var sellSum = 0;
	for (var i = 0; i < sellSize; i++) {
		var sd = selltop5List[i];
		var count = sd[1];
		if (count > maxCount) {
			maxCount = count;
		}
		sellSum += count;
	}
	
	var linesell = "";
	for (var i = sellSize - 1; i >= 0; i--) {
		var sd = selltop5List[i];
		if (jgws >= 0) {
			price = formatnumber(sd[0], jgws)
			countNum = formatnumber(sd[1], slws);
			sumNum = formatnumber(sellSum, slws);
		} else {
			price = sd[0];
			countNum = sd[1];
			sumNum = sellSum;
		}
		sellSum -= sd[1];
		
		var progress = 0;
		if (maxCount > 0) {
			progress = sd[1] / maxCount * 100;
		}
		
		linesell = linesell + "<tr onclick=\"trade_price_market(2,'" + price + "', '"+ sumNum +"');\"  ><td><span class='sell'> "+ price + "</span></td><td> " + countNum + "</td>";
		linesell = linesell + "<td> " + sumNum + "</td><td style='width:" + progress + "%;' class='progress'></td></tr>";
	}

	maxCount = 0;
	for (var i = 0; i < buySize; i++) {
		var sd = buytop5List[i];
		var count = sd[1];
		if (count > maxCount) {
			maxCount = count;
		}
	}
	
	var linebuy = "";
	var buyCount = 0;
	for (var i = 0; i < buySize; i++) {
		var sd = buytop5List[i];
		buyCount += sd[1]
		
		if (jgws >= 0) {
			price = formatnumber(sd[0], jgws);
			countNum = formatnumber(sd[1], slws);
			sumNum = formatnumber(buyCount, slws);
		} else {
			price = sd[0];
			countNum = sd[1];
			sumNum = buyCount;
		}
		
		var progress = 0;
		if (maxCount > 0) {
			progress = sd[1] / maxCount * 100;
		}
		linebuy = linebuy + "<tr onclick=\"trade_price_market(1,'" + price + "', '"+ sumNum +"');\" ><td><span class='buy'> "+ price + "</span></td><td> " + countNum + "</td>";
		linebuy = linebuy + "<td>  " + sumNum + "</td><td style='width:" + progress + "%;' class='buy-progress'></td></tr>";
	}
	$("#sellTbody").html(linesell);
	$("#buyTbody").html(linebuy);
}

function updateTrades(data) {
	if (!data) return;

	var digits = parseInt(data.digits);
	var amountDigits = parseInt(data.amountDigits);
	var trades = data.data;
	if (trades != null) {
		var lineorder = "";
		for ( var i = 0; i < trades.length; i++) {
			var date = trades[i].date;
			var amount = trades[i].amount;
			var price = trades[i].price;
			var tradeType = trades[i].type;
			var change = 0;
			if (type != "spot") {
				change = parseFloat(trades[i].change);
			} else {
				change = parseFloat(trades[i].vol);
			}
			
			price = Fractional(price, digits);
			amount = Fractional(amount, amountDigits);

			lineorder += "<tr><td style='width:33.3333%'>" + getLocalTime(date) + "</td>" +
					"<td  style='width:33.3333%' class='" + tradeType + "'>" + price + "</td>" +
					"<td style='text-align:right;width:33.3334%'><span>"
					+ amount + "</span></td></tr>";
		}
		
		$("#ordertbody").html(lineorder);
	}
}

function updatepriceCoin() {
	var coinPairName = $('#coinPairName').val();
	$.ajax({
        type: "GET",    
        url: basePath +"spotTradeAjax/getCommonData.do",
        data: {tradePair: coinPairName},
        dataType: "json",  
        success: function(data) {
        },
        error: function(msg) {
        	
        }
	});
}

function tradesRefresh() {
	var bqCode = $('#coinPairName').val();
	var url = basePath + "futuresApi/trades.do?type=spot&symbol=" + bqCode + "&bfx=1&ra=" + Math.round(Math.random() * 100);
	$.ajax( {
		type : 'GET',
		url : url,
		data : {},
		dataType : "text",
		cache : false,
		statusCode : {
			404 : function() {// 404错误
			}
		},
		success : function(data) {
			var data = eval('(' + data + ')');

			if (data.code == 0) {
				updateTrades(data);
			}
			
			marketEntrustTime = setTimeout(tradesRefresh, 5000);
		},
		error : function(msg) {
			marketEntrustTime = setTimeout(tradesRefresh, 5000);
		}
	});
}


function initEventHandle() {
	ws.onclose = function() {
		console.log("ws close: ");
		reconnect();
	};
	ws.onerror = function() {
		console.log("ws error: ");
		reconnect();
	};
	ws.onopen = function() {
		//开启
		isOpen = true;
		var code = $('#coinPairName').val();
		var type  = "spot";
		
		var sub = {
			'sub' : 'trade.' + type + '.' + code,
		}
		
		var req = {
			'req' : 'trade.' + type + '.' + code,
		}
		
		var subDepth = {
			'sub' : 'market.' + type + '.' + code + '.depth',
		}
			
		sendMessage(JSON.stringify(subDepth));
		sendMessage(JSON.stringify(req));
		sendMessage(JSON.stringify(sub));
		
		if (klineSubStr) {
			var subKline = {
				'sub' : klineSubStr
			}
			
			sendMessage(JSON.stringify(subKline));
		}
	};

	ws.onmessage = function(e) {
		//收到消息
		var msg = e.data;
		var obj = eval('(' + msg + ')');

		if (obj.ping != null) {
			var msg = {
				'pong' : obj.ping
			}
			keepAlive(JSON.stringify(msg));
		}

		if (obj.data != null) {
			var code = $('#transaction_code').val();
			var type = $('#tradePath').val(); // "contract";
			var tradeType = $('#tradePath').val();
			if (tradeType == "vtrade") {
				type = "vcontract";
			} else if (tradeType == "spotTrade") {
				type = "spot";
			} else if (tradeType == "dtrade") {
				type = "delivery";
			} else if (tradeType == "trade") {
				type = "contract";
			}

			var str = obj.ch;
			if (!str) str = obj.rep;
			
			var ch = 'trade.' + type + '.' + code;

			if (ch == str) {
				handleData(obj.data);
			} else if (str == ('market.' + type + '.' + code + '.depth')) {
				updateDepth(obj.data.data);
			} else if (klineSubStr == str) {
				klineData = obj;
				if (prevLastPrice > 0) {
					var len = klineData.data.data.length;
					if (len > 0) {
						klineData.data.data[len - 1].last = prevLastPrice;
					}
				}
			}
		}
	}
}

function handleData(data) {
	if (data.tradesdata) {
		updateTrades(data.tradesdata);
	}
	
	if (data.commondata) {
		updateCommonData(data.commondata.data);
	}

	if (data.depthdata) {
		depthData = data.depthdata.data;
	}
	
	if (data.tickersdata) {
		sortContractList(data.tickersdata.data);
	}
}

function subKline(subStr) {
	klineData = null;
	prevLastPrice = 0;
	klineSubStr = subStr;
	
	if (ws && isOpen) {
		var subKline = {
			'sub' : klineSubStr
		}
		
		sendMessage(JSON.stringify(subKline));
	}
}

function unsubKline() {
	if (klineSubStr && ws && isOpen) {
		var unsub = {
			'unsub' : klineSubStr
		}
		sendMessage(JSON.stringify(unsub));
	}
	
	klineData = null;
	klineSubStr = null;
}

function sendMessage(msg) {
	ws.send(msg);
}

function keepAlive(msg) {
	sendMessage(msg);
}

function reconnect() {
	if (lockReconnect) return;
	lockReconnect = true;
	isOpen = false;
	//没连接上会一直重连，设置延迟避免请求过多
	setTimeout(function() {
		createWebSocket();
		lockReconnect = false;
	}, 2000);
	
	console.log("reconnect...");
}


function createWebSocket() {
	try {
		ws = new WebSocket(wsUrl);
		initEventHandle();
	} catch (e) {
		reconnect();
	}
}

//var marketEntrustTime = setTimeout(tradesRefresh, 5000);
//var stockCoindata = setTimeout(updatepriceCoin, 2000);

var ws;
var isOpen = false;
var wsUrl;
var lockReconnect = false;
var klineSubStr;
var klineData;

$(function() {
	var index = basePath.lastIndexOf("//");
	var path = basePath.substr(index, basePath.length)
	var wssPath = "wss:" + path + "ws";
	var wsPath = "ws:" + path + "ws";
	wsUrl = location.protocol == "https:" ? wssPath : wsPath;

	createWebSocket();
});
