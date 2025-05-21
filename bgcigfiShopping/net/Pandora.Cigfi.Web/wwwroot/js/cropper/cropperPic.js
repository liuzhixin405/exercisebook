if (document.querySelector('#cropperPic')) { 
    initCropper();
}

function initCropper() {

    var image1 = document.querySelector('#midImg');
    $(image1).on('click', function () {
        //console.log('click')
        //console.log($('#CoverImg').data("type"))
        $('#CoverImg').click();
    })


    $('#CoverImg').on('change', function () {
        //console.log($('#CoverImg').data("type"))

        var sizeType = $('#CoverImg').data("type");

        var reader = new FileReader();
        var file = $(this).prop('files')[0];
        if (!file) {
            return false;
        }
        
        reader.readAsDataURL(file);
        reader.onload = function (e) {
            var dom = $(`<div id="cropperWindow">
                            <div class="tit">编辑</div>
                            <div class="content">
                                <img src="" id="cropperWindowPic">
                            </div>
                            <div class="actionbar">
                                <button id="doCropper" class="btn btn-primary">确定</button>
                                <button id="cancel" class="btn  btn-danger">取消</button>
                            </div>
                         </div>`);
            var cropper = null;
            var aRatio = 0;
            var image = dom.find('#cropperWindowPic');  
            var nImg = new Image();
            nImg.src = this.result;
            nImg.onload = function () {
                w = nImg.width;
                h = nImg.height;
                if (96 == sizeType)
                {
                    aRatio = 96 / 96;
                }
                else if (72 == sizeType)
                {
                    aRatio = 72 / 72;
                }
                else
                {
                    aRatio = 4/3;
                }
                cropper = new Cropper(image[0], {
                    aspectRatio: aRatio,
                    zoomLevel: 1,
                    autoCropArea: 1,
                    center: false,
                    crop(event) {
                        /* console.log(event.detail.x);
                        console.log(event.detail.y);
                        console.log(event.detail.width);
                        console.log(event.detail.height);
                        console.log(event.detail.rotate);
                        console.log(event.detail.scaleX);
                        console.log(event.detail.scaleY); */
                    },
                });

            }
            image.attr('src', this.result);
            $('body').append(dom);
           
            $('#cropperWindow #doCropper').click(function () {
                var result = cropper.getCroppedCanvas().toDataURL('image/png')
                var nImg2 = new Image();
                nImg2.src = result;
                
                nImg2.onload = function () {
                    w = nImg2.width;
                    h = nImg2.height;
                    if (96 == sizeType)
                    {
                        if (w < 96 || h < 96) {
                            alert('图片过小！请上传大等于96px*96px的图片')
                            cropper.destroy()
                            cropper = null
                            $('#CoverImg').val(null)
                            $('#cropperWindow').remove()
                            return
                        }
                        else {
                            $("#midImg").attr('src', result);
                            $("#CoverImgUrl").val(result);
                            cropper.destroy()
                            cropper = null
                            $('#CoverImg').val(null)
                            $('#cropperWindow').remove()
                        }
                    }
                    else if (72 == sizeType) {
                        console.log(w, h)
                        if (w > 400 || h >400) {
                            alert('图片过大！请上传小等于400px*400px的图片')
                            cropper.destroy()
                            cropper = null
                            $('#CoverImg').val(null)
                            $('#cropperWindow').remove()
                            return
                        }
                        else {
                            $("#midImg").attr('src', result);
                            $("#CoverImgUrl").val(result);
                            cropper.destroy()
                            cropper = null
                            $('#CoverImg').val(null)
                            $('#cropperWindow').remove()
                        }
                    }
                    else
                    {
                        if (w < 190 || h < 140) {
                            alert('图片过小！请上传大等于190px*140px的图片')
                            cropper.destroy()
                            cropper = null
                            $('#CoverImg').val(null)
                            $('#cropperWindow').remove()
                        }
                        else if (w > 1080 || h > 1080) {
                            alert('图片过大！请上传小等于1080px*1080px的图片')
                            cropper.destroy()
                            cropper = null
                            $('#CoverImg').val(null)
                            $('#cropperWindow').remove()
                            return
                        }
                        else
                        {
                            $("#midImg").attr('src', result);
                            $("#CoverImgUrl").val(result);
                            cropper.destroy();
                            cropper = null;
                            $('#CoverImg').val(null);
                            $('#cropperWindow').remove();
                        }
                    }
                }  
                 
            })
            $('#cropperWindow #cancel').click(function () {
                cropper.destroy();
                cropper = null;
                $('#CoverImg').val(null);
                $('#cropperWindow').remove();
            })
            
        }
    })
}

