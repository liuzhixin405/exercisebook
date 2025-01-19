//获取当前跟路径
function getRoot_Path() {
	// 获取当前网址，如： http://localhost:8083/
	var curWwwPath = window.document.location.href;
	var p = curWwwPath.lastIndexOf("?");
	if (p > 0) {
		curWwwPath = curWwwPath.substr(0, p);
	}
	
	// 获取主机地址之后的目录，如： uimcardprj/share/meun.jsp
	var pathName = window.document.location.pathname;
	var pos = curWwwPath.indexOf(pathName);
	if (pathName == "" || pathName == "/") {
		pos = curWwwPath.length - (pathName == "" ? 0 : 1);
	}
	// 获取主机地址，如： http://localhost:8083
	var localhostPaht = curWwwPath.substring(0, pos);
	// 获取带"/"的项目名，如：/uimcardprj
//	var projectName = pathName.substring(0, pathName.substr(1).indexOf('/') + 1);
	return (localhostPaht);
}

function getCookie(name){ 
    var arr,reg=new RegExp("(^| )"+name+"=([^;]*)(;|$)");
    if(arr=document.cookie.match(reg))
        return unescape(arr[2]);
    else
    	return null;
}

function checkInputDigits(oInput, digits) {
	var len = 8;
	var exp = /\d{1,8}\.{0,1}\d{0,3}/;
	if (digits == 0) {
		var exp = /\d{1,8}\.{0,0}\d{0,0}/;
	} else if (digits == 1) {
		var exp = /\d{1,8}\.{0,1}\d{0,1}/;
	} else if (digits == 2) {
		var exp = /\d{1,8}\.{0,1}\d{0,2}/;
	} else if (digits == 3) {
		var exp = /\d{1,8}\.{0,1}\d{0,3}/;
	} else if (digits == 4) {
		var exp = /\d{1,8}\.{0,1}\d{0,4}/;
	} else if (digits == 5) {
		var exp = /\d{1,8}\.{0,1}\d{0,5}/;
	} else if (digits == 6) {
		var exp = /\d{1,8}\.{0,1}\d{0,6}/;
	} else if (digits == 7) {
		var exp = /\d{1,8}\.{0,1}\d{0,7}/;
	} else if (digits == 8) {
		var exp = /\d{1,8}\.{0,1}\d{0,8}/;
	} else if (digits == 9) {
		var exp = /\d{1,8}\.{0,1}\d{0,9}/;
	}
	
	
	if ('' != oInput.value.replace(exp, '')) { // 含有数字的话
		oInput.value = oInput.value.match(exp) == null ? '' : oInput.value.match(exp);
		if (oInput.value.indexOf('.') == -1 && oInput.value.length > len) {
			oInput.value = oInput.value.substr(0, len) + '.' + oInput.value.substr(len);
		}
	} else {
		if (oInput.value.indexOf('.') == -1 && oInput.value.length > len) {
			oInput.value = oInput.value.substr(0, len) + '.' + oInput.value.substr(len);
		}
	}
	
	if (oInput.value.length > 0 && Number(oInput.value) == 0 && oInput.value.indexOf('.') == -1) {
		oInput.value = 0;
	}
}

Date.prototype.Format = function(fmt) {
	var o = {
		 "M+": this.getMonth()+1,
		 "d+": this.getDate(),
		 "H+": this.getHours(),
		 "m+": this.getMinutes(),
		 "s+": this.getSeconds(),
		 "S+": this.getMilliseconds()
	};
 
	if(/(y+)/.test(fmt)){
		fmt=fmt.replace(RegExp.$1,(this.getFullYear()+"").substr(4-RegExp.$1.length));
	}
	
	for(var k in o){
		if (new RegExp("(" + k +")").test(fmt)){
			fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(String(o[k]).length)));
		}
	}	
	return fmt;
}


//var basePath = "http://btc-donghui:8080/";
var basePath=getRoot_Path()+"/";
var depthData;


var ls = "${locale }"
// 模态框显示
function show(id, s) {
    id.fadeIn(300, function() {
        $(s).show();
    });
};
// 模态框隐藏
function hide(id, d) {
    id.fadeOut(300, function() {
        $(d).hide();
    });
};

//提示框
function tipAlert(type) {
	$("#alert-windows").show();
	var insidehtm = "<div id='tb-overlay-duiping' class='tb-overlay'></div><div class='add-wrap alert-wrap' style='display: block;' id='invite-BFX'>"
				+"<div class='dialogs-title'>"
				+"<h2 class='lt'>"
					+MSG['warmTip']
				+"</h2>"
				+"<a class='close-icon icons-close' id='close-alert' href='javascript:closealert()'></a>"
				+"</div>"
				+"<div class='dialogs-body'>"
				+"<p style='text-align: center; font-size: 16px; margin-bottom: 30px;'>"+ type +"</p>"
				+"<div class='form-group-btn text-center'>"
					+"<a class='btn sub-btn center' style='margin-right: 0px;' href='javascript:closealert();'>"+MSG['sureTip']+"</a>"
				+"</div>"
				+"</div>"
			+"</div>";
	$("#alert-windows").html(insidehtm);
	center($('.alert-wrap'));
}

function center(obj) { 
    var screenWidth = $(window).width(); 
    var screenHeight = $(window).height();  
    var scrolltop = $(document).scrollTop();

    var objLeft = (screenWidth - obj.width())/2 ;
    var objTop = (screenHeight - obj.height())/2 + scrolltop;
    obj.css({left: objLeft + 'px', top: objTop + 'px','display': 'block'});
    //浏览器窗口大小改变时
    $(window).resize(function() {
        screenWidth = $(window).width();
        screenHeight = $(window).height();
        scrolltop = $(document).scrollTop();
       
        objLeft = (screenWidth - obj.width())/2 ;
        objTop = (screenHeight - obj.height())/2 + scrolltop;
       
        obj.css({left: objLeft + 'px', top: objTop + 'px','display': 'block'});
       
    });
}

function closealert() {
	$("#alert-windows").hide();
}

function logoutClick() {
	window.localStorage.removeItem("token");
	window.localStorage.removeItem("user_name");
	document.location.href = basePath + "login/logout.do";
	return false;
}

function checkLogin() {
	var token = window.localStorage.getItem("token");
	if (!token && !isLogin) return;
	
	$.ajax({
		type:"get",
		url:basePath + "login/checkLogin.do",
		data:{
			token: token
		},
		dataType:"json",
		success:function(data) {
			if (data.code == 0) {
				window.localStorage.setItem("token", data.token);
				window.localStorage.setItem("user_name", data.userName);
				if (!loadLogin) {
					window.location.reload();
				}
			} else {
				window.localStorage.removeItem("token");
				window.localStorage.removeItem("user_name");
			}
		},
		error:function() {
			
		}
	});
}
