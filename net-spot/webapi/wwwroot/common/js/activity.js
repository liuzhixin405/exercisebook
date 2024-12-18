function refrushRank(type){
	
	jQuery("#openDiv").find(".cur").each(function(){
		jQuery(this).removeClass("cur");
	});
	jQuery("#openBtn"+type).addClass("cur");
	
	var url = "/active/refrushRank.do?type="+type+"&random="+Math.round(Math.random()*100);
	jQuery("#openList").load(url,null,function (data){
	});
	
}
function showreplayFutureActiveBlock(){
	dialogBoxShadow();
	document.getElementById("replayFutureActive").style.display="block";
}
function closereplayFutureActiveBlock(){
	dialogBoxHidden();
	document.getElementById("replayFutureActive").style.display="none";
}
function showGiftGetBlock(elme){
	dialogBoxShadow();
//	document.getElementById("dialogBoxShadowActive").style.display="block";
	document.getElementById("giftGet").style.display="block";
	document.getElementById("giftGetInformation").innerHTML = elme;
}

function closeGiftGetBlock(){
	dialogBoxHidden();
//	document.getElementById("dialogBoxShadowActive").style.display="none";
	document.getElementById("giftGet").style.display="none";
	document.getElementById("giftGetInformation").innerHTML = "";
}

function rotateFunc (awards,angle,text){
	jQuery('#lotteryBtn').stopRotate();
	jQuery("#lotteryBtn").rotate({
		angle:0, 
		duration: 5000, 
		animateTo: angle+2880, //angle是图片上各奖项对应的角度，1440是我要让指针旋转4圈。所以最后的结束的角度就是这样子^^
		callback:function(){
			showGiftGetBlock(text);
		}
	}); 
}
function freshDraw(type){
    if(1==type){
//        jQuery("#tabHref1").atrr("class","cur");
        document.getElementById("tabHref1").className="cur";
        document.getElementById("tabHref2").className="";
//        jQuery("#tabHref2").atrr("class","");
    }else{
        document.getElementById("tabHref2").className="cur";
        document.getElementById("tabHref1").className="";
    }

    var url  ="/active/drawData.do?type="+type+"&random="+Math.round(Math.random()*100);
    jQuery("#pageDrawDiv").load(url,null,function (data){
    });
}
var isclick=true;
function round(){

    /**if(isclick){
        return;
    }
    isclick =true;
    var url = ""+Math.round(Math.random()*100);
    jQuery.post(url,null,function(data){
        isclick=false;
        document.getElementById("isChance").innerHTML = 0;
        document.getElementById("drawTries").innerHTML =moreTries0;
        if(data==-1){
            showlogin(0);
        }else if(data == -2){
            showGiftGetBlock(jsNoChanceToday+"<br/><a href='/future/future.do'>"+jsGo2TradeCenter+"</a>");
            document.getElementById("iconCueImg").className = "iconCue iconCue1";
        }else if(data == -3){
            showGiftGetBlock(jsFinisRoateToday);
            document.getElementById("iconCueImg").className = "iconCue iconCue1";
        }else if(data==9){//类型9为宙斯矿机 1为银鱼矿机
            rotateFunc(0,112,jsMiner);
            document.getElementById("iconCueImg").className = "iconCue iconCue3";
        }else if(data==2){
            rotateFunc(2,202,js1BTC);
            document.getElementById("iconCueImg").className = "iconCue iconCue3";
        }else if(data==3){
            rotateFunc(3,247,js0point1BTC);
            document.getElementById("iconCueImg").className = "iconCue iconCue3";
        }else if(data==4){
            rotateFunc(3,292,jsRoseonly);
            document.getElementById("iconCueImg").className = "iconCue iconCue3";
        }else if(data==5){
            rotateFunc(3,337,js1LTC);
            document.getElementById("iconCueImg").className = "iconCue iconCue3";
        }else if(data==6){
            rotateFunc(3,22,js0point1LTC);
            document.getElementById("iconCueImg").className = "iconCue iconCue3";
        }else if(data==7){
            rotateFunc(3,67,js0point01LTC);
            document.getElementById("iconCueImg").className = "iconCue iconCue3";
        }else {
            rotateFunc(1,157,loseHope);
            document.getElementById("iconCueImg").className = "iconCue iconCue2";
        }
    });*/
}
//type =0 :获奖名单 type=1 我的奖品
function entrustGift(type){
	var currentPage = document.getElementById("activityCurrentPage").value;
	var url  ="/active/draw.do?type="+type+"&currentPage="+currentPage+"&random="+Math.round(Math.random()*100);
	jQuery("#pageDrawDiv").load(url,null,function (data){
	});
}

//function worldCupVote(matchId,name,result){
//	var url = "/active/worldCupVote.do?matchId="+matchId+"&result="+result+"&random="+Math.round(Math.random()*100);
//	document.getElementById("worldCupName").value = name;
//	var person = "#peopleStation1";
//	if(matchId == 3){
//		person = "#peopleStation2";
//	}else if(matchId == 4){
//		person = "#peopleStation3";
//	}
//	jQuery.post(url,null,function(data){
//		if(data==0){
//			var callback={okBack:function(){window.location.href= document.getElementById("coinMainUrl").value+"/active/worldCup.do";},noBack:function(){return false;}};
//			okcoinAlertNew("投票成功，谢谢支持！", matchId, name, null, callback, null);
////			showReturnCastVote(0);
//		}else if(data==-1){
//			showlogin(0);
//		}else if(data==-2){
//			//"该场次还没开始"
//			okcoinAlertNew("当次投票还没开始，谢谢支持！", matchId, name, null, null, null);
////			showReturnCastVote(-2);
//		}else if(data ==-3){
//			//投票已经结束
//			okcoinAlertNew("当次投票已经结束啦，谢谢支持！", matchId, name, null, null, null);
////			showReturnCastVote(-3);
//		}else if(data ==-4){
//			okcoinAlertNew("已经投过票啦，谢谢支持！", matchId, name, null, null, null);
////			showReturnCastVote(-4);
//		}
//	});
//}

function sendWeibo(amount,matchId,name,code,type){
	var url = document.getElementById("worldCupUrl").value;
	var mostGoal = document.getElementById("mostCupUserNum").value
	var desc;
	if(matchId != -1){
		var std = "worldCupCastResult"+matchId;
		var str = document.getElementById(std).value;
		if(type == 2 && str == 1){
			amount = document.getElementById("worldCupCastHomeName"+matchId).value;
		}else if(type == 2 && str == 2){
			amount = document.getElementById("worldCupCastAwayName"+matchId).value;
		}else{
			amount = name;
		}
	}
	
//	url += code;
	if(type==1){
		desc = '哎呦不错喔～ 在#我是世界杯预测帝#活动中，我已得到'+amount+'分，目前预测帝得'+mostGoal+'分。小伙伴们一起来 @OKCoin比特币 ，争做预测帝，免费得1比特币和 @SF银鱼矿机 刀片矿机 [给力]，参与和转发微博都有68coin.com提供的彩票拿，走你～';
	}else if(type == 2){
		desc =  '有点意思哇！在#我是世界杯预测帝#活动中，我支持了'+amount+'，轻松一注彩票到手 。一起来 @OKCoin比特币 参加吧，预测帝可免费得1比特币和 @SF银鱼矿机 刀片矿机[给力]，参与和转发微博的小伙伴都有68coin.com提供的彩票拿，走你～';
	}else if(type == 3){
		desc =  registeringaccount;
	}else{
		desc =  '太给力了！在#我是世界杯预测帝#活动中，我成功预测'+amount+'小组赛得胜方，得'+code+'分 [哈哈]目前预测帝得'+mostGoal+'分。伙伴们一起来 @OKCoin比特币 ，争做预测帝，免费得1比特币和 @SF银鱼矿机 刀片矿机 [给力]，参与和转发微博都有68coin.com提供的彩票拿，走你～';
	}
//	var codeSpanPreUrl = document.getElementById("codeSpanPreUrl").value;
	var p = {
		url:url, /*获取URL，可加上来自分享到QQ标识，方便统计*/
		desc:'', /*分享理由(风格应模拟用户对话),支持多分享语随机展现（使用|分隔）*/
		title:desc, /*分享标题(可选)*/
		summary:'', /*分享摘要(可选)*/
//		pic:codeSpanPreUrl+'/image/wap_luck/weibopic1.jpg?1'+'||'+codeSpanPreUrl+'/image/wap_luck/logo.jpg'+'||'+codeSpanPreUrl+'/image/wap_luck/weibopic2.jpg', /*分享图片(可选)*/
		pic:'', /*分享图片(可选)*/
		flash: '', /*视频地址(可选)*/
		site:'', /*分享来源(可选) 如：QQ分享*/
		style:'102', 
		width:63,
		height:24
	};
	var s = [];
	for(var i in p){
		s.push(i + '=' + encodeURIComponent(p[i]||''));
	}
	var openurl = "http://service.weibo.com/share/share.php?"+s.join('&');
	window.open(openurl);
}
function sendToSeas(type){
	var url = document.getElementById("worldCupUrl").value;
	var openurl = "";
	if(type==1){//分享到facebook
		openurl = "http://www.facebook.com/sharer.php?u="+url;
	}else if(type==2){//分享到twitter
		openurl = "http://twitter.com/home/?status="+url;
	}
	
	window.open(openurl);
}

function sendQQ(type){
		var url = document.getElementById("worldCupUrl").value;
		var desc;
		if(type ==1){
			desc =  registeringaccount;
		}
		var p = {
			url:url, /*获取URL，可加上来自分享到QQ标识，方便统计*/
			desc:desc, /*分享理由(风格应模拟用户对话),支持多分享语随机展现（使用|分隔）*/
			title:'邀请好友注册交易，海量反馈', /*分享标题(可选)*/
			summary:'我在OKCOin，邀请你成为我的好友，和我及另外一些不错的朋友们一起讨论比特币！', /*分享摘要(可选)*/
			pics:'', /*分享图片(可选)*/
			flash: '', /*视频地址(可选)*/
			site:'', /*分享来源(可选) 如：QQ分享*/
			style:'102',
			width:63,
			height:24
		};
		var s = [];
		for(var i in p){
			s.push(i + '=' + encodeURIComponent(p[i]||''));
		}
		var openurl = "http://sns.qzone.qq.com/cgi-bin/qzshare/cgi_qzshare_onekey?"+s.join('&');
		window.open(openurl);
}
function sendWeixin(type){
	if(document.getElementById("output").style.display=="none") {
		document.getElementById("output").style.display="";
//        jQuery("#output").fadeIn();
    }else {
        document.getElementById("output").style.display="none";
//    	jQuery("#output").fadeOut();
    }
	
}

function initclip(copyclickid,copytextid) {
	if(navigator.userAgent.indexOf("MSIE")>0){
		jQuery('#'+copyclickid).click(function(){
			window.clipboardData.setData("Text", jQuery('#'+copytextid).val());
		});
	}else{
		jQuery('#'+copyclickid).zclip({
			path:'/js/jquery/ZeroClipboard.swf',
			copy:jQuery('#'+copytextid).val()
		});
	}
}
function sendWeixinFriend(){
	dialogBoxShadowMove(false);
	document.getElementById("WeiXinLayer").style.display="block";
	addMoveEvent("dialog_title","dialog_content");
}
function closeWechat(tag){
	dialogBoxHidden();
	document.getElementById('WeiXinLayer').style.display = 'none';
}
function okcoinAlertNew(str,matchId,name,pro,callback,btnTitle) {
	/*
	*@str 传入提示内容
	*@pro 可选，取消按钮
	*返回值，确定为true，取消和关闭都为false
	*/
		if(btnTitle == "" || btnTitle == "undefined" || btnTitle==null){
			btnTitle = oksure;
		}
		var d = document, obj , tempStr = [] , dEle = d.documentElement , ieSix = (!window.XMLHttpRequest);
		var callback=callback||{okBack:function(){return true;},noBack:function(){return false;}};
		function gid(id){return d.getElementById(id);}
		if(!!gid("okcoinAlert")){		
			d.body.removeChild(gid("okcoinAlert"));
		}
		obj = d.createElement("div");	
		obj.className="okcoinPop";
		obj.id="returnCastVote";	
		
		tempStr.push('<div class="castVoteBody" id="castVoteBody">');
		tempStr.push('<a id="castVoteClosed" class="castVote_closed" href="javascript:void(0);"></a>');
		tempStr.push('<span class="castVoteResult" id="castVoteResult">'+str+'</span>');
		tempStr.push('<a class="castVoteButton castVoteButtonOne" id="castVoteButtonOk">确定</a>');
		tempStr.push('<a class="castVoteButton castVoteButtonTwo" id="castVoteButtonShare" target="_blank"onclick="javascript:sendWeibo(1,'+matchId+',\''+name+'\',0,2)">分享到新浪微博</a>');
		tempStr.push('</div>');
		obj.innerHTML=tempStr.join("");
		d.body.appendChild(obj);
		dialogBoxShadow();
		var os = obj.style;
		os.display="block";
		var temptop = d.body.scrollTop+d.documentElement.scrollTop;
		os.left=(dEle.clientWidth-obj.clientWidth)/2+dEle.scrollLeft+"px"; 	
		os.top=(dEle.clientHeight-obj.clientHeight)/2+dEle.scrollTop+d.body.scrollTop+"px";	
		if(ieSix){os.top=(dEle.clientHeight-obj.clientHeight)/2+temptop+"px";}
		os.position ="absolute";
		os.zIndex="100000";
		function fixed(){
			os.top=(dEle.clientHeight-obj.clientHeight)/2+dEle.scrollTop+d.body.scrollTop+"px";			
		}		
		if(ieSix){
			addEV(window,"scroll",fixed);
		}else{
			addEV(window,"scroll",fixed);
		}
		function hideObj(){
			d.body.removeChild(obj);
			dialogBoxHidden();
			os.display="none";
			if(ieSix){
				window.detachEvent("onscroll",fixed);
			}
		}		
		gid("castVoteClosed").onclick=function(){
			hideObj();
			if(!!callback.noBack){
				callback.noBack();
			}
			return false;
		};
		gid("castVoteButtonOk").onclick=function(){
			hideObj();
			if(!!callback.okBack){
				callback.okBack();
			}
			return true;
		};
		return true;
	}
function applyForActive(){
	var url = "/hd/submitParticipate.do?random="+Math.round(Math.random()*100);
	var phone = trim(document.getElementById("applyPhone").value);
	var name = trim(document.getElementById("applyName").value);
	var areaCode = document.getElementById("applyAreaCode").value;
	document.getElementById("applyErrorTips").innerHTML = "";
	if(name == ""){
		document.getElementById("applyErrorTips").innerHTML = "请输入您姓名";
		return;
	}
	if(phone == ""){
		document.getElementById("applyErrorTips").innerHTML = "请输入您的手机号";
		return;
	}
	var param={phone:phone,name:name,areaCode:areaCode};
	jQuery.post(url,param,function(data){
		if(data > 0){
			document.getElementById("applyForActiveBlock").innerHTML = "";
			var callback={okBack:function(){window.location.href= document.getElementById("coinMainUrl").value+"/hd/participate.do";},noBack:function(){return false;}};
			okcoinAlert("恭喜您报名成功",null,callback,"确定");
		}else if(data == -1){
			document.getElementById("applyErrorTips").innerHTML = "手机号输入不合法";
		}else if(data == -2){
			document.getElementById("applyErrorTips").innerHTML = "手机号已存在";
		}else if(data == -3){
			document.getElementById("applyErrorTips").innerHTML = "您的姓名输入不合法";
		}
	});
}

function showApplyPopBlock(){
	dialogBoxShadow();
	document.getElementById("applyForActiveBlock").style.display = "block";
}
function closeApplyPopBlock(){
	dialogBoxHidden();
	document.getElementById("applyForActiveBlock").style.display = "none";
}
function showtableListBlock(type){
	if(document.getElementById("tableListBody"+type).style.display=="none") {
        document.getElementById("tableListBody"+type).style.display="";
        document.getElementById("tableListTitle"+type).className ="cur";
    }else {
        document.getElementById("tableListBody"+type).style.display="none";
        document.getElementById("tableListTitle"+type).className ="normal";
    }
}