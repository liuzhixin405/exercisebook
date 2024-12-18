//查看收起API
$('.showapidetile').bind('click', function(){
    if($(this).html() == MSG['lookOver']){
        $('.showapidetile').html(MSG['lookOver']);
         $(this).html(MSG['applyPickup']);
        $(".showInfo").hide("slow");
        $(this).parent().parent().next('tr').show("9999");
        return;
    }
    if($(this).html() == MSG['applyPickup']){
        $(this).html(MSG['lookOver']);
         $(this).parent().parent().next("tr").hide("9999");
    }
});
    
var apicode_bol = true;
// 验证手机验证码
function checkCode() {
	var tel = $("#mobile").val();
	var code = $("#code").val();
	var unLen = code.replace(/[^\x00-\xff]/g, "**").length;
	if (unLen == 0) {
		document.getElementById("error1").style.color = "red";
		$("#error1").html(MSG['codeNull']);
		return false;
	}
	var path = getRootPath();
	$.post(path + 'center/checkTelCode.do', {code:code,tel:tel}, function(data) {
		if (data == "false") {
			document.getElementById("error1").style.color = "red";
			$("#error1").html(MSG['codeError']);
			return false;
		} else {
			$("#error1").html(MSG['codeCorrect']);
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
		document.getElementById("googleerror").style.color = "red";
		$("#googleerror").html(MSG['codeNull']);
		return false;
	}

	$("#googleerror").html("");
	document.getElementById("googleerror").style.color = "green";

	return true;
}
//申请子账号API
function checkSubApiFlag() {
	var applayerror = "${applayerror}";
	if (applayerror != null && applayerror != '' && applayerror == 'true') {
		document.getElementById("errorApi").style.color = "red";
		$("#errorApi").html(MSG['tipsApi']);
		return false;
	}

	if (apicode_bol) {
		return true;
	}
	return false;
}
//显示api详情框
function showapi(type1,type2,type3) {
	$("#alert-windows").show();
	if(type3 == "") {
		var dateTm = MSG['dateTmNinty']
	}
	if(type3 != "") {
		var dateTm = MSG['dateTmNover']
	}
	var insidehtm = "<div id='tb-overlay-duiping' class='tb-overlay'></div><div class='add-wrap alert-wrap' style='display: block;' id='invite-BFX'>"
				+"<div class='dialogs-title'>"
				+"<h2 class='text-center'>"
					+MSG['applySuc']
				+"</h2>"
				+"<a class='close-icon icons-close' id='close-alert' href='javascript:closealert()'></a>"
				+"</div>"
				+"<div class='dialogs-body' style='line-height: 20px;padding:20px 0px;'>"
				+"<div style='padding: 20px 0px;background-color: #e5f5f9;'>"
				+"<p style='font-size: 16px;margin-left: 30px;'>" + dateTm + "</p>"
				+"<p style='font-size: 16px;margin-left: 30px;margin-top: 10px;'>" + MSG['apisecrete'] + "</p>"
				+"<p style='font-size: 16px;margin-left: 30px;margin-top: 10px;'>" + MSG['replyapisecrete'] + "</p>"
				+"</div>"
				+"<p style='font-size: 16px;margin-left: 30px;margin-top: 10px;color: #808080;'>"+ MSG['apiPublic'] + type1 +"</p>"
				+"<p style='font-size: 16px;margin-left: 30px;margin-top: 10px;color: #808080;'>"+ MSG['apiPerson'] + type2 +"</p>"
				+"<p style='font-size: 16px;margin: 10px 0px 10px 30px;color: #808080;'>IP："+ type3 +"</p>"
				+"<div class='form-group-btn text-center'>"
					+"<a class='btn sub-btn center' style='margin-right: 0px;' href='javascript:closealert();'>"+MSG['sureTip']+"</a>"
				+"</div>"
				+"</div>"
			+"</div>";
	$("#alert-windows").html(insidehtm);
	center($('.alert-wrap'));
}
//ip地址验证
function checkIpRight() {
	var ipRight = $("#allowIp").val();			
	var reg =  /^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])$/;
	var ableinp = ipRight.split(',');
	if(ipRight == "") {
		apicode_bol = true;
		$("#iperror").html('');
		return true;
	} 
	if(ipRight != "") {
		if(ableinp.length > 10) {
			$("#iperror").html(MSG['arrayIp']);
			document.getElementById("iperror").style.color = "red";
			apicode_bol = false;
			return false;	
		}else {
			for(let i = 0;i < ableinp.length;i++) {
			  if(reg.test(ableinp[i])){
			  	continue;		  
			  }else {
			  	$("#iperror").html(MSG['ipEorror']);
				document.getElementById("iperror").style.color = "red";
				apicode_bol = false;
				return false;	
			  }
			}
			$("#iperror").html(MSG['ipCorrect']);
			document.getElementById("iperror").style.color = "green";
			apicode_bol = true;
			return true;	
		}	
	} 
	$("#iperror").html(MSG['ipEorror']);
	document.getElementById("iperror").style.color = "red";
	apicode_bol = false;
	return false;
}
//弹框ip地址验证
function checkIpRightEdit() {
	var ipRight = $("#testIp").val();			
	var reg =  /^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])$/;
	var ableinp = ipRight.split(',');
	if(ipRight == "") {
		apicode_bol = true;
		$("#iperrorEdit").html('');
		return true;
	} 
	if(ipRight != "") {
		if(ableinp.length > 10) {
			$("#iperrorEdit").html(MSG['arrayIp']);
			document.getElementById("iperrorEdit").style.color = "red";
			apicode_bol = false;
			return false;	
		}else {
			for(let i = 0;i < ableinp.length;i++) {
			  if(reg.test(ableinp[i])){
			  	continue;		  
			  }else {
			  	$("#iperrorEdit").html(MSG['ipEorror']);
				document.getElementById("iperrorEdit").style.color = "red";
				apicode_bol = false;
				return false;	
			  }
			}
			$("#iperrorEdit").html(MSG['ipCorrect']);
			document.getElementById("iperrorEdit").style.color = "green";
			apicode_bol = true;
			return true;	
		}	
	} 
	$("#iperrorEdit").html(MSG['ipEorror']);
	document.getElementById("iperrorEdit").style.color = "red";
	apicode_bol = false;
	return false;
}
//显示编辑弹框
function showEditBox(subUserId) {
	$("#sub-edit-id").val(subUserId);
	$("#editShowBox").show();
	$("#black_overlay").show();
}
//关闭编辑弹框
function closeEditBox() {
	$("#editShowBox").hide();
	$("#black_overlay").hide();
}
//验证手机验证码
function checkCode1() {
	var tel = $("#mobile2").val();
	var code = $("#telCode").val();
	var unLen = code.replace(/[^\x00-\xff]/g, "**").length;
	if (unLen == 0) {
		document.getElementById("error-edit-showBox").style.color = "red";
		$("#error-edit-showBox").html(MSG['codeNull']);
		return false;
	}
	var path = getRootPath();
	$.post(path + 'center/checkTelCode.do', {code:code,tel:tel}, function(data) {
		if (data == "false") {
			document.getElementById("error-edit-showBox").style.color = "red";
			$("#error-edit-showBox").html(MSG['codeError']);
			return false;
		} else {
			$("#error-edit-showBox").html("");
			document.getElementById("error-edit-showBox").style.color = "green";
			return true;
		}
	}, "text")
}

//检查谷歌验证码
function checkGoogle1() {
	var code = $("#googleCode").val();
	var unLen = code.replace(/[^\x00-\xff]/g, "**").length;
	if (unLen == 0) {
		document.getElementById("googleError-edit").style.color = "red";
		$("#googleError-edit").html(MSG['codeNull']);
		return false;
	}
	
	$("#googleError-edit").html("");
	document.getElementById("googleError-edit").style.color = "green";
	
	return true;
}
//修改子账号API
function changeSubaccountApi() {
	var googleCode = $("#googleCode").val();
	var telCode = $("#telCode").val();
	var apiAddress = $("#testIp").val();
	var subUserId = $("#sub-edit-id").val();
	
	$.post(basePath+"userSubAccount/updateSubAppKey.do",{subUserId:subUserId, code:telCode, google:googleCode, allowIp:apiAddress},function(data){
		data = JSON.parse(data);
		if(data.code == "success"){
			var times = 2;
			$("#editShowBoxError").html(data.msg + "(2s)");
			$("#editShowBoxError").css("color", "green");
			$("#editShowBoxError").show();
			
			setInterval(function() {
				times--;
				$("#editShowBoxError").html(data.msg + "(" + times + "s)");
				if (times == 0) {
					window.location.reload();
				}
			}, 1000);
			
		} else {
			$("#editShowBoxError").html(data.msg);
			$("#editShowBoxError").show();
		}
	}, "text");
	return false;
}
