$(function(){
	 var mySwiper = new Swiper ('.swiper-container', {
		spaceBetween: 30,
		centeredSlides: true,
		// autoplay: {
		//   delay: 5000,
		//   disableOnInteraction: false,
		// },
		pagination: {
		el: '.swiper-pagination',
		clickable: true,
		},
        // navigation: {
	    //     nextEl: '.swiper-button-next',
	    //     prevEl: '.swiper-button-prev',
      	// },
      	loop : true,
      });
	 
	$(".mapVol").each(function(){
		if(($(this).html()) > 9999) {
				var count = ($(this).html());
				var i = count/10000;
				var your_target =Math.round( i );
				var vals = formatNumWan(Math.round($(this).html()))
				$(this).html(vals);
		}else {
			var vals = formatNumWan(Math.round($(this).html()));
			$(this).html(vals);
		}
	});

	$(".index-change").click(function() {
		indx++;
		
		var data = contactDatas;
		sortContractList(data);
		
		if(indx%2 == 0) {
			$(".up-down em.arcshape-up-arrow").css({
				"border-left": "2px solid #4980E3",
				"border-bottom": "2px solid #4980E3",	
			})
			$(".up-down em.arcshape-down-arrow").css({
				"border-left": "2px solid #94abc0",
				"border-bottom": "2px solid #94abc0",	
			})
		}else {
			$(".up-down em.arcshape-up-arrow").css({
				"border-left": "2px solid #94abc0",
				"border-bottom": "2px solid #94abc0",	
			})
			$(".up-down em.arcshape-down-arrow").css({
				"border-left": "2px solid #4980E3",
				"border-bottom": "2px solid #4980E3",	
			})
		}
	});

	// $("#iosDownBtn").hover(function(){
	// 	$("#appQrcode").css("margin-left", "0");
	//     $("#appQrcode").css("display", "block");
	// },function(){
	// 	$("#appQrcode").css("display", "none");
	// });

	$("#androidDownBtn").hover(function(){
		$("#appQrcode").css("margin-left", "248px");
	    $("#appQrcode").css("display", "block");
	},function(){
		$("#appQrcode").css("display", "none");
	});
	
	if (!isLogin) {
		var favpath = "bfx_fav";
		if (type != "contract") {
			favpath = "bfx_" + type + "_fav";
		}
		var codes = window.localStorage.getItem(favpath);
		if (codes) {
			var array = JSON.parse(codes);
			for (var i = 0; i < array.length; i++) {
				var code = array[i];
				if (code != "") {
					favorite += (code + ",");
				}
			}
		}
	} else {
		if (favorite.length > 0) {
			favorite += ",";
		}
	}
	
	currPartition = favorite;
	// var getUsdtPartitionIndex = window.localStorage.getItem("usdt-partition-index");
	// if (currPartition.length == 0) {
	// 	currPartition = partionsIndex0;
	// 	if(getUsdtPartitionIndex){
	// 		$("#partitions" + getUsdtPartitionIndex).click();
	// 	} else {
	// 		$("#partitions0").click();
	// 	}
	// } else {
	// 	if(getUsdtPartitionIndex){
	// 		$("#partitions" + getUsdtPartitionIndex).click();
	// 	} else {
	// 		$("#favoritePartition").click();
	// 	}
	// }
	var getSpotPartitionIndex = window.localStorage.getItem("spot-partition-index");
	if(currPartition.length == 0) {
		$("#spotIndexPartition").click();
	} else{
		if(getSpotPartitionIndex == 1) {
			$("#spotIndexPartition").click();
		} else {
			$("#favoriteSpotPartition").click();
		}
	}


	showDaysAndAmount();
	refreshContractsTicker();
	refreshNews();
	refreshExpressNews();
	initRankData();
	
	interval = setInterval(showDaysAndAmount, 80);
	setInterval(refreshContractsTicker, 10000); 
	setInterval(refreshDaysAndAmount, 30000);
	setInterval(initRankData, 60000);
	setInterval(refreshExpressNews, 10000);
});

var message = {};
message["zh_CN"] = {
	emailNull : "邮箱不能为空！",
	emailError : "邮箱格式错误！",
	pwdNull : "验证码不能为空！",
	pwdLtEightChar : "登录密码小于8个字符",
	pwdGtTwentyChar : "登录密码小大于20个字符",
	pwdAvailable : "密码可用！",
	confirmPwdNull : "确认密码不能为空！",
	confirmPwdAvailable : "确认密码可用！",
	twicePwdNotEq : "密码不一致！",
};
message["zh_HK"] = {
	emailNull : "郵箱不能為空！",
	emailError : "郵箱格式錯誤！",
	pwdNull : "驗證碼不能為空！",
	pwdLtEightChar : "登錄密碼小於8個字元",
	pwdGtTwentyChar : "登錄密碼小大於20個字元",
	pwdAvailable : "密碼可用！",
	confirmPwdNull : "確認密碼不能為空！",
	confirmPwdAvailable : "確認密碼可用！",
	twicePwdNotEq : "密碼不一致！",
};
message["en_US"] = {
	emailNull : "The email can not be empty !",
	emailError : "Incorrect login password !",
	pwdNull : "The account has been frozen. please contact customer service !",
	pwdLtEightChar : "The verifying code can not be empty !",
	pwdGtTwentyChar : "Verification code error !",
	pwdAvailable : "Available Password !",
	confirmPwdNull : "Confirm Password cannot be blank !",
	confirmPwdAvailable : "Available Confirm Password !",
	twicePwdNotEq : "Inconsistent passwords !",
};
message["ja_JP"] = {
	emailNull : "メールの欄は空欄になれません！",
	emailError : "メールアドレスの形式が間違っています！",
	pwdNull : "認証コードを空欄にすることはできません！",
	pwdLtEightChar : "パスワードが8文字以下",
	pwdGtTwentyChar : "パスワードが20文字以上",
	pwdAvailable : "このパスワードが利用可能です！",
	confirmPwdNull : "確認パスワードの欄は空欄にすることはできません！",
	confirmPwdAvailable : "確認パスワードが利用可能です！",
	twicePwdNotEq : "パスワードが一致しません！",
};
message["ko_KR"] = {
	emailNull : "사서함은 비워 둘 수 없습니다!",
	emailError : "우편함 형식이 잘못되었습니다!",
	pwdNull : "인증 코드는 비워 둘 수 없습니다!",
	pwdLtEightChar : "로그인 비밀번호가 8자 미만입니다",
	pwdGtTwentyChar : "로그인 비밀번호가 20자 이상입니다",
	pwdAvailable : "비밀번호를 사용할 수 있습니다!",
	confirmPwdNull : "비밀번호는 비워 둘 수 없습니다!",
	confirmPwdAvailable : "비밀번호가 사용 가능한지 확인하십시오!",
	twicePwdNotEq : "일치하지 않는 비밀번호!",
};
message["ru_RU"] = {
	emailNull : "Электронная почта не может быть пустой !",
	emailError : " Неверный пароль для входа!",
	pwdNull : " Аккаунт был заморожен. Пожалуйста, свяжитесь со службой поддержки!",
	pwdLtEightChar : "Код верификации не может быть пустым !",
	pwdGtTwentyChar : "Ошибка кода подтверждения !",
	pwdAvailable : "Доступный пароль !",
	confirmPwdNull : "Подтверждения пароля не может быть пустым !",
	confirmPwdAvailable : "Доступно подтверждение пароля !",
	twicePwdNotEq : "Пароли не соответствуют!",
};
var MSG = message[locale];

var currFavoriteShow = true;
var currPartition = favorite;
var indx = 0;
var contactDatas = [];
var password_bol = false;
var password2_bol = false;
var nowTimes = 0;
var count = 10;
var interval;
var type = "spot";
var firstusdr = true;

function formatNumWan(n){
	var b=parseInt(n).toString();
	var len = b.length;
	if((b.indexOf("-") != -1)) {
		b = b.substr(1,len-1);
		len = b.length;
		if(len<=3) {
		return "-" + b;
		}
		var r=len % 3;
		return r>0?"-" + b.slice(0,r)+","+b.slice(r,len).match(/\d{3}/g).join(","):"-" + b.slice(r,len).match(/\d{3}/g).join(",");
	}else {
		if(len<=3) {
		return b;
		}
		var r=len % 3;
		return r>0?b.slice(0,r)+","+b.slice(r,len).match(/\d{3}/g).join(","):b.slice(r,len).match(/\d{3}/g).join(",");	
	}
	
}

function formatDateTime(data) {
	data = data * 1000;
	var date = new Date(data);
	let y = date.getFullYear();
	let m = date.getMonth() + 1;
	m = m < 10 ? ('0' + m) : m;
	let d = date.getDate();
	d = d < 10 ? ('0' + d) : d;
	let h = date.getHours();
	h = h < 10 ? ('0' + h) : h;
	let minute = date.getMinutes();
	let second = date.getSeconds();
	minute = minute < 10 ? ('0' + minute) : minute;
	second = second < 10 ? ('0' + second) : second;
	return y + '.' + m + '.' + d + ' ' + h + ':' + minute + ':' + second;
//	return m + '.' + d + ' ' + h + ':' + minute + ':' + second;
}

/*USDT合约 & 币币市场 按钮切换*/
function contractTypeChange(self,flag) {
	$(self).addClass('active').siblings().removeClass('active');
	if(flag == 1) {
		type = "contract";
		$("#contract-partition-usdt").css('display', 'block');
		$("#indexDataHeader").css('display', 'block');
		$("#contract-partition-spot").css('display', 'none');
		$("#indexSpotDataHeader").css('display', 'none');

	} else {
		type= "spot";
		$("#contract-partition-spot").css('display', 'block');
		$("#indexSpotDataHeader").css('display', 'block');
		$("#contract-partition-usdt").css('display', 'none');
		$("#indexDataHeader").css('display', 'none');
	}
	
	favorite = "";
	if (!isLogin) {
		var favpath = "bfx_fav";
		if (type != "contract") {
			favpath = "bfx_" + type + "_fav";
		}
		var codes = window.localStorage.getItem(favpath);
		if (codes) {
			var array = JSON.parse(codes);
			for (var i = 0; i < array.length; i++) {
				var code = array[i];
				if (code != "") {
					favorite += (code + ",");
				}
			}
		}
	} else {
		if (type == "contract") {
			favorite = usdtFavorite; 
		} else {
			favorite = spotFavorite;
		}
	}
	
	currPartition = favorite;
	if (type == "contract") {
		var getUsdtPartitionIndex = window.localStorage.getItem("usdt-partition-index");
		if (currPartition.length == 0) {
			currPartition = partionsIndex0;
			if(getUsdtPartitionIndex){
				$("#partitions" + getUsdtPartitionIndex).click();
			} else {
				$("#partitions0").click();
			}
		} else {
			if(getUsdtPartitionIndex){
				$("#partitions" + getUsdtPartitionIndex).click();
			} else {
				$("#favoritePartition").click();
			}
		}
	} else {
		var getSpotPartitionIndex = window.localStorage.getItem("spot-partition-index");
		if(currPartition.length == 0) {
			$("#spotIndexPartition").click();
		} else{
			if(getSpotPartitionIndex == 1) {
				$("#spotIndexPartition").click();
			} else {
				$("#favoriteSpotPartition").click();
			}
		}
	}
	
	refreshContractsTicker();
}

function indexTabClick(sender, codes, isFavorite, index) {
	$(sender).addClass("current").siblings().removeClass("current");
	if (isFavorite) {
		currPartition = favorite;
		currFavoriteShow = true;
		window.localStorage.removeItem("usdt-partition-index");
	} else {
		currPartition = codes;
		currFavoriteShow = false;
		window.localStorage.setItem("usdt-partition-index", index);
	}
	
	if (contactDatas.length > 0) {
		showContractList(contactDatas);
	}
}
function indexTabSpotClick(sender, isFavorite, index) {
	$(sender).addClass("current").siblings().removeClass("current");
	if(isFavorite) {
		currFavoriteShow = true;//自选显示
		currPartition = favorite;
		window.localStorage.removeItem("spot-partition-index");
	} else {
		currFavoriteShow = false;//自选隐藏
		currPartition = spotFavorite;
		window.localStorage.setItem("spot-partition-index", index);
	}
	if (contactDatas.length > 0) {
		showSpotContractList(contactDatas);
	}
}

function refreshContractsTicker() {
	$.ajax({
        type: "GET",
        dataType: "json",         
        url: basePath +"futuresApi/tickersData.do?type=" + type,
        data: {},
        success: function(data) {
			var dataListdetail = eval('(' + data.data + ')');

			if (locale != "zh_CN") {
				for (var i = 0; i < dataListdetail.length; i++) {
					var item = dataListdetail[i];
					if (locale == "en_US") {
						if (item.enName && item.enName != "") {
							item.showName = item.enName;
						}
					} else if (locale == "ja_JP") {
						if (item.jpName && item.jpName != "") {
							item.showName = item.jpName;
						}
					} else if (locale == "ko_KR") {
						if (item.krName && item.krName != "") {
							item.showName = item.krName;
						}
					} else if (locale == "zh_HK") {
						if (item.hkName && item.hkName != "") {
							item.showName = item.hkName;
						}
					}
				}
			}

        	sortContractList(dataListdetail);
        }
	});
}

function refreshDaysAndAmount() {
	$.ajax({
        type: "GET",
        dataType: "json",         
        url: basePath +"home/getDaysAndAmount.do",
        success: function(data) {
        	updateDaysAndAmount(data.years, data.days, data.amount, data.dayAmount);
        }
	});
}

function refreshNews() {
	$.ajax({
        type: "GET",
        dataType: "json",         
        url: basePath +"newsApi/list.do?size=3&categoryId=1&type=web",
        success: function(data) {
        	if (data.code == 0) {
        		var news = data.data;
        		var tickerHtml = "";
        		for (var index = 0; index < news.length; index++) {
            		var obj = news[index];
            		tickerHtml += "<a target='_blank' href='" + basePath + "news/noticedo.do?id=" + obj.article_id + "'>" + obj.title + "</a>";
            	}
        		$("#news").html(tickerHtml);
        	}
        }
	});
}

function gotoNews(id){
	var url = basePath + "news/noticedo.do?id=" + id;
	location.href = url;
}

function refreshExpressNews() {
	$.ajax({
        type: "GET",
        dataType: "json",         
        url: basePath +"home/getExpressNews.do",
        success: function(data) {
        	if (data.code == 0) {
        		data = data.data;
    			for (var index = 0; index < data.length; index++) {
    				var obj = data[index];
    				var num = index + 1;
    				var content = "<a target='_blank' href='" + basePath + "news/newsdo.do?id=" + obj.article_id + "'>" + obj.title + obj.description + "</a>";
    				$("#express-" + num).html(content);
    				$("#express-time-" + num).html(formatDateTime(obj.dateline));
    			}
        	}
        }
	});
}

function contractSearchChange() {
	var data = contactDatas;
	sortContractList(data);
}

function sortContractList(data) {
	if (indx == 0) {
		contactDatas = data;
	} else {
		var temp = [];
		for (var index = 0; index < data.length; index++) {
			var map1 = data[index];
			var fudu1 = parseFloat(map1.fudu.replace("%", ""));
			var rIndex = temp.length;
			for (var i = 0; i < temp.length; i++) {
				var map2 = temp[i];
				var fudu2 = parseFloat(map2.fudu.replace("%", ""));
				
				if (fudu1 > fudu2) {
					rIndex = i;
					for (var j = temp.length; j > i; j--) {
						temp[j] = temp[j-1];
					}
					break;
				}
				
			}
			
			temp[rIndex] = map1;
		}
		
		if (indx % 2 == 0) {
			contactDatas = temp.reverse()
		} else {
			contactDatas = temp;
		}
	}
	if (type == "contract") {
		showContractList(contactDatas);
	} else {
		showSpotContractList(contactDatas);
	}

}

function changeFavorite(sender, code) {
	var cd = code;
	var direct = 1;
	var fav = favorite.split(",");
	if ($.inArray(code, fav) >= 0) {
		favorite = "";
		for (var i = 0; i < fav.length; i++) {
			if (fav[i] != code && fav[i] != "") {
				favorite += (fav[i] + ",");
			}
		}
		direct = 2;
	} else {
		code += ",";
		favorite += code;
	}
	
	if (currFavoriteShow) {
		currPartition = favorite;
		if (type == "contract") {
			showContractList(contactDatas);
		} else {
			showSpotContractList(contactDatas);
		}
	} else {
		if (direct == 1) {
			$(sender).children("i").removeClass("icon-shoucang1");
			$(sender).children("i").addClass("icon-shoucang");
		} else {
			$(sender).children("i").removeClass("icon-shoucang");
			$(sender).children("i").addClass("icon-shoucang1");
		}
	}
	
	var evt = window.event;
	if (evt.stopPropagation) {
		evt.stopPropagation();
	} else { 
		evt.cancelBubble = true;
	}
	
	if (isLogin) {
		updateFav(cd, direct);
	} else {
		if (type == "contract") {
			window.localStorage.setItem("bfx_fav", JSON.stringify(favorite.split(",")));
		} else {
			window.localStorage.setItem("bfx_spot_fav", JSON.stringify(favorite.split(",")));
		}
	}
	
	if (type == "contract") {
		usdtFavorite = favorite;
	} else {
		spotFavorite = favorite;
	}
}

function updateFav(code, direc) {
	// console.log("code: " + code + ", direc: " + direc);
	var url = basePath + "tradeAjax/addOrUpdateFavorite.do";
	$.ajax({
		type : 'POST',
		url : url,
		data : "code=" + code + "&type="+ type + "&direc=" + direc,
		cache : false,
		success : function(data) {
			var data = eval('(' + data + ')');
			if (data.code == 'success') {
			}
		},
		error : function(msg) {
		}
	});
}

function showContractList(data) {
	
	var filter = $("#contractSearchInput").val();
	var tickerHtml = "";

	if (filter && filter.length > 0) {
		
		for (var index = 0; index < data.length; index++) {
			var style = "";
			var map = data[index];
			
			var showname = map.showName;
			if (!showname.startsWith(filter.toUpperCase())) {
				continue;
			}
			
			var starCss = "icon-shoucang1";
			var fav = favorite.split(",");
			if ($.inArray(map.typeCode, fav) >= 0) {
				starCss = "icon-shoucang";
			}
			
			tickerHtml += "<li " + style + "onclick='" + "gotoTrade(\"" + map.tranCode + "\");'>"
				+ "<div class='fav-th' " + "onclick='" + "changeFavorite(this, \""+ map.typeCode + "\");'><i class='iconfont " + starCss + " index-table-star'></i></div>"
				+ "<div class='name-th'>" + map.showName + "</div>"
				+ "<div class='index-th map-bpip2'>" + map.indexPrice + "</div>"
				+ "<div>" + map.lastPrice + "</div>"
				+ "<div>" + map.highPrice + "</div>"
				+ "<div>" + map.lowPrice + "</div>"
				+ (map.lastPrice >= map.firstPrice ? "<div class='buy'>" : "<div class='sell'>") + map.fudu + "</div>"
				+ "<div class='last-th'>" + formatNumWan(map.vol)  + "</div>"
				+ "</li>";
		}
		
	} else {
		if (indx > 0) {
			for (var index = 0; index < data.length; index++) {
				var style = "";
				var map = data[index];
				var codes = currPartition.split(",");
				var ii = $.inArray(map.typeCode, codes);
				if (ii < 0) continue;
				
				var starCss = "icon-shoucang1";
				var fav = favorite.split(",");
				if ($.inArray(map.typeCode, fav) >= 0) {
					starCss = "icon-shoucang";
				}
				
				tickerHtml += "<li " + style + "onclick='" + "gotoTrade(\"" + map.tranCode + "\");'>"
					+ "<div class='fav-th' " + "onclick='" + "changeFavorite(this, \""+ map.typeCode + "\");'><i class='iconfont " + starCss + " index-table-star'></i></div>"
					+ "<div class='name-th'>" + map.showName + "</div>"
					+ "<div class='index-th map-bpip2'>" + map.indexPrice + "</div>"
					+ "<div>" + map.lastPrice + "</div>"
					+ "<div>" + map.highPrice + "</div>"
					+ "<div>" + map.lowPrice + "</div>"
					+ (map.lastPrice >= map.firstPrice ? "<div class='buy'>" : "<div class='sell'>") + map.fudu + "</div>"
					+ "<div class='last-th'>" + formatNumWan(map.vol)  + "</div>"
					+ "</li>";
			}
		} else {
			var codes = currPartition.split(",");
			for (var i = 0; i < codes.length; i++) {
				var code = codes[i];
				for (var index = 0; index < data.length; index++) {
					var style = "";
					var map = data[index];
					
					if (map.typeCode != code) continue;
					
					var starCss = "icon-shoucang1";
					var fav = favorite.split(",");
					if ($.inArray(map.typeCode, fav) >= 0) {
						starCss = "icon-shoucang";
					}
					
					tickerHtml += "<li " + style + "onclick='" + "gotoTrade(\"" + map.tranCode + "\");'>"
						+ "<div class='fav-th' " + "onclick='" + "changeFavorite(this, \""+ map.typeCode + "\");'><i class='iconfont " + starCss + " index-table-star'></i></div>"
						+ "<div class='name-th'>" + map.showName + "</div>"
						+ "<div class='index-th map-bpip2'>" + map.indexPrice + "</div>"
						+ "<div>" + map.lastPrice + "</div>"
						+ "<div>" + map.highPrice + "</div>"
						+ "<div>" + map.lowPrice + "</div>"
						+ (map.lastPrice >= map.firstPrice ? "<div class='buy'>" : "<div class='sell'>") + map.fudu + "</div>"
						+ "<div class='last-th'>" + formatNumWan(map.vol)  + "</div>"
						+ "</li>";
					
					break;
				}
			}
		}
	}
	
	$("#tickerTbody").html(tickerHtml);
	$("#indexTableLoading").hide();
}

function showSpotContractList(data) {
	var filter = $("#contractSearchInput").val();
	var tickerHtml = "";

	for (var index = 0; index < data.length; index++) {
		var style = "";
		var map = data[index];

		var marketName = map.marketName;
		var coinName = map.coinName;
		if (filter && filter.length > 0) {
			var reg = new RegExp(filter.toUpperCase());
			if (!marketName.match(reg) && !coinName.match(reg)) {
				continue;
			}
		}

		if (currFavoriteShow) {
			var codes = favorite.split(",");
			var ii = $.inArray(map.pairName, codes);
			if (ii < 0) continue;
		}

		var starCss = "icon-shoucang1";
		var fav = favorite.split(",");
		if ($.inArray(map.pairName, fav) >= 0) {
			starCss = "icon-shoucang";
		}
		if(map.coinName != undefined || map.marketName != undefined) {
			tickerHtml += "<li " + style + "onclick='" + "gotoTrade(\"" + map.pairId + "\");'>"
				+ "<div class='fav-th' " + "onclick='" + "changeFavorite(this, \""+ map.pairName + "\");'><i class='iconfont " + starCss + " index-table-star'></i></div>"
				+ "<div class='name-th changeWidth'>" + map.coinName + "/" + map.marketName + "</div>"
				+ "<div class='changeWidth changeTextAligncenter'>" + map.lastPrice + "</div>"
				+ "<div class='changeWidth changeTextAligncenter'>" + map.lowPrice + "</div>"
				+ "<div class='changeWidth changeTextAligncenter'>" + map.highPrice + "</div>"
				+ (map.lastPrice >= map.firstPrice ? "<div class='buy changeWidth changeTextAlignright'>" : "<div class='sell changeWidth changeTextAlign'>") + map.fudu + "</div>"
				+ "<div class='last-th changeWidth changeTextAlignright'>" + formatNumWan(map.vol)  + "</div>"
				+ "</li>";
		}
	}

	$("#tickerTbody").html(tickerHtml);
	$("#indexTableLoading").hide();
}

function initRankData() {
	getInviteRanking();
	getRebateRanking();
	getIncomeRanking();
}

function getInviteRanking() {
	$.ajax({
		type: "GET",
		dataType: "json",         
		url: basePath +"home/getInviteRanking.do",
		success: function(data) {
			var tickerHtml = "";
			for (var index = 0; index < data.length; index++) {
				var rank = index + 1;
				if (index == 0) {
					rank = "";
				}
				var obj = data[index];
				tickerHtml += "<li><div class='ranking-th'>" + rank + "</div><div class='email-th'>" + obj[0] + "</div><div class='last-th'>" + obj[1] + "</div></li>";
			}
			$("#inviteRanking").html(tickerHtml);
		}
	});
}

function getRebateRanking() {
	$.ajax({
        type: "GET",
        dataType: "json",         
        url: basePath +"home/getRebateRanking.do",
        success: function(data) {
        	var tickerHtml = "";
        	for (var index = 0; index < data.length; index++) {
        		var rank = index + 1;
				if (index == 0) {
					rank = "";
				}
				var obj = data[index];
				tickerHtml += "<li><div class='ranking-th'>" + rank + "</div><div class='email-th'>" + obj.userName + "</div><div class='last-th'>" + obj.rebate + "</div></li>";
        	}
        	$("#rebateRanking").html(tickerHtml);
        }
	});
}

function getIncomeRanking() {
	$.ajax({
		type: "GET",
		dataType: "json",         
		url: basePath +"home/getIncomeRanking.do",
		success: function(data) {
        	var tickerHtml = "";
        	for (var index = 0; index < data.length; index++) {
        		var rank = index + 1;
				if (index == 0) {
					rank = "";
				}
				var obj = data[index];
				tickerHtml += "<li><div class='ranking-th'>" + rank + "</div><div class='email-th'>" + obj.userName + "</div><div class='last-th'>" + obj.rebate + "</div></li>";
        	}
        	$("#incomeRanking").html(tickerHtml);
        }
	});
}

function openHome() {
	$("#alert-windows").hide();	
	location.href = basePath;
}

function isNewusers() {
	$("#alert-windows").hide();	
	$("#alert-pwd").show();	

	if(domilyVals == "true") {
		$("#alert-login").show();		
	} else {
		location.href = basePath + "doBcexLogin.do";
	}
}

function cancelClick() {
	location.href = basePath;
	return false;
}

function checkPwd() {
	var password = $("#password").val();
	var unLen = password.length;
	if (unLen == 0) {
		document.getElementById("error2").style.color = "red";
		$("#error2").html(MSG['pwdNull']);
		password_bol = false;
		return false;
	}
	if (unLen < 8) {
		document.getElementById("error2").style.color = "red";
		$("#error2").html(MSG['pwdLtEightChar']);
		password_bol = false;
		return false;
	}
	if (unLen > 20) {
		document.getElementById("error2").style.color = "red";
		$("#error2").html(MSG['pwdGtTwentyChar']);
		password_bol = false;
		return false;
	}
	var repassword = $("#password2").val();
	if (repassword != "") {
		if (password != repassword) {
			document.getElementById("error3").style.color = "red";
			$("#error3").html(MSG['twicePwdNotEq']);
			password2_bol = false;
			return false;
		} else {
			document.getElementById("error3").style.color = "green";
			$("#error3").html(MSG['pwdAvailable']);
			password2_bol = true;
		}
	}
	document.getElementById("error2").style.color = "green";
	$("#error2").html(MSG['pwdAvailable']);
	password_bol = true;
	return true;
}

function checkPwd2() {
	var password1 = $("#password").val();
	var password2 = $("#password2").val();
	
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

function checkParam() {
	if(password_bol && password2_bol) {
		return true;
	}
	
	if(!password_bol){
		document.getElementById("error2").style.color = "red";
		$("#error2").html(MSG['pwdLtEightChar']);
	}else if(!password2_bol){
		document.getElementById("error3").style.color = "red";
		$("#error3").html(MSG['twicePwdNotEq']);
	}
	
	return false;
}

function gotoTrade(code){
	if (type == "contract") {
		location.href = basePath + "trade/trade.do?transactionCode=" + code;
	} else {
		location.href = basePath + "spotTrade/spotTrade.do?coinPairId=" + code;
	}
}

function updateDaysAndAmount(years, days, amount, dayAmount) {
	var daysStr = days + "";
	while(daysStr.length < 3) {
		daysStr = "0" + daysStr;
	}
	
	var dataAmount = amount / 10000;
	var dayA = dayAmount / 10000;
	if (locale == 'en_US') {
		dataAmount = dataAmount / 10;
		dayA = dayA / 10;
	}
	
	$("#yearsValue").html(years);
	$("#daysValue").html(daysStr);
	$("#amountValue").html(formatNumWan(dataAmount));
	$("#dayAmountValue").html(formatNumWan(dayA));
}


function showDaysAndAmount() {
	var year = parseInt(data.years / count * nowTimes);
	var day = parseInt(data.days / count * nowTimes);
	var dataAmount = data.amount;
	var dayAmount = data.dayAmount;
	var a = dataAmount / 2 +  (dataAmount / 2) / count * amount;
	var amount = parseInt(dataAmount / 2 + (dataAmount / 2) / count * nowTimes);
	var dAmount = parseInt(dayAmount / 2 + (dayAmount / 2) / count * nowTimes);
	updateDaysAndAmount(year, day, amount, dAmount);
	nowTimes++;
	if (nowTimes > count) {
		clearInterval(interval);
	}
}

function getLocalTime(nS) {
	var date = new Date(parseInt(nS) * 1000);
	var month = date.getMonth() + 1 + "";
	if (month.length < 2) {
		month = "0" + month;
	}
	var day = date.getDate() + "";
	if (day.length < 2) {
		day = "0" + day;
	}
	var hour = date.getHours() + "";
	if (hour.length < 2) {
		hour = "0" + hour;
	}
	var minutes = date.getMinutes() + "";
	if (minutes.length < 2) {
		minutes = "0" + minutes;
	}
	return month + "-" + day + " " + hour + ":" + minutes;
}
