//获取当前跟路径
function getRootPath() {
	return basePath;
}
var tel_bol = false;
var code_bol = false;
var email_bol = false;
var goog_bol = false;
var email_bind_bol = false;
var email_change_bol = false;
var email_new_bol = false;
var email_code_bol = false;
// 手机号
function checkTel(isNew) {
	var tel = $("#mobile" + (isNew ? "new":"")).val();
	// var phone = /^1[0-9]{10}$/;
	var phone = /^[0-9]*$/;
	if (tel != "") {
		if (!phone.test(tel)) {
			if (isNew) {
				$("#mobileError").html(MSG['phoneError']);
				document.getElementById("mobileError").style.color = "#E04444";
			} else {
				$("#error").html(MSG['phoneError']);
				document.getElementById("error").style.color = "#E04444";
			}
			
			tel_bol = false;
			return false;
		} else {
			if (isNew) {
				document.getElementById("mobileError").style.color = "green";
				$("#mobileError").html("");
			} else {
				document.getElementById("error").style.color = "green";
				$("#error").html("");
			}
			tel_bol = true;
			return true;
		}
	} else {
		if (isNew) {
			$("#mobileError").html(MSG['phoneNull']);
			document.getElementById("mobileError").style.color = "#E04444";
		} else {
			$("#error").html(MSG['phoneNull']);
			document.getElementById("error").style.color = "#E04444";
		}
		tel_bol = false;
		return false;
	}
}
/*
// 验证google验证码
function checkGoogle() {
	var code = $("#google").val();
	//var secretKey = $("#secretKey").val();
	var unLen = code.replace(/[^\x00-\xff]/g, "**").length;
	if (unLen == 0) {
		document.getElementById("googleerror").style.color = "#E04444";
		$("#googleerror").html("验证码不能为空");
		code_bol = false;
		return false;
	}
	var path = getRootPath();
	$.post(path + 'center/checkGoogle.do', {
		code : code
	}, function(data) {
		if (data == "false") {
			document.getElementById("googleerror").style.color = "#E04444";
			$("#googleerror").html("请输入正确的验证码");
			code_bol = false;
			return false;
		} else {
			$("#googleerror").html("");
			document.getElementById("googleerror").style.color = "green";
			code_bol = true;
			return true;
		}
	}, "text")
}
*/
// 验证手机验证码
function checkCode(old) {
	var tel = $("#mobile").val();
	var code = $("#code").val();
	var zone =$("#areaCode").val();
	var unLen = code.replace(/[^\x00-\xff]/g, "**").length;
	if (unLen == 0) {
		document.getElementById("error1").style.color = "#E04444";
		$("#error1").html(MSG['codeNull']);
		code_bol = false;
		return false;
	}
	var path = getRootPath();
	$.post(path + 'center/checkTelCode.do', {code:code,tel:tel,zone: old ? "" : zone}, function(data) {
		if (data == "false") {
			document.getElementById("error1").style.color = "#E04444";
			$("#error1").html(MSG['codeError']);
			code_bol = false;
			return false;
		} else {
			$("#error1").html("");
			document.getElementById("error1").style.color = "green";
			code_bol = true;
			return true;
		}
	}, "text")
}
//修改手机号--检查新输入的手机号
function checkCodeNew() {
	var code = $("#codenew").val();	
	var tel = $("#mobilenew").val();
	var zone =$("#areaCode").val();
	var unLen = code.replace(/[^\x00-\xff]/g, "**").length;
	if (unLen == 0) {
		document.getElementById("errornew").style.color = "#E04444";
		$("#errornew").html(MSG['codeNull']);
		code_bol = false;
		return false;
	}
	var path = getRootPath();
	$.post(path + 'center/checkTelCode.do', {code:code,tel:tel,zone:zone}, function(data) {
		if (data == "false") {
			document.getElementById("errornew").style.color = "#E04444";
			$("#errornew").html(MSG['codeError']);
			code_bol = false;
			return false;
		} else {
			$("#errornew").html("");
			document.getElementById("errornew").style.color = "green";
			code_bol = true;
			return true;
		}
	}, "text")
}

function checkEmailCode() {
	var code = $("#emailCode").val();
	var unLen = code.replace(/[^\x00-\xff]/g, "**").length;
	if (unLen == 0) {
		document.getElementById("error2").style.color = "#E04444";
		$("#error2").html(MSG['codeNull']);
		email_bol = false;
		return false;
	}
	
	$("#error2").html("");
	document.getElementById("error2").style.color = "green";
	
	email_bol = true;
	return true;
}

function checkGoogle() {
	var code = $("#google").val();
	var unLen = code.replace(/[^\x00-\xff]/g, "**").length;
	if (unLen == 0) {
		document.getElementById("googleerror").style.color = "#E04444";
		$("#googleerror").html(MSG['codeNull']);
		goog_bol = false;
		return false;
	}
	
	$("#googleerror").html("");
	document.getElementById("googleerror").style.color = "green";
	
	goog_bol = true;
	return true;
}

function checkEmailGoog() {
	checkEmailCode();
	checkGoogle();
	return (email_bol && goog_bol);
}

function checkCodeGoog() {
	checkGoogle();
	return (code_bol && goog_bol);
}

function checkEmailFlag() {
	if (tel_bol && code_bol && email_bol) {
		return true;
	}
	return false;
}

function checkFlag() {
	if (tel_bol && code_bol) {
		return true;
	}
	return false;
}

//检查输入的邮箱
function checkbindEmail(){
	var Email = $('#bindEmail').val();
	if(Email == ""){
		$("#bindError").html(MSG['emailNull']);
		document.getElementById("bindError").style.color = "#E04444";
		$("#bindEmail").css("border","1px solid #E04444");
		email_bind_bol = false;
		return false;
	}
	var reg = /^([a-zA-Z0-9_-]+(\.?))+([a-zA-Z0-9_-]+(\.?))+([a-zA-Z0-9_-])+@([a-zA-Z0-9_-])+((\.[a-zA-Z0-9_-]{2,3}){1,2})$/;
	if(!reg.test(Email)){
		$("#bindError").html(MSG['emailError']);
		document.getElementById("bindError").style.color = "#E04444";
		$("#bindEmail").css("border","1px solid #E04444");
		email_bind_bol = false;
		return false;
	}else{
		if ($('#login_but_submit').val() == MSG['login']) {
			$("#bindError").html("");
			$("#bindEmail").css("border","1px solid #F1F3F4");
			email_bind_bol = true;
			return true;
		}

		$("#bindError").html("");
		$("#bindEmail").css("border","1px solid #F1F3F4");
		$("#bindEmail:focus+label").css('transform', 'translateY(-10px) scale(.85)');
		email_bind_bol = true;
		return true;
	}
}

//检查邮箱验证码
function checkSandEmailCode() {
	var email = $('#bindEmail').val();
	var code = $("#emailCode").val();
	var unLen = code.replace(/[^\x00-\xff]/g, "**").length;
	if (unLen == 0) {
		document.getElementById("error2").style.color = "#E04444";
		$("#error2").html(MSG['codeNull']);
		email_bol = false;
		return false;
	}
	var path = getRootPath();
	$.post(path + 'center/checkEmailCode.do', {code:code,email:email}, function(data) {
		if (data == 'true') {
			$("#error2").html("");
			document.getElementById("error2").style.color = "green";
			email_bol = true;
			return true;
		} else {
			document.getElementById("error2").style.color = "#E04444";
			$("#error2").html(MSG['codeNull']);
			email_bol = false;
			return false;
		}
	})
}
//检查旧邮箱验证码
function changeSandsEmailCode() {
	var code = $("#emailCode").val();
	var unLen = code.replace(/[^\x00-\xff]/g, "**").length;
	if (unLen == 0) {
		document.getElementById("emailCodeError2").style.color = "#E04444";
		$("#emailCodeError2").html(MSG['codeNull']);
		$("#emailCode").css("border","1px solid #E04444");
		email_code_bol = false;
		return false;
	}
	var path = getRootPath();
	$.post(path + 'center/checkEmailCode.do', {code:code}, function(data) {
		if (data == 'true') {
			$("#emailCodeError2").html("");
			document.getElementById("emailCodeError2").style.color = "green";
			$("#emailCode").css("border","1px solid #f1f3f4");
			email_code_bol = true;
			return true;
		} else {
			document.getElementById("emailCodeError2").style.color = "#E04444";
			$("#emailCodeError2").html(MSG['codeError']);
			email_code_bol = false;
			return false;
		}
	})
}

//检查输入新邮箱
function changeBindEmail(){
	var Email = $('#emailNew').val();
	if(Email == ""){
		$("#emailNewError").html(MSG['emailNull']);
		document.getElementById("emailNewError").style.color = "#E04444";
		$("#emailNew").css("border","1px solid #E04444");
		email_change_bol = false;
		return false;
	}
	var reg = /^([a-zA-Z0-9_-]+(\.?))+([a-zA-Z0-9_-]+(\.?))+([a-zA-Z0-9_-])+@([a-zA-Z0-9_-])+((\.[a-zA-Z0-9_-]{2,3}){1,2})$/;
	if(!reg.test(Email)){
		$("#emailNewError").html(MSG['emailError']);
		document.getElementById("emailNewError").style.color = "#E04444";
		$("#emailNew").css("border","1px solid #E04444");
		email_change_bol = false;
		return false;
	}else{
		if ($('#login_but_submit').val() == MSG['login']) {
			$("#emailError").html("");
			$("#emailNew").css("border","1px solid #F1F3F4");
			email_change_bol = true;
			return true;
		}

		$("#emailNewError").html("");
		$("#emailNew").css("border","1px solid #F1F3F4");
		$("#emailNew:focus+label").css('transform', 'translateY(-10px) scale(.85)');
		email_change_bol = true;
		return true;
	}
}

//检查新邮箱验证码
function changeSandEmailCode() {
	var email = $('#emailNew').val();
	var code = $("#changeCodeNew").val();
	var unLen = code.replace(/[^\x00-\xff]/g, "**").length;
	if (unLen == 0) {
		document.getElementById("changeErrorNew").style.color = "#E04444";
		$("#changeErrorNew").html(MSG['codeNull']);
		email_new_bol = false;
		return false;
	}
	var path = getRootPath();
	$.post(path + 'center/checkEmailCode.do', {code:code,email:email}, function(data) {
		if (data == 'true') {
			$("#changeErrorNew").html("");
			document.getElementById("changeErrorNew").style.color = "green";
			email_new_bol = true;
			return true;
		} else {
			document.getElementById("changeErrorNew").style.color = "#E04444";
			$("#changeErrorNew").html(MSG['codeError']);
			email_new_bol = false;
			return false;
		}
	})
}