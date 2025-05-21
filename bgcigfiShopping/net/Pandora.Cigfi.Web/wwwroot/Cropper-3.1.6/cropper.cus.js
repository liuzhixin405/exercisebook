var sizeType = $('#CropperImage').data("type");
 
$("#ShowImg").on('click', function () {
    $('#CropperImage').click();

    'use strict';

    $('.float-e-margins').append('<div id="cropperAdmin" style="display:none"><div class= "container" ><div class="row"><div class="col-md-9"><div class="img-container"><img id="image"></div></div > <div class="col-md-3"><div class="docs-preview clearfix"><div class="img-preview preview-lg"></div><div class="img-preview preview-md"></div><div class="img-preview preview-sm"></div><div class="img-preview preview-xs"></div></div><div class="docs-data"><div class="input-group input-group-sm"><span class="input-group-prepend">' +
        '<label class="input-group-text" for="dataX">X</label></span><input type="text" class="form-control" id="dataX" placeholder="x"><span class="input-group-append"><span class="input-group-text">px</span></span></div><div class="input-group input-group-sm"><span class="input-group-prepend"><label class="input-group-text" for="dataY">Y</label></span><input type="text" class="form-control" id="dataY" placeholder="y"><span class="input-group-append"><span class="input-group-text">px</span></span></div><div class="input-group input-group-sm"><span class="input-group-prepend"><label class="input-group-text" for="dataWidth">Width</label>' +
        '</span><input type="text" class="form-control" id="dataWidth" placeholder="width"><span class="input-group-append"><span class="input-group-text">px</span></span></div><div class="input-group input-group-sm"><span class="input-group-prepend"><label class="input-group-text" for="dataHeight">Height</label></span><input type="text" class="form-control" id="dataHeight" placeholder="height"><span class="input-group-append"><span class="input-group-text">px</span></span></div><div class="input-group input-group-sm"><span class="input-group-prepend"><label class="input-group-text" for="dataRotate">Rotate</label></span>' +
        '<input type="text" class="form-control" id="dataRotate" placeholder="rotate"><span class="input-group-append"><span class="input-group-text">deg</span></span></div><div class="input-group input-group-sm"><span class="input-group-prepend"><label class="input-group-text" for="dataScaleX">ScaleX</label></span><input type="text" class="form-control" id="dataScaleX" placeholder="scaleX"></div><div class="input-group input-group-sm"><span class="input-group-prepend"><label class="input-group-text" for="dataScaleY">ScaleY</label></span><input type="text" class="form-control" id="dataScaleY" placeholder="scaleY"></div></div></div></div>' +
        '<div class="row"><div class="col-md-9 docs-buttons"><button onclick="cusConfirm()" class="btn btn-primary">确定</button><button id="CancelCropper" class="btn  btn-danger">取消</button></div><div class="col-md-3 docs-toggles"><div class="btn-group d-flex flex-nowrap" data-toggle="buttons"><label class="btn btn-primary active"><input type="radio" class="sr-only" id="aspectRatio0" name="aspectRatio" value="1.7777777777777777"><span class="docs-tooltip" data-toggle="tooltip" data-animation="false" title="aspectRatio: 16 / 9">16:9</span></label><label class="btn btn-primary">' +
        '<input type="radio" class="sr-only" id="aspectRatio1" name="aspectRatio" value="1.3333333333333333"><span class="docs-tooltip" data-toggle="tooltip" data-animation="false" title="aspectRatio: 4 / 3">4:3</span></label><label class="btn btn-primary"><input type="radio" class="sr-only" id="aspectRatio2" name="aspectRatio" value="1"><span class="docs-tooltip" data-toggle="tooltip" data-animation="false" title="aspectRatio: 1 / 1">1:1</span></label><label class="btn btn-primary"><input type="radio" class="sr-only" id="aspectRatio3" name="aspectRatio" value="0.6666666666666666"><span class="docs-tooltip" data-toggle="tooltip" data-animation="false"' +
        'title="aspectRatio: 2 / 3" > 2: 3</span ></label > <label class="btn btn-primary"><input type="radio" class="sr-only" id="aspectRatio4" name="aspectRatio" value="NaN"><span class="docs-tooltip" data-toggle="tooltip"data-animation="false" title="aspectRatio: NaN">Free</span></label></div><div class="btn-group d-flex flex-nowrap" data-toggle="buttons"><label class="btn btn-primary active"><input type="radio" class="sr-only" id="viewMode0" name="viewMode" value="0" checked><span class="docs-tooltip" data-toggle="tooltip" data-animation="false" title="View Mode 0">VM0</span></label><label class="btn btn-primary">' +
        '<input type="radio" class="sr-only" id="viewMode1" name="viewMode" value="1"><span class="docs-tooltip" data-toggle="tooltip" data-animation="false" title="View Mode 1">VM1</span></label><label class="btn btn-primary"><input type="radio" class="sr-only" id="viewMode2" name="viewMode" value="2"><span class="docs-tooltip" data-toggle="tooltip" data-animation="false" title="View Mode 2">VM2</span></label><label class="btn btn-primary"><input type="radio" class="sr-only" id="viewMode3" name="viewMode" value="3"><span class="docs-tooltip" data-toggle="tooltip" data-animation="false" title="View Mode 3">VM3</span></label></div></div>');

    var console = window.console || { log: function () { } };
    var URL = window.URL || window.webkitURL;
    var $image = $('#image');
    //var $download = $('#download');
    var $dataX = $('#dataX');
    var $dataY = $('#dataY');
    var $dataHeight = $('#dataHeight');
 
    var $dataWidth = $('#dataWidth');
    var $dataRotate = $('#dataRotate');
    var $dataScaleX = $('#dataScaleX');
    var $dataScaleY = $('#dataScaleY');

    console.log("sizeType: ", sizeType);

    if (580326 == sizeType) {
        $("#aspectRatio0").parent().addClass("active");
        $("#aspectRatio0").parent().siblings().removeClass("active");
        aRatio = 16 / 9;
    }
    else if (180135 == sizeType) {
        $("#aspectRatio1").parent().addClass("active");
        $("#aspectRatio1").parent().siblings().removeClass("active");
        aRatio = 4 / 3;
    } else if (0 == sizeType) {
        $("#aspectRatio4").parent().addClass("active");
        $("#aspectRatio4").parent().siblings().removeClass("active");
        aRatio ="NaN";
    }
    else {
        $("#aspectRatio2").parent().addClass("active");
        $("#aspectRatio2").parent().siblings().removeClass("active");
        aRatio = 1;
    }

    var options = {
        aspectRatio: aRatio,
        preview: '.img-preview',
        modal: true,
        viewMode: 1,
        crop: function (e) {
             var ex = e.detail;
            if (0 == sizeType) { 

                $dataX.val(0);
                $dataY.val(0);
                $dataHeight.val(0);
                $dataWidth.val(0);
                $dataRotate.val(0);
                $dataScaleX.val(0);
                $dataScaleY.val(0);
            } else
            {
                $dataX.val(Math.round(ex.x));
                $dataY.val(Math.round(ex.y));
                $dataHeight.val(Math.round(ex.height));
                $dataWidth.val(Math.round(ex.width));
                $dataRotate.val(ex.rotate);
                $dataScaleX.val(ex.scaleX);
                $dataScaleY.val(ex.scaleY);
          }
           
        }
    };
    var originalImageURL = $image.attr('src');
    var uploadedImageName = 'cropped.jpg';
    var uploadedImageType = 'image/jpeg';
    var uploadedImageURL;
    $('#midImg').attr('src', this.result);

    // Tooltip
    $('[data-toggle="tooltip"]').tooltip();

    // Buttons
    if (!$.isFunction(document.createElement('canvas').getContext)) {
        $('button[data-method="getCroppedCanvas"]').prop('disabled', true);
    }

    if (typeof document.createElement('cropper').style.transition === 'undefined') {
        $('button[data-method="rotate"]').prop('disabled', true);
        $('button[data-method="scale"]').prop('disabled', true);
    }

    // Download
    /*if (typeof $download[0].download === 'undefined') {
        $download.addClass('disabled');
    }*/

    // Options
    $('.docs-toggles').on('change', 'input', function () {
        var $this = $(this);
        var name = $this.attr('name');
        var type = $this.prop('type');
        var cropBoxData;
        var canvasData;

        if (!$image.data('cropper')) {
            return;
        }
        if (type === 'checkbox') {
            options[name] = $this.prop('checked');
            cropBoxData = $image.cropper('getCropBoxData');
            canvasData = $image.cropper('getCanvasData'); 
            imagedata = $image.cropper('getImageData'); 
            $image.cropper('setCropBoxData', imagedata);
            options.ready = function () {
                $image.cropper('setCropBoxData', imagedata);
          //      $image.cropper('setCropBoxData', cropBoxData);
                $image.cropper('setCanvasData', canvasData);
            };
        } else if (type === 'radio') {
            options[name] = $this.val();
        }
        $image.cropper('destroy').cropper(options);
    });


    // Methods
    $('.docs-buttons').on('click', '[data-method]', function () {
        var $this = $(this);
        var data = $this.data();
        var cropper = $image.data('cropper');
        //console.log('.docs-buttons' + cropper);
        var cropped;
        var $target;
        var result;

        if ($this.prop('disabled') || $this.hasClass('disabled')) {
            return;
        }
        if (cropper && data.method) {
            data = $.extend({}, data); // Clone a new one
            if (typeof data.target !== 'undefined') {
                $target = $(data.target);

                if (typeof data.option === 'undefined') {
                    try {
                        data.option = JSON.parse($target.val());
                    } catch (e) {
                        console.log(e.message);
                    }
                }
            }
            cropped = cropper.cropped;

            switch (data.method) {
                case 'rotate':
                    if (cropped && options.viewMode > 0) {
                        $image.cropper('clear');
                    }

                    break;
                case 'getCroppedCanvas':
                    if (uploadedImageType === 'image/jpeg') {
                        if (!data.option) {
                            data.option = {};
                        }

                        data.option.fillColor = '#fff';
                    }
                    break;
            }

            result = $image.cropper(data.method, data.option, data.secondOption);

            switch (data.method) {
                case 'rotate':
                    if (cropped && options.viewMode > 0) {
                        $image.cropper('crop');
                    }
                    break;
                case 'scaleX':
                case 'scaleY':
                    $(this).data('option', -data.option);
                    break;
                case 'getCroppedCanvas':
                    if (result) {
                        // Bootstrap's Modal
                        $('#getCroppedCanvasModal').modal().find('.modal-body').html(result);
                        if (!$download.hasClass('disabled')) {
                            download.download = uploadedImageName;
                            $download.attr('href', result.toDataURL(uploadedImageType));
                        }
                    }
                    break;
                case 'destroy':
                    if (uploadedImageURL) {
                        URL.revokeObjectURL(uploadedImageURL);
                        uploadedImageURL = '';
                        $image.attr('src', originalImageURL);
                    }
                    break;
            }

            if ($.isPlainObject(result) && $target) {
                try {
                    $target.val(JSON.stringify(result));
                } catch (e) {
                    console.log(e.message);
                }
            }
        }
    });


    // Keyboard
    $(document.body).on('keydown', function (e) {

        if (!$image.data('cropper') || this.scrollTop > 300) {
            return;
        }

        switch (e.which) {
            case 37:
                e.preventDefault();
                $image.cropper('move', -1, 0);
                break;

            case 38:
                e.preventDefault();
                $image.cropper('move', 0, -1);
                break;

            case 39:
                e.preventDefault();
                $image.cropper('move', 1, 0);
                break;

            case 40:
                e.preventDefault();
                $image.cropper('move', 0, 1);
                break;
        }
    });

    // Import image
    var $CropperImage = $('#CropperImage');
    if (URL) {
        $CropperImage.change(function () {
            var files = this.files;
            var file;
            if (!$image.data('cropper')) {
                return;
            }
            //console.log("length:" + files.length)
            if (files && files.length) {
                file = files[0];
                if (/^image\/\w+$/.test(file.type)) {
                    uploadedImageName = file.name;
                    uploadedImageType = file.type;

                    if (uploadedImageURL) {
                        URL.revokeObjectURL(uploadedImageURL);
                    }
                    uploadedImageURL = URL.createObjectURL(file);
                  
                    var img = new Image();
                    img.src = uploadedImageURL; //dataURL is a base64 image data code previusly obtained.
                    img.onload = function () {
                        setTimeout(function () {
                            if (img.width >= 800) {
                                $image.cropper('setCropBoxData', { width: img.width * 0.5, height: img.height * 0.5 });
                                $image.cropper("zoomTo", 0.5);

                            }else  if (img.width >= 100) {
                                $image.cropper('setCropBoxData', { width: img.width, height: img.height });
                                $image.cropper("zoomTo", 1);

                            } else {

                                $image.cropper('setCropBoxData', { width: img.width * 2, height: img.height * 2 });
                                $image.cropper("zoomTo", 2);
                            }
                        }, 0);
                   }
                          
                    //var nImg = new Image();
                    //nImg.src = file;
                    //nImg.onload = function (e) { console.log(e); };
                    $image.cropper('destroy').attr('src', uploadedImageURL).cropper(options);
                 
                    $('#cropperAdmin').show();
                    $('.ibox-content').hide();
                } else {
                    window.alert('Please choose an image file.');
                }
            }
        });

    } else {
        $CropperImage.prop('disabled', true).parent().addClass('disabled');
    }
    // Cropper
    $image.on({
        ready: function (e) {
            var nImg2 = new Image();
            nImg2.src =  uploadedImageURL;
            var h = nImg2.height; 
            $image.cropper('setData', {
                height: h
            });
        },
        cropstart: function (e) {
            //console.log(e.type, e.action);
        },
        cropmove: function (e) {
            //console.log(e.type, e.action);
        },
        cropend: function (e) {
            //console.log(e.type, e.action);
        },
        crop: function (e) {
            //console.log(e.type, e.x, e.y, e.width, e.height, e.rotate, e.scaleX, e.scaleY);
        },
        zoom: function (e) {
            //console.log(e.type, e.ratio);
        }
    }).cropper(options);
    $('#cropperAdmin #CancelCropper').click(function () {
        $('.ibox-content').show();
        $('#cropperAdmin').hide();
        $CropperImage.val('');
    })
});
function cusConfirm() {
    var result = $("#image").cropper('getCroppedCanvas').toDataURL()
    var nImg2 = new Image();
    nImg2.src = result;
    nImg2.onload = function () {
        var imagew = nImg2.width;
        var imageh = nImg2.height;

        /*if (128 == sizeType && (imagew < 128 || imageh < 128)) {
            alert('图片规格要求为，128*128')
            return
        }
        else if (72 == sizeType && (imagew < 72 || imageh < 72)) {
            alert('图片规格要求为，72*72')
            return
        }
        else if (200 == sizeType && (imagew < 200 || imageh < 200)) {
            alert('图片规格要求为，200*200')
            return
        }
        else if (180135 == sizeType && (imagew < 180 || imageh < 135)) {
            alert('图片规格要求为，180*135')
            return
        }
        else if (580326 == sizeType && (imagew < 580 || imageh < 326)) {
            console.log("imagew: ", imagew);
            console.log("imageh:", imageh);

            alert('图片规格要求为，580*325')
            return
        }*/
        $("#ShowImg").attr('src', result);
        $("#resImgUrl").val(result);
        $('#cropperAdmin').remove();
        $('.ibox-content').show();
    }

}

