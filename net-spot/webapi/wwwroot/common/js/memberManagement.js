// 获取当前跟路径
function getRootPath(){
	return basePath;
}

function closeDelManagement() {
	$("#delManagement").hide();
	$("#gray_overlay").hide();
	$("html").css('overflow-y','auto')
}

//显示隐藏返佣详情列表
function checkDetilbtn(index){
	if ($("#checkDetilList" + index).attr('src') == '../common/images/check_open.png') {
		$("#checkDetilList" + index).attr('src', '../common/images/check_close.png');
		$("#rebateListBox" + index).show();
		$("#checkDetilText" + index).html(MSG['checkCloseList'])
		getRebateList(index);
		check(index);
	} else if($("#checkDetilList" + index).attr('src') =='../common/images/check_close.png'){
		$("#checkDetilList" + index).attr('src', '../common/images/check_open.png');
		$("#rebateListBox" + index).hide();
		$("#checkDetilText" + index).html(MSG['checkOpenList'])
	}
}
//关闭其他查看窗口
function check(index) {
	for (var i = 0; i < 15; i++) {
		if (i == index) {
			continue;
		}
		if ($("#checkDetilList" + i).attr('src') == '../common/images/check_close.png') {
			$("#checkDetilText" + i).html(MSG['checkOpenList'])
			$("#rebateListBox" + i).hide();
			$("#checkDetilList" + i).attr('src', '../common/images/check_open.png');
		}
	}
}

//查询返佣详情列表
function getRebateList(index) {
	var userName = $("#userEmail" + index).html();
	$.ajax({
        type: "POST",
        dataType: "json",         
        url: basePath +"center/rebateMore.do?userName=" + userName,
        data: {},
        success: function(data) {
        	var rebateListHtml = "";
        	if(data.code == 'success') {
        		var list = data.data;
        		var height = 134;
        		height += index * 55;
        		var tabHeight = 169;
        		tabHeight += index * 55;
        		rebateListHtml = `<div class='rebatePointto' style="top: ${height}px;"></div>
        						<table class="rebateBox" id="rebateBox" cellpadding="0" cellspacing="0" style="top: ${tabHeight}px;z-index: 10000;">
										<thead>
											<tr class="rebateBoxTr" style="color: #ffffff;background-color: #4a5f78;font-family: PingFangSC-Regular;font-size: 14px;">
												<th style="color: #ffffff;height: 34px;">
													${fmtCommissionType}
												</th>
												<th style="color: #ffffff;height: 34px;">
													${fmtWeekCommission}
												</th>
												<th style="color: #ffffff;height: 34px;">
													${fmtMonthsCommission}
												</th>
												<th style="color: #ffffff;height: 34px;">
													${fmtTotalCommission}
												</th>
											</tr>
										</thead>
										<tbody>`;
        		for (var i = 0; i < list.length; i++) {
					var obj = list[i];
					rebateListHtml += `<tr>
											<td style="height: 34px;">
												${obj.currenyName}
											</td>
											<td style="height: 34px;">
												${obj.weekRebate}
											</td>
											<td style="height: 34px;">
												${obj.monthRebate}
											</td>
											<td style="height: 34px;">
												${obj.totalRebate}
											</td>
										</tr>`;
				}
        		rebateListHtml += `</tbody>
									</table>`;
        		$("#rebateListBox" + index).html(rebateListHtml);
	    	}
        }
	});
}

//点用户邮箱跳转链接
function clickGotoUserDel(index) {
	var inviter = $("#inviter").html();
	var level = $("#level").html();
	var userName = $("#userEmail" + index).html();
	var subLevel = $("#vipGrade" + index).html();
	level = level.substring(3);
	subLevel = subLevel.substring(3);
	if (level > subLevel) {
		$("#delManagement").hide();
		$("#gray_overlay").hide();
		var url = basePath + "center/inviteManagement.do?userName=" + userName + "&inviter=" + inviter;
		location.href = url;
	} else {
		$("#delManagement").show();
		$("#gray_overlay").show();
		/*$("html").css('overflow-y','hidden')*/
	}
}

