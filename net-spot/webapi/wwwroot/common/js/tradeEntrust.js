function changeCode(){
	var code = $("#transactionCode").val();
	window.location.href = 'entrust.do?transactionCode=' + code;
}

function cancelDeal(direction){
	var code = $("#transactionCode").val();
	$.get(basePath +"tradeAjax/cancleDeal.do", {
		direction : direction,
		code : code,
		token : getCookie("_token")
	}, function(data) {
		location.reload();
		return;
	});
}