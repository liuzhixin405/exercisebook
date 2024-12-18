/**
 * jQuery upload v1.2
 * http://www.ponxu.com
 *
 * @author xwz
 */
(function($) {
	var noop = function(){ return true; };
	var frameCount = 0;
	
	$.uploadDefault = {
		url: '',
		fileName: 'filedata',
		dataType: 'json',
		params: {},
		onSend: noop,
		onSubmit: noop,
		onComplate: noop
	};

	$.upload = function(options) {
		var opts = $.extend(jQuery.uploadDefault, options);
		if (opts.url == '') {
			return;
		}
		
		var canSend = opts.onSend();
		if (!canSend) {
			return;
		}
		
		var frameName = 'upload_frame_' + (frameCount++);
		var iframe = $('<iframe style="position:absolute;top:-9999px" />').attr('name', frameName);
		var form = $('<form method="post" style="display:none;" enctype="multipart/form-data" />').attr('name', 'form_' + frameName);
		form.attr("target", frameName).attr('action', opts.url);
		
		// form中增加数据域
		var formHtml = '<input type="file" name="' + opts.fileName + '" onchange="onChooseFile(this)">';
		for (key in opts.params) {
			formHtml += '<input type="hidden" name="' + key + '" value="' + opts.params[key] + '">';
		}
		form.append(formHtml);

		iframe.appendTo("body");
		form.appendTo("body");
		
		form.submit(opts.onSubmit);
		
		// iframe 在提交完成之后
		iframe.load(function() {
			var contents = $(this).contents().get(0);
			var data = $(contents).find('body').text();
			if ('json' == opts.dataType) {
				data = window.eval('(' + data + ')');
			}
			if ($.trim(data).length > 0) {
				opts.onComplate(data);
				iframe.remove();
				form.remove();
			}
		});
		
		// 文件框
		var fileInput = $('input[type=file][name=' + opts.fileName + ']', form);
		fileInput.click();
	};
})(jQuery);

// 选中文件, 提交表单(开始上传)
var onChooseFile = function(fileInputDOM) {
	var form = $(fileInputDOM).parent();
	form.submit();
};


function upload(fun) {
	$.upload({
    	url: basePath + '/file/upload.html', // 上传地址
    	fileName: 'file', // 文件域名字
    	// 其他表单数据 params: {name: 'pxblog'},
    	dataType: 'text', // 上传完成后, 返回json, text
    	onSend: function() {
        	return true; // 上传之前回调,return true表示可继续上传
     	},
     	// 上传之后回调
    	onComplate: function(data) {
        	if (null != fun && 'undefined' != typeof(fun)) {
        		if ("error" == data) {
					alert('对不起上传错误，请重试！');
				} else {
					fun(data);
				}
        	}
    	}
    });
}

function uploadFile2(fun) {
	$.upload({
    	url: basePath + '/file/uploadFile.html', // 上传地址
    	fileName: "file", // 文件域名字
    	// 其他表单数据 params: {name: 'pxblog'},
    	dataType: 'text', // 上传完成后, 返回json, text
    	onSend: function() {
        	return true; // 上传之前回调,return true表示可继续上传
     	},
     	// 上传之后回调
    	onComplate: function(data) {
        	if (null != fun && 'undefined' != typeof(fun)) {
        		if ("error" == data) {
					alert('对不起上传文件错误，请重试！');
				} else {
					fun(data);
				}
        	}
    	}
    });
}


/**
 * 带水印的图片上传
 * @param fun
 */
function uploadImageWatermark(fun) {
	$.upload({
    	url: basePath + '/file/uploadWatermarkImage.html', // 上传地址
    	fileName: "file", // 文件域名字
    	// 其他表单数据 params: {name: 'pxblog'},
    	dataType: 'text', // 上传完成后, 返回json, text
    	onSend: function() {
        	return true; // 上传之前回调,return true表示可继续上传
     	},
     	// 上传之后回调
    	onComplate: function(data) {
        	if (null != fun && 'undefined' != typeof(fun)) {
        		if (data.indexOf("error:") == 0) {
					alert('对不起上传文件错误，请重试！原因：' + data.substr(6));
				} else {
					fun(data);
				}
        	}
    	}
    });
}

/**
 * 带水印的图片上传，判断登录
 * @param fun
 */
function uploadLoginImageWatermark(fun) {
	$.upload({
    	url: "http://127.0.0.1:8080/otc-platform/manager/otc-platform/admin/" + '/file/uploadLoginWatermarkImage.otc', // 上传地址
    	fileName: "file", // 文件域名字
    	// 其他表单数据 params: {name: 'pxblog'},
    	dataType: 'text', // 上传完成后, 返回json, text
    	onSend: function() {
        	return true; // 上传之前回调,return true表示可继续上传
     	},
     	// 上传之后回调
    	onComplate: function(data) {
        	if (null != fun && 'undefined' != typeof(fun)) {
        		if (data.indexOf("error:") == 0) {
					alert('对不起上传文件错误，请重试！原因：' + data.substr(6));
				} else {
					fun(data);
				}
        	}
    	}
    });
}