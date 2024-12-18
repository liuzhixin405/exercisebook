var email_bol=false;
var password_bol = false;
var password2_bol = false;
var password1_bol=false;

// 获取当前跟路径
function getRootPath(){
	return basePath;
}

//显示子账号弹框
function showRegsubAccount() {
	var regsubAccountAlert = $("#regsubAccount-alert");
	var dpzz = $("#tb-overlay-duiping");
	show(regsubAccountAlert);
	dpzz.show();
}

//隐藏子账号弹框
function closeRegsubAccount() {
	var regsubAccountAlert = $("#regsubAccount-alert");
	var dpzz = $("#tb-overlay-duiping");
	hide(regsubAccountAlert);
	dpzz.hide();
}
//备注选中
/*function addActive() {
	$(".edit-remark").addClass('edit-remark-active');
}
*/
//显示重置密码框
function resetPasswords(subUserId) {
	$("#sub-reset-pwd").val(subUserId);
	var resetPasswordsAlert = $("#resetPasswords-alert");
	var dpzz = $("#tb-overlay-duiping");
	show(resetPasswordsAlert);
	dpzz.show();
}
//隐藏重置密码框
function closeResetPasswords() {
	var resetPasswordsAlert = $("#resetPasswords-alert");
	var dpzz = $("#tb-overlay-duiping");
	hide(resetPasswordsAlert);
	dpzz.hide();
}

function delSubAccount(subUserId, subUserName) {
	$("#del-sub-id").val(subUserId);
	$("#user-account-id").html(subUserName);
	$("#delSubAccount").show();
	$("#black_overlay").show();
}

function closeDelSubAccount() {
	$("#delSubAccount").hide();
	$("#black_overlay").hide();
}

function fundsTtransfer(subUserId, subUserName) {
	$("#sub-tranfer-id").val(subUserId);
	$("#sub-tranfer-name").val(subUserName);
	$("#fundsTtransfer").show();
	$("#black_overlay").show();
	getAmount();
}

function closeFundsTtransfer() {
	$("#fundsTtransfer").hide();
	$("#black_overlay").hide();
}

//显示冻结子账户确认弹框
function freezeSubAccount() {
	$("#freezeSubAccount").show();
	$("#black_overlay").show();
}
//隐藏冻结子账户确认弹框
function closefreezeSubAccount() {
	$("#freezeSubAccount").hide();
	$("#black_overlay").hide();
}
//解冻子账户
function unfreezeSubAccount() {

}

//显示隐藏密码
var a = $("#loginPassword").val();
$("#showPsw").toggle(
		function() {
			$(this).attr('src', '../common/images/close-eyes.png');
			$("#loginPassword").replaceWith(`<input class="loginPassword" value="" type="text" name="loginPassword" id="loginPassword" placeholder="请输入登录密码">`);
//			 $("#loginPassword").attr("type", "text");
		},
		function() {
			$(this).attr('src', '../common/images/open-eyes.png');
			$("#loginPassword").replaceWith(`<input class="loginPassword" value="" type="password" name="loginPassword" id="loginPassword" placeholder="请输入登录密码">`);
//			$("#loginPassword").attr("type", "password");
		}
)

//批量增加子账号列表
function addSubaccountList() {
	 var $newoption=$("#subAccount-allpage-box").append("<option value='"+mytext+"'>"+mytext+"</option>");
}

function getByteLen(val) {
    var len = 0;
    for (var i = 0; i < val.length; i++) {
         var patt = new RegExp(/[^\x00-\xff]/ig);
         var a = val[i];
         if (patt.test(a)) 
        {
            len += 2;
        }
        else
        {
            len += 1;
        }
    }
    return len;
}

//验证登录名
function checkLoginName() {
	var loginName = $.trim($('#email').val());
/*	var regx = new RegExp("[`~!@#$^&*()=|{}':;',\\[\\].<>/?~！@#￥……&*（）&;—|{}【】‘；：”“'。，、？]");*/
	var regx = new RegExp("^[a-zA-Z][_a-zA-Z0-9]{4,32}$");
	var lengsChar = getByteLen(loginName);
	var unLen = loginName.length;
	if (loginName == "" || unLen == 0 ) {
		$("#loginNameError").html(MSG['emailNull']);
		document.getElementById("loginNameError").style.color = "#E04444";
		$("#email").css("border","1px solid #E04444");
		email_bol = false;
		return false;
	}
	if (lengsChar < 4 || lengsChar > 32) {
		$("#loginNameError").html(MSG['nickNameRule']);
		document.getElementById("loginNameError").style.color = "#E04444";
		$("#email").css("border","1px solid #E04444");
		email_bol = false;
		return false;
	}
	if (!regx.test(loginName)) {
		$("#loginNameError").html(MSG['hasnoOK']);
		document.getElementById("loginNameError").style.color = "#E04444";
		$("#email").css("border","1px solid #E04444");
		email_bol = false;
		return false;
	}
	email_bol = true;
	return true;
}

//检查密码
function checkPassword() {
	var password = $("#password").val();
	var unLen = password.length;
	if (unLen == 0) {
		document.getElementById("error2").style.color = "#E04444";
		$("#password").css("border","1px solid #E04444");
		$("#error2").html(MSG['pwdNull']);
		password_bol = false;
		return false;
	}
	if (unLen < 8) {
		document.getElementById("error2").style.color = "#E04444";
		$("#password").css("border","1px solid #E04444");
		$("#error2").html(MSG['pwdLtEightChar']);
		password_bol = false;
		return false;
	}
	if (unLen > 20) {
		document.getElementById("error2").style.color = "#E04444";
		$("#password").css("border","1px solid #E04444");
		$("#error2").html(MSG['pwdGtTwentyChar']);
		password_bol = false;
		return false;
	}
	var repassword = $("#password2").val();
	if (repassword != "") {
		if (password != repassword) {
			document.getElementById("error3").style.color = "#E04444";
			$("#password2").css("border","1px solid #E04444");
			$("#error3").html(MSG['twicePwdNotEq']);
			password2_bol = false;
			return false;
		} else {
			document.getElementById("error3").style.color = "green";
			$("#password2").css("border","1px solid #F1F3F4");
			$("#error3").html(MSG['pwdAvailable']);
			password2_bol = true;
		}
	}
	document.getElementById("error2").style.color = "green";
	$("#password").css("border","1px solid #F1F3F4");
	$("#error2").html(MSG['pwdAvailable']);
	password_bol = true;
	return true;
}

//确认密码
function checkPassword2() {
	var password1 = $("#password").val();
	var password2 = $("#password2").val();
	
	if(password2 == ""){
		document.getElementById("error3").style.color = "#E04444";
		$("#password2").css("border","1px solid #E04444");
		$("#error3").html(MSG['confirmPwdNull']);
		password2_bol = false;
		return false;
	}
	if (password1 == password2) {
		document.getElementById("error3").style.color = "green";
		$("#password2").css("border","1px solid #F1F3F4");
		$("#error3").html(MSG['confirmPwdAvailable']);
		password2_bol = true;
		return true;
	} else {
		document.getElementById("error3").style.color = "#E04444";
		$("#password2").css("border","1px solid #E04444");
		$("#error3").html(MSG['twicePwdNotEq']);
		password2_bol = false;
		return false;
	}
}

//验证手机验证码
function checkCode() {
	var tel = $("#mobile").val();
	var code = $("#code").val();
	var unLen = code.replace(/[^\x00-\xff]/g, "**").length;
	if (unLen == 0) {
		document.getElementById("error1").style.color = "#E04444";
		$("#code").css("border","1px solid #E04444");
		$("#error1").html(MSG['codeNull']);
		return false;
	}
	var path = getRootPath();
	$.post(path + 'center/checkTelCode.do', {code:code,tel:tel}, function(data) {
		if (data == "false") {
			document.getElementById("error1").style.color = "#E04444";
			$("#code").css("border","1px solid #E04444");
			$("#error1").html(MSG['codeError']);
			return false;
		} else {
			$("#error1").html("");
			document.getElementById("error1").style.color = "green";
			$("#code").css("border","1px solid #F1F3F4");
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
		$("#google").css("border","1px solid #E04444");
		$("#googleerror").html(MSG['codeNull']);
		return false;
	}
	
	$("#googleerror").html("");
	document.getElementById("googleerror").style.color = "green";
	$("#google").css("border","1px solid #F1F3F4");
	
	return true;
}

//重置验证手机验证码
function checkCodereset() {
	var tel = $("#mobile").val();
	var code = $("#telCode").val();
	var unLen = code.replace(/[^\x00-\xff]/g, "**").length;
	if (unLen == 0) {
		document.getElementById("errorReset").style.color = "#E04444";
		$("#telCode").css("border","1px solid #E04444");
		$("#errorReset").html(MSG['codeNull']);
		return false;
	}
	var path = getRootPath();
	$.post(path + 'center/checkTelCode.do', {code:code,tel:tel}, function(data) {
		if (data == "false") {
			document.getElementById("errorReset").style.color = "#E04444";
			$("#telCode").css("border","1px solid #E04444");
			$("#errorReset").html(MSG['codeError']);
			return false;
		} else {
			$("#errorReset").html("");
			document.getElementById("errorReset").style.color = "green";
			$("#telCode").css("border","1px solid #F1F3F4");
			return true;
		}
	}, "text")
}

//重置检查谷歌验证码
function checkGooglereset() {
	var code = $("#googleCode").val();
	var unLen = code.replace(/[^\x00-\xff]/g, "**").length;
	if (unLen == 0) {
		document.getElementById("googleerrorReset").style.color = "#E04444";
		$("#googleCode").css("border","1px solid #E04444");
		$("#googleerrorReset").html(MSG['codeNull']);
		return false;
	}
	
	$("#googleerrorReset").html("");
	document.getElementById("googleerrorReset").style.color = "green";
	$("#googleCode").css("border","1px solid #F1F3F4");
	
	return true;
}


//表单信息验证
function checkFlag(){
	if(password_bol && password2_bol && email_bol){
		return true;
	}
	if(!email_bol){
		$("#loginNameError").html(MSG['emailBoxError']);
     document.getElementById("loginNameError").style.color = "#E04444";
     $("#email").css("border","1px solid #E04444");
	}else if(!password_bol){
		document.getElementById("error2").style.color = "#E04444";
		$("#password").css("border","1px solid #E04444");
		$("#error2").html(MSG['pwdLtEightChar']);
	}else if(!password2_bol){
		document.getElementById("error3").style.color = "#E04444";
		$("#password2").css("border","1px solid #E04444");
		$("#error3").html(MSG['twicePwdNotEq']);
	}
	
	return false;
}

function addSubUser() {
	if (!checkFlag()) return;
	$.post(basePath+'userSubAccount/subUserCreate.do',$('#regsubAccountForm').serialize(),function(data){
		if (data.code == 'success') {
			var times = 2;
			$("#regsubAccountError").html(data.msg + "(2s)");
			$("#regsubAccountError").css("color", "green");
			$("#regsubAccountError").show();
			
			setInterval(function() {
				times--;
				$("#regsubAccountError").html(data.msg + "(" + times + "s)");
				if (times == 0) {
					window.location.reload();
				}
			}, 1000);
			
		} else {
			$("#regsubAccountError").html(data.msg);
			$("#regsubAccountError").show();
		}
		
	},"json");
	
	return false;
}

//重置表单信息验证
function checkresatPw(){
	if(password1_bol && password2_bol){
		return true;
	}
	if(!password1_bol){
		document.getElementById("error2").style.color = "#E04444";
		$("#password").css("border","1px solid #E04444");
		$("#error2").html(MSG['pwdLtEightChar']);
	}else if(!password2_bol){
		document.getElementById("error3").style.color = "#E04444";
		$("#password2").css("border","1px solid #E04444");
		$("#error3").html(MSG['twicePwdNotEq']);
	}
	
	return false;
}

//检查新密码
function checknewPwd() {
	var password = $("#newpasswd1").val();
	var unLen = password.length;
	if (unLen == 0) {
		document.getElementById("error2").style.color = "#E04444";
		$("#password").css("border","1px solid #E04444");
		$("#error2").html(MSG['pwdNull']);
		password1_bol = false;
		return false;
	}
	if (unLen < 8) {
		document.getElementById("error2").style.color = "#E04444";
		$("#password").css("border","1px solid #E04444");
		$("#error2").html(MSG['pwdLtEightChar']);
		password1_bol = false;
		return false;
	}
	if (unLen > 20) {
		document.getElementById("error2").style.color = "#E04444";
		$("#password").css("border","1px solid #E04444");
		$("#error2").html(MSG['pwdGtTwentyChar']);
		password1_bol = false;
		return false;
	}
	var repassword = $("#newpasswd2").val();
	if (repassword != "") {
		if (password != repassword) {
			document.getElementById("error3").style.color = "#E04444";
			$("#password2").css("border","1px solid #E04444");
			$("#error3").html(MSG['twicePwdNotEq']);
			password1_bol = false;
//			return false;
		} else {
			document.getElementById("error3").style.color = "green";
			$("#password2").css("border","1px solid #F1F3F4");
			$("#error3").html(MSG['twicePwdEq']);
	
			document.getElementById("error2").style.color = "green";
			$("#password").css("border","1px solid #F1F3F4");
			$("#error2").html(MSG['pwdAvailable']);	
			password1_bol = true;	
//			return true;
		}
	}
	document.getElementById("error2").style.color = "green";
	$("#password").css("border","1px solid #F1F3F4");
	$("#error2").html(MSG['pwdAvailable']);
	password1_bol = true;
	return true;
}
//确认新密码
function checknewPwd2() {
	var password1 = $("#newpasswd1").val();
	var password2 = $("#newpasswd2").val();
	
	if(password2 == ""){
		document.getElementById("error3").style.color = "#E04444";
		$("#password2").css("border","1px solid #E04444");
		$("#error3").html(MSG['confirmPwdNull']);
		password2_bol = false;
		return false;
	}
	if (password1 == password2) {
		document.getElementById("error3").style.color = "green";
		$("#password2").css("border","1px solid #F1F3F4");
		$("#error3").html(MSG['confirmPwdAvailable']);
		password2_bol = true;
		return true;
	} else {
		document.getElementById("error3").style.color = "#E04444";
		$("#password2").css("border","1px solid #E04444");
		$("#error3").html(MSG['twicePwdNotEq']);
		password2_bol = false;
		return false;
	}
}

function resetPasswordsSubUser() {
	if (!checkresatPw()) return;
	$.post(basePath+'userSubAccount/resetPwd.do',$('#resetPasswordsForm').serialize(),function(data){
		if (data.code == 'success') {
			var times = 2;
			$("#resetPasswordsError").html(data.msg + "(2s)");
			$("#resetPasswordsError").css("color", "green");
			$("#resetPasswordsError").show();
			
			setInterval(function() {
				times--;
				$("#resetPasswordsError").html(data.msg + "(" + times + "s)");
				if (times == 0) {
					window.location.reload();
				}
			}, 1000);
			
			
		} else {
			$("#resetPasswordsError").html(data.msg);
			$("#resetPasswordsError").show();
		}
		
	},"json");
	
	return false;
}

//删除
function deleteSubUser() {
	var subUserId = $("#del-sub-id").val();
	$.post(basePath+"userSubAccount/deleteSubUser.do", {subUserId: subUserId}, function(data){
		data = JSON.parse(data);
		if(data.code == "success"){
			closeDelSubAccount();
			tipAlertSub(data.msg, true);
		} else {
			closeDelSubAccount();
			tipAlertSub(data.msg, false);
		}
	}, "text");
}

//冻结&解冻操作
function updateFreeze(subUserId) {
	$.post(basePath+"userSubAccount/updateFreeze.do", {subUserId: subUserId}, function(data){
		data = JSON.parse(data);
		if(data.code == "success"){
			tipAlertSub(data.msg, true);
		} else {
			tipAlertSub(data.msg, true);
		}
	}, "text");
}

//切换母子账户
$(function(){
	var dir = 1;
	$("#sub-tranfer-direction").val(dir);
	$("#changeTtransfer").toggle(
		function() {
			if (dir == 1) {
				dir = 2;
			}
			$(this).attr('src', '../common/images/transfer-active.png');
			$(".fundsTtransfer-l").show();
			$(".fundsTtransfer-s").hide();
			$("#sub-tranfer-direction").val(dir);
			getAmount();
		},
		function() {
			if (dir == 2) {
				dir = 1;
			}
			$(this).attr('src', '../common/images/transfer-normal.png');
			$(".fundsTtransfer-l").hide();
			$(".fundsTtransfer-s").show();
			$("#sub-tranfer-direction").val(dir);
			getAmount();
		}
	)
})

function getAmount() {
	$.post(basePath+'userSubAccount/getAmount.do',$('#fundsTtransferForm').serialize(),function(data){
		if (data.code == 'success') {
			var balance = data.frozen;
			$("#sub-balance").html(balance);
		} else {
		}
		
	},"json");
	
	return false;
}
function tranferAll() {
	$.post(basePath+'userSubAccount/getAmount.do',$('#fundsTtransferForm').serialize(),function(data){
		if (data.code == 'success') {
			var balance = data.frozen;
			$("#amount").val(balance);
		} else {
		}
		
	},"json");
	
	return false;
}
//备注
var oldNoteVal;
function addactive(index) {
	$("#edit-remark" + index).addClass('edit-remark-active');
	oldNoteVal = $("#edit-remark" + index).val();
}
//备注选中
function addNormal(subUserId, index) {
	$("#edit-remark" + index).removeClass('edit-remark-active')
	var note = $("#edit-remark" + index).val();
	if (note !== oldNoteVal) {
		$.post(basePath+"userSubAccount/updateSubNote.do", {subUserId: subUserId, note: note}, function(data){
			data = JSON.parse(data);
		}, "text");
	} else {
		return;
	}

}

//提示框
function tipAlertSub(msg, needReload) {
	$("#alert-windows").show();
	var insidehtm = "<div id='tb-overlay-duiping' class='tb-overlay'></div><div class='add-wrap alert-wrap' style='display: block;' id='invite-BFX'>"
				+"<div class='dialogs-body' style='padding: 50px 35px 35px 35px;'>"
				+"<p style='text-align: center; font-size: 16px; margin-bottom: 30px;'>"+ msg +"</p>"
				+"<div class='form-group-btn text-center'>"
					+"<a class='btn sub-btn center' style='margin-right: 0px;padding: 11px 41px;' href='javascript:closealertSub("+ needReload +");'>"+MSG['sureTip']+"</a>"
				+"</div>"
				+"</div>"
			+"</div>";
	$("#alert-windows").html(insidehtm);
	center($('.alert-wrap'));
}

function closealertSub(needReload) {
	$("#alert-windows").hide();
	if (needReload) {
		window.location.reload();
	}
}
//资金划转
function fundsTtransferSubUser() {
	if ($("#amount") == null) return;
	$.post(basePath+'userSubAccount/transfer.do',$('#fundsTtransferForm').serialize(),function(data){
		if (data.code == 'success') {
			var times = 2;
			$("#fundsTtransferError").html(data.msg + "(2s)");
			$("#fundsTtransferError").css("color", "green");
			$("#fundsTtransferError").show();
			
			setInterval(function() {
				times--;
				$("#fundsTtransferError").html(data.msg + "(" + times + "s)");
				if (times == 0) {
					window.location.reload();
				}
			}, 1000);
			
		} else {
			$("#fundsTtransferError").html(data.msg);
			$("#fundsTtransferError").show();
		}
		
	},"json");
	
	return false;
}
