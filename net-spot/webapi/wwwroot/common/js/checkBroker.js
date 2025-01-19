var bol_username = false;
var bol_userNo = false;
var bol_userTel = false;

/**
 * 检查用户真实姓名
 * 
 * @return
 */
function checkUserName_keydown() {
	var reg = /^\w{1,12}|([\u4e00-\u9fa5]{1,12})$/;
	var userName = $("#userName").val();
	userName = userName.trim();
	if (reg.test(userName)) {
		$("#validateName").css( { "color" : "green" }).html("");
		bol_username = true;
		return true;
	} else {
		$("#validateName").css( { "color" : "red" }).html("*用户姓名不合法!");
		bol_username = false;
		return false;
	}
}
/**
 * 检查用户真实姓名
 * 
 * @return
 */
function checkUserName_blur() {
	var userName = $("#userName").val();
	if (!userName) {
		$("#validateName").css( { "color" : "red" }).html("*请输入用户姓名!");
		bol_username = false;
		return false;
	}
}

/**
 * 检查用户身份证号
 * 
 * @return
 */
function checkUserNo() {
	var userNo = $("#userNo").val();
	userNo = userNo.replace(/[ ]/g, "");
	if (userNo == "") {
		$("#validateNo").css( { "color" : "red" }).html("*请输入身份证号!");
		bol_userNo = false;
		return false;
	} else {
		$("#validateNo").css( { "color" : "green" }).html("");
		bol_userNo = true;
		return true;
	}
}
/**
 * 检查手机号码
 * 
 * @return
 */
function checkUserTel() {
	var userTel = $("#userTel").val();
	if (userTel == "") {
		$("#validateTelephone").css( { "color" : "red" }).text("*请输入您的电话!");
		bol_userTel = false;
		return false;
	} 
	if(isNaN(userTel)){
		$("#validateTelephone").css( { "color" : "red" }).text("*电话不合法!");
		bol_userTel = false;
		return false;
	} else {
		$("#validateTelephone").css( { "color" : "green" }).text("");
		bol_userTel = true;
		return true;
	}
}
/**
 * 检查页面输入数据
 * @return
 */
function checkFlag(){
	if (!bol_userNo) {
		checkUserNo();
	}
	if (!bol_userTel) {
		checkUserTel();
	}
	if (!bol_username) {
		checkUserName_keydown();
	}
	if (bol_username && bol_userTel && bol_userNo) {
		return true;
	}
	return false;
}