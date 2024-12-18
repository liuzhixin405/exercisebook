$(function() {
	getData();

	$("#hidden-asset-detail").click(function() {
		if ($(this).attr("checked")) {
			hiddenZero = true;
		} else {
			hiddenZero = false;
		}
		showData(dataArr);
	});
});

var hiddenZero = false;
var dataArr = [];
var type = $('#actionName').val();
var actionName = $('#actionName').val();
if (actionName == "vCenter") {
	type = "vcontract";
} else if (actionName == "dCenter") {
	type = "delivery";
} else if (actionName == "center") {
	type = "contract";
}

function getData() {
	$.ajax({
		type : "GET",
		dataType : "json",
		url : basePath + "center/detail.do?type=" + type,
		data : {},
		success : function(data) {
			if (data.code == 0) {
				dataArr = data.data;
				showData(dataArr);
			}
		}
	});
}

function searchChange() {
	var data = dataArr;
	showData(data);
}

function showData(data) {
	var filter = $("#asset-detail-search").val();
	var tickerHtml = "";
	for (var index = 0; index < data.length; index++) {
		var obj = data[index];

		var maxStr = parseFloat(obj.f_btc) + "";
		if (maxStr.indexOf('.') != -1 && maxStr.length - maxStr.indexOf('.') > 4) {
			maxStr = maxStr.substring(0, maxStr.indexOf('.') + 4 + 1);
		}
		var contractValueStr = parseFloat(obj.contractValue) + "";
		if (contractValueStr.indexOf('.') != -1 && contractValueStr.length - contractValueStr.indexOf('.') > 4) {
			contractValueStr = contractValueStr.substring(0, contractValueStr.indexOf('.') + 4 + 1);
		}

		obj.f_btc = parseFloat(maxStr);
		obj.contractValue = parseFloat(contractValueStr);
		if (hiddenZero) {
			var total = obj.buyAmount + obj.sellAmount + obj.buydelegation
			+ obj.selldelegation + obj.f_btc + obj.contractValue
			if (total <= 0) {
				continue;
			}
		}

		var showname = obj.showName;
		if (filter && filter.length > 0) {
			if (!showname.startsWith(filter.toUpperCase())) {
				continue;
			}
		}

		var goTotradeUrl = basePath + "trade/trade.do?transactionCode=" + obj.code;
		if (type == "vcontract") {
			goTotradeUrl = basePath + "vtrade/trade.do?transactionCode=" + obj.code;
		} else if(type == "delivery"){
			goTotradeUrl = basePath + "dtrade/trade.do?transactionCode=" + obj.code;
		} else if (type == "contract") {
			goTotradeUrl = basePath + "trade/trade.do?transactionCode=" + obj.code;
		} else {
			goTotradeUrl = basePath + "ntrade/" + type + "/trade.do?transactionCode=" + obj.code;
		}

		tickerHtml += "<tr>"
			+ "<td>" + obj.showName + "</td>"
			/*+ "<td>" + parseFloat(obj.totalAmount) + "</td>"*/
			+ "<td>" + parseFloat(obj.buyAmount) + "</td>"
			+ "<td>" + parseFloat(obj.sellAmount) + "</td>"
			/*+ "<td>" + parseFloat(obj.delegationAmount) + "</td>"*/
			+ "<td>" + parseFloat(obj.buydelegation) + "</td>"
			+ "<td>" + parseFloat(obj.selldelegation) + "</td>"
			+ "<td>" + obj.f_btc + "</td>"
			/*+ "<td>" + obj.contractValue + "</td>"*/
			+ "<td>"
			+ "<a onclick=\"userfundsTransferBox('" + obj.code + "', '" + obj.showName + "', '" + obj.transfer_in_enable + "', '" + obj.transfer_out_enable + "', '" + type + "');\"" + " class='btn btn-blue'>" + MSG['transfer'] + "</a>"
			+ "<a href='" + goTotradeUrl + "' class='btn btn-blue'>" + MSG['goTotrade'] + "</a>"
			+ "</td>"
			+ "</tr>";
	}
	$("#tickerTbody").html(tickerHtml);
}