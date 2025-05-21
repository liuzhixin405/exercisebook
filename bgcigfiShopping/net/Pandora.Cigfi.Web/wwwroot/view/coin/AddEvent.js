$("#audit_sbm").click(function () {
  var data = {
    CoinCode: $("#CoinCode").val(),
    TokenPlatForm: $("#tab-11 select[name='TokenPlatForm_audit']").val(),
    ContractAddress: $("#tab-11 input[name='ContractAddress']").val(),
    BlockChainLink: $("#tab-11 textarea[name='BlockChainLink']").val(),
    audit_orgname: (function () {
      var r = "";
      $("input[name='audit_orgname']").each(function () {
        r += "`" + $(this).val();
      });
      return r.substring(1);
    })(),
    audit_link: (function () {
      var r = "";
      $("input[name='audit_link']").each(function () {
        r += "`" + $(this).val();
      });
      return r.substring(1);
    })(),
  };
  $.ajax({
    url: "/admincp/coin/SaveAudit",
    type: "post",
    data: data,
    success: function (result) {
      if (result.status != "error") {
        layer.msg(result.message);
        setTimeout(closethisdialog, 2000);
      } else {
        layer.alert(result.message, { icon: 2 });
      }
    },
  });
  return false;
});
$("#tab-11 textarea[name=BlockChainLink]").blur(function () {
  $("#BlockChainLink").val($(this).val());
});
$("#tab-11 input[name=ContractAddress]").blur(function () {
  $("#ContractAddress").val($(this).val());
});
$("#tab-11 select[name=TokenPlatForm_audit]").change(function () {
  $("#TokenPlatForm option[value='" + $(this).val() + "']").prop(
    "selected",
    true
  );
});

$("#BlockChainLink").blur(function () {
  $("#tab-11 textarea[name=BlockChainLink]").val($(this).val());
});
$("#ContractAddress").blur(function () {
  $("#tab-11 input[name=ContractAddress]").val($(this).val());
});
$("#TokenPlatForm").change(function () {
  $(
    "#tab-11 select[name=TokenPlatForm_audit] option[value='" +
      $(this).val() +
      "']"
  ).prop("selected", true);
});
$("#average_price_status").click(function () {
  setAverage_PriceStatus();
});

$("#SaveFormExplain").click(function () {
  var query = {
    ID: 0,
    CoinCode: $("#CoinCode").val(),
    LogoWriting: $("#LogoWriting").val(),
    ExpositoryWriting: $("#ExpositoryWriting").val(),
    EnglishWriting: $("#EnglishWriting").val(),
    Detail: getPosterData(),
  };
  $.ajax({
    url: "/admincp/coin/SaveCoinExplain/",
    type: "post",
    contentType: "application/json",
    data: JSON.stringify(query),
    success: function (r) {
      if (r.status == "success") {
        layer.msg("修改成功");
      } else {
        layer.alert("修改失败", { icon: 2 });
      }
    },
  });
});

$("#imglist #txtfile").change(function () {
  imgChange(this);
});

$("#teaminfo_save").click(function () {
  var data = {
    coincode: $("#CoinCode").val(),
    orgaddress: $("#tab-10 textarea[name='orgaddress']").val(),
    orgimg: $("#tab-10 input[name='orgimg']").val(),
    foundermore: $("#tab-10 textarea[name='foundermore']").val(),
  };
  if (data.orgimg == "") {
    layer.alert("组织图片不能为空");
    return;
  }
  $.ajax({
    url: "/admincp/coin/teaminfosave",
    type: "post",
    data: JSON.stringify(data),
    contentType: "application/json",
    beforeSend: function () {
      $("#teaminfo_save").prop("disabled", true);
    },
    success: function (result) {
      if (result.status != "error") {
        layer.msg(result.message);
      } else {
        layer.alert(result.message, { icon: 2 });
      }
    },
    complete: function () {
      $("#teaminfo_save").prop("disabled", false);
    },
  });
});

//机构/人物
$(".searchValue").keydown(function (event) {
  var vc = $(this).data("vc");
  var va = $(this).val();

  if (event.which == "13") search(vc, va);
});

$(".searchOrg").click(function () {
  var vc = $(this).data("vc");
  var va = $(this).prev().children().val();
  search(vc, va);
});

//更新ICO
$("#saveIco").click(function () {
  var status = $("#Status").val();
  var token_PlatForm = $("#Token_PlatForm").val();
  var public_Portfolio = $("#Public_Portfolio").val();
  var token_Percentage_For_Investors = $(
    "#Token_Percentage_For_Investors"
  ).val();
  var total_Tokens_Supply = $("#Total_Tokens_Supply").val();
  var token_Reserve_Split = $("#Token_Reserve_Split").val();
  var start_Date = $("#Start_Date").val();
  var end_Date = $("#End_Date").val();
  var sale_Website = $("#Sale_Website").val();
  var legal_Advisers = $("#Legal_Advisers").val();
  var jurisdiction = $("#Jurisdiction").val();
  var legal_Form = $("#Legal_Form").val();
  var security_Audit = $("#Security_Audit").val();
  var features = $("#Features").val();
  var funds_Raised_Cny = $("#Funds_Raised_Cny").val();
  var funds_Raised_Usd = $("#Funds_Raised_Usd").val();
  var funds_Raised_List = $("#Funds_Raised_List").val();
  var average_Price_Cny = $("#Average_Price_Cny").val();
  var average_Price = $("#Average_Price").val();
  var average_price_status = $("#average_price_status").val();
  var funding_Cap = $("#Funding_Cap").val();
  var funding_Target = $("#Funding_Target").val();
  var payment_Method = $("#Payment_Method").val();
  var startPriceCurrency = $("#StartPriceCurrency").val();
  var start_Price = $("#Start_Price").val();
  var coinCode = $("#IcoId").attr("data-code");
  $.ajax({
    type: "post",
    url: "/admincp/Coin/Ico",
    data: {
      status: status,
      token_PlatForm: token_PlatForm,
      public_Portfolio: public_Portfolio,
      token_Percentage_For_Investors: token_Percentage_For_Investors,
      total_Tokens_Supply: total_Tokens_Supply,
      token_Reserve_Split: token_Reserve_Split,
      start_Date: start_Date,
      end_Date: end_Date,
      sale_Website: sale_Website,
      legal_Advisers: legal_Advisers,
      jurisdiction: jurisdiction,
      legal_Form: legal_Form,
      security_Audit: security_Audit,
      features: features,
      funds_Raised_Cny: funds_Raised_Cny,
      funds_Raised_Usd: funds_Raised_Usd,
      funds_Raised_List: funds_Raised_List,
      average_Price_Cny: average_Price_Cny,
      average_Price: average_Price,
      average_price_status: average_price_status,
      funding_Cap: funding_Cap,
      funding_Target: funding_Target,
      payment_Method: payment_Method,
      startPriceCurrency: startPriceCurrency,
      start_Price: start_Price,
      coinCode: coinCode,
    },
    success: function (result) {
      if (result.status != "error") {
        layer.msg(result.message);
      } else {
        layer.alert(result.message, { icon: 2 });
      }
    },
  });
});

//更新社交信息
$("#SavaCoinIndex").click(function () {
  var coinCode = $("#CoinCode").val();
  //获取值
  var CodeLink = $("#CodeLink").val();
  var weiboLink = $("#weiboLink").val();
  var RedditLink = $("#RedditLink").val();
  var twitterLink = $("#twitterLink").val();
  var TelegramLink = $("#TelegramLink").val();
  var FacebookLink = $("#FacebookLink").val();
  var MediumLink = $("#MediumLink").val();
  var CrawLink = $("#CrawLink").val();

  //console.log(CrawLink);
  $.ajax({
    type: "post",
    url: "/admincp/Coin/CoinCrawIndex",
    dataType: "text",
    data: {
      coinCode: coinCode,
      Link: CrawLink,
    },
    success: function (data) {
      var result = JSON.parse(data);
    },
  });

  $.ajax({
    type: "post",
    url: "/admincp/Coin/CoinIndex",
    dataType: "text",
    data: {
      coinCode: coinCode,
      CodeLink: CodeLink,
      weiboLink: weiboLink,
      RedditLink: RedditLink,
      twitterLink: twitterLink,
      TelegramLink: TelegramLink,
      FacebookLink: FacebookLink,
      MediumLink: MediumLink,
    },
    success: function (data) {
      var result = JSON.parse(data);
      if (result.status != "error") {
        layer.msg(result.message);
      } else {
        layer.alert(result.message, { icon: 2 });
      }
    },
  });
});

$("#SaveCoinTypeTag").click(function () {
    var coinCode = $("#CoinCode").val();
  //loadding效果
  $.ajax({
    url: "/admincp/coin/SaveCoinTypeTag/",
    type: "post",
    data: {
      TagName: $("#CoinTagName").val(),
      TagUrl: $("#TagUrl").val(),
      NewsID: $("#NewsID").val(),
      KeyCode: coinCode,
    },
    success: function (r) {
      if (r.status == "success") {
        layer.msg("修改成功");
      } else {
        layer.alert("修改失败", { icon: 2 });
      }
    },
  });

  if ($("#CoinTagName").val() == "") {
    $("#TagUrl").val("");
    $("#NewsID").val(0);
  }
});

$("#SavaTag").click(function () {
    var coinCode = $("#CoinCode").val();
  var CodeString = "";
  var arr = [];
  for (var i = 0; i < $(".tagAllCoin").length; i++) {
    if ($(".tagAllCoin").eq(i).prop("checked")) {
      arr.push($(".tagAllCoin").eq(i).val());
    }
  }
  CodeString = arr.join(",");
  //loadding效果
  var loading = layer.load(0, {
    shade: [0.2, "#000"], //0.1透明度的背景
  });
  $.ajax({
    url: "/admincp/coin/SaveTagByCoinCode/",
    type: "post",
    data: {
      tagCodeString: CodeString,
      coinCode: coinCode,
    },
    success: function (r) {
      if (r.status == 1) {
        layer.msg("修改成功");
      } else {
        layer.alert("修改失败", { icon: 2 });
      }
      },
      complete: function () {
          layer.close(loading);
      }
  });
});

$("#saveRate").click(function () {
  var totalnum = $("#totalNum").val();

  if (totalnum == "") {
    totalnum = 0;
  }

  var isPass = true;
  //校验是否正确通过
  $("label[name=RateNameValidate]").each(function () {
    var html = $(this).html();
    var mark = $(this).attr("mark");
    if (html != "") {
      isPass = false;
      var t = eval('$("#RateName' + mark + '")').offset().top;
      $(window).scrollTop(t);
      return false;
    }
  });
  var dataListRate = new Array();
  var totalRateValue = 0;
  $("label[name=RateValueValidate]").each(function () {
    var html = $(this).html();
    var mark = $(this).attr("mark");
    if (html != "") {
      isPass = false;
      var t = eval('$("#RateValue' + mark + '")').offset().top;
      $(window).scrollTop(t);
      return false;
    } else {
      var rateName = eval('$("#RateName' + mark + '")').val();
      var rateValue = eval('$("#RateValue' + mark + '")').val();
      var rateRemark = eval('$("#RateRemark' + mark + '")').val();
      //alert("rateName=" + rateName + ",rateValue=" + rateValue + "," + (rateName == " ") + (rateName == ''));
      if ((rateName == "" || rateName == " ") && rateValue != "") {
        isPass = false;

        layer.msg("有占比得代币名称不可为空！");

        return true;
      }
      if (rateName != "" && rateName != " " && rateValue == "") {
        isPass = false;

        layer.msg("有代币名称的占比不能为空！");

        return true;
      }

      // alert("totalRateValue=" + totalRateValue + ",rateValue=" + rateValue + ",parseFloat(rateValue).toFixed(2)=" + parseFloat(rateValue).toFixed(2))
      if (rateName != "" && rateValue != "") {
        var data = {};
        data.RateName = rateName;
        data.RateValue = rateValue;
        data.RateRemark = rateRemark;
        data.No = mark;
        data.RateNum = parseFloat(
          (parseFloat(data.RateValue) * parseFloat(totalnum)) / 100
        );
        dataListRate.push(data);
        totalRateValue =
          parseFloat(totalRateValue) + parseFloat(rateValue * 100);
      }
    }
  });
  totalRateValue = totalRateValue / 100;
  //console.log("totalRateValue:", totalRateValue.toFixed(0));
  if (totalRateValue.toFixed(0) - 100 != 0) {
    if (dataListRate.length > 0) {
      isPass = false;
      layer.msg("占比只能是100%,请核对占比后再操作");
      return true;
    }
  }
  if (isPass) {
    //校验通过，开始请求url链接

    var jsondata = {};
    jsondata.CoinCode = $("#coinRateId").val();
    jsondata.CoinRateSaveDataList = dataListRate;
    $.ajax({
      url: "/admincp/coin/SaveCoinRate/",
      type: "post",
      dataType: "json",
      data: {
        CoinRateSaveCommand: JSON.stringify(jsondata),
      },
      success: function (r) {
        if (r.status == 1) {
          layer.msg("修改成功");
        } else {
          layer.alert("修改失败", { icon: 2 });
        }
        //layer.close(loading);
      },
    });
  }
});


$("#CoinIcon").bind("input propertychange", function () {
  $("#CIImg").attr("src", $("#CoinIcon").val());
});

//打开网址
$(".openSiteLink").click(function () {
  var siteLink = $("#SiteLink").val();
  if (siteLink != "") {
    var array = siteLink.split("\n");
    if (array.length != 0) {
      array.forEach((a) => {
        if (a.indexOf("https://") == -1 && a.indexOf("http://") == -1) {
          a = "https://" + a;
        }
        window.open(a);
      });
    }
  }
});

$(".openBlockChainLink").click(function () {
  var blockChainLink = $(this)
    .parent()
    .next()
    .find("textarea[name='BlockChainLink']")
    .val();
  if (blockChainLink != "") {
    var array = blockChainLink.split("\n");
    if (array.length != 0) {
      array.forEach((a) => {
        if (a.indexOf("https://") == -1 && a.indexOf("http://") == -1) {
          a = "https://" + a;
        }
        window.open(a);
      });
    }
  }
});

$(".openCodeLink").click(function () {
  var codeLinks = $("#CodeLink").val();
  if (codeLinks != "") {
    var array = codeLinks.split("\n");
    if (array.length != 0) {
      array.forEach((a) => {
        if (a.indexOf("https://") == -1 && a.indexOf("http://") == -1) {
          a = "https://" + a;
        }
        window.open(a);
      });
    }
  }
});

$(".openweiboLink").click(function () {
  var codeLink = $("#weiboLink").val();
  if (codeLink != "") {
    var array = codeLink.split("\n");
    if (array.length != 0) {
      array.forEach((a) => {
        if (a.indexOf("https://") == -1 && a.indexOf("http://") == -1) {
          a = "https://" + a;
        }
        window.open(a);
      });
    }
  }
});

$(".openCrawLink").click(function () {
  var codeLink = $("#CrawLink").val();
  if (codeLink != "") {
    var array = codeLink.split("\n");
    if (array.length != 0) {
      array.forEach((a) => {
        if (a.indexOf("https://") == -1 && a.indexOf("http://") == -1) {
          a = "https://" + a;
        }
        window.open(a);
      });
    }
  }
});

$(".openRedditLink").click(function () {
  var codeLink = $("#RedditLink").val();
  if (codeLink != "") {
    var array = codeLink.split("\n");
    if (array.length != 0) {
      array.forEach((a) => {
        if (a.indexOf("https://") == -1 && a.indexOf("http://") == -1) {
          a = "https://" + a;
        }
        window.open(a);
      });
    }
  }
});

$(".opentwitterLink").click(function () {
  var codeLink = $("#twitterLink").val();
  if (codeLink != "") {
    var array = codeLink.split("\n");
    if (array.length != 0) {
      array.forEach((a) => {
        if (a.indexOf("https://") == -1 && a.indexOf("http://") == -1) {
          a = "https://" + a;
        }
        window.open(a);
      });
    }
  }
});

$(".openTelegramLink").click(function () {
  var codeLink = $("#TelegramLink").val();
  if (codeLink != "") {
    var array = codeLink.split("\n");
    if (array.length != 0) {
      array.forEach((a) => {
        if (a.indexOf("https://") == -1 && a.indexOf("http://") == -1) {
          a = "https://" + a;
        }
        window.open(a);
      });
    }
  }
});

$(".openFacebookLink").click(function () {
  var codeLink = $("#FacebookLink").val();
  if (codeLink != "") {
    var array = codeLink.split("\n");
    if (array.length != 0) {
      array.forEach((a) => {
        if (a.indexOf("https://") == -1 && a.indexOf("http://") == -1) {
          a = "https://" + a;
        }
        window.open(a);
      });
    }
  }
});

$("#qingkong").click(function () {
  $("#Status").val("");
  $("#Token_PlatForm").val("");
  $("#Public_Portfolio").val("");
  $("#Token_Percentage_For_Investors").val("");
  $("#Total_Tokens_Supply").val("0");
  $("#Token_Reserve_Split").val("");
  $("#Start_Date").val("1990-01-01");
  $("#End_Date").val("1990-01-01");
  $("#Start_Price").val("0");
  $("#StartPriceCurrency").val("");
  $("#Payment_Method").val("");
  $("#Funding_Target").val("");
  $("#Funding_Cap").val("");
  $("#Average_Price").val("0");
  $("#Average_Price_Cny").val("0");
  $("#Funds_Raised_List").val("");
  $("#Funds_Raised_Usd").val("0");
  $("#Funds_Raised_Cny").val("0");
  $("#Features").val("");
  $("#Security_Audit").val("");
  $("#Legal_Form").val("");
  $("#Jurisdiction").val("");
  $("#Legal_Advisers").val("");
  $("#Sale_Website").val("");
});

$("#btntag").click(function () {
  var html = getTagHtml("", "", "");
  $("#divTag").append(html);
});

//TDK变更语言时
$("#SeoLangSelect").change(function () {
    var coinCode = $("#CoinCode").val();
  //save();
  var id = $(this).val();
  $.ajax({
    url: "/admincp/coin/SeoFindLang?langCode=" + id + "&seoCode=" + coinCode,
    success: function (r) {
      $("#seosavelang").attr("data-id", id);
      $("#seodeletelang").attr("data-id", id);
      $("#SeoTitle").val(r == null ? "" : r.title);
      $("#SeoKeyWords").val(r == null ? "" : r.keyWords);
      $("#SeoDescription").val(r == null ? "" : r.description);
    },
  });
});

//TDK多语言保存
$("#seosavelang").click(function () {
  seosave();
});

//TDK多语言删除
$("#seodeletelang").click(function () {
  seodel();
});

$("#WalletTable").delegate(".upbutton", "click", function () {
  var coinCode = $("#WalletTable").attr("data-coin");
  var walletIdOne = $(this).parent().parent().attr("data-id");
  var orderNoOne = $(this).parent().parent().attr("data-orderno");
  var walletIdTwo = $(this).parent().parent().prev().attr("data-id");
  var orderNoTwo = $(this).parent().parent().prev().attr("data-orderno");

  $.ajax({
    url: "/admincp/coin/UpdateOrderNoAsync",
    type: "post",
    data: {
      walletIdOne: walletIdOne,
      orderNoOne: orderNoOne,
      walletIdTwo: walletIdTwo,
      orderNoTwo: orderNoTwo,
      coinCode: coinCode,
    },
    success: function (result) {
      if (result.status != "error") {
        layer.msg(result.message);

        $("#WalletTr").find("tr").remove();
        getWallet();
      } else {
        layer.alert(result.message, { icon: 2 });
      }
    },
  });
});

$("#WalletTable").delegate(".downbutton", "click", function () {
  var coinCode = $("#WalletTable").attr("data-coin");
  var walletIdOne = $(this).parent().parent().attr("data-id");
  var orderNoOne = $(this).parent().parent().attr("data-orderno");

  var walletIdTwo = $(this).parent().parent().next().attr("data-id");
  var orderNoTwo = $(this).parent().parent().next().attr("data-orderno");

  $.ajax({
    url: "/admincp/coin/UpdateOrderNoAsync",
    type: "post",
    data: {
      walletIdOne: walletIdOne,
      orderNoOne: orderNoOne,
      walletIdTwo: walletIdTwo,
      orderNoTwo: orderNoTwo,
      coinCode: coinCode,
    },
    success: function (result) {
      if (result.status != "error") {
        layer.msg(result.message);
        $("#WalletTr").find("tr").remove();
        getWallet();
      } else {
        layer.alert(result.message, { icon: 2 });
      }
    },
  });
});

$("#mainpoint_langselect").change(function () {
    document.getElementById("btnadd").style.display = "block";
    var id = $(this).val();
    $("#mainpoint_panel").empty();
    $.ajax({
        url: "/admincp/coin/GetMainPointByLang?langCode=" + id + "&coinCode=" + coinCode,
        success: function (r) {
            var obj = eval(r);
                for (i = 0; i < obj.length; i++) {
                var dh = '';
                var div = document.createElement("div");
                var newid = "pannl" + i
                div.setAttribute("id", newid);
                $("#mainpoint_panel").append(div);
                dh += '<div class="form-group" style="margin-top:55px;">';
                dh += '  <label class="col-sm-2 control-label" style="padding-right:0px">一级标题：</label>';
                dh += '     <div class="col-sm-2">';
                dh += '     <input style="padding: 0px 5px;" id=' + 'itemname' + obj[i].title + ' value=' + obj[i].title + ' name=' + 'itemname' + ' maxlength="10" data-vc="2" type="text" class="form-control searchValue">';
                dh += '     </div>';
                dh += "     <a class='col-sm-2 btn btn-primary searchOrg' style='width:60px;' id=" + obj[i].title + " value=" + obj[i].title + " onclick='MainUpTitle(" + newid + "," + newid + ")' data-vc='2'>上移</a>";
                dh += "     <a class='col-sm-2 btn btn-primary searchOrg' style='width:60px;' id=" + obj[i].title + " value=" + obj[i].title + " onclick='MainDownTitle(" + newid + "," + newid + ")' data-vc='2'>下移</a>";
                dh += " <a class='col-sm-2 btn btn-danger searchOrg' style='width:60px;' id='searchOrg' data-vc='2' onclick=DeleteMain(" + newid + ")>删除</a>";
                dh += '</div>';
                for (k = 0; k < obj[i].subtitle.length; k++) {
                    var newobj = {
                        "newid": newid,
                        "id": obj[i].subtitle[k].second_title
                    }
                    arr = obj[i].subtitle[k];
                    dh += "<div class='form-group' id=" + "sub" + newid + JSON.stringify(k) + " style='margin-top:15px;' >";
                    dh += '  <label class="col-sm-2 control-label" style="padding-right:0px">二级标题：</label>';
                    dh += '     <div class="col-sm-2">';
                    dh += '     <input style="padding: 0px 5px;" id=' + obj[i].subtitle[k].second_title + ' value=' + obj[i].subtitle[k].second_title + ' name=' + newid + 'subitemname' + ' maxlength="10" data-vc="2" type="text" class="form-control searchValue">';
                    dh += '     </div>';
                    dh += ' <label class="col-sm-2 control-label" style="padding-right:0px;width: 130px;">正文：</label>';
                    dh += '     <div class="col-sm-4">';
                    //dh += '     <input style="padding: 0px 5px;" id=' + newid+k + ' value=' + obj[i].subtitle[k].subContent.substring(0, 3) + ' name=' + 'subitemdesc'+' data-vc="2" type="text" class="form-control searchValue">';
                    dh += '      <textarea style="padding: 0px 5px;" id=' + 'main' + newid + k + ' name=' + 'subitemdesc' + '  data-vc="2" type="text" class="form-control searchValue" required>' + obj[i].subtitle[k].subContent + '</textarea>';
                    dh += '     </div>';
                    dh += "     <a class='col-sm-2 btn btn-primary searchOrg' style='width:60px;margin-left:15px;' id=" + obj[i].subtitle[k].second_title + " value=" + obj[i].subtitle[k].second_title + " onclick='UpTitle(" + "sub" + newid + JSON.stringify(k) + "," + newid + ")' data-vc='2'>上移</a>";
                    dh += "     <a class='col-sm-2 btn btn-primary searchOrg' style='width:60px;margin-left:5px;' id=" + obj[i].subtitle[k].second_title + " value=" + obj[i].subtitle[k].second_title + " onclick='DownTitle(" + "sub" + newid + JSON.stringify(k) + "," + newid + ")' data-vc='2'>下移</a>";
                    dh += " <a class='col-sm-2 btn btn-primary searchOrg' style='width:60px;' id='searchOrg' onclick='addsubtitle(" + JSON.stringify(newobj).replace(/'/g, '&quot;') + ", " + newid + ")' data-vc='2' >添加</a>";
                    dh += " <a class='col-sm-2 btn btn-danger searchOrg' style='width:60px;' id='searchOrg' data-vc='2' onclick='DeleteMain(" + "sub" + newid + JSON.stringify(k) + ")'>删除</a>";
                    dh += '</div>';
                }
                document.getElementById(newid).innerHTML = dh;
            }

        }
    });
});
$("#mainpoint_from button[type='submit']").click(function () {
    var data = $("#mainpoint_from").serializeArray();
    var mainlist = []
    var submainlist = []
    var qued = 0;
    var recopy = 0;
    for (var index = 0; index < data.length; index++) {
        var maintitle = {
            id: 0,
            languageCode: data[1].value,
            code: data[0].value,
            title: "",
        }
        if (data[index].name == "itemname") {
            var CoreMainID = parseInt(Math.random() * 100000)
            maintitle.title = data[index].value;
            maintitle.id = CoreMainID;
            mainlist.push(maintitle);
        }
    }
    for (var index1 = 1; index1 < data.length - 2; index1++) {
        var res = {
            subtitle: "",
            subcontent: "",
            sort: 0,
            CoreMainID: 0
        }
        if (data[index1 + 1].name == "subitemdesc" && data[index1 + 2].name != "itemname") {
            qued += 1;
            res.subtitle = data[index1].value.replace('"', "'");
            res.subcontent = data[index1 + 1].value.replace('"', "”");
            res.subcontent = res.subcontent.replace(/\"/g, "”");
            res.CoreMainID = mainlist[recopy].id;
            res.sort = qued;
            if (res.subcontent != "" && res.subcontent.indexOf('"') < 0)
                submainlist.push(res);
            else {
                res.subcontent.replace('"', '“')
                submainlist.push(res);
            }
        }
        if (data[index1 + 1].name == "subitemdesc" && data[index1 + 2].name == "itemname") {
            qued += 1;
            res.subtitle = data[index1].value;
            res.subcontent = data[index1 + 1].value.replace('"', "”");
            res.subcontent = res.subcontent.replace(/\"/g, "”");
            res.CoreMainID = mainlist[recopy].id;
            res.sort = qued;
            if (res.subcontent != "" && res.subcontent.indexOf('"') < 0)
                submainlist.push(res);
            else {
                res.subcontent.replace('"', '“')
                submainlist.push(res);
            }
            recopy += 1;
            qued = 0;
        }
    }
    //console.log("mainlist:", JSON.stringify(submainlist))
    if (mainlist.length == 0 && submainlist.length == 0) {
        var maintitle = {
            id: 0,
            languageCode: data[1].value,
            code: data[0].value,
            title: "",
        }
        mainlist.push(maintitle)
    }
    postdata = {
        mainlist: mainlist,
        submainlist: submainlist
    }
    $.ajax({
        url: "/admincp/coin/SaveCoreMainByLang",
        type: "post",
        contentType: "application/json",
        data: JSON.stringify(postdata),
        success: function (r) {
            if (r.status == "success") {
                layer.msg('修改成功');
                layer.msg(r.message, {
                    time: 5000 //2秒关闭（如果不配置，默认是3秒）
                },
                    //closethisdialog()
                );
            } else {
                layer.alert("修改失败", { icon: 2 });
            }

        }
    });
    return false;
});




$("#crowdfunding_from button[type='button']").click(function () {
    AddCrowdfunding_SingleItem(undefined);
});
$("#crowdfunding_from button[type='submit']").click(function () {
    var data = $("#crowdfunding_from").serializeArray();

    //var xx = JSON.stringify(data);
    //alert(xx);
    //return false;

    var postdata = {
        name: "",
        icofirst: "",
        icototal: "",
        ProjectDesc: []
    };
    var tmparray = new Array();
    var carray = { name: "", items: [] };
    data.forEach(function (v) {
        //if (v.name == "name") {
        //    carray = { name: v.value,items:[]};
        //    postdata.ProjectDesc.push(carray)
        //} else
        if (v.name == "icofirst") {
            postdata.icofirst = $.trim(v.value);
        }
        if (v.name == "icosum") {
            postdata.icototal = $.trim(v.value);
        }

        if (v.name == "itemname") {
            tmparray = new Array();
            var tempv = $.trim(v.value);
            if (tempv.length > 0) {
                tmparray.push(tempv);
            }
        } else if (v.name == "itemvalue") {
            var tempv = $.trim(v.value);
            if (tmparray.length > 0 && tempv.length > 0) {
                tmparray.push(tempv);
            } else {
                tmparray.push('');
            }
        } else if (v.name == "itemdesc") {
            var tempv = $.trim(v.value);
            if (tmparray.length > 1 && tempv.length > 0) {
                tmparray.push(tempv);

                var tmparraytmp = new Array();
                tmparraytmp[0] = tmparray[0];
                tmparraytmp[1] = tmparray[2];
                tmparraytmp[2] = tmparray[1];
                carray.items.push(tmparraytmp);
            }
        } else {
            postdata[v.name] = v.value;
        }
    });
    if (carray.items.length > 0) {
        postdata.ProjectDesc.push(carray);
    }
    postdata.ProjectDesc = JSON.stringify(postdata.ProjectDesc);
    //console.log("postdata:", JSON.stringify(postdata));
    $.ajax({
        url: "/admincp/coin/SaveCrowdfundingByLang",
        type: "post",
        contentType: "application/json",
        data: JSON.stringify(postdata),
        success: function (r) {
            if (r.status == "success") {
                layer.msg('修改成功');
            } else {
                layer.alert("修改失败", { icon: 2 });
            }
        }
    });
    return false;
});

$("#crowdfunding_langselect").change(function () {
    var coinCode = $("#CoinCode").val();
    var id = $(this).val();
    $("#crowdfunding_panel").empty();
    $.ajax({
        url: "/admincp/coin/GetCrowdfundingByLang?langCode=" + id + "&coinCode=" + coinCode,
        success: function (r) {
            var data = [{ name: "", items: [] }];
            if (r) {
                $("#crowdfunding_from input[name='name']").val(r.name);
                if (r.projectdesc) {
                    data = JSON.parse(r.projectdesc);
                }
            }
            if (data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    var ditem = data[i];
                    AddCrowdfunding_Item(ditem);
                }
            } else {
                AddCrowdfunding_Item(undefined);
            }
        }
    });
    return false;
});

$("#lockpositionreleaseinfo_langselect").change(function () {
    var coinCode = $("#CoinCode").val();
    var id = $(this).val();
    $.ajax({
        url: "/admincp/coin/FindLang?langCode=" + id + "&coinCode=" + coinCode,
        success: function (r) {
            $("#LockPositionReleaseInfo").val(r == null ? "" : r.lockPositionReleaseInfo);
        }
    });
});
$("#lockpositionreleaseinfo_from button[type='submit']").click(function () {
    var data = $("#lockpositionreleaseinfo_from").serializeArray();
    if ($("#lockpositionreleaseinfo_from select[name='LanguageCode']").val() == "") {
        layer.alert("请选择语言");
        return false;
    }
    $.ajax({
        url: "/admincp/coin/SaveLockPositionReleaseInfoByLang",
        type: "post",
        data: data,
        success: function (r) {
            if (r.status == "success") {
                layer.msg('修改成功');
            } else {
                layer.alert("修改失败", { icon: 2 });
            }
        }
    });
    return false;
});

$("#forking_from button[type='button']").click(function () {
    Addforking_Item(undefined);
});
$("#forking_from button[type='submit']").click(function () {
    if ($("#forking_from select[name='LanguageCode']").val() == "") {
        layer.alert("请选择语言");
        return false;
    }
    var data = $("#forking_from").serializeArray();
    var postdata = {
        name: "",
        forking: []
    };
    var tmparray = new Array();
    data.forEach(function (v) {
        if (v.name == "name") {
            postdata.name = v.value;
        } else if (v.name == "itemname") {
            tmparray = new Array();
            var tempv = $.trim(v.value);
            if (tempv.length > 0) {
                tmparray.push(tempv);
            }
        } else if (v.name == "itemdesc") {
            var tempv = $.trim(v.value);
            if (tmparray.length > 0 && tempv.length > 0) {
                tmparray.push(tempv);
                postdata.forking.push(tmparray);
            }
        } else {
            postdata[v.name] = v.value;
        }
    });
    postdata.forking = JSON.stringify(postdata.forking);
    //console.log(postdata);
    $.ajax({
        url: "/admincp/coin/SaveforkingByLang",
        type: "post",
        contentType: "application/json",
        data: JSON.stringify(postdata),
        success: function (r) {
            if (r.status == "success") {
                layer.msg('修改成功');
            } else {
                layer.alert("修改失败", { icon: 2 });
            }
        }
    });
    return false;
});

$("#forking_langselect").change(function () {
    var id = $(this).val();
    var coinCode = $("#CoinCode").val();
    $("#forking_panel").empty();
    $.ajax({
        url: "/admincp/coin/GetForkingByLang?langCode=" + id + "&coinCode=" + coinCode,
        success: function (r) {
            var itemlist = new Array();
            if (r) {
                $("#forking_from input[name='name']").val(r.name);
                if (r.forking) {
                    itemlist = JSON.parse(r.forking);
                }
            }
            for (var i = 0; i < 20; i++) {
                var item = itemlist.length > i ? itemlist[i] : ["", ""];
                Addforking_Item(item);
            }
        }
    });
});

$("[name=addexplaincoin]").click(function () {
    // 0为img 1为video
    var type = 0
    var defaultData = { skipType: type };
    addcoin(defaultData);
    return false;
});
$("[name=addexplainurl]").click(function () {
    // 0为img 1为video
    var type = 1
    var defaultData = { skipType: type };
    addcoin(defaultData);
    return false;
});

document.onkeydown = function (event) {
  var target, code, tag;
  if (!event) {
    event = window.event; //针对ie浏览器
    target = event.srcElement;
    code = event.keyCode;
    if (code == 13) {
      tag = target.tagName;
      if (tag == "TEXTAREA") {
        return true;
      } else {
        return false;
      }
    }
    if (code == 27) {
      closethisdialog();
    }
  } else {
    target = event.target; //针对遵循w3c标准的浏览器，如Firefox
    code = event.keyCode;
    if (code == 13) {
      tag = target.tagName;
      if (tag == "INPUT") {
        return false;
      } else {
        return true;
      }
    }
    if (code == 27) {
      closethisdialog();
    }
  }
};
