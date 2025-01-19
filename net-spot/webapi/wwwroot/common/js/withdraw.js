var password_bol=false;
var amount_bol=false;
//获取当前跟路径
function getRootPath(){
	return basePath;
}
function loadAmount(){
	//从数据库中取
	$.post(basePath + "center/loadAmount.do", function(data) {
		if(data==0){
			$("#amountAccount").html("0");
			$("#totalAccountNum").html("0");
		}else{
			$("#amountAccount").html(parseFloat(data));
			$("#totalAccountNum").html(parseFloat(data));
			var usdRate = $("#usdRate").val();
			if (usdRate) {
				var totalCny = parseFloat(data) * usdRate;
				$("#totalAccountNumCny").html(totalCny.toFixed(2));
			}
		}
		
		var total = parseFloat(data);
		var path = basePath;
		if (basePath.indexOf("127.") >= 0) {
			path = "http://" + document.domain + ":808/";
		} else if (basePath.indexOf("47.") >= 0) {
			path = "http://" + document.domain + "/";
		}
		
		// $.ajax({
		// 	type : "GET",
		// 	dataType : "json",
		// 	url : path + "otc/otcAssetsApi/datas",
		// 	data : {
		// 		token: localStorage.token
		// 	},
		// 	success : function(data) {
		// 		if (data.code == 0) {
		// 			dataArr = data.data;
		// 			for (var index = 0; index < dataArr.length; index++) {
		// 				var obj = dataArr[index];
		// 				if (obj.coinName == "USDT" || obj.coinName == "USDR") {
		// 					total += (parseFloat(obj.available) + parseFloat(obj.frozen));
		// 				}
		// 			}
		//
		// 			var tot = Math.floor(total * Math.pow(10, 4)) / Math.pow(10, 4);
		// 			$("#amountAccount").html(tot);
		// 			$("#totalAccountNum").html(tot);
		// 			var usdRate = $("#usdRate").val();
		// 			if (usdRate) {
		// 				var totalCny = total * usdRate;
		// 				$("#totalAccountNumCny").html(totalCny.toFixed(2));
		// 			}
		// 		}
		// 	}
		// });
	});
}

function clickopbtc(min,max,fee) {
	var oInput = document.getElementById("amount");
	var amountbtc = document.getElementById("amount").value;
	if($("#change").attr("checked") != "checked"){
		fee = 0;
	}
	var len = 	8;
	var exp	=	/\d{1,8}\.{0,1}\d{0,2}/;
	
    if('' != oInput.value.replace(exp,'')){  
		oInput.value = oInput.value.match(exp) == null ? '' :oInput.value.match(exp);  
        if(oInput.value.indexOf('.') == -1){     
			oInput.value =   oInput.value.substr(0,len)+'.'+ oInput.value.substr(len);    
        }         
    }else{
     	if(oInput.value.indexOf('.') == -1 && oInput.value.length>len){     
           	oInput.value =   oInput.value.substr(0,len)+'.'+ oInput.value.substr(len);   
      	}
    }
	
	document.getElementById("error2").innerHTML = MSG['withdrawAm'];
	
	if(amountbtc < min){
		document.getElementById("error2").innerHTML = MSG['withdrawLeast'] + min +"BTC";
		oInput.value=min;
		return;
	}	
	var ountbtc = amountbtc - fee;
	if(isFinite(ountbtc)){
		ountbtc = ountbtc.toString().substring(0,ountbtc.toString().indexOf('.')+5);
		document.getElementById("error2").innerHTML = MSG['actualAm'] + ountbtc + "BTC";
	}
}
//提交验证
function submitopbtc(min,max,fee,name) {
	var oInput = document.getElementById("amount");
	var amountbtc = document.getElementById("amount").value;
	var amo=Number(amountbtc);
	var balance=document.getElementById("balance").value;
	var bal=Number(balance);

	if (amountbtc == '') {
		return false;
	}
	
	var address = $("#withdrawaddress").val();
	var unLen = address.length;
	var len=Number(unLen);
	if (len == 0) {
		tipAlert(MSG['withdrawNull']);
		return false;
	} else if ((len < 27 || len > 34) && len != 42) {
		tipAlert(MSG['addressError']);
		 return false;
	}
	
	var reg = /^[A-Za-z0-9]+$/;
	if (!reg.test(address)) {
		tipAlert(MSG['addressError']);
		return false;
	}
	
	var len = 8;
	var exp	=/\d{1,8}\.{0,1}\d{0,4}/;
	
    if('' != oInput.value.replace(exp,'')){  
		oInput.value = oInput.value.match(exp) == null ? '' :oInput.value.match(exp);  
        if(oInput.value.indexOf('.') == -1){     
			oInput.value =   oInput.value.substr(0,len)+'.'+ oInput.value.substr(len);    
        }         
    }else{
     	if(oInput.value.indexOf('.') == -1 && oInput.value.length>len){     
           	oInput.value =   oInput.value.substr(0,len)+'.'+ oInput.value.substr(len);   
      	}
    }
/*	if(amo < min){
		tipAlert(MSG['withdrawLeast'] + min + name);
		return false;
	}
	if(amo > max){
		tipAlert(MSG['withdrawMaximum'] + max + name);
		return false;
	}
	if(amo<=0){
		tipAlert(MSG['notAm']);
		amount_bol = false;
		return false;
	}
	if(amo>bal){
		tipAlert(MSG['notEnough']);
		amount_bol = false;
		return false;
	}*/
}
//验证google验证码
function checkGoogle2() {
	var code = $("#google2").val();
	var secretKey = $("#secretKey2").val();
	var unLen = code.replace(/[^\x00-\xff]/g, "**").length;
	if (unLen == 0) {
		document.getElementById("googleerror2").style.color = "red";
		$("#googleerror2").html(MSG['codeNull']);
		code_bol = false;
		return false;
	}
	var path = getRootPath();
	$.post(path + 'center/checkGoogle.do', {
		code : code,
		secretKey : secretKey
	}, function(data) {
		if (data == "false") {
			document.getElementById("googleerror2").style.color = "red";
			$("#googleerror2").html(MSG['codeError']);
			code_bol = false;
			return false;
		} else {
			$("#googleerror2").html("");
			document.getElementById("googleerror2").style.color = "green";
			code_bol = true;
			return true;
		}
	}, "text")
}

// 验证手机验证码
function checkCode2() {
	var tel = $("#mobile2").val();
	var code = $("#code2").val();
	var unLen = code.replace(/[^\x00-\xff]/g, "**").length;
	if (unLen == 0) {
		document.getElementById("errorcode2").style.color = "red";
		$("#errorcode2").html(MSG['codeNull']);
		code_bol = false;
		return false;
	}
	var path = getRootPath();
	$.post(path + '/center/checkTelCode.do', {
		code : code,
		tel:tel
	}, function(data) {
		if (data == "false") {
			document.getElementById("errorcode2").style.color = "red";
			$("#errorcode2").html(MSG['codeError']);
			code_bol = false;
			return false;
		} else {
			$("#errorcode2").html("");
			document.getElementById("errorcode2").style.color = "green";
			code_bol = true;
			return true;
		}
	}, "text")
}

//添加新地址验证google验证码
function checkAddoogle2() {
	var code = $("#googleCode").val();
	var secretKey = $("#secretKey2").val();
	var unLen = code.replace(/[^\x00-\xff]/g, "**").length;
	if (unLen == 0) {
		document.getElementById("checkGoogleerror").style.color = "red";
		$("#checkGoogleerror").html(MSG['codeNull']);
		return false;
	}
	var path = getRootPath();
	$.post(path + 'center/checkGoogle.do', {
		code : code,
		secretKey : secretKey
	}, function(data) {
		if (data == "false") {
			document.getElementById("checkGoogleerror").style.color = "red";
			$("#checkGoogleerror").html(MSG['codeError']);
			code_bol = false;
			return false;
		} else {
			$("#checkGoogleerror").html("");
			document.getElementById("checkGoogleerror").style.color = "green";
			code_bol = true;
			return true;
		}
	}, "text");
}

//添加新地址验证验证手机验证码
function checkPhoneCode2() {
	var tel = $("#mobile").val();
	var code = $("#phoneCode").val();
	var unLen = code.replace(/[^\x00-\xff]/g, "**").length;
	if (unLen == 0) {
		document.getElementById("phoneCodError").style.color = "red";
		$("#phoneCodError").html(MSG['codeNull']);
		code_bol = false;
		return false;
	}
	var path = getRootPath();
	$.post(path + '/center/checkTelCode.do', {
		code : code,
		tel:tel
	}, function(data) {
		if (data == "false") {
			document.getElementById("phoneCodError").style.color = "red";
			$("#phoneCodError").html(MSG['codeError']);
			code_bol = false;
			return false;
		} else {
			$("#phoneCodError").html("");
			document.getElementById("phoneCodError").style.color = "green";
			code_bol = true;
			return true;
		}
	}, "text")
}

function checkAddress(){
	var address = $("#address").val();
	var unLen = address.length;
	var len=Number(unLen);
	if (len == 0) {
		$("#addressError").html(MSG['withdrawNull']);
		$("#addressError").show();
		return false;
	} else if ((len < 27 || len > 34) && len != 42) {
		 $("#addressError").html(MSG['addressError']);
		 $("#addressError").show();
		 return false;
	}
	
	var reg = /^[A-Za-z0-9]+$/;
	if (!reg.test(address)) {
		$("#addressError").html(MSG['addressError']);
		$("#addressError").show();
		return false;
	}
	
	$("#addressError").html("");
	$("#addressError").hide();
	return true;
}

function checkAll() {
	var address = $("#address").val();
	var unLen = address.length;
	if (unLen == 0) {
		$("#addressError").html(MSG['withdrawNull']);
		$("#addressError").show();
		return false;
	} else if ((unLen < 27 || unLen > 34) && unLen != 42) {
		 $("#addressError").html(MSG['addressError']);
		 $("#addressError").show();
		 return false;
	}
	
	var reg = /^[A-Za-z0-9]+$/;
	if (!reg.test(address)) {
		$("#addressError").html(MSG['addressError']);
		$("#addressError").show();
		return false;
	}
	
	var label = $("#label").val();
	var unLen = label.length;
	if (unLen == 0) {
		$("#addressError").html(MSG['labelNull']);
		$("#addressError").show();
		return false;
	}
	if(code_bol == false) return;
	
	$("#addressError").html("");
	$("#addressError").hide();
	return true;
}

function changesure(id,address,name) {	
	$("#addressForm").show();
	$("#address").attr("value",address);
	$("#label").attr("value",name);
	$("#addid").attr("value",id);
}

function preventDown() {
	window.event.preventDefault();
}

function selectWithdrawAddress(address) {
	$("#withdrawaddress").val(address);
	$("#withdrawaddress").blur();
}

function deleteWithdrawAddress(sender, addressId) {
	$.post(basePath+'center/deleteAddress.do',{ id: addressId },function(data){
		if (data.code == 0) {
			$(sender).parent().remove();
		} else {

		}
	},"json");
}

function addWithdrawAddredd() {
	if (!checkAll()) return;
	
	$.post(basePath+'center/withdrawAddress.do',$('#addressForm').serialize(),function(data){
		if (data.code == 0) {
			var times = 2;
			$("#addressError").html(data.msg + "(2s)");
			$("#addressError").css("color", "green");
			$("#addressError").show();
			
			setInterval(function() {
				times--;
				$("#addressError").html(data.msg + "(" + times + "s)");
				if (times == 0) {
					window.location.reload();
				}
			}, 1000);
			
		} else {
			$("#addressError").html(data.msg);
			$("#addressError").show();
		}
	},"json");
	
	return false;
}