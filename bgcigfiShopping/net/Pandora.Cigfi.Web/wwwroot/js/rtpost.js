//所有异步提交 formId 不需要加上#  last:2015-11-12  by Hogen Wang
//依赖jquery jquery.form jquery.validate jquery.metadata
function DoPost(formId, rules, messages) {
    //提交地址
    var $myform = $("#" + formId);
    var url = $myform.attr("action");
    var btn;
    if ($myform.find("input[type='submit']").length > 0) {
        btn = $myform.find("input[type='submit']");
    } else if ($myform.find("button[type='submit']").length > 0) {
        btn = $myform.find("button[type='submit']");
    }

    var v = $myform.validate({
        rules: rules,
        messages: messages,
        submitHandler: function (form) {
            //loadding效果
            var loading = layer.load(0, {
                shade: [0.2, '#000'] //0.1透明度的背景
            });

            btn.attr("disabled", "disabled");
            //执行loadding 并且ajax提交
            //如果是有CKEditor的话，需要update，否则取不到值
            if (window.CKEDITOR) {
                for (instance in CKEDITOR.instances)
                    CKEDITOR.instances[instance].updateElement();
            }

            var queryString = $myform.formSerialize();
            $.ajax({
                type: "POST",
                url: url,
                data: queryString,
                dataType: "JSON",
                success: function (data) {
                    if (data.status == "success") {
                        let time = 1000;
                        if (data.message) {
                            if (data.message.length > 200) {
                                time = 5000;
                            } else if (data.message.length > 50) {
                                time = 3000;
                            }
                        }
                        layer.msg(data.message, {
                            time: time //2秒关闭（如果不配置，默认是3秒）
                        }, function () {
                            if (data.returnUrl) {
                                if (data.returnUrl == "close") {

                                    var dindex = parent.layer.getFrameIndex(window.name);
                                    if (dindex) {
                                        parent.layer.close(dindex);
                                        var current = top.$(".page-tabs-content .active").attr("data-id");
                                        var target = top.$('.J_iframe[data-id="' + current + '"]');
                                        if (target[0].contentWindow.reloadlist) {
                                            target[0].contentWindow.reloadlist();
                                        }
                                        else if (target[0].contentWindow.doSearch) {
                                            target[0].contentWindow.doSearch();
                                        }
                                        else if (target[0].contentWindow.refresh) {
                                            target[0].contentWindow.refresh();
                                        }
                                        else {
                                            top.refreshCurrentTab();
                                        }
                                    } else {
                                        window.location.href = window.location.href;
                                    }
                                }
                                else if (data.returnUrl == "closeandreload") {
                                    var dindex = parent.layer.getFrameIndex(window.name);
                                    if (dindex) {
                                        //top.refreshCurrentTab();
                                        //刷新页面列表 如果有就刷新，没有就刷新整个界面
                                        var current = top.$(".page-tabs-content .active").attr("data-id");
                                        var target = top.$('.J_iframe[data-id="' + current + '"]');
                                        if (target[0].contentWindow.reloadlist) {
                                            target[0].contentWindow.reloadlist();
                                        }
                                        else if (target[0].contentWindow.doSearch) {
                                            target[0].contentWindow.doSearch();
                                        }
                                        else if (target[0].contentWindow.refresh) {
                                            target[0].contentWindow.refresh();
                                        }
                                        else {
                                            top.refreshCurrentTab();
                                        }
                                        parent.layer.close(dindex);
                                    }
                                }
                                else if (data.returnUrl == "closeandrefresh") {
                                    var dindex = parent.layer.getFrameIndex(window.name);
                                    if (dindex) {
                                        parent.layer.close(dindex);
                                    }
                                    window.location.href = window.location.href;

                                    var current = top.$(".page-tabs-content .active").attr("data-id");
                                    var target = top.$('.J_iframe[data-id="' + current + '"]');
                                    target[0].contentWindow.document.getElementsByName('refresh')[0].click();
                                }
                                else if (data.returnUrl == "repeatsubmitandcallback") {
                                    DoPostCallBack();
                                    btn.removeAttr("disabled");
                                } else if (data.returnUrl == "callback") {
                                    DoPostCallBack();
                                }
                                else {
                                    window.location.href = data.returnUrl;
                                }
                            }
                            else {
                                window.location.href = window.location.href;

                                var current = top.$(".page-tabs-content .active").attr("data-id");
                                var target = top.$('.J_iframe[data-id="' + current + '"]');
                                target[0].contentWindow.document.getElementsByName('refresh')[0].click();
                            }

                        });
                    } else {
                        layer.msg(data.message ? data.message : "发生异常了，请联系管理员", { icon: 2, time: 5000 }, function () {
                            /*var dindex = parent.layer.getFrameIndex(window.name);
                            if (dindex) {
                                parent.layer.close(dindex);
                            }
                            var current = top.$(".page-tabs-content .active").attr("data-id");
                            var target = top.$('.J_iframe[data-id="' + current + '"]');
                            target[0].contentWindow.document.getElementsByName('refresh')[0].click();*/
                        });
                        btn.removeAttr("disabled");
                    }
                },
                complete: function () {
                    layer.close(loading);
                },
                error: function () {
                    //layer.close(loading); 
                    layer.alert('执行错误，请联系管理员！', { icon: 2 }, function () {
                        return;
                        var dindex = parent.layer.getFrameIndex(window.name);
                        if (dindex) {
                            parent.layer.close(dindex);
                        }
                        var current = top.$(".page-tabs-content .active").attr("data-id");
                        var target = top.$('.J_iframe[data-id="' + current + '"]');
                        target[0].contentWindow.document.getElementsByName('refresh')[0].click();
                    });
                    btn.removeAttr("disabled");
                }
            });
        }
    });
    return false;
}

//后台异步删除
function doDel(url, id) {
    layer.confirm('确认要删除当前记录？此操作不可恢复！', {
        icon: 3,
        title: '系统提示',
        btn: ['确定', '取消 '] //按钮
    }, function () {
        var loading = layer.load(0, {
            shade: [0.2, '#000'] //0.1透明度的背景
        });
        $.ajax({
            type: "POST",
            url: url,
            data: 'id=' + id,
            dataType: "JSON",
            success: function (data) {
                if (data.status == "success") {
                    layer.close(loading);
                    layer.msg(data.message ? data.message : "操作成功", {
                        time: 1000 //2秒关闭（如果不配置，默认是3秒）
                    }, function () {
                        //if ($("#row_" + id).length > 0) {
                        //    $("#row_" + id).fadeOut(300).remove();
                        //} else {
                        //    //window.location.href = window.location.href;
                        //    var dindex = parent.layer.getFrameIndex(window.name);
                        //    if (dindex) {
                        //        layer.close(dindex);
                        //        return;
                        //    }
                        //    if (top.toplayerindex) {
                        //        top.refreshCurrentTab();//刷新本tab

                        //    } else {
                        //        var current = top.$(".page-tabs-content .active").attr("data-id");
                        //        var target = top.$('.J_iframe[data-id="' + current + '"]');
                        //        if (target[0].contentWindow.reloadlist) {
                        //            target[0].contentWindow.reloadlist();
                        //        }
                        //        else if (target[0].contentWindow.doSearch) {
                        //            target[0].contentWindow.doSearch();
                        //        }
                        //    }
                        //}
                        var current = top.$(".page-tabs-content .active").attr("data-id");
                        var target = top.$('.J_iframe[data-id="' + current + '"]');
                        target[0].contentWindow.document.getElementsByName('refresh')[0].click();
                    });
                } else {
                    layer.close(loading);
                    layer.alert(data.message, { icon: 2 });

                }

            },
            error: function () {
                layer.close(loading);
                layer.alert('执行错误，请联系管理员！', { icon: 2 });
            }
        });
    }, function () {
    });
}

//后台异步删除
function doDelJson(url, json) {
    //   alert(JSON.stringify(json));
    layer.confirm('确认要删除当前记录？此操作不可恢复！', {
        icon: 3,
        title: '系统提示',
        btn: ['确定', '取消 '] //按钮
    }, function () {
        var loading = layer.load(0, {
            shade: [0.2, '#000'] //0.1透明度的背景
        });
        $.ajax({
            type: "POST",
            url: url,
            data: json,
            dataType: "JSON",
            success: function (data) {
                if (data.status == "success") {
                    layer.close(loading);
                    layer.msg(data.message, {
                        time: 1000 //2秒关闭（如果不配置，默认是3秒）
                    }, function () {
                        var current = top.$(".page-tabs-content .active").attr("data-id");
                        var target = top.$('.J_iframe[data-id="' + current + '"]');
                        target[0].contentWindow.document.getElementsByName('refresh')[0].click();
                    });
                } else {
                    layer.close(loading);
                    layer.alert(data.message, { icon: 2 });

                }

            },
            error: function () {
                layer.close(loading);
                layer.alert('执行错误，请联系管理员！', { icon: 2 });
            }
        });
    }, function () {
    });
}

//执行添加
function doAdd(formid, url) {
    var strQuery = $("#" + formid + " .addnew").fieldSerialize();

    var loading = layer.load(0, {
        shade: [0.2, '#000'] //0.1透明度的背景
    });
    $.ajax({
        type: "POST",
        url: url,
        data: strQuery,
        dataType: "JSON",
        success: function (data) {
            if (data.status == "success") {
                layer.close(loading);
                layer.msg(data.message, {
                    time: 1000 //2秒关闭（如果不配置，默认是3秒）
                }, function () {
                    window.location.href = window.location.href;
                });
            } else {
                layer.close(loading);
                layer.alert(data.message, { icon: 2 });

            }
        },
        error: function () {
            layer.close(loading);
            layer.alert('执行错误，请联系管理员！', { icon: 2 });
        }
    });
}
//执行异步编辑
function doEdit(formid, url, id) {
    var strQuery = $("#" + formid + " #row_" + id + " .rt-edit").fieldSerialize();
    var loading = layer.load(0, {
        shade: [0.2, '#000'] //0.1透明度的背景
    });
    $.ajax({
        type: "POST",
        url: url,
        data: strQuery,
        dataType: "JSON",
        success: function (data) {
            if (data.status == "success") {
                layer.close(loading);
                layer.msg(data.message, {
                    time: 1000 //2秒关闭（如果不配置，默认是3秒）
                }, function () {
                    window.location.href = window.location.href;
                });
            } else {
                layer.close(loading);
                layer.alert(data.message, { icon: 2 });
            }
        },
        error: function () {
            layer.close(loading);
            layer.alert('执行错误，请联系管理员！', { icon: 2 });
        }
    });
}

//后台异步提示操作
function doAction(url, id, tiptitle) {
    layer.confirm(tiptitle, {
        icon: 3,
        title: '系统提示',
        btn: ['确定', '取消 '] //按钮
    }, function () {
        var loading = layer.load(0, {
            shade: [0.2, '#000'] //0.1透明度的背景
        });
        $.ajax({
            type: "POST",
            url: url,
            data: 'id=' + id,
            dataType: "JSON",
            success: function (data) {
                if (data.status == "success") {
                    layer.close(loading);
                    layer.msg(data.message, {
                        time: 1000 //2秒关闭（如果不配置，默认是3秒）
                    }, function () {
                        var index = parent.layer.getFrameIndex(window.name); //获取窗口索引
                        //window.location.href = window.location.href;
                        if (index) {
                            window.location.href = window.location.href;
                        } else {
                            //判断是否是列表
                            var current = top.$(".page-tabs-content .active").attr("data-id");
                            var target = top.$('.J_iframe[data-id="' + current + '"]');
                            if (target[0].contentWindow.doSearch) {
                                target[0].contentWindow.doSearch();
                            } else {
                                refreshCurrentTab();
                            }
                            //refreshCurrentTab();//刷新本tab
                        }
                    });
                } else {
                    layer.close(loading);
                    layer.alert(data.message, { icon: 2 });

                }
            },
            error: function () {
                layer.close(loading);
                layer.alert('执行错误，请联系管理员！', { icon: 2 });
            }
        });
    }, function () {
    });
}

function DoAdminLogin(formId, rules, messages) {
    //提交地址
    var $myform = $("#" + formId);
    var url = $myform.attr("action");
    var rvtoken = $("input[name='__RequestVerificationToken']").val();
    var btn;
    if ($myform.find("input[type='submit']").length > 0) {
        btn = $myform.find("input[type='submit']");
    } else if ($myform.find("button[type='submit']").length > 0) {
        btn = $myform.find("button[type='submit']");
    }
    var v = $myform.validate({
        rules: rules,
        messages: messages,
        submitHandler: function (form) {
            //loadding效果
            var loading = layer.load(0, {
                shade: [0.2, '#000'] //0.1透明度的背景
            });
            //执行loadding 并且ajax提交

            var mobile = $("#Mobile").val();
            var smsCode = $("#Sms_Code").val();
            var areaCode = $("#AreaCode").val();
            if (mobile == "" || smsCode == "") {
                layer.alert("输入手机号及验证码", { icon: 2 });
                layer.close(loading);
                return false;
            }

            mobile = encMe(mobile, encrypt_key, 1, 0);
            smsCode = encMe(smsCode, encrypt_key, 1, 0);
            var queryString = $myform.formSerialize();

            btn.attr("disabled", "disabled");

            $.ajax({
                type: "POST",
                url: url,
                data: { Sms_Code: smsCode, Mobile: mobile, AreaCode: areaCode, des_key: encrypt_key },
                dataType: "JSON",
                success: function (data) {
                    if (data.status == "success") {
                        layer.close(loading);
                        layer.msg(data.message, {
                            time: 1000 //2秒关闭（如果不配置，默认是3秒）
                        }, function () {
                            window.location.href = data.returnUrl;
                        });
                    } else {
                        layer.close(loading);
                        layer.alert(data.message, { icon: 2 });
                        $("#Sms_Code").val();
                        btn.removeAttr("disabled");
                    }
                },
                error: function () {
                    layer.close(loading);
                    layer.alert('执行错误，请联系管理员！', { icon: 2 });
                    btn.removeAttr("disabled");
                }
            });
        }
    });
    return false;
}