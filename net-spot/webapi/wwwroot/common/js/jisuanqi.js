var jHoldDuoCount = 0;
var jHoldDuoPrice = 0;
var jDuoFromRemain = 0;
var jDuoDeposit = 0;

var jHoldKongCount = 0;
var jHoldKongPrice = 0;
var jKongFromRemain = 0;
var jKongDeposit = 0;

var jBalance = 0;
var jCoefficient = 1;
var jDigits = 4;

var jDuoFundingFee = 0;
var jKongFundingFee = 0;

function jisuanIncome(showLog) {
	var jMultiple = $("#MultipleSelect option:selected").val();//杠杆倍数
	var jAmount = $("#jisuanOpenAmount").val();//开仓数量
	var jPrice = $("#jisuanOpenPrice").val();//开仓价格
	var jClosePrice = $("#jisuanClosePrice").val();//平仓价格
	var tradePath = $('#tradePath').val();
	var myCommissionRate = Number($('#myCommissionRate').val());//（吃单费率）
	var pendingRate = Number($('#pendingRate').val());//（挂单费率）
	jMultiple = jMultiple.replace("x", "");
	
	if (jAmount == "") {
		if(showLog)
			$("#jisuanError1").html("请输入开仓数量");
	} else if (jPrice == "") {
		if(showLog)
			$("#jisuanError1").html("请输入开仓价格");
	} else if (jClosePrice == "") {
		if(showLog)
			$("#jisuanError1").html("请输入平仓价格");
	} else {
		$("#jisuanError1").html("");
		var jDeposit = jAmount * jPrice / jMultiple;
		var income = (jClosePrice - jPrice) * jAmount;
		var jType = $("#jisuanType").val();
		if (jType == 2) {
			income = -income;
		}
		var incomeRate = income / jDeposit * 100;

		jDeposit = parseFloat(jDeposit.toFixed(8));
		income = parseFloat(income.toFixed(8));
		incomeRate = parseFloat(incomeRate.toFixed(2));

		$("#jisuanDeposit").html(jDeposit + (tradePath == "vtrade" ? " BFC" : (tradePath == "usdr" ? " " + tradePath.toUpperCase() : " USDT")));
		$("#jisuanIncome").html((income > 0 ? '+' : '') + income + (tradePath == "vtrade" ? " BFC" : (tradePath == "usdr" ? " " + tradePath.toUpperCase() : " USDT")));
		$("#jisuanIncomeRate").html((income > 0 ? '+' : '') + incomeRate + "%");


		if ($('#userEmail').val() != null && $('#userEmail').val() != "") {
			console.log("11")
			var orderPendingRate = (jPrice * jAmount) * pendingRate + (jClosePrice * jAmount) * pendingRate;//挂单手续费
			var eatMyCommissionRate = (jPrice * jAmount) * myCommissionRate + (jClosePrice * jAmount) * myCommissionRate;//吃单手续费
			orderPendingRate = parseFloat(orderPendingRate.toFixed(8));
			eatMyCommissionRate = parseFloat(eatMyCommissionRate.toFixed(8));
			$("#eatMyCommissionRate").html(eatMyCommissionRate + (tradePath == "vtrade" ? " BFC" : (tradePath == "usdr" ? " " + tradePath.toUpperCase() : " USDT")));
			$("#orderPendingRate").html(orderPendingRate  + (tradePath == "vtrade" ? " BFC" : (tradePath == "usdr" ? " " + tradePath.toUpperCase() : " USDT")));
		}
	}
}

$(".jisuanqi-direction .jisuanqi-direction-type1").click(function() {
	$(this).addClass("current");
	$(".jisuanqi-direction .jisuanqi-direction-type2").removeClass("current");
});

$(".jisuanqi-direction .jisuanqi-direction-type2").click(function() {
	$(this).addClass("current");
	$(".jisuanqi-direction .jisuanqi-direction-type1").removeClass("current");
});

$(".jisuanqi-tab span").click(function() {
	$(this).addClass("current").siblings().removeClass();
});

function jisuanIncomeClick(type) {
	$("#jisuanType").val(type);
	jisuanIncome(false);
}

function jisuanTabClick(type) {
	if (type == 1) {
		$(".jisuanqi-income").css("display", "block");
		$(".jisuanqi-qpj").css("display", "none");
	} else {
		$(".jisuanqi-income").css("display", "none");
		$(".jisuanqi-qpj").css("display", "block");
		
		getHolds();
	}
}

function jisuanQpjClick(type) {
	$("#jisuanQpjType").val(type);
	jisuanQpj(false);
}

function getHolds() {
	if ($('#userEmail').val() == null || $('#userEmail').val() == "") {
		return false;
	}
	
	var digits = $('#myDigits').val();
	var transaction_code = $('#transaction_code').val();
	var suspend = $('#suspend').val();
	var tradePath = $('#tradePath').val();
	var ajaxPath = 'trade';
	if (tradePath == 'vtrade') {
		ajaxPath = 'vtrade';
	} else if (tradePath == 'trade') {
		ajaxPath = 'trade';
	} else {
		ajaxPath = 'ntrade/' + tradePath;
	}
	$.ajax({
        type: "POST",
        dataType: "json",
        url: basePath + ajaxPath + "/jisuanMemTransData.do",
        data: {"transactionCode":transaction_code, token : getCookie("_token")},
        error: function (err) {
    		
        },
        success: function(data) {
        	var buyMemTrans = eval('(' + data.buyMemberTransaction + ')');
        	jBalance = data.balance;
        	jCoefficient = data.coefficient;
        	jDigits = data.digits;
        	$("#jisuanqiBalance").html(parseFloat(jBalance.toFixed(4)));
        	if (buyMemTrans.length > 0) {
        		var tran = buyMemTrans[0];
        		jHoldDuoCount = tran.hold_count + tran.lock_count;
        		jHoldDuoPrice = tran.average_price;
        		jDuoFromRemain = tran.f_frozenremain + tran.f_fromremain + tran.f_min_frozenremain;
        		jDuoDeposit = tran.deposit;
        		jDuoFundingFee = tran.deposit2;
        	}
        	
        	var sellMemTrans = eval('(' + data.sellMemberTransaction + ')');
        	if (sellMemTrans.length > 0) {
        		var tran = sellMemTrans[0];
        		jHoldKongCount = tran.hold_count + tran.lock_count;
        		jHoldKongPrice = tran.average_price;
        		jKongFromRemain = tran.f_frozenremain + tran.f_fromremain + tran.f_min_frozenremain;
        		jKongDeposit = tran.deposit;
        		jKongFundingFee = tran.deposit2;
        	}
        	
        	var qpType = $("#jisuanQpjType").val();
        	if (qpType == 1) {
        		$("#jisuanQpjHold").html(jHoldDuoCount + " 个");
        		if (jHoldDuoPrice > 0) {
        			var price = jHoldDuoPrice.toFixed(digits);
        			$("#jisuanAvgPrice").html(price + " U");
        		}
        	} else {
        		$("#jisuanQpjHold").html(jHoldKongCount + " 个");
        		if (jHoldKongPrice > 0) {
        			var price = jHoldKongPrice.toFixed(digits);
        			$("#jisuanAvgPrice").html(price + " U");
        		}
        	}
        }
    });
}

function jisuanQpj(showLog) {
	var jMultiple = $("#MultipleSelectQ option:selected").val();
	jMultiple = jMultiple.replace("x", "");
	
	var jAmount = Number($("#jisuanQpjAmount").val());
	var jPrice = Number($("#jisuanQpjPrice").val());
	var qpType = $("#jisuanQpjType").val();
	
	if (jAmount == "") {
		jAmount = 0;
		jPrice = 0;
	} else if (jPrice == "" || jPrice <= 0) {
		if(showLog) {
			$("#jisuanError2").html("请输入开仓价格");
		}
		
		return;
	}
	
	var digits = $('#myDigits').val();
	var positionDeposit = jDuoDeposit;
	var positionPrice = jHoldDuoPrice;
	var positionHoldCount = jHoldDuoCount;
	var positionRemain = jDuoFromRemain;
	var positionFundingFee = jDuoFundingFee;
	$("#jisuanQpjHold").html(jHoldDuoCount + " 个");
	if (jHoldDuoPrice > 0) {
		$("#jisuanAvgPrice").html(jHoldDuoPrice.toFixed(digits) + " U");
	} else {
		$("#jisuanAvgPrice").html("");
	}
	
	if (qpType != 1) {
		positionDeposit = jKongDeposit;
		positionPrice = jHoldKongPrice;
		positionHoldCount = jHoldKongCount;
		positionRemain = jKongFromRemain;
		positionFundingFee = jKongFundingFee;
		$("#jisuanQpjHold").html(jHoldKongCount + " 个");
		if (jHoldDuoPrice > 0) {
			$("#jisuanAvgPrice").html(jHoldKongPrice.toFixed(digits) + " U");
		} else {
			$("#jisuanAvgPrice").html("");
		}
	}
	
	$("#jisuanQpjHold2").html("");
	$("#jisuanQpjTriPrice").html("");
	$("#jisuanError2").html("");
	var jDeposit = positionDeposit;
	var spend = jAmount * jPrice / jMultiple;
	jDeposit += spend;
	var income = 0;
	var qpPrice = 0;
	if (jAmount < 0) {
		var amt = -jAmount;
		income = (jPrice - positionPrice) * amt;
	}
	
	jPrice = (positionHoldCount * positionPrice + jAmount * jPrice) / (positionHoldCount + jAmount);
	jAmount += positionHoldCount;
	if (jAmount <= 0) {
		return;
	}
	
	if (qpType == 1) {
		// avg_price - (deposit - 0.2 * posDeposit) / (amount * coefficient)
		qpPrice = jPrice - (jDeposit + positionRemain + positionFundingFee + jBalance + income - spend - 0.2 * jDeposit) / (jAmount * jCoefficient);
	} else {
		income = -income;
		// avg_price + (deposit - 0.2 * posDeposit) / (amount * coefficient)
		qpPrice = jPrice + (jDeposit + positionRemain + positionFundingFee + jBalance + income - spend - 0.2 * jDeposit) / (jAmount * jCoefficient);
	}
	
	var hold = parseFloat(jAmount.toFixed(8));
	qpPrice = parseFloat(qpPrice.toFixed(jDigits));
	
	$("#jisuanQpjHold2").html(hold + " 个");
	$("#jisuanAvgPrice").html(jPrice.toFixed(digits) + " U");
	$("#jisuanQpjTriPrice").html(qpPrice + (tradePath == "vtrade" ? "" : " U"));
}

function closeJisuan() {
	$("#jisuan-windows").css("display", "none");
}

function openJisuan() {
	$("#jisuan-windows").css("display", "block");
}