var email_bol = false;
var password_bol = false;

// 获取当前跟路径
function getRootPath() {
	return basePath;
}

//显示隐藏密码
var a = 0;
function changeShowPwd(index) {
	a += 1;
//	console.log(a)
	if (a % 2 != 0) {
//		console.log(a % 2);
		$('#openPsw' + index).hide();
		$('#closePsw' + index).show();
		$('#pwd' + index)[0].type = "text";
	} else if (a % 2 == 0) {
		$('#closePsw' + index).hide();
		$('#openPsw' + index).show();
		$('#pwd' + index)[0].type = "password";
	}
}

//批量增加子账号列表
function addSubaccountList() {
	var i = $("#subAccount-allpage .subAccount-allpage-box").length;
	i++;
	var $newoption = $("#subAccount-allpage").append(
		"<div class='subAccount-allpage-box' id='subAccount-box" + i + "'>" +
		"<div class='subAccount-allpage-title'>" +
		"<h3>" + fmtsubAccount + '-' + i + "</h3>" +
		"<botton onclick='deleteCreatesub(" + i + ")' type='submit'>" + fmtdelete + "</botton>" +
		"</div>" +
		"<span class='loginPassword-item'>" +
		fmtSubaccountloginName + ':' + "<em class='blank-part3'>" + "</em>" +
		"<input name='email" + i + "' id='email" + i + "' autocomplete='off' onblur='checkLoginName(" + i + ")'" +
		" placeholder='" + fmtwriteSubloginName + "'>" +
		"<span class='error' id='emailError" + i + "'>" + "</span>" +
		"</span>" +
		"<span class='loginPassword-item'>" +
		fmtloginPwd + ':' + "<em class='blank-part4'>" + "</em>" +
		"<input class='loginPassword'" + "onblur='checkPassword(" + i + ")'" + "type='password' autocomplete='off' name='pwd" + i + "' id='pwd" + i + "' placeholder='" + fmtwriteloginPassword + "'>" +
		"<span class='error' id='pwdError" + i + "'>" + "</span>" +
		"<img class='showPsws' id='openPsw" + i + "' onclick='changeShowPwd(" + i + ")'" + " src='../common/images/open-eyes.png'>" +
		"<img class='showPsws pswnone' id='closePsw" + i + "' onclick='changeShowPwd(" + i + ")'" + " src='../common/images/close-eyes.png'>" +
		"</span>" +
		"</div>"
	);
}

function deleteCreatesub(index) {
	var b = $("#subAccount-allpage .subAccount-allpage-box").length;
	$("#subAccount-box" + index).remove();
	for (var i = index + 1; i <= b; i++) {
		$("#subAccount-box" + i).replaceWith(
			"<div class='subAccount-allpage-box' id='subAccount-box" + (i-1) + "'>" +
			"<div class='subAccount-allpage-title'>" +
			"<h3>" + fmtsubAccount + '-' + (i-1) + "</h3>" +
			"<botton onclick='deleteCreatesub(" + (i-1) + ")' type='submit'>" + fmtdelete + "</botton>" +
			"</div>" +
			"<span class='loginPassword-item'>" +
			fmtSubaccountloginName + ':' + "<em class='blank-part3'>" + "</em>" +
			"<input name='email" + (i-1) + "' id='email" + (i-1) + "' autocomplete='off' onblur='checkLoginName(" + (i-1) + ")'" +
			" placeholder='" + fmtwriteSubloginName + "'>" +
			"<span class='error' id='emailError" + (i-1) + "'>" + "</span>" +
			"</span>" +
			"<span class='loginPassword-item'>" +
			fmtloginPwd + ':' + "<em class='blank-part4'>" + "</em>" +
			"<input class='loginPassword'" + "onblur='checkPassword(" + (i-1) + ")'" + "type='password' autocomplete='off' name='pwd" + (i-1) + "' id='pwd" + (i-1) + "' placeholder='" + fmtwriteloginPassword + "'>" +
			"<span class='error' id='pwdError" + (i-1) + "'>" + "</span>" +
			"<img class='showPsws' id='openPsw" + (i-1) + "' onclick='changeShowPwd(" + (i-1) + ")'" + " src='../common/images/open-eyes.png'>" +
			"<img class='showPsws pswnone' id='closePsw" + (i-1) + "' onclick='changeShowPwd(" + (i-1) + ")'" + " src='../common/images/close-eyes.png'>" +
			"</span>" +
			"</div>"
		);
	}
}

function getByteLen(val) {
	var len = 0;
	for (var i = 0; i < val.length; i++) {
		var patt = new RegExp(/[^\x00-\xff]/ig);
		var a = val[i];
		if (patt.test(a)) {
			len += 2;
		} else {
			len += 1;
		}
	}
	return len;
}

//验证登录名
function checkLoginName(i) {
	var loginName = $.trim($('#email' + i).val());
	/*	var regx = new RegExp("[`~!@#$^&*()=|{}':;',\\[\\].<>/?~！@#￥……&*（）&;—|{}【】‘；：”“'。，、？]");*/
	var regx = new RegExp("^[a-zA-Z][_a-zA-Z0-9]{4,32}$");
	var lengsChar = getByteLen(loginName);
	var unLen = loginName.length;
	if (loginName == "" || unLen == 0) {
		$("#emailError" + i).html(MSG['emailNull']);
		$("#emailError" + i).css("color", "#E04444");
		email_bol = false;
		return false;
	}
	if (lengsChar < 4 || lengsChar > 32) {
		$("#emailError" + i).html(MSG['nickNameRule']);
		$("#emailError" + i).css("color", "#E04444");
		email_bol = false;
		return false;
	}
	
	if (!regx.test(loginName)) {
		$("#emailError" + i).html(MSG['hasnoOK']);
		document.getElementById("emailError" + i).css("color", "#E04444");
		email_bol = false;
		return false;
	}
	
	email_bol = true;
	return true;
}

//检查密码
function checkPassword(i) {
	var password = $("#pwd" + i).val();
	var unLen = password.length;
	if (unLen == 0) {
		$("#pwdError" + i).css("color", "#E04444");
		$("#pwdError" + i).html(MSG['pwdNull']);
		password_bol = false;
		return false;
	}
	if (unLen < 8) {
		$("#pwdError" + i).css("color", "#E04444");
		$("#pwdError" + i).html(MSG['pwdLtEightChar']);
		password_bol = false;
		return false;
	}
	if (unLen > 20) {
		$("#pwdError" + i).css("color", "#E04444");
		$("#pwdError" + i).html(MSG['pwdGtTwentyChar']);
		password_bol = false;
		return false;
	}
	$("#pwdError" + i).css("color", "green");
	$("#pwdError" + i).html(MSG['pwdAvailable']);
	password_bol = true;
	return true;
}


//验证手机验证码
function checkCode() {
	var tel = $("#mobile").val();
	var code = $("#code").val();
	var unLen = code.replace(/[^\x00-\xff]/g, "**").length;
	if (unLen == 0) {
		document.getElementById("error1").style.color = "#E04444";
		$("#error1").html(MSG['codeNull']);
		return false;
	}
	var path = getRootPath();
	$.post(path + 'center/checkTelCode.do', {
		code : code,
		tel : tel
	}, function(data) {
		if (data == "false") {
			document.getElementById("error1").style.color = "#E04444";
			$("#error1").html(MSG['codeError']);
			return false;
		} else {
			$("#error1").html("");
			document.getElementById("error1").style.color = "green";
			return true;
		}
	}, "text")
}

//检查谷歌验证码
function checkGoogle() {
	var code = $("#google").val();
	var unLen = code.replace(/[^\x00-\xff]/g, "**").length;
	if (unLen == 0) {
		document.getElementById("googleerror").style.color = "#E04444";
		$("#googleerror").html(MSG['codeNull']);
		return false;
	}

	$("#googleerror").html("");
	document.getElementById("googleerror").style.color = "green";

	return true;
}

function checkFlag() {
	if (password_bol && email_bol) {
		return true;
	}
	if (!email_bol) {
		$("#loginNameError").html(MSG['emailBoxError']);
		document.getElementById("loginNameError").style.color = "#E04444";
	} else if (!password_bol) {
		document.getElementById("error2").style.color = "#E04444";
		$("#error2").html(MSG['pwdLtEightChar']);
	}
	return false;
}

function addBatchSubUser() {
	if (!checkFlag()) return;
	var length = $("#subAccount-allpage .subAccount-allpage-box").length;
	var params = "";
	for (var i = 1; i < length + 1; i++) {
		var name = $.trim($('#email' + i).val());
		var pwd = $.trim($('#pwd' + i).val());
		params += "info=" + name + "," + pwd + "&";
	}
	var code = $("#code").val();
	code = "code=" + code;
	params += code;
	var google = $("#google").val();
	google = "&google=" + google;
	params += google;

	var url = basePath + "userSubAccount/subUserBatchCreate.do?" + params;
	$.ajax({
		type : 'POST',
		url : url,
		data : {},
		contentType : "application/json; charset=utf-8",
		dataType : "text",
		cache : false,
		statusCode : {
			404 : function() { // 404错误
			}
		},
		success : function(data) {
			data = JSON.parse(data);
			if (data.code == "success") {
				tipAlertSub(data.msg);
			} else {
				tipAlertSub(data.msg);
			}
		},
		error : function(data) {}
	});

	return false;
}

function tipAlertSub(type) {
	$("#alert-windows").show();
	var insidehtm = "<div id='tb-overlay-duiping' class='tb-overlay'></div><div class='add-wrap alert-wrap' style='display: block;' id='invite-BFX'>"
		+ "<div class='dialogs-title'>"
		+ "<h2 class='lt'>"
		+ MSG['warmTip']
		+ "</h2>"
		+ "<a class='close-icon icons-close' id='close-alert' href='javascript:closealertSub()'></a>"
		+ "</div>"
		+ "<div class='dialogs-body'>"
		+ "<p style='text-align: center; font-size: 16px; margin-bottom: 30px;'>" + type + "</p>"
		+ "<div class='form-group-btn text-center'>"
		+ "<a class='btn sub-btn center' style='margin-right: 0px;' href='javascript:closealertSub();'>" + MSG['sureTip'] + "</a>"
		+ "</div>"
		+ "</div>"
		+ "</div>";
	$("#alert-windows").html(insidehtm);
	center($('.alert-wrap'));
}

function closealertSub() {
	$("#alert-windows").hide();
	$(window).attr('location', basePath + "userSubAccount/subUser.do");
}