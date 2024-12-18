var balance;
var transferDirection = 1; // 划转方向，默认转入
var currCode;
var transfer_in_enable;
var transfer_out_enable;

//user-显示资金划转框
function userfundsTransferBox(code, showName, in_enable, out_enable, contractType) {
	currCode = code;
	transfer_in_enable = in_enable;
	transfer_out_enable = out_enable;
	$("#showName").html(showName.toUpperCase());
	$("#amountmsg").html("");
	getAmount(contractType);
	var fundsTransferAlert = $("#user-fundsTtransfer-windows");
	var fundsTransfer = $("#user-fundsTransfer-alert");
	var dpzz = $("#tb-overlay-duiping");
	fundsTransfer.css({
	        position: "absolute",
	        top: ($(document).scrollTop()),
	    });
	show(fundsTransferAlert);
	show(fundsTransfer);
	dpzz.show();
}

//user-关闭资金划转框
function usercloseFundsAlert() {
	var fundsTransferAlert = $("#user-fundsTtransfer-windows");
	var fundsTransfer = $("#user-fundsTransfer-alert");
	var dpzz = $("#tb-overlay-duiping");
	hide(fundsTransferAlert);
	hide(fundsTransfer);
	dpzz.hide();
}
//切换划转方向,默认转出
function changeTransferDirection(contractType) {
	var tradeTypeCode = $(".trade-fundsTtransfer-typeCode").html();
	var tradeAccountType = $(".trade-fundsTtransfer-accountType").html();

	if ($(".trade-fundsTtransfer-typeCode").html() == tradeTypeCode) {
		$(".trade-fundsTtransfer-typeCode").html(tradeAccountType);
		$(".trade-fundsTtransfer-accountType").html(tradeTypeCode);
	} else {
		$(".trade-fundsTtransfer-typeCode").html(tradeTypeCode);
		$(".trade-fundsTtransfer-accountType").html(tradeAccountType);
	}
	$('#transferAmount').val("");
	if (transferDirection == 1) {
		transferDirection = 2;
	} else {
		transferDirection = 1;
	}
	getAmount(contractType);
}

//查询可转数量
function getAmount(contractType) {
	$.post(basePath + 'transferAccount/getAmount.do', {
		bqCode : currCode,
		contractType : contractType,
		type : transferDirection,
	}, function(data) {
		/*console.log(data);*/
		if (data.code == 'success') {
			balance = data.frozen;
			$("#trade-totalnum").html(balance);
			/*console.log(balance);*/
		} else {
			console.log("fail");
		}
	}, "json");

	return false;
}
//全部划转
function tranferAll() {
	$("#transferAmount").val(balance);
	$("#amountmsg").html("");
}
function transferRecord(type) {
	var url = "transferAccount/transfer.do";
	if ("vcontract" == type) {
		url = "transferAccount/vtransfer.do"+ '?type=' + transferDirection;
	} else if ("delivery" == type) {
		url = "transferAccount/dtransfer.do"+ '?type=' + transferDirection
	} else if ("contract" == type || "" == type) {
		url = "transferAccount/transfer.do"+ '?type=' + transferDirection
	} else {
		url = "transferAccount/ntransfer.do" + '?partition=' + type + '&type=' + transferDirection
	}
	document.location.href = basePath + url + '&bqCode=' + currCode;
}
/**
 * 检查输入的数量是否可用
 * @param max
 * @return
 */
function checkAmount(max) {
	var amount = Number($('#transferAmount').val());
	if (amount > max) {
		showMsg(MSG['transferNum']);
		return false;
	}
	$('#amountmsg').html("");
	return true;
}

function checkNull() {
	var amount = Number($('#transferAmount').val());
	if (amount > balance) {
		$('#transferAmount').val(balance);
	}
	if (amount <= 0) {
		showMsg(MSG['amounttrsNull']);
		return false;
	}
	$('#amountmsg').html("");
	return true;
}
/**
 * 转入转出
 * @return
 */
function recharge(type) {
	var amount = Number($('#transferAmount').val());
	if (!checkAmount(amount)) {
		return false;
	}
	
	var URL = "";
	if (transferDirection == 1) {
		if (transfer_in_enable == 'false') {
			showMsg(MSG['riskPause'] + code.toUpperCase() + MSG['transferIn']);
			return false;
		}

		var action = "transferAccount/transfertodo.do";
		if ("vcontract" == type) {
			action = "transferAccount/vtransfertodo.do";
		} else if ("delivery" == type) {
			action = "transferAccount/dtransfertodo.do";
		} else if ("contract" == type || "" == type) {
			action = "transferAccount/transfertodo.do";
		} else {
			action = "transferAccount/ntransfertodo.do";
		}
		
		URL = basePath + action;
	} else {
		if (transfer_out_enable == 'false') {
			showMsg(MSG['riskPause'] + code.toUpperCase() + MSG['transferOut']);
			return false;
		}
		
		var action = "transferAccount/transferoutdo.do";
		if ("vcontract" == type) {
			action = "transferAccount/vtransferoutdo.do";
		} else if ("delivery" == type) {
			action = "transferAccount/dtransferoutdo.do";
		} else if ("contract" == type || "" == type) {
			action = "transferAccount/transferoutdo.do";
		} else {
			action = "transferAccount/ntransferoutdo.do";
		}
		
		URL = basePath + action;
	}
	$.ajax({
		type : "POST",
		dataType : "JSON",
		url : URL,
		data : {
			bqCode : currCode,
			amount : amount,
			partition: type,
			payPwd : $('#payPwd').val(),
			token : getCookie("_token")
		},
		success : function(data) {
			/*console.log(data);*/
			showSuccInfo_transfer(data.msg, data.code);
			getAmount(type ? type : 'contract');
			$('#transferAmount').val("");
		}
	});
}
function showSuccInfo_transfer(text, code) {
	if (text.length > 0) {
		$("#amountmsg").css("display", "block");
		$("#amountmsg").html(text);
		$("#amountmsg").show();
		if (code == "success") {
			$("#amountmsg").css("color", "green");
			setTimeout('showSuccInfo_transfer("", "' + code + '")', 3000);
			getData();
		} else {
			$("#amountmsg").css("color", "red");
		}
	} else {
		usercloseFundsAlert();
	}
}
function showMsg(text) {
	if (text.length > 0) {
		$("#amountmsg").html(text);
		$("#amountmsg").show();
	}
}

/**
 * 检查输入
 * 
 * @param oInput
 * @param type
 * @param e
 * @return
 */
function checkInput(oInput, type, e) {
	if (type == 'num') {
		var len = 8;
		var exp = /\d{1,8}\.{0,1}\d{0,3}/;
	} else if (type == 'int') {
		var len = 8;
		var exp = /\d{1,8}/;
	} else if (type == 'bp') {
		var len = 8;
		var exp = /\d{1,8}\.{0,1}\d{0,2}/;
	} else if (type == 'cash') {
		var len = 16;
		var exp = /\d{1,8}\.{0,1}\d{0,5}/;
	} else if (type == 'tinynum') {
		var len = 6;
		var exp = /\d{1,6}\.{0,1}\d{0,3}/;
	} else if (type == 'price2') {
		var len = 5;
		var exp = /\d{1,5}\.{0,1}\d{0,3}/
	} else if (type == 'price3') {
		var len = 5;
		var exp = /\d{1,5}\.{0,1}\d{0,4}/
	} else if (type == 'pricebp') {
		var len = 4;
		var exp = /\d{1,5}\.{0,1}\d{0,4}/
	} else if (type == 'loan') {
		var len = 2;
		var exp = /\d{1,1}\.{0,1}\d{0,2}/
	} else if (type == 'esix') {
		var len = 2;
		var exp = /\d{1,8}\.{0,1}\d{0,5}/
	} else {
		var len = 4;
		var exp = /\d{1,5}\.{0,1}\d{0,3}/
	}
	if ('' != oInput.value.replace(exp, '')) { // 含有数字的话
		oInput.value = oInput.value.match(exp) == null ? '' : oInput.value.match(exp);
		if (oInput.value.indexOf('.') == -1 && oInput.value.length > len) {
			oInput.value = oInput.value.substr(0, len) + '.' + oInput.value.substr(len);
		}
	} else {
		if (oInput.value.indexOf('.') == -1 && oInput.value.length > len) {
			oInput.value = oInput.value.substr(0, len) + '.' + oInput.value.substr(len);
		}
	}
	if (oInput.value.length > 0 && Number(oInput.value) == 0 && oInput.value.indexOf('.') == -1) {
		oInput.value = 0;
	}
}