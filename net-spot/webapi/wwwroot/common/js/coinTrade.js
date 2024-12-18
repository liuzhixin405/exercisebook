function tcoinSubmit(url, name, direction) {
	
	if ($('#userEmail').val() == null || $('#userEmail').val() == "") {
		tipAlert(MSG['opAfterLogin']);
		return false;
	}
	
	var price = 0;
	var amount = 0;
	if (direction == 1) {
		price = Number($('#buyPrice').val());
		amount = Number($('#buyAmount').val());
	} else {
		price = Number($('#sellPrice').val());
		amount = Number($('#sellAmount').val())
	}
	
	if (isNaN(price) || price <= 0) {
		tipAlert(MSG['alertLegalPrice']);
		return false;
	}
	
	if (isNaN(amount) || amount <= 0) {
		tipAlert(MSG['alertLegalNum']);
		return false;
	}
	
	var type = "buy";
	if (direction == 2) type = "sell";
	if (checkAmount(type, 1)) {
		tradeCoin(url,name,direction,price,amount);
	}
}

function checkAmount(type, showAlert) {
	var amount = Number($('#' + type + 'Amount').val());
	var price = Number($('#' + type + 'Price').val());
	
	if ($('#userEmail').val() == null || $('#userEmail').val() == "") {
		return false;
	} else {
		if (type == 'buy' && price > 0) {
			var maxAmount = Number($('#marketBalance').val()) / price;
			var digits = Number($('#amountDigits').val()) + 1;
			var maxStr = maxAmount + "";
			if (maxStr.indexOf('.') != -1 && maxStr.length - maxStr.indexOf('.') > digits) {
				maxStr = maxStr.substring(0, maxStr.indexOf('.') + digits);
			}
			
			maxAmount = Number(maxStr);
			$('#buyKinds1').html(maxAmount);
		}
		
		if (price == 0 || amount == 0) {
			$('#' + type + 'Money').html(0);
			return false;
		}
		
		if (amount < Number($('#minKcNums').val())) {
			if (typeof(showAlert) != "undefined" ) {
				tipAlert( MSG['alertLitOpenNum'] + "：" + Number($('#minKcNums').val()) + "个");
			}
			amount = $('#minKcNums').val();
			var money = parseFloat((amount * price).toFixed(8));
			$('#' + type + 'Amount').val(amount);
			$('#' + type + 'Money').html(money);
			return false;
		}
		
		if (type == 'buy') {
			var maxAmount = Number($('#marketBalance').val()) / price;
			var digits = Number($('#amountDigits').val()) + 1;
			var maxStr = maxAmount + "";
			if (maxStr.indexOf('.') != -1 && maxStr.length - maxStr.indexOf('.') > digits) {
				maxStr = maxStr.substring(0, maxStr.indexOf('.') + digits);
			}
			
			maxAmount = Number(maxStr);
			$('#buyKinds1').html(maxAmount);
			if (amount > maxAmount) {
				if (typeof(showAlert) != "undefined" ) {
					tipAlert(MSG['maxBuy'] + "：" + maxAmount + MSG['alertGe']);
				}
				amount = maxAmount;
				$('#' + type + 'Amount').val(amount);
				var money = parseFloat((amount * price).toFixed(8));
				$('#' + type + 'Money').html(money);
				return false;
			}
		} else {
			var maxAmount = Number($('#coinBalance').val());
			var digits = Number($('#amountDigits').val()) + 1;
			var maxStr = maxAmount + "";
			if (maxStr.indexOf('.') != -1 && maxStr.length - maxStr.indexOf('.') > digits) {
				maxStr = maxStr.substring(0, maxStr.indexOf('.') + digits);
			}
			
			maxAmount = Number(maxStr);
			if (amount > maxAmount) {
				if (typeof(showAlert) != "undefined" ) {
					tipAlert(MSG['maxSell'] + "：" + maxAmount + MSG['alertGe']);
				}
				amount = maxAmount;
				$('#' + type + 'Amount').val(amount);
				var money = parseFloat((amount * price).toFixed(8));
				$('#' + type + 'Money').html(money);
				return false;
			}
		}
		
		var money = parseFloat((amount * price).toFixed(8));
		$('#' + type + 'Money').html(money);
		return true;
	}
	
	
}

function tradeCoin(url, name, direction, price, amount) {
	$.ajax( {
		url : url,
		type : "POST",
		data : {direction: direction, coinPairName: name, amount: amount, price: price},
		dataType : "JSON",
		cache : false,
		statusCode : {
			404 : function() {
//				location.reload();
			}
		},
		success : function(data) {
			showSuccInfo_coin_kc(direction, data.msg, data.code);
			if (data.code == 'success') {
				$("#buyAmount").val("");
				$("#buyPrice").val("");
				$("#buyKinds1").html("0");
				$("#buyMoney").html("0");
				
				$("#sellAmount").val("");
				$("#sellPrice").val("");
				$("#buyKinds2").html("0");
				$("#sellMoney").html("0");
				refreshAccount();
			}
		},
		error : function(XMLHttpRequest, textStatus, errorThrown) {
			
		}
	});
}

function showSuccInfo_coin_kc(direction, text, code) {
	var direct = "buy";
	if (direction == 2) direct = "sell";
	if(text.length>0){
		showMsgBox(code, text);
		setTimeout('refreshCurrentTab()', 500);
		setTimeout('finishshowSuccInfo_coin_kc("' + direct + '","'+code+'")', 3000);
	} else {
		$("#infomsg_kc" + direct).css("display", "none");
	}
}

function finishshowSuccInfo_coin_kc(direct, tcode) {
//	$("#infomsg_kc" + direct).css("display", "none");
	$("#tab-trade .current").click();
	refreshAccount();
}

function refreshAccount() {
	if ($('#userEmail').val() == null || $('#userEmail').val() == "") {
		return;
	}
	
	var coinPairName = $('#coinPairName').val();
	$.ajax({
        type: "POST",
        dataType: "json",         
        url: basePath +"spotTrade/spotTradeData.do",
        data: {"coinPairName":coinPairName, token : getCookie("_token")},
        success: function(data) {
			$("#bestBuyPrice").html(data.maeketCoinAccount);
			$("#bestSellPrice").html(data.coinAccount);
			
			if ($("#buyPrice").val() != "" && $("#buyAmount").val() != "") {
				checkAmount('buy');
			}
			if ($("#sellPrice").val() != "" && $("#sellAmount").val() != "") {
				checkAmount('sell');
			}
		}
    });
}

function refreshTradeLog() {
	if ($('#userEmail').val() == null || $('#userEmail').val() == "") {
		return;
	}
	
	var coinPairName = $('#coinPairName').val();
	$.ajax({
        type: "POST",
        dataType: "json",         
        url: basePath +"spotTrade/tradeLogData.do",
        data: {"coinPairName":coinPairName, token : getCookie("_token")},
        success: function(data) {
			var tradeHtml = "";
			if (data.length == 0) {
				tradeHtml = "<tr><td colspan='12'>" + MSG['noTrade'] + "</td></tr>";
			} else {
				var coinName =  $('#coinName').val();
				var marketName =  $('#marketName').val();
				for (var index = 0; index < data.length; index++) {
					var tl = data[index];
					var tradeType = "";
					var unit = '(' + marketName + ')';
					if (tl.trade_trans_dirction == 1) {
						unit = '(' + coinName + ')';
						tradeType = "<b class='buy'>" + MSG['postionBuy'] + "</b>";
					} else {
						tradeType = "<b class='sell'>" + MSG['postionSell'] + "</b>";
					}
					
					var tradeFee = tl.trade_fee;
					if (tl.trade_bfx_fee > 0) {
						tradeFee = tl.trade_bfx_fee;
						unit = "(bfx)";
					}
					
					tradeHtml += "<tr>"
						+ "<td>" + dateFmt(tl.trade_time * 1000) + "</td>"
						+ "<td>" + tl.coin_pair_name.toUpperCase() + "</td>"
						+ "<td>" + tradeType + "</td>"
						+ "<td>" + parseFloat(tl.trade_average_price) + "</td>"
						+ "<td>" + parseFloat(tl.trade_count) + "</td>"
						+ "<td>" + parseFloat(tl.trade_amount) + "</td>"
						+ "<td>" + parseFloat(tradeFee.toFixed(4)) + unit + "</td>"
						+ "<td>" + MSG['finish'] + "</td>"
					+ "</tr>"
				}
			}
			$("#tradeTbody").html(tradeHtml);
        }
	});
}

function refreshPending() {
	if ($('#userEmail').val() == null || $('#userEmail').val() == "") {
		return;
	}
	
	var coinPairName = $('#coinPairName').val();
	$.ajax({
        type: "POST",
        dataType: "json",         
        url: basePath +"spotTrade/pendingData.do",
        data: {"coinPairName":coinPairName, token : getCookie("_token")},
        success: function(data) {
			var pdHtml = "";
			if (data.length == 0) {
				pdHtml = "<tr><td colspan='12'>" + MSG['noEntrustment'] + "</td></tr>";
			} else {
				for (var index = 0; index < data.length; index++) {
					var pd = data[index];
					var pdType = "";
					var cancel = "";
					if (pd.direction == 1) {
						pdType = "<b class='buy'>" + MSG['postionBuy'] + "</b>";
					} else {
						pdType = "<b class='sell'>" + MSG['postionSell'] + "</b>";
					}
					
					cancel = "<a href='javascript:void(0);' class='btn btn-blue' "
						+ " onclick=\"coincancelDealAnsyc(" + pd.orderId + ",'" + pd.symbol + "');\">" + MSG['cancel'] + "</a>"
				
					pdHtml += "<tr>"
						+ "<td>" + dateFmt(pd.createTime * 1000) + "</td>"
						+ "<td>" + pd.coinPairName.toUpperCase() + "</td>"
						+ "<td>" + pdType + "</td>"
						+ "<td>" + parseFloat(pd.price) + "</td>"
						+ "<td>" + parseFloat(pd.amount) + "</td>"
						+ "<td>" + parseFloat(pd.dealAmount) + "</td>"
						+ "<td>" + parseFloat(pd.undealAmount) + "</td>"
						+ "<td>" + cancel + "</td>"
						+ "</tr>";
				}
			}
			$("#pendingTbody").html(pdHtml);
        }
	});
}

function refreshHistoryPending () {
	if ($('#userEmail').val() == null || $('#userEmail').val() == "") {
		return;
	}
	
	var coinPairName = $('#coinPairName').val();
	$.ajax({
        type: "POST",
        dataType: "json",         
        url: basePath +"spotTrade/historyPendingData.do",
        data: {"coinPairName":coinPairName, token : getCookie("_token")},
        success: function(data) {
			var pdHtml = "";
			if (data.length == 0) {
				pdHtml = "<tr><td colspan='12'>" + MSG['noEntrustment'] + "</td></tr>";
			} else {
				var coinName =  $('#coinName').val();
				var marketName =  $('#marketName').val();
				for (var index = 0; index < data.length; index++) {
					var pd = data[index];
					var pdType = "";
					var avgPrice = "--";
					var state = MSG['submitted'];
					var unit = '(' + marketName + ')';
					if (pd.direction == 1) {
						unit = '(' + coinName + ')';
						pdType = "<b class='buy'>" + MSG['postionBuy'] + "</b>";
					} else {
						pdType = "<b class='sell'>" + MSG['postionSell'] + "</b>";
					}
					
					if (pd.action == 0) {
						if (pd.deal_count > 0) {
							state = MSG['partDeal'];
						}
					} else if (pd.action == 1) {
						state = MSG['allDeal'];
					} else if (pd.action == 4) {
						if (pd.deal_count == 0) {
							state = MSG['allreadyCancel'];
						} else {
							state = MSG['partDealCancel'];
						}
					}
					
					if (pd.avg_price > 0) {
						avgPrice = parseFloat(pd.avg_price);
					}
					
					var tradeFee = pd.fee;
					if (pd.bfx_fee > 0) {
						tradeFee = pd.bfx_fee;
						unit = "(bfx)";
					}
					
					pdHtml += "<tr>"
						+ "<td>" + dateFmt(pd.delegation_time * 1000) + "</td>"
						+ "<td>" + pd.coin_pair_name.toUpperCase() + "</td>"
						+ "<td>" + pdType + "</td>"
						+ "<td>" + parseFloat(pd.delegation_price) + "</td>"
						+ "<td>" + parseFloat(pd.delegation_count) + "</td>"
						+ "<td>" + parseFloat(pd.deal_count) + "</td>"
						+ "<td>" + parseFloat(pd.undeal_count) + "</td>"
						+ "<td>" + avgPrice + "</td>"
						+ "<td>" + parseFloat(tradeFee.toFixed(4)) + unit + "</td>"
						+ "<td>" + state + "</td>"
						+ "</tr>";
				}
			}
			$("#historyPendingTbody").html(pdHtml);
        }
	});
}

function formatToTwoDigits1(floatNum, amountDigits) {
	var maxStr = floatNum + "";
	if (maxStr.indexOf('.') != -1 && maxStr.length - maxStr.indexOf('.') > amountDigits) {
		maxStr = maxStr.substring(0, maxStr.indexOf('.') + amountDigits + 1);
	}
	return maxStr;
};

function recalculation() {
	var coinPairName = $('#coinPairName').val();
	$.ajax({
		type: "POST",
		dataType: "json",
		url: basePath +"spotTrade/spotTradeData.do",
		data: {"coinPairName":coinPairName, token : getCookie("_token")},
		success: function(data) {
			$("#bestBuyPrice").html(data.maeketCoinAccount);
			$("#bestSellPrice").html(data.coinAccount);
			if ($("#buyPrice").val() != "" && $(".percent-buy").hasClass("active")) {
				var percent = $(".percent-buy[class~=active]").html();
				var float = percent.replace("%","");
				float = float / 100;
				var bestBuyPrice = $('#bestBuyPrice').html();             //可用
				var buyPrice = Number($("#buyPrice").val());              //买入价格
				var amountDigits = Number($('#amountDigits').val());      //小数点2位
				if (buyPrice != "" && !isNaN(bestBuyPrice)) {
					var amount = float * bestBuyPrice / buyPrice;
					$("#buyAmount").val(formatToTwoDigits1(amount, amountDigits));
					recalculCheckAmount('buy', data.maeketCoinAccount, $("#buyAmount").val());
				} else {
					return false;
				}
			}
			if ($("#sellPrice").val() != "" && $(".sell-percent").hasClass("active")) {
				var percent = $(".sell-percent[class~=active]").html();
				var float = percent.replace("%","");
				float = float / 100;
				var bestSellPrice = $('#bestSellPrice').html();             //可用
				var sellPrice = Number($("#sellPrice").val());             //卖出价格
				var amountDigits = Number($('#amountDigits').val());      //小数点2位
				if (sellPrice != "") {
					var amount = float * bestSellPrice;
					$("#sellAmount").val(formatToTwoDigits1(amount, amountDigits));
					recalculCheckAmount('sell', data.coinAccount, $("#sellAmount").val());
				} else {
					return false;
				}
			}
		}
	});
}

function recalculCheckAmount(type, bestPrice, bestAmount) {
	var price = Number($('#' + type + 'Price').val());
	var amountDigits = Number($('#amountDigits').val());      //小数点2位
	if(type == 'buy') {
		$("#buyAmount").val(formatToTwoDigits1(bestAmount, amountDigits));
		var money = parseFloat((bestAmount * price).toFixed(8));
		$('#buyMoney').html(money);
	} else {
		$("#sellAmount").val(formatToTwoDigits1(bestAmount, amountDigits));
		var money = parseFloat((bestAmount * price).toFixed(8));
		$('#sellMoney').html(money);
	}
}

/**
 * 撤单
 * 
 * @return
 */
function coincancelDeal(direction, code) {
	$.get(basePath +"spotTradeAjax/cancleDeal.do?t=" + new Date().getTime()+"&token="+getCookie("_token"), {
		direction : direction,
		code : code
	}, function(data) {
		refreshPending();
		refreshAccount();
	});
}

function coincancelDealAnsyc(id, code) {
	$.get(basePath +"spotTradeAjax/canclePendingAnsyc.do?t=" + new Date().getTime()+"&token="+getCookie("_token"), {
		pdId : id,
		code : code
	}, function(data) {
		refreshPending();
		refreshAccount();
		//如果选择了盘口的数据，百分比选中状态，撤单后，将重新计算交易数量和交易金额
		if($(".percent-buy").hasClass("active") || $(".sell-percent").hasClass("active")) {
			recalculation();
		}
	});

}

function trade_price_market(type, price, amount) {
	var code = $('#coinPairName').val();
	$("#buyPrice").val(price);
	$("#sellPrice").val(price);

	if($(".percent-buy").hasClass("active")) {
		recalculation();
	} else {
		$("#buyAmount").val(amount);
		checkAmount('buy');
	}
	if($(".sell-percent").hasClass("active")) {
		recalculation();
	} else {
		$("#sellAmount").val(amount);
		checkAmount('sell');
	}
}

function changeTo(locale) {
	$.get(basePath + "home/setLocale.do?locale=" + locale, function(data) {
		window.location.reload(true);
	});
}

$(function() {
	var buyPrice = Number($("#buyPrice").val());              //买入价格 	
	var sellPrice = Number($("#sellPrice").val());             //卖出价格
	if (buyPrice != "") {
		canDoBuyPrice();
	}
	if (sellPrice != "") {
		canDoSellPrice();
	}

	$("#sellPrice").keyup(function() {
		var sellPrice = Number($("#sellPrice").val());
		if (sellPrice != "") {
			canDoSellPrice();
		} else {
			$("#canSellPrice").html("--");
		}
	});
	$("#buyPrice").keyup(function() {
		var buyPrice = Number($("#buyPrice").val());  
		if (buyPrice != "") {
			canDoBuyPrice();
		} else {
			$("#canBuyPrice").html("--");
		}
	});
	
	$(".spot-percent-buy span").on('click', function() {
		if(!$(this).hasClass("active")){
			$(".spot-percent-buy span").removeClass("active");
			$(this).addClass("active");
		}else {
			$(this).removeClass("active");
		}

		var percent = $(this).html();
		var float = percent.replace("%","");
		float = float / 100;
		var bestBuyPrice = $('#bestBuyPrice').html();             //可用
		var buyPrice = Number($("#buyPrice").val());              //买入价格 		
		var amountDigits = Number($('#amountDigits').val());      //小数点2位
		
		if (buyPrice != "" && !isNaN(bestBuyPrice)) {
			var amount = float * bestBuyPrice / buyPrice;
			$("#buyAmount").val(formatToTwoDigits(amount, amountDigits));
			checkAmount('buy');
		} else {
			return false;
		}
	});
	
	$(".spot-percent-sell span").on('click', function() {
		if(!$(this).hasClass("active")){
			$(".spot-percent-sell span").removeClass("active");
			$(this).addClass("active");
		}else {
			$(this).removeClass("active");
		}
		 
		var percent = $(this).html();
		var float = percent.replace("%","");
		float = float / 100;
		
		var bestSellPrice = $('#bestSellPrice').html();             //可用	
		var sellPrice = Number($("#sellPrice").val());             //卖出价格
		var amountDigits = Number($('#amountDigits').val());      //小数点2位
		if (sellPrice != "") {
			var amount = float * bestSellPrice;			
		    $("#sellAmount").val(formatToTwoDigits(amount, amountDigits));
		    checkAmount('sell');
		} else {
			return false;
		}

	});

	function formatToTwoDigits(floatNum, amountDigits) {
	    var maxStr = floatNum + "";
	    if (maxStr.indexOf('.') != -1 && maxStr.length - maxStr.indexOf('.') > amountDigits) {
	        maxStr = maxStr.substring(0, maxStr.indexOf('.') + amountDigits + 1);
	    }
	    return maxStr;
	};

	function canDoSellPrice() {
		var bestSellPrice = $('#bestSellPrice').html();             //可用	
		var sellPrice = Number($("#sellPrice").val());             //卖出价格
		var amountDigits = Number($('#amountDigits').val());      //小数点2位		
			var amount = bestSellPrice / sellPrice;
			$("#canSellPrice").html(formatToTwoDigits(amount, amountDigits));
			
	}
	
	function canDoBuyPrice() {
		var bestBuyPrice = $('#bestBuyPrice').html();             //可用
		var buyPrice = Number($("#buyPrice").val());              //买入价格 
		var amountDigits = Number($('#amountDigits').val());      //小数点2位	
			var amount = bestBuyPrice / buyPrice;
			$("#canBuyPrice").html(formatToTwoDigits(amount, amountDigits));

	}
	
});

$(".chose-canvas-kd span").click(function() {
	$(this).addClass("active").siblings().removeClass("active");
	if($(this).html() == fmtDepth) {
		$(".cav-cter-kdraw").css("display", "none");
		$(".cav-cter-depath").css("display", "block");
		$("#chart-depath").show();
	}
	
	if($(this).html() == fmtKLine) {
		$(".cav-cter-depath").css("display", "none");
		$("#chart-depath").hide();
		$(".cav-cter-kdraw").css("display", "block");
	}
});

var currTab = 0;
function changeTab(type) {
	currTab = type;
}

function refreshTab() {
	
	if ($('#userEmail').val() == null || $('#userEmail').val() == "") {
		return;
	}
	
	if (currTab == 0) {
		refreshPending();
	} else if (currTab == 1) {
		refreshHistoryPending();
	}  else if (currTab == 2) {
		refreshTradeLog();
	}
	
	refreshAccount();
}

setInterval(refreshTab, 15000);