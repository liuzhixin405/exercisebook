var bol_login_email = false;
var bol_login_password = false;

function checkTelOrEmail(){
	var tel = $("#email" + (false ? "new":"")).val();
	var phoneNum = /^[0-9]*$/;
//	alert(phoneNum.test(tel));
	if (phoneNum.test(tel)) {
		checkTelForEmail(false);
	} else {
		checkEmail();
	}
}

// 验证邮箱
function checkLoginEmailNoAjax() {
	var Email = $('#email').val();
	if (Email == "") {
		showErrorType(1, MSG['emailNull']);
		bol_login_email = false;
		return false;
	}
	//var reg = /^([a-zA-Z0-9_-])+@([a-zA-Z0-9_-])+((\.[a-zA-Z0-9_-]{2,3}){1,2})$/;
//	var reg = /^([a-zA-Z0-9_-])+(\.{0,1})+([a-zA-Z0-9_-])+@([a-zA-Z0-9_-])+((\.[a-zA-Z0-9_-]{2,3}){1,2})$/;
	var reg = /^([a-zA-Z0-9_-]+(\.?))+([a-zA-Z0-9_-]+(\.?))+([a-zA-Z0-9_-])+@([a-zA-Z0-9_-])+((\.[a-zA-Z0-9_-]{2,3}){1,2})$/;
	
	if (!reg.test(Email)) {
		showErrorType(1, MSG['emailError']);
		bol_login_email = false;
		return false;
	} else {
		bol_login_email = true;
		showErrorType(1, "");
		return true;
	}
}
//验证登录密码
function checkLoginPasswordNoAjax() {
	var password = $("#password").val();
	var unLen = password.length;
	if (unLen == 0) {
		showErrorType(2, MSG['pwdNull']);
		bol_login_password = false;
		return false;
	}
	if (unLen < 8) {
		showErrorType(2, MSG['pwdLtEightChar']);
		bol_login_password = false;
		return false;
	}
	if (unLen > 20) {
		showErrorType(2, MSG['pwdGtTwentyChar']);
		bol_login_password = false;
		return false;
	}
	showErrorType(2, "");
	bol_login_password = true;
	return true;
}

function showErrorType(type, msg){
	if (type == 1) {
		if ( msg != "") {
			$("#email").removeClass('error-style');
			$("#email").addClass('error-style');
		}else {
			$("#email").removeClass('error-style');
		}
	} else if (type == 2) {
		if ( msg != "") {
			$("#password").removeClass('error-style');
			$("#password").addClass('error-style');
		}else {
			$("#password").removeClass('error-style');
		}
	}
}

function checkIndexNoAjax(){
	if(bol_login_email&&bol_login_password){
		return true;
	}
	if(!bol_login_email){
		checkLoginEmailNoAjax();
	}
	if(!bol_login_password){
		checkLoginPasswordNoAjax();
	}
	if(bol_login_email&&bol_login_password){
		return true;
	}
	return false;
}


function login() {
	if($("#email").val()==""){
		seterror(-2);
		$("#email").focus();
		return false;
	}
	if($("#password").val()==""){
		seterror(-2);
		$("#password").focus();
		return false;
	}
	if($("#div_yzm").css("display") != 'none' && $("#validcode").val()==""){
		seterror(-3);
		$("#validcode").focus();
		return false;
	}
	if($("#email").val()!="" && $("#password").val()!="") {
		return true;
	}
	return false;
}

//子账号
//function loginSubEmail(){
//	if(checkSubEmail()){
//		$.post(basePath+'login/loginTime.do',$('#loginform').serialize(),function(data){
//			if(data== "true"){
//				document.getElementById("div_yzm").style.display="block";
//				$("#emailerror").html("");
//			}else{
//				document.getElementById("div_yzm").style.display="none";
//				$("#emailerror").html("");
//				return false;
//			}
//		},"text");
//	}
//}
//子账号验证不为空
//function checkSubEmail(){
//	var Email = $('#email').val();
//	if(Email == ""){
//		$("#emailerror").html(MSG['subEmailNull']);
//		  document.getElementById("emailerror").style.color = "red";
//		email_bol = false;
//		return false;			
//	}
//	return true;
//}


function seterror(tsy) {
	$(".warning").remove();
	var vhtml = '<div class="warning">';
	var vhtml2 = '';
	if (tsy == 4) {
		vhtml2 = MSG['emailOrPwdError'];
	} else if (tsy == -1) {
		vhtml2 = MSG['accountFfrozen'];
	}else if (tsy == -2) {
		vhtml2 = MSG['emailOrPwdNull'];
	}else if (tsy == -3){
		vhtml2 = MSG['codeNull'];
	}else if (tsy == -4){
		vhtml2 = MSG['codeError'];
	} else
		vhtml2 = MSG['emailOrPwdError'];
	var vhtml3 = '</div>';
	document.getElementById("loginheader").innerHTML = vhtml + vhtml2 + vhtml3;
	
		
    src = basePath+"Captcha/CaptchaAction.do?r=" + new Date().getTime();
    
	$("#imgValidcode").attr("src",src);
}	

function changeTransaction(name) {
	var transactionCode = name ;
	if (transactionCode == null || transactionCode == "") {
		return;
	} else {
		document.getElementById("transaction_code").value=transactionCode;
		$.ajax({
			url:basePath+ "home/homedo.do?transactionCode="+transactionCode,
			type : "POST",
			dataType : "JSON",
			async: true,
			success : function(result) {
			    var data=eval("(" + result + ")");
				$("#ask").empty();
				var asks = data.listbuy;
				var bids = data.listsell;
				var litr = data.listtrade;
				var html=null;
				$.each(asks, function(count,val){
	              html = '<tr>' 

	                               + '</tr><tr>' + bq

	                               + '</tr>';
				
				
				})
		 		$('#ask').html(html);
			}
		}) 
	}
}