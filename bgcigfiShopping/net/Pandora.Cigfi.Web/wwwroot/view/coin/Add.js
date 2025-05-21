$(function () {

    $.ajax({
        type: "get",
        url: "/admincp/coin/FindCoinExplain?coincode=" + $("#CoinCode").val(),
        success: function (model) {
            //console.log("Model:", JSON.stringify(model));
            $("#LogoWriting").val(model.logoWriting)
            $("#ExpositoryWriting").val(model.expositoryWriting)
            $("#EnglishWriting").val(model.englishWriting)
            for (var i = 0; i < model.detail.length; i++) {
                addcoin(model.detail[i]);
            }
        }
    })

    //var coinCode = "@coinCode";

    var coinCode = $("#CoinCode").val();

    var starLabel = ' <option value="';
    var endLabel = '</option>';
    //富文本 将textarea替换成富文本编辑框
    var CKTeamDesc = CKEDITOR.replace('TeamDesc');
    var CKDescription = CKEDITOR.replace('Description');


    //变更语言时
    $("#LangSelect").change(function () {
        var coinCode = $("#CoinCode").val();
        var id = $(this).val();
        console.log(1);
        //console.log("语言变更时id的变换:", id);
        $("#divTag").html("");
        if (id == "") {
            $("#divTagAdd").css("display", "none");
        } else {
            $("#divTagAdd").css("display", "");
        }
        console.log(2);
        $.ajax({
            url: "/admincp/coin/FindLang?langCode=" + id + "&coinCode=" + coinCode,
            success: function (r) {
                console.log(3);
                console.log("成功")
                console.log(r)
                $("#savelang").attr("data-id", id);
                $("#NativeName").val(r == null ? "" : r.nativeName);

                $("#SceneTag").val(r == null ? "" : r.sceneTag);
                CKTeamDesc.setData(r == null ? "" : r.teamDesc);
                CKDescription.setData(r == null ? "" : r.description);

                var tags = r.sceneTagList;

                if (tags.length > 0) {
                    var html = "";
                    for (var i = 0; i < tags.length; i++) {
                        html =
                            html + getTagHtml(tags[i].keywords, tags[i].link, tags[i].jumpId);
                    }
                    $("#divTag").append(html);
                } else {
                    $("#divTag").append(getTagHtml("", "", ""));
                }
            },
        });
        //save();
    });

    //多语言保存
    $("#savelang").click(function () {
        console.log(5);
        save(CKTeamDesc.getData(), CKDescription.getData());
    });


    //关联机构
    layui.use('table', function () {
        var table = layui.table;
        var tableIns = table.render({
            elem: '#table-9'
            , height: 500
            , initSort: {
                field: 'orderNo' //排序字段，对应 cols 设定的各字段名
            }
            , loading: false
            , url: '/admincp/coin/CoinRelatedOrd' //数据接口
            , where: { //设定异步数据接口的额外参数，任意设
                coinCode: coinCode,
                rtype: 3,
                itype: 2

            }
            , page: true //开启分页
            , cols: [[ //表头
                { field: 'itemCode', title: '代码', width: 210, fixed: 'left' }
                , { field: 'itemName', title: '名称', width: 210, event: 'detail' }
                , { field: 'orderNo', title: '排序', sort: true, edit: 'text', width: 80 }
                , { fixed: 'right', width: 70, title: "操作", align: 'center', toolbar: '#barCustom' }
            ]]
            , response: {
                statusName: 'status' //规定数据状态的字段名称，默认：code
                , statusCode: 200 //规定成功的状态码，默认：0
                , countName: 'total' //规定数据总数的字段名称，默认：count
                , dataName: 'rows' //规定数据列表的字段名称，默认：data
            }
        });
        table.on('tool(table-9-filter)', function (obj) {
            var data = obj.data; //获得当前行数据
            if ("del" == obj.event) {
                $.ajax({
                    url: "/admincp/coin/DelOrgCoin",
                    type: "post",
                    data: {
                        coinCode: coinCode,
                        itemCode: obj.data.itemCode,
                        rtype: 3
                    },
                    success: function (result) {
                        if (result.status != "error") {
                            layer.msg(result.message);
                        }
                        else {
                            layer.alert(result.message, { icon: 2 });
                        }
                        obj.del(); //删除对应行（tr）的DOM结构，并更新缓存
                        tableIns.reload({
                            initSort: obj //记录初始排序，如果不设的话，将无法标记表头的排序状态。
                            , where: { //设定异步数据接口的额外参数，任意设
                                coinCode: coinCode,
                                rtype: 3,
                                itype: 2
                            }
                            , page: {
                                curr: 1 //重新从第 1 页开始
                            }
                        });
                    }

                })
            }
            else if ("detail" == obj.event) {
                layer.open({
                    type: 2,
                    title: '币圈编辑',
                    shadeClose: false,
                    shade: 0.6,
                    maxmin: true, //开启最大化最小化按钮
                    area: ['1050px', '730px'],
                    content: '/admincp/baike/Detail?id=' + obj.data.itemCode,
                    end: function () {
                        tableIns.reload({
                            initSort: obj //记录初始排序，如果不设的话，将无法标记表头的排序状态。
                            , where: { //设定异步数据接口的额外参数，任意设
                                coinCode: coinCode,
                                rtype: 3,
                                itype: 2
                            }
                            , page: {
                                curr: 1 //重新从第 1 页开始
                            }
                        });

                    }

                })
            }
        });
        table.on('edit(table-9-filter)', function (obj) {
            if (obj.field == "orderNo") {
                $.ajax({
                    url: "/admincp/coin/UpdateOrgCoin",
                    type: "post",
                    data: {
                        coinCode: coinCode,
                        itemCode: obj.data.itemCode,
                        orderNo: obj.value,
                        rtype: 3,

                    },
                    success: function (result) {
                        if (result.status != "error") {
                            layer.msg(result.message);
                            tableIns.reload({
                                initSort: obj //记录初始排序，如果不设的话，将无法标记表头的排序状态。
                                , where: { //设定异步数据接口的额外参数，任意设
                                    coinCode: coinCode,
                                    rtype: 3,
                                    itype: 2
                                }
                                , page: {
                                    curr: 1 //重新从第 1 页开始
                                }
                            });
                        }
                        else {
                            layer.alert(result.message, { icon: 2 });
                        }
                    }

                })

            }
        });

        //关联成员
        var tablePerson = table.render({
            id: "table-10",
            elem: '#table-10'
            , height: 500
            , initSort: {
                field: 'orderNo' //排序字段，对应 cols 设定的各字段名
            }
            , url: '/admincp/coin/CoinRelatedOrd' //数据接口
            , where: { //设定异步数据接口的额外参数，任意设
                coinCode: coinCode,
                rtype: 3,
                itype: 1
            }
            , page: true //开启分页
            , cols: [[ //表头
                { field: 'itemCode', title: '代码', width: 180 }
                , { field: 'itemName', title: '名称', width: 180, event: 'detail' },
                {
                    field: 'itemName', title: '是否顾问', width: 90, event: 'isConsultant'
                    , templet: function (d) {//UpdateIsFounder(coinCode,  itemCode,  rtype,  itpye,  IsFounder) return "<input type='radio' name='isFounder' " + (d.isFounder == 1 ? "checked='checked'" : "") + " onclick='UpdateIsFounder(\"" + coinCode + "\",\"" + d.itemCode + "\"," + 3 + "," + 1 +",this.checked?1:0)'/>";
                        var checked = (d.isConsultant == 1 ? "checked='checked'" : "");
                        return "&nbsp;&nbsp;<input type='checkbox' name='isConsultant' isConsultant=" + d.isConsultant + " " + checked + "/>";
                    }
                },
                {
                    field: 'itemName', title: '是否创始人', width: 100, event: 'isFounder'
                    , templet: function (d) {//UpdateIsFounder(coinCode,  itemCode,  rtype,  itpye,  IsFounder) return "<input type='radio' name='isFounder' " + (d.isFounder == 1 ? "checked='checked'" : "") + " onclick='UpdateIsFounder(\"" + coinCode + "\",\"" + d.itemCode + "\"," + 3 + "," + 1 +",this.checked?1:0)'/>";
                        var checked = (d.isFounder == 1 ? "checked='checked'" : "");
                        return "&nbsp;&nbsp;<input type='checkbox' name='isFounder' isFounder=" + d.isFounder + " " + checked + "/>";
                    }
                },
                //{
                //    type: 'radio', title: '是否创始人', width: 100, event: 'isFounder'

                //},
                , { field: 'orderNo', title: '排序', sort: true, edit: 'text', width: 80 }
                , { fixed: 'right', width: 70, title: "操作", align: 'center', toolbar: '#barCustom' }
            ]]
            , response: {
                statusName: 'status' //规定数据状态的字段名称，默认：code
                , statusCode: 200 //规定成功的状态码，默认：0
                , countName: 'total' //规定数据总数的字段名称，默认：count
                , dataName: 'rows' //规定数据列表的字段名称，默认：data
            }
        });
        table.on('tool(table-10-filter)', function (obj) {
            var data = obj.data; //获得当前行数据
            if ("del" == obj.event) {
                $.ajax({
                    url: "/admincp/coin/DelOrgCoin",
                    type: "post",
                    data: {
                        coinCode: coinCode,
                        itemCode: obj.data.itemCode,
                        rtype: 3
                    },
                    success: function (result) {
                        if (result.status != "error") {
                            layer.msg(result.message);
                        }
                        else {
                            layer.alert(result.message, { icon: 2 });
                        }
                        obj.del(); //删除对应行（tr）的DOM结构，并更新缓存
                        tablePerson.reload({
                            initSort: obj //记录初始排序，如果不设的话，将无法标记表头的排序状态。
                            , where: { //设定异步数据接口的额外参数，任意设
                                coinCode: coinCode,
                                rtype: 3,
                                itype: 1
                            }
                            , page: {
                                curr: 1 //重新从第 1 页开始
                            }
                        });
                    }

                })
            }
            else if ("detail" == obj.event) {
                layer.open({
                    type: 2,
                    title: '币圈编辑',
                    shadeClose: false,
                    shade: 0.6,
                    maxmin: true, //开启最大化最小化按钮
                    area: ['1050px', '730px'],
                    content: '/admincp/baike/Detail?id=' + obj.data.itemCode,
                    end: function () {
                        tablePerson.reload({
                            initSort: obj //记录初始排序，如果不设的话，将无法标记表头的排序状态。
                            , where: { //设定异步数据接口的额外参数，任意设
                                coinCode: coinCode,
                                rtype: 3,
                                itype: 1
                            }
                            , page: {
                                curr: 1 //重新从第 1 页开始
                            }
                        });
                    }
                })
            } else if (obj.event == "isFounder") {
                //layer.msg("isFounder");
                var IsFounder = obj.tr.find("input[name='isFounder']").prop("checked") ? 1 : 0;
                UpdateIsFounder(coinCode, obj.data.itemCode, 3, 1, IsFounder);
                //console.log(obj.tr.find("input[type='layTableCheckbox']").length);
                //console.log(obj.tr);
                //console.log(obj.data);
                //var checkStatus = table.checkStatus('table-10-filter'); //idTest 即为基础参数 id 对应的值

                //console.log(checkStatus.data) //获取选中行的数据
                //checkStatus = table.checkStatus('table-10'); //idTest 即为基础参数 id 对应的值

                //console.log(checkStatus.data) //获取选中行的数据
            } else if (obj.event == "isConsultant") {
                var IsConsultant = obj.tr.find("input[name='isConsultant']").prop("checked") ? 1 : 0;
                UpdateIsConsultant(coinCode, obj.data.itemCode, 3, 1, IsConsultant);
            }
        });
        table.on('edit(table-10-filter)', function (obj) { //注：tool是工具条事件名，test是table原始容器的属性 lay-filter="对应的值"
            $.ajax({
                url: "/admincp/coin/UpdateOrgCoin",
                type: "post",
                data: {
                    coinCode: coinCode,
                    itemCode: obj.data.itemCode,
                    orderNo: obj.value,
                    rtype: 3
                },
                success: function (result) {
                    if (result.status != "error") {
                        layer.msg(result.message);
                        tablePerson.reload({
                            initSort: obj //记录初始排序，如果不设的话，将无法标记表头的排序状态。
                            , where: { //设定异步数据接口的额外参数，任意设
                                coinCode: coinCode,
                                rtype: 3,
                                itype: 1
                            }
                            , page: {
                                curr: 1 //重新从第 1 页开始
                            }
                        });
                    }
                    else {
                        layer.alert(result.message, { icon: 2 });
                    }
                }

            })
        });

        $("#tab-9,#tab-10").delegate(".AddCoin", "click", function () {
            var coin = $(this).attr("data-code");
            var type = $(this).attr("data-type");
            $.ajax({
                url: "/admincp/coin/AddOrgCoin",
                type: "post",
                data: {
                    coinCode: coinCode,
                    itemCode: coin,
                    rtype: 3
                },
                success: function (result) {
                    layer.msg(result.message);
                    if (1 == type) {
                        tablePerson.reload({
                            where: { //设定异步数据接口的额外参数，任意设
                                coinCode: coinCode,
                                itype: 1,
                                rtype: 3
                            }
                            , page: {
                                curr: 1 //重新从第 1 页开始
                            }
                        });
                    }
                    else {
                        tableIns.reload({
                            where: { //设定异步数据接口的额外参数，任意设
                                coinCode: coinCode,
                                itype: 2,
                                rtype: 3
                            }
                            , page: {
                                curr: 1 //重新从第 1 页开始
                            }
                        });
                    }
                }
            })
        })

        $(".nav-tabs li a").click(function () {
            var tabId = $(this).parent().attr("id");
            if ("tab10" == tabId || "tab9" == tabId) {
                $(".searchValue").val("");
            }
        });
        $.ajax({
            url: "/admincp/coin/Getteaminfosave",
            data: { coincode: coinCode },
            success: function (data) {
                if (data) {
                    $("#tab-10 textarea[name='orgaddress']").val(data.orgaddress);
                    $("#tab-10 input[name='orgimg']").val(data.orgimg);
                    $("#tab-10 #imglist img").prop("src", data.orgimg);
                    $("#tab-10 textarea[name='foundermore']").val(data.foundermore);
                }
            }
        });
    });

    if (coinCode == null || coinCode == "") {
        coinCode = new Date().getTime();
    }
    var imgCodeupload = coinCode;
    //初始化上传控件
    $(".upload-imgsmall").InitUploader({ filesize: "10240", sendurl: "/AdminCP/Upload/DoWebuploaderLogo?type=1", swf: "/js/webuploader/uploader.swf", filetypes: "png", name: imgCodeupload + "_36.png", judgesize: true });
    $(".upload-imgmid").InitUploader({ filesize: "10240", sendurl: "/AdminCP/Upload/DoWebuploaderLogo?type=1", swf: "/js/webuploader/uploader.swf", filetypes: "png", name: imgCodeupload + "_72.png", judgesize: true });
    $(".upload-imgbig").InitUploader({ filesize: "10240", sendurl: "/AdminCP/Upload/DoWebuploaderLogo?type=1", swf: "/js/webuploader/uploader.swf", filetypes: "png", name: imgCodeupload + "_200.png", judgesize: true });
    $(".upload-imgwebp-small").InitUploader({ filesize: "10240", sendurl: "/AdminCP/Upload/DoWebuploaderLogo?type=1", swf: "/js/webuploader/uploader.swf", filetypes: "webp", name: imgCodeupload + "_36.webp", judgesize: true });
    $(".upload-imgwebp").InitUploader({ filesize: "10240", sendurl: "/AdminCP/Upload/DoWebuploaderLogo?type=1", swf: "/js/webuploader/uploader.swf", filetypes: "webp", name: imgCodeupload + "_72.webp", judgesize: true });
    $(".upload-imgOnekey").InitUploader({ filesize: "10240", sendurl: "/AdminCP/Upload/ConvertImgFormatAndSize?type=1", swf: "/js/webuploader/uploader.swf", filetypes: "png,webp,jpg,jpeg", name: imgCodeupload, IsConvert: true });
    DoPost("addForm");

    if (coinCode == "" || coinCode == null) {
        $("#tab2").css("display", "none");
        $("#tab3").css("display", "none");
        $("#tab7").css("display", "none");
        $("#tab8").css("display", "none");
        $("#tab9").css("display", "none");
    }

    var CoinStatusValuenum = $("#CoinStatusValue").val();
    var CoinStatusValuenumbers = $("#CoinStatus").find("option");
    for (var j = 1; j < CoinStatusValuenumbers.length; j++) {
        if ($(CoinStatusValuenumbers[j]).val() == CoinStatusValuenum) {
            $(CoinStatusValuenumbers[j]).attr("selected", "selected");
        }
    };

    var num = $("#coinTypeValue").val();
    var numbers = $("#CoinType").find("option");
    for (var j = 1; j < numbers.length; j++) {
        if ($(numbers[j]).val() == num) {
            $(numbers[j]).attr("selected", "selected");
        }
    }
    //阿拉伯数字转中文大写
    var MaxSupplyCH = $("#MaxSupply").val();
    var MaxSupplyCHZNumb = Arabia_To_SimplifiedChinese(MaxSupplyCH);
    $("#MaxSupplyCHZNumb").html(MaxSupplyCHZNumb);




    var TotalSupplyCH = $("#TotalSupply").val();
    var TotalSupplyCHZNumb = Arabia_To_SimplifiedChinese(TotalSupplyCH);
    $("#TotalSupplyCHZNumb").html(TotalSupplyCHZNumb);

    var CheckOk = [];

    var platform = $("#TokenPlatFormValue").val();


    if (coinCode != null || coinCode != "") {
        //获取ico数据
        $.ajax({
            type: "get",
            url: "/admincp/coin/Ico?id=" + coinCode,
            success: function (model) {
                $("#IcoId").attr("data-id", coinCode);
                $("#Status").val(model.status == null ? "" : model.status);
                $("#Token_PlatForm").val(model.token_PlatForm == null ? "" : model.token_PlatForm);
                $("#Public_Portfolio").val(model.public_Portfolio == null ? "" : model.public_Portfolio);
                $("#Token_Percentage_For_Investors").val(model.token_Percentage_For_Investors == null ? "" : model.token_Percentage_For_Investors);
                $("#Total_Tokens_Supply").val(model.total_Tokens_Supply == null ? "" : model.total_Tokens_Supply);
                $("#Token_Reserve_Split").val(model.token_Reserve_Split == null ? "" : model.token_Reserve_Split);
                $("#End_Date").val(model.end_Date == "0001-01-01T00:00:00" ? "1990-01-01" : model.end_Date.split("T")[0]);
                $("#Start_Date").val(model.start_Date == "0001-01-01T00:00:00" ? "1990-01-01 " : model.start_Date.split("T")[0]);
                $("#Start_Price").val(model.start_Price == null ? "" : getFullNum(model.start_Price));
                $("#StartPriceCurrency").val(model.startPriceCurrency == null ? "" : model.startPriceCurrency);
                $("#Payment_Method").val(model.payment_Method == null ? "" : model.payment_Method);
                $("#Funding_Target").val(model.funding_Target == null ? "" : model.funding_Target);
                $("#Funding_Cap").val(model.funding_Cap == null ? "" : model.funding_Cap);
                $("#Average_Price").val(model.average_Price == null ? "" : getFullNum(model.average_Price));
                $("#average_price_status").val(model.average_Price_Status == null ? "" : model.average_Price_Status);
                $("#Average_Price_Cny").val(model.average_Price_Cny == null ? "" : model.average_Price_Cny);
                $("#Funds_Raised_List").val(model.funds_Raised_List == null ? "" : model.funds_Raised_List);
                $("#Funds_Raised_Usd").val(model.funds_Raised_Usd == null ? "" : model.funds_Raised_Usd);
                $("#Funds_Raised_Cny").val(model.funds_Raised_Cny == null ? "" : model.funds_Raised_Cny);
                $("#Features").val(model.features == null ? "" : model.features);
                $("#Security_Audit").val(model.security_Audit == null ? "" : model.security_Audit);
                $("#Legal_Form").val(model.legal_Form == null ? "" : model.legal_Form);
                $("#Jurisdiction").val(model.jurisdiction == null ? "" : model.jurisdiction);
                $("#Legal_Advisers").val(model.legal_Advisers == null ? "" : model.legal_Advisers);
                $("#Sale_Website").val(model.sale_Website == null ? "" : model.sale_Website);
                setAverage_PriceStatus();
            }
        })
        $.ajax({
            type: "get",
            url: "/admincp/coin/CoinCrawIndex?id=" + coinCode,
            success: function (model) {
                if (model) {
                    $("#CrawLink").val(model.link == null ? "" : model.link);
                }
            }
        })
        //获取社交信息 CoinCode
        $.ajax({
            type: "get",
            url: "/admincp/coin/CoinIndex?id=" + coinCode,
            success: function (model) {
                $("#coinIndexId").attr("data-id", coinCode);

                $("#CodeLink").val(model.codeLink == null ? "" : model.codeLink);
                $("#weiboLink").val(model.weiboLink == null ? "" : model.weiboLink);
                $("#twitterLink").val(model.twitterLink == null ? "" : model.twitterLink);
                $("#TelegramLink").val(model.telegramLink == null ? "" : model.telegramLink);
                $("#RedditLink").val(model.redditLink == null ? "" : model.redditLink);
                $("#FacebookLink").val(model.faceBookLink == null ? "" : model.faceBookLink);


                $("#GitHub_Watch").val(model.gitHub_Watch == null ? "" : model.gitHub_Watch);
                $("#GitHub_Star").val(model.gitHub_Star == null ? "" : model.gitHub_Star);
                $("#GitHub_Fork").val(model.gitHub_Fork == null ? "" : model.gitHub_Fork);
                $("#GitHub_Contributors").val(model.gitHub_Contributors == null ? "" : model.gitHub_Contributors);
                $("#GitHub_Commits").val(model.gitHub_Commits == null ? "" : model.gitHub_Commits);
                $("#GitHub_Commits_RecentMonth").val(model.gitHub_Commits_RecentMonth == null ? "" : model.gitHub_Commits_RecentMonth);
                $("#GitHub_Issues").val(model.gitHub_Issues == null ? "" : model.gitHub_Issues);
                $("#GitHub_LastUpdate").val(model.gitHub_LastUpdate == "0001/1/1 0:00:00" ? "1990-01-01 " : model.gitHub_LastUpdate.split("T")[0]);
                $("#Reddit_Focus").val(model.reddit_Focus == null ? "" : model.reddit_Focus);
                $("#Reddit_Increase_LastWeek").val(model.reddit_Increase_LastWeek == null ? "" : model.reddit_Increase_LastWeek);
                $("#Reddit_LastUpdate").val(model.reddit_LastUpdate == "0001/1/1 0:00:00" ? "1990-01-01 " : model.reddit_LastUpdate.split("T")[0]);
                $("#Twitter_Focus").val(model.twitter_Focus == null ? "" : model.twitter_Focus);
                $("#Twitter_Increase_LastWeek").val(model.twitter_Increase_LastWeek == null ? "" : model.twitter_Increase_LastWeek);
                $("#Twitter_LastUpdate").val(model.twitter_LastUpdate == "0001/1/1 0:00:00" ? "1990-01-01 " : model.twitter_LastUpdate.split("T")[0]);
                $("#Facebook_Focus").val(model.facebook_Focus == null ? "" : model.facebook_Focus);
                $("#Facebook_Increase_LastWeek").val(model.facebook_Increase_LastWeek == null ? "" : model.facebook_Increase_LastWeek);
                $("#Facebook_LastUpdate").val(model.facebook_LastUpdate == "0001/1/1 0:00:00" ? "1990-01-01 " : model.facebook_LastUpdate.split("T")[0]);


            }
        })
    }
    //获取代币平台
    $.ajax({
        type: "get",
        url: "/admincp/coin/GetPlatformAsync",
        success: function (model) {
            var optionS = "";
            if (model == null) {
                optionS = "<option value=''>请选择</option>";
            } else {
                if (platform == "") {
                    optionS = "<option value='' selected='selected'>请选择</option>";
                } else {
                    optionS = "<option value=''>请选择</option>";
                }
                model.forEach((a) => {
                    if (platform == a.dictCode) {
                        optionS += "<option value='" + a.dictCode + "'  selected='selected'>" + a.dictName + "</option>";
                    } else {
                        optionS += "<option value='" + a.dictCode + "'>" + a.dictName + "</option>";
                    }
                })
            }
            $("#TokenPlatForm,#TokenPlatForm_audit").append(optionS);
        }
    })

    $.ajax({
        url: "/admincp/coin/FindTagByCoinCode?coinCode=" + coinCode,
        success: function (r) {
            if (r != null) {
                r.forEach((val) => {
                    CheckOk.push(val.tagCode);
                })
            }

            getAllTagHtml(CheckOk)
        }
    });

    $.ajax({
        url: "/admincp/coin/FindCoinTypeTag?coinCode=" + coinCode,
        success: function (r) {
            if (r != null) {
                //console.log("r:", JSON.stringify(r));
                $("#CoinTagName").val(r.tagName);
                $("#TagUrl").val(r.tagUrl);
                $("#NewsID").val(r.tagID);
            }

        }
    });



    //获取所有语言
    $.ajax({
        url: "/admincp/coin/GetAllLang",
        success: function (r) {
            var divString = ''
            r.forEach((val) => {

                let string = starLabel + val.languageCode + '"> ' + val.nativeName + endLabel
                divString = divString + string
            })
            $("#LangSelect,#SeoLangSelect,#crowdfunding_langselect,#lockpositionreleaseinfo_langselect,#forking_langselect,#mainpoint_langselect").append(divString);
        }
    });

        //document.getElementById("btnadd").style.display = "none";
        var id = "zh-CN";
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


        //crowdfunding_from
        var id = "zh-CN";
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
        //return false;
        $.ajax({
            url: "/admincp/coin/FindLang?langCode=" + "zh-CN" + "&coinCode=" + coinCode,
            success: function (r) {
                $("#LockPositionReleaseInfo").val(r == null ? "" : r.lockPositionReleaseInfo);
            }
        });

        //forking_from
        $.ajax({
            url: "/admincp/coin/GetForkingByLang?langCode=" + "zh-CN" + "&coinCode=" + coinCode,
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

        $("#divTagAdd").css("display", "");

        $.ajax({
            url: "/admincp/coin/FindLang?langCode=" + "zh-CN" + "&coinCode=" + coinCode,
            success: function (r) {
                $("#savelang").attr("data-id", "zh-CN");
                $("#NativeName").val(r == null ? "" : r.nativeName);

                $("#SceneTag").val(r == null ? "" : r.sceneTag);

                CKTeamDesc.setData(r == null ? "" : r.teamDesc);
                CKDescription.setData(r == null ? "" : r.description);

                if (r != null && r != undefined && r.hasOwnProperty("sceneTagList")){
                    var tags = r.sceneTagList;
                    if (tags.length > 0) {
                        var html = '';
                        for (var i = 0; i < tags.length; i++) {
                            html = html + getTagHtml(tags[i].keywords, tags[i].link, tags[i].jumpId);
                        }
                        $("#divTag").append(html);
                    } else {
                        $("#divTag").append(getTagHtml('', '', ''));
                    }
                }
            }
        });
 
    getWallet();


})


