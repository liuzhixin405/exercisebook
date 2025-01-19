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

var fundingFeeRunningTimeArray = [];
//资金倒计时
function getTime() {
	var transaction_code = $('#transaction_code').val();
	var path = "fundingFee/getRunningTime.do";
	var tradePath = $('#tradePath').val();
	if (tradePath == "vtrade") {
		path = "vfundingFee/getRunningTime.do";
	} else if (tradePath == "trade") {
		path = "fundingFee/getRunningTime.do";
	} else {
		path = tradePath + "/fundingFee/getRunningTime.do";
	}
	$.ajax({
		type : "GET",
		dataType : "text",
		url : basePath + path,
		data : {
			symbol : transaction_code
		},
		success : function(data) {
			var data = eval('(' + data + ')');
			if (data.time) {
				fundingFeeRunningTimeArray = data.time.split(",");
				orderbyRunningTime(fundingFeeRunningTimeArray);
				getNextRunningTime(fundingFeeRunningTimeArray);
			}
		}
	});
}

function orderbyRunningTime(array) {
	array = fundingFeeRunningTimeArray.sort();
}

function getNextRunningTime(array) {
	var endTime = null;
	for (i = 0; i < array.length; i++) {
		var endtimeStr = array[i];
		var now = new Date(); //设定开始时间(等于系统当前时间)
		var nowStr = now.getHours() + ":" + now.getMinutes() + ":" + now.getSeconds();
		
		endTime = compare(nowStr, endtimeStr);
		if (endTime) {
			break;
		}
	}
	if(endTime == null) {
		endTime = compare(nowStr, array[0]+ ':00', true);
	}
	return endTime;
}

function compare(time_s, time_e, flag=false) {
	var D = new Date();
	var now = D.getFullYear() + '/' + (D.getMonth() + 1) + '/' + D.getDate();
	var s = Date.parse(now + ' ' + time_s);
	if(flag) {
		now = D.getFullYear() + '/' + (D.getMonth() + 1) + '/' + (D.getDate()+1);
	}
	var e = Date.parse(now + ' ' + time_e);
	if (e > s) {
		return now + ' ' + time_e;
	} else {
		return null;
	}
}

function showTime() {
	var endDateStr  =  getNextRunningTime(fundingFeeRunningTimeArray);
	if (!endDateStr) {
		return;
	}
	
	var time_end = new Date(endDateStr).getTime(); 
	var time_start = new Date(); //设定开始时间(等于系统当前时间)

	//计算时间差 
	var time_distance = time_end - time_start;
	if (time_distance > 0) {

		var int_hour = Math.floor(time_distance / 3600000)
		time_distance -= int_hour * 3600000;

		var int_minute = Math.floor(time_distance / 60000)
		time_distance -= int_minute * 60000;

		var int_second = Math.floor(time_distance / 1000)

		// 时分秒为单数时,前面加零 
		if (int_hour < 10) {
			int_hour = "0" + int_hour;
		}
		if (int_minute < 10) {
			int_minute = "0" + int_minute;
		}
		if (int_second < 10) {
			int_second = "0" + int_second;
		}
		// 显示时间 
		$("#time_hour").html(int_hour);
		$("#time_minute").html(int_minute);
		$("#time_second").html(int_second);

	} else {
		$("#time_hour").html('00');
		$("#time_minute").html('00');
		$("#time_second").html('00');
	}
}

String.prototype.replaceAll = function(s1,s2){ 
	var ret = this.replace(new RegExp(s1,"gm"),s2);
	return ret;
}

var prevLastPrice = 0;
var showSuspend = false;
var jgws=$("myDigits").val();
var slws=$("amountDigits").val();
function updateCommonData(data) {
	if (data == null) return;
	
	var transaction_code = $('#transaction_code').val();
	var transaction_type_code = $('#transaction_type_code').val();
	var tradePath = $('#tradePath').val();
	var coeffcient = $('#coeffcient').val();
	
	if(transaction_code.length == 0){
		return;
	}
	
	if (data.suspend && !showSuspend) {
		showSuspend = true;
		$(".suspend-text").html(MSG['tradeSuspend']);
		$(".suspend-div").show();
		$(".normal-trade-div").hide();
		tipAlert(transaction_code.toUpperCase() + MSG['suspend']);
	} else if (!data.tradeTime) {
		$(".suspend-text").html(MSG['noTradeTime']);
		$(".suspend-div").show();
		$(".normal-trade-div").hide();
	} else if (!data.suspend) {
		showSuspend = false;
		$(".suspend-div").hide();
		$(".normal-trade-div").show();
	}
	
//	var up = true;
//	if (data.indexFudu.indexOf("-") == 0) {
//		up = false;
//	}
	
	$("#" + transaction_type_code + "-bpi2").html("<span class='" + ( indexFuduUp ? "buy" : "sell") + "'>" + data.btc_index_price + "</span>");
	
	if (data.lastPrice != prevLastPrice) {
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
		
		$("#lastPriceFull").html("<span class='" + lastPriceType + "' id='lastPriceSpan'>" + data.lastPrice + "</span>");
		if($(".right-cont-top-item-alone").css("display") == "block") {
			$("#lastPriceFullOnly").html("<span class='" + lastPriceType + "' id='lastPriceSpanOnly'>" + data.lastPrice + "</span>");
		}
		if (currFirstPrice > 0) {
			var fudu = (data.lastPrice - currFirstPrice) / currFirstPrice * 100;
			var fuduClass = "sell";
			if (fudu >= 0) {
				fuduClass = "buy";
			}

			isLastPriceChange = true;
			$($(".contract-list-li.later").children(".contract-list-cell-price")[0]).html("<span class='" + fuduClass + "'>" + data.lastPrice + "</span>");
			$($(".contract-list-li.later").children(".contract-list-cell-fudu")[0]).html("<span class='" + fuduClass + "'>" + fudu.toFixed(2) + "%</span>");
		}
		
		// 计算浮动盈亏
		var buyLen = $("#depotbuyTbody tr").length - 1;
		for (var i = 0; i < buyLen; i++) {
			var houdCountStr = $("#buyHoldCount" + i).html();
			if (houdCountStr == null || typeof(houdCountStr) == undefined) {
				continue;
			}
			
			var lockCountStr = $("#buyLockCount" + i).html();
			var buyDepositStr = $("#buyDeposit" + i).html();
			var holdCount = parseFloat(houdCountStr.replaceAll(",",""));
			var lockCount = parseFloat(lockCountStr.replaceAll(",",""));
			var avgPrice = $("#buy_accuratePrice" + i).val();
			var deposit = parseFloat(buyDepositStr.replaceAll(",",""));
			
			var ykdiffStr = "0";
			var ykRateStr = "0%";
			var ykRate = 0, ykdiff = 0;	// 盈亏变动
			ykdiff = (data.lastPrice - avgPrice) * (holdCount + lockCount) * coeffcient;
			if (deposit > 0) 
				ykRate = ykdiff / deposit;
			ykdiff = ykdiff.toFixed(2);
			ykRate = (ykRate * 100).toFixed(2);
			
			ykdiffStr = ykdiff + "";
			ykRateStr = ykRate + "%";
			if (ykdiff < 0) {
				$("#floatGainLossbuy" + i).removeClass("buy");
				$("#floatGainLossbuy" + i).addClass("sell");
			} else {
				$("#floatGainLossbuy" + i).removeClass("sell");
				$("#floatGainLossbuy" + i).addClass("buy");
			}
			
			$("#floatGainLossbuy" + i).html(ykdiffStr);
			$("#floatGainLossRatebuy" + i).html(ykRateStr);
		}
		
		var sellLen = $("#depotsellTbody tr").length - 1;
		for (var i = 0; i < sellLen; i++) {
			var houdCountStr = $("#sellHoldCount" + i).html();
			
			if (houdCountStr == null || typeof(houdCountStr) == undefined) {
				continue;
			}
			
			var lockCountStr = $("#sellLockCount" + i).html();
			var sellDepositStr = $("#sellDeposit" + i).html();
			var holdCount = parseFloat(houdCountStr.replaceAll(",",""));
			var lockCount = parseFloat(lockCountStr.replaceAll(",",""));
			var avgPrice = $("#sell_accuratePrice" + i).val();
			var deposit = parseFloat(sellDepositStr.replaceAll(",",""));
			
			var ykdiffStr = "0";
			var ykRateStr = "0%";
			var ykRate = 0, ykdiff = 0;	// 盈亏变动
			ykdiff = (avgPrice - data.lastPrice) * (holdCount + lockCount) * coeffcient;
			if (deposit > 0) 
				ykRate = ykdiff / deposit;
			ykdiff = ykdiff.toFixed(2);
			ykRate = (ykRate * 100).toFixed(2);
			
			ykdiffStr = ykdiff + "";
			ykRateStr = ykRate + "%";
			if (ykdiff < 0) {
				$("#floatGainLosssell" + i).removeClass("buy");
				$("#floatGainLosssell" + i).addClass("sell");
			} else {
				$("#floatGainLosssell" + i).removeClass("sell");
				$("#floatGainLosssell" + i).addClass("buy");
			}
			
			$("#floatGainLosssell" + i).html(ykdiffStr);
			$("#floatGainLossRatesell" + i).html(ykRateStr);
		}
	}
	
	var digits = data.digits;
	$("#lastPrice").html(data.lastPrice);
	$("#btc_index_price").val(data.btc_index_price);

	jgws=digits;
	slws=data.amountDigits;
	
	$("#todayHigh").html(data.todayHigh);
	$("#todayLow").html(data.todayLow);
	if  (locale == "zh_CN") {
		$("#tradevols").html(data.tradevols + " 个");
	} else if (locale == "zh_HK") {
		$("#tradevols").html(data.tradevols + " 個");
	}else {
		$("#tradevols").html(data.tradevols);
	}
	
	var volume = data.trades;
	if  (locale == "zh_CN" || locale == "zh_HK") {
		if (volume > 10000) {
			volume /= 10000;
			volume = formatnumber(volume, 2);
			volume += "万";
		}
	} else {
		 if (volume > 1000000) {
			volume /= 1000000;
			volume = formatnumber(volume, 2);
			volume += "M";
		} else if (volume > 1000) {
			volume /= 1000;
			volume = formatnumber(volume, 2);
			volume += "K";
		}
	}
	
	$("#volumn").html(volume + (tradePath == "vtrade" ? " BFC" : (tradePath == "usdr" ? " " + tradePath.toUpperCase() : " USDT")));
	$("#maxBuy").html(parseFloat(data.maxPrice));
	$(".maxBuyText").html(parseFloat(data.maxPrice));
	$("#minSell").html(parseFloat(data.minPrice));
	$(".minSellText").html(parseFloat(data.minPrice));
	$("#fundingFeeRate").html(parseFloat((data.fundingFeeRate * 100).toFixed(4)) + '%');
		
	if (data.lastPrice) {
		document.title = parseFloat(data.lastPrice) +  (tradePath == "vtrade" ? "BFC" : (tradePath == "usdr" ? tradePath.toUpperCase() : "USDT")) + "|" + transaction_code.toUpperCase() + MSG['systeam'];
	}
	
	var selltop5List = data.selltop5List;
	var buytop5List = data.buytop5List;
	
	var linebuy = "";
	var linesell = "";
	var sell1 = "";
	if(selltop5List.length<=0){
		sell1="0";
	}else{
		sell1 = formatnumber(selltop5List[selltop5List.length-1].price, jgws);
	}
	var buy1 = "";
	if(buytop5List.length<=0){
		buy1="0";
	}else{
		buy1=formatnumber(buytop5List[0].price, jgws);
	}
	
	$("#markPrice").html(data.markPrice);
	
	$("#buy1").val(buy1);
	$("#sell1").val(sell1);
	$("#sellOneNum").val(sell1);
}

function updateTradesHide(data) {
	if (!data) return;
	
	var type = $('#tradePath').val();
	var tradePath = $('#tradePath').val();
	if (tradePath == "vtrade") {
		type = "vcontract";
	} else if (tradePath == 'dtrade') {
		type = 'delivery';
	} else if (tradePath == 'trade') {
		type = "contract";
	}
	
	var bqCode = "";
	if (type == "spot") {
		bqCode = $('#coinPairName').val();
	} else {
		bqCode = $('#transaction_code').val();
	}
	
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
			change = Fractional(change, amountDigits);
			
			var changeamount;
			if (type != "spot") {
				if (change < 0)
					changeamount = change;
				else if (change == 0)
					changeamount = "0";
				else
					changeamount = "+" + change;
			} else {
				changeamount = change;
			}
			
			lineorder += "<tr><td>" + getLocalTime(date) + "</td>" +
					"<td class='" + tradeType + "'>" + price + "</td>" +
					"<td>" + amount + "</td>" +
					"<td>" + changeamount + "</td></tr>";
		}
		$("#ordertbody").html(lineorder);
	}
}
function updateTrades(data) {
	if (!data) return;
	
	var type = $('#tradePath').val();
	var tradePath = $('#tradePath').val();
	if (tradePath == "vtrade") {
		type = "vcontract";
	} else if (tradePath == 'dtrade') {
		type = 'delivery';
	} else if (tradePath == 'trade') {
		type = 'contract';
	}
	
	var bqCode = "";
	if (type == "spot") {
		bqCode = $('#coinPairName').val();
	} else {
		bqCode = $('#transaction_code').val();
	}
	
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
			change = Fractional(change, amountDigits);
			
			var changeamount;
			if (type != "spot") {
				if (change < 0)
					changeamount = change;
				else if (change == 0)
					changeamount = "0";
				else
					changeamount = "+" + change;
			} else {
				changeamount = change;
			}
			
			lineorder += "<tr><td>" + getLocalTime(date) + "</td>" +
					"<td class='" + tradeType + "'>" + price + "</td>" +
					"<td>" + amount + "</td>" +
					"<td>" + changeamount + "</td></tr>";
		}
		$("#ordertbodyonly").html(lineorder);
	}
}

function updateBPI(data) {
	if (!data) return;
	
	var transaction_type_code = $('#transaction_type_code').val();
	for (var obj in data) {
		var object = data[obj];
		if (object["typeCode"] == transaction_type_code) { 
			indexFuduUp = object["up"];
			break;
		}
		
//		$("#" + object["typeCode"] + "-bpi2").html("<span class='" + ( object["up"] ? "buy" : "sell") + "'>" + object["last"] + "</span>");
	}
}

function updateStockPrices(){
	var transaction_code = $('#transaction_code').val();
	var tradePath = $('#tradePath').val();
	var ajaxPath = 'tradeAjax';
	if (tradePath == 'vtrade') {
		ajaxPath = 'vtradeAjax';
	} else if (tradePath == 'dtrade') {
		ajaxPath = 'dtradeAjax';
	} else if (tradePath == 'trade') {
		ajaxPath = 'tradeAjax';
	} else {
		ajaxPath = tradePath + '/tradeAjax';
	}
	var coeffcient = $('#coeffcient').val();
	$.ajax({
        type: "POST",
        dataType: "json",         
        url: basePath + ajaxPath + "/getCommonData.do",
        data: {bqCode : transaction_code},
        error : function(msg) {
        	stockInterval = setTimeout(updateStockPrices, 2000);
		},
        success: function(data) {
        	updateCommonData(data);
        	stockInterval = setTimeout(updateStockPrices, 2000);
        }
	});
	
}

function tradesRefresh() {
	var type = $('#tradePath').val();
	var tradePath = $('#tradePath').val();
	if (tradePath == "vtrade") {
		type = "vcontract";
	} else if (tradePath == 'dtrade') {
		type = 'delivery';
	} else if (tradePath == 'trade') {
		type = 'contract';
	}
	
	var bqCode = "";
	if (type == "spot") {
		bqCode = $('#coinPairName').val();
	} else {
		bqCode = $('#transaction_code').val();
	}
	
	var url = basePath + "futuresApi/trades.do?type=" + type + "&symbol=" + bqCode + "&bfx=1&ra=" + Math.round(Math.random() * 100);
	$.ajax( {
		type : 'GET',
		url : url,
		data : {},
		contentType : "application/json; charset=utf-8",
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
				updateTradesHide(data);
			}
			
			marketEntrustTime = setTimeout(tradesRefresh, 5000);
		},
		error : function(msg) {
			
		}
	});
}

function getBPI() {
	$.ajax( {
		type : "POST",
		dataType : "text",
		url : basePath + "tradeAjax/getBPIData.do",
		data : {},
		success : function(data) {
			var data = eval('(' + data + ')');
			updateBPI(data);
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
		var code = $('#transaction_code').val();
		var type = $('#tradePath').val();
		var tradeType = $('#tradePath').val();
		if (tradeType == "vtrade") {
			type = "vcontract";
		} else if (tradeType == "spotTrade") {
			type = "spot";
		} else if (tradeType == "dtrade") {
			type = "delivery";
		} else if (tradeType == 'trade') {
			type = 'contract';
		}
		
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
//		console.log("Message received: " + e.data);
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
			var type = $('#tradePath').val();
			var tradeType = $('#tradePath').val();
			if (tradeType == "vtrade") {
				type = "vcontract";
			} else if (tradeType == "spotTrade") {
				type = "spot";
			} else if (tradeType == "dtrade") {
				type = "delivery";
			} else if (tradeType == 'trade') {
				type = 'contract';
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
			countNum  = sd[1];
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
	if($(".right-cont-top-item-alone").css("display") == "block") {
		$("#sellTbodyOnly").html(linesell);
		$("#buyTbodyOnly").html(linebuy);
	}
}

function handleData(data) {
	if (data.tradesdata) {
		updateTrades(data.tradesdata);
		updateTradesHide(data.tradesdata);
	}
	
	if (data.bpidata) {
		updateBPI(data.bpidata.data);
	}
	
	if (data.commondata) {
		updateCommonData(data.commondata.data);
	}

	if (data.depthdata) {
		depthData = data.depthdata.data;
	}
	
	if (data.tickersdata) {
		var tickers = data.tickersdata.data;

		if (locale != "zh_CN") {
    		for (var i = 0; i < tickers.length; i++) {
        		var item = tickers[i];
        		if (locale == "en_US") {
        			if (item.enName && item.enName != "") {
        				item.showName = item.enName;
        			}
        		} else if (locale == "ja_JP") {
        			if (item.jpName && item.jpName != "") {
        				item.showName = item.jpName;
        			}
        		} else if (locale == "ko_KR") {
        			if (item.krName && item.krName != "") {
        				item.showName = item.krName;
        			}
        		} else if (locale == "zh_HK") {
        			if (item.hkName && item.hkName != "") {
        				item.showName = item.hkName;
        			}
        		}
        	}
    	}
		
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


//var afds = setInterval(getBPI, 5000)
//var marketEntrustTime = setTimeout(tradesRefresh, 5000);
//var stockInterval = setTimeout(updateStockPrices, 2000);

setInterval(showTime, 1000)

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
	
	getTime();
	setTimeout("showTime()", 1000);
});
