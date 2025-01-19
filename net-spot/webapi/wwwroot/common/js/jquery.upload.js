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
		
		// form������������
		var formHtml = '<input type="file" name="' + opts.fileName + '" onchange="onChooseFile(this)">';
		for (key in opts.params) {
			formHtml += '<input type="hidden" name="' + key + '" value="' + opts.params[key] + '">';
		}
		form.append(formHtml);

		iframe.appendTo("body");
		form.appendTo("body");
		
		form.submit(opts.onSubmit);
		
		// iframe ���ύ���֮��
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
		
		// �ļ���
		var fileInput = $('input[type=file][name=' + opts.fileName + ']', form);
		fileInput.click();
	};
})(jQuery);

// ѡ���ļ�, �ύ��(��ʼ�ϴ�)
var onChooseFile = function(fileInputDOM) {
	var form = $(fileInputDOM).parent();
	form.submit();
};


function upload(fun) {
	$.upload({
    	url: basePath + '/file/upload.html', // �ϴ���ַ
    	fileName: 'file', // �ļ�������
    	// ���������� params: {name: 'pxblog'},
    	dataType: 'text', // �ϴ���ɺ�, ����json, text
    	onSend: function() {
        	return true; // �ϴ�֮ǰ�ص�,return true��ʾ�ɼ����ϴ�
     	},
     	// �ϴ�֮��ص�
    	onComplate: function(data) {
        	if (null != fun && 'undefined' != typeof(fun)) {
        		if ("error" == data) {
					alert('�Բ����ϴ����������ԣ�');
				} else {
					fun(data);
				}
        	}
    	}
    });
}

function uploadFile2(fun) {
	$.upload({
    	url: basePath + '/file/uploadFile.html', // �ϴ���ַ
    	fileName: "file", // �ļ�������
    	// ���������� params: {name: 'pxblog'},
    	dataType: 'text', // �ϴ���ɺ�, ����json, text
    	onSend: function() {
        	return true; // �ϴ�֮ǰ�ص�,return true��ʾ�ɼ����ϴ�
     	},
     	// �ϴ�֮��ص�
    	onComplate: function(data) {
        	if (null != fun && 'undefined' != typeof(fun)) {
        		if ("error" == data) {
					alert('�Բ����ϴ��ļ����������ԣ�');
				} else {
					fun(data);
				}
        	}
    	}
    });
}


/**
 * ��ˮӡ��ͼƬ�ϴ�
 * @param fun
 */
function uploadImageWatermark(fun) {
	$.upload({
    	url: basePath + '/file/uploadWatermarkImage.html', // �ϴ���ַ
    	fileName: "file", // �ļ�������
    	// ���������� params: {name: 'pxblog'},
    	dataType: 'text', // �ϴ���ɺ�, ����json, text
    	onSend: function() {
        	return true; // �ϴ�֮ǰ�ص�,return true��ʾ�ɼ����ϴ�
     	},
     	// �ϴ�֮��ص�
    	onComplate: function(data) {
        	if (null != fun && 'undefined' != typeof(fun)) {
        		if (data.indexOf("error:") == 0) {
					alert('�Բ����ϴ��ļ����������ԣ�ԭ��' + data.substr(6));
				} else {
					fun(data);
				}
        	}
    	}
    });
}

/**
 * ��ˮӡ��ͼƬ�ϴ����жϵ�¼
 * @param fun
 */
function uploadLoginImageWatermark(fun) {
	$.upload({
    	url: "http://127.0.0.1:8080/otc-platform/manager/otc-platform/admin/" + '/file/uploadLoginWatermarkImage.otc', // �ϴ���ַ
    	fileName: "file", // �ļ�������
    	// ���������� params: {name: 'pxblog'},
    	dataType: 'text', // �ϴ���ɺ�, ����json, text
    	onSend: function() {
        	return true; // �ϴ�֮ǰ�ص�,return true��ʾ�ɼ����ϴ�
     	},
     	// �ϴ�֮��ص�
    	onComplate: function(data) {
        	if (null != fun && 'undefined' != typeof(fun)) {
        		if (data.indexOf("error:") == 0) {
					alert('�Բ����ϴ��ļ����������ԣ�ԭ��' + data.substr(6));
				} else {
					fun(data);
				}
        	}
    	}
    });
}