//获取当前跟路径
function getRootPath(){
	return basePath;
}
var price=false;
var amount=false;
function changPrice(){
	var amount=$("#amount").val();
	var balance=$("#balance").val();
	// 判断单价应该是多少，并计算金额
	if(amount.length==0){
		$("#price").html("");
		document.getElementById("price").style.color = "red";
		$("#total").html("");
		document.getElementById("total").style.color = "green";
		alert("购买积分数量为空");
		return false; 
	}
	if(isNaN(amount)){
		alert("输入购买数量不正确");
		return false; 
	}
	if(parseInt(amount)!=amount){
		alert("输入购买数量要为整数");
		return false; 
	}
	
	if(amount<=0){
		$("#price").html("0");
		document.getElementById("price").style.color = "red";
		$("#total").html(0*amount);
		document.getElementById("total").style.color = "green";
		if(balance<0*amount){
			alert("可用余额不足");
			var price=false;
			var amount=false;
			return false; 
		}else{
			alert("输入购买数量不正确");
			var price=false;
			var amount=false;
			return false; 
		}
			
	}else if(amount<=9999){
		$("#price").html("0.000015");
		document.getElementById("price").style.color = "red";
		$("#total").html(Fractional(0.000015*amount));
		document.getElementById("total").style.color = "green";
		if(balance<0.000015*amount){
			alert("可用余额不足");
			var price=false;
			var amount=false;
			return false; 
		}
	}else if(amount<=99999){
		$("#price").html("0.000009");
		document.getElementById("price").style.color = "red";
		$("#total").html(Fractional(0.000009*amount));
		document.getElementById("total").style.color = "green";
		if(balance<0.000009*amount){
			alert("可用余额不足");
			var price=false;
			var amount=false;
			return false; 
		}
	}else if(amount<=499999){
		$("#price").html("0.000006");
		document.getElementById("price").style.color = "red";
		$("#total").html(Fractional(0.000006*amount));
		document.getElementById("total").style.color = "green";
		if(balance<0.000006*amount){
			alert("可用余额不足");
			var price=false;
			var amount=false;
			return false; 
		}
	}else if(amount<=999999){
		$("#price").html("0.000003");
		document.getElementById("price").style.color = "red";
		$("#total").html(Fractional(0.000003*amount));
		document.getElementById("total").style.color = "green";
		if(balance<0.000003*amount){
			alert("可用余额不足");
			var price=false;
			var amount=false;
			return false; 
		}
	}else if(amount<=4999999){
		$("#price").html("0.0000021");
		document.getElementById("price").style.color = "red";
		$("#total").html(Fractional(0.0000021*amount));
		document.getElementById("total").style.color = "green";
		if(balance<0.0000021*amount){
			alert("可用余额不足");
			var price=false;
			var amount=false;
			return false; 
		}
	}else if(amount<=9999999){
		$("#price").html("0.0000015");
		document.getElementById("price").style.color = "red";
		$("#total").html(Fractional(0.0000015*amount));
		document.getElementById("total").style.color = "green";
		if(balance<0.0000015*amount){
			alert("可用余额不足");
			var price=false;
			var amount=false;
			return false; 
		}
	}else if(amount<=49999999){
		$("#price").html("0.0000009");
		document.getElementById("price").style.color = "red";
		$("#total").html(Fractional(0.0000009*amount));
		document.getElementById("total").style.color = "green";
		if(balance<0.0000009*amount){
			alert("可用余额不足");
			var price=false;
			var amount=false;
			return false; 
		}
	}else if(amount>49999999){
		$("#price").html("0.0000001");
		document.getElementById("price").style.color = "red";
		$("#total").html(Fractional(0.0000001*amount));
		document.getElementById("total").style.color = "green";
		if(balance<0.0000001*amount){
			alert("可用余额不足");
			var price=false;
			var amount=false;
			return false; 
		}

	}
	var price=true;
	var amount=true;
	return true; 
}
	function checkBalance(){
		if(price&&amount){
			return true;
		}else{
			return false;
		}
	}


	// 小数位数控制，可以四舍五入
	function Fractional(n) {
		 // 小数保留位数
		var bit = 8;
		 // 加上小数点后要扩充1位
		bit++;
		// 数字转为字符串
		n = n.toString();
		// 获取小数点位置
		var point = n.indexOf('.');
		 // n的长度大于保留位数长度
		if (n.length > point + bit) {
			// 保留小数后一位是否大于4，大于4进位
			if (parseInt(n.substring(point + bit, point + bit + 1)) > 4) {
				return n.substring(0, point) + "." + (parseInt(n.substring(point + 1, point + bit)) + 1);
			}
			else {
				return n.substring(0, point) + n.substring(point, point + bit);
			}
		}
		return n;
	}