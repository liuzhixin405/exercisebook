var email_bol=false;
var password1_bol=false;
var password2_bol=false;
var password_bol=false;
// 获取当前跟路径
function getRootPath(){
	return basePath;
}

function showError(msg){
	$("#error").html(msg);
	if(msg!="")
		$("#error").css("display", "block");
	else
		$("#error").css("display", "none");
}

//检查交易密码
function checkTradePassword() {
	var Email = $('#email').val();
	if(Email == ""){
		$("#emailerror").html(MSG['emailNull']);
		email_bol = false;
		return false;			
	}
	var password = $("#password").val();
	var unLen = password.length;
	if (unLen == 0) {
		$("#passworderror").html(MSG['tradePwdNull']);
		password_bol = false;
		return false;
	}
	if (unLen < 8) {
		$("#passworderror").html(MSG['tradePwdLtEightChar']);
		password_bol = false;
		return false;
	}
	if (unLen > 20) {
		$("#passworderror").html(MSG['tradePwdGtTwentyChar']);
		password_bol = false;
		return false;
	}
	password_bol = true;
	return true;
}
//登录密码验证
function checkPassword() {
	var Email = $('#email').val();
	if(Email == ""){
		$("#emailerror").html(MSG['emailNull']);
		email_bol = false;
		return false;			
	}
	var password = $("#password").val();
	var unLen = password.length;
	if (unLen == 0) {
		showError(MSG['loginPwdNull']);
		password_bol = false;
		return false;
	}
	if (unLen < 8) {
		showError(MSG['loginPwdLtEightChar']);
		password_bol = false;
		return false;
	}
	if (unLen > 20) {
		showError(MSG['loginPwdGtTwentyChar']);
		password_bol = false;
		return false;
	}
	
	password_bol = true;
	return true;
}
// 检查密码
function checknewPwd() {
	var password = $("#newpasswd1").val();
	var unLen = password.length;
	if (unLen == 0) {
		document.getElementById("error2").style.color = "red";
		$("#error2").html(MSG['pwdNull']);
		password1_bol = false;
		return false;
	}
	if (unLen < 8) {
		document.getElementById("error2").style.color = "red";
		$("#error2").html(MSG['pwdLtEightChar']);
		password1_bol = false;
		return false;
	}
	if (unLen > 20) {
		document.getElementById("error2").style.color = "red";
		$("#error2").html(MSG['pwdGtTwentyChar']);
		password1_bol = false;
		return false;
	}
	var repassword = $("#newpasswd2").val();
	if (repassword != "") {
		if (password != repassword) {
			document.getElementById("error3").style.color = "red";
			$("#error3").html(MSG['twicePwdNotEq']);
			password1_bol = false;
//			return false;
		} else {
			document.getElementById("error3").style.color = "green";
			$("#error3").html(MSG['twicePwdEq']);
	
			document.getElementById("error2").style.color = "green";
			$("#error2").html(MSG['pwdAvailable']);	
			password1_bol = true;	
//			return true;
		}
	}
	document.getElementById("error2").style.color = "green";
	$("#error2").html(MSG['pwdAvailable']);
	password1_bol = true;
	return true;
}
// 确认密码
function checknewPwd2() {
	var password1 = $("#newpasswd1").val();
	var password2 = $("#newpasswd2").val();
	
	if(password2 == ""){
		document.getElementById("error3").style.color = "red";
		$("#error3").html(MSG['confirmPwdNull']);
		password2_bol = false;
		return false;
	}
	if (password1 == password2) {
		document.getElementById("error3").style.color = "green";
		$("#error3").html(MSG['confirmPwdAvailable']);
		password2_bol = true;
		return true;
	} else {
		document.getElementById("error3").style.color = "red";
		$("#error3").html(MSG['twicePwdNotEq']);
		password2_bol = false;
		return false;
	}
}
function checkPwd(){
	if(password2_bol&&password1_bol){
		return true;
	}
	return false;
}
function check(){
	if(email_bol){
		return true;
	}
	return false;
}
function checkPasswd(){
	if(password_bol&&password2_bol&&password1_bol){
		return true;
	}
	return false;
}
function checkTradePwd() {
	var password = $("#password").val();
	var unLen = password.length;
	if (unLen == 0) {
		document.getElementById("error3").style.color = "red";
		$("#error3").html(MSG['tradePwdNull']);
		password_bol = false;
		return false;
	}
	if (unLen < 8) {
		document.getElementById("error3").style.color = "red";
		$("#error3").html(MSG['tradePwdLtEightChar']);
		password_bol = false;
		return false;
	}
	if (unLen > 20) {
		document.getElementById("error3").style.color = "red";
		$("#error3").html(MSG['tradePwdGtTwentyChar']);
		password_bol = false;
		return false;
	}
	password_bol = true;
	return true;
}
var amount_bol=false;
function checkAmount() {
	var amount = $("#amount").val();
	var balance = $("#balance").val();
	var exp	=	/\d{1,8}\.{0,1}\d{0,4}/;
	if(!amount.match(exp)){
		document.getElementById("error2").style.color = "red";
		$("#error2").html(MSG['amountError']);
		amount_bol = false;
		return false;
	}
	var amo=Number(amount);
	if(isNaN(amo)){
		document.getElementById("error2").style.color = "red";
		$("#error2").html(MSG['amountError']);
		amount_bol = false;
		return false;
	}
	if(amo<10){
		document.getElementById("error2").style.color = "red";
		$("#error2").html(MSG['amountError']);
		amount_bol = false;
		return false;
	}
	var ban=Number(balance);
	if(amo>ban){
		document.getElementById("error2").style.color = "red";
		$("#error2").html(MSG['amountInsufficient']);
		amount_bol = false;
		return false;
	}
	document.getElementById("error2").style.color = "green";
	$("#error2").html("");
	amount_bol = true;
	return true;
}
function checkTradeUser(){
	if(password_bol&&amount_bol){
		return true;
	}
	return false;
}

function checkIndex(){
	if(password_bol&&email_bol){
		return true;
	}
	return false;
}
// 设置资金密码不输入
function changTradeNoInput(){
	var code = $("#code").val();
	var google = $("#google").val();
	var password = $("#password").val();
	
	var path=getRootPath();
	$.post(path+'password/changTradeInputNot.do',{code:code,google:google,password:password},function(data){
		if(data==MSG['setNoTradePwdSuccess']){
			window.location.reload();
		} else {
			$("#add-wrap-sfyz").hide();
			$("#tb-overlay-sfyz").hide();
			tipAlert(data);
			$("#user-alert").hide();
			  
			var alertWin = document.getElementById("alert-windows");
			alertWin.addEventListener("click", function(event) {
				window.location.reload();
			});
			
			$("#close-alert").click(function() {
				window.location.reload();
			});  
		}
	},"text")
}
// 设置资金密码输入
function changTradeInInput(){
	var state = 1;
	var path=getRootPath();
	$.post(path+'password/changTradeInputOrNot.do',{state:state},function(data){
		window.location.reload();
	},"text")
}

function checkAmountTibi(min,max,fee,name) {
	var amount = $("#amount").val();
	var balance = $("#balance").val();
	var exp	=	/\d{1,8}\.{0,1}\d{0,4}/;
	var amo=Number(amount);
	var ban=Number(balance);
	var currencyId = $("#coinId").val();
	
	if(!amount.match(exp)){
		document.getElementById("error2").style.color = "red";
		$("#error2").html(MSG['amountError']);
		amount_bol = false;
		return false;
	}
	
	if(isNaN(amo)){
		document.getElementById("error2").style.color = "red";
		$("#error2").html(MSG['amountError']);
		amount_bol = false;
		return false;
	}
	if(amo < min){
		document.getElementById("error2").style.color = "red";
		$("#error2").html(MSG['withdrawLeast'] + min + name);
		$("#amount").val("").focus().val(amount);
		return false;
	}
	if(amo > max){
		document.getElementById("error2").style.color = "red";
		$("#error2").html(MSG['withdrawMaximum'] + max + name);
		$("#amount").val("").focus().val(amount);
		return false;
	}
	if(amo <= 0){
		document.getElementById("error2").style.color = "red";
		$("#error2").html(MSG['notAm']);
		amount_bol = false;
		return false;
	}
	
	if(amo > ban){
		document.getElementById("error2").style.color = "red";
		$("#error2").html(MSG['notEnough']);
		amount_bol = false;
		return false;
	}
	//查询当日限额
	$.ajax({
        type: "POST",
        dataType: "json",         
        url: basePath + "center/checkDayMaxWithdraw.do",
        data: {"currencyId":currencyId, "amo" : amount},
        success: function(data) {
        	var dayWithdraw = Number(data.msg);
        	var frozen = Number(data.frozen);
        	var surplus = frozen - dayWithdraw;
        	
        	if(amo > surplus) {
        		document.getElementById("error2").style.color = "red";
        		$("#error2").html(MSG['dayMaxTips1'] + frozen + name + MSG['dayMaxTips2'] + surplus + name);
        		amount_bol = false;
        		return false;
        	}
        }
    });

	$("#error2").html("");
	amount_bol = true;
	return true;
}

//提示框
function tipAlertt(type) {
	$("#alert-windows").show();
	var insidehtm = "<div id='tb-overlay-duiping' class='tb-overlay'></div><div class='add-wrap alert-wrap' style='display: block;' id='invite-BFX'>"
				+"<div class='dialogs-title'>"
				+"<h2 class='lt'>"
					+MSG['warmTip']
				+"</h2>"
				+"<a class='close-icon icons-close' id='close-alert' href='javascript:closealertt()'></a>"
				+"</div>"
				+"<div class='dialogs-body'>"
				+"<p style='text-align: center; font-size: 16px; margin-bottom: 30px;'>"+ type +"</p>"
				+"<div class='form-group-btn text-center'>"
					+"<a class='btn sub-btn center' style='margin-right: 0px;' href='javascript:closealertt();'>"+MSG['sureTip']+"</a>"
				+"</div>"
				+"</div>"
			+"</div>";
	$("#alert-windows").html(insidehtm);
	center($('.alert-wrap'));
}

function closealertt() {
	$("#alert-windows").hide();
	$("#amount").focus();
}