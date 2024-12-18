//获取当前跟路径
function getRootPath(){
	return basePath;
}
var username_bol=false;
function checkBTC100user(){
	var username = $("#username").val();
	var unLen = username.length;
	if (unLen == 0) {
		document.getElementById("errorname").style.color = "red";
		$("#errorname").html("用户名不能为空");
		username_bol = false;
		return false;
	}
	
	var path=getRootPath();
	$.ajax({		
		url :path + 'center/checkBtc100User.do?username='+username,
		type : "POST",

		dataType : "JSON",
		cache : false,
		success : function(data) {		
			if (data == null) {
				document.getElementById("errorname").style.color = "red";
				$("#errorname").html("该用户不存在");
				username_bol = false;
				return;
			}
			if (data.user != "success") {
				document.getElementById("errorname").style.color = "red";
				$("#errorname").html("该用户异常");
				username_bol = false;
				return;
			}
			if(data.telCode==""){
				$("#telphoneNum").css("display", "none");
			}
			else{
				$("#telphoneNum").css("display", "block");
				$("#mobile").val(data.telCode+"");
//				$("#mobile").attr("value",data.telCode);
//				alert(data.telCode);
			}
			if(data.googleCode==""){
				document.getElementById("googleNum").style.display="none";
			}else{
				document.getElementById("googleNum").style.display="block";
				$("#secrety").attr("value",data.googleCode);
			}
		}
	})
	document.getElementById("errorname").style.color = "green";
	$("#errorname").html("");
	username_bol = true;
	return true;
}
var password_bol=false;
function checkBTC100UserPassword(){
	var username = $("#username").val();
	var password = $("#password").val();
	var unLen = username.length;
	var lenPaw=password.length;
	if (unLen == 0) {
		document.getElementById("errorname").style.color = "red";
		$("#errorname").html("用户名不能为空");
		username_bol = false;
		return false;
	}
	if (lenPaw == 0) {
		document.getElementById("errorpwd").style.color = "red";
		$("#errorpwd").html("密码不能为空");
		password_bol = false;
		return false;
	}
	var path=getRootPath();
	$.post(path + 'center/checkBtc100UserPassword.do', {
		username : username,
		password:password
	}, function(data) {
		if (data == "false") {
			document.getElementById("errorpwd").style.color = "red";
			$("#errorpwd").html("用户名或密码不正确");
			password_bol = false;
			return false;
		} else {
			$("#errorpwd").html("");
			document.getElementById("errorpwd").style.color = "green";
			password_bol = true;
			return true;
		}
	}, "text")
}