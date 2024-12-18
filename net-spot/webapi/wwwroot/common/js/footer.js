	function getMessageTest() {
		$.ajax({
				type:"get",
				url:basePath + "order/getMessage.do",
				data:{
				},
				dataType:"json",
				success:function(data) {
					if(null != data) {
						$("#messageCountNum").html("");
						if(data["unReadCount"] > 0 ) {
							$("#messageCountDiv").css("display","block"); 
							$("#messageCountNum").html(data["unReadCount"]);
						} else {
							$("#messageCountDiv").css("display","none"); 
						}
					}
					setTimeout(getMessageTest, 30000);
					/* setTimeout(getOrderList, 30000); */
				},
				error:function() {
					setTimeout(getMessageTest, 120000);
				}
			});
	}

	function changeTo(locale) {
		$.get(basePath + "home/setLocale.do?locale=" + locale, function(data) {
			window.location.reload(true);
		});
	}
	
	function getMessageTestaa() {
		$.ajax({
				type:"get",
				url:basePath + "order/getUnreadOrders.do",
				data:{
				},
				dataType:"json",
				success:function(data) {
					if(data != null) {
						$(".message-control").mouseover(function() {
							$(".message-top-menu").show();
							$(".icons-ring").css("opacity",0.7);	
						})
						$(".message-control").mouseout(function() {
							$(".message-top-menu").hide();
							$(".message-top-menu").css('border','none');
							$(".icons-ring").css("opacity",1);		
						})
						var htmls = "<div class='arrow-top' style='z-index: 50px;left: 125px;'></div><div class='megunread-box'>";
						for (var index = 0; index < data.length; index++) {
							htmls += "<div class='megunread-pri'>"
								+ "<a class='megunread-link' style='color: black;display: block;' href='" + basePath + "order/orderDetail.do?id=" + data[index].id + "'>"
								+ "<span>"
								+"[" + data[index].f_order_code + "]"
								+"</span><span style='margin-left: 10px; width: 180px; overflow: hidden;white-space: nowrap;text-overflow: ellipsis;'><fmt:message key='msg.pg.top.messagetheme'/>：" 
								+ data[index].f_theme +"</span><div class='clear'></div><span style='float:right;'>" + dateFmt(data[index].f_update_time*1000) + "</span>" 
								+ "<div class='clear'></div>"
								+ "</a>"
								+ "</div>"			
						}
						$(".message-top-menu").html(htmls + "</div>")	
					}
					if(data.length == 0) {
						$(".message-control").mouseover(function() {
							$(".message-top-menu").hide();	
						})
					}
					setTimeout(getMessageTestaa, 30000);
				},
				error:function() {
					setTimeout(getMessageTestaa, 120000);
				}
			});
	}
	
	function dateFmt(date) {
		if (isNaN(date)) {
			return "";
		}
		date = new Date(date);
		var year = date.getFullYear();
		var month = date.getMonth();
		month = month < 9 ? "0" + (month + 1) : (month + 1);
		var day = date.getDate();
		day = day < 10 ? "0" + day : day;
		
		var hour = date.getHours();
		hour = hour < 10 ? "0" + hour : hour;
		var min = date.getMinutes();
		min = min < 10 ? "0" + min : min;
		var sec = date.getSeconds();
		sec = sec < 10 ? "0" + sec : sec;
		
		return  year+"-"+ month+"-"+day + " " + hour + ":" + min + ":" + sec; 
	}
	
	$(function() {
		// var visits = window.localStorage.getItem("visit");
		// var fromMobile = window.sessionStorage.getItem("fromMobile");
		// if (!fromMobile || fromMobile != '1') {
		// 	var isPCMode = document.location.href.indexOf('view_mode=pc');
		// 	if (isPCMode < 0) {
		// 		var isPhone = false;
		// 		var userAgent = window.navigator.userAgent;
		// 		if ((userAgent.indexOf('iPhone') >= 0 || userAgent.indexOf('iPod') >= 0)) {
		// 			isPhone = true;
		// 		} else if (userAgent.indexOf('Android') >= 0) {
		// 			isPhone = true;
		// 		}
		//
		// 		if (isPhone) {
		// 			var isInvid = document.location.href.indexOf('?invid=');
		// 			if(!visits || visits != '1'){
		// 				if(isInvid < 0) {
		// 					document.location.href = basePath + "download.do";
		// 				}
		// 			}
		// 			window.localStorage.removeItem("visit");

					// if (registerInvid != '') {
					// 	document.location.href = basePath + "h5/index.html#/pages/my/inviteThirdparty/inviteThirdparty?invid=" + registerInvid + "&v=" + new Date().getTime();
					// } else {
					// 	if (window.location.href.indexOf('/news/') >= 0) {
					// 		var id = "";
					// 		var query = window.location.search.substring(1);
					// 		var vars = query.split("&");
					// 		for (var i = 0; i < vars.length; i++) {
					// 			var pair = vars[i].split("=");
					// 			if(pair[0] == "id") {
					// 				id = pair[1];
					// 			}
					// 		}
					//
					// 	    if (id != "") {
					// 	    	document.location.href = basePath + "h5/index.html#/pages/my/notice-detail/notice-detail?id=" + id;
					// 	    } else {
					// 	    	document.location.href = basePath + "h5/index.html?t=" + new Date().getTime();
					// 	    }
					// 	} else {
					// 		document.location.href = basePath + "h5/index.html?t=" + new Date().getTime();
					// 	}
					// }
			// 	};
			// } else {
			// 	window.sessionStorage.setItem("fromMobile", '1');
			// }
		// }

		
		// 客服
		$("#service-icon").click(function() {
	    	$(this).hide();
	    	$("#service-main").show();
	    	
	    	$("#se-title").click(function() {
    			$("#service-main").hide();
    			
    			$("#service-icon").show();
	    	});
	    	
	    	$("#wx1").toggle(function() {
	    		$("#wx-img").show();
	    	}, function() {
	    		$("#wx-img").hide();
	    	});
	    });
		
		getMessageTest();
		getMessageTestaa();
		
		checkLogin();
	});
	function showOnlineService() {
		// console.log(window.KF5SupportBoxAPI);
		// window.KF5SupportBoxAPI.open();
		// window.KF5SupportBoxAPI.hideButton();
	}
