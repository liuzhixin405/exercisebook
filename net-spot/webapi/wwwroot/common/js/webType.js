$(function() {
	checkBrowser();//判断浏览器
	function checkBrowser() {
	    var ua = navigator.userAgent.toLocaleLowerCase();
	    var screenWidth=window.screen.width;
	    var bodyHeight=$("body").height();
	    /*console.log("屏幕宽度 "+screenWidth);
	    console.log("内容高度 "+bodyHeight);*/
	    var browserType = null;
	    if (ua.match(/firefox/) != null) {
	    	//console.log("火狐")
	        browserType = "火狐";
	        //针对火狐浏览器在此处写入样式
	        $(".marginrft").css("margin-right", "0");
			$(".trade-detail .percent-sell, .trade-detail .buy-percent, .trade-detail .percent, a.role-dexx, a.role-dex").css("font-family", "'Microsoft YaHei', Arial");
	    } else if (ua.match(/ubrowser/) != null) {
	    	//console.log("UC")
	        browserType = "UC";
	        //针对UC浏览器在此处写入样式
	        $(".trade-detail .percent-sell, .trade-detail .buy-percent, .trade-detail .percent").css({"font-family" : "'Microsoft YaHei', Arial", "font-weight" : "normal"});
			$("a.role-dexx, a.role-dex, .sub-btn-entrust, .sub-btn-entrust-plan, .sub-entrust-btn, .sub-entrust-btn-buy").css({"font-family" : "'Microsoft YaHei', Arial", "font-weight" : "normal"});
	    } else if (ua.match(/chrome/) != null) {
	        var is360 = _mime("type", "application/vnd.chromium.remoting-viewer");
	        if (is360) {
	        	//console.log("360")
	            browserType = '360';
	        } else {
	        	//console.log("谷歌")
	            browserType = "谷歌";
	        }
	    } else if (ua.match(/msie/) != null || ua.match(/trident/) != null) {
	    	//console.log("IE")
	    	//IE 11 浏览器不渲染字体图标，部分按钮切换功能不生效，部分字体显示不正常，还没完全解决，待完善
	        browserType = "IE";
	        browserVersion = ua.match(/msie ([\d.]+)/) != null ? ua.match(/msie ([\d.]+)/)[1] : ua.match(/rv:([\d.]+)/)[1];
	        $(".trade-main").css("padding", "0px 15px");
			$(".trade-detail .percent-sell, .trade-detail .buy-percent, .trade-detail .percent, .sub-btn-entrust, .sub-btn-entrust-plan, .sub-entrust-btn, .sub-entrust-btn-buy").css({"font-family" : "'Microsoft YaHei', Arial", "font-weight" : "normal"});
			/*$(".icon-search").html("<img style='width: 14px;height: 14px;' src='../common/images/search@3x.png' alt='search'/>");*/
			$("#search-box-wrap-top .icon-sousuo").css({"top" : "6px" , "left" : "234px"})
			$(".trade-search-box-top:-ms-input-placeholder").css("color", "#586876");
			$(".trade-search-box-top::-ms-clear").css("display", "none");
			/*$(".index-tab-title .contract-search .contract-search-icon").html("<img style='width: 14px;height: 14px;' src='../common/images/search@3x.png' alt='search'/>")*/
	        $(".index-tab-title .contract-search .contract-search-icon").css({"top" : "2px" , "left" : "-6px"});
	        $(".index-block-txt h4, .index-block-txt p, h3.news-refer, a.role-dexx, a.role-dex, .download-area-text, .download-area-text-Tagline, .iosDownBtn, .androidDownBtn").css({"font-family" : "'Microsoft YaHei', Arial", "font-weight" : "normal"});
			//针对IE浏览器在此处写入样式
	    }else if (ua.match(/safari/) != null) {
	    	//console.log("Safari")
	        browserType = "Safari";
	    } else if (ua.match(/opera/) != null) {
	    	//console.log("欧朋")
	        browserType = "欧朋";
	        //针对欧朋浏览器在此处写入样式
	    } else if (ua.match(/metasr/) != null) {
	    	//console.log("搜狗")
	        browserType = "搜狗";
	        //针对搜狗浏览器在此处写入样式
	    } else if (ua.match(/tencenttraveler/) != null || ua.match(/qqbrowse/) != null) {
	    	//console.log("QQ")
	        browserType = "QQ";
	        //针对QQ浏览器在此处写入样式
	    } else if (ua.match(/maxthon/) != null) {
	    	//console.log("遨游")
	        browserType = "遨游";
	        //针对遨游浏览器在此处写入样式
	    } 
	    function _mime(option, value) {
	        var mimeTypes = navigator.mimeTypes;
	        for (var mt in mimeTypes) {
	            if (mimeTypes[mt][option] == value) {
	                return true;
	            }
	        }
	        return false;
	    }
	    /*console.log(browserType);*/
	    return browserType;//返回浏览器类型
	}
})
