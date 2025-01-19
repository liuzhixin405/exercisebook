var email_bol=false;
var password_bol = false;
var password2_bol = false;
var validcode_bol = false;

//获取当前跟路径
function getRootPath(){
	return basePath;

}
//判断验证邮箱/手机号/子账号
function checkEmailOrTel(){
	var tel = $("#email" + (false ? "new":"")).val();
	var phone =/^1[0-9]{10}$/;
	var emailReg = /^([a-zA-Z0-9_-]+(\.?))+([a-zA-Z0-9_-]+(\.?))+([a-zA-Z0-9_-])+@([a-zA-Z0-9_-])+((\.[a-zA-Z0-9_-]{2,3}){1,2})$/;
	if (phone.test(tel)) {
		return checkTelForEmail(false);
	} else if(emailReg.test(tel)) {
		return checkEmail();
	} else {
		return checkLoginName();
	}
}

//手机号
function checkTelForEmail(isNew) {
	var tel = $("#email" + (isNew ? "new":"")).val();
	var phone = /^1[0-9]{10}$/;
	if (tel != "") {
		if (!phone.test(tel)) {
			if (isNew) {
				$("#mobileError").html(MSG['phoneError']);
				document.getElementById("mobileError").style.color = "#E04444";
				$("#email").css("border","1px solid #E04444");
			} else {
				$("#emailerror").html(MSG['phoneError']);
				document.getElementById("emailerror").style.color = "#E04444";
				$("#email").css("border","1px solid #E04444");
			}
			email_bol = false;
			return false;
		} else {
			if ($('#login_but_submit').val() == MSG['login']) {
//				email_bol = true;
				return true;
			}
			if (isNew) {
				$("#mobileError").html("");
				$("#email").css("border","1px solid #F1F3F4");
				document.getElementById("mobileError").style.color = "green";
			} else {
				$("#emailerror").html("");
				$("#email").css("border","1px solid #F1F3F4");
				document.getElementById("emailerror").style.color = "green";
			}
			email_bol = true;
			return true;
		}
	} else {
		if (isNew) {
			$("#mobileError").html(MSG['phoneNull']);
			document.getElementById("mobileError").style.color = "#E04444";
			$("#email").css("border","1px solid #E04444");
		} else {
			$("#emailerror").html(MSG['phoneNull']);
			document.getElementById("emailerror").style.color = "#E04444";
			$("#email").css("border","1px solid #E04444");
		}
		email_bol = false;
		return false;
	}
}

function checkPhone(){
	var Email = $('#email').val();
	if(Email == ""){
		$("#emailerror").html(MSG['phoneNull']);
		document.getElementById("emailerror").style.color = "#E04444";
		$("#email").css("border","1px solid #E04444");
		email_bol = false;
		return false;
	}
	// var reg = /^1[0-9]{10}$/;
	var reg = /^[0-9]*$/;
	if(!reg.test(Email)){
		$("#emailerror").html(MSG['phoneError']);
        document.getElementById("emailerror").style.color = "#E04444";
        $("#email").css("border","1px solid #E04444");
		email_bol = false;
		return false;
	}else{
		if ($('#login_but_submit').val() == MSG['login']) {
			$("#emailerror").html("");
			$("#email").css("border","1px solid #F1F3F4");
			email_bol = true;
			return true;
		}
		$("#emailerror").html("");
		$("#email").css("border","1px solid #F1F3F4");
		$("#email:focus+label").css('transform', 'translateY(-10px) scale(.85)');
		email_bol = true;
		return true;
	}
}

//验证邮箱
function checkEmail(){
	var Email = $('#email').val();
	if(Email == ""){
		$("#emailerror").html(MSG['emailNull']);
		document.getElementById("emailerror").style.color = "#E04444";
		$("#email").css("border","1px solid #E04444");
		email_bol = false;
		return false;
	}
	var reg = /^([a-zA-Z0-9_-]+(\.?))+([a-zA-Z0-9_-]+(\.?))+([a-zA-Z0-9_-])+@([a-zA-Z0-9_-])+((\.[a-zA-Z0-9_-]{2,3}){1,2})$/;
	if(!reg.test(Email)){
		$("#emailerror").html(MSG['emailError']);
        document.getElementById("emailerror").style.color = "#E04444";
        $("#email").css("border","1px solid #E04444");
		email_bol = false;
		return false;
	}else{
		if ($('#login_but_submit').val() == MSG['login']) {
			$("#emailerror").html("");
			$("#email").css("border","1px solid #F1F3F4");
			email_bol = true;
			return true;
		}
		
		$("#emailerror").html("");
		$("#email").css("border","1px solid #F1F3F4");
		$("#email:focus+label").css('transform', 'translateY(-10px) scale(.85)');
		email_bol = true;
		return true;
	}
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

//验证子账号登录名
function checkLoginName() {
	var loginName = $.trim($('#email').val());
	var lengsChar = getByteLen(loginName);
	var unLen = loginName.length;
	if (loginName == "" || unLen == 0 ) {
		$("#emailerror").html(MSG['emailNull']);
		document.getElementById("emailerror").style.color = "#E04444";
		$("#email").css("border","1px solid #E04444");
		email_bol = false;
		return false;
	} else {
		$("#emailerror").html("");
		$("#email").css("border","1px solid #F1F3F4");
		email_bol = true;
		return true;
	}
}
// 检查密码
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
// 确认密码
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
//验证码
function checkValidcode(){
	var validcode = $('#validcode').val();
	var path=getRootPath();	
	if(validcode == ""){
		document.getElementById("error4").style.color = "#E04444";
		$('#validcode').css("border","1px solid #E04444");
		$("#error4").html(MSG['codeNull']);
		validcode_bol = false;
		return false;			
	}	
	$("#error4").html("");
	$('#validcode').css("border","1px solid #F1F3F4");
	validcode_bol = true;
	return true;
}

function isChecked(){
	if($("#servicebox").attr('checked') == 'checked') {
		$("#errorservice").html('');
	}
};

// 表单信息验证
function checkFlag(){
	var checkbox=document.getElementById('servicebox').checked;
	
	if(!checkbox){
		$("#errorservice").html(MSG['agreementService']);
		document.getElementById("errorservice").style.color = "#E04444";
	}else if(!email_bol){
		$("#emailerror").html(isEmail ? MSG['emailBoxError'] : ($('#email').val() == '' ?  MSG['phoneNull'] : MSG['phoneError']));
        document.getElementById("emailerror").style.color = "#E04444";
        $("#email").css("border","1px solid #E04444");
	}else if(!password_bol){
		document.getElementById("error2").style.color = "#E04444";
		$("#password").css("border","1px solid #E04444");
		$("#error2").html(MSG['pwdLtEightChar']);
	}else if(!password2_bol){
		document.getElementById("error3").style.color = "#E04444";
		$("#password2").css("border","1px solid #E04444");
		$("#error3").html(MSG['twicePwdNotEq']);
	}else if(!validcode_bol){
		document.getElementById("error4").style.color = "#E04444";
		$('#validcode').css("border","1px solid #E04444");
		$("#error4").html(MSG['codeNull']);
	} else if(!choseCode) {
		document.getElementById("emailerror").style.color = "#E04444";
		$('#areaCode').css("border","1px solid #E04444");
		$("#emailerror").html(MSG['choseCode']);
	} else if(!choseCityCode) {
		document.getElementById("countryError").style.color = "#E04444";
		$('.selected-flag-new').css("border","1px solid #E04444");
		$("#countryError").html(MSG['choseCityCode']);
	}

	if(password_bol && password2_bol && email_bol && validcode_bol && checkbox  && choseCode && choseCityCode){
		return true;
	}
	return false;
}

function RefreshCaptcha(obj) {
	var path=getRootPath();	
    obj.src = path+"Captcha/CaptchaAction.do?r=" + new Date().getTime();
    return false;
}