$(function(){
	$("#changeUpDown").click(function() {
		indx++;
		
		var data = marketDatas;
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
		var codes = window.localStorage.getItem("bfx_spot_fav");

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
	} else {
		currFavoriteShow = true;
		$("#favoritePartition").click();
	}
	
//	refreshTicker();
//	setInterval(refreshContractsTicker, 6000);
});

var currFavoriteShow = true;
var indx = 0;
var marketDatas = [];
var interval;
var currFirstPrice = 0;
var isLastPriceChange = false;
var type = "spot";

function indexTabClick(sender, type) {
	$(sender).addClass("current").siblings().removeClass();
	if (type == 0) {
		$(".item_data .trade-history-seft-spot").removeClass("hide");
		$(".item_data .trade-history-Content-spot").addClass("hide");
		currFavoriteShow = true;
	} else if (type == 1){
		$(".item_data .trade-history-seft-spot").removeClass("hide");
		$(".item_data .trade-history-Content-spot").addClass("hide");
		currFavoriteShow = false;
	} else if (type == 2){
		$(".item_data .trade-history-seft-spot").addClass("hide");
		$(".item_data .trade-history-Content-spot").removeClass("hide");
//		currFavoriteShow = false;
	}
	
	if (marketDatas.length > 0) {
		showList(marketDatas);
	}
}

function refreshTicker() {
	$.ajax({
        type: "GET",
        dataType: "json",         
        url: basePath +"futuresApi/tickersData.do?type=" + type,
        data: {},
        success: function(data) {	        	    
        	if(data.code == 0) {
        		var dataList = eval('(' + data.data + ')');
        		sortContractList(dataList);
	    	}
        }
	});
}

function searchChange() {
	var data = marketDatas;
	sortContractList(data);
}

function sortContractList(data) {
	if (indx == 0) {
		marketDatas = data;
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
			marketDatas = temp.reverse()
		} else {
			marketDatas = temp;
		}
	}
	
	showList(marketDatas);
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
		showList(marketDatas);
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
		window.localStorage.setItem("bfx_spot_fav", JSON.stringify(favorite.split(",")));
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

function showList(data) {
	var filter = $("#search-box").val();
	var tickerHtml = "";
	var bqCode = $('#transaction_code').val();
	for (var index = 0; index < data.length; index++) {
		var map = data[index];
		
		var bacCSS = "";
		var colorClass = "sell";
		
		if (bqCode == map.pairName) {
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
			
			if (map.lastPrice >= map.firstPrice) colorClass = "buy";
			$("#lastPrice").html("<span class='" + colorClass + "'>" + map.lastPrice + "</span>");
		}
		if (currFavoriteShow) {
			var codes = favorite.split(",");
			var ii = $.inArray(map.pairName, codes);
			if (ii < 0) continue;
		}
		
		var marketName = map.marketName;
		var coinName = map.coinName;
		if (filter && filter.length > 0) {
			var reg = new RegExp(filter.toUpperCase());
			if (!marketName.match(reg) && !coinName.match(reg)) {
				continue;
			}
		}
		
		var starCss = "icon-shoucang1";
		var fav = favorite.split(",");
		if ($.inArray(map.pairName, fav) >= 0) {
			starCss = "icon-shoucang";
		}
		
		if (map.lastPrice >= map.firstPrice) colorClass = "buy";
		tickerHtml += "<li class='contract-list-li " + bacCSS + "' onclick='" + "gotoTrade("+ map.pairId + ");'>"
			+ "<div class='contract-list-cell' ><i class='iconfont " + starCss + " index-table-star' onclick='" + "changeFavorite(this, \""+ map.pairName + "\");'></i><span>" + map.coinName + "/" + map.marketName + "</span></div>"
			+ "<div class='contract-list-cell contract-list-cell-price'><span class='" + colorClass + "'>" + map.lastPrice + "</span></div>"
			+ "<div class='contract-list-cell contract-list-cell-fudu'><span class='" + colorClass + "'>" + map.fudu + "</span></div>"
			+ "</li>";
	}
	$("#tickerTbody").html(tickerHtml); 
}

function gotoTrade(id){
	var url = basePath + "spotTrade/spotTrade.do?coinPairId=" + id;
	location.href = url;
}

