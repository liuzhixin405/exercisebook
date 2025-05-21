//顶部打开dialog
function showDialog(id, title, url, width, height) {
    top.showDialog(id, title, url, width, height)
}
//删除数据
function doDel(url, id) {
    top.doDel(url, id);
}
//刷新网页
function refresh() {

    window.location = window.location;
}
//
dName = "";
function BrowseServer(id) {
    dName = id;
    // You can use the "CKFinder" class to render CKFinder in a page:
    var finder = new CKFinder();
    finder.basePath = '/static/editor/ckeditor/ckfinder_net/'; // The path for the installation of CKFinder (default = "/ckfinder/").
    finder.selectActionFunction = SetFileField;
    finder.popup();
}
function SetFileField(fileUrl) {
    document.getElementById(dName).value = decodeURI(fileUrl);
}
//显示隐藏新增行
function showAddRow() {
    $("#addnew").toggleClass('hidden');
}
//显示编辑
function showEdit(id) {
    $("#row_" + id + " .rt-editpanel").hide();
    $("#row_" + id + " .rt-savepanel").removeClass('hidden');
}
//取消编辑
function cancelEdit(id) {
    $("#row_" + id + " .rt-editpanel").show();
    $("#row_" + id + " .rt-savepanel").addClass('hidden');
}

//后台异步审核
function doAuditing(url, id, status) {
    if (status == "-1" || status == "1") {
        var msg = '确认不通过？';
        if (status == "1") {
            msg = '确认通过？';
        }
        layer.confirm(msg, {
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
                data: {
                    "ReviewStatus": status,
                    "id": id
                },
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
}