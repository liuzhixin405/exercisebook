
(function () {
    let tools = {
        limitFileSize: function (file, limitSize) {
            var arr = ["KB", "MB", "GB"]
            var limit = limitSize.toUpperCase();
            var limitNum = 0;
            for (var i = 0; i < arr.length; i++) {
                var leval = limit.indexOf(arr[i]);
                if (leval > -1) {
                    limitNum = parseInt(limit.substr(0, leval)) * Math.pow(1024, (i + 1))
                    break
                }
            }
            if (file.size > limitNum) {
                return false
            }
            return true
        }
    };
    function init(panel, imgsrc) {
        function imgChange(obj) {
            //console.log(obj);
            var image = obj.files[0]; //获取文件域中选中的图片
            if (!tools.limitFileSize(image, "10MB")) {
                layer.msg("图片大小不能超过10MB");
                return;
            }
            var reader = new FileReader(); //实例化文件读取对象
            reader.readAsDataURL(image); //将文件读取为 DataURL,也就是base64编码
            reader.onload = function (ev) { //文件读取成功完成时触发
                var dataURL = ev.target.result; //获得文件读取成功后的DataURL,也就是base64编码
                //$("#imglist img").prop("src", dataURL); //将DataURL码赋值给img标签
                Addimgview(dataURL);
                bindImgviewlistClick();
                // console.log(dataURL);
            }
            //console.log(image);
        }
        function bindImgviewlistClick() {
            jqimgpanel.find(".dellink").unbind("click").click(function () {
                $(this).parent().parent().remove();
                return false;
            });
        }
        function selectimgfile() {
            //if ($("#imgviewlist img").length >= 3) {
            //    layer.msg("最多添加3张图片");
            //    return;
            //}
            jqifileinput.click()
        }
        function Addimgview(url) {
            //var html = $("#imgviewtpl").html();
            //console.log(html);
            //html = html.replace(/[{]url[}]/g, encodeURI(url));
            //$("#imgviewlist").append(html);
            if (url) {
                jqimgobj.prop("src", url);
                jqiimgsrcinput.val(url);
            } else {
                jqimgobj.prop("src", "/images/default.JPG?1");
                jqiimgsrcinput.val("");
            }
        }
        let jqimgpanel;
        let jqimgobj;
        let jqifileinput;
        let jqueryfilename;
        let jqiimgsrcinput;
        jqimgpanel = panel;
        jqimgpanel.empty();
        jqueryfilename = panel.attr("name");
        jqimgobj = $("<img src='' style='max-width:100px;max-height:100px;min-width:50px;min-height:50px'/>");
        jqimgobj.click(function () { selectimgfile(); });
        jqifileinput = $('<input type="file" style="opacity: 0">');
        jqiimgsrcinput = $('<input type="hidden" name="' + jqueryfilename + '">');
        jqifileinput.change(function () {
            imgChange(this);
        });
        Addimgview(imgsrc);
        jqimgpanel.append(jqimgobj);
        jqimgpanel.append(jqifileinput);
        jqimgpanel.append(jqiimgsrcinput);
    }
    let fxhimgupload = {
        init: init//function (panel, imgsrc) {
        //     init(panel, imgsrc);
        // }
    };
    window.fxhimgupload = fxhimgupload;

}());