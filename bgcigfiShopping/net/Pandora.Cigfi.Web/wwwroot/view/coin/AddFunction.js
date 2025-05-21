//流通量与当前供应量保持一致复选框注册事件
function changeIsCirculationEqualToCollection() {
    if ($("#IsCirculationEqualToCollection").is(':checked')) {
        if ($("#CirculatingSupply").val() !== $("#TotalSupply").val()) {
            //如果流通总量和当前总量不一致，则流通总量等于当前总量
            $("#CirculatingSupply").val($("#TotalSupply").val());
        }
    }
}

function setAverage_PriceStatus() {
  var status = $("#average_price_status").val();
  if (status == "1") {
    $("#Average_Price,#Average_Price_Cny").parent().parent().hide();
  } else if (status == "2") {
    $("#Average_Price,#Average_Price_Cny").parent().parent().hide();
  } else {
    $("#Average_Price,#Average_Price_Cny").parent().parent().show();
  }
}

function addcoin(value) {
  var tpl = $($("#brandtpl").html());

  tpl.find("[name=del]").click(function () {
    tpl.remove();
  });

  if (value.skipType == 0) {
    tpl.find("[name=brandtype]").val(0);
    tpl.find("[id=video]").hide();
    if (value.skipUrl) {
      tpl.find("[name=SkipUrl0]").val(value.skipUrl);
      tpl.find("[name=SkipWriting0]").val(value.skipWriting);
    }
  } else {
    tpl.find("[name=brandtype]").val(1);
    tpl.find("[id=img]").hide();
    if (value.skipUrl) {
      tpl.find("[name=SkipUrl1]").val(value.skipUrl);
      tpl.find("[name=SkipWriting1]").val(value.skipWriting);
    }
  }
  $(".page-list").append(tpl);
  return false;
}

function getPosterData() {
  var rows = $(".page-list .data-row");
  var data = new Array();
  rows.each(function (index, elem) {
    var row = $(elem);
    var types = row.find("[name=brandtype]").val();
    if (types == 0) {
      var dataitem = {
        ID: 0,
        CoinCode: $("#CoinCode").val(),
        SkipType: 0,
        SkipUrl: row.find("[name=SkipUrl0]").val(),
        SkipWriting: row.find("[name=SkipWriting0]").val(),
      };
    } else {
      var dataitem = {
        ID: 0,
        CoinCode: $("#CoinCode").val(),
        SkipType: 1,
        SkipUrl: row.find("[name=SkipUrl1]").val(),
        SkipWriting: row.find("[name=SkipWriting1]").val(),
      };
    }
    //console.log(dataitem);
    data.push(dataitem);
  });
  return data;
}


//阿拉伯数字转换为大写汉字
function Arabia_To_SimplifiedChinese(n) {
  if (!/^(0|[1-9]\d*)(\.\d+)?$/.test(n)) return "数据不正确";
  var unit = "京亿万仟佰拾兆万仟佰拾亿仟佰拾万仟佰拾元角分",
    str = "";
  n += "00";
  var p = n.indexOf(".");
  if (p >= 0) n = n.substring(0, p) + n.substr(p + 1, 2);
  unit = unit.substr(unit.length - n.length);
  for (var i = 0; i < n.length; i++)
    str += "零壹贰叁肆伍陆柒捌玖".charAt(n.charAt(i)) + unit.charAt(i);
  return str
    .replace(/零(仟|佰|拾|角)/g, "零")
    .replace(/(零)+/g, "零")
    .replace(/零(兆|万|亿|元)/g, "$1")
    .replace(/(兆|亿)万/g, "$1")
    .replace(/(京|兆)亿/g, "$1")
    .replace(/(京)兆/g, "$1")
    .replace(/(京|兆|亿|仟|佰|拾)(万?)(.)仟/g, "$1$2零$3仟")
    .replace(/^元零?|零分/g, "")
    .replace(/(元|角)$/g, "")
    .replace("分", "");
}
function MaxSupplyFunction() {
  var num = $("#MaxSupply").val();
  var MaxSupplyCHZNumb = Arabia_To_SimplifiedChinese(num);
  $("#MaxSupplyCHZNumb").html(MaxSupplyCHZNumb);
}
function TotalSupplyFunction() {
  var TotalSupplyCH = $("#TotalSupply").val();
  var TotalSupplyCHZNumb = Arabia_To_SimplifiedChinese(TotalSupplyCH);
  $("#TotalSupplyCHZNumb").html(TotalSupplyCHZNumb);
}
function RateValidateFunction(index) {
  var RateName = eval('$("#RateName' + index + '")').val();
  var RateValue = eval('$("#RateValue' + index + '")').val();
  if (RateName == "" && RateValue == "") {
    eval('$("#RateNameValidate' + index + '")').html("");
    eval('$("#RateValueValidate' + index + '")').html("");
  } else {
    if (RateName == "" && RateValue != "") {
      eval('$("#RateNameValidate' + index + '")').html("不能为空");
    } else if (RateName != "" && RateValue == "") {
      //eval('$("#RateValueValidate' + index + '")').html("不能为空");
    } else {
      eval('$("#RateNameValidate' + index + '")').html("");
    }
    if (RateValue != "") {
      //校验数据准确性
      //if (!/^(0|[1-9]\d*)(\.\d{1,2})?$/.test(RateValue)) {
      //    eval('$("#RateValueValidate' + index + '")').html("格式不正确(小数点最多2位的数字)");
      //} else {
      //    eval('$("#RateValueValidate' + index + '")').html("");
      //}
    }
  }
}

function DeleteMain(data) {
  $("#" + data.id).remove();
}
function DeleteSubMain(id) {
  $("#" + id).remove();
}
function addtitle() {
  var dh = "";
  var len = $("#mainpoint_panel").children("div").length;
  if (len > 0) {
    var dom = $("#mainpoint_panel").children("div")[len - 1].id;
    var newobj = {
      newid: dom + "1",
      id: len - 1,
    };
    dh += "<div id=" + dom + "1" + ">";
    dh += '<div class="form-group" style="margin-top:55px;">';
    dh +=
      '  <label class="col-sm-2 control-label" style="padding-right:0px">一级标题：</label>';
    dh += '     <div class="col-sm-2">';
    dh +=
      '     <input style="padding: 0px 5px;" name="itemname" maxlength="10" data-vc="2" type="text" class="form-control " required>';
    dh += "     </div>";
    dh +=
      " <a class='col-sm-2 btn btn-danger searchOrg' style='width:60px;' id='searchOrg' data-vc='2' onclick='DeleteMain(" +
      dom +
      "1" +
      ")' >删除</a>";
    dh += "</div>";
    dh += '<div class="form-group" style="margin-top:15px;">';
    dh +=
      '  <label class="col-sm-2 control-label" style="padding-right:0px">二级标题：</label>';
    dh += '     <div class="col-sm-2">';
    dh +=
      '     <input style="padding: 0px 5px;" id=' +
      dom +
      "2" +
      " name=" +
      "subitemname" +
      newobj.newid +
      ' maxlength="10" data-vc="2" type="text" class="form-control searchValue" required>';
    dh += "     </div>";
    dh +=
      ' <label class="col-sm-2 control-label" style="padding-right:0px;width: 130px;">正文：</label>';
    dh += '     <div class="col-sm-4">';
    //dh += '     <input style="padding: 0px 5px;" id=' + dom + "2" + ' name=' + 'subitemdesc' + ' data-vc="2" type="text" class="form-control searchValue" required>';
    dh +=
      '<textarea style="padding: 0px 5px;" id=' +
      dom +
      "2" +
      " name=" +
      "subitemdesc" +
      ' data-vc="2" type="text" class="form-control searchValue" required></textarea>';
    dh += "     </div>";
    dh +=
      " <a class='col-sm-2 btn btn-primary searchOrg' style='width:60px;' id='searchOrg' onclick='addsubtitle(" +
      JSON.stringify(newobj).replace(/'/g, "&quot;") +
      "," +
      newobj.newid +
      ")' data-vc='2' >添加</a>";
    dh += " </div>";
    dh += "</div>";
    $("#mainpoint_panel").append(dh);
  } else {
    dom = "pannl";
    var newobj = {
      newid: dom + "1",
      id: 1,
    };
    dh += "<div id=" + dom + "1" + ">";
    dh += '<div class="form-group" style="margin-top:55px;">';
    dh +=
      '  <label class="col-sm-2 control-label" style="padding-right:0px">一级标题：</label>';
    dh += '     <div class="col-sm-2">';
    dh +=
      '     <input style="padding: 0px 5px;" name="itemname" maxlength="10" data-vc="2" type="text" class="form-control required">';
    dh += "     </div>";
    dh +=
      " <a class='col-sm-2 btn btn-danger searchOrg' style='width:60px;' id='searchOrg' data-vc='2' >删除</a>";
    dh += "</div>";
    dh += '<div class="form-group" style="margin-top:15px;">';
    dh +=
      '  <label class="col-sm-2 control-label" style="padding-right:0px">二级标题：</label>';
    dh += '     <div class="col-sm-2">';
    dh +=
      '     <input style="padding: 0px 5px;" id=' +
      dom +
      "2" +
      ' maxlength="10" name=' +
      "subitemname" +
      newobj.newid +
      ' data-vc="2" type="text" class="form-control searchValue" required >';
    dh += "     </div>";
    dh +=
      ' <label class="col-sm-2 control-label" style="padding-right:0px;width: 130px;">正文：</label>';
    dh += '     <div class="col-sm-4">';
    //dh += '     <input style="padding: 0px 5px;" id=' + dom + "2" + ' name=' + 'subitemdesc' + ' data-vc="2" type="text" class="form-control searchValue" required>';
    dh +=
      '<textarea style="padding: 0px 5px;" id=' +
      dom +
      "2" +
      " name=" +
      "subitemdesc" +
      ' data-vc="2" type="text" class="form-control searchValue" required></textarea>';
    dh += "     </div>";
    dh +=
      " <a class='col-sm-2 btn btn-primary searchOrg' style='width:60px;' id='searchOrg' onclick='addsubtitle(" +
      JSON.stringify(newobj).replace(/'/g, "&quot;") +
      "," +
      newobj.newid +
      ")' data-vc='2' >添加</a>";
    dh += " </div>";
    dh += "</div>";
    $("#mainpoint_panel").append(dh);
  }
}
function MainUpTitle(data, newdata) {
  if ($(data).index() > 0) {
    $("#mainpoint_panel")
      .children()
      .eq($(data).index() - 1)
      .before(data);
  }
}
function MainDownTitle(data, newdata) {
  if ($("#mainpoint_panel").children().length - $(data).index() == 2) {
    var div = $("#mainpoint_panel>div:last")[0];
    $("#mainpoint_panel")
      .children()
      .eq($(div).index() - 1)
      .before(div);
  } else {
    if ($(data).index() > -1) {
      $("#mainpoint_panel")
        .children()
        .eq($(data).index() + 2)
        .before(data);
    }
  }
}

function UpTitle(data, newdata) {
  if ($(data).index() > 1) {
    $("#" + newdata.id)
      .children()
      .eq($(data).index() - 1)
      .before(data);
  }
}
function DownTitle(data, newdata) {
  if ($("#" + newdata.id).children().length - $(data).index() == 2) {
    var div = $("#" + newdata.id + ">div:last")[0];
    $("#" + newdata.id)
      .children()
      .eq($(div).index() - 1)
      .before(div);
  } else {
    if ($(data).index() > 0) {
      $("#" + newdata.id)
        .children()
        .eq($(data).index() + 2)
        .before(data);
    }
  }
}
function addsubtitle(data, newdata) {
  var ids = data.id + "k" + parseInt(Math.random() * 100);
  var dh = "";
  dh += '<div class="form-group" id=' + ids + ' style="margin-top:15px;">';
  dh +=
    '  <label class="col-sm-2 control-label" style="padding-right:0px">二级标题：</label>';
  dh += '     <div class="col-sm-2">';
  dh +=
    '     <input style="padding: 0px 5px;" name="subitemname" maxlength="10" data-vc="2" type="text" class="form-control searchValue" required>';
  dh += "     </div>";
  dh +=
    ' <label class="col-sm-2 control-label" style="padding-right:0px;width: 130px;">正文：</label>';
  dh += '     <div class="col-sm-4">';
  //dh += '     <input style="padding:0px 5px;" name="subitemdesc" data-vc="2" type="text" class="form-control searchValue" required>';
  dh +=
    '<textarea style="padding: 0px 5px;" name=' +
    "subitemdesc" +
    ' data-vc="2" type="text" class="form-control searchValue" required></textarea>';
  dh += "     </div>";
  dh +=
    " <a class='col-sm-2 btn btn-danger searchOrg' style='width:60px;' id='searchOrg' data-vc='2' onclick=DeleteSubMain(" +
    JSON.stringify(ids) +
    ")>删除</a>";
  dh += "</div>";
  $("#" + data.newid + "").append(dh);
}
//科学计数法转还原数字
function getFullNum(num) {
  //处理非数字
  if (isNaN(num)) {
    return num;
  }

  //处理不需要转换的数字
  var str = "" + num;
  if (!/e/i.test(str)) {
    return num;
  }

  return num.toFixed(18).replace(/\.?0+$/, "");
}

function imgChange(obj) {
  //console.log(obj);
  var image = obj.files[0]; //获取文件域中选中的图片
  var tools = {
    limitFileSize: function (file, limitSize) {
      var arr = ["KB", "MB", "GB"];
      var limit = limitSize.toUpperCase();
      var limitNum = 0;
      for (var i = 0; i < arr.length; i++) {
        var leval = limit.indexOf(arr[i]);
        if (leval > -1) {
          limitNum = parseInt(limit.substr(0, leval)) * Math.pow(1024, i + 1);
          break;
        }
      }
      if (file.size > limitNum) {
        return false;
      }
      return true;
    },
  };
  if (!tools.limitFileSize(image, "10MB")) {
    layer.msg("图片大小不能超过10MB");
    return;
  }
  var reader = new FileReader(); //实例化文件读取对象
  reader.readAsDataURL(image); //将文件读取为 DataURL,也就是base64编码
  reader.onload = function (ev) {
    //文件读取成功完成时触发
    var dataURL = ev.target.result; //获得文件读取成功后的DataURL,也就是base64编码
    $("#imglist img").prop("src", dataURL); //将DataURL码赋值给img标签
    $("#tab-10 input[name='orgimg']").val(dataURL);
    // console.log(dataURL);
  };
}

function calcratenum() {
  var totalnum = $("#totalNum").val();

  totalnum = parseFloat(totalnum);

  $("input[name=RateValue]").each(function () {
    var mark = $(this).attr("mark");
    var value = $(this).val();

    var num = (parseFloat(value) * totalnum) / 100;
    num = parseFloat(num);
    eval('$("#RateNum' + mark + '")').html(num);
    if (num > 0) {
      var q = "#RateNum" + mark;
      $(q).val(num);
    }
  });
}

function calcratenum_percent() {
  var totalnum = $("#totalNum").val();

  totalnum = parseFloat(totalnum);

  $("input[name=RateNum]").each(function () {
    var mark = $(this).attr("mark");
    var value = $(this).val();
    var num = (parseFloat(value / totalnum) * 100).toFixed(4);
    num = parseFloat(num);
    //console.log("num:", num);
    if (num > 0) {
      var q = "#RateValue" + mark;
      $(q).val(num);
    }
  });
}

function UpdateIsConsultant(coinCode, itemCode, rtype, itpye, IsConsultant) {
  $.ajax({
    url: "/admincp/coin/UpdateIsConsultant",
    type: "post",
    data: {
      coinCode: coinCode,
      itemCode: itemCode,
      rtype: rtype,
      itpye: itpye,
      IsConsultant: IsConsultant,
    },
    success: function (result) {
      if (result.status != "error") {
        layer.msg(result.message);
      } else {
        layer.alert(result.message, { icon: 2 });
      }
      layui.table.reload("table-10", {
        page: {
          curr: 1, //重新从第 1 页开始
        },
        where: {
          //设定异步数据接口的额外参数，任意设
          coinCode: coinCode,
          rtype: rtype,
          itype: itpye,
        },
      });
    },
  });
}

function UpdateIsFounder(coinCode, itemCode, rtype, itpye, IsFounder) {
  $.ajax({
    url: "/admincp/coin/UpdateIsFounder",
    type: "post",
    data: {
      coinCode: coinCode,
      itemCode: itemCode,
      rtype: rtype,
      itpye: itpye,
      IsFounder: IsFounder,
    },
    success: function (result) {
      if (result.status != "error") {
        layer.msg(result.message);
      } else {
        layer.alert(result.message, { icon: 2 });
      }
      layui.table.reload("table-10", {
        page: {
          curr: 1, //重新从第 1 页开始
        },
        where: {
          //设定异步数据接口的额外参数，任意设
          coinCode: coinCode,
          rtype: rtype,
          itype: itpye,
        },
      });
    },
  });
}

function search(vc, value) {
  $(".remark").remove();
  $.ajax({
    url: "/admincp/coin/GetOrgByFuzzyAsync",
    type: "post",
    data: {
      key: value,
      itype: vc,
    },
    success: function (data) {
      $("#searchResult-" + vc)
        .find("tr")
        .remove();

      if (data != null && data != "" && data != undefined) {
        for (var i = 0; i < data.length; i++) {
          $("#searchResult-" + vc).append(
            '<tr style="line-height:35px"><td class= "col-sm-1"></td><td class= "col-sm-9"> <a  target="_blank">' +
              data[i].itemCode +
              "（" +
              data[i].itemName +
              "）" +
              '</a></td> <td style="float:right"><a class="AddCoin" data-code="' +
              data[i].itemCode +
              '" data-type="' +
              data[i].itemType +
              '" class="btn btn-default" title="添加">>></a> </td></tr >'
          );
        }
      } else {
        $("#searchResult-" + vc).append(
          '<tr style="line-height:35px; text-align:center"><td class= "col-sm-1"></td><td class= "col-sm-9">无数据</td></tr >'
        );
      }
    },
  });
}

function getAllTagHtml(CheckOk) {
  $.ajax({
    url: "/admincp/coin/GetAllTag",
    success: function (r) {
      var divString = "";
      if (r != null) {
        r.forEach((val) => {
            let string = isCheck(val, CheckOk);
          divString = divString + string;
        });
      }
      $("#TagCheck").append(divString);
    },
  });
}

function isCheck(val, CheckOk) {

    var endLabelTag = '</label >'
  var starLabelTag = ' <label class="col-sm-3"> <input  type="checkbox" value="'
  var result =
    starLabelTag +
    val.tagCode +
    '"  class="tagAllCoin"/> ' +
    val.cn +
    "(" +
    val.tagName +
    ")" +
    endLabelTag;
  CheckOk.forEach((item) => {
    if (val.tagCode == item) {
      result =
        starLabelTag +
        val.tagCode +
        '" checked="checked"  class="tagAllCoin"  /> ' +
        val.cn +
        "(" +
        val.tagName +
        ")" +
        endLabelTag;
    }
  });
  return result;
}

function save(TeamDesc, Description) {
  var coinCode = $("#CoinCode").val();
  var id = $("#savelang").attr("data-id");
  var NativeName = $("#NativeName").val();
  //var TeamDesc = CKTeamDesc.getData();
  //var Description = CKDescription.getData();

  var keywords = $("input[name=Keywords]");
  var link = $("input[name=Link]");
  var jumpid = $("input[name=JumpId]");

  var SceneTags = [];

  for (var i = 0; i < keywords.length; i++) {
    var keywordsValue = keywords[i].value;
    var linkValue = link[i].value;
    var jumpidValue = jumpid[i].value;
    var tag = { Keywords: keywordsValue, Link: linkValue, JumpId: jumpidValue };
    if (keywordsValue == "" && linkValue == "" && jumpidValue == "") {
      //全部为空则不需要保存到数据库中
    } else {
      SceneTags.push(tag);
    }
  }
  var postdata = {
    LanguageCode: id,
    CoinCode: coinCode,
    NativeName: NativeName,
    TeamDesc: TeamDesc,
    Description: Description,
    SceneTag: $("#SceneTag").val(),
    SceneTagList: SceneTags,
  };

  $.ajax({
    url: "/admincp/coin/SaveLang",
    type: "post",
    contentType: "application/json",
    data: JSON.stringify(postdata),
    //data: {
    //    "id": id,
    //    "CoinCode": coinCode,
    //    "NativeName": NativeName,
    //    "TeamDesc": TeamDesc,
    //    "Description": Description,
    //    "SceneTag": $("#SceneTag").val()
    //},
    success: function (r) {
      if (r.status == "success") {
        layer.msg("修改成功");
      } else {
        layer.alert("修改失败", { icon: 2 });
      }
    },
  });
}

function getTagHtml(key, link, id) {
  var html =
    '  <div class="form-group">' +
    '<label class="col-sm-1 control-label">关键词：</label>' +
    ' <div class="col-sm-2">' +
    '  <input  name="Keywords" class="bjyqinput" type="text" value="' +
    key +
    '" placeholder="">' +
    "  </div>" +
    '  <label class="col-sm-1 control-label">跳转链接：</label>' +
    ' <div class="col-sm-3">' +
    ' <input  name="Link" class="bjyqinput" type="text" value="' +
    link +
    '" placeholder="">' +
    "  </div>" +
    '  <label class="col-sm-1 control-label">文章ID：</label>' +
    ' <div class="col-sm-2">' +
    ' <input  name="JumpId" class="bjyqinput" type="text" value="' +
    id +
    '" placeholder="">' +
    "   </div>" +
    " </div>";
  return html;
}

function getWallet() {
  $.ajax({
    url: "/admincp/coin/GetWalletByCoinAsync",
    type: "post",
    data: {
      coinCode: $("#WalletTable").attr("data-coin"),
    },
    success: function (data) {
      if (data.length != 0) {
        $("#WalletTfoot").remove();
      }
      if (data.length == 1) {
        $("#WalletTr").append(
          '<tr data-id="' +
            data[0].walletID +
            '" data-orderno="' +
            data[0].orderNo +
            '"><td><b>' +
            data[0].walletName +
            "</b></td><td><b>" +
            data[0].orderNo +
            '</b></td><td><span class="falebutton"><b>升</b></span><span class="falebutton"><b>降</b></span></td></tr>'
        );
      } else {
        for (var i = 0; i < data.length; i++) {
          switch (i) {
            case 0:
              $("#WalletTr").append(
                '<tr data-id="' +
                  data[i].walletID +
                  '" data-orderno="' +
                  data[i].orderNo +
                  '"><td><b>' +
                  data[i].walletName +
                  "</b></td><td><b>" +
                  data[i].orderNo +
                  '</b></td><td><span class="falebutton"><b>升</b></span><a class="downbutton"><b>降</b></a></td></tr>'
              );
              break;
            case data.length - 1:
              $("#WalletTr").append(
                '<tr data-id="' +
                  data[i].walletID +
                  '" data-orderno="' +
                  data[i].orderNo +
                  '"><td><b>' +
                  data[i].walletName +
                  "</b></td><td><b>" +
                  data[i].orderNo +
                  '</b></td><td><a class="upbutton"><b>升</b></a><span class="falebutton"><b>降</b></span></td></tr>'
              );
              break;
            default:
              $("#WalletTr").append(
                '<tr data-id="' +
                  data[i].walletID +
                  '" data-orderno="' +
                  data[i].orderNo +
                  '"><td><b>' +
                  data[i].walletName +
                  "</b></td><td><b>" +
                  data[i].orderNo +
                  '</b></td><td><a class="upbutton"><b>升</b></a><a class="downbutton"><b>降</b></a></td></tr>'
              );
              break;
          }
        }
      }
    },
  });
}

function seosave() {
  var id = $("#seosavelang").attr("data-id");
  var SeoTitle = $("#SeoTitle").val();
  var SeoKeyWords = $("#SeoKeyWords").val();
  var SeoDescription = $("#SeoDescription").val();
  var coinCode = $("#CoinCode").val();

  $.ajax({
    url: "/admincp/coin/SeoSaveLang",
    type: "post",
    data: {
      id: id,
      SeoCode: coinCode,
      SeoTitle: SeoTitle,
      SeoKeyWords: SeoKeyWords,
      SeoDescription: SeoDescription,
    },
    success: function (r) {
      if (r.status == 1) {
        layer.msg("修改成功");
      } else {
        layer.alert("修改失败", { icon: 2 });
      }
    },
  });
}

function seodel() {
  var coinCode = $("#CoinCode").val();
  var id = $("#seodeletelang").attr("data-id");
  $.ajax({
    url: "/admincp/coin/SeoDelLang",
    type: "post",
    data: {
      seoCode: coinCode,
      langCode: id,
    },
    success: function (result) {
      if (result.status != "error") {
        layer.msg(result.message);
        setTimeout(closethisdialog, 2000);
      } else {
        layer.alert(result.message, { icon: 2 });
      }
    },
  });
}

function AddCrowdfunding_Item(dataitem) {
  var nametpl = $($("#crowdfunding_name_panel_tpl").html());
  var itemstpl = $('<div class="form-group"></div>');
  if (dataitem == undefined) {
    dataitem = { name: "", items: [] };
  }
  nametpl.find("input[name='name']").val(dataitem.name);
  for (var i = 0; i < 10; i++) {
    var itemtpl = $($("#crowdfunding_items_panel_tpl").html());
    var item = dataitem.items.length > i ? dataitem.items[i] : ["", ""];

    if (item.length == 2) {
      itemtpl.find("input[name='itemname']").val(item[0]);
      itemtpl.find("[name='itemdesc']").val(item[1]);
    } else if (item.length == 3) {
      itemtpl.find("input[name='itemname']").val(item[0]);
      itemtpl.find("input[name='itemvalue']").val(item[2]);
      itemtpl.find("[name='itemdesc']").val(item[1]);
    }

    itemstpl.append(itemtpl);
  }
  //$("#crowdfunding_panel").append(nametpl);
  $("#crowdfunding_panel").append(itemstpl);
}
function AddCrowdfunding_SingleItem(dataitem) {
  var itemstpl = $("#crowdfunding_panel div[class='form-group']:first");
  var itemtpl = $($("#crowdfunding_items_panel_tpl").html());
  var item = ["", ""];
  itemtpl.find("input[name='itemname']").val(item[0]);
  itemtpl.find("[name='itemdesc']").val(item[1]);
  itemstpl.append(itemtpl);
}

function Addforking_Item(item) {
  if (item == undefined) {
    item = ["", ""];
  }
  var tpl = $("#forking_panel_tpl").html();
  var htmlitem = $(tpl);
  htmlitem.find("input[name='itemname']").val(item[0]);
  htmlitem.find("[name='itemdesc']").val(item[1]);
  $("#forking_panel").append(htmlitem);
}


