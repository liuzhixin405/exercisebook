/**
 * 点击杠杆刷新页面
 * @param val
 * @param obj
 * @return
 */
function multipleClick(val, obj) {
	$(".multiple").removeClass("current");
	$(obj).addClass("current");
	$('#multiple').val(val);
	var amount = Number($('#amount').val());
	if (amount != 0) {
		$('#baozhengjin').html((amount / val).toFixed(4));
	}
}

/** *************** 以下是限时刷新 *************** */
var marketEntrustTime;
var jsecond = 0;

function updateMarketSecond(type) {
	clearTimeout(marketEntrustTime);
	changeSecond(type);
}

function changeSecond(type) {
	$('#secondNumber').html(jsecond + "s");
	if (jsecond == 0) {
		ordersRefresh(type);
		tradesRefresh(type);
		jsecond = $('#updateSecond').val() / 1000;
	}

	jsecond--;
	
	marketEntrustTime = setTimeout("changeSecond('" + type + "')", 1000);
}

function ordersRefresh(type) {
	var transType="baocang";
	var bqCode = "";
	if (type == "spot") {
		transType="normal";
		bqCode = $('#coinPairName').val();
	} else {
		bqCode = $('#transaction_code').val();
	}
	
	var url = basePath + "futuresApi/depth.do?type=" + type + "&symbol=" + bqCode + "&transType=" + transType + "&bfx=1&r=" + Math.round(Math.random() * 100);

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
			var result = null;
			if (data.code == 0) {
				result = data.data;
			}
			
			if (result != null) {
				var bids = result.bids; // 买多
				var asks = result.asks; // 卖空
				var digits = parseInt(data.digits);
				var amountDigits = parseInt(data.amountDigits);
				
				if (bids != null) {
					var linebuy = "";
					var total = 0;
					for ( var i = 0; i < bids.length; i++) {
						var price = parseFloat(bids[i][0]);
						var amount = parseFloat(bids[i][1]);
						var type = 0;
						if (type != "spot") {
							type = parseFloat(bids[i][2]);
						}
						total = total + amount;
						var s = "";
						price = Fractional(price, digits);
						amount = Fractional(amount, amountDigits);
						s = Fractional(total, amountDigits);
						
						linebuy = linebuy + "<tr style='cursor: pointer; background-color: rgb(255, 255, 255);' onmouseout=\"this.style.backgroundColor='#fff';\"" +
						" onmouseover=\"this.style.cursor='pointer';this.style.backgroundColor='#FFFFAA';\">" +
						"<td class='fc_02'>" + (i + 1) + "</td>"+
						"<td><span class='buy'>" + price + "</span></td>";
						if (type == 3) {
							linebuy = linebuy + "<td><span style='color:#ff7f01;'>" + amount + "</span></td>";
						} else 
							linebuy = linebuy + "<td><span>" + amount + "</span></td>";
						
						linebuy = linebuy + "<td><span>" + s + "</span>" + "</td></tr>";
					}
					$("#buytbody").html(linebuy);
				}
				
				if (asks != null) {
					var linesell = "";
					var total = 0;
					for ( var i = 0; i < asks.length; i++) {
						var price = parseFloat(asks[i][0]);
						var amount = parseFloat(asks[i][1]);
						var type = 0;
						if (type != "spot") {
							type = parseFloat(asks[i][2]);
						}
						
						total = total + amount;
						
						var s = "";
						price = Fractional(price, digits);
						amount = Fractional(amount, amountDigits);
						s = Fractional(total, amountDigits);
							
						var style = type == 3 ? "color:#ff7f01;" : "";
						
						linesell += ("<tr style=\"cursor: pointer; background-color: rgb(255, 255, 255);\" onmouseout=\"this.style.backgroundColor='#fff';\" onmouseover=\"this.style.cursor='pointer';this.style.backgroundColor='#FFFFAA';\">" +
						"<td class=\"fc_02\">" + (i + 1) + "</td>"+
						"<td><span class=\"sell\">" + price + "</span></td>"+
						"<td><span style=\"" + style + "\">" + amount + "</span></td>"+
						"<td><span>" + s + "</span>" + "</td></tr>");
						
					}
					$("#selltbody").html(linesell);
				}
			}

		},
		error : function(msg) {
		}
	});
}

function tradesRefresh(type) {
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
			var trades = null;
			if (data.code == 0) {
				trades = data.data;
			}
			
			var digits = parseInt(data.digits);
			var amountDigits = parseInt(data.amountDigits);
			
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
		},
		error : function(msg) {
		}
	});
}

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
