
function Uploader(file, listener, tag) {
	lrz(file, {
		width : 1200,
		height : 1200
	}).then(function(rst) {
		rst.listener = listener;
		uploadImage(rst, tag);
        }).catch(function(reason) {
            listener.uploadFailed();
        });
}

function uploadImage(rst, tag) {
	var uploader = this;
	this.upload = function() {
		var formData = new FormData();
		formData.append('file', rst.base64);
		$.ajax({
			url : basePath + 'file/uploadWorkOrder.do?tag=' + tag,
			type : 'POST',
			xhr : function() {
				var myXhr = $.ajaxSettings.xhr();
				if (myXhr.upload) {
					myXhr.upload.addEventListener('progress', uploader.onProgress, false);
				}
				return myXhr;
			},
			beforeSend : uploader.beforeSend,
			success : uploader.uploadComplete,
			error : uploader.errorHandler,
			data : formData,
			cache : false,
			contentType : false,
			processData : false
		});
	};

	this.onProgress = function(e) {
		if (e.lengthComputable) {
			var val = e.loaded / e.total * 100;
			val = val.toFixed(2);
			rst.listener.onProgress(val);
		}
	}

	this.beforeSend = function(e) {}

	this.uploadComplete = function(data) {
		rst.listener.uploadComplete(data);
	}

	this.errorHandler = function(e) {
		rst.listener.uploadFailed();
	}

	this.upload();
}
