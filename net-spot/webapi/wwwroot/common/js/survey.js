var num=new Array();
function loadStyle(idpar,idson){
	var num1=['num1span1','num1span2','num1span3'];
	var num2=['num2span1','num2span2','num2span3','num2span4','num2span5'];
	var num3=['num3span1','num3span2','num3span3'];
	var num4=['num4span1','num4span2','num4span3'];
	var num5=['num5span1','num5span2','num5span3'];
	var num6=['num6span1','num6span2','num6span3'];
	var num7=['num7span1','num7span2','num7span3'];
	
	//清空所有子节点的样式
	if(idpar=="num1"){
		for ( var it = 0; it < num1.length; it++) {			
			$("#"+num1[it]).removeClass('current');	
		}	
	}		
	//清空所有子节点的样式
	if(idpar=="num2"){
		for ( var it = 0; it < num2.length; it++) {			
			$("#"+num2[it]).removeClass('current');	
		}	
	}
	//清空所有子节点的样式
	if(idpar=="num3"){
		for ( var it = 0; it < num3.length; it++) {			
			$("#"+num3[it]).removeClass('current');	
		}	
	}	
	//清空所有子节点的样式
	if(idpar=="num4"){
		for ( var it = 0; it < num4.length; it++) {			
			$("#"+num4[it]).removeClass('current');	
		}	
	}	
	//清空所有子节点的样式
	if(idpar=="num5"){
		for ( var it = 0; it < num5.length; it++) {			
			$("#"+num5[it]).removeClass('current');	
		}	
	}	
	//清空所有子节点的样式
	if(idpar=="num6"){
		for ( var it = 0; it < num6.length; it++) {			
			$("#"+num6[it]).removeClass('current');	
		}	
	}	
	//清空所有子节点的样式
	if(idpar=="num7"){
		for ( var it = 0; it < num7.length; it++) {			
			$("#"+num7[it]).removeClass('current');	
		}	
	}	
	//赋值点击id文字下的样式
	$("#"+idson).addClass('current');
	for(var ii=0;ii<num.length;ii++){
		var sub=num[ii].substring(0,4);
		if(sub==idson.substring(0,4)){
			num.splice(ii, 1);
		}
	}
	num.push(idson);
	
}
function styleText(){
	if(num.length!=7){
		alert("请做完所有的单选题目！");
		return false;
	}
	$("#numarray").attr("value",num);	
	return true;
}

