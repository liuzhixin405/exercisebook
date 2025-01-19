/**
 * 四舍5入
 * 
 * @param src
 * @param pos
 *            位数
 * @return
 */
function formatRound(src, pos) {
	return Math.round(src * Math.pow(10, pos)) / Math.pow(10, pos);
}

function getMonth(date){  
    var month = "";  
    month = date.getMonth() + 1; //getMonth()得到的月份是0-11  
    if(month<10){  
        month = "0" + month;  
    }  
    return month;  
}  
//返回01-30的日期  
function getDay(date){  
    var day = "";  
    day = date.getDate();  
    if(day<10){  
        day = "0" + day;  
    }  
    return day;  
}
//返回小时
function getHours(date){
    var hours = "";
    hours = date.getHours();
    if(hours<10){  
        hours = "0" + hours;  
    }  
    return hours;  
}
//返回分
function getMinutes(date){
    var minute = "";
    minute = date.getMinutes();
    if(minute<10){  
        minute = "0" + minute;  
    }  
    return minute;  
}
//返回秒
function getSeconds(date){
    var second = "";
    second = date.getSeconds();
    if(second<10){  
        second = "0" + second;  
    }  
    return second;  
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

/** HashMap */
function HashMap(){  
    /** Map 大小 * */  
    var size = 0;  
    /** 对象 * */  
    var entry = new Object();  
      
    /** 存 * */  
    this.put = function (key , value)  
    {  
        if(!this.containsKey(key))  
        {  
            size ++ ;  
        }  
        entry[key] = value;  
    }  
      
    /** 取 * */  
    this.get = function (key)  
    {  
        return this.containsKey(key) ? entry[key] : null;  
    }  
      
    /** 删除 * */  
    this.remove = function ( key )  
    {  
        if( this.containsKey(key) && ( delete entry[key] ) )  
        {  
            size --;  
        }  
    }  
      
    /** 是否包含 Key * */  
    this.containsKey = function ( key )  
    {  
        return (key in entry);  
    }  
      
    /** 是否包含 Value * */  
    this.containsValue = function ( value )  
    {  
        for(var prop in entry)  
        {  
            if(entry[prop] == value)  
            {  
                return true;  
            }  
        }  
        return false;  
    }  
      
    /** 所有 Value * */  
    this.values = function ()  
    {  
        var values = new Array();  
        for(var prop in entry)  
        {  
            values.push(entry[prop]);  
        }  
        return values;  
    }  
      
    /** 所有 Key * */  
    this.keys = function ()  
    {  
        var keys = new Array();  
        for(var prop in entry)  
        {  
            keys.push(prop);  
        }  
        return keys;  
    }  
      
    /** Map Size * */  
    this.size = function ()  
    {  
        return size;  
    }  
      
    /* 清空 */  
    this.clear = function ()  
    {  
        size = 0;  
        entry = new Object();  
    }  
}


function formatnumber(srcstr,nafterdot){
	if(nafterdot==0){
		return parseInt(srcstr).toString();
	}
	　　var srcstr,nafterdot;
	　　var resultstr,nten;
	　　srcstr = ""+srcstr+"";
	　　var strlen = srcstr.length;
	　　var dotpos = srcstr.indexOf(".",0);
	　　if (dotpos == -1){
	　　　　resultstr = srcstr+".";
	　　　　for (i=0;i<nafterdot;i++){
	　　　　　　resultstr = resultstr+"0";
	　　　　}
	　　　　return resultstr;
	　　}else{
	　　　　if ((strlen - dotpos - 1) >= nafterdot){
	　　　　　　nafter = dotpos + nafterdot + 1;
	　　　　　　nten =1;
	　　　　　　for(j=0;j<nafterdot;j++){
	　　　　　　　　nten = nten*10;
	　　　　　　}
	　　　　　　resultstr = Math.round(parseFloat(srcstr)*nten)/nten;
	　　　　　　return resultstr;
	　　　　}
	　　　　else{
	　　　　　　resultstr = srcstr;
	　　　　　　for (i=0;i<(nafterdot - strlen + dotpos + 1);i++){
	　　　　　　　　resultstr = resultstr+"0";
	　　　　　　}
	　　　　　　return resultstr;
	　　　　}
	　　}
}

function showSuccInfo_kc(text, code) {
	if(text.length>0){
		$("#infomsg_kc").css("display", "block");
		if (code == "success") {
			$("#infomsg_kc").removeClass('warning');
			$("#infomsg_kc").addClass('success');
			$('#amount').val('0');
		}else {
			$("#infomsg_kc").addClass('warning');
			$("#infomsg_kc").removeClass('success');
		}
		
		$("#infomsg_kc").html(text);
		setTimeout('refreshCurrentTab()', 500);
		
		var time = 3000;
		if (code == 'error') {
			time = 5000;
		}
		setTimeout('finishshowSuccInfo_kc("'+code+'")', time);
	} else {
		$("#infomsg_kc").css("display", "none");
	}
}

function refreshCurrentTab() {
	$("#tab-trade .current").click();
}

function finishshowSuccInfo_kc(tcode) {
	$("#infomsg_kc").css("display", "none");
	$("#tab-trade .current").click();
	refreshAccount();
}

function showSuccInfo_d(text,code) {
	if(text.length>0){
		var showBox = $(".show-msg-box");
		if (showBox.size() == 0) {
			$("#alert_tr_pc_buy").css("display", "");
		}
		if (code == "success") {
			$("#infomsg_d").removeClass('warning');
			$("#infomsg_d").addClass('success');
		}else {
			$("#infomsg_d").addClass('warning');
			$("#infomsg_d").removeClass('success');
		}
		
		$("#infomsg_d").html(text);
		
		var time = 3000;
		if (code == 'error') {
			time = 6000;
		}
		setTimeout('finishshowSuccInfo_d("'+code+'")', time);
	} else {
		$("#alert_tr_pc_buy").css("display", "none");
	}
	
}

function finishshowSuccInfo_d(tcode) {
	$("#alert_tr_pc_buy").css("display", "none");
	$("#tab-trade .current").click();
	refreshAccount();
}

function showSuccInfo_k(text,code) {
	if(text.length>0){
		var showBox = $(".show-msg-box");
		if (showBox.size() == 0) {
			$("#alert_tr_pc_sell").css("display", "");
		}
		if (code == "success") {
			$("#infomsg_k").removeClass('warning');
			$("#infomsg_k").addClass('success');
		}else {
			$("#infomsg_k").addClass('warning');
			$("#infomsg_k").removeClass('success');
		}
		var time = 3000;
		if (code == 'error') {
			time = 6000;
		}
		$("#infomsg_k").html(text);
		setTimeout('finishshowSuccInfo_k("'+code+'")', time);
	} else {
		$("#alert_tr_pc_sell").css("display", "none");
	}
}

function showSuccInfo_pcd(text,code) {
	if(text.length>0){
		$("#infomsg_pcd").css("display", "block");
		if (code == "success") {
			$("#infomsg_pcd").removeClass('warning');
			$("#infomsg_pcd").addClass('success');
		}else {
			$("#infomsg_pcd").addClass('warning');
			$("#infomsg_pcd").removeClass('success');
		}
		var time = 3000;
		if (code == 'error') {
			time = 6000;
		}
		$("#infomsg_pcd").html(text);
		setTimeout('finishshowSuccInfo_pcd("'+code+'")', time);
	} else {
		$("#infomsg_pcd").css("display", "none");
	}
}

function finishshowSuccInfo_pcd(tcode) {
	$("#infomsg_pcd").css("display", "none");
	refreshMemberTrans();
	refreshAccount();
}

function showSuccInfo_pck(text,code) {
	if(text.length>0){
		$("#infomsg_pck").css("display", "block");
		if (code == "success") {
			$("#infomsg_pck").removeClass('warning');
			$("#infomsg_pck").addClass('success');
		}else {
			$("#infomsg_pck").addClass('warning');
			$("#infomsg_pck").removeClass('success');
		}
		var time = 3000;
		if (code == 'error') {
			time = 6000;
		}
		$("#infomsg_pck").html(text);
		setTimeout('finishshowSuccInfo_pck("'+code+'")', time);
	} else {
		$("#infomsg_pck").css("display", "none");
	}
}

function finishshowSuccInfo_pck(tcode) {
	$("#infomsg_pck").css("display", "none");
	refreshMemberTrans();
	refreshAccount();
}

function finishshowSuccInfo_k(tcode) {
	$("#alert_tr_pc_sell").css("display", "none");
	$("#tab-trade .current").click();
	refreshAccount();
}


/**
 * 只舍不入
 * 
 * @param src
 * @param pos
 *            位数
 * @return
 */
function formatFloat(src, pos) {
	return Math.floor(src * Math.pow(10, pos)) / Math.pow(10, pos);
}
/**
 * 只入不舍
 * 
 * @param src
 * @param pos
 *            位数
 * @return
 */
function formatFloatUp(src, pos) {
	if(pos==0){
		return parseInt(src);
	}
	return (Math.floor(src * Math.pow(10, pos))+1) / Math.pow(10, pos);
}
/**
 * 四舍5入
 * 
 * @param src
 * @param pos
 *            位数
 * @return
 */
function formatRound(src, pos) {
	return Math.round(src * Math.pow(10, pos)) / Math.pow(10, pos);
}

/**
 * 检查输入
 * 
 * @param oInput
 * @param type
 * @param e
 * @return
 */
function checkInput(oInput, type, e) {
	if (type == 'num') {
		var len = 8;
		var exp = /\d{1,8}\.{0,1}\d{0,3}/;
	} else if (type == 'int') {
		var len = 8;
		var exp = /\d{1,8}/;
	} else if (type == 'bp') {
		var len = 8;
		var exp = /\d{1,8}\.{0,1}\d{0,2}/;
	} else if (type == 'cash') {
		var len = 16;
		var exp = /\d{1,8}\.{0,1}\d{0,5}/;
	} else if (type == 'tinynum') {
		var len = 6;
		var exp = /\d{1,6}\.{0,1}\d{0,3}/;
	} else if (type == 'price2') {
		var len = 5;
		var exp = /\d{1,5}\.{0,1}\d{0,3}/
	} else if (type == 'pricebp') {
		var len = 4;
		var exp = /\d{1,5}\.{0,1}\d{0,4}/
	} else if (type == 'loan') {
		var len = 2;
		var exp = /\d{1,1}\.{0,1}\d{0,2}/
	} else if (type == 'esix') {
		var len = 2;
		var exp = /\d{1,8}\.{0,1}\d{0,5}/
	} else if (type == 'szamount') {
		var len = 8;
		var exp = /\d{1,8}/
	} else if (type == 'szprice') {
		var len = 7;
		var exp = /\d{1,5}\.{0,1}\d{0,1}/;
	} else {
		var len = 4;
		var exp = /\d{1,5}\.{0,1}\d{0,3}/
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

function showMsgBox(code, text) {
	if(text.length > 0){
		if (code == "success") {
			$("#showBoxText").removeClass('warningNew');
			$("#showBoxText").addClass('successNew');
		} else {
			$("#showBoxText").addClass('warningNew');
			$("#showBoxText").removeClass('successNew');
		}
		
		$("#showBoxText").html(text);
		
		var time = 3000;
		if (code == 'error') {
			time = 5000;
		}
		
		$("#showBox").css("display", "block");
		
		setTimeout(function() {
			$("#showBox").css("display", "none");
		}, time + 50);
	}
}

var message = {};
message["zh_CN"] = {
	systeam : "合约-Btcside区块链资产合约交易平台",
	systeam1 : "交易-Btcside区块链资产合约交易平台",
	alertLeverage : "请选择杠杆倍数！",
	alertLegalNum : "请输入合法的数量",
	alertLitOpenNum : "最少开仓数",
	alertGe : "个",
	alertLegalPrice : "请输入合法的价格",
	alertTriPrice : "请输入合法的触发价格",
	alertMaxBuyPrice : "最高可买入价为",
	alertMinSellPrice : "最低可卖出价为",
	curtnotEnoughMoney : "当前账户余额不足，最多可开：",
	curtStop : "当前停盘中",
	nullClosePostion : "可用持仓为0，不能平仓！",
	legalClosePostion : "请输入合法的平仓数量",
	legalClosePostionPrice : "请输入合法的平仓价格",
	nullTrade : "现在不可以交易",
	multipleBuy : "多仓最大可买入",
	nullMinSell : "空仓最小可卖出",
	postionBuy : "买入",
	postionSell : "卖出",
	onOfFlatMore : "做多可用持仓为0，不能平仓！",
	onOfFlatNull : "做空可用持仓为0，不能平仓！",
	onOfFlatPriceRange : "对平价格不在限价范围之内",
	onOfFlatPriceBuy : "对平价格应当在买一价和卖一价之间",
	onOfFlatPriceBlaste : "对平价格应当在买多爆仓和卖空爆仓之间",
	onOfFlatSuccess : "对平成功",
	unableOfFlat : "无法开启对平操作",
	noSellPosition : "暂无卖空持仓",
	noBuyPosition : "暂无买多持仓",
	goLong: "多",
	goShort: "空",
	rules: "您的报价不符合系统的限价规则，当前",
	confirmBy: "您是否确定以 ",
	ok: "确定",
	close: "关闭",
	maxBuy: "最多可买数量",
	maxSell: "最多可卖数量",
	noTrade: "无成交记录",
	noEntrustment: "暂无委托单",
	finish: "完成",
	cancel: "撤单",
	invalid: "未生效",
	executed: "已生效",
	failed: "失败",
	allreadyCancel: "已撤单",
	submitted: "已提交",
	allDeal: "全部成交",
	partDeal: "部分成交",
	partDealCancel: "部分成交撤单",
	olp: "多单开仓",
	csp: "空单平仓",
	blp: "多单爆仓",
	osp: "空单开仓",
	clp: "多单平仓",
	bsp: "空单爆仓",
	buyStopProfit: "多单止盈",
	buyStopLoss: "多单止损",
	sellStopProfit: "空单止盈",
	sellStopLoss: "空单止损",
	burned: "已爆仓",
	warmTip: "温馨提示",
	sureTip: "确定",
	buylong: "买入开多（看涨）",
	shortsell: "卖出开空（看跌）",
	submiting: "提交中...",
	pc: "限价平仓",
	offline: "离线，请重新",
	login: "登录",
	opAfterLogin: "请登录后操作",
	suspend: "合约维护中，交易暂停",
	suspend1: "交易维护中，交易暂停",
	tradeSuspend: "交易暂停",
	noTradeTime: "非交易时间",
	textYour : "您的",
	textBuy : "多单",
	textSell : "空单",
	textContract : "合约",
	textNote1 : "倍杠杆，因标记价格达到强平触发价",
	textNote12: "触发强制平仓，您的订单将以",
	textNote2 : "USDT（爆仓价格）被系统托管，该价格不会在k线上显示。",
	amountError : "金额不正确",
	amountInsufficient : "余额不足",
	transferNum : "转账数量错误",
	amounttrsNull : "数量不能为0或空",
	warmTip: "温馨提示",
	sureTip: "确定",
	riskPause: "风控系统暂停",
	transferOut: "合约转出功能",
	transferIn: "合约转入功能",
	alertNumAndPercent: '请填写数量或者选择百分比!',
	fastClosePosition: '一键平仓',
	PositionNotEnputTips: '平仓数量不能为空',
	higherThanPositionTips: '平仓数量不能高于持仓量',
	fmtLimitedBuyMore: '限价平仓',
	planMarketPrice: '市价',
	markPrice: '标记价格',
	fulloBuyMore: '买入平空',
	fulloSellMore: '卖出平多',
	becameInvalid: '已失效',
	inputTriPrice: '请输入触发价！',
	inputComPrice: '请输入委托价格！',
	oneClickCloseOk: '确认一次全部平仓当前所有仓位？',
	ClosetheBox: '取消',
};
message["zh_HK"] = {
	systeam : "合約-Btcside區塊鏈資產合約交易平臺",
	systeam1 : "交易-Btcside區塊鏈資產合約交易平臺",
	alertLeverage : "請選擇杠杆倍數！",
	alertLegalNum : "請輸入合法的數量",
	alertLitOpenNum : "最少開倉數",
	alertGe : "個",
	alertLegalPrice : "請輸入合法的價格",
	alertTriPrice : "請輸入合法的觸發價格",
	alertMaxBuyPrice : "最高可買入價為",
	alertMinSellPrice : "最低可賣出價為",
	curtnotEnoughMoney : "當前帳戶餘額不足，最多可開：",
	curtStop : "當前停盤中",
	nullClosePostion : "可用持倉為0，不能平倉！",
	legalClosePostion : "請輸入合法的平倉數量",
	legalClosePostionPrice : "請輸入合法的平倉價格",
	nullTrade : "現在不可以交易",
	multipleBuy : "多倉最大可買入",
	nullMinSell : "空倉最小可賣出",
	postionBuy : "買入",
	postionSell : "賣出",
	onOfFlatMore : "做多可用持倉為0，不能平倉！",
	onOfFlatNull : "做空可用持倉為0，不能平倉！",
	onOfFlatPriceRange : "對平價格不在限價範圍之內",
	onOfFlatPriceBuy : "對平價格應當在買一價和賣一價之間",
	onOfFlatPriceBlaste : "對平價格應當在買多爆倉和賣空爆倉之間",
	onOfFlatSuccess : "對平成功",
	unableOfFlat : "無法開啟對平操作",
	noSellPosition : "暫無賣空持倉",
	noBuyPosition : "暫無買多持倉",
	goLong: "多",
	goShort: "空",
	rules: "您的報價不符合系統的限價規則，當前",
	confirmBy: "您是否確定以 ",
	ok: "確定",
	close: "關閉",
	maxBuy: "最多可買數量",
	maxSell: "最多可賣數量",
	noTrade: "無成交記錄",
	noEntrustment: "暫無委託單",
	finish: "完成",
	cancel: "撤單",
	invalid: "未生效",
	executed: "已生效",
	failed: "失敗",
	allreadyCancel: "已撤單",
	submitted: "已提交",
	allDeal: "全部成交",
	partDeal: "部分成交",
	partDealCancel: "部分成交撤單",
	olp: "多單開倉",
	csp: "空單平倉",
	blp: "多單爆倉",
	osp: "空單開倉",
	clp: "多單平倉",
	bsp: "空單爆倉",
	burned: "已爆倉",
	warmTip: "溫馨提示",
	sureTip: "確定",
	buylong: "買入開多（看漲）",
	shortsell: "賣出開空（看跌）",
	submiting: "提交中...",
	pc: "限價平倉",
	offline: "離線，請重新",
	login: "登錄",
	opAfterLogin: "請登錄後操作",
	suspend: "合約維護中，交易暫停",
	suspend1: "交易維護中，交易暫停",
	tradeSuspend: "交易暫停",
	noTradeTime: "非交易時間",
	textYour : "您的",
	textBuy : "多單",
	textSell : "空單",
	textContract : "合約",
	textNote1 : "倍杠杆，因標記價格達到强平觸發價",
	textNote12: "觸發強制平倉，您的訂單將以",
	textNote2 : "USDT（爆倉價格）被系統託管，該價格不會在k線上顯示。",
	amountError : "金額不正確",
	amountInsufficient : "餘額不足",
	transferNum : "轉賬數量錯誤",
	amounttrsNull : "數量不能為0或空",
	warmTip: "溫馨提示",
	sureTip: "確定",
	riskPause: "風控系統暫停",
	transferOut: "合約轉出功能",
	transferIn: "合約轉入功能",
	alertNumAndPercent: '請填寫數量或者選擇百分比!',
	fastClosePosition: '一鍵平倉',
	PositionNotEnputTips: '平倉數量不能為空',
	higherThanPositionTips: '平倉數量不能高於持倉量',
	fmtLimitedBuyMore: '限價平倉',
	planMarketPrice: '市價',
	markPrice: '標記價格',
	fulloBuyMore: '買入平空',
	fulloSellMore: '賣出平多',
	becameInvalid: '已失效',
	inputTriPrice: '請輸入觸發價！',
	inputComPrice: '請輸入委託價格！',
	oneClickCloseOk: '確認一次全部平倉當前所有倉位？',
	ClosetheBox: '取消',
};
message["en_US"] = {
	systeam : " Contract-Btcside Digital Asset Futures Contract Trading Platform",
	systeam1 : "Trade-Btcside Digital Asset Futures Contract Trading Platform",
	alertLeverage : "Please select leverage！",
	alertLegalNum : "Please enter the legal number",
	alertLitOpenNum : "Minimum open",
	alertGe : " ",
	alertLegalPrice : "Please enter a valid price",
	alertTriPrice : "Please enter a valid trigge pricer",
	alertMaxBuyPrice : "The highest buyable price is",
	alertMinSellPrice : "The minimum saleable price is",
	curtnotEnoughMoney : "Insufficient balance in current account, can open: ",
	curtStop : "Current suspension",
	nullClosePostion : "Available positions are 0 and cannot be closed！",
	legalClosePostion : "Please enter the legal closing amount",
	legalClosePostionPrice : "Please enter a valid closing price",
	nullTrade : "Can't trade now",
	multipleBuy : "Longest to buy",
	nullMinSell : "The shortest position can be sold",
	postionBuy : "Buy",
	postionSell : "Sell",
	onOfFlatMore : "Long open position is 0, can not be closed！",
	onOfFlatNull : "Short available position is 0, can not be closed！",
	onOfFlatPriceRange : "The parity price is not within the limit price range",
	onOfFlatPriceBuy : "The parity price should be between buying a price and selling a price",
	onOfFlatPriceBlaste : "The parity price should be between the buy and sell positions",
	onOfFlatSuccess : "Successful on level",
	unableOfFlat : "Unable to open paired operation",
	noSellPosition : "No short-selling position",
	noBuyPosition : "No position",
	goLong: "go long",
	goShort: "go short",
	rules: "Your offer does not conform to the rules of the system, ",
	confirmBy: "Do you make sure that ",
	ok: "Ok",
	close: "Close",
	maxBuy: "Max buy",
	maxSell: "Max sell",
	noTrade: "No transaction record",
	noEntrustment: "No entrustment",
	finish: "Finish",
	cancel: "Cancel",
	invalid: "Invalid",
	executed: "Executed",
	failed: "Failed",
	allreadyCancel: "Withdrawn",
	submitted: "Submitted",
	allDeal: "Filled",
	partDeal: "Partial-filled",
	partDealCancel: "Partial-canceled",
	olp: "Open long positions",
	csp: "Close short positions",
	blp: "Burned long positions",
	osp: "Open short positions",
	clp: "Close long positions",
	bsp: "Burned short positions",
	burned: "Burned",
	warmTip : "Warm prompt",
	sureTip : "Sure",
	buylong: "Open Long（Buy）",
	shortsell: "Open Short（Sell）",
	submiting: "Sending ...",
	pc: "Close a",
	offline: "Please re-",
	login: "login",
	opAfterLogin: "Please login",
	suspend: " contract maintenance, transaction suspended",
	suspend1: " trade maintenance, transaction suspended",
	tradeSuspend: "Transaction suspension",
	noTradeTime: "Non-trading time",
	textYour : "Your ",
	textBuy : " short-selling ",
	textSell : " going long ",
	textContract : " contract ",
	textNote1 : " times leveraged. Because the mark price reaches the strong trigger price ",
	textNote12: " and triggers the forced liquidation, your order will be managed by the system at ",
	textNote2 : " USDT(the burst price), and the price will not be displayed on the k line.",
	amountError : "incorrect amount",
	amountInsufficient : "Insufficient balance",
	transferNum : "Wrong amount",
	amounttrsNull : "Quantity can't be zero or empty",
	warmTip : "Warm prompt",
	sureTip: "Sure",
	riskPause: "Risk control system paused ",
	transferOut: " contract transfer out",
	transferIn: " contract transfer in",
	alertNumAndPercent: 'Please fill in quantity or select percentage!',
	fastClosePosition: 'Quickly Close',
	PositionNotEnputTips: 'The number of positions closed cannot be empty',
	higherThanPositionTips: 'The number of positions closed must not be higher than the amount of positions held',
	fmtLimitedBuyMore: 'Limit close',
	planMarketPrice: 'Market Price',
	markPrice: 'Mark Price',
	fulloBuyMore: 'Close short',
	fulloSellMore: 'Close long',
	becameInvalid: 'Became Invalid',
	inputTriPrice: 'Please enter the trigger price！',
	inputComPrice: 'Please enter the commission price！',
	oneClickCloseOk: 'Confirm a full closing of all current positions？',
	ClosetheBox: 'cancel',
};
message["ja_JP"] = {
		systeam : "Btcsideブロックチェーン資産契約取引プラットフォーム",
		systeam1 : "Btcsideブロックチェーン資産契約取引プラットフォーム",
		alertLeverage : "テッド倍数を選択してください！",
		alertLegalNum : "合法的な数量を入力してください",
		alertLitOpenNum : "オープンポジションの最小数",
		alertGe : "個",
		alertLegalPrice : "法定価格を入力してください",
		alertTriPrice : "法定トリガー価格を入力してください",
		alertMaxBuyPrice : "最高の購入価格は",
		alertMinSellPrice : "最低で値段を売る",
		curtnotEnoughMoney : "経常収支は不十分で、最大で：",
		curtStop : "今は止まっている",
		nullClosePostion : "持っている倉は0として,平倉はできない！",
		legalClosePostion : "合法的な平倉数を入力してください",
		legalClosePostionPrice : "合法的な平倉価格を入力してください",
		nullTrade : "今は取引ができません",
		multipleBuy : "多倉の最大購入可能",
		nullMinSell : "空倉は最小で売ることができる",
		postionBuy : "買う",
		postionSell : "売る",
		onOfFlatMore : "做多可用持仓为0，不能平仓！",
		onOfFlatNull : "做空可用持仓为0，不能平仓！",
		onOfFlatPriceRange : "価格は定価範囲にない",
		onOfFlatPriceBuy : "对平价格应当在买一价和卖一价之间",
		onOfFlatPriceBlaste : "对平价格应当在买多爆仓和卖空爆仓之间",
		onOfFlatSuccess : "对平成功",
		unableOfFlat : "无法开启对平操作",
		noSellPosition : "暂无卖空持仓",
		noBuyPosition : "暂无买多持仓",
		goLong: "以上",
		goShort: "空欄",
		rules: "あなたのオファーはシステムの制限規則に合致しません",
		confirmBy: "よろしいですか ",
		ok: "確定",
		close: "閉じる",
		maxBuy: "利用可能な最大数量",
		maxSell: "利用可能な最大数量",
		noTrade: "取引記録なし",
		noEntrustment: "注文なし",
		finish: "完成",
		cancel: "撤单",
		invalid: "効かない",
		executed: "有効",
		failed: "失敗",
		allreadyCancel: "撤回",
		submitted: "送信済み",
		allDeal: "全部成交",
		partDeal: "部分成交",
		partDealCancel: "部分成交撤单",
		olp: "多单开仓",
		csp: "空单平仓",
		blp: "多单爆仓",
		osp: "空单开仓",
		clp: "複数のシングルクロージング",
		bsp: "空单爆仓",
		burned: "已爆仓",
		warmTip: "ヒント",
		sureTip: "確定",
		buylong: "買い過ぎる（上昇）",
		shortsell: "空を売る（下落）",
		submiting: "送信しています...",
		pc: "ポジションを閉じる",
		offline: "オフライン、再入力してください",
		login: "登録",
		opAfterLogin: "ログイン後に操作してください",
		suspend: "契約のメンテナンス中、取引は一時停止します",
		suspend: "取引のメンテナンス中、取引は一時停止します",
		tradeSuspend: "取引一時停止",
		noTradeTime: "非取引時間",
		textYour : "",
		textBuy : "購入",
		textSell : "空売",
		textContract : "契約",
		textNote1 : "倍レバーは、タグ価格が強平トリガ価格に達したため",
		textNote12: "強制的な平倉をトリガします。ご注文は",
		textNote2 : "USDT（爆倉価格）でシステムに保管されます。この価格はk線に表示されません。",
		amountError : "金額が正しくありません",
		amountInsufficient : "残高不足",
		transferNum : "振込数が間違っています",
		amounttrsNull : "数量を0または空欄にすることはできません",
		warmTip: "ヒント",
		sureTip: "確定",
		riskPause: "リスト管理システムは一時停止する",
		transferOut: "契約転出機能",
		transferIn: "契約転入機能",
		alertNumAndPercent: '数量またはパーセントを選択してください!',
		fastClosePosition: 'Quickly Close',
		PositionNotEnputTips: '平倉の数は空にしてはならない',
		higherThanPositionTips: '平倉の数量は保持倉の数量より高いことができない',
		fmtLimitedBuyMore: '値幅制限',
		planMarketPrice: 'Market Price',
		markPrice: 'Mark Price',
		fulloBuyMore: 'Close short',
		fulloSellMore: 'Close long',
		becameInvalid: 'Became Invalid',
		inputTriPrice: 'Please enter the trigger price！',
		inputComPrice: 'Please enter the commission price！',
		oneClickCloseOk: 'Confirm a full closing of all current positions？',
		ClosetheBox: 'cancel',
	};
message["ko_KR"] = {
		systeam : "계약 -Btcside 블록 체인 자산 계약 거래 플랫폼",
		systeam1 : "Trading-Btcside 블록 체인 자산 계약 거래 플랫폼",
		alertLeverage : "레버리지 배수를 선택하십시오!",
		alertLegalNum : "올바른 수량을 입력하십시오",
		alertLitOpenNum : "최소 열린 위치 수",
		alertGe : "하나",
		alertLegalPrice : "올바른 가격을 입력하십시오",
		alertTriPrice : "법적 트리거 가격을 입력하십시오",
		alertMaxBuyPrice : "최고 입찰가는",
		alertMinSellPrice : "최저 입찰가는",
		curtnotEnoughMoney : "현재 계정 잔액이 부족하여 최대 :",
		curtStop : "현재 정학 중",
		nullClosePostion : "사용 가능한 위치는 0이며 닫을 수 없습니다!",
		legalClosePostion : "올바른 가격을 입력하십시오",
		legalClosePostionPrice : "올바른 종가를 입력하십시오",
		nullTrade : "지금 거래 할 수 없습니다",
		multipleBuy : "다중 포지션이 가장 많이 구입할 수 있습니다",
		nullMinSell : "짧은 포지션 판매 가능",
		postionBuy : "구매",
		postionSell : "매도",
		onOfFlatMore : "더 많은 열린 위치가 0이면 닫을 수 없습니다!",
		onOfFlatNull : "사용 가능한 짧은 위치는 0이며 닫을 수 없습니다!",
		onOfFlatPriceRange : "고정 가격이 한도 범위 내에 있지 않습니다",
		onOfFlatPriceBuy : "고정 가격의 경우 한 가격과 한 가격 사이에서 구매해야합니다.",
		onOfFlatPriceBlaste : "고정 가격의 경우 여러 개의 짧은 포지션과 짧은 포지션 사이에서 구매해야합니다.",
		onOfFlatSuccess : "성공",
		unableOfFlat : "플랫 작업을 열 수 없습니다",
		noSellPosition : "짧은 포지션 없음",
		noBuyPosition : "더 이상의 직책을 사지 마십시오",
		goLong: "더",
		goShort: "비우기",
		rules: "귀하의 제안이 현재 시스템의 제한 규칙을 충족하지 않습니다",
		confirmBy: "확실합니까? ",
		ok: "결정",
		close: "닫기",
		maxBuy: "사용 가능한 최대 수량",
		maxSell: "사용 가능한 최대 수량",
		noTrade: "거래 기록이 없습니다",
		noEntrustment: "주문 없음",
		finish: "완료",
		cancel: "인출",
		invalid: "효과적이지 않다",
		executed: "효과적",
		failed: "실패",
		allreadyCancel: "철회",
		submitted: "제출",
		allDeal: "모든 거래",
		partDeal: "부분 거래",
		partDealCancel: "부분 거래 인출",
		olp: "여러 개의 열린 위치",
		csp: "빈 주문",
		blp: "다중 단일 버스트",
		osp: "단일 포지션 열기",
		clp: "여러 단일 위치",
		bsp: "빈 싱글 샷",
		burned: "폭발",
		warmTip: "팁",
		sureTip: "결정",
		buylong: "공매수（강세장 예상）",
		shortsell: "공매도（약세장 예상）",
		submiting: "제출...",
		pc: "위치를 닫습니다",
		offline: "오프라인, 다시",
		login: "로그인",
		opAfterLogin: "작동하려면 로그인하십시오",
		suspend: "계약 유지 보수 중 거래가 일시 중지됨",
		suspend1: "거래 유지 보수 중에 거래가 일시 중지됨",
		tradeSuspend: "거래 중단",
		noTradeTime: "비 거래 시간",
		textYour : "당신",
		textBuy : "청구서를 지불",
		textSell : "빈 주문",
		textContract : "계약",
		textNote1 : "배로대, 지수 가격으로 강평의 촉발가격에 이르다",
		textNote12: "강제 평창 을 촉발하여, 당신의 주문서 는 사용할 것입니다",
		textNote2 : " USDT(버스트 가격)로 주문을 관리하며 가격은 k 라인에 표시되지 않습니다.",
		amountError : "금액이 잘못되었습니다",
		amountInsufficient : "잔액 부족",
		transferNum : "잘못된 금액",
		amounttrsNull : "수량은 0이거나 비어있을 수 없습니다",
		warmTip: "팁",
		sureTip: "결정",
		riskPause: "에어 컨트롤 시스템 중단",
		transferOut: "계약 롤아웃 기능",
		transferIn: "계약 이체 기능",
		alertNumAndPercent: '수량이나 백분율을 기입하십시오!',
		fastClosePosition: 'Quickly Close',
		PositionNotEnputTips: '매진 창고의 물량을 비워서는 안 된다.',
		higherThanPositionTips: '평판 매장량이 전체 매장량보다 많아서는 안 된다.',
		fmtLimitedBuyMore: 'Limit close',
		planMarketPrice: 'Market Price',
		markPrice: 'Mark Price',
    	fulloBuyMore: 'Close short',
		fulloSellMore: 'Close long',
		becameInvalid: 'Became Invalid',
		inputTriPrice: 'Please enter the trigger price！',
		inputComPrice: 'Please enter the commission price！',
		oneClickCloseOk: 'Confirm a full closing of all current positions？',
		ClosetheBox: 'cancel',
	};
message["ru_RU"] = {
		systeam : "Платформа для торговли цифровыми активами Контракт Btcside",
		systeam1 : "Торговая платформа для торговли фьючерсными контрактами на цифровые активы Торговля-Btcside",
		alertLeverage : "Пожалуйста, выберите кредитное плечо！",
		alertLegalNum : "Пожалуйста, введите юридический номер ",
		alertLitOpenNum : "Минимальный открытый ",
		alertGe : " ",
		alertLegalPrice : "Пожалуйста, введите действительную цену ",
		alertTriPrice : "Пожалуйста, введите действительную цену триггера ",
		alertMaxBuyPrice : "Самая высокая цена покупки ",
		alertMinSellPrice : "Минимальная цена продажи",
		curtnotEnoughMoney : "Недостаточный баланс на текущем счете, может открыть: ",
		curtStop : "Текущая приостановка",
		nullClosePostion : "Доступные позиции - 0 и не могут быть закрыты！",
		legalClosePostion : "Пожалуйста, введите «законную» сумму закрытия ",
		legalClosePostionPrice : "Пожалуйста, введите действительную цену закрытия ",
		nullTrade : "Не могу торговать сейчас ",
		multipleBuy : "Длительно к покупке",
		nullMinSell : "Самая короткая позиция может быть продана ",
		postionBuy : "Купить",
		postionSell : "Продать",
		onOfFlatMore : "Длинная открытая позиция - 0, не может быть закрыта！",
		onOfFlatNull : "Короткая доступная позиция – 0, не может быть закрыта！",
		onOfFlatPriceRange : "Цена паритета не находится в пределах диапазона цен ",
		onOfFlatPriceBuy : "Цена паритета должна быть между ценой покупки и ценой продажи ",
		onOfFlatPriceBlaste : "Цена паритета должна быть между позициями покупки и продажи ",
		onOfFlatSuccess : "Успешно на уровне ",
		unableOfFlat : "Невозможно открыть парную операцию ",
		noSellPosition : "Нет коротко-продаваемых позиций",
		noBuyPosition : "Нет позиции",
		goLong: "Идти в длинную",
		goShort: "Идти в короткую",
		rules: "Ваше предложение не соответствует правилам системы, ",
		confirmBy: "Вы уверены, что ",
		ok: "Ок",
		close: "Закрыть",
		maxBuy: "Максимальная продажа",
		maxSell: "Максимальная продажа",
		noTrade: "Нет записи транзакции",
		noEntrustment: "Нет согласия\доверия",
		finish: "Конец",
		cancel: "Отмена",
		invalid: "Недействительный",
		executed: "Выполненный",
		failed: "Не удалось",
		allreadyCancel: "Снятые",
		submitted: "Отправлено",
		allDeal: " Заполненный ",
		partDeal: "Частично заполненный",
		partDealCancel: "Частично отмененный",
		olp: "Открыть длинные позиции",
		csp: "Закрыть короткие позиции",
		blp: "Сожженные длинные позиции",
		osp: "Открыть короткую позицию",
		clp: "Закрыть длинные позиции",
		bsp: "Сожженные короткие позиции",
		burned: "Сожженные",
		warmTip : "Важное напоминание\подсказка ",
		sureTip : "Конечно",
		buylong: "Открыть длинную（Купить）",
		shortsell: "Открыть короткую（Продать）",
		submiting: "Отправка ...",
		pc: "Закрыть",
		offline: "Пожалуйста, повторно-",
		login: "Логин",
		opAfterLogin: "Пожалуйста, войдите\логин",
		suspend: "контрактное обслуживание, транзакция приостановлена",
		suspend1: "торговое обслуживание, транзакция приостановлена",
		tradeSuspend: "Приостановка транзакции ",
		noTradeTime: "Неторговое время ",
		textYour : "Ваша ",
		textBuy : "короткая продажа ",
		textSell : "идти в длинную ",
		textContract : "контракт ",
		textNote1 : "период кредитного плеча. Потому что цена индекса достигает сильной цены триггера ",
		textNote12: "и инициирует принудительную ликвидацию, ваш заказ будет обрабатываться системой по адресу ",
		textNote2 : "USDT (цена разрыва), и цена не будет отображаться в строке .",
		amountError : "неверное количество",
		amountInsufficient : "Недостаточный баланс ",
		transferNum : "Неверное количество",
		amounttrsNull : "Количество не может быть нулевым или пустым",
		warmTip : "Важное напоминание\подсказка",
		sureTip: "Конечно",
		riskPause: "Система контроля рисков приостановлена",
		transferOut: "перевод контракта",
		transferIn: "перевод контракта в",
		alertNumAndPercent: 'Пожалуйста, заполните сумму или выберите процент!',
		fastClosePosition: 'Quickly Close',
		PositionNotEnputTips: 'Количество разрядов не должно быть пустым',
		higherThanPositionTips: 'Количество горизонтальных уровней не должно превышать количество держателей',
		fmtLimitedBuyMore: 'Limit close',
		planMarketPrice: 'Market Price',
		markPrice: 'Mark Price',
		fulloBuyMore: 'Close short',
		fulloSellMore: 'Close long',
		becameInvalid: 'Became Invalid',
		inputTriPrice: 'Please enter the trigger price！',
		inputComPrice: 'Please enter the commission price！',
		oneClickCloseOk: 'Confirm a full closing of all current positions？',
		ClosetheBox: 'cancel',
	};
var MSG = message[locale];