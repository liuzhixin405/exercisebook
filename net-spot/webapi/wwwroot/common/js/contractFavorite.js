$(function(){
	$("#changeUpDown").click(function() {
		indx++;
		
		var data = contactDatas;
		sortContractList(data);
		
		if(indx%2 == 0) {
			$(".change-up-down").children("em.arcshape-up-arrow").css({
				"border-left": "2px solid #4980E3",
				"border-bottom": "2px solid #4980E3",
			})
			$(".change-up-down em.arcshape-down-arrow").css({
				"border-left": "2px solid #94abc0",
				"border-bottom": "2px solid #94abc0",
			})
		}else {
			$(".change-up-down em.arcshape-up-arrow").css({
				"border-left": "2px solid #94abc0",
				"border-bottom": "2px solid #94abc0",
			})
			$(".change-up-down").children("em.arcshape-down-arrow").css({
				"border-left": "2px solid #4980E3",
				"border-bottom": "2px solid #4980E3",
			})
		}
	});

	if (!isLogin) {
		var codes = window.localStorage.getItem("bfx_fav");
		if (type == 'vcontract') {
			codes = window.localStorage.getItem("bfx_v_fav");
		} else if (type == 'delivery') {
			codes = window.localStorage.getItem("bfx_d_fav");
		} else if (type == 'contract' || type == '') {
			
		} else {
			codes = window.localStorage.getItem("bfx_" + type +"_fav");
		}

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
	
	if (favorite.length == 0) {
		currFavoriteShow = false;
		$("#sideNavigationLi").click();
	
	} else {
		currFavoriteShow = true;
		$("#favoritePartition").click();
		
	}
	
	if (tradePath == "vtrade") {
		if (favorite.length == 0) {
			currFavoriteShow = false;
			$("#marketCurrent").click();
		
		} else {
			currFavoriteShow = true;
			$("#favoritePartitionV").click();
			
		}
	}
	
	function getQuery(name) {
	    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
	    var r = window.location.search.substr(1).match(reg);
	    if (r != null) return unescape(r[2]); return null;
	}
	var selectedSideNavId = getQuery("sideNav");
	if (selectedSideNavId != undefined && selectedSideNavId != null) {
		$("#"+selectedSideNavId).click();
	}

//  commonData.js
// 	refreshContractsTicker();
// 	setInterval(refreshContractsTicker, 6000);
});
var currPartition = favorite;
var currFavoriteShow = true;
var showContractListon = true;
var indx = 0;
var contactDatas = [];
var nowTimes = 0;
var count = 10;
var interval;
var currFirstPrice = 0;
var isLastPriceChange = false;
var type = $('#tradePath').val();
var tradePath = $('#tradePath').val();
if (tradePath == "vtrade") {
	type = "vcontract";
} else if (tradePath == 'dtrade') {
	type = "delivery";
} else if (tradePath == 'trade') {
	type = "contract";
}

function indexTabClick(sender, type) {
	$(sender).addClass("current").siblings().removeClass();
	if (type == 1){
		$(".item_data .trade-history-seft").removeClass("hide");
		currFavoriteShow = false;
	} else if (type == 0) {
		$(".item_data .trade-history-seft").removeClass("hide");
		currFavoriteShow = true;
	} 
	showContractListon = false;
	if (contactDatas.length > 0) {
		showContractList(contactDatas);
	}
}

function refreshContractsTicker() {
	$.ajax({
        type: "GET",
        dataType: "json",         
        url: basePath +"futuresApi/tickersData.do?type=" + type,
        data: {},
        success: function(data) {	        	    
        	if(data.code == 0) {
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
        	
        }
	});
}

function searchChange() {
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
	if (showContractListon == true) {
		showContractListSide(contactDatas);
	} else {
		showContractList(contactDatas);
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
		if (showContractListon == true) {
			showContractListSide(contactDatas);
		} else {
			showContractList(contactDatas);
		}
	} else {
		if (direct == 1) {
			$(sender).removeClass("icon-shoucang1");
			$(sender).addClass("icon-shoucang");
		} else {
			$(sender).removeClass("icon-shoucang");
			$(sender).addClass("icon-shoucang1");
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
		if (type == "vcontract") {
			window.localStorage.setItem("bfx_v_fav", JSON.stringify(favorite.split(",")));
		} else if (type == "delivery") {
			window.localStorage.setItem("bfx_d_fav", JSON.stringify(favorite.split(",")));
		} else if (type == "contract") {
			window.localStorage.setItem("bfx_fav", JSON.stringify(favorite.split(",")));
		} else {
			window.localStorage.setItem("bfx_" + type + "_fav", JSON.stringify(favorite.split(",")));
		}
	}
}

function updateFav(code, direc) {
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
	var filter = $("#search-box-top").val();
	var tickerHtml = "";
	var bqCode = $('#transaction_code').val();
	for (var index = 0; index < data.length; index++) {
		var map = data[index];
		
		var bacCSS = ""
		if (bqCode == map.tranCode) {
			if (currFirstPrice != 0 && isLastPriceChange) {
				var lastPrice = $($(".contract-list-li.later").children(".contract-list-cell-price")[0]).children("span").html();
				var lastFudu = $($(".contract-list-li.later").children(".contract-list-cell-fudu")[0]).children("span").html();
				if (lastPrice) {
					map.lastPrice = lastPrice;
				}
				
				if (lastFudu) {
					map.fudu = lastFudu;
				}
			}
			
			bacCSS = "later";
			currFirstPrice = map.firstPrice;
		}
		if (currFavoriteShow) {
			var codes = favorite.split(",");
			var ii = $.inArray(map.tranCode, codes);
			if (ii < 0) continue;
		}
		
		var showname = map.showName;
		if (filter && filter.length > 0) {
			if (!showname.toUpperCase().startsWith(filter.toUpperCase())) {
				continue;
			}
		}
		
		var starCss = "icon-shoucang1";
		var fav = favorite.split(",");
		if ($.inArray(map.tranCode, fav) >= 0) {
			starCss = "icon-shoucang";
		}
		
		var colorClass = "sell";
		if (map.lastPrice >= map.firstPrice) colorClass = "buy";
		tickerHtml += "<li class='contract-list-li " + bacCSS + "' onclick='" + "gotoTrade(\""+ map.tranCode + "\");'>"
			+ "<div class='contract-list-cell' ><i class='iconfont " + starCss + " index-table-star' onclick='" + "changeFavorite(this, \""+ map.tranCode + "\");'></i><span>" + map.showName + "</span></div>"
			+ "<div class='contract-list-cell contract-list-cell-price'><span>" + map.lastPrice + "</span></div>"
			+ "<div class='contract-list-cell contract-list-cell-fudu'><span class='" + colorClass + "'>" + map.fudu + "</span></div>"
			+ "</li>";
	}
	$("#tickerTbody").html(tickerHtml);
}

function gotoTrade(code){
	
	var url = basePath + "trade/trade.do?transactionCode=" + code;
	if (type == "vcontract") {
		url = basePath + "vtrade/trade.do?transactionCode=" + code;
	} else if (type == "delivery") {
		url = basePath + "dtrade/trade.do?transactionCode=" + code;
	} else if (type == "contract") {
		url = basePath + "trade/trade.do?transactionCode=" + code;
	} else {
		url = basePath + "/ntrade/" + type + "/trade.do?transactionCode=" + code;
	}
	var selectedSideNavId = $(".side-navigation-li.curActive").attr("id");
	if(selectedSideNavId){
		url += "&sideNav=" + selectedSideNavId;
	}
	location.href = url;
}
//侧边切换币种按钮
function indexTabClickAside(sender, codes, showSide, isFavorite) {
	if(showSide == 0) {
		$(".item_data .trade-history-seft").removeClass("hide");
		$(".side-navigation-li").removeClass("curActive");
		$("#sideNavigationLi").addClass("curActive");
		currFavoriteShow = false;
		showContractListon = false;
		if (contactDatas.length > 0) {
			showContractList(contactDatas);
		}
	} else {
		/*$("#marketCurrent").addClass("current");*/
		$("#favoritePartition").removeClass("current");
		$(sender).addClass("curActive").siblings().removeClass("curActive");
		if (isFavorite) {
			currPartition = favorite;
			currFavoriteShow = true;
		} else {
			currPartition = codes;
			currFavoriteShow = false;
		}
		if (contactDatas.length > 0) {
			showContractListSide(contactDatas);
		}
		showContractListon = true;
	}
}
//获取币种list
function showContractListSide(data) {
	var filter = $("#search-box-top").val();
	var tickerHtml = "";
	if (filter && filter.length > 0) {
		for (var index = 0; index < data.length; index++) {
			var map = data[index];
			var bacCSS = ""
			var showname = map.showName;
			if (filter && filter.length > 0) {
				if (!showname.startsWith(filter.toUpperCase())) {
					continue;
				}
			}
			
			var starCss = "icon-shoucang1";
			var fav = favorite.split(",");
			if ($.inArray(map.tranCode, fav) >= 0) {
				starCss = "icon-shoucang";
			}
			
			var colorClass = "sell";
			if (map.lastPrice >= map.firstPrice) colorClass = "buy";
			tickerHtml += "<li class='contract-list-li " + bacCSS + "' onclick='" + "gotoTrade(\"" + map.tranCode + "\");'>"
			+ "<div class='contract-list-cell' ><i class='iconfont " + starCss + " index-table-star' onclick='" + "changeFavorite(this, \""+ map.tranCode + "\");'></i><span>" + map.showName + "</span></div>"
			+ "<div class='contract-list-cell contract-list-cell-price'><span>" + map.lastPrice + "</span></div>"
			+ "<div class='contract-list-cell contract-list-cell-fudu'><span class='" + colorClass + "'>" + map.fudu + "</span></div>"
			+ "</li>";
		}
	} else {
		if (indx > 0) {
			for (var index = 0; index < data.length; index++) {
				var bacCSS = "";
				var map = data[index];
				var codes = currPartition.split(",");
				var ii = $.inArray(map.tranCode, codes);
				if (ii < 0) continue;
				
				var starCss = "icon-shoucang1";
				var fav = favorite.split(",");
				if ($.inArray(map.tranCode, fav) >= 0) {
					starCss = "icon-shoucang";
				}
				var colorClass = "sell";
				if (map.lastPrice >= map.firstPrice) colorClass = "buy";
				tickerHtml += "<li class='contract-list-li " + bacCSS + "' onclick='" + "gotoTrade(\"" + map.tranCode + "\");'>"
				+ "<div class='contract-list-cell' ><i class='iconfont " + starCss + " index-table-star' onclick='" + "changeFavorite(this, \""+ map.tranCode + "\");'></i><span>" + map.showName + "</span></div>"
				+ "<div class='contract-list-cell contract-list-cell-price'><span>" + map.lastPrice + "</span></div>"
				+ "<div class='contract-list-cell contract-list-cell-fudu'><span class='" + colorClass + "'>" + map.fudu + "</span></div>"
				+ "</li>";
			}
		} else {
			var codes = currPartition.split(",");
			for (var i = 0; i < codes.length; i++) {
				var code = codes[i];
				for (var index = 0; index < data.length; index++) {
					var bacCSS = "";
					var map = data[index];
					
					if (map.tranCode != code) continue;
					
					var starCss = "icon-shoucang1";
					var fav = favorite.split(",");
					if ($.inArray(map.tranCode, fav) >= 0) {
						starCss = "icon-shoucang";
					}
					var colorClass = "sell";
					if (map.lastPrice >= map.firstPrice) colorClass = "buy";
					tickerHtml += "<li class='contract-list-li " + bacCSS + "' onclick='" + "gotoTrade(\"" + map.tranCode + "\");'>"
					+ "<div class='contract-list-cell' ><i class='iconfont " + starCss + " index-table-star' onclick='" + "changeFavorite(this, \""+ map.tranCode + "\");'></i><span>" + map.showName + "</span></div>"
					+ "<div class='contract-list-cell contract-list-cell-price'><span>" + map.lastPrice + "</span></div>"
					+ "<div class='contract-list-cell contract-list-cell-fudu'><span class='" + colorClass + "'>" + map.fudu + "</span></div>"
					+ "</li>";
					
					break;
				}
			}
		}
	}
	$("#tickerTbody").html(tickerHtml);
}
