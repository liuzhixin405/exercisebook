/*******************************************************************************
 * Author : xuyw * Version: v1.0 * Email : xyw10000@163.com *
 ******************************************************************************/

function randomnum(smin, smax) { // 获取2个值之间的随机数
	var Range = smax - smin;
	var Rand = Math.random();
	return (smin + Math.round(Rand * Range));
}

function runzp() {

	var myreturn = new Object;

	$.get("/campaign/findlottery.do", function(data, status) {
		if (data.type == -1) {
			windows.location.href = "/login/login.do";
		} else if (data.type == 0) {
			alert("对不起, 您的可用抽奖次数已用完! 快去开仓吧! ");
			return null;
		} else if (data.type > 0) {
			if (data.type == 1)
				myreturn.message = "恭喜你, 获得" + data.award + "积分";
			else if (data.type == 2)
				myreturn.message = "恭喜你, 获得" + data.award + "BTC";
			myreturn.angle = data.jd;
		}
	}, "json");

	return myreturn;
}
