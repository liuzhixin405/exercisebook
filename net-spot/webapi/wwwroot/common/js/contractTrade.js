var map ;
var mapUp;
var mapDown;

var tradePath = $('#tradePath').val();
var ajaxPath = 'trade';
var tradeAjaxPath = 'tradeAjax';
if (tradePath == 'vtrade') {
	ajaxPath = 'vtrade';
	tradeAjaxPath = 'vtradeAjax';
} else if (tradePath == 'dtrade') {
	ajaxPath = 'dtrade';
	tradeAjaxPath = 'dtradeAjax';
} else if (tradePath == 'trade') {
	ajaxPath = 'trade';
	tradeAjaxPath = 'tradeAjax';
} else {
	tradeAjaxPath = 'ntradeAjax/' + tradePath;
	ajaxPath = 'ntrade/' + tradePath;
}

$(function() {
	$(".trade-percent-btn span").on('click', function() {
		 if(!$(this).hasClass("active")){
			 $(".trade-percent-btn span").removeClass("active");
			 $(this).addClass("active");
		 }else {
			 $(this).removeClass("active");
		 }

		var percent = $(this).html();
		var float = percent.replace("%","");
		float = float / 100;
		
		var canOpen = parseFloat($("#maxAmount").html());
		if (!isNaN(canOpen)) {
			var amountDigits = Number($('#amountDigits').val());
			var amount = float * canOpen;
			var maxStr = amount + "";
			if (maxStr.indexOf('.') != -1 && maxStr.length - maxStr.indexOf('.') > amountDigits) {
				maxStr = maxStr.substring(0, maxStr.indexOf('.') + amountDigits + 1);
			}
			$("#amount").val(maxStr);
		}
		
		trade_amount(true);
	});
	/*slider multiple*/
	$('.single-slider').jRange({
		from: 1,
		to: 100,
		step: 1,
		scale: [1,20,40,60,80,100],
		format: '%s',
		width: 345,
		showLabels: true,
		showScale: true
	});
	var transaction_code = $('#transaction_code').val(); // 合约号
	var getItemlocalStorage = window.localStorage.getItem("ntrade/usdr_" + transaction_code + "_multiple");
	if(getItemlocalStorage) {
		$(".order-input-inner").val('x' + getItemlocalStorage);
		multipleClick(getItemlocalStorage);
		multipleMarketClick(getItemlocalStorage);
		multiplePlanClick(getItemlocalStorage);
	}
	transIntroduce();
	refreshMultiple();
	refreshPending();
	refreshMemberTrans();
	refreshPlanEntrust();//计划委托
});

var message = {};
message["zh_CN"] = {

};
var showsliderBoxMemberTransBuy = true;
var showsliderBoxMemberTransSell = true;
var showsliderBoxPending = true;
var showsliderBoxPlanEntrust = true;

function trade_buy_price_bp() {
	// 定义变量数据类型（可用余额 - 可用余额 * 手续费率）
	var buy_jieguo = Number(ky_bzj) / price;

	if (amount > buy_jieguo) {
		var vindex = buy_jieguo.toString().indexOf('.');
		var amountvalue = 0;
		if (vindex < 0) {
			amountvalue = buy_jieguo.toString();
		} else if (vindex == 0) {
			amountvalue = "0" + buy_jieguo.toString().substring(0, 4);
		} else {
			amountvalue = buy_jieguo.toString().substring(0, buy_jieguo.toString().indexOf('.') + 4);
		}

		amount = buy_jieguo;
		$('#amount').val(amountvalue.toFixed(2));
	}

	if (!isNaN(amount)) {
		$('#buy_need_bzj').html((amount * shouxu).toFixed(3));
		$('#trademoney').val((amount * price).toFixed(6));
	} else {
		$('#buy_need_bzj').html('0.0000');
		$('#trademoney').val('0.000000');
	}
	if (amount == 0) {
		$('#buy_need_bzj').html('0.0000');
	}
	return false;
}

function showAlertMoney(price, amount, direct, url, type){
	if (type == 0) {
		$('.alert-money').css("display", "none");
	} else {
		$('#text_alert').html(direct == 1 ? MSG['multipleBuy'] : MSG['nullMinSell']);
		$('#maxBuyPrice1_alert').html(price);
		$('#maxBuyPrice2_alert').html(price);
		$('#buyAmount_alert').html(amount);
		
		if (direct == 1) {
			$('#direct_alert').addClass("buy");
			$('#direct_alert').removeClass("sell");
		} else {
			$('#direct_alert').removeClass("buy");
			$('#direct_alert').addClass("sell");
		}
		$('#direct_alert').html(direct == 1 ? MSG['postionBuy'] : MSG['postionSell']);
		
		$('#submit_alert').removeAttr("onclick");
		$('#submit_alert').attr("onclick", "tradetj('" + url + "');");
		
		$('#close_alert').removeAttr("onclick");
		$('#close_alert').attr("onclick", "showAlertMoney(0,0,0,0,0,0);");
		
		$('.alert-money').css("display", "block");
	}
}

/**
 * 买多操作
 * 
 * @param sact
 *            URL
 * @param cointype
 *            类型（现在只有BTC）
 * @param direction
 *            交易方向（1、买；2、卖）
 * @param flag
 *            委托类型（1、限价；2、市价）
 * @return
 */
function del_sure_trade(sact, cointype, direction, flag) {
	var multiple = Number($('#multiple').val());
	var amount = Number($('#amount').val());
	var price = Number($('#price').val());	// 报价
	var maxPrice = Number($('#maxBuy').html());
	var minPrice = Number($('#minSell').html());
	var btc_index_price = Number($('#btc_index_price').val());// 当前价格指数
	var transaction_code = $('#transaction_code').val(); // 合约号
	var remainAmount = Number($('#myRemainBtc').val());// 本金余额 我的可用余额
	var amountDigits = Number($('#amountDigits').val());
	var coeffcient = Number($('#coeffcient').val());
	
	var bol_tijiao = true;
	
	var marketMultiple = Number($('#market_multiple').val());
	var marketAmount = Number($('#marketAmount').val());
	var marketPrice = Number($('#lastPriceSpan').html());
	var btc_index_price_val = Number($('#btc_index_price_val').val());// 当前价格指数
	var market_transaction_code = $('#market_transaction_code').val(); // 合约号
	var marketRemainAmount = Number($('#market_myRemainBtc').val());// 本金余额 我的可用余额
	var marketAmountDigits = Number($('#market_amountDigits').val());
	var marketCoeffcient = Number($('#market_coeffcient').val());
	
	var percent = $("#percent").val();//选择的百分比
	
	
	if(flag == 1) {
		if ($('#userEmail').val() == null || $('#userEmail').val() == "") {
			tipAlert(MSG['opAfterLogin']);
			return false;
		}
		
		if (multiple == 0) {
			tipAlert(MSG['alertLeverage']);
			return false;
		}
		
		if (isNaN(amount) || amount <= 0) {
			tipAlert(MSG['alertLegalNum']);
			return false;
		}
		
		if (amount < Number($('#minKcNums').val())) {
			tipAlert(MSG['alertLitOpenNum'] + "：" + Number($('#minKcNums').val()) + MSG['alertGe']);
			return false;
		}
		
		
		if (isNaN(price) || price <= 0) {
			tipAlert(MSG['alertLegalPrice']);
			return false;
		}
		
		
		if (direction == 1 && price > maxPrice) {
			tipAlert(MSG['alertMaxBuyPrice']+ "：" + maxPrice);
			return false;
		}
		
		if (direction == 2 && price < minPrice) {
			tipAlert(MSG['alertMinSellPrice']+ "："+ minPrice);
			return false;
		}
		if (cointype == "szi") {
			var bpiother = Number($("#bpiother").val());
			var latest_transaction_price = Number($("#lastPriceOneMinAgo").val()); // 一分钟前成交价
			
			if (amount * price > formatRound(remainAmount * multiple * 10000,10)) {
				tipAlert(MSG['curtnotEnoughMoney']);
				return false;
			}
			
			// 判断是否可以交易
			var nomalTrade = $("#nomalTrade").val();
			if (nomalTrade != 1) {
				tipAlert(MSG['curtStop']);
				return;
			}
			
			var avagpriceysetady = Number($('#avagpriceysetady').val());// 昨日均价
			
			var maxavagpriceysetady = 0;
			var minavagpriceysetady = 0;
			
			if (latest_transaction_price > 0) {
				maxavagpriceysetady = bpiother*1.05; 
				minavagpriceysetady = bpiother*0.95;
				
				var lastPrice = formatFloat(latest_transaction_price * 1.01,2); // 最高可写入价为60秒前最新成交价+1%点
				var price1 = formatFloat(maxavagpriceysetady > lastPrice ? lastPrice : maxavagpriceysetady, 1);
				
				if (price > price1) {
					$('#price').val(price1.toFixed(1));
					showAlertMoney(price1, amount, 1, sact, 1);
					bol_tijiao = false;
				}
			} else {
				maxavagpriceysetady = formatFloat(bpiother*1.1,1); 
				if (maxavagpriceysetady < price){
					price = pricejsbos(maxavagpriceysetady,-1);
					$('#price').val(price.toFixed(1));
					showAlertMoney(price, amount, 1, sact, 1);
					bol_tijiao = false;
				}
			}
		} else {
			var feeRate = Number($('#myCommissionRate').val());						// 佣金（手续费）
			var maxAmount = remainAmount / (price / multiple + price * feeRate) / coeffcient;	// 最大可开仓数量
			var maxStr = maxAmount + "";
			if (maxStr.indexOf('.') != -1 && maxStr.length - maxStr.indexOf('.') > amountDigits) {
				maxStr = maxStr.substring(0, maxStr.indexOf('.') + amountDigits + 1);
			}
			maxAmount = Number(maxStr);
			$('#maxAmount').html(maxAmount);
			if (amount > maxAmount) {
				tipAlert(MSG['curtnotEnoughMoney'] + maxAmount + MSG['alertGe']);
				return false;
			}
			
			$('#baozhengjin').html((price * ($('#amount').val() / multiple) * coeffcient).toFixed(4));
			bol_tijiao = true;
			
		}			
		if (bol_tijiao) {
			$('#direction').val(direction);
			tradetj(sact, ''); // 提交给后台
		}
	} else if (flag == 2) {
		if ($('#marketUserEmail').val() == null || $('#marketUserEmail').val() == "") {
			tipAlert(MSG['opAfterLogin']);
			return false;
		}
		
		if (marketMultiple == 0) {
			tipAlert(MSG['alertLeverage']);
			return false;
		}
		
		if ((isNaN(marketAmount) || marketAmount <= 0) && (percent == null || percent == "")) {
			tipAlert(MSG['alertNumAndPercent']);
			return false;
		}
		
		if (marketAmount < Number($('#market_minKcNums').val()) && (percent == null || percent == "")) {
			tipAlert(MSG['alertLitOpenNum'] + "：" + Number($('#market_minKcNums').val()) + MSG['alertGe']);
			return false;
		}
		
		if (cointype == "szi") {
			var bpiother = Number($("#bpiother").val());
			var latest_transaction_price = Number($("#lastPriceOneMinAgo").val()); // 一分钟前成交价
			
			if (amount * price > formatRound(remainAmount * multiple * 10000,10)) {
				tipAlert(MSG['curtnotEnoughMoney']);
				return false;
			}
			
			// 判断是否可以交易
			var nomalTrade = $("#nomalTrade").val();
			if (nomalTrade != 1) {
				tipAlert(MSG['curtStop']);
				return;
			}
			
			var avagpriceysetady = Number($('#avagpriceysetady').val());// 昨日均价
			
			var maxavagpriceysetady = 0;
			var minavagpriceysetady = 0;
			
			if (latest_transaction_price > 0) {
				maxavagpriceysetady = bpiother*1.05; 
				minavagpriceysetady = bpiother*0.95;
				
				var lastPrice = formatFloat(latest_transaction_price * 1.01,2); // 最高可写入价为60秒前最新成交价+1%点
				var price1 = formatFloat(maxavagpriceysetady > lastPrice ? lastPrice : maxavagpriceysetady, 1);
				
				if (marketPrice > price1) {
					$('#marketPrice').val(price1.toFixed(1));
					showAlertMoney(price1, marketAmount, 1, sact, 1);
					bol_tijiao = false;
				}
			} else {
				maxavagpriceysetady = formatFloat(bpiother*1.1,1); 
				if (maxavagpriceysetady < marketPrice){
					marketPrice = pricejsbos(maxavagpriceysetady,-1);
					$('#marketPrice').val(marketPrice.toFixed(1));
					showAlertMoney(marketPrice, marketAmount, 1, sact, 1);
					bol_tijiao = false;
				}
			}
		} else {
			var feeRate = Number($('#market_myCommissionRate').val());						// 佣金（手续费）
			var maxAmount = marketRemainAmount / (marketPrice / marketMultiple + marketPrice * feeRate) / marketCoeffcient;	// 最大可开仓数量
			var maxStr = maxAmount + "";
			if (maxStr.indexOf('.') != -1 && maxStr.length - maxStr.indexOf('.') > marketAmountDigits) {
				maxStr = maxStr.substring(0, maxStr.indexOf('.') + marketAmountDigits + 1);
			}
			maxAmount = Number(maxStr);
			$('#maxMarketAmount').html(maxAmount);
			if (marketAmount > maxAmount) {
				tipAlert(MSG['curtnotEnoughMoney'] + maxAmount + MSG['alertGe']);
				return false;
			}
			
			$('#baozhengjinMarket').html((marketPrice * ($('#marketAmount').val() / marketMultiple) * marketCoeffcient).toFixed(4));
			bol_tijiao = true;
		}
		if (bol_tijiao) {
			$('#market_direction').val(direction);
			tradetj(sact, 'Market'); // 提交给后台
		}
	}
}

/**
 * 开仓操作
 * 
 * @param sact 提交地址
 * @return
 */
function tradetj(sact, orderType) {
	var formId = '#top' + orderType + 'SearchForm';
	$('.alert-money').css("display", "none");
	var direction = $("#direction").val();
	if (direction == 1) {
		$("#btnBtc").html(MSG['submiting']);
	} else {
		$("#sellBtc").html(MSG['submiting']);
	}
	$.ajax( {
		url : sact,
		type : "POST",
		data : $(formId).serialize(),
		dataType : "JSON",
		cache : false,
		statusCode : {
			404 : function() {
				location.reload();
			}
		},
		success : function(data) {
			if (direction == 1) {
				$("#btnBtc").html(MSG['buylong']);
				$("#marketBtnBtc").html(MSG['buylong']);
			} else {
				$("#sellBtc").html(MSG['shortsell']);
				$("#marketSellBtc").html(MSG['shortsell']);
			}
			showMsgBox(data.code, data.msg);
			showSuccInfo_kc(data.msg, data.code);
			if (data.code == 'success') {
				$("#amount").val("");
				$("#price").val("");
				$("#payPd").val("");
				$("#baozhengjin").html("0");
				$('#maxAmount').html("0");
				// $("#marketAmount").val("");
				$("#marketPrice").val("");
				$("#marketPayPd").val("");
				$("#baozhengjinMarket").html("0");
				$('#maxMarketAmount').html("0");
				$("#percent").val('');
				$(".trade-market-percent span").removeClass("active");
				$(".trade-percent-btn span").removeClass("active");
				if (IsWhite) {
					$("#marketAmount").attr({'value': ''}).css('background-color', '#ffffff');
				} else {
					$("#marketAmount").attr({'value': ''}).css('background-color', '#0d1923');
				}
				clearInterval(marketInterval);
				marketInterval = null;
				refreshAccount();
				refreshMultiple();
				refreshPending();
				interimOrder();
				setTimeout("removeInterimOrder()", 3000);
				isNullPrice = true;
			}
		},
		error : function(XMLHttpRequest, textStatus, errorThrown) {
			// msgAlert("系统繁忙，请稍候再试！", "系统提示", false, callbackreload, "");
			if (direction == 1) {
				$("#btnBtc").html(MSG['buylong']);
			} else {
				$("#sellBtc").html(MSG['shortsell']);
			}
		}
	});
}

function refreshAccount() {
	if ($('#userEmail').val() == null || $('#userEmail').val() == "") {
		return;
	}
	
	var transaction_code = $('#transaction_code').val();
	
	$.ajax({
        type: "POST",
        dataType: "json",         
        url: basePath + ajaxPath + "/tradedata.do",
        data: {"transactionCode":transaction_code, token : getCookie("_token")},
        success: function(data) {
        	$("#myRemainBtc").val(data.userAccountBtc);
        	$("#totalamount").html(data.userAccountBtc);
        	$("#settleamount").html(data.userAccountBtc);
        	$("#planTotalamount").html(data.userAccountBtc);
        	$("#marketTotalamount").html(data.userAccountBtc);
        	$("#market_myRemainBtc").val(data.userAccountBtc);

        	if($("#full-market-order").css('display') == 'block') {
				var market_myRemainBtc = $("#market_myRemainBtc").val();
				refreshMaxAmount(market_myRemainBtc);
			}
        }
    });
}

function refreshMultiple() {
	/*if ($('#userEmail').val() == null || $('#userEmail').val() == "") {
		return;
	}*/
	
	var transaction_code = $('#transaction_code').val();
	$.ajax({
        type: "POST",
        dataType: "json",         
        url: basePath + ajaxPath + "/getMultiple.do",
        data: {"transactionCode":transaction_code, token : getCookie("_token")},
        success: function(data) {
        	var multiples = data.multiples;
        	multiples = JSON.parse(multiples);
        	var multipleHtml = "";
        	var planMultipleHtml = "";
        	var marketMultipleHtml = "";
			var closePlanMultipleHtml = "";
        	var defaultMultiple = 20;
        	var j = 0;
        	
        	var sessionMultiple = window.localStorage.getItem(ajaxPath + "_" + transaction_code + "_multiple");
    		if (sessionMultiple) {
    			defaultMultiple = sessionMultiple;
    		}
    		
    		var have = false;
    		for (var i = 0; i < multiples.length; i++) {
        		var obj = multiples[i];
        		var multiple = obj.multiple;
        		var state = obj.state;
        		if (state != 1) {
        			continue;
        		}
        		
        		if (defaultMultiple == multiple) {
        			have = true;
        		}
        	}
    		
    		if (!have) {
    			for (var i = 0; i < multiples.length; i++) {
            		var obj = multiples[i];
            		var multiple = obj.multiple;
            		var state = obj.state;
            		if (state != 1) {
            			continue;
            		}
            		
            		defaultMultiple = multiple;
            		break;
            	}
    		}
    		
    		for (var i = 0; i < multiples.length; i++) {
    			var obj = multiples[i];
    			var multiple = obj.multiple;
    			var state = obj.state;
    			if (state != 1) {
    				continue;
    			}
    			if (defaultMultiple == multiple) {
    				multipleHtml += `<span class="option-multiple active" id="multiple${multiple}" onclick="multipleClick(${multiple},this);">x${multiple}</span>`;
					planMultipleHtml += `<span class="option-multiple active" id="planMultiple${multiple}" onclick="multiplePlanClick(${multiple},this);">x${multiple}</span>`;
					marketMultipleHtml += `<span class="option-multiple active" id="market_multiple${multiple}" onclick="multipleMarketClick(${multiple},this);">x${multiple}</span>`;
					closePlanMultipleHtml += `<span class="option-multiple-close active" id="sellCloseMultiple-${multiple}" onclick="closeMultipleClick(${multiple},this);">x${multiple}</span>`;
    			} else {
    				multipleHtml += `<span class="option-multiple" id="multiple${multiple}" onclick="multipleClick(${multiple},this);">x${multiple}</span>`;
					planMultipleHtml += `<span class="option-multiple" id="planMultiple${multiple}" onclick="multiplePlanClick(${multiple},this);">x${multiple}</span>`;
					marketMultipleHtml += `<span class="option-multiple" id="market_multiple${multiple}" onclick="multipleMarketClick(${multiple},this);">x${multiple}</span>`;
					closePlanMultipleHtml += `<span class="option-multiple-close" id="sellCloseMultiple-${multiple}" onclick="closeMultipleClick(${multiple},this);">x${multiple}</span>`;
    			}
    		}
        	
        	planMultipleHtml += `<input type="hidden" name="planMultiple" id="planMultiple" value="${defaultMultiple}">`;
        	marketMultipleHtml += `<input type="hidden" name="multiple" id="market_multiple" value="${defaultMultiple}">`;
			closePlanMultipleHtml += `<input type="hidden" name="multiple" id="closePlanMultiple" value="${defaultMultiple}">`;
        	$("#selectOrderBox").html(multipleHtml);
        	$("#selectPlanOrderBox").html(planMultipleHtml);
        	$("#selectMarketOrderBox").html(marketMultipleHtml);
			$("#selectCloseOrderBox").html(closePlanMultipleHtml);

        	if ($('#userEmail').val() != null && $('#userEmail').val() != "") {
        		refreshMultipleClick(defaultMultiple);
        	}

			$(".order-input-inner").val('x' + defaultMultiple);
        	$("#multiple" + defaultMultiple).click();
			$('#multiple').val(defaultMultiple);
			$('#planMultiple').val(defaultMultiple);
			$('#market_multiple').val(defaultMultiple);
			$("#closePlanMultiple").val(defaultMultiple);
        	setMultiple(defaultMultiple);
        	getSellOneUsable();
        }
    });
}

function setMultiple(multiple) {
	var transaction_code = $('#transaction_code').val();
	window.localStorage.setItem(ajaxPath + "_" + transaction_code + "_multiple", multiple);
	$('.single-slider').jRange('setValue', multiple);
}

function refreshTradeLog() {
	if ($('#userEmail').val() == null || $('#userEmail').val() == "") {
		return;
	}
	
	var transaction_code = $('#transaction_code').val();
	var bigCode = transaction_code.toUpperCase();
	$.ajax({
        type: "POST",
        dataType: "json",         
        url: basePath + ajaxPath + "/tradeLogData.do",
        data: {"transactionCode":transaction_code, token : getCookie("_token")},
        success: function(data) {
			var tradeHtml = "";
			if (data.length == 0) {
				tradeHtml = "<tr><td colspan='12'>" + MSG['noTrade'] + "</td></tr>";
			} else {
				for (var index = 0; index < data.length; index++) {
					var tl = data[index];
					var tradeType = "";
					if (tl.trade_type_id == 1) {
						tradeType = "<b class='buy'>" + MSG['olp'] + "</b>";
					} else if (tl.trade_type_id == 2) {
						tradeType = "<b class='buy'>" + MSG['clp'] + "</b>";
					} else if (tl.trade_type_id == 3) {
						tradeType = "<b class='sell'>" + MSG['osp'] + "</b>";
					} else if (tl.trade_type_id == 4) {
						tradeType = "<b class='sell'>" + MSG['csp'] + "</b>";
					} else {
						var textTrade = MSG['textSell'];
						var textType = MSG['bsp'];
						if (tl.trade_type_id == 5) {
							textTrade = MSG['textBuy'];
							textType = MSG['blp'];
						}
						
						var textTop = "";
						if (index == 4) {
							textTop = "top:-43px;";
						}
						
						var forcePrice = "";
						if (tl.force_price > 0) {
							forcePrice = "<span style='color:red;'>" + tl.force_price + "</span>USDT";
						} 
						tradeType = "<b class='loss-all'>" + textType + "</b>"
							+ "<div class='addTips' onmouseout='hideOverloadTip(" + index + ")' onmouseover='showOverloadTip(" + index + ")'>"
							+ "<div class='addTips-arrow' id='baocangTips_arrow_" + index + "'></div>"
							+ "<div class='baocangTips' id='baocangTips_" + index + "' style='" + textTop + "'>" +
							MSG['textYour'] + bigCode +
							MSG['textContract'] + textTrade + tl.trade_multiple +
							MSG['textNote1'] + forcePrice +
							MSG['textNote12'] + "<span style='color:red;'>" + tl.trade_average_price + "</span>" +
							MSG['textNote2']
							+ "</div></div>";
					}
					
					var holdPrice = "";
					if (tl.trade_type_id != 1 && tl.trade_type_id != 3) {
						holdPrice = parseFloat(tl.hold_price);
					}
					
					var tradeIncome = "";
					if (tl.trade_type_id != 1 && tl.trade_type_id != 3) {
						if (tl.trade_type_id == 5 || tl.trade_type_id == 6) {
							tradeIncome = parseFloat(tl.trade_income - tl.trade_fee).toFixed(4);
						} else {
							tradeIncome = parseFloat(tl.trade_income).toFixed(4);
						}
						
						tradeIncome = parseFloat(tradeIncome);
					}
					
					var tradeAveragePrice = "--";
					var tradeDeposit = "--";
					var tradeFee = "--";
					if (tl.trade_type_id != 5 && tl.trade_type_id != 6) {
						tradeAveragePrice = parseFloat(tl.trade_average_price);
						tradeDeposit = parseFloat(tl.trade_deposit).toFixed(4);
						var tFee = tl.trade_bfx_fee > 0 ? tl.trade_bfx_fee : tl.trade_fee;
						tradeFee = parseFloat(tFee).toFixed(4);
						
						tradeDeposit = parseFloat(tradeDeposit);
						tradeFee = parseFloat(tradeFee);
						if (tl.trade_bfx_fee > 0) {
							tradeFee = tradeFee + "(bfx)";
						} else if (tradePath == "trade") {
							tradeFee = tradeFee + "(usdt)";
						} else if (tradePath != "vtrade") {
							tradeFee = tradeFee + "(" + tradePath + ")";
						}
					}
					
					tradeHtml += "<tr>"
						+ "<td>" + dateFmt(tl.trade_time * 1000) + "</td>"
						+ "<td>" + tl.transaction_code.toUpperCase() + "</td>"
						+ "<td>" + tradeType + "</td>"
						+ "<td>" + holdPrice + "</td>"
						+ "<td>" + tradeAveragePrice + "</td>"
						+ "<td>" + parseFloat(tl.trade_count) + "</td>"
						+ "<td>" + tradeDeposit + "</td>"
						+ "<td " + (tl.trade_income < 0 ? "class='sell'" : "class='buy'") + ">" + tradeIncome + "</td>"
						+ "<td>" + tradeFee + "</td>"
						+ "<td>" + MSG['finish'] + "</td>"
					+ "</tr>"
				}
			}
			$("#tradeTbody").html(tradeHtml);
        }
	});
}

function hideOverloadTip(index) {
	$("#baocangTips_" + index).hide();
	$("#baocangTips_arrow_" + index).hide();
}

function showOverloadTip(index) {
	$("#baocangTips_" + index).show();
	$("#baocangTips_arrow_" + index).show();
}

function refreshHistoryPending() {
	if ($('#userEmail').val() == null || $('#userEmail').val() == "") {
		return;
	}
	
	var transaction_code = $('#transaction_code').val();
	$.ajax({
        type: "POST",
        dataType: "json",         
        url: basePath + ajaxPath + "/historyPendingData.do",
        data: {"transactionCode":transaction_code, token : getCookie("_token")},
        success: function(data) {
			var pdHtml = "";
			if (data.length == 0) {
				pdHtml = "<tr><td colspan='12'>" + MSG['noEntrustment'] + "</td></tr>";
			} else {
				for (var index = 0; index < data.length; index++) {
					var pd = data[index];
					var pdType = "";
					var state = MSG['submitted'];
					var avgPrice = "--";
					if (pd.trade_type_id == 1) {
						pdType = "<b class='buy'>" + MSG['olp'] + "</b>";
					} else if (pd.trade_type_id == 2) {
						pdType = "<b class='sell'>" + MSG['clp'] + "</b>";
					} else if (pd.trade_type_id == 5) {
						pdType = "<b class='loss-all'>" + MSG['bsp'] + "</b>";
					} else if (pd.trade_type_id == 3) {
						pdType = "<b class='sell'>" + MSG['osp'] + "</b>";
					} else if (pd.trade_type_id == 4) {
						pdType = "<b class='buy'>" + MSG['csp'] + "</b>";
					} else if (pd.trade_type_id == 6) {
						pdType = "<b class='loss-all'>" + MSG['blp'] + "</b>";
					}
					
					if (pd.action == 0) {
						if (pd.deal_count > 0) {
							state = MSG['partDeal'];
						}
					} else if (pd.action == 1) {
						state = MSG['allDeal'];
					} else if (pd.action == 3) {
						state = MSG['burned'];
					} else if (pd.action == 4) {
						if (pd.deal_count == 0) {
							state = MSG['allreadyCancel'];
						} else {
							state = MSG['partDealCancel'];
						}
					}
					
					var income = "--";
					var tradeFee = "--";
					if (pd.avg_price > 0) {
						avgPrice = parseFloat(pd.avg_price);
						
						var tFee = pd.bfx_fee > 0 ? pd.bfx_fee : pd.fee;
						var tradeFee = parseFloat(tFee).toFixed(4);
						
						tradeFee = parseFloat(tradeFee);
						if (pd.bfx_fee > 0) {
							tradeFee = tradeFee + "(bfx)";
						} else if (tradePath == "trade") {
							tradeFee = tradeFee + "(usdt)";
						} else if (tradePath != "vtrade") {
							tradeFee = tradeFee + "(" + tradePath + ")";
						}
						
						if (pd.trade_type_id == 2 || pd.trade_type_id == 4) {
							income = parseFloat(pd.income);
							if (income >= 0) {
								income = "<span class='buy'>" + income + "</span>";
							} else {
								income = "<span class='sell'>" + income + "</span>";
							}
						}
					}
					
					pdHtml += "<tr>"
						+ "<td>" + dateFmt(pd.delegation_time * 1000) + "</td>"
						+ "<td>" + pd.transaction_code.toUpperCase() + "</td>"
						+ "<td>" + pdType + "</td>"
						+ "<td>" + parseFloat(pd.delegation_price) + "</td>"
						+ "<td>" + parseFloat(pd.delegation_count) + "</td>"
						+ "<td>" + parseFloat(pd.deal_count) + "</td>"
						+ "<td>" + avgPrice + "</td>"
						+ "<td>" + income + "</td>"
						+ "<td>" + tradeFee + "</td>"
						+ "<td>" + state + "</td>"
						+ "</tr>";
				}
			}
			$("#historyPendingTbody").html(pdHtml);
        }
	});
}

function refreshPending() {
	if ($('#userEmail').val() == null || $('#userEmail').val() == "") {
		return;
	}
	
	var transaction_code = $('#transaction_code').val();
	$.ajax({
        type: "POST",
        dataType: "json",         
        url: basePath + ajaxPath + "/pendingData.do",
        data: {"transactionCode":transaction_code, token : getCookie("_token")},
        success: function(data) {
			var pdHtml = "";
			if(data.size != 0) {
				$("#limitOrderSize").html(' (' + data.size + ')');
			} else {
				$("#limitOrderSize").html('');
			}
			var data = data.data;
			if (data.length == 0) {
				pdHtml = "<tr><td colspan='12'>" + MSG['noEntrustment'] + "</td></tr>";
				window.localStorage.removeItem("ntrade/usdr_" + transaction_code + "_multiple");
				showsliderBoxPending = true;
			} else {
				for (var index = 0; index < data.length; index++) {
					var pd = data[index];
					var pdType = "";
					var cancel = MSG['burned'];
					if (pd.direction == 1 && pd.priority == 1) {
						pdType = "<b class='buy'>" + MSG['olp'] + "</b>";
					} else if (pd.direction == 1 && pd.priority == 2) {
						pdType = "<b class='buy'>" + MSG['csp'] + "</b>";
					} else if (pd.direction == 1 && pd.priority == 3) {
						pdType = "<b class='loss-all'>" + MSG['bsp'] + "</b>";
					} else if (pd.direction == 2 && pd.priority == 1) {
						pdType = "<b class='sell'>" + MSG['osp'] + "</b>";
					} else if (pd.direction == 2 && pd.priority == 2) {
						pdType = "<b class='sell'>" + MSG['clp'] + "</b>";
					} else if (pd.direction == 2 && pd.priority == 3) {
						pdType = "<b class='loss-all'>" + MSG['blp'] + "</b>";
					}

					if (pd.priority < 3) {
						cancel = "<a href='javascript:void(0);' class='btn btn-blue' "
							+ " onclick=\"cancelDealAnsyc('" + pd.orderId + "','" + pd.symbol + "'," + (pd.state == "submitting" ? "true" : "false") + ");\">" + MSG['cancel'] + "</a>"
					}
					
					pdHtml += "<tr>"
						+ "<td>" + dateFmt(pd.createTime * 1000) + "</td>"
						+ "<td>" + pd.symbol.toUpperCase() + "</td>"
						+ "<td>" + pdType + "</td>"
						+ "<td>" + pd.multiple + "</td>"
						+ "<td>" + parseFloat(pd.price) + "</td>"
						+ "<td>" + parseFloat(pd.amount) + "</td>"
						+ "<td>" + parseFloat(pd.dealAmount) + "</td>"
						+ "<td>" + parseFloat(pd.undealAmount) + "</td>"
						+ "<td>" + parseFloat(pd.deposit) + "</td>"
						+ "<td>" + cancel + "</td>"
						+ "</tr>";
				}
				showsliderBoxPending = false;
			}
			$("#pendingTbody").html(pdHtml);
        }
	});
}
var isNullPrice = true;//判断是否使用卖一价格计算可用
var refershSellOne;
//获得卖一价格
function getSellOneUsable() {
	if(isNullPrice) {
		refreshSellOneMaxAmount();
	}
}
//按照卖一刷新可用
function refreshSellOneMaxAmount() {
	var myRemainAmount = Number($('#myRemainBtc').val());		// 合约当前可用
	var remainAmount = myRemainAmount;							// 合约当前可用
	var multiple = Number($('#multiple').val());
	var amountDigits = Number($('#amountDigits').val());
	var coeffcient = Number($('#coeffcient').val());
	var sellNum = $("#sellOneNum").val();                                 //卖一的价格
	var maxAmount = 0;
	if (sellNum > 0) {
		var feeRate = Number($('#myCommissionRate').val());						// 佣金（手续费）
		maxAmount = remainAmount / (sellNum / multiple + sellNum * feeRate) / coeffcient;	// 最大可开仓数量

		var maxStr = maxAmount + "";
		if (maxStr.indexOf('.') != -1 && maxStr.length - maxStr.indexOf('.') > amountDigits) {
			maxStr = maxStr.substring(0, maxStr.indexOf('.') + amountDigits + 1);
		}
		maxAmount = Number(maxStr);
		$('#maxAmount').html(maxAmount);
	} else {
		$('#maxAmount').html("0");
	}
}

var sellTrans = [];
var buyTrans = [];
//刷新持仓管理
function refreshMemberTrans() {
	if ($('#userEmail').val() == null || $('#userEmail').val() == "") {
		return false;
	}
	
	var transaction_code = $('#transaction_code').val();
	var suspend = $('#suspend').val();
	$.ajax({
        type: "POST",
        dataType: "json",
        url: basePath + ajaxPath + "/memTransData.do",
        data: {"transactionCode":transaction_code, token : getCookie("_token")},
        error: function (err) {
    		$("#alert_tr_pc_buy").css("display", "");
    		$("#infomsg_d").addClass('warning');
    		$("#infomsg_d").removeClass('success');
    		$("#infomsg_d").html(MSG['offline'] + "<a href='" + basePath + "login/login.do'>" + MSG['login'] + "</a>！");
    		
    		$("#alert_tr_pc_sell").css("display", "");
    		$("#infomsg_k").addClass('warning');
    		$("#infomsg_k").removeClass('success');
    		$("#infomsg_k").html(MSG['offline'] + "<a href='" + basePath + "login/login.do'>" + MSG['login'] + "</a>！");
    		
    		clearSellClosePanel();
    		clearBuyClosePanel();
        },
        success: function(data) {
        	var buyMemTrans = eval('(' + data.buyMemberTransaction + ')');
			var digits = data.digits;
			var amountDigits = data.amountDigits;
			var buy1 = data.buy1;
			var sell1 = data.sell1;
			
    		$("#alert_tr_pc_buy").css("display", "none");
    		$("#alert_tr_pc_sell").css("display", "none");
			
			if (data.buyMemeberTransCount > 0 && data.sellMemeberTransCount > 0) {
				$("#duipingDiv").css("display", "block");
			} else {
				$("#duipingDiv").css("display", "none");
			}
			
			$("#duipingPrice").val(data.lastPrice);
			
			
			var buyHtml = "<tr id='alert_tr_pc_buy' style='display: none;'>"
						+ "<td colspan='12'><div class='trade-alert' style='margin-top: 0;'>"
						+ "<div class='success' id='infomsg_d'></div>"
						+ "</div>"
						+ "<input type='hidden' value='" + data.buyMemeberTransCount + "' id='buyTransCount'/></td>"
						+ "</tr>";
			if (data.buyMemeberTransCount == 0) {
				buyHtml += "<tr><td colspan='12' id='no-buy-position'>" + MSG['noBuyPosition'] + "</td></tr>";
				$("#depotbuyTbody").html(buyHtml);
				clearBuyClosePanel();
				showsliderBoxMemberTransBuy = true;
			} else {
				var currMemberTranIds = [];//储存当前用户的持仓id列表
				$('tr[id^=tran_buy_]').each(function(){
				   var mt_id = $(this).attr('id').split("_")[2];
				   currMemberTranIds.push(mt_id);
				});
				for (var index = 0; index < buyMemTrans.length; index++) {
					var tran = buyMemTrans[index];
					var transIdIndex = currMemberTranIds.indexOf(tran.mt_id.toString());
					if(transIdIndex != -1) {
						currMemberTranIds[transIdIndex] = null;
					}
					var tr = $("#tran_buy_" + tran.mt_id);
					if(tr.length > 0) {
						updateMemberTransBuyData(tran);
					} else {
						var newTr = `<tr id="tran_buy_${tran.mt_id}">
							<td id="buyMultiple${tran.mt_id}"><em class="buy">${MSG['goLong']}</em>x${tran.multiple}</td>
							<td id="buyHoldCount${tran.mt_id}">${tran.holdCount}</td>
							<td id="buyLockCount${tran.mt_id}">${tran.lockCount}</td>
							<td id="buyAvgPrice${tran.mt_id}">${tran.averagePrice}</td>
							<td id="buyQpPrice${tran.mt_id}">${tran.qpPrice}</td>
							<td id="buyDeposit${tran.mt_id}">${tran.deposit}</td>
							<td class="${tran.floatingProfitLoss > 0 ? 'buy' : 'sell'}" id="floatGainLossbuy${tran.mt_id}">${tran.floatingProfitLoss}</td>
							<td id="floatGainLossRatebuy${tran.mt_id}">${tran.profitLossRatio}</td>
							<td>
								<input type="hidden" id="buy_trade_multiple${tran.mt_id}" value="${tran.multiple}"/>
								<input type="hidden" id="buy_all_count${tran.mt_id}" value="${tran.allCount}"/>
								<input type="hidden" id="buy_hold_count${tran.mt_id}" value="${tran.holdCount}"/>
								<input type="hidden" id="buy_average_price${tran.mt_id}" value="${tran.averagePrice}"/>
								<input type="hidden" id="buy_deposit${tran.mt_id}" value="${tran.deposit}"/>
								<input type="hidden" id="buy_qpj${tran.mt_id}" value="${tran.qpPrice}"/>
								<input type="hidden" id="buy_accuratePrice${tran.mt_id}" value="${tran.accuratePrice}"/>
								<input type="hidden" id="orderType${tran.mt_id}" value="0"/>
								<span class="td-child-control">
									 <input type="text" class="form-control price-buy-input td-input" id="pcPrice_d${tran.mt_id}" name="price"
                                  		value="" onkeyup="checkInputDigits(this, ${digits});" autocomplete="off"/>
									<div class="price-main" id="close_form_pc_buy${tran.mt_id}" style="display: none;">
										<div class="tb-overlay"></div>
										<div class="price-alert">
											<div class="arrow-left"></div>
											<p>${MSG['rules']}${MSG['alertMinSellPrice']}：
												<b id="price_pc_buy_1${tran.mt_id}">0</b> <br>${MSG['confirmBy']}
												<b id="price_pc_buy_2${tran.mt_id}">0</b>
												<em class="sell">${MSG['postionSell']}</em>
												<b id="amount_pc_buy${tran.mt_id}">0</b>
												${transaction_code.toUpperCase()}
											</p>
											<div class="calculat-sub">
												<button type="button" class="btn sub-btn" id="submit_but_buy${tran.mt_id}">${MSG['ok']}</button>
												<button type="button" class="btn sub-btn-return close-price-window" onclick="closePcBut(${tran.mt_id});">${MSG['close']}</button>
											</div>
										</div>
									</div>
								</span>
								
							</td>
							<td>
								<span class="td-child-control">
									<input type="text" class="form-control td-input" id="pcAmount_d${tran.mt_id}" name="amount" value="" autocomplete="off"
									 onblur="checkPcAmount('pcAmount_d${tran.mt_id}','${tran.holdCount}');" onkeyup="checkInputDigits(this, ${amountDigits});"/>
								  </span>
							</td>
							<td>
								<span class="td-child-control pc">
									<a href="javascript:void(0);" class="control-btn btn pc-btn" onclick="pc_d('${tran.bqCode}',${tran.mt_id});" ${suspend == "true" ? disabled="true" : ""}>${MSG['fmtLimitedBuyMore']}</a>
									<a href="javascript:void(0);" class="btn-all-pc" ${suspend == "true" ? disabled="true" : ""} onclick="oneClickCloseOk('${tran.bqCode}', ${tran.mt_id}, 1);">${MSG['fastClosePosition']}</a>
						
							
								<a class="zszy-btn ${tran.stopLoss == 0 ? "sprites-zszy-off" : "sprites-zszy-on"}" id="alertId_${tran.mt_id}" onclick="showFullProlitLoss(${tran.trade_type_id}, ${tran.mt_id});"></a>
								<!--止盈止损弹窗 -->
								<span class="zyzs-alert-window" id="trade-alert_dt${tran.mt_id}"></span>
								</span>
							</td>
						</tr>`;
						$("#no-buy-position").remove();
						$("#depotbuyTbody:last-child").append(newTr);
					}
				}
				for(var i= 0; i < currMemberTranIds.length; i++) {
					$("#tran_buy_" + currMemberTranIds[i]).remove();
				}
				showsliderBoxMemberTransBuy = false;
			}
			// resetBuyClosePanel(buyMemTrans);
			var buyDp = "";
			for (var index = 0; index < buyMemTrans.length; index++) {
				var tran = buyMemTrans[index];
				buyDp += "<tr onclick='clickSelectedIcon(1, " + index + ");'>"
					+ "<td><input type='hidden' value='" + tran.bqCode  + "' id='bqcodebuy" + index + "'>"
					+ "<input type='hidden' value='" + parseFloat(tran.holdCount)  + "' id='holdcountbuy" + index + "'>"
					+ "<input type='hidden' value='" + tran.mt_id + "' id='mtidbuy" + index + "'>"
					+ "<input type='hidden' value='" + parseFloat(tran.qpPrice)  + "' id='baocangbuy" + index + "'>"
					+ "<i class='checked' id='selectedIconBuy" + index + "'></i> " + tran.bqCode.toUpperCase() + " <b class='buy'>" + MSG['goLong'] + "</b>x" + tran.multiple + " </td>"
					+ "<td>" + parseFloat(tran.holdCount) + parseFloat(tran.lockCount)  + "</td>"
					+ "<td>" + parseFloat(tran.holdCount) + "</td>"
					+ "<td>" + parseFloat(tran.averagePrice) + "</td>"
					+ "<td class='" + (tran.floatingProfitLoss > 0 ? 'buy':'sell') + "'>" + parseFloat(tran.floatingProfitLoss) + "</td>"
					+ "</tr>"
			}
			$("#duipingBuyTbody").html(buyDp);
			
			var sellMemTrans = eval('(' + data.sellMemberTransaction + ')');
			var sellHtml = "<tr id='alert_tr_pc_sell' style='display: none;'>"
					+ "<td colspan='12'><div class='trade-alert' style='margin-top:0;'>"
					+ "<div class='success' id='infomsg_k'> </div>"
					+ "</div>"
					+ "<input type='hidden' value='" + data.sellMemeberTransCount + "' id='sellTransCount'/></td>"
					+ "</tr>";
			if (data.sellMemeberTransCount == 0) {
				sellHtml += "<tr><td colspan='12' id='no-sell-position'>" + MSG['noSellPosition'] + "</td></tr>";
				$("#depotsellTbody").html(sellHtml);
				clearSellClosePanel();
				showsliderBoxMemberTransSell = true;
			} else {
				var currMemberTranIds = [];//储存当前用户的持仓id列表
				$('tr[id^=tran_sell_]').each(function () {
					var mt_id = $(this).attr('id').split("_")[2];
					currMemberTranIds.push(mt_id);
				});
				for (var index = 0; index < sellMemTrans.length; index++) {
					var tran = sellMemTrans[index];
					var transIdIndex = currMemberTranIds.indexOf(tran.mt_id.toString());
					if (transIdIndex != -1) {
						currMemberTranIds[transIdIndex] = null;
					}
					var tr = $("#tran_sell_" + tran.mt_id);
					if (tr.length > 0) {
						updateMemberTransSellData(tran);
					} else {
						var newTr =  `<tr id="tran_sell_${tran.mt_id}">
							<td id="sellMultiple${tran.mt_id}"><em class="sell">${MSG['goShort']}</em> x${tran.multiple}</td>
							<td id="sellHoldCount${tran.mt_id}">${tran.holdCount}</td>
							<td id="sellLockCount${tran.mt_id}">${tran.lockCount}</td>
							<td id="sellAvgPrice${tran.mt_id}">${tran.averagePrice}</td>
							<td id="sellQpPrice${tran.mt_id}">${tran.qpPrice}</td>
							<td id="sellDeposit${tran.mt_id}">${tran.deposit}</td>
							<td class="${tran.floatingProfitLoss > 0 ? 'buy' : 'sell'}" id="floatGainLosssell${tran.mt_id}">${tran.floatingProfitLoss}
							</td>
							<td id="floatGainLossRatesell${tran.mt_id}">${tran.profitLossRatio}</td>
							<td>
								<input type="hidden" id="sell_trade_multiple${tran.mt_id}" value="${tran.multiple}"/>
								<input type="hidden" id="sell_all_count${tran.mt_id}" value="${tran.allCount}"/>
								<input type="hidden" id="sell_hold_count${tran.mt_id}" value="${tran.holdCount}"/>
								<input type="hidden" id="sell_average_price${tran.mt_id}" value="${tran.averagePrice}"/>
								<input type="hidden" id="sell_deposit${tran.mt_id}" value="${tran.deposit}"/>
								<input type="hidden" id="sell_qpj${tran.mt_id}" value="${tran.qpPrice}"/>
								<input type="hidden" id="sell_accuratePrice${tran.mt_id}" value="${tran.accuratePrice}"/>
								<input type="hidden" id="orderType${tran.mt_id}" value="0"/>
								<span class="td-child-control" >
								 	<input type="text" class="form-control price-sell-input td-input" id="pcPrice_k${tran.mt_id}" name="pcPrice_k" value=""
										onkeyup="checkInputDigits(this, ${digits});" autocomplete="off"/>
									<div class="price-main" id="close_form_pc_sell${tran.mt_id}" style="display: none;">
										<div class="tb-overlay"></div>
										<div class="price-alert" >
										  <div class="arrow-left"></div>
										  <p>${MSG['rules']} ${MSG['alertMaxBuyPrice']}： <b id="price_pc_sell_1${tran.mt_id}">0</b> <br>
											  ${MSG['confirmBy']}<b id="price_pc_sell_2${tran.mt_id}">0</b><em class="buy">${MSG['postionBuy']}</em><b id="amount_pc_sell${tran.mt_id}">0</b>${transaction_code.toUpperCase()}  </p>
										  <div class="calculat-sub">
											<button type="button" class="btn sub-btn" id="submit_but_sell${tran.mt_id}">${MSG['sureTip']}</button>
											<button type="button" class="btn sub-btn-return close-price-window" onclick="closePcBut(${tran.mt_id});">${MSG['close']}</button>
										  </div>
										</div>
									</div>
							  	</span>
							</td>
						  	<td>
						  		<span class="td-child-control">
									<input type="text" class="form-control td-input" id="pcAmount_k${tran.mt_id}" name="pc_Amount_k" value="" autocomplete="off"
								 		onblur="checkPcAmount('pcAmount_k${tran.mt_id}','${tran.holdCount}');" onkeyup="checkInputDigits(this,${amountDigits});" />
								</span>
						  	</td>
								<td>
								<span class="td-child-control pc">
									<a href="javascript:" class="control-btn btn pc-btn" onclick="pc_k('${tran.bqCode}',${tran.mt_id});" ${suspend == "true" ? disabled="true" : ""}>${MSG['fmtLimitedBuyMore']}</a>
									<a href="javascript:void(0);" class="btn-all-pc" ${suspend == "true" ? disabled="true" : ""} onclick="oneClickCloseOk('${tran.bqCode}', ${tran.mt_id}, 2);">${MSG['fastClosePosition']}</a>
									
									<a class="zszy-btn ${tran.stopLoss == 0 ? 'sprites-zszy-off' : 'sprites-zszy-on' } " id="alertId_${tran.mt_id}" onclick="showFullProlitLoss(${tran.trade_type_id}, ${tran.mt_id});"></a>
									<!--止盈止损弹窗 -->
									<span class="zyzs-alert-window" id="trade-alert_kt${tran.mt_id}"></span>
								</span>
							</td>
						</tr>`;
						$("#no-sell-position").remove();
						$("#depotsellTbody:last-child").append(newTr);
						}
					}
					for (var i = 0; i < currMemberTranIds.length; i++) {
						$("#tran_sell_" + currMemberTranIds[i]).remove();
					}
					showsliderBoxMemberTransSell = false;
				}
			// resetSellClosePanel(sellMemTrans);
			resetClosePanel(buyMemTrans,sellMemTrans);
			sellTrans = sellMemTrans;
	    	buyTrans = buyMemTrans;
			var sellDp = "";
			for (var index = 0; index < sellMemTrans.length; index++) {
				var tran = sellMemTrans[index];
				sellDp += "<tr onclick='clickSelectedIcon(2, " + index + ");' class=''>"
					+ "<td><input type='hidden' value='" + tran.bqCode  + "' id='bqcodesell" + index + "'>"
					+ "<input type='hidden' value='" + parseFloat(tran.holdCount)  + "' id='holdcountsell" + index + "'>"
					+ "<input type='hidden' value='" + tran.mt_id + "' id='mtidsell" + index + "'>"
					+ "<input type='hidden' value='" + parseFloat(tran.qpPrice)  + "' id='baocangsell" + index + "'>"
					+ "<i class='checked' id='selectedIconSell" + index + "'></i> " + tran.bqCode.toUpperCase() + " <em class='sell'>空</em>x" + tran.multiple + " </td>"
					+ "<td>" + parseFloat(tran.holdCount) + parseFloat(tran.lockCount)  + "</td>"
					+ "<td>" + parseFloat(tran.holdCount) + "</td>"
					+ "<td>" + parseFloat(tran.averagePrice) + "</td>"
					+ "<td class='" + (tran.floatingProfitLoss > 0 ? 'buy':'sell') + "'>" + parseFloat(tran.floatingProfitLoss)  + "</td>"
					+ "</tr>"
			}
			$("#duipingSellTbody").html(sellDp);
			showsliderMultiqleBox = false;
        }
    });
}
var buyAveragePrice = '';
//更新持仓数据-buy
function updateMemberTransBuyData(tran) {
	$("#buyMultiple" + tran.mt_id).html("<em class='buy'>" + MSG['goLong'] + "</em> x" + tran.multiple);
	$("#buyHoldCount" + tran.mt_id).html(tran.holdCount);
	$("#buyLockCount" + tran.mt_id).html(tran.lockCount);
	$("#buyAvgPrice" + tran.mt_id).html(tran.averagePrice);
	$("#buyQpPrice" + tran.mt_id).html(tran.qpPrice);
	$("#buyDeposit" + tran.mt_id).html(tran.deposit);
	$("#floatGainLossbuy" + tran.mt_id).html(tran.floatingProfitLoss);
	$("#floatGainLossRatebuy" + tran.mt_id).html(tran.profitLossRatio);
	$("#buy_trade_multiple" + tran.mt_id).val(tran.multiple);
	$("#buy_all_count" + tran.mt_id).val(tran.allCount);
	$("#buy_hold_count" + tran.mt_id).val(tran.holdCount);
	$("#buy_average_price" + tran.mt_id).val(tran.averagePrice);
	$("#buy_deposit" + tran.mt_id).val(tran.deposit);
	$("#buy_qpj" + tran.mt_id).val(tran.qpPrice);
	$("#buy_accuratePrice" + tran.mt_id).val(tran.accuratePrice);
	buyAveragePrice = tran.averagePrice;
}
var sellAveragePrice = '';
//更新持仓数据-sell
function updateMemberTransSellData(tran) {
	$("#sellMultiple" + tran.mt_id).html("<em class='sell'>" + MSG['goShort'] + "</em> x" + tran.multiple);
	$("#sellHoldCount" + tran.mt_id).html(tran.holdCount);
	$("#sellLockCount" + tran.mt_id).html(tran.lockCount);
	$("#sellAvgPrice" + tran.mt_id).html(tran.averagePrice);
	$("#sellQpPrice" + tran.mt_id).html(tran.qpPrice);
	$("#sellDeposit" + tran.mt_id).html(tran.deposit);
	$("#floatGainLosssell" + tran.mt_id).html(tran.floatingProfitLoss);
	$("#floatGainLossRatesell" + tran.mt_id).html(tran.profitLossRatio);
	$("#sell_trade_multiple" + tran.mt_id).val(tran.multiple);
	$("#sell_all_count" + tran.mt_id).val(tran.allCount);
	$("#sell_hold_count" + tran.mt_id).val(tran.holdCount);
	$("#sell_average_price" + tran.mt_id).val(tran.averagePrice);
	$("#sell_deposit" + tran.mt_id).val(tran.deposit);
	$("#sell_qpj" + tran.mt_id).val(tran.qpPrice);
	$("#sell_accuratePrice" + tran.mt_id).val(tran.accuratePrice);
	sellAveragePrice = tran.averagePrice;
}

//重置杠杆
function resetClosePanel(buyMemTrans, sellMemTrans) {
	var multiplesHtml = "";
	var defaultMultiple = null;
	for (var index = 0; index < sellMemTrans.length; index++) {
		var tran = sellMemTrans[index];
		multiplesHtml += `<span class="option-multiple-close" id="sellCloseMultiple-${tran.multiple}" onclick="sellMultipleClick(${tran.multiple}, this)">x${tran.multiple}</a>`;
	}

	for (var index = 0; index < buyMemTrans.length; index++) {
		if (sellMemTrans.find(x => x.multiple == buyMemTrans[index].multiple)) {
			continue;
		}
		var tran = buyMemTrans[index];
		multiplesHtml += `<span class="option-multiple-close" id="sellCloseMultiple-${tran.multiple}" onclick="sellMultipleClick(${tran.multiple}, this)">x${tran.multiple}</a>`;
	}

	$("#sellCloseMultiples").html(multiplesHtml);
	var sellCloseMultiple = $("#sellCloseMultiple").val();

	if(sellCloseMultiple && sellCloseMultiple != 0){
		sellMultipleClick(sellCloseMultiple, $("#sellCloseMultiple-" + sellCloseMultiple));
	} else {
		$(".option-multiple-close").each(function(index) {
			if(index == 0) {
				$(this).addClass("active");
				var defaultMultiple = 0;
				if(sellMemTrans.length > 0) {
					defaultMultiple = sellMemTrans[0].multiple;
				} else {
					if(buyMemTrans.length > 0) {
						defaultMultiple = buyMemTrans[0].multiple;
					}
				}
				if(defaultMultiple > 0) {
					sellMultipleClick(defaultMultiple, $(this));
				}

			}
		});
	}
}

//重置杠杆后点击
function sellMultipleClick(multiple, sender) {
	$(".option-multiple-close").removeClass("active");
	$(sender).addClass("active");
	var activeMsg = $(sender).html()
	$(".close-input-inner").val(activeMsg);

	var sell = sellTrans.find(x => x.multiple == multiple);
	$("#sellCloseMultiple").val(multiple);
	$("#buyCloseMultiple").val(multiple);
	if (sell) {
		$("#holdSellAvgPrice").html($("#sellAvgPrice" + sell.mt_id).html());
		$("#holdSellAmount").html($("#sellHoldCount" + sell.mt_id).html());
		$("#sellCloseMId").val(sell.mt_id);

	} else {
		$("#holdSellAmount").html("0");
		$("#sellCloseMId").val();
	}

	var buy = buyTrans.find(x => x.multiple == multiple);
	if (buy) {
		$("#holdBuyAvgPrice").html($("#buyAvgPrice" + buy.mt_id).html());
		$("#holdBuyAmount").html($("#buyHoldCount" + buy.mt_id).html());
		$("#buyCloseMId").val(buy.mt_id);
	} else {
		$("#holdBuyAmount").html("0");
		$("#buyCloseMId").val();
	}
}

function resetSellClosePanel(sellMemTrans) {
	var sellCloseMId = $("#sellCloseMId").val();
	if ((sellCloseMId == null || typeof(sellCloseMId) == "undefined") && sellMemTrans.length > 0) {
		sellCloseMId = sellMemTrans[0].mt_id;
	}

	var sellMultiplesHtml = "";
	var currIndex = 0;
	for (var index = 0; index < sellMemTrans.length; index++) {
		var tran = sellMemTrans[index];
		var current = "";
		if (tran.mt_id == sellCloseMId) {
			current = "current";
			currIndex = index;
		}
		sellMultiplesHtml += "<a href='javascript:void(0);' " 
						+ "class='sell-close-multiple " +  current +"' id='sellCloseMultiple-" + index + "' "
						+ "onclick='sellMultipleClick(" + tran.mt_id + "," + tran.multiple + ",this);'>x" + tran.multiple + "</a>";
	}
	
	$("#sellCloseMultiples").html(sellMultiplesHtml);
	$("#sellCloseMultiple-" + currIndex).click();
}

function resetBuyClosePanel(buyMemTrans) {
	var buyCloseMId = $("#buyCloseMId").val();
	if ((buyCloseMId == null || typeof(buyCloseMId) == "undefined") && buyMemTrans.length > 0) {
		buyCloseMId = buyMemTrans[0].mt_id;
	}

	var buyMultiplesHtml = "";
	var currIndex = 0;
	for (var index = 0; index < buyMemTrans.length; index++) {
		var tran = buyMemTrans[index];
		var current = "";
		if (tran.mt_id == buyCloseMId) {
			current = "current";
			currIndex = index;
		}
		buyMultiplesHtml += "<a href='javascript:void(0);' "
						+ "class='buy-close-multiple " +  current +"' id='buyCloseMultiple-" + index + "' "
						+ "onclick='buyMultipleClick(" + tran.mt_id + "," + tran.multiple + ",this);'>x" + tran.multiple + "</a>";
	}
	
	$("#buyCloseMultiples").html(buyMultiplesHtml);
	$("#buyCloseMultiple-" + currIndex).click();
}

function clearSellClosePanel() {
	$(".sell-close-multiple").remove();
	$("#holdSellAvgPrice").html("0");
	$("#holdSellAmount").html("0");
}
function clearBuyClosePanel() {
	$(".buy-close-multiple").remove();
	$("#holdBuyAvgPrice").html("0");
	$("#holdBuyAmount").html("0");
}

/**
 * 平仓 对做多仓
 * 
 * @return
 */
/*限价平仓---卖出平多提交*/
function pc_d_from_closePos(code) {
	var id = $("#buyCloseMId").val();
	if (typeof(id) == "undefined" || id <= 0) {
		tipAlert(MSG['nullClosePostion']);
		return false;
	}
	
	var hold_count = Number($('#holdBuyAmount').html());
	if(hold_count <= 0){
		tipAlert(MSG['nullClosePostion']);
		return false;
	}
	
	var amount = Number($("#closeSellAmount").val());
	if (isNaN(amount) || amount == 0 || amount > hold_count) {
		tipAlert(MSG['legalClosePostion']);
		return false;
	}
	
	var closeOrderType = $("#closeOrderType").val();
	if(closeOrderType == '') {
		var price = Number($("#closeSellPrice").val());
		if (isNaN(price) || price == 0) {
			tipAlert(MSG['legalClosePostionPrice']);
			return false;
		}
		
		var minPrice = Number($('#minSell').html());
		if (price < minPrice) {
			tipAlert(MSG['alertMinSellPrice']+ "："+ minPrice);
			return false;
		}
	}
	
	$.ajax({
        type: "POST",
        dataType: "json",         
        url: basePath + tradeAjaxPath + "/tradePDC.do",
        data: {
    		price : price,
    		amount : amount,
    		code : code,
    		id : id,
    		token : getCookie("_token"),
    		orderType : closeOrderType
    	},
        success: function(data){
        	showMsgBox(data.code, data.msg);
    		showSuccInfo_pcd(data.msg, data.code);
    		return;
        }
	});
}

function pc_d(code, id) {
	var hold_count = Number($('#buy_hold_count'+id).val());
	if(hold_count <= 0){
		tipAlert(MSG['nullClosePostion']);
		return false;
	}
	
	var pcAmount_d = Number($("#pcAmount_d"+id).val());
	if (isNaN(pcAmount_d) || pcAmount_d == 0 || pcAmount_d > hold_count) {
		tipAlert(MSG['legalClosePostion']);
		return false;
	}
	
	var price = Number($("#pcPrice_d"+id).val());
	if (isNaN(price) || price == 0) {
		tipAlert(MSG['legalClosePostionPrice']);
		return false;
	}
	
	var multiple = Number($("#buy_trade_multiple"+id).val());
	var transaction_code = $('#transaction_code').val();
	
	var minPrice = Number($('#minSell').html());
	if (price < minPrice) {
		$('#pcPrice_d'+id).val(minPrice);
		showSuccInfo_pc(id, Number(minPrice), pcAmount_d, code, multiple, hold_count, 1);
		return false;
	}
	
	closePcBut(id);
	
	$.ajax({
        type: "POST",
        dataType: "json",         
        url: basePath + tradeAjaxPath + "/tradePDC.do",
        data: {
    		price : price,
    		amount : pcAmount_d,
    		code : code,
    		id : id,
    		token : getCookie("_token")
    	},
        success: function(data){
//    		$('#pcAmount_d'+id).val((hold_count-pcAmount_d).toFixed(2));
			$(".show-pc-box" + id).hide();
    		showMsgBox(data.code, data.msg);
    		showSuccInfo_d(data.msg, data.code);
    		return;
        }
	});
}

/*平仓杠杆点击*/
function closeMultipleClick(multiple, self) {
	var code = $('#transaction_code').val();
	var activeMsg = $(self).html();
	$(".option-multiple-close").removeClass("active");
	$(self).addClass("active");
	if($(self).hasClass("active")) {
		$(".close-input-inner").val(activeMsg).css("border", "1px solid #4A5F78");
		$(".select-close-item").hide();
	}
	$("#closeSellAmount").val('');
	$("#closeSellPrice").val('');
	$(".trade-percent-sell span").removeClass("active");
	$.ajax({
		type: "POST",
        dataType: "json",
        url: basePath + tradeAjaxPath + "/getMemberTransaction.do",
        data: {
        	multiple : multiple,
        	code : code
    	},
        success: function(data){
        	var dataArr = data.data;
        	if (dataArr.length == 0) {
        		$("#holdBuyAmount").html('0');
        		$("#holdSellAmount").html('0');
        	} else if (dataArr.length == 1) {
        		if(dataArr.direction == 1) {
					$("#holdSellAmount").html('0');
				} else {
					$("#holdBuyAmount").html('0');
				}
        	}
    		for (var i = 0; i < dataArr.length; i++) {
    			var dataArray = dataArr[i];
    			if(dataArray.direction == 1) {
    				$("#buyCloseMId").val(dataArray.positionId);
    				$("#buyCloseMultiple").val(dataArray.multiple);
					$("#holdBuyAmount").html(dataArray.holdCount);
				} else {
					$("#sellCloseMId").val(dataArray.positionId);
					$("#sellCloseMultiple").val(dataArray.multiple);
					$("#holdSellAmount").html(dataArray.holdCount);
				}
    		}
        }
	})
}

/*开仓后，刷新持仓可平数量*/
function refreshMultipleClick(multiple) {
	var code = $('#transaction_code').val();
	$.ajax({
		type: "POST",
        dataType: "json",
        url: basePath + tradeAjaxPath + "/getMemberTransaction.do",
        data: {
        	multiple : multiple,
        	code : code
    	},
        success: function(data){
        	var dataArr = data.data;
        	if (dataArr.length == 0) {
        		$("#holdBuyAmount").html('0');
        		$("#holdSellAmount").html('0');
        	} else if (dataArr.length == 1) {
        		if(dataArr.direction == 1) {
					$("#holdSellAmount").html('0');
				} else {
					$("#holdBuyAmount").html('0');
				}
        	}
    		for (var i = 0; i < dataArr.length; i++) {
    			var dataArray = dataArr[i];
    			if(dataArray.direction == 1) {
    				$("#buyCloseMId").val(dataArray.positionId);
    				$("#buyCloseMultiple").val(dataArray.multiple);
					$("#holdBuyAmount").html(dataArray.holdCount);
				} else {
					$("#sellCloseMId").val(dataArray.positionId);
					$("#sellCloseMultiple").val(dataArray.multiple);
					$("#holdSellAmount").html(dataArray.holdCount);
				}
    		}
        }
	})
}

/**
 * 对平
 * 
 * @return
 */
function pc_dp() {
	
	var buy_hold_count = Number($('#holdCountBuy').val());
	var sell_hold_count = Number($('#holdCountSell').val());

	if(isNaN(buy_hold_count) || buy_hold_count <= 0) {
		tipAlert(MSG['onOfFlatMore']);
		return false;
	}
	
	if(isNaN(sell_hold_count) || sell_hold_count <= 0) {
		tipAlert(MSG['onOfFlatNull']);
		return false;
	}
	
	var pcAmount = Number($("#duipingNum").val());
	var holdCountBuy = Number($("#holdCountBuy").val());
	var holdCountSell = Number($("#holdCountSell").val());
	if (isNaN(pcAmount) || pcAmount == 0 || pcAmount > holdCountBuy || pcAmount > holdCountSell) {
		tipAlert(MSG['legalClosePostion']);
		return false;
	}
	
	var price = Number($("#duipingPrice").val());
	if (isNaN(price) || price == 0) {
		tipAlert(MSG['legalClosePostionPrice']);
		return false;
	}
	
	var maxPrice = Number($('#maxBuy').html());
	var minPrice = Number($('#minSell').html());
	if (price > maxPrice || price < minPrice) {
		tipAlert(MSG['onOfFlatPriceRange'] + minPrice + "~" + maxPrice);
		return false;
	}
	
	// 买一卖一之间
	var buy1 = Number($("#buy1").val());
	var sell1 = Number($("#sell1").val());
	if(isNaN(buy1) || buy1 <= 0) {
		tipAlert(MSG['onOfFlatPriceBuy']);
		return false;
	}
	
	if(isNaN(sell1) || sell1 <= 0) {
		tipAlert(MSG['onOfFlatPriceBuy']);
		return false;
	}
	
	if (price <= buy1 && buy1 > 0){
		tipAlert(MSG['onOfFlatPriceBuy']);
		return false;
	}
	
	if(price >= sell1 && sell1>0){
		tipAlert(MSG['onOfFlatPriceBuy']);
		return false;
	}
	
	// 买多，不得低于爆仓价，卖空，不得高于爆仓价
	var baocangBuy = Number($("#baocangBuy").val());
	var baocangSell = Number($("#baocangSell").val());
	if (price <= baocangBuy || price >= baocangSell) {
		tipAlert(MSG['onOfFlatPriceBlaste']);
		return false;
	}
	
    var codeBuy = $("#bqCodeBuy").val();
    var tidBuy = $("#mtIdBuy").val();
    
    var codeSell = $("#bqCodeSell").val();
    var tidSell = $("#mtIdSell").val();
	
	duiping(price, pcAmount, codeBuy, tidBuy, basePath + tradeAjaxPath + "/tradePDC.do");
	duiping(price, pcAmount, codeSell, tidSell, basePath + tradeAjaxPath + "/tradePKC.do");
}

function duiping(price, pcAmount, codeSell, tidSell, URL){
	$.ajax({
        type: "POST",
        dataType: "json",         
        url: URL,
        data:  {
    		price : price,
    		amount : pcAmount,
    		code : codeSell,
    		id : tidSell,
    		token : getCookie("_token")
    	},
        success: function(data){
    		showSuccInfo_dp(data.msg, data.code);
        }
	});
}

function showSuccInfo_dp(text, code) {
	if(text.length > 0) {
		$("#msg_duiping").html("");
		if (code == "success") {
			$("#msg_duiping").html(MSG['onOfFlatSuccess']);
		}else {
			$("#msg_duiping").html("");
		}
		var time = 3000;
		if (code == 'error') {
			time = 6000;
		}
		setTimeout("showSuccInfo_dp('','')", time);
	} else {
		var duiping = $("#duiping-wrap"); 
		var dpzz = $("#tb-overlay-duiping");
		hide(duiping);
		dpzz.hide();
		
		$("#tab-trade .current").click();
		refreshAccount();
		refreshMultiple();
	}
	
}

function showSuccInfo_pc(id, price, pcAmount_d, code, multiple, hold_count, type){
	if (type == 1) {
		$('#close_form_pc_buy'+id).css('display','block');
		$('#price_pc_buy_1'+id).html(price);
		$('#price_pc_buy_2'+id).html(price);
		$('#amount_pc_buy'+id).html(pcAmount_d);
		$('#submit_but_buy'+id).removeAttr("onclick");
		$('#submit_but_buy'+id).attr("onclick", "pc_d('"+code+"',"+id+");");
	} else if (type == 2) {
		$('#close_form_pc_sell'+id).css('display', 'block');
		$('#price_pc_sell_1'+id).html(price);
		$('#price_pc_sell_2'+id).html(price);
		$('#amount_pc_sell'+id).html(pcAmount_d);
		$('#submit_but_sell'+id).removeAttr("onclick");
		$('#submit_but_sell'+id).attr("onclick", "pc_k('"+code+"',"+id+");");
	}
}

function closePcBut(id){
	$('#close_form_pc_buy'+id).css('display','none');
	$('#close_form_pc_sell'+id).css('display','none');
}

/**
 * 平仓 对做空仓
 * 
 * @return
 */
function pc_k(code, id) {
	var hold_count =  Number($('#sell_hold_count'+id).val());
	if(hold_count <= 0){
		tipAlert(MSG['nullClosePostion']);
		return false;
	}
	
	var pcAmount_k = Number($("#pcAmount_k"+id).val());
	if (isNaN(pcAmount_k) || pcAmount_k == 0 || pcAmount_k > hold_count) {
		tipAlert(MSG['legalClosePostion']);
		return false;
	}
	
	var price = Number($("#pcPrice_k"+id).val());
	if (isNaN(price) || price == 0) {
		tipAlert(MSG['legalClosePostionPrice']);
		return false;
	}
	
	var multiple = Number($("#sell_trade_multiple"+id).val());
	var transaction_code = $('#transaction_code').val();
	var maxPrice = Number($('#maxBuy').html());
	if (price > maxPrice) {
		$('#pcPrice_k'+id).val(maxPrice);
		showSuccInfo_pc(id, Number(maxPrice), pcAmount_k, code, multiple, hold_count, 2);
		return false;
	}
	
	closePcBut(id);
	
	$.ajax({
        type: "POST",
        dataType: "json",         
        url: basePath + tradeAjaxPath + "/tradePKC.do",
        data:  {
    		price : price,
    		amount : pcAmount_k,
    		code : code,
    		id : id,
    		token : getCookie("_token")
    	},
        success: function(data){
//    		$('#pcAmount_k'+id).val((hold_count-pcAmount_k).toFixed(2));
			$(".show-pc-box" + id).hide();
    		showMsgBox(data.code, data.msg);
    		showSuccInfo_k(data.msg, data.code);
    		return;
        }
	});

}

/*限价平仓---买入平空提交*/
function pc_k_from_closePos(code) {
	var id = $("#sellCloseMId").val();
	if (typeof(id) == "undefined" || id <= 0) {
		tipAlert(MSG['nullClosePostion']);
		return false;
	}
	
	var hold_count =  Number($('#holdSellAmount').html());
	if(hold_count <= 0){
		tipAlert(MSG['nullClosePostion']);
		return false;
	}
	
	var amount =  Number($('#closeSellAmount').val());
	if (isNaN(amount) || amount == 0 || amount > hold_count) {
		tipAlert(MSG['legalClosePostion']);
		return false;
	}
	
	var closeOrderType = $("#closeOrderType").val();
	if(closeOrderType == '') {
		var price =  Number($('#closeSellPrice').val());
		if (isNaN(price) || price == 0) {
			tipAlert(MSG['legalClosePostionPrice']);
			return false;
		}
		
		var maxPrice = Number($('#maxBuy').html());
		if (price > maxPrice) {
			tipAlert(MSG['alertMaxBuyPrice']+ "：" + maxPrice);
			return false;
		}
	}

	
	$.ajax({
        type: "POST",
        dataType: "json",         
        url: basePath + tradeAjaxPath + "/tradePKC.do",
        data:  {
    		price : price,
    		amount : amount,
    		code : code,
    		id : id,
    		token : getCookie("_token"),
    		orderType : closeOrderType
    	},
        success: function(data){
        	showMsgBox(data.code, data.msg);
        	showSuccInfo_pck(data.msg, data.code);
    		return;
        }
	});

}

/**
 * 校验平仓数量
 */
function checkPcAmount(id ,holdCount){
	var pc_amount = Number($("#"+id).val());
	if(pc_amount > holdCount ){
		$("#"+id).val(holdCount);
	}
}

/**
 * 校验上证平仓价格
 */
function checkszPcPrice(id){
	var price = Number($("#"+id).val());

	if (price < 2000) {
		price = 2000;
	}
	
	if (price > 10000) {
		price = 10000;
	}
	
	if (price.toString().indexOf('.') >= 0) {
		
		var weishu =  price.toString().substr(price.toString().indexOf('.') + 1);
		if (weishu == '' || weishu % 2 != 0) {
			weishu = weishu - 1;
		}
		price = price.toString().substr(0, price.toString().indexOf('.')) + "." + weishu;
	}
	$("#"+id).val(price);

}

/**
 * 校验平仓价格
 */
function checkPcPrice(direction){
	var btc_index_price = Number($('#btc_index_price').val());// 当前价格指数
	var latest_transaction_price = Number($('#lastPrice').val());// 最新成交价
	latest_transaction_price = Number($("#lastPriceOneMinAgo").val());
	
	var transaction_code = $('#transaction_code').val();
// var btc_index_price_fd = Number(map.get(transaction_code));// 价格指数的上下浮动比例
	
	// var maxPrice = formatFloat(latest_transaction_price * 1.01,2);
	// var minPrice = formatFloat(latest_transaction_price * 0.99,2);
	
	
	
	if(direction ==1){// 平多仓2
		var btc_index_price_fd = Number(mapDown.get(transaction_code));// 价格指数的上下浮动比例
		var minIndexPrice = btc_index_price*(1 - btc_index_price_fd);
		var price = Number($("#pcPrice_d").val());
		if (latest_transaction_price <= 0) {
			if (price < minIndexPrice) {
				$('#pcPrice_d').val(minIndexPrice.toFixed(2));
				return false;
			}
		} else {
			var minPrice = formatFloat(latest_transaction_price * 0.99,2);
			
			var maxprice = Math.max(minIndexPrice, minPrice);
			if (price < maxprice) {
				$('#pcPrice_d').val(maxprice.toFixed(2));
				return false;
			}
		}
		
	}else{// 平空仓
		var btc_index_price_fd = Number(mapUp.get(transaction_code));// 价格指数的上下浮动比例
		var maxIndexPrice = btc_index_price*(1 + btc_index_price_fd);
		
		var price = Number($("#pcPrice_k").val());
		
		if (latest_transaction_price <= 0) {
			if (price > maxIndexPrice) {
				$('#pcPrice_k').val(formatFloat(maxIndexPrice, 2));
				return false;
			}
		} else {
			var maxPrice = formatFloat(latest_transaction_price * 1.01,2);
			
			var minprice = Math.min(maxIndexPrice, maxPrice);
			if (price > minprice) {
				$('#pcPrice_k').val(formatFloat(minprice, 2));
				return false;
			}
		}
		
	}
}
/**
 * 一键平仓确认弹框
 *
 * @return
 */
function oneClickCloseOk(code, id, direction) {
	$("#add-windows-close").show();
	var insidehtm = `<div id="tb-overlay-duiping" class="tb-overlay"></div>
						<div class="add-wrap alert-wrap" style="display:block;"  id='invite-BFX'>
							<div class="dialogs-title">
								<h2 style="text-align: center;text-indent:0;">${MSG['fastClosePosition']}</h2>
								<a href="javascript:closeWindowsCloseAlert()" class="close-icon icons-close"></a>
							</div>
							<div class="dialogs-body">
								<p style="text-align: center; font-size: 16px; margin-bottom: 30px;">
									${MSG['oneClickCloseOk']}
								</p>
								<div class="form-group-btn text-center">
									<a class="btn sub-btn center" style="margin-right: 0px;background-color: #c55555;" href="javascript:closeWindowsCloseAlert();">${MSG['ClosetheBox']}</a>;
									<a class="btn sub-btn center" style="margin-right: 0px;" href="javascript:void(0)" onclick="pc('${code}', ${id}, ${direction})">${MSG['sureTip']}</a>
								</div>
							</div>
						</div>`
	$("#add-windows-close").html(insidehtm);
	center($('.alert-wrap'));
}

function closeWindowsCloseAlert() {
	$("#add-windows-close").css("display", "none");
}
/**
 * 平仓 对做空仓
 * 
 * @return
 */
function pc(code, id, direction) {
	$("#add-windows-close").css("display", "none");
	var hold_count = Number($('#buy_hold_count'+id).val());
	if (direction == 2) {
		hold_count = Number($('#sell_hold_count'+id).val());
	}
	if(hold_count <= 0){
		tipAlert(MSG['nullClosePostion']);
		return false;
	}
	
	var pcAmount_k = Number($("#pcAmount_k"+id).val());
	var transaction_code = $('#transaction_code').val();
	
	closePcBut(id);
	
	$.ajax({
        type: "POST",
        dataType: "json",         
        url: basePath + tradeAjaxPath + "/tradePC.do",
        data:  {
    		code : code,
    		id : id,
    		token : getCookie("_token")
    	},
        success: function(data){
//    		$('#pcAmount_k'+id).val((hold_count-pcAmount_k).toFixed(2));
    		showMsgBox(data.code, data.msg);
    		showSuccInfo_k(data.msg, data.code);
    		return;
        }
	});

}

/**
 * 撤单
 * 
 * @return
 */
function cancelDeal(direction, code) {
	$.get(basePath + tradeAjaxPath + "/cancleDeal.do?t=" + new Date().getTime()+"&token="+getCookie("_token"), {
		direction : direction,
		code : code
	}, function(data) {
		refreshPending();
		refreshAccount();
		refreshMultiple();
	});

}

function cancelDealAnsyc(id, code, cache) {
	$.get(basePath + tradeAjaxPath + "/canclePendingAnsyc.do?t=" + new Date().getTime()+"&token="+getCookie("_token"), {
		pdId : id,
		code : code,
		cache: cache
	}, function(data) {
		refreshMultiple();
		refreshPending();
		refreshAccount();
	});
}

function pricejsbos(price,bz){
	if(bz=-1)
		price=price + 0.1;
	if((parseInt(price*10))%2!=0){
		if(bz=-1)
			price=price-0.1;
		else
			price=price+0.1;
	}
	
	return Number((price).toFixed(1));
}

function pricejsbos2(price,bz){
	if((parseInt(price*10))%2!=0){
		if(bz==-1)
			price=price-0.1;
		else
			price=price+0.1;
	}
	
	return Number((price).toFixed(1));
}


function multipleClick(val, obj) {
	var activeMsg = $(obj).html();
	$(".option-multiple").removeClass("active");
	$(obj).addClass("active");
	$('#multiple').val(val);
	$("#planMultiple" + val).addClass("active");
	$('#planMultiple').val(val);
	$("#market_multiple" + val).addClass("active");
	$('#market_multiple').val(val);
	if($(obj).hasClass("active")) {
		$(".order-input-inner").val(activeMsg).css("border", "1px solid #4A5F78");
		$(".select-order-item").hide();
	}
	setMultiple(val);
	trade_amount();
}

/**
 * 开仓做多(数量)
 * 
 * @return
 */
function trade_amount(showAlert, maxLimit) {
	var myRemainAmount = Number($('#myRemainBtc').val());		// 合约当前可用
	var remainAmount = myRemainAmount;							// 合约当前可用
	var amount = Number($('#amount').val());					// 开仓数量
	var price = Number($('#price').val());
	var multiple = Number($('#multiple').val());
	var amountDigits = Number($('#amountDigits').val());
	var coeffcient = Number($('#coeffcient').val());	
	
	var maxAmount = 0;
	if (price > 0) {
		var feeRate = Number($('#myCommissionRate').val());						// 佣金（手续费）
		maxAmount = remainAmount / (price / multiple + price * feeRate) / coeffcient;	// 最大可开仓数量
		
		var maxStr = maxAmount + "";
		if (maxStr.indexOf('.') != -1 && maxStr.length - maxStr.indexOf('.') > amountDigits) {
			maxStr = maxStr.substring(0, maxStr.indexOf('.') + amountDigits + 1);
		}
		maxAmount = Number(maxStr);
		$('#maxAmount').html(maxAmount);
		isNullPrice = false;
		clearInterval(refershSellOne);
	} else {
		$('#maxAmount').html("0");
		isNullPrice = true;
		refreshSellOneMaxAmount();
	}
	
	if (price == 0 || amount == 0) {
		$('#baozhengjin').html(0);
		return false;
	}

	if (amount < Number($('#minKcNums').val())) {
		if (typeof(showAlert) != "undefined" && showAlert) {
			tipAlert(MSG['alertLitOpenNum'] + "：" + Number($('#minKcNums').val()) + MSG['alertGe']);
		}
		amount = $('#minKcNums').val();
		return false;
	}

	// 开仓数大于最大可开仓数时,更改 开仓数
	if (amount > maxAmount) {
		if (typeof(showAlert) != "undefined" && showAlert) {
			tipAlert(MSG['curtnotEnoughMoney'] + maxAmount + MSG['alertGe']);
			$('#amount').val(maxAmount);
		}
		if (maxLimit) {
			$('#amount').val(maxAmount);
		}
	} else {
		$('#amount').val(formatFloat(amount, amountDigits));
	}
	
	$('#baozhengjin').html((price * ($('#amount').val() / multiple) * coeffcient).toFixed(4));
	
	return true;
}

function sztrade_amount() {
	var remainAmount = Number($('#myRemainBtc').val());// 可用余额
	var multiple = Number($('#multiple').val());// 选中的杠杆（倍数）
	var amount = Number($('#amount').val());// 开仓数
	var price = Number($('#price').val());// 报价
	
	if (amount <= 0) {
		$('#amount').val("1")
		amount = 1;
	}
	
	if (price > 0) {
		var am = parseInt(remainAmount * multiple * 10000 / price);
		amount = amount > am ? am : amount;
		if(amount<0){
			amount=0;
		}
		if (amount < 1) {
			$('#amount').val(0);
			return;
		}
		$('#amount').val(formatFloat(amount, 0));
		$('#baozhengjin').html((price * amount / (multiple * 10000)).toFixed(4));		
	}
	
	return false;
}

/**
 * 开仓做多(单价)
 * 
 * @return
 */
function trade_price() {
	trade_amount();
}

function sztrade_price() {
	var remainAmount = Number($('#myRemainBtc').val());// 可用余额
	var multiple = Number($('#multiple').val());// 选中的杠杆（倍数）
	var amount = Number($('#amount').val());// 开仓数量
	var price = Number($('#price').val());// 报价
	
	if (price < 2000) {
		$('#price').val("2000")
		price = 2000;
	}
	
	if (price > 10000) {
		$('#price').val("10000")
		price = 10000;
	}
	
	if (price.toString().indexOf('.') >= 0) {
		var weishu =  price.toString().substr(price.toString().indexOf('.') + 1);
		if (weishu == '' || weishu % 2 != 0) {
			weishu = weishu - 1;
		}
		price = price.toString().substr(0, price.toString().indexOf('.')) + "." + weishu;
	}
	$('#price').val(price);
	if (amount > 0) {
		var am = parseInt(remainAmount * multiple * 10000 / price);
		amount = amount > am ? am : amount;
		if(amount<0){
			amount=0;
		}
		if (amount < 1) {
			$('#amount').val(0);
			return;
		}
		$('#amount').val(formatFloat(amount, 0));
		$('#baozhengjin').html((price * amount / (multiple * 10000)).toFixed(4));
	}
	
	return false;
}

function trade_market_click(type, price, amount) {
	$('#price').val(price);
	
	var selectedPercentage =  $(".percent.active");
	if (selectedPercentage.length > 0) {
		var percentageValue = selectedPercentage.html().replace("%","") / 100;		
		calculateNewMaxAmount();
		var maxAmount = Number($("#maxAmount").html());
		amount = (Number(maxAmount) * percentageValue).toString();		
	}

    $('#closeSellPrice').val(price);
	$(".price-buy-input").val(price);
	$(".price-sell-input").val(price);

	trade_amount(false, true);
}

function calculateNewMaxAmount(){
	var remainAmount = Number($('#myRemainBtc').val());		// 合约当前可用
	var price = Number($('#price').val());
	var multiple = Number($('#multiple').val());
	var amountDigits = Number($('#amountDigits').val());
	var coeffcient = Number($('#coeffcient').val());
	
	var maxAmount = 0;
	if (price > 0) {
		var feeRate = Number($('#myCommissionRate').val());						// 佣金（手续费）
		maxAmount = remainAmount / (price / multiple + price * feeRate) / coeffcient;	// 最大可开仓数量
		
		var maxStr = maxAmount + "";
		if (maxStr.indexOf('.') != -1 && maxStr.length - maxStr.indexOf('.') > amountDigits) {
			maxStr = maxStr.substring(0, maxStr.indexOf('.') + amountDigits + 1);
		}
		maxAmount = Number(maxStr);
		$('#maxAmount').html(maxAmount);
	} else {
		$('#maxAmount').html("0");
	}
}

function trade_price_market(type, price, amount) {
	trade_market_click(type, price, amount);
}

function refreshContractsTicker() {
	$.ajax({
        type: "POST",
        dataType: "json",         
        url: basePath + ajaxPath + "/tickersData.do",
        data: {token : getCookie("_token")},
        success: function(data) {
    		var tickerHtml = "";
    		for (var index = 0; index < data.length; index++) {
    			var map = data[index];
    			tickerHtml += "<tr class='tradesA' onclick=\"gotoTrade('" + map.tranCode  + "');\">"
    			+ "<td>" + map.tranCode.toUpperCase() + "</td>"
    			+ "<td>" + map.lastPrice + "</td>"
    			+ (map.lastPrice >= map.firstPrice ? "<td class='buy'>" : "<td class='sell'>") + map.fudu + "</td>"
    			+ "<td>" + formatNumWan(map.vol)  + "</td>"
    			+ "</tr>";
    		}
			$("#tickerTbody").html(tickerHtml);
        }
	});
}

////////////////////////////*full_market_order  */////////////////////////////
var marketInterval = null;
function trade_market_price() {
	trade_market_amount();
	if(marketInterval != null){//判断计时器是否为空
		clearInterval(marketInterval);
		marketInterval = null;
	};
	marketInterval = setInterval(trade_market_amount, 5000);
}

function multipleMarketClick(val, obj) {
	var activeMsg = $(obj).html();
	$(".multiple").removeClass("active");
	$(obj).addClass("active");
	$('#market_multiple').val(val);
	$("#multiple" + val).addClass("active");
	$('#multiple').val(val);
	$("#planMultiple" + val).addClass("active");
	$('#planMultiple').val(val);
	if($(obj).hasClass("active")) {
		$(".order-input-inner").val(activeMsg).css("border", "1px solid #4A5F78");
		$(".select-order-item").hide();
	}
	setMultiple(val);
	trade_market_amount();
}

function trade_market_amount(showAlert) {
	var marketMyRemainAmount = Number($('#market_myRemainBtc').val());		
	var marketRemainAmount = marketMyRemainAmount;							
	var marketAmount = Number($('#marketAmount').val());					
	var marketPrice = Number($('#lastPriceSpan').html());
	var marketMultiple = Number($('#market_multiple').val());
	var marketAmountDigits = Number($('#market_amountDigits').val());
	var marketCoeffcient = Number($('#market_coeffcient').val());
	
	var maxAmount = 0;
	if (marketPrice > 0) {
		var feeRate = Number($('#market_myCommissionRate').val());						// 佣金（手续费）
		maxAmount = marketRemainAmount / (marketPrice / marketMultiple + marketPrice * feeRate) / marketCoeffcient;	// 最大可开仓数量
		
		var maxStr = maxAmount + "";
		if (maxStr.indexOf('.') != -1 && maxStr.length - maxStr.indexOf('.') > marketAmountDigits) {
			maxStr = maxStr.substring(0, maxStr.indexOf('.') + marketAmountDigits + 1);
		}
		maxAmount = Number(maxStr);
		$('#maxMarketAmount').html(maxAmount);
		// $("#marketAmount").val(maxAmount);
	} else {
		$('#maxMarketAmount').html("0");
	}
	
	if (marketPrice == 0 || marketAmount == 0) {
		$('#baozhengjinMarket').html(0);
		return false;
	}

	if (marketAmount < Number($('#market_minKcNums').val())) {
		if (typeof(showAlert) != "undefined" ) {
			tipAlert(MSG['alertLitOpenNum'] + "：" + Number($('#market_minKcNums').val()) + MSG['alertGe']);
		}
		marketAmount = $('#market_minKcNums').val();
		return false;
	}

	// 开仓数大于最大可开仓数时,更改 开仓数
	if (marketAmount > maxAmount) {
		if (typeof(showAlert) != "undefined" ) {
			tipAlert(MSG['curtnotEnoughMoney'] + maxAmount + MSG['alertGe']);
		}
		$('#marketAmount').val(maxAmount);
		return false;
	} else {
		$('#marketAmount').val(formatFloat(marketAmount, marketAmountDigits));
	}
	
	$('#baozhengjinMarket').html((marketPrice * ($('#marketAmount').val() / marketMultiple) * marketCoeffcient).toFixed(4));
	
	return true;
}
/*刷新可开*/
function refreshMaxAmount(marketMyRemainAmount) {
	var marketMyRemainAmount = Number($('#market_myRemainBtc').val());
	var marketRemainAmount = marketMyRemainAmount;
	var marketPrice = Number($('#lastPriceSpan').html());
	var marketMultiple = Number($('#market_multiple').val());
	var marketAmountDigits = Number($('#market_amountDigits').val());
	var marketCoeffcient = Number($('#market_coeffcient').val());
	var maxAmount = 0;
	if (marketPrice > 0) {
		var feeRate = Number($('#market_myCommissionRate').val());						// 佣金（手续费）
		maxAmount = marketRemainAmount / (marketPrice / marketMultiple + marketPrice * feeRate) / marketCoeffcient;	// 最大可开仓数量

		var maxStr = maxAmount + "";
		if (maxStr.indexOf('.') != -1 && maxStr.length - maxStr.indexOf('.') > marketAmountDigits) {
			maxStr = maxStr.substring(0, maxStr.indexOf('.') + marketAmountDigits + 1);
		}
		maxAmount = Number(maxStr);
		$('#maxMarketAmount').html(maxAmount);
	} else {
		$('#maxMarketAmount').html("0");
	}
}

//////////////////////////// 右侧止盈止损操作框 //////////////////////////////

function tradeStopLossTabClick() {
	if ($('#userEmail').val() == null || $('#userEmail').val() == "") {
		return false;
	}
	
	var transaction_code = $('#transaction_code').val();
	var suspend = $('#suspend').val();
	$.ajax({
        type: "POST",
        dataType: "json",
        url: basePath + ajaxPath + "/memTransData.do",
        data: {"transactionCode":transaction_code, token : getCookie("_token")},
        error: function (err) {
    		
        },
        success: function(data) {

        	var stopLossPosHtml = "";
        	var buyMemTrans = eval('(' + data.buyMemberTransaction + ')');
			for (var index = 0; index < buyMemTrans.length; index++) {
				var tran = buyMemTrans[index];
				stopLossPosHtml += `<span class="option-multiples" onclick="mtClick(${tran.mt_id}, this, ${tran.averagePrice}, ${tran.qpPrice}, ${tran.trade_type_id});">${MSG['goLong']}x${tran.multiple}</span>`;
			}

			var sellMemTrans = eval('(' + data.sellMemberTransaction + ')');
			for (var index = 0; index < sellMemTrans.length; index++) {
				var tran = sellMemTrans[index];
				stopLossPosHtml += `<span class="option-multiples" onclick="mtClick(${tran.mt_id}, this, ${tran.averagePrice}, ${tran.qpPrice}, ${tran.trade_type_id});">${MSG['goShort']}x${tran.multiple}</span>`;
			}

			$("#selectOptionsBox").html(stopLossPosHtml);

			$(".option-multiples").each(function(index) {
				if(index == 0) {
					$(this).addClass("active").click();
					var activeMsg = $(this).html()
					$(".input-inner").val(activeMsg);
				}
			});
        }
    });
}
//点击止盈止损杠杆
function mtClick(mtid, obj, avgPrice, qpPrice, dirction) {
	$(obj).addClass("active").siblings().removeClass("active");
	var activeMsg = $(obj).html();
	if($(obj).hasClass("active")) {
		$(".input-inner").val(activeMsg).css("border", "1px solid #4A5F78");
		$(".select-options-item").hide();
	}

	$('#mt_mtid').val(mtid);
	$('#mt_direction').val(dirction);
	$('#mt_qp_price').val(Number(qpPrice));
	$('#mt_avg_price').val(Number(avgPrice));
	
	$.ajax({
		cache : false,
		async : false,
		dataType : 'JSON',
		type : 'POST',
		url : basePath + tradeAjaxPath + "/stopLossClick.do?t=" + new Date().getTime(),
		data : {
			transaction_member_id : mtid
		},
		success : function(data) {
			if (data && data.length > 0) {
				var order_type = data[0].trade_type; //后台拿到的trade_type--0是限价-1是市价
				if (order_type == 1) {
					$(".type-option-limit").removeClass('active');
					$(".type-option-market").addClass('active');
					$(".type-input-inner").val(fmtFullMarketOrder);
					$("#mt_pending_win_price").attr({"placeholder": bestPriceOrder, 'value': '', 'disabled' : 'disabled'}).css({'background-color':'#1E2939'}).removeAttr("onblur onkeyup").addClass('order-input-disabled').removeClass('order-input');
					$("#mt_pending_loss_price").attr({"placeholder": bestPriceOrder, 'value': '', 'disabled' : 'disabled'}).css({'background-color':'#1E2939'}).removeAttr("onblur onkeyup").addClass('order-input-disabled').removeClass('order-input');
					if(IsWhite) {
						$("#mt_pending_win_price").css({'background-color':'#f6fafd'});
						$("#mt_pending_loss_price").css({'background-color':'#f6fafd'});
					}
					$(".pending_loss_unit").html("");
				}
				$("#mt_OrderType").val(order_type);
				var array = data;
				insertStopLossData(array);
			}
		}
	});
}

function insertStopLossData(data) {
	var order_type = data[0].trade_type;
	var type = data[0].stop_loss_type_id; // 类型（1止盈 2止损）
	var stop_loss_price_zy = 0, stop_loss_price_zs = 0;
	var pending_trans_price_zy = 0, pending_trans_price_zs = 0;
	var pending_amount = data[0].amount;
	$("#mt_number_positions_amount").val(pending_amount);

	if (data == null || data.length == 0) { // 既无止赢又无止损
		$('#mt_has_win').prop("checked", false);
		$('#mt_has_loss').prop("checked", false);
		$('#mt_pending_win_price').val("");
		$('#mt_stop_win_price').val("");
		$('#mt_pending_loss_price').val("");
		$('#mt_stop_loss_price').val("");
		$('#mt_stop_win_price_span').val("");
		$('#mt_stop_loss_price_span').val("");
		$("#mt_number_positions_amount").val("");

	} else if (data.length == 1) { //止盈止损只有一个
		if (type == 1) { // 止盈
			stop_loss_price_zy = data[0].triPrice;
			pending_trans_price_zy = data[0].price;
			$('#mt_has_win').prop("checked", true);
			$('#mt_has_loss').prop("checked", false);
			$('#mt_stop_win_price').val(stop_loss_price_zy);//止盈-触发价格
			if (order_type != 1) {
				$('#mt_pending_win_price').val(pending_trans_price_zy).css('background-color','#0d1923');//止盈-委托价格
				if(IsWhite) {
					$('#mt_pending_win_price').val(pending_trans_price_zy).css('background-color','#FFFFFF');//止盈-委托价格
				}
				$(".type-input-inner").val(fmtFullLimitOrder);
			}
			// mt_check_pending_win_price();

		} else if (type == 2) { // 止损
			stop_loss_price_zs = data[0].triPrice;
			pending_trans_price_zs = data[0].price;
			$('#mt_has_win').prop("checked", false);
			$('#mt_has_loss').prop("checked", true);
			if (order_type != 1) {
				$('#mt_pending_loss_price').val(pending_trans_price_zs).css('background-color','#0d1923');//止损-委托价格
				if(IsWhite) {
					$('#mt_pending_loss_price').val(pending_trans_price_zs).css('background-color','#FFFFFF');//止损-委托价格
				}
				$(".type-input-inner").val(fmtFullLimitOrder);
			}
			$('#mt_stop_loss_price').val(stop_loss_price_zs);//止损-触发价格
			// $('#mt_pending_win_price').val("");
			// $('#mt_stop_win_price').val("");
			// $('#mt_stop_win_price_span').val("");
			// mt_check_pending_loss_price();

		}
	} else if(data.length == 2) { //既有止赢又有止损
		if (type == 1) { // 止盈
			stop_loss_price_zy = data[0].triPrice;
			stop_loss_price_zs = data[1].triPrice;
			pending_trans_price_zy = data[0].price;
			pending_trans_price_zs = data[1].price;
		} else if (type == 2) { // 止损
			stop_loss_price_zy = data[1].triPrice;
			stop_loss_price_zs = data[0].triPrice;
			pending_trans_price_zy = data[1].price;
			pending_trans_price_zs = data[0].price;
		}

		$('#mt_has_win').prop("checked", true);
		$('#mt_has_loss').prop("checked", true);
		$('#mt_stop_win_price').val(stop_loss_price_zy);
		$('#mt_stop_loss_price').val(stop_loss_price_zs);
		if (order_type != 1) {
			$('#mt_pending_win_price').val(pending_trans_price_zy);
			$('#mt_pending_loss_price').val(pending_trans_price_zs);
			$(".type-input-inner").val(fmtFullLimitOrder);
		}
		// mt_check_pending_win_price();
		// mt_check_pending_loss_price();
	}
}
//切换求数据
function triggerStopLossData(dir, tid, orderType) {
	if (dir == 1) {
		var buy_hold_count =  Number($('#buy_hold_count').val());
		if(buy_hold_count <=0){
			return false;
		}
	}else {
		var sell_hold_count =  Number($('#sell_hold_count').val());
		if(sell_hold_count <=0){
			return false;
		}
	}
	var digits = $('#myDigits').val();
	if (orderType == 1) {
		$("#mt_pending_win_price").attr({"placeholder": bestPriceOrder, 'value': '', 'disabled' : 'disabled'}).css({'background-color':'#1E2939'}).removeAttr("onblur onkeyup").addClass('order-input-disabled').removeClass('order-input');
		$("#mt_pending_loss_price").attr({"placeholder": bestPriceOrder, 'value': '', 'disabled' : 'disabled'}).css({'background-color':'#1E2939'}).removeAttr("onblur onkeyup").addClass('order-input-disabled').removeClass('order-input');
		if(IsWhite) {
			$("#mt_pending_win_price").css({'background-color':'#f6fafd'});
			$("#mt_pending_loss_price").css({'background-color':'#f6fafd'});
		}
		$(".type-input-inner").val(fmtFullMarketOrder);
		$(".pending_loss_unit").html("");
	} else {
		$("#mt_pending_win_price").attr({"placeholder": entruePrice,'disabled' : false, 'onblur': 'mt_check_pending_win_price()', 'onkeyup' : 'checkInputDigits(this, '+digits+')'}).css({'background-color':'#0D1824'}).removeClass('order-input-disabled').addClass('order-input');
		$("#mt_pending_loss_price").attr({"placeholder": entruePrice,'disabled' : false, 'onblur': 'mt_check_pending_loss_price()', 'onkeyup' : 'checkInputDigits(this, '+digits+')'}).css({'background-color':'#0D1824'}).removeClass('order-input-disabled').addClass('order-input');
		if(IsWhite) {
			$("#mt_pending_win_price").css({'background-color':'#FFFFFF'});
			$("#mt_pending_loss_price").css({'background-color':'#FFFFFF'});
		}
		$(".type-input-inner").val(fmtFullLimitOrder);
		$(".pending_loss_unit").html(tradePath =='vtrade' ? 'BFC' : (tradePath=='usdr' ? tradePath.toUpperCase() : 'USDT'));
	}
	$.ajax({
		cache: false,
		async: false,
		dataType: 'JSON',
		type: 'POST',
		url: basePath + tradeAjaxPath + "/stopLossClick.do?t=" + new Date().getTime(),
		data : {transaction_member_id : tid},
		success: function (data) {
			if (data && data.length > 0) {
				var order_type = data[0].trade_type; //后台拿到的trade_type--0是限价-1是市价
				if (orderType != order_type) {
					$("#mt_has_win").attr({'checked': false});
					$("#mt_has_loss").attr({'checked': false});
					$("#mt_stop_win_price").val("").attr({"placeholder": triPrice,'disabled' : false, 'onkeyup' : 'checkInputDigits(this, '+digits+')'}).css({'background-color':'#0D1824'});
					$("#mt_stop_loss_price").val("").attr({"placeholder": triPrice,'disabled' : false, 'onkeyup' : 'checkInputDigits(this, '+digits+')'}).css({'background-color':'#0D1824'});
					if(IsWhite) {
						$("#mt_stop_win_price").val("").attr({"placeholder": triPrice,'disabled' : false, 'onkeyup' : 'checkInputDigits(this, '+digits+')'}).css({'background-color':'#FFFFFF'});
						$("#mt_stop_loss_price").val("").attr({"placeholder": triPrice,'disabled' : false, 'onkeyup' : 'checkInputDigits(this, '+digits+')'}).css({'background-color':'#FFFFFF'});
					}
					$("#mt_number_positions_amount").val("");
				} else {
					var array = data;
					insertStopLossData(array);
				}
			}
		}
	});
}
//止盈-判断触发价格
function mt_check_pending_win_triPrice() {
	var tid = $('#mt_mtid').val();
	var orderType = $("#mt_OrderType").val();
	if (tid == '' || tid == null) {
		$('#mt_loss_error').html(fmtChosePosition);
		return false;
	}

	var dir = $('#mt_direction').val();
	var stop_win_price = Number($('#mt_stop_win_price').val());
	if(stop_win_price == 0 || stop_win_price == '') {
		$('#mt_win_error').html(MSG['inputTriPrice']);//请输入触发价！
		return false;
	}
	if(dir == 1) {
		if(stop_win_price <= buyAveragePrice) {
			$('#mt_win_error').html(entrustedPriceGtAveragePrice);//触发价必须大于持仓均价
			return false;
		}
	} else {
		if(stop_win_price >= sellAveragePrice) {
			$('#mt_win_error').html(entrustedPriceLtAveragePrice);//触发价必须小于持仓均价
			return false;
		}
	}
	$('#mt_win_error').html("");
	return true;
}

function mt_check_pending_win_price(){
	var tid = $('#mt_mtid').val();
	var orderType = $("#mt_OrderType").val();
	if (tid == '' || tid == null) {
		$('#mt_loss_error').html(fmtChosePosition);
		return false;
	}
	var dir = $('#mt_direction').val();
	var digits = $('#myDigits').val();
	var avg_price = Number($('#lastPriceSpan').html());
	var avg_price_only = Number($('#lastPriceSpanOnly').html());
	var stop_win_price = Number($('#mt_stop_win_price').val());
	var pending_win_price = Number($('#mt_pending_win_price').val());

	if(pending_win_price == 0 || pending_win_price == '') {
		if(orderType != 1) {
			$('#mt_win_error').html(MSG['inputComPrice']);//请输入委托价格！
			return false;
		}
	}
	if(dir == 1) {
		if(orderType != 1){
			if(pending_win_price <= buyAveragePrice) {
				$('#mt_win_error').html(multiplePriceGtAveragePrice);//多仓止盈价必须大于持仓均价
				return false;
			}
		}
	} else {
		if(orderType != 1){
			if(pending_win_price >= sellAveragePrice){
				$('#mt_win_error').html(shortPriceLtAveragePrice);//空仓止盈价必须小于持仓均价
				return false;
			}
		}
	}
	
	$('#mt_win_error').html("");
	return true;
}
//止损-判断触发价格
function mt_check_pending_loss_triPrice() {
	var tid = $('#mt_mtid').val();
	if (tid == '' || tid == null) {
		$('#mt_loss_error').html(fmtChosePosition);
		return false;
	}
	var dir = $('#mt_direction').val();
	var stop_loss_price = Number($('#mt_stop_loss_price').val());

	if(stop_loss_price == 0 || stop_loss_price == '') {
		$('#mt_loss_error').html(MSG['inputTriPrice']);//请输入触发价格！
		return false;
	}
	if(dir == 1) {
		if(stop_loss_price >= buyAveragePrice) {
			$('#mt_loss_error').html(entrustedPriceLtAveragePrice);//触发价必须小于持仓均价
			return false;
		}
	} else {
		if(stop_loss_price <= sellAveragePrice) {
			$('#mt_loss_error').html(entrustedPriceGtAveragePrice);//触发价必须大于持仓均价
			return false;
		}
	}
	$('#mt_loss_error').html("");
	return true;
}
function mt_check_pending_loss_price() {
	var tid = $('#mt_mtid').val();
	var orderType = $("#mt_OrderType").val();
	if (tid == '' || tid == null) {
		$('#mt_loss_error').html(fmtChosePosition);
		return false;
	}
	var dir = $('#mt_direction').val();
	var avg_price = Number($('#lastPriceSpan').html());
	var avg_price_only = Number($('#lastPriceSpanOnly').html());
	var stop_loss_price = Number($('#mt_stop_loss_price').val());
	var pending_loss_price = Number($('#mt_pending_loss_price').val());
	var qpPrice = Number($('#mt_qp_price').val());

	if(pending_loss_price == 0 || pending_loss_price == '') {
		if(orderType != 1) {
			$('#mt_loss_error').html(MSG['inputComPrice']);//请输入委托价格！
			return false;
		}
	}
	if(dir == 1) {
		if(orderType != 1) {
			/*要求止损价小于均价*/
			if(pending_loss_price >= buyAveragePrice) {
				$('#mt_loss_error').html(multiplePriceLtAveragePrice);//多仓止损价必须小于持仓均价
				return false;
			}
			/*要求止损价大于爆仓价*/
			if (pending_loss_price <= qpPrice) {
				$('#mt_loss_error').html(fmtMultigtStrong);//多仓止损价须大于强平价
				return false;
			}
		}
	} else {
		if(orderType != 1) {
			/*要求止损价大于均价*/
			if(pending_loss_price <= sellAveragePrice) {
				$('#mt_loss_error').html(shortPriceGtAveragePrice);//空仓止损价必须大于持仓均价价
				return false;
			}
			/*要求止损价小于爆仓价*/
			if(pending_loss_price >= qpPrice){
				$('#mt_loss_error').html(fmtEmptyStrong);//空仓止损价须小于强平价
				return false;
			}
		}
	}
	$('#mt_loss_error').html("");
	return true;
}

//左边止盈止损-验证平仓数量
function mt_check_number_positions_amount() {
	var tid = $('#mt_mtid').val();
	var dir = $('#mt_direction').val();
	var buyHoldCount = parseFloat($("#buyHoldCount" + tid).html());
	var sellHoldCount = parseFloat($("#sellHoldCount"+ tid).html());
	var numberPositionsAmount = Number($("#mt_number_positions_amount").val());
	if (dir == 1) {
		if(numberPositionsAmount == null || numberPositionsAmount == 0) {
			tipAlert(MSG['PositionNotEnputTips']);
			// $('#mt_amount_error').html(PositionNotEnputTips);
			return false;
		}
		if (numberPositionsAmount > buyHoldCount) {
			tipAlert(MSG['higherThanPositionTips']);
			// $('#mt_amount_error').html(higherThanPositionTips);
			return false;
		}
	} else {
		if(numberPositionsAmount == null || numberPositionsAmount == 0) {
			tipAlert(MSG['PositionNotEnputTips']);
			// $('#mt_amount_error').html(PositionNotEnputTips);
			return false;
		}
		if (numberPositionsAmount > sellHoldCount) {
			tipAlert(MSG['higherThanPositionTips']);
			// $('#mt_amount_error').html(higherThanPositionTips);
			return false;
		}
	}
	$('#mt_amount_error').html("");
	return true;
}

function mt_stopLossBtn(){
	var hasWin = "0";
	var hasLoss = "0";
	var dir = $('#mt_direction').val();
	var tid = $('#mt_mtid').val();
	var orderType = $("#mt_OrderType").val();
	if ($("#mt_has_win").attr("checked")) {
		hasWin = 1;
		var stop_win_price = Number($('#mt_stop_win_price').val());
		var pending_win_price = Number($('#mt_pending_win_price').val());

		if(orderType == 0) {
			if(stop_win_price <= 0 || pending_win_price <= 0){
				$('#mt_win_error').html(fmtFillProfit);
				return false;
			}
		} else {
			if(stop_win_price <= 0){
				$('#mt_win_error').html(triPrice);
				return false;
			}
		}
		
		if (!mt_check_pending_win_price()) {
			return false;
		}
	}
	
	$('#mt_win_error').html("");
	
	if ($("#mt_has_loss").attr("checked")) {
		hasLoss = 1;
		var stop_loss_price = Number($('#mt_stop_loss_price').val());
		var pending_loss_price = Number($('#mt_pending_loss_price').val());

		if(orderType == 0) {
			if(stop_loss_price == 0 || pending_loss_price == 0){
				$('#mt_loss_error').html(fmtFillLoss);
				return false;
			}
		} else {
			if(stop_loss_price == 0){
				$('#mt_loss_error').html(triPrice);
				return false;
			}
		}


		if (!mt_check_pending_loss_price()) {
			return false;
		}
	}
	
	$('#mt_loss_error').html("");
	
	$.ajax({
		cache: false,
        async: false, 
        dataType: 'JSON',
        type: 'POST',
        url: basePath + tradeAjaxPath +"/stopLossChange.do?t=" + new Date().getTime(),
		data : {
			transaction_member_id : tid, 
			has_win : hasWin, 
			has_loss : hasLoss,
        	stop_win_price : $("#mt_stop_win_price").val(),
        	pending_win_price : $("#mt_pending_win_price").val(),
        	stop_loss_price : $("#mt_stop_loss_price").val(), 
        	pending_loss_price : $("#mt_pending_loss_price").val(),
			pending_amount: $("#mt_number_positions_amount").val(),
			orderType: orderType,
        	token : getCookie("_token")
		},
		success: function (data) {
			 if(data.code == "success") {//success表示止盈止损开启
				$("#alertId_"+tid).removeClass("sprites-zszy-off");
				$("#alertId_"+tid).addClass("sprites-zszy-on");
				showMsgBox("success",fmtOptionStatusSuc);
				if (hasWin == 0) {
					$('#mt_pending_win_price').val("");
					$('#mt_stop_win_price').val("");
					$('#mt_stop_win_price_span').val("");
				}
				if (hasLoss == 0) {
					$('#mt_pending_loss_price').val("");
					$('#mt_stop_loss_price').val("");
					$('#mt_stop_loss_price_span').val("");
				}
			} else if (data.code == "success2") {//success2表示止盈止损关闭
				$("#alertId_"+tid).removeClass("sprites-zszy-on");
				$("#alertId_"+tid).addClass("sprites-zszy-off");
				
				showMsgBox("success",fmtOptionStatusSuc);
				if (hasWin == 0) {
					$('#mt_pending_win_price').val("");
					$('#mt_stop_win_price').val("");
					$('#mt_stop_win_price_span').val("");
					$("#mt_number_positions_amount").val("");
				}
				if (hasLoss == 0) {
					$('#mt_pending_loss_price').val("");
					$('#mt_stop_loss_price').val("");
					$('#mt_stop_loss_price_span').val("");
					$("#mt_number_positions_amount").val("");
				}
			} else if (data.code=="error") {
				if (data.msg.indexOf("止盈") >= 0) {
					$('#mt_win_error').html(""+data.msg);
					showMsgBox("error",fmtOptionStatusFail);
				} else {
					$('#mt_loss_error').html(""+data.msg);
					showMsgBox("error",fmtOptionStatusFail);
				}
			}
		}
	});
}

if(locale == 'en_US' || locale == 'ja_JP') {
	$(".control-form .full-talk").css("line-height","1em");
}
//右边持仓数量全部填充方法
function mt_allChangeAmount() {
	if ($('#userEmail').val() == null || $('#userEmail').val() == "") {
		tipAlert(MSG['opAfterLogin']);
		return false;
	}
	var dir = $('#mt_direction').val();
	var tid = $('#mt_mtid').val();
	var buyHoldCount = parseFloat($("#buyHoldCount" + tid).html());
	var sellHoldCount = parseFloat($("#sellHoldCount"+ tid).html());

	if (dir == 1) {
		$("#mt_number_positions_amount").val(buyHoldCount);
		if($('#mt_amount_error').html() != '') {
			$('#mt_amount_error').html("");
		}
	} else {
		$("#mt_number_positions_amount").val(sellHoldCount);
		if($('#mt_amount_error').html() != '') {
			$('#mt_amount_error').html("");
		}
	}
}

////////////////////////////trade manager panel //////////////////////////////
var currTab = 0;
function changeTab(type) {
	currTab = type;
}

function refreshTab() {
	if ($('#userEmail').val() == null || $('#userEmail').val() == "") {
		return;
	}
	
	if (currTab == 0) {
		refreshMemberTrans();
	} else if (currTab == 1) {
		refreshPending();
	} else if (currTab == 2) {
		refreshPlanEntrust();
	} else if (currTab == 3) {
		refreshHistoryPending();
	} else if (currTab == 4) {
		refreshTradeLog();
	}
	
	refreshAccount();
}
refershSellOne = setInterval(getSellOneUsable, 3000);
setInterval(refreshTab, 10000);
    
$(".dp-close-alert").click(function() {
	var duiping = $("#duiping-wrap"); 
	var dpzz = $("#tb-overlay-duiping");
	hide(duiping);
	dpzz.hide();
});

$(document).keyup(function(event){
	 switch(event.keyCode) {
	 	case 27: {
	 		$(".dp-close-alert").click();
	 	}
	 }
});

// 持仓管理tab
$('#tab-trade span').click(function() {
	$(this).addClass("current").siblings().removeClass();
	$(".trade-manage-main .trade-cont").hide().eq($('#tab-trade span').index(this)).show();
	$(".block-title-trade .trade-control").hide().eq($('#tab-trade span').index(this)).show();
});




////////////////////////////左侧持仓 止盈止损弹框 //////////////////////////////

//显示止盈止损弹框
function showFullProlitLoss(dir, tid) {
	var digits = $('#myDigits').val();
	var orderType = $("#stopOrderType").val();
	var dataText = `<div id="tb-overlay" class="tb-overlay"></div>
			<div class="alert-window alert-window-full">
				<i class="iconfont icon-guanbi" onclick="closeForm();"></i>
				<div class="triangle"></div>
				<form action="" method="post">
						<h2 class="control-title-style">${fmtTargetProfit} ${fmtStopLoss}</h2>
						<div class="type-btn-change">
							<span class="type-btn-limit active" onclick="typeBtnChange(this,0,${dir},${tid});">${fmtFullLimitOrder}</span>
							<span class="type-btn-market" onclick="typeBtnChange(this,1,${dir},${tid});">${fmtFullMarketOrder}</span>
						</div>
						<div class="control-form bd-bt">
						<div class="control-box">
							<div class="checkbox">
								<input type="checkbox" class="ipt" value="0" id="has_win">
								${fmtTargetProfit}
							</div>
								<p>
									<em>
										<input type="text" id="stop_win_price" class="form-control" value=""  placeholder="${triPrice}" onblur="check_pending_win_triPrice(${tid},${dir});" onkeyup="checkInputDigits(this, ${digits});">
									</em>
									<em>
										<input type="text" id="pending_win_price" class="form-control" value=""  placeholder="${entruePrice}" onblur="check_pending_win_price(${tid},${dir});" onkeyup="checkInputDigits(this, ${digits});">
									</em>
									<span id="win_error" class="error"></span>
								</p>
							</div>
						</div>
						<div class="halving-line"></div>
						<div class="control-form">
							<div class="control-box">
							<div class="checkbox">
								<input type="checkbox" class="ipt" value="0" id="has_loss">
								${fmtStopLoss}
							</div>
							<p>
								<em>
									<input type="text" id="stop_loss_price" class="form-control" value=""  placeholder="${triPrice}" onblur="check_pending_loss_triPrice(${tid},${dir});" onkeyup="checkInputDigits(this, ${digits});">
								</em>
								<em>
									<input type="text" id="pending_loss_price" class="form-control" value="" placeholder="${entruePrice}" onblur="check_pending_loss_price(${tid},${dir});" onkeyup="checkInputDigits(this, ${digits});">
								</em>
								<span id="loss_error" class="error"></span>
							</p>
						</div>
					</div>
					<div class="control-form">
						<div class="control-box">
							<div class="checkbox">${closevolumn}
								<p>
									<em class="relative">
										<input type="text" id="number_positions_amount" class="form-control" value="" placeholder="${closevolumn}" onblur="check_number_positions_amount(${tid},${dir});" onkeyup="checkInputDigits(this, ${digits})">
										<span class="exchange-all" onclick="allChangeAmount(${dir},${tid});">${exchangeAll}</span>
									</em>
									<span id="amount_error" style="font-weight: normal;line-height: 16px;" class="error"></span>
								</p>
							</div>
						</div>
					</div>
					<div class="btn-sub relative">
						<button type="button" onclick="stopLossBtn(${dir},${tid});" value="" class="btn sub-btn-sure true">${fmtLoginSure}</button>
					</div>
					<div class="talk">${targetProfitStopLossTips}</div>
				</form>
			</div>`;
	if (dir == 1) {
		$("#trade-alert_dt"+tid).html(dataText);
	}else {
		$("#trade-alert_kt"+tid).html(dataText);
	}
	firstFetchData(dir, tid);
}
/** 止盈止损相关 firstFetchData 初次获取止盈止损**/
function firstFetchData(dir, tid){
	if (dir == 1) {
		var buy_hold_count =  Number($('#buy_hold_count').val());
		if(buy_hold_count <=0){
			return false;
		}
	}else {
		var sell_hold_count =  Number($('#sell_hold_count').val());
		if(sell_hold_count <=0){
			return false;
		}
	}
	var digits = $('#myDigits').val();
	$.ajax({
		cache: false,
		async: false,
		dataType: 'JSON',
		type: 'POST',
		url: basePath + tradeAjaxPath + "/stopLossClick.do?t=" + new Date().getTime(),
		data : {transaction_member_id : tid},
		success: function (data) {
			if (data && data.length > 0) {
				var order_type = data[0].trade_type; //后台拿到的trade_type--0是限价-1是市价
				if (order_type == 1) {
					$(".type-btn-limit").removeClass('active').hide();
					$(".type-btn-market").addClass('active').show();
					$(".talk").html(targetProfitStopLossTips2);
					$("#pending_win_price").attr({"placeholder": bestPriceOrder, 'value': '', 'disabled' : 'disabled'}).css({'background-color':'#F9FCFF'}).removeAttr("onblur onkeyup");
					$("#pending_loss_price").attr({"placeholder": bestPriceOrder, 'value': '', 'disabled' : 'disabled'}).css({'background-color':'#F9FCFF'}).removeAttr("onblur onkeyup");

				} else if (order_type == 0){
					$(".type-btn-limit").addClass('active').show();
					$(".type-btn-market").removeClass('active').hide();
				} else {
					$(".type-btn-limit").removeClass('active').show();
					$(".type-btn-market").removeClass('active').show();
				}
				var array = data;
				insertData(array);
			}
		}
	});
}
/** 止盈止损弹框-限价委托&市价委托按钮切换* */
function typeBtnChange(self, type, dir, tid) {
	$(self).addClass("active").siblings().removeClass("active");
	var str = targetProfitStopLossTips;
	if(type == 1) {
		str = targetProfitStopLossTips2;
	}
	$(".talk").html(str);
	$("#orderType" + tid).val(type);
	triggerData(dir, tid, type);
}
/** 止盈止损相关 triggerData 切换限价&&市价获取数据 **/
function triggerData (dir, tid, orderType){
	if (dir == 1) {
		var buy_hold_count =  Number($('#buy_hold_count').val());
		if(buy_hold_count <=0){
			return false;
		}
	}else {
		var sell_hold_count =  Number($('#sell_hold_count').val());
		if(sell_hold_count <=0){
			return false;
		}
	}
	var digits = $('#myDigits').val();
	if (orderType == 1) {
		$("#pending_win_price").attr({"placeholder": bestPriceOrder, 'value': '', 'disabled' : 'disabled'}).css({'background-color':'#F9FCFF'}).removeAttr("onblur onkeyup");
		$("#pending_loss_price").attr({"placeholder": bestPriceOrder, 'value': '', 'disabled' : 'disabled'}).css({'background-color':'#F9FCFF'}).removeAttr("onblur onkeyup");
	} else {
		$("#pending_win_price").attr({"placeholder": entruePrice,'disabled' : false, 'onblur': 'check_pending_win_price('+tid+','+dir+')', 'onkeyup' : 'checkInputDigits(this, '+digits+')'}).css({'background-color':'#FFFFFF'});
		$("#pending_loss_price").attr({"placeholder": entruePrice,'disabled' : false, 'onblur': 'check_pending_loss_price('+tid+','+dir+')', 'onkeyup' : 'checkInputDigits(this, '+digits+')'}).css({'background-color':'#FFFFFF'});
	}
	$.ajax({
		cache: false,
		async: false,
		dataType: 'JSON',
		type: 'POST',
		url: basePath + tradeAjaxPath + "/stopLossClick.do?t=" + new Date().getTime(),
		data : {transaction_member_id : tid},
		success: function (data) {
			if (data && data.length > 0) {
				var order_type = data[0].trade_type; //后台拿到的trade_type--0是限价-1是市价
				if (orderType != order_type) {
					$("#has_win").val(0).attr({'checked': false});
					$("#has_loss").val(0).attr({'checked': false});
					$("#stop_win_price").val("").attr({"placeholder": triPrice,'disabled' : false, 'onkeyup' : 'checkInputDigits(this, '+digits+')'}).css({'background-color':'#FFFFFF'});
					$("#stop_loss_price").val("").attr({"placeholder": triPrice,'disabled' : false, 'onkeyup' : 'checkInputDigits(this, '+digits+')'}).css({'background-color':'#FFFFFF'});
					$("#number_positions_amount").val("");
				} else {
					var array = data;
					insertData(array);
				}
			}
		}
	});
}

function insertData(data){
	var order_type = data[0].trade_type;
	var type = data[0].stop_loss_type_id; // 类型（1止盈 2止损）
	var stop_loss_price_zy = 0, stop_loss_price_zs = 0;
	var pending_trans_price_zy = 0, pending_trans_price_zs = 0;
	var pending_amount = data[0].amount;
	$("#number_positions_amount").val(pending_amount);
	if (data.length == 1) { //止盈止损只有一个
		if (type == 1) { // 止盈
			stop_loss_price_zy = data[0].triPrice;
			pending_trans_price_zy = data[0].price;
			$("#has_win").val(1).attr({'checked': true});
			$("#has_loss").val(0);
			$("#stop_win_price").val(stop_loss_price_zy);//止盈-触发价格
			if (order_type != 1) {
				$("#pending_win_price").val(pending_trans_price_zy);//止盈-委托价格
			}
		} else if (type == 2) { // 止损
			stop_loss_price_zs = data[0].triPrice;
			pending_trans_price_zs = data[0].price;
			$("#has_win").val(0);
			$("#has_loss").val(1).attr({'checked': true});
			$("#stop_loss_price").val(stop_loss_price_zs);//止损-触发价格
			if (order_type != 1) {
				$("#pending_loss_price").val(pending_trans_price_zs);//止损-委托价格
			}
		}
	} else if(data.length == 2) { //既有止赢又有止损
		if (type == 1) { // 止盈
			stop_loss_price_zy = data[0].triPrice;
			stop_loss_price_zs = data[1].triPrice;
			pending_trans_price_zy = data[0].price;
			pending_trans_price_zs = data[1].price;
		} else if (type == 2) { // 止损
			stop_loss_price_zy = data[1].triPrice;
			stop_loss_price_zs = data[0].triPrice;
			pending_trans_price_zy = data[1].price;
			pending_trans_price_zs = data[0].price;
		}
		$("#has_win").val(1).attr({'checked': true});
		$("#has_loss").val(1).attr({'checked': true});
		$("#stop_win_price").val(stop_loss_price_zy);//止盈-触发价格
		$("#stop_loss_price").val(stop_loss_price_zs);//止损-触发价格
		if (order_type != 1) {
			$("#pending_win_price").val(pending_trans_price_zy);//止盈-委托价格
			$("#pending_loss_price").val(pending_trans_price_zs);//止损-委托价格
		}
	}
}

function closeForm(){
	$(".zyzs-alert-window ").empty();
}

function check_pending_win_triPrice(tid, dir){
	var stop_win_price = Number($('#stop_win_price').val());
	if(stop_win_price == 0 || stop_win_price == '') {
		$('#win_error').html(MSG['inputTriPrice']);//请输入触发价！
		return false;
	}
	if(dir == 1) {
		if(stop_win_price <= buyAveragePrice) {
			$('#win_error').html(entrustedPriceGtAveragePrice);//触发价必须大于持仓均价
			return false;
		}
	} else {
		if(stop_win_price >= sellAveragePrice){
			$('#win_error').html(entrustedPriceLtAveragePrice);//触发价必须小于持仓均价
			return false;
		}
	}
	$('#win_error').html("");
	return true;
}
function check_pending_win_price(tid, dir){
	var digits = $('#myDigits').val();
	var orderType = $("#orderType" + tid).val();
	var avg_price = Number($('#lastPriceSpan').html());
	var avg_price_only = Number($('#lastPriceSpanOnly').html());
	var stop_win_price = Number($('#stop_win_price').val());
	var pending_win_price = Number($('#pending_win_price').val());

	if(pending_win_price == 0 || pending_win_price == '') {
		if(orderType != 1) {
			$('#win_error').html(MSG['inputComPrice']);//请输入委托价格！
			return false;
		}
	}
	if(dir == 1) {
		if(orderType != 1) {
			if(pending_win_price <= buyAveragePrice) {
				$('#win_error').html(multiplePriceGtAveragePrice);//多仓止盈价必须大于持仓均价
				return false;
			}
		}
	} else {
		if(orderType != 1) {
			if(pending_win_price >= sellAveragePrice){
				$('#win_error').html(shortPriceLtAveragePrice);//空仓止盈价必须小于持仓均价
				return false;
			}
		}
	}
	$('#win_error').html("");
	return true;
}
function check_pending_loss_triPrice(tid, dir) {
	var stop_loss_price = Number($('#stop_loss_price').val());
	if(stop_loss_price == 0 || stop_loss_price == '') {
		$('#loss_error').html(MSG['inputTriPrice']);//请输入触发价！
		return false;
	}
	if(dir == 1) {
		if(stop_loss_price >= buyAveragePrice) {
			$('#loss_error').html(entrustedPriceLtAveragePrice);//触发价必须小于持仓均价
			return false;
		}
	} else {
		if(stop_loss_price <= sellAveragePrice) {
			$('#loss_error').html(entrustedPriceGtAveragePrice);//触发价必须大于持仓均价
			return false;
		}
	}
	$('#loss_error').html("");
	return true;
}
function check_pending_loss_price(tid, dir) {
	var orderType = $("#orderType" + tid).val();
	var avg_price = Number($('#lastPriceSpan').html());
	var avg_price_only = Number($('#lastPriceSpanOnly').html());
	var stop_loss_price = Number($('#stop_loss_price').val());
	var pending_loss_price = Number($('#pending_loss_price').val());

	if(pending_loss_price == 0 || pending_loss_price == '') {
		if(orderType != 1) {
			$('#loss_error').html(MSG['inputComPrice']);//请输入委托价格！
			return false;
		}
	}
	if(dir == 1) {
		var qpPrice = Number($('#buy_qpj'+tid).val());
		if(orderType != 1) {
			/*要求止损价小于均价*/
			if(pending_loss_price >= buyAveragePrice) {
				$('#loss_error').html(multiplePriceLtAveragePrice);//多仓止损价必须小于持仓均价
				return false;
			}
			/*要求止损价大于爆仓价*/
			if (pending_loss_price <= qpPrice) {
				$('#loss_error').html(fmtMultigtStrong);//多仓止损价须大于强平价
				return false;
			}
		}
	} else {
		var qpPrice = Number($('#sell_qpj'+tid).val());
		if(orderType != 1) {
			/*要求止损价大于均价*/
			if(pending_loss_price <= sellAveragePrice) {
				$('#loss_error').html(shortPriceGtAveragePrice);//空仓止损价必须大于持仓均价价
				return false;
			}
			/*要求止损价小于爆仓价*/
			if(pending_loss_price >= qpPrice){
				$('#loss_error').html(fmtEmptyStrong);//空仓止损价须小于强平价
				return false;
			}
		}
	}
	$('#loss_error').html("");
	return true;
}

/*
* 止盈止损-验证平仓数量
* dir:持仓类型，1是buy，2是sell
* tid：持仓ID，唯一
* */
function check_number_positions_amount(tid, dir) {
	var buyHoldCount = parseFloat($("#buyHoldCount" + tid).html());
	var sellHoldCount = parseFloat($("#sellHoldCount"+ tid).html());
	var numberPositionsAmount = Number($("#number_positions_amount").val());
	if (dir == 1) {
		if(numberPositionsAmount == null || numberPositionsAmount == 0) {
			$('#amount_error').html(PositionNotEnputTips);
			return false;
		}
		if (numberPositionsAmount > buyHoldCount) {
			$('#amount_error').html(higherThanPositionTips);
			return false;
		}
	} else {
		if(numberPositionsAmount == null || numberPositionsAmount == 0) {
			$('#amount_error').html(PositionNotEnputTips);
			return false;
		}
		if (numberPositionsAmount > sellHoldCount) {
			$('#amount_error').html(higherThanPositionTips);
			return false;
		}
	}
	$('#amount_error').html("");
	return true;
}
/*
* 平仓数量全部划转
* dir:持仓类型，1是buy，2是sell
* tid：持仓ID，唯一
* flag：1左边弹框，2右边操作框
* */
function allChangeAmount(dir, tid) {
	var buyHoldCount = parseFloat($("#buyHoldCount" + tid).html());
	var sellHoldCount = parseFloat($("#sellHoldCount"+ tid).html());
	if (dir == 1) {
		$("#number_positions_amount").val(buyHoldCount);
		if($('#amount_error').html() != '') {
			$('#amount_error').html("");
		}
	} else {
		$("#number_positions_amount").val(sellHoldCount);
		if($('#amount_error').html() != '') {
			$('#amount_error').html("");
		}
	}
}

//止盈止損弹框确定方法
function stopLossBtn(dir, tid){
	var orderType = $("#orderType" + tid).val();
	var hasWin = "0";
	var hasLoss = "0";

	if ($("#has_win").attr("checked")) {
		hasWin = 1;
		var stop_win_price = Number($('#stop_win_price').val());
		var pending_win_price = Number($('#pending_win_price').val());
		if(orderType == 0) {
			if(stop_win_price <= 0 || pending_win_price <= 0){
				$('#win_error').html(fmtFillProfit);
				return false;
			}
		} else {
			if(stop_win_price <= 0){
				$('#win_error').html(triPrice);
				return false;
			}
		}


		if (!check_pending_win_price(tid, dir)) {
			return false;
		}
	}

	$('#win_error').html("");

	if ($("#has_loss").attr("checked")) {
		hasLoss = 1;
		var stop_loss_price = Number($('#stop_loss_price').val());
		var pending_loss_price = Number($('#pending_loss_price').val());
		if(orderType == 0) {
			if(stop_loss_price == 0 || pending_loss_price == 0){
				$('#loss_error').html(fmtFillLoss);
				return false;
			}
		} else {
			if(stop_loss_price == 0){
				$('#loss_error').html(triPrice);
				return false;
			}
		}


		if (!check_pending_loss_price(tid, dir)) {
			return false;
		}
	}

	$('#loss_error').html("");
	$.ajax({
		cache: false,
		async: false,
		dataType: 'JSON',
		type: 'POST',
		url: basePath + tradeAjaxPath +"/stopLossChange.do?t=" + new Date().getTime(),
		data : {
			transaction_member_id : tid,
			has_win : hasWin,
			has_loss : hasLoss,
			stop_win_price : $("#stop_win_price").val(),
			pending_win_price : $("#pending_win_price").val(),
			stop_loss_price : $("#stop_loss_price").val(),
			pending_loss_price : $("#pending_loss_price").val(),
			pending_amount: $("#number_positions_amount").val(),
			orderType: orderType,
			token : getCookie("_token")
		},
		success: function (data) {
			if(data.code == "success") {
				$("#alertId_"+tid).removeClass("sprites-zszy-off");
				$("#alertId_"+tid).addClass("sprites-zszy-on");
				closeForm();
			} else if (data.code == "success2") {
				$("#alertId_"+tid).removeClass("sprites-zszy-on");
				$("#alertId_"+tid).addClass("sprites-zszy-off");
				closeForm();
			} else if (data.code=="error") {
				if (data.msg.indexOf("止盈") >= 0) {
					$('#win_error').html(""+data.msg);
				} else {
					$('#loss_error').html(""+data.msg);
				}
			}
		}
	});
}



//////////////////////////// full trade //////////////////////////////

$(".tab-close-pos span").click(function() {
	$(".tab-close-pos span").removeClass("current");
	$(this).addClass("current");
	
	if($(this).html() == fmtLimitedBuyMore) {
		$("#close-sell-position").show();
		$("#close-market-position").hide();
	} else if($(this).html() == fmtFullMarketClose) {
		$("#close-sell-position").hide();
		$("#close-market-position").show();
	}
});

$(".tab-stop-loss span").click(function() {
	$(".tab-stop-loss span").removeClass("current");
	$(this).addClass("current");
	
	if($(this).html() == fmtFullOpen) {
		//开仓
		$("#full-order").show();
		$("#full-stop-loss").hide();
		$("#full-market-order").hide();
		$("#full-plan-entrust").hide();
		$("#close-sell-position").hide();
		$("#close-market-position").hide();
		var sessionDelegateType = window.localStorage.getItem("delegateType");
		if(sessionDelegateType == fmtFullLimitOrder) {
			$("#full-order").show();
			$("#full-stop-loss").hide();
			$("#full-market-order").hide();
			$("#full-plan-entrust").hide();
			$("#close-sell-position").hide();
			$("#close-market-position").hide();
		} else if(sessionDelegateType == fmtFullPlanEntrust) {
			$("#full-plan-entrust").show();
			$("#full-order").hide();
			$("#full-market-order").hide();
			$("#full-stop-loss").hide();
			$("#close-sell-position").hide();
			$("#close-market-position").hide();
		} else if(sessionDelegateType == fmtFullMarketOrder) {
			$("#full-market-order").show();
			$("#full-order").hide();
			$("#full-plan-entrust").hide();
			$("#close-sell-position").hide();
			$("#full-stop-loss").hide();
			$("#close-market-position").hide();
		}
	} else if($(this).html() == fmtFullProlitLoss) {
		//止盈/止损
		$("#full-stop-loss").show();
		$("#full-order").hide();
		$("#full-market-order").hide();
		$("#full-plan-entrust").hide();
		$("#close-sell-position").hide();
		$("#close-market-position").hide();
		tradeStopLossTabClick();
	} else if($(this).html() == fmtClosePosition){
		//平仓
		$("#close-sell-position").show();
		$("#full-order").hide();
		$("#full-market-order").hide();
		$("#full-plan-entrust").hide();
		$("#full-stop-loss").hide();
		$("#close-market-position").hide();
		refreshMemberTrans();
	}
});

function setDelegateType(delegateType) {
	window.localStorage.setItem("delegateType", delegateType);
}

$(".sub-entrust-btn-buy").click(function() {
	if($(this).html() == fmtLimitedBuyMore) {
		$("#close-sell-position").show();
		$("#close-market-position").hide();
		$("#full-order").hide();
		$("#full-stop-loss").hide();
		$("#full-plan-entrust").hide();
	} else if($(this).html() == fmtFullMarketClose){
		$("#close-market-position").show();
		$("#close-sell-position").hide();
		$("#full-plan-entrust").hide();
		$("#full-order").hide();
		$("#full-stop-loss").hide();
	}
});

$("#tapeReading").click (function() {
	$(this).addClass("current").siblings().removeClass("current");
	$(".item_data .trade-history-Content").css('display', 'none');
	$(".item_data .trade-tape-Content").css('display', 'block');
});

$("#history").click(function() {
	$(this).addClass("current").siblings().removeClass("current");
	$(".item_data .trade-history-Content").css('display', 'block');
	$(".item_data .trade-tape-Content").css('display', 'none');
});

//屏幕大小变大
$(window).resize(function () {
	if($(window).width() > 1600){
		$("#tapeReading").addClass("current").siblings().removeClass("current");
		$(".item_data .trade-tape-Content").css('display', 'block');
		$(".item_data .trade-history-Content").css('display', 'none');
	}
});

function transIntroduce() { 
	var bqCode = $('#transaction_code').val();
	var type = $('#tradePath').val();
	var tradePath = $('#tradePath').val();
	if (tradePath == "vtrade") {
		type = "vcontract";
	} else if (tradePath == "dtrade") {
		type = "delivery";
	} else if (tradePath == "trade") {
		type = "contract";
	}
	var url = basePath + "futuresApi/introduce.do?symbol=" + bqCode + "&type=" + type;
	$.ajax( {
		type : 'GET',
		url : url,
		data : {},
		cache : false,
		statusCode : {
			404 : function() {// 404错误
			}
		},
		success : function(data) {
			var data = eval('(' + data + ')');
			if (data.code == 0) {
				var content = data.data.zh_introduce;
				if (locale == "en_US") {
					content = data.data.en_introduce;
				} else if (locale == "ja_JP") {
					content = data.data.ja_introduce;
				} else if (locale == "ko_KR") {
					content = data.data.ko_introduce;
				} else if (locale == "zh_HK") {
					content = data.data.hk_introduce;
				}
			    
				$("#introduce-content").html(content);
				if($(".right-cont-top-item-alone").css("display") == "block") {
					$("#introduce-content-only").html(content);
				}
			};
		},
		error : function(msg) {
			
		}
	});
}

function gotoTrade(code) {
	document.location.href = 'trade.do?transactionCode=' + code;
}

function changeTo(locale) {
	$.get(basePath + "home/setLocale.do?locale=" + locale, function(data) {
		window.location.reload(true);
	});
}

/* 计划委托  */
function refreshPlanEntrust() {
	if ($('#userEmail').val() == null || $('#userEmail').val() == "") {
		return;
	}
	
	var transaction_code = $('#transaction_code').val();
	$.ajax({
		type: "POST",
		dataType: "json",
		url: basePath + ajaxPath + "/planPendingData.do",
		data: {"transactionCode":transaction_code, token : getCookie("_token")},
		success: function(data) {
			var planHtml = "";
			if(data.size != 0) {
				$("#planEntrustSize").html(' (' + data.size + ')');
			} else {
				$("#planEntrustSize").html('');
			}
			var data = data.data;
			if (data.length == 0) {
				planHtml = "<tr><td colspan='12'>" + MSG['noEntrustment'] + "</td></tr>";
				showsliderBoxPlanEntrust = true;
			} else {
				for (var index = 0; index < data.length; index++) {
					var pd = data[index];
					var pdType = "";
					var triPrice = "";
					var status = "";
					var cancel = "";
					var price = "";
					if ((pd.trade_type == 0 || pd.trade_type == null) && pd.direction == 1) {
						pdType = "<b class='buy'>" + MSG['olp'] + "</b>";/*olp: "多单开仓",*/
					} else if ((pd.trade_type == 0 || pd.trade_type == null) && pd.direction == 2) {
						pdType = "<b class='sell'>" + MSG['osp'] + "</b>";/*osp: "空单开仓",*/
					} else if (pd.trade_type == 1 && pd.direction == 1 && (pd.stop_loss_type_id <= 0 || pd.stop_loss_type_id == null)) {
						pdType = "<b class='sell'>" + MSG['csp'] + "</b>";/*clp: "空单平仓",*/
					} else if (pd.trade_type == 1 && pd.direction == 2 && (pd.stop_loss_type_id <= 0 || pd.stop_loss_type_id == null)) {
						pdType = "<b class='buy'>" + MSG['clp'] + "</b>";/*csp: "多单平仓",*/
					} else if (pd.trade_type == 1 && pd.direction == 1 && pd.stop_loss_type_id == 1) {
						pdType = "<b class='sell'>" + MSG['sellStopProfit'] + "</b>";/*buyStopProfit: "空单止盈",*/
					} else if (pd.trade_type == 1 && pd.direction == 2 && pd.stop_loss_type_id == 1) {
						pdType = "<b class='buy'>" + MSG['buyStopProfit'] + "</b>";/*sellStopProfit: "多单止盈",*/
					} else if (pd.trade_type == 1 && pd.direction == 1 && pd.stop_loss_type_id == 2) {
						pdType = "<b class='sell'>" + MSG['sellStopLoss'] + "</b>";/*buyStopLoss: "空单止损",*/
					} else if (pd.trade_type == 1 && pd.direction == 2 && pd.stop_loss_type_id == 2) {
						pdType = "<b class='buy'>" + MSG['buyStopLoss'] + "</b>";/*sellStopLoss: "多单止损",*/
					}

					if (pd.status == 0) {
						status = MSG['invalid'];
					} else if (pd.status == 1) {
						status = MSG['executed'];
					} else if (pd.status == 2) {
						status =  MSG['becameInvalid'];
					} else if (pd.status == 3) {
						status = MSG['allreadyCancel'];
					}

					if (pd.type == 1) {
						triPrice = MSG['markPrice'] + "≤" + parseFloat(pd.triPrice);
					} else if (pd.type == 2) {
						triPrice = MSG['markPrice'] + "≥" + parseFloat(pd.triPrice);
					}

					if (pd.status == 0) {
						cancel = "<a href='javascript:void(0);' class='btn btn-blue' "
							+ " onclick=\"cancelPlanDealAnsyc('" + pd.delegation_id + "','" + pd.symbol + "');\">" + MSG['cancel'] + "</a>"
					} else {
						cancel = ""
					}

					if(pd.price == 0) {
						price = MSG['planMarketPrice'];
					} else {
						price = parseFloat(pd.price)
					}
					planHtml += "<tr>"
						+ "<td>" + dateFmt(pd.createTime * 1000) + "</td>"
						+ "<td>" + pd.symbol.toUpperCase() + "</td>"
						+ "<td>" + pdType + "</td>"
						+ "<td>" + pd.multiple + "</td>"
						+ "<td>" + parseFloat(pd.amount) + "</td>"
						+ "<td>" + triPrice + "</td>"
						+ "<td>" + price + "</td>"
						+ "<td>" + status + "</td>"
						+ "<td>" + cancel + "</td>"
						+ "</tr>";
				}
			}
			$("#planEntrustTbody").html(planHtml);
			if (status == MSG['invalid']) {
			  showsliderBoxPlanEntrust = false;
			}
		}
	});
}
/**
 * 计划委托撤单
 * 
 * @return
 */
function cancelPlanDeal(direction, code) {
	$.get(basePath + tradeAjaxPath + "/canclePlanDeal.do?t=" + new Date().getTime()+"&token="+getCookie("_token"), {
		direction : direction,
		code : code
	}, function(data) {
		refreshPlanEntrust();
		refreshMultiple();
	});

}

function cancelPlanDealAnsyc(id, code) {	
	$.get(basePath + tradeAjaxPath + "/canclePlanPendingAnsyc.do?t=" + new Date().getTime()+"&token="+getCookie("_token"), {
		pdId : id,
		code : code,
	}, function(data) {
		refreshPlanEntrust();
		refreshMultiple();
	});
}

/**
 * 买多操作
 * 
 * @param sact
 *            URL
 * @param cointype
 *            类型（现在只有BTC）
 * @param direction
 *            交易方向（1、买；2、卖）
 * @return
 */
function del_sure_Plantrade(sact, cointype, direction) {
	var orderType = $("#planOrder").val();
	if ($('#userEmail').val() == null || $('#userEmail').val() == "") {
		tipAlert(MSG['opAfterLogin']);
		return false;
	}

	var multiple = Number($('#planMultiple').val()); // 杠杆
	if (multiple == 0) {
		tipAlert(MSG['alertLeverage']);
		return false;
	}

	var amount = Number($('#planAmount').val()); //数量
	if (isNaN(amount) || amount <= 0) {
		tipAlert(MSG['alertLegalNum']);
		return false;
	}
	var price = Number($('#planPrice').val());	//委托价格
	if(orderType == 0){
		if (isNaN(price) || price <= 0) {
			tipAlert(MSG['alertLegalPrice']);
			return false;
		}
	}
	var triPrice = Number($('#planTriPrice').val());	// 触发价格
	if (isNaN(triPrice) || triPrice <= 0) {
		tipAlert(MSG['alertTriPrice']);
		return false;
	}
	var myRemainAmount = Number($('#planMyRemainBtc').val());		// 合约当前可用
	var remainAmount = myRemainAmount;							// 合约当前可用
	var amount = Number($('#planAmount').val());					// 开仓数量
	var price = Number($('#planPrice').val());
	var multiple = Number($('#planMultiple').val());
	var amountDigits = Number($('#planAmountDigits').val());
	var coeffcient = Number($('#planCoeffcient').val());
	var marketPrice = Number($('#lastPriceSpan').html());//最新价

	var feeRate = Number($('#myCommissionRates').val());						// 佣金（手续费）
	var planMaxAmount = remainAmount / (price / multiple + price * feeRate) / coeffcient;	// 最大可开仓数量
	if(orderType == 0){
		var planMaxAmount = remainAmount / (price / multiple + price * feeRate) / coeffcient;	// 最大可开仓数量
	} else {
		var planMaxAmount = remainAmount / (marketPrice / marketPrice + price * feeRate) / coeffcient;	// 最大可开仓数量
	}
	var maxStr = planMaxAmount + "";
	if (maxStr.indexOf('.') != -1 && maxStr.length - maxStr.indexOf('.') > amountDigits) {
		maxStr = maxStr.substring(0, maxStr.indexOf('.') + amountDigits + 1);
	}
	planMaxAmount = Number(planMaxAmount);
	$('#planMaxAmount').html(planMaxAmount);
	if (amount > planMaxAmount) {
		$('#planAmount').val(planMaxAmount);
		return false;
	}
	if(orderType == 0){
		$('#cash_deposit').html((price * ($('#planAmount').val() / multiple) * coeffcient).toFixed(4));
	} else {
		$('#cash_deposit').html((marketPrice * ($('#planAmount').val() / multiple) * coeffcient).toFixed(4));
	}

	
	var bol_tijiao = true;
	if (bol_tijiao) {
		$('#planDirection').val(direction);
		tradePlantj(sact); // 提交给后台
	}
}

function tradePlantj(sact) {
	$('.alert-money').css("display", "none");
	var direction = $("#planDirection").val();
	if (direction == 1) {
		$("#btnBtcPlan").html(MSG['submiting']);
	} else {
		$("#sellBtcPlan").html(MSG['submiting']);
	}
	$.ajax( {
		url : sact,
		type : "POST",
		data : $('#topPlanSearchForm').serialize(),
		dataType : "JSON",
		cache : false,
		statusCode : {
			404 : function() {
				location.reload();
			}
		},
		success : function(data) {
			if (direction == 1) {
				$("#btnBtcPlan").html(MSG['buylong']);
			} else {
				$("#sellBtcPlan").html(MSG['shortsell']);
			}
			showMsgBox(data.code, data.msg);
			showSuccInfo_kc(data.msg, data.code);
			if (data.code == 'success') {
				$("#planAmount").val("");
				$("#planPrice").val("");
				$("#planTriPrice").val("");
				$("#planPayPwd").val("");
				$("#cash_deposit").html("0");
				$('#planMaxAmount').html("0");
				refreshMultiple();
				refreshPlanEntrust();
				interimOrder();
				setTimeout("removeInterimOrder()", 3000);
			}
		},
		error : function(XMLHttpRequest, textStatus, errorThrown) {
			// msgAlert("系统繁忙，请稍候再试！", "系统提示", false, callbackreload, "");
			if (direction == 1) {
				$("#btnBtcPlan").html(MSG['buylong']);
			} else {
				$("#sellBtcPlan").html(MSG['shortsell']);
			}
		}
	});
}
/**
 * 平仓计划委托提交
 *
 * @param sact
 *            URL
 * @param cointype
 *            类型（例如BTC）
 * @param direction
 *            交易方向（1、买；2、卖）
 * @return
 */
function del_sure_closePlantrade(code, direction) {
	var closeOrderType = $("#closePlanOrder").val();
	if ($('#userEmail').val() == null || $('#userEmail').val() == "") {
		tipAlert(MSG['opAfterLogin']);
		return false;
	}
	var multiple = Number($('#closePlanMultiple').val()); // 杠杆
	if (multiple == 0) {
		tipAlert(MSG['alertLeverage']);
		return false;
	}

	var amount = Number($('#closePlanAmount').val()); //数量
	if (isNaN(amount) || amount <= 0) {
		tipAlert(MSG['alertLegalNum']);
		return false;
	}
	var price = Number($('#closePlanPrice').val());	//委托价格
	if(closeOrderType == 0) {
		if (isNaN(price) || price <= 0) {
			tipAlert(MSG['alertLegalPrice']);
			return false;
		}
	}

	var triPrice = Number($('#closePlanTriPrice').val());	// 触发价格
	if (isNaN(triPrice) || triPrice <= 0) {
		tipAlert(MSG['alertTriPrice']);
		return false;
	}

	$('#closePlanDirection').val(direction);
	tradeClosePlantj(code, multiple, amount, price, triPrice, direction); // 提交给后台
}

function tradeClosePlantj(code, multiple, amount, price, triPrice, direction) {
	var closeOrderType = $("#closePlanOrder").val();
	var id = null;
	if (direction == 1) {
		$("#closePlanBuy").html(MSG['submiting']);
		id = $("#sellCloseMId").val();
	} else {
		$("#closePlanSell").html(MSG['submiting']);
		id = $("#buyCloseMId").val();
	}
	$.ajax( {
		type : "POST",
		dataType : "JSON",
		url: basePath + tradeAjaxPath + "/orderPlanPendingPC.do",
		data:  {
			planPrice : price,//委托价格
			planTriPrice: triPrice,//触发价格
			amount : amount,
			planCode : code,
			planMultiple: multiple,
			transactionMemberId : id,
			planDirection: direction,
			planOrderType: closeOrderType,
			token : getCookie("_token"),
		},
		success : function(data) {
			if (direction == 1) {
				$("#closePlanBuy").html(MSG['fulloBuyMore']);
			} else {
				$("#closePlanSell").html(MSG['fulloSellMore']);
			}
			showMsgBox(data.code, data.msg);
			showSuccInfo_kc(data.msg, data.code);
			if (data.code == 'success') {
				$("#closePlanAmount").val("");
				$("#closePlanPrice").val("");
				$("#closePlanTriPrice").val("");
				$("#closePlanPayPwd").val("");
				refreshMultiple();
				refreshPlanEntrust();
				interimOrder();
				setTimeout("removeInterimOrder()", 3000);
			}
		},
		error : function(XMLHttpRequest, textStatus, errorThrown) {
			// msgAlert("系统繁忙，请稍候再试！", "系统提示", false, callbackreload, "");
			if (direction == 1) {
				$("#closePlanBuy").html(MSG['fulloBuyMore']);
			} else {
				$("#closePlanSell").html(MSG['fulloSellMore']);
			}
		}
	});
}
function multiplePlanClick(val, obj) {
	var activeMsg = $(obj).html();
	$(".multiple").removeClass("active");
	$(obj).addClass("active");
	$('#planMultiple').val(val);
	$("#multiple" + val).addClass("active");
	$('#multiple').val(val);
	$("#market_multiple" + val).addClass("active");
	$('#market_multiple').val(val);
	var amount = Number($('#planAmount').val());
	if (amount != 0) {
		$('#cash_deposit').html((amount / val).toFixed(4));
	}
	if($(obj).hasClass("active")) {
		$(".order-input-inner").val(activeMsg).css("border", "1px solid #4A5F78");
		$(".select-order-item").hide();
	}
	setMultiple(val);
}

function trade_plan_amount(showAlert) {
	var orderType = $("#planOrder").val();
	var marketPrice = Number($('#lastPriceSpan').html());//最新价
	var myRemainAmount = Number($('#planMyRemainBtc').val());		// 合约当前可用
	var remainAmount = myRemainAmount;							// 合约当前可用
	var amount = Number($('#planAmount').val());					// 开仓数量
	var price = Number($('#planPrice').val());
	var multiple = Number($('#planMultiple').val());
	var amountDigits = Number($('#planAmountDigits').val());
	var coeffcient = Number($('#planCoeffcient').val());
	
	var planMaxAmount = 0;
	var feeRate = Number($('#myCommissionRates').val());						// 佣金（手续费）
	if(orderType == 1) {
		if (marketPrice > 0) {
			planMaxAmount = remainAmount / (marketPrice / multiple + marketPrice * feeRate) / coeffcient;	// 最大可开仓数量

			var maxStr = planMaxAmount + "";
			if (maxStr.indexOf('.') != -1 && maxStr.length - maxStr.indexOf('.') > amountDigits) {
				maxStr = maxStr.substring(0, maxStr.indexOf('.') + amountDigits + 1);
			}
			planMaxAmount = Number(maxStr);
			$('#planMaxAmount').html(planMaxAmount);
		} else {
			$('#planMaxAmount').html("0");
		}
		if (marketPrice == 0 || amount == 0) {
			$('#cash_deposit').html(0);
			return false;
		}
	} else {
		if (price > 0) {
			planMaxAmount = remainAmount / (price / multiple + price * feeRate) / coeffcient;	// 最大可开仓数量

			var maxStr = planMaxAmount + "";
			if (maxStr.indexOf('.') != -1 && maxStr.length - maxStr.indexOf('.') > amountDigits) {
				maxStr = maxStr.substring(0, maxStr.indexOf('.') + amountDigits + 1);
			}
			planMaxAmount = Number(maxStr);
			$('#planMaxAmount').html(planMaxAmount);
		} else {
			$('#planMaxAmount').html("0");
		}
		if (price == 0 || amount == 0) {
			$('#cash_deposit').html(0);
			return false;
		}
	}

	if (amount < Number($('#minKcNums').val())) {
		amount = $('#minKcNums').val();
		return false;
	}
	// 开仓数大于最大可开仓数时，更改 开仓数
	if (amount > planMaxAmount) {
		$('#planAmount').val(planMaxAmount);
		return false;
	} else {
		$('#planAmount').val(formatFloat(amount, amountDigits));
	}
	//保证金
	if (orderType == 1) {
		$('#cash_deposit').html((marketPrice * ($('#planAmount').val() / multiple) * coeffcient).toFixed(2));
	} else {
		$('#cash_deposit').html((price * ($('#planAmount').val() / multiple) * coeffcient).toFixed(2));
	}

	return true;
}

function showTipsBox() {
	$(".show-usetip-box").show();
}
function hideTipsBox() {
	$(".show-usetip-box").hide();
}
function showLeverBox() {
	$(".show-lever-box").show();
}
function hideLeverBox() {
	$(".show-lever-box").hide();
}
function showMarketTipsBox() {
	$(".show-marketTip-box").show();
}
function hideMarketTipsBox() {
	$(".show-marketTip-box").hide();
}
function showMarketCloseTipsBox() {
	$(".show-marketCloseTip-box").show();
}
function hideMarketCloseTipsBox() {
	$(".show-marketCloseTip-box").hide();
}
//显示限价平仓弹框
function showPcBox(id) {
	$(".show-pc-box" + id).show();
}
//隐藏限价平仓弹框
function hidePcBox(id, event) {
	$(".show-pc-box" + id).hide();
	//阻止向上冒泡
	window.event.stopPropagation();
}
//限价平仓弹框百分比切换
function percentLimitClose(self, id, type) {
	if(!$(self).hasClass("active")){
		$(".trade-percent-liclose span").removeClass("active");
		$(self).addClass("active");
	}else {
		$(self).removeClass("active");
	}

	var percent = $(self).html();
	var float = percent.replace("%","");
	float = float / 100;
	var buyHoldCount = parseFloat($("#buyHoldCount" + id).html())
	var sellHoldCount = parseFloat($("#sellHoldCount"+ id).html());

	if(type == 1) {
		var canOpen = buyHoldCount;
	} else {
		var canOpen = sellHoldCount;
	}
	if (!isNaN(canOpen)) {
		var amountDigits = Number($('#amountDigits').val());
		var amount = float * canOpen;
		var maxStr = amount + "";
		if (maxStr.indexOf('.') != -1 && maxStr.length - maxStr.indexOf('.') > amountDigits) {
			maxStr = maxStr.substring(0, maxStr.indexOf('.') + amountDigits + 1);
		}
		if(type == 1) {
			$("#pcAmount_d"+id).val(maxStr);
		} else {
			$("#pcAmount_k"+id).val(maxStr);
		}
	}
}

//开仓-委托类型下拉框显示
function showOrderOptions() {
	$(".type-options-order").show();
	$(".type-order-inner").css("border", "1px solid #4980E3")
	$(".show-marketCloseTip-box").hide();
}
//开仓-委托类型下拉框隐藏
function hideOrderOptions() {
	$(".type-options-order").hide();
	$(".type-order-inner").css("border", "1px solid #4A5F78")
}
//开仓-杠杆下拉框显示
function showLeverOptions() {
	$(".select-order-item").show();
	$(".order-input-inner").css("border", "1px solid #4980E3");
}
//开仓-杠杆下拉框隐藏
function hideLeverOptions() {
	$(".select-order-item").hide();
	$(".order-input-inner").css("border", "1px solid #4A5F78")
}

//开仓-杠杆下拉框显示
function showCloseTypeOptions() {
	$(".type-close-order").show();
	$(".type-close-inner").css("border", "1px solid #4980E3");
}
//开仓-杠杆下拉框隐藏
function hideCloseTypeOptions() {
	$(".type-close-order").hide();
	$(".type-close-inner").css("border", "1px solid #4A5F78");
}

//平仓-杠杆下拉框显示
function showCloseOptions() {
	$(".select-close-item").show();
	$(".close-input-inner").css("border", "1px solid #4980E3")
}

//平仓-杠杆下拉框隐藏
function hideCloseOptions() {
	$(".select-close-item").hide();
	$(".close-input-inner").css("border", "1px solid #4A5F78")
}

//止盈止损-杠杆下拉框显示
function showSelectOptions() {
	$(".select-options-item").show();
	$(".input-inner").css("border", "1px solid #4980E3");
}

//止盈止损-杠杆下拉框隐藏
function hideSelectOptions() {
	$(".select-options-item").hide();
	$(".input-inner").css("border", "1px solid #4A5F78");
}

//止盈止损-显示限价&市价类型下拉框
function showTypeOptions() {
	$(".type-options-item").show();
	$(".type-input-inner").css("border", "1px solid #4980E3")

	if($(".type-input-inner").val() == fmtFullLimitOrder) {
		$(".type-option-limit").addClass("active");
	} else {
		$(".type-option-market").addClass("active");
	}
}
//止盈止损-隐藏限价&市价类型下拉框
function hideTypeOptions() {
	$(".type-options-item").hide();
	$(".type-input-inner").css("border", "1px solid #4A5F78")
}
//止盈止损-限价&市价按钮切换
function typeOptionsBtnChange(self, type) {
	$(self).addClass("active").siblings().removeClass("active");
	var activeMsg = $(self).html();
	var tid = $('#mt_mtid').val();
	var dir = $('#mt_direction').val();

	if($(self).hasClass("active")) {
		$(".type-input-inner").val(activeMsg).css("border", "1px solid #4A5F78");
		$(".type-options-item").hide();
	}

	$("#mt_OrderType").val(type);

	triggerStopLossData(dir, tid, type);
	//阻止向上冒泡
	window.event.stopPropagation();
}
//开仓委托类型切换方法
function typeOrderBtnChange(self, type) {
	var activeMsg = $(self).html();
	$(".type-order-inner").val(activeMsg).css("border", "1px solid #4A5F78");
	$(".type-options-order").hide();

	if(type == 0) {
		$("#full-order").show();
		$("#full-stop-loss").hide();
		$("#full-market-order").hide();
		$("#full-plan-entrust").hide();
		$("#close-sell-position").hide();
		$("#close-market-position").hide();
		setDelegateType($(self).html());
		$(".type-order-inner").val(activeMsg).css("border", "1px solid #4A5F78");
		$(".type-options-order").hide();
	} else if(type == 1){
		$("#full-plan-entrust").show();
		$("#full-order").hide();
		$("#full-market-order").hide();
		$("#full-stop-loss").hide();
		$("#close-sell-position").hide();
		$("#close-market-position").hide();
		setDelegateType($(self).html());
	} else if(type == 2) {
		$("#full-market-order").show();
		$("#full-order").hide();
		$("#full-plan-entrust").hide();
		$("#close-sell-position").hide();
		$("#full-stop-loss").hide();
		$("#close-market-position").hide();
		setDelegateType($(self).html());
		$(".market-order-inner").attr({'onmouseover': 'showMarketTipsBox()', 'onmouseout': 'hideMarketTipsBox()'});
	}
	//阻止向上冒泡
	window.event.stopPropagation();
}
//平仓类型切换方法
function typeCloseBtnChange(self, type) {
	var activeMsg = $(self).html();
	$(".type-close-inner").val(activeMsg).css("border", "1px solid #4A5F78");
	$(".type-close-order").hide();
	if(type == 0) {
		//限价平仓
		$("#closePriceInput").show();
		$("#closeAmountInput").show();
		$("#closePercentBtn").show();
		$("#triPriceInput").hide();
		$("#entruePriceInput").hide();
		$("#planAmountInput").hide();
		$("#closeTradeBtn").show();
		$("#closePlanTradeBtn").hide();
		$(self).addClass("active").siblings().removeClass("active");
		$("#closeSellPrice").attr({'disabled' : false, 'placeholder' : fmtCloseValue,'value': ''}).css('background-color', '#0d1923').removeClass('order-input-disabled').addClass('order-input');
		if (IsWhite) {
			$("#closeSellPrice").css('background-color', '#fff');
		}
		$(".closeUnitid").css('display', 'block');
		$("#closeOrderType").val('');
		$(".market-underline").hide();
		$(".type-close-inner").removeAttr('onmouseover onmouseout');
	} else if(type == 1){
		//计划委托
		$(self).addClass("active").siblings().removeClass("active");
		if (IsWhite) {
			$("#closeSellPrice").css('background-color', '#f6fafd');
		}
		$(".closeUnitid").css('display', 'none');
		$("#closeOrderType").val('');
		$(".market-underline").hide();

		/*限价平仓&市价平仓显示框隐藏*/
		$("#closePriceInput").hide();
		$("#closeAmountInput").hide();
		$("#closePercentBtn").hide();
		$("#triPriceInput").show();
		$("#entruePriceInput").show();
		$("#planAmountInput").show();
		$("#closeTradeBtn").hide();
		$("#closePlanTradeBtn").show();
	} else if(type == 2){
		//市价平仓
		$("#closePriceInput").show();
		$("#closeAmountInput").show();
		$("#closePercentBtn").show();
		$("#triPriceInput").hide();
		$("#entruePriceInput").hide();
		$("#planAmountInput").hide();
		$("#closeTradeBtn").show();
		$("#closePlanTradeBtn").hide();
		$(self).addClass("active").siblings().removeClass("active");
		$("#closeSellPrice").attr({'disabled' : 'disabled', 'placeholder' : fmtBestPriceOrder, 'value': ''}).css('background-color', '#1B2737').removeAttr("onblur onkeyup").addClass('order-input-disabled').removeClass('order-input');
		if (IsWhite) {
			$("#closeSellPrice").css('background-color', '#f6fafd');
		}
		$(".closeUnitid").css('display', 'none');
		$("#closeOrderType").val(1);
		$(".market-underline").show();
		$(".type-close-inner").attr({'onmouseover': 'showMarketCloseTipsBox()', 'onmouseout': 'hideMarketCloseTipsBox()'});
	}
	//阻止向上冒泡
	window.event.stopPropagation();
}
var activeUseMarketPrice = false;
var activeCloseMarketPrice = false;
//计划委托市价按钮
function useMarketPrice(self) {
	var orderType = $("#planOrder").val();
	var planPrice = $("#planPrice");
	var digits = $('#myDigits').val();
	if(activeUseMarketPrice == false){
		activeUseMarketPrice = true;
		$("#planOrder").val('1')
		planPrice.attr({"placeholder": bestPriceOrder, 'value': '', 'disabled' : 'disabled'}).css({'background-color':'#1E2939'}).removeAttr("onblur onkeyup").addClass('order-input-disabled').removeClass('order-input');
		if(IsWhite) {
			planPrice.attr({"placeholder": bestPriceOrder, 'value': '', 'disabled' : 'disabled'}).css({'background-color':'#f9fcff'}).removeAttr("onblur onkeyup").addClass('order-input-disabled').removeClass('order-input');
		}
		$(".use-unit").hide();
		$(self).css({'color': '#4980E3', 'border': '1px solid #4980E3'});
	} else {
		activeUseMarketPrice = false;
		$("#planOrder").val('0')
		planPrice.attr({"placeholder": entruePrice, 'disabled' : false,'onblur': 'trade_plan_amount()', 'onkeyup' : 'checkInputDigits(this, '+digits+')'}).css({'background-color':'#0D1824'}).removeClass('order-input-disabled').addClass('order-input');
		if(IsWhite) {
			planPrice.attr({"placeholder": entruePrice, 'disabled' : false,'onblur': 'trade_plan_amount()', 'onkeyup' : 'checkInputDigits(this, '+digits+')'}).css({'background-color':'#FFFFFF'}).removeClass('order-input-disabled').addClass('order-input');
		}
		$(".use-unit").show();
		$(self).css({'color': '#94abc0', 'border': '1px solid #4a5f78'});
	}
}
//平仓计划委托市价按钮
function closeUseMarketPrice() {
	var closePlanPrice = $("#closePlanPrice");
	var digits = $('#myDigits').val();
	if(activeCloseMarketPrice == false) {
		activeCloseMarketPrice = true;
		$("#closePlanOrder").val('1')
		closePlanPrice.attr({"placeholder": bestPriceOrder, 'value': '', 'disabled' : 'disabled'}).css({'background-color':'#1E2939'}).removeAttr("onkeyup").addClass('order-input-disabled').removeClass('order-input');
		if(IsWhite) {
			closePlanPrice.attr({"placeholder": bestPriceOrder, 'value': '', 'disabled' : 'disabled'}).css({'background-color':'#f9fcff'}).removeAttr("onkeyup").addClass('order-input-disabled').removeClass('order-input');
		}
		$(self).css({'color': '#4980E3', 'border': '1px solid #4980E3'});
	} else {
		activeCloseMarketPrice = false;
		$("#closePlanOrder").val('0')
		closePlanPrice.attr({"placeholder": entruePrice, 'disabled' : false, 'onkeyup' : 'checkInputDigits(this, '+digits+')'}).css({'background-color':'#0D1824'}).removeClass('order-input-disabled').addClass('order-input');
		if(IsWhite) {
			closePlanPrice.attr({"placeholder": entruePrice, 'disabled' : false, 'onkeyup' : 'checkInputDigits(this, '+digits+')'}).css({'background-color':'#FFFFFF'}).removeClass('order-input-disabled').addClass('order-input');
		}
		$(self).css({'color': '#94abc0', 'border': '1px solid #4a5f78'});
	}
}
//交易页面-显示滑动杠杆框
function showSliderBlock() {
	var transaction_code = $('#transaction_code').val();
	var sliderMultiple = $("#slider-multiple-windows");
	var dpzz = $("#background_overlay");
	var avtiveMultiple = $(".order-input-inner").val().split('x')[1];

	refreshMemberTrans();//持仓管理
	refreshPending();//限价委托
	refreshPlanEntrust();//计划委托
	if(showsliderBoxMemberTransBuy == true && showsliderBoxMemberTransSell == true && showsliderBoxPending == true && showsliderBoxPlanEntrust == true) {
		show(sliderMultiple);
		dpzz.show();
		$("#sliderMultipleNum").val(avtiveMultiple + 'x');
	} else {
		showMsgMultipleBox(requestCanMultiple);
	}
}

//关闭滑动杠杆框
function closeSliderBlock() {
	var sliderMultiple = $("#slider-multiple-windows");
	var avtiveMultiple = $(".order-input-inner").val().split('x')[1];
	var dpzz = $("#background_overlay");
	$('.single-slider').jRange('setValue', parseInt(avtiveMultiple, 10));
	hide(sliderMultiple);
	dpzz.hide();
}

//确认提交杠杆数值
function submitMultipleNum() {
	var avtiveMultiple = $("#sliderMultipleNum").val().split('x')[0];
	$(".order-input-inner").val('x' + avtiveMultiple);
	$('#multiple').val(avtiveMultiple);
	$('#planMultiple').val(avtiveMultiple);
	$('#market_multiple').val(avtiveMultiple);
	multipleClick(avtiveMultiple);
	closeSliderBlock();
}

//提示当前支持杠杆
function showMsgMultipleBox(text) {
		$("#showBoxText").addClass('warningNew');
		$("#showBoxText").removeClass('successNew');
		$("#showBoxText").html(text);
		var time = 7000;
		$("#showBox").css("display", "block");
		setTimeout(function() {
			$("#showBox").css("display", "none");
		}, time);
}

//暂时禁止按钮操作
function interimOrder() {
	$(".slider-block").addClass('pointerEvents');
}
//移除禁止按钮操作
function removeInterimOrder() {
	$(".slider-block").removeClass('pointerEvents');
}
//关闭风险tips框
function closeWraningTips() {
	$(".test-site-wraning").hide();
}