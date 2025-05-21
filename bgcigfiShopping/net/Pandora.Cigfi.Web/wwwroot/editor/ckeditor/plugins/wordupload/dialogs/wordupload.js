/*
 Copyright (c) 2003-2018, CKSource - Frederico Knabben. All rights reserved.
 For licensing, see LICENSE.md or https://ckeditor.com/legal/ckeditor-oss-license
*/
CKEDITOR.dialog.add("wordupload",
    function (a) {
        return {
            title: "word文件>>资讯正文",
            minWidth: 350,
            minHeight: 200,
            onOk: function () {
                var index = layer.load(2); 
                $.ajax({
                    url: "/AdminCP/Upload/ConvertToHtml",
                    type: 'POST',
                    cache: false,
                    data: new FormData($("#" + a.name)[0]),
                    processData: false,
                    contentType: false,
                    dataType: "json",
                    success: function (data) {
                        layer.close(index);
                        if (data.status == "success")
                        {
                            a.setData(data.message);
                        }
                        else
                        {
                            layer.msg(data.message, {icon:2, time:2000});
                        }
                    }
                });           

            },
            contents: [{
                id: "wordContent",
                elements: [
                    {
                        type: "html",
                        html: '<form id="' + a.name + '" enctype="multipart/form-data"><input type="file" name="wordFile"  accept="application/msword|application/vnd.ms-works"   style="border: none;"></form>'
                    }
                 ]
            }]
        }
    });