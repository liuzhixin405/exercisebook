function oncePush(newsid,type,coincode) {
    $.post("/admincp/MediaNewsPush/DirectlyPush?id=" + newsid + "&type=" + type + "&coincode=" + coincode, function (ret) {
        layer.msg(ret.message, {
            icon: 1,
            time: 3000 //3��رգ���������ã�Ĭ����3�룩
        }, function () {
            var index = parent.layer.getFrameIndex('zxpush');
            parent.layer.close(index);
            var current = top.$(".page-tabs-content .active").attr("data-id");
            var target = top.$('.J_iframe[data-id="' + current + '"]');
            target[0].contentWindow.document.getElementsByName('refresh')[0].click();
        });
    });
}