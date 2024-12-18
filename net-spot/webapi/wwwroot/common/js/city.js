function loadProvice() {
	var cCode = $("#countrySelect option:selected").val();
	if(cCode != "CN") {
		$("#proviceSelect").hide();
		$("#citySelect").hide();
		$("#areaSelect").hide();
	}else {
		$("#proviceSelect").show();
		$("#citySelect").show();
		$("#areaSelect").show();
	}
	if (cCode == null || cCode == "") {
//		 alert("找不到省");
	} else {
		$.ajax({
			type:"GET",
			url:basePath + "city/loadProvice.do?countryCode="+cCode,
			date:null,
			dataType:"json",
			contentType: "application/x-www-form-urlencoded; charset=utf-8",
			error:function(data){// console.info(data);
			
		},
		
			success:function(data){
			    if(data==null){			    	
			    	var proviceSelect = $("#proviceSelect");
					proviceSelect.html("");
					var citySelect = $("#citySelect");
					citySelect.html("");
					
			    }else{
					var proviceSelect = $("#proviceSelect");
					var myProvince = $("#myProvince").val();
					proviceSelect.html("");
					for ( var i = 0; i < data.length; i++) {
						var code0 = data[i].code;
						if (myProvince != null && myProvince != "" && code0 == myProvince) {
							proviceSelect.append("<option selected='selected' value='"
									+ data[i].code + "'>" + data[i].name
									+ "</option>");
						} else {
							proviceSelect.append("<option value='" + data[i].code + "'>"
									+ data[i].name + "</option>");
						}
					}
					loadCity();
			    }
			}});
		   
	}
};
/**
 * 加载市
 * 
 */
function loadCity() {
	var proviceCode = $("#proviceSelect option:selected").val();
	if (proviceCode == null || proviceCode == "" || proviceCode < 1) {
		alert("找不到市");
	} else {
		$.post(basePath + "city/loadCity.do", {
			proviceCode : proviceCode
		}, function(data, result) {
			if (data == "noId") {
				alert("请求错误");
			} else if (data == "null") {
//				alert("城市为空");
//				alert(proviceCode);
				$.post(basePath + "city/loadCityProvice.do", {
					proviceCode : proviceCode},function(data){
						data = eval("{" + data + "}");
						var citySelect = $("#citySelect");
						var myCity = $("#myCity").val();
						citySelect.html("");
						for ( var i = 0; i < data.length; i++) {
							var code0 = data[i].code;
							if (myCity != null && myCity != "" && myCity == code0 ) {
								citySelect.append("<option selected='selected' value='"
												+ data[i].code + "'>" + data[i].name
												+ "</option>");
							} else {
								citySelect.append("<option value='" + data[i].code
										+ "'>" + data[i].name + "</option>");
							}
						}
					}
				);
			} else {
				data = eval("{" + data + "}");
				var citySelect = $("#citySelect");
				var myCity = $("#myCity").val();
				citySelect.html("");
				for ( var i = 0; i < data.length; i++) {
					var code0 = data[i].code;
					if (myCity != null && myCity != "" && myCity == code0 ) {
						citySelect.append("<option selected='selected' value='"
										+ data[i].code + "'>" + data[i].name
										+ "</option>");
					} else {
						citySelect.append("<option value='" + data[i].code
								+ "'>" + data[i].name + "</option>");
					}
				}
				loadArea();
			}
		});
	}
};

/*
* 加载区
* */
function loadArea() {
	var cityCode = $("#citySelect option:selected").val();
	if (cityCode == null || cityCode == "" || cityCode < 1) {
		alert("找不到区");
	} else {
		$.post(basePath + "city/loadArea.do", {
			cityCode : cityCode
		}, function(data) {
			if (data == "noId") {
				alert("请求错误");
			} else {
				data = eval("{" + data + "}");
				// console.log(data);
				var areaSelect = $("#areaSelect");
				var myArea = $("#myArea").val();
				areaSelect.html("");
				for (var i = 0; i < data.length; i++) {
					var code0 = data[i].code;
					if (myArea != null && myArea != "" && myArea == code0 ) {
						areaSelect.append("<option selected='selected' value='"
							+ data[i].code + "'>" + data[i].name
							+ "</option>");
					} else {
						areaSelect.append("<option value='" + data[i].code
							+ "'>" + data[i].name + "</option>");
					}
				}

			}
		})
	}
}

