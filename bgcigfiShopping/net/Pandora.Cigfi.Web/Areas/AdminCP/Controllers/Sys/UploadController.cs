using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Aliyun.OSS;
using Pandora.Cigfi.Common;
using Pandora.Cigfi.Models;
using Pandora.Cigfi.Models.Consts;
using Pandora.Cigfi.Models.Site;
using Pandora.Cigfi.Web.Models;
using FXH.Common.Oss.AliOss;
using FXH.Common.Oss.AliOss.Interface;
using FXH.Common.Upload;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Pandora.Cigfi.Web.Areas.AdminCP.Controllers
{
    #region CKEditor错误返回实体类
    /// <summary>
    /// CKEditor错误返回实体类
    /// </summary>
    public class CKFileUploadError
    {
        public CKFileUploadErrorMessage error { get; set; } = new CKFileUploadErrorMessage();
    }
    public class CKFileUploadErrorMessage
    {
        /// <summary>
        /// 错误代码
        /// </summary>
        public int number { get; set; } = 100;
        /// <summary>
        /// 错误信息
        /// </summary>
        public string message { get; set; }
    }
    #endregion

    /// <summary>
    /// 后台上传文件帮助类
    /// </summary>
    public class UploadController : AdminBaseController
    {
        private readonly string PrefixUrl = "https://s1.bqiapp.com";
        private readonly OssClient _ossClient;
        private readonly string CkNewsPath = "news/ckcontent";
        private readonly SystemSetting _attachsetting;
        private readonly ImageConfig _imageConfig;
        private IHostingEnvironment _env;
        private AttachConfigEntity attach;
        private IAliOssService _aliOssService;
       
        public UploadController(IAliOssService aliOssService, IHostingEnvironment env, IOptions<SystemSetting> attachsetting, IServiceProvider serviceProvider, IOptions<ImageConfig> imageConfig) : base(serviceProvider)
        {
            // attach = Config.GetSystemConfig().AttachConfigEntity;
            attach = new AttachConfigEntity()
            {
                AttachPatch = "userfiles",
                ImgMaxHeight = 2000,
                ImgMaxWidth = 1920,
                ThumMaxHeight = 200,
                ThumMaxWidth = 200,
                ThumQty = 80,
                WaterMarkDiaphaneity = 80,
                WaterMarkMinHeight = 400,
                WaterMarkMinWidth = 400,
                WaterMarkPlace = 9,
                WaterMarkQty = 80,
                WaterMarkTextColor = "#0FF",
            };
            
            _aliOssService = aliOssService;
            _ossClient = new OssClient("xxx", "xxx", "xxxxxx");
            _env = env;
            _attachsetting = attachsetting.Value;
            _imageConfig = imageConfig.Value;
            if(null!= _imageConfig && !string.IsNullOrEmpty(_imageConfig.PrefixUrl))
            {
                PrefixUrl = _imageConfig.PrefixUrl;
            }
          
        }
        #region 上传word文档转html
        /// <summary>
        /// 上传word文档转html
        /// </summary>
        /// <param name="wordFile"></param>
        /// <returns></returns>
        public async Task<IActionResult> ConvertToHtml(IFormFile wordFile)
        {
            tip.Status = JsonTip.SUCCESS;
            if (wordFile == null)
            {
                tip.Message = "上传失败";
                tip.Status = JsonTip.ERROR;
                return Json(tip);
            }
            else if(wordFile != null && !wordFile.FileName.EndsWith(".docx") && !wordFile.FileName.EndsWith(".doc"))
            {
                tip.Message = "上传 .docx 或 .doc 文件";
                tip.Status = JsonTip.ERROR;
                return Json(tip);
            }
            
            tip.Message = OfficeUtil.ConvertToHtml(wordFile, _imageConfig.PrefixUrl, ImagePathConsts.NEWSPATH);
            return Json(tip);
        }
        #endregion
        #region CKEditor 上传图片
        /// <summary>
        /// 用于Banner等模块的图片上传方法
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> UploadImage()
        {
            var upload = Request.Form.Files[0];
            if (upload == null)
            {
                return Json(new { status = 0, msg = "请选择一张图片!" });
            }
            
            //判断是否是图片类型
            List<string> imgtypelist = new List<string> { "image/pjpeg", "image/png", "image/x-png", "image/gif", "image/bmp", "image/jpeg" };
            if (imgtypelist.FindIndex(x => x == upload.ContentType) == -1)
            {
                return Json(new { status = 0, msg = "请选择一张图片!" });
            }
            
            string path = string.Empty;
            try
            {
                string sFileNameNoExt = Guid.NewGuid().ToString("N");//文件名字，不带扫展名
                string sFullExtension = CommonUtils.GetFileExtName(upload.FileName);//扫展名
                var filePath = _imageConfig.NewsContent + DateTime.Now.ToString("yyyyMMdd") + "/" + CommonUtils.MD5(sFileNameNoExt) + sFullExtension;
                var result = _aliOssService.PutObject(filePath, upload.OpenReadStream());
                path = _imageConfig.PrefixUrl + "/" + filePath;
                
                return Json(new { status = 1, path = path, name = upload.FileName });
            }
            catch (Exception ex)
            {
                return Json(new { status = 0, msg = "图片上传失败，" + ex.Message + "!" });
            }
        }
        
        /// <summary>
        /// ckeditor 上传图片
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> CKUploadImage()
        {
            string callback = Request.Query["CKEditorFuncNum"];//要求返回值
            var upload = Request.Form.Files["upload"];
            CKFileUploadError errorJson = new CKFileUploadError();
            if (upload == null)
            {
                errorJson.error.message = "请选择一张图片!";
                return Json(errorJson);
            }
            //判断是否是图片类型
            List<string> imgtypelist = new List<string> { "image/pjpeg", "image/png", "image/x-png", "image/gif", "image/bmp", "image/jpeg" };
            if (imgtypelist.FindIndex(x => x == upload.ContentType) == -1)
            {
                errorJson.error.message = "请选择一张图片!";
                return Json(errorJson);
            }
            string path = string.Empty;
            try
            {
                #region 云图片保存
                string sFileNameNoExt = Guid.NewGuid().ToString("N");//文件名字，不带扩展名
                string sFullExtension = CommonUtils.GetFileExtName(upload.FileName);//扩展名
                var filePath = _imageConfig.NewsContent + DateTime.Now.ToString("yyyyMMdd") + "/" + CommonUtils.MD5(sFileNameNoExt) + sFullExtension;
                var NcRes =  _aliOssService.PutObject(filePath, upload.OpenReadStream());
                path = _imageConfig.PrefixUrl + "/" + filePath;
                #endregion
            }
            catch (Exception ex)
            {
                errorJson.error.message = "图片上传失败，" + ex.Message + "!";
                return Json(errorJson);
            }
            dynamic successJson = new
            {
                fileName = upload.FileName,
                uploaded = 1,
                url = path
            };
            return Json(successJson);
        }
        #endregion

        #region CKEditor 上传文件
        public async Task<IActionResult> CKUploadFile()
        {
            string callback = Request.Query["CKEditorFuncNum"];//要求返回值
            var upload = Request.Form.Files["upload"];
            CKFileUploadError errorJson = new CKFileUploadError();
            if (upload == null)
            {
                errorJson.error.message = "请选择一个文件！";
                return Json(errorJson);
            }
            string sFileNameNoExt = CommonUtils.GetFileNameWithoutExtension(upload.FileName);//文件名字，不带扩展名
            string sFullExtension = CommonUtils.GetFileExtName(upload.FileName);//扩展名
            if (string.IsNullOrEmpty(sFullExtension))
            {
                errorJson.error.message = "错误的文件类型！";
                return Json(errorJson);
            }
            //判断是否是允许文件扩展名
            string sAllowedExtensions = _attachsetting.FileAllowedExtensions;
            List<string> listAllowedExtensions = new List<string>();
            string[] arrAllowedExtensions = sAllowedExtensions.Split(new string[] { "," }, StringSplitOptions.None);
            if (arrAllowedExtensions != null && arrAllowedExtensions.Length > 0)
            {
                foreach (var s in arrAllowedExtensions)
                {
                    listAllowedExtensions.Add(s);
                }
            }
            if (listAllowedExtensions.Find(x => x == sFullExtension.ToLower().Replace(".", "")) == null)
            {
                errorJson.error.message = $"{sFullExtension}的文件类型，不允许上传！";
                return Json(errorJson);
            }
            string path;
            try
            {
                #region 云保存
                var filePath = _imageConfig.NewsContent + DateTime.Now.ToString("yyyyMMdd") + "/" + CommonUtils.MD5(sFileNameNoExt) + sFullExtension;
                var NcRes = _aliOssService.PutObject(filePath, upload.OpenReadStream());
                path = _imageConfig.PrefixUrl + "/" + filePath;
                #endregion
            }
            catch (Exception ex)
            {
                errorJson.error.message = "文件上传失败：" + ex.Message;
                return Json(errorJson);
            }
            dynamic successJson = new
            {
                fileName = upload.FileName,
                uploaded = 1,
                url = path
            };
            return Json(successJson);
        }
        #endregion

        #region CKEditor 上传多媒体

        #endregion

        #region Webuploader 上传图片
        //public IActionResult WUUploadImage()
        //{
        //    string isbinary = Request.Query["isbinary"];
        //    switch (isbinary)
        //    {
        //        case "1"://二进制上传
        //            return DoWebuploaderImageByBinary();
        //        default://默认上传
        //            return DoWebuploaderImage();
        //    }
        //}

        #region 二进制上传图片
        /// <summary>
        /// 二进制上传图片（未实现）
        /// </summary>
        /// <returns></returns>
        public IActionResult DoWebuploaderImageByBinary()
        {
            var fileinfo = "";
            var msg = "上传失败!";
            var status = 0;
            var name = "";
            var path = "";
            var thumb = "";
            var size = "";
            var ext = "";
            try
            {
                string upname = Request.Query["name"];
                //判断是否是允许文件扩展名
                string sAllowedExtensions = _attachsetting.ImageAllowedExtensions;
                List<string> listAllowedExtensions = new List<string>();
                string[] arrAllowedExtensions = sAllowedExtensions.Split(new string[] { "," }, StringSplitOptions.None);

                string sFileNameNoExt = CommonUtils.GetFileNameWithoutExtension(upname);//文件名字，不带扩展名
                string sFullExtension = CommonUtils.GetFileExtName(upname);//扩展名

                if (arrAllowedExtensions != null && arrAllowedExtensions.Length > 0)
                {
                    foreach (var s in arrAllowedExtensions)
                    {
                        listAllowedExtensions.Add(s);
                    }
                }
                if (listAllowedExtensions.Find(x => x == sFullExtension.ToLower().Replace(".", "")) == null)
                {
                    return Json(new { status = status, msg = $"{sFullExtension}的文件类型，不允许上传，请上传一张图片！", name = name, path = path, thumb = thumb, size = size, ext = ext });
                }
                var req = HttpContext.Request;
                var f = Request.Form.Files[0];

                Request.EnableBuffering();
                Image image = null;
                using (StreamReader reader = new StreamReader(req.Body, Encoding.UTF8, true, 1024, true))
                {
                    var bytes = ConvertToBinary(reader.BaseStream);
                    image = ReturnPic(bytes);
                    size = bytes.Length.ToString();
                }

                string filepath = $"{_env.WebRootPath}\\{attach.AttachPatch}\\images\\";
                string thumbsFilePath = $"{_env.WebRootPath}\\{attach.AttachPatch}\\_thumbs\\images\\";
                //根据附件配置，设置上传图片目录
                string imgPath = DateTime.Now.Year.ToString();//默认按年
                switch (attach.SaveType)
                {
                    case 1://按月份
                        imgPath = $"{DateTime.Now.Year.ToString()}\\{DateTime.Now.ToString("MM")}";
                        break;
                    case 2:
                        imgPath = $"{DateTime.Now.Year.ToString()}\\{DateTime.Now.ToString("MM")}\\{DateTime.Now.ToString("dd")}";
                        break;
                }
                filepath += imgPath;//存放路径
                thumbsFilePath += imgPath;//缩略图路径
                //图片名字
                string imgname = CommonUtils.GetOrderNum() + CommonUtils.GetFileExtName(upname);
                switch (attach.IsRandomFileName)
                {
                    case 0://不随机
                        imgname = upname;
                        //判断是否存在
                        if (System.IO.File.Exists(Path.Combine(filepath, imgname)))
                        {
                            imgname = sFileNameNoExt + "(1)" + sFullExtension;
                        }
                        break;
                    case 1://随机字符串
                        imgname = CommonUtils.GetShortGUId() + sFullExtension;
                        break;
                    case 2://时间
                        imgname = CommonUtils.GetOrderNum() + sFullExtension;
                        break;
                }
                string fullpath = Path.Combine(filepath, imgname);//图片
                string fullThumbPath = Path.Combine(thumbsFilePath, imgname);//缩略图

                //判断路径
                if (!Directory.Exists(filepath))
                    Directory.CreateDirectory(filepath);
                //缩略图路径
                if (!Directory.Exists(thumbsFilePath))
                    Directory.CreateDirectory(thumbsFilePath);

                //保存图片
                image.Save(filepath);
                //生成缩略图
                if (attach.IsCreateThum == 1)
                {
                    ThumbnailHelper.MakeThumbnailImage(fullpath, fullThumbPath, attach.ThumMaxWidth, attach.ThumMaxHeight, ThumbnailHelper.CutMode.Cut);
                }
                //添加水印
                if (attach.IsWaterMark == 1 && !string.IsNullOrEmpty(attach.WaterMarkImg))
                {
                    string watermarkimg = _env.WebRootPath + attach.WaterMarkImg.Replace("/", "\\");
                    if (System.IO.File.Exists(watermarkimg))
                    {
                        //先复制一张图片出来
                        string copyfullpath = fullpath.Replace(sFullExtension, "_copy" + sFullExtension);
                        System.IO.File.Copy(fullpath, copyfullpath);

                        Image waterpic = new Bitmap(watermarkimg);
                        Image srcPic = new Bitmap(copyfullpath);
                        if (waterpic.Width < srcPic.Width && waterpic.Height < srcPic.Height)
                        {
                            waterpic.Dispose();
                            //srcPic.Dispose();
                            try
                            {
                                WatermarkHelper.AddImageSignPic(srcPic, fullpath, watermarkimg, attach.WaterMarkPlace, attach.WaterMarkQty, attach.WaterMarkDiaphaneity);
                                srcPic.Dispose();
                                System.IO.File.Delete(copyfullpath);
                            }
                            catch
                            {
                                if (System.IO.File.Exists(copyfullpath))
                                {
                                    System.IO.File.Delete(copyfullpath);
                                }
                            }

                        }
                        else
                        {
                            waterpic.Dispose();
                            srcPic.Dispose();
                        }
                    }
                }
                image.Dispose();
                status = 1;
                name = imgname;
                path = $"/{attach.AttachPatch}/images/{imgPath.Replace("\\", "/")}/" + imgname;
                thumb = $"/{attach.AttachPatch}/_thumbs/images/{imgPath.Replace("\\", "/")}/" + imgname;
                ext = sFullExtension;
                fileinfo = $"/{attach.AttachPatch}/images/{imgPath.Replace("\\", "/")}/" + imgname;
                msg = "上传成功!";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return Json(new { status = status, msg = msg, name = name, path = path, thumb = thumb, size = size, ext = ext });
        }
        #endregion

        /// <summary>
        /// 转换格式（webp）与大小(16*16,72*72)
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> ConvertImgFormatAndSize()
        {
            var msg = "上传失败!";
            var status = 0;
            var name = Request.Query["name"];
            var type = Request.Query["type"];

            name = Regex.Replace(name, @"[^a-zA-Z0-9]", "").ToLower();//只留英文、数字;全小写
            var path = "";
            var size = "";
            var data = Request.Form.Files[0];
            string smallPath = "";
            string smallPath_webp = "";
            string midPath = "";
            string bigPath = "";
            string webpPath = "";
            if (data != null)
            {
                if (string.IsNullOrEmpty(name))
                {
                    msg = "请传入文件Code！";
                    return Content(JsonConvert.SerializeObject(new { status = status, msg = msg, name = name, path = path, size = size }), "text/plain");
                }
                name = name.ToString().ToLower();

                //判断是否是图片类型
                List<string> imgtypelist = new List<string> { "image/pjpeg", "image/png", "image/x-png", "image/gif", "image/bmp", "image/jpeg", "image/webp" };
                if (imgtypelist.FindIndex(x => x == data.ContentType) == -1)
                {
                    msg = "只能上传一张图片格式文件！";
                    return Content(JsonConvert.SerializeObject(new { status = status, msg = msg, name = name, path = path, size = size }), "text/plain");
                }
                string filepath = $"{_env.WebRootPath}\\{attach.AttachPatch}\\images\\";

                //根据附件配置，设置上传图片目录
                string imgPath = DateTime.Now.Year.ToString();//默认按年
                switch (attach.SaveType)
                {
                    case 1://按月份
                        imgPath = $"{DateTime.Now.Year.ToString()}\\{DateTime.Now.ToString("MM")}";
                        break;
                    case 2:
                        imgPath = $"{DateTime.Now.Year.ToString()}\\{DateTime.Now.ToString("MM")}\\{DateTime.Now.ToString("dd")}";
                        break;
                }
                filepath += imgPath;//存放路径

                var suffix = DataTypeToSuffix(data.ContentType);
                var fullpath = Path.Combine(filepath, name + suffix);//原图片保存路径
                try
                {
                    //判断路径
                    if (!Directory.Exists(filepath))
                        Directory.CreateDirectory(filepath);

                    if (data != null)
                    {
                        await Task.Run(() =>
                        {
                            using (FileStream fs = new FileStream(fullpath, FileMode.OpenOrCreate))
                            {
                                data.CopyTo(fs);
                            }
                        });

                        #region 本地图片保存

                        //var list = new List<Tuple<string, int, int, ImageMagick.MagickFormat>>();
                        //using (var image = new ImageMagick.MagickImage(fullpath))
                        //{
                        //    if (image.Width > 15 || image.Height > 15)
                        //    {
                        //        string small = Path.Combine(filepath, name + "_small.png");
                        //        list.Add(new Tuple<string, int, int, ImageMagick.MagickFormat>(small, 16, 16, ImageMagick.MagickFormat.Png));
                        //        smallPath = $"/{attach.AttachPatch}/images/{imgPath.Replace("\\", "/")}/" + name + "_small.png";
                        //    }
                        //    if (image.Width > 71 || image.Height > 71)
                        //    {
                        //        string mid = Path.Combine(filepath, name + "_mid.png");
                        //        string webp = Path.Combine(filepath, name + "_webp.webp");
                        //        list.Add(new Tuple<string, int, int, ImageMagick.MagickFormat>(mid, 72, 72, ImageMagick.MagickFormat.Png));
                        //        list.Add(new Tuple<string, int, int, ImageMagick.MagickFormat>(webp, 72, 72, ImageMagick.MagickFormat.WebP));
                        //        midPath = $"/{attach.AttachPatch}/images/{imgPath.Replace("\\", "/")}/" + name + "_mid.png";
                        //        webpPath = $"/{attach.AttachPatch}/images/{imgPath.Replace("\\", "/")}/" + name + "_webp.webp";
                        //    }
                        //    if (image.Width > 199 || image.Height > 199)
                        //    {
                        //        string big = Path.Combine(filepath, name + "_big.png");
                        //        list.Add(new Tuple<string, int, int, ImageMagick.MagickFormat>(big, 200, 200, ImageMagick.MagickFormat.Png));
                        //        bigPath = $"/{attach.AttachPatch}/images/{imgPath.Replace("\\", "/")}/" + name + "_big.png";
                        //    }
                        //}
                        //foreach (var item in list)
                        //{
                        //    using (var fs = new FileStream(item.Item1, FileMode.OpenOrCreate))
                        //    {
                        //        using (var image = new ImageMagick.MagickImage(fullpath))
                        //        {
                        //            image.Resize(item.Item2, item.Item3);
                        //            image.Quality = 100;
                        //            image.Format = item.Item4;
                        //            image.Write(fs);
                        //        }
                        //    }
                        //}

                        #endregion
                        
                        #region 云图片保存
                        var list = new List<ImageModel>();
                        list.Add(new ImageModel()
                        {
                            name = name + "_200",
                            suffix = "png",
                            size = new FXH.Common.Upload.Size() { height = 200, width = 200 }
                        });
                        list.Add(new ImageModel()
                        {
                            name = name + "_72",
                            suffix = "png",
                            size = new FXH.Common.Upload.Size() { height = 72, width = 72 }
                        });
                        list.Add(new ImageModel()
                        {
                            name = name + "_36",
                            suffix = "png",
                            size = new FXH.Common.Upload.Size() { height = 36, width = 36 }
                        });
                        list.Add(new ImageModel()
                        {
                            name = name + "_36",
                            suffix = "webp",
                            size = new FXH.Common.Upload.Size() { height = 36, width = 36 }
                        });
                        list.Add(new ImageModel()
                        {
                            name = name + "_72",
                            suffix = "webp",
                            size = new FXH.Common.Upload.Size() { height = 72, width = 72 }
                        });
                        var storePath = string.Empty;
                        if (type == "1")
                        {
                            storePath = "logo/1/";
                        }
                        else if (type == "2")
                        {
                            storePath = "logo/2/";
                        }
                        var base64string = FileToBase64(fullpath);
                        /*var bitmap = CommonUtils.ImageProcess(base64string, new System.Drawing.Size(190, 142));
                        var ms = new MemoryStream();
                        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                        byte[] bytes = ms.GetBuffer();
                        ms.Close();
                        var newBase64 = Convert.ToBase64String(bytes);*/

                        var model = await ImageUploadHelper.SavaImage("data:image/png;base64," + base64string, list, _imageConfig.UploadImageAPI, storePath);
                        if (model.Status)
                        {
                            if (model.Url.Count == list.Count)
                            {
                                bigPath = _imageConfig.PrefixUrl + model.Url[0].Url;
                                midPath = _imageConfig.PrefixUrl + model.Url[1].Url;
                                smallPath = _imageConfig.PrefixUrl + model.Url[2].Url;
                                smallPath_webp = _imageConfig.PrefixUrl + model.Url[3].Url; 
                                webpPath = _imageConfig.PrefixUrl + model.Url[4].Url;
                            }
                        }
                        else
                        {
                            msg = "图片上传到服务器失败，请联系管理员!";
                        }

                        #endregion
                    }
                    status = 1;
                    path = $"/{attach.AttachPatch}/images/{imgPath.Replace("\\", "/")}/" + name + suffix;
                    msg = "上传成功!";
                }
                catch (Exception ex)
                {
                    msg = "图片上传失败：" + ex.Message;
                }
            }
            else
            {
                msg = "请上传一张图片";
            }

            return Content(JsonConvert.SerializeObject(new { status = status, msg = msg, name = name, path = path, size = size, smallPath = smallPath, smallPath_webp = smallPath_webp, midPath = midPath, bigPath = bigPath, webpPath = webpPath }), "text/plain");
        }


        /// <summary>
        /// 上传资讯图片
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> DoWebuploaderNewsImage()
        {
            var msg = "上传失败!";
            var status = 0;
            var name = "";
            var path = "";
            var thumb = "";
            var size = "";
            var ext = "";
            var data = Request.Form.Files[0];

          
            if (data != null)
            {
                //判断是否是图片类型
                List<string> imgtypelist = new List<string> {"image/png", "image/x-png", "image/jpeg", "image/webp" };
                if (imgtypelist.FindIndex(x => x == data.ContentType) == -1)
                {
                    msg = "只能上传png或jpg格式文件！";
                    return Content(JsonConvert.SerializeObject(new { status = status, msg = msg, name = name, path = path, thumb = thumb, size = size, ext = ext }), "text/plain");
                }

                string filepath = string.Format("{0}/{1}", ImagePathConsts.NEWSPATH, DateTime.Now.ToString("yyyyMMdd"));

                string sFileNameNoExt = CommonUtils.GetOrderNum();//文件名字，不带扩展名
                string sFullExtension = CommonUtils.GetFileExtName(data.FileName);//扩展名


                //图片名字
                string imgname = sFileNameNoExt + sFullExtension;
                

                string fullpath = Path.Combine(filepath, imgname);//图片 
                try
                {
                    //判断路径
                    if (!Directory.Exists(filepath))
                        Directory.CreateDirectory(filepath); 
                    if (data != null)
                    {
                        ///图片自身的宽度
                        int originalWidth = 0;
                        ///图片自身的高度
                        int originalHeight= 0;
                        await Task.Run(() =>
                        {
                            using (FileStream fs = new FileStream(fullpath, FileMode.Create))
                            {
                                data.CopyTo(fs);
                            }
                            using (var imgBmp = new Bitmap(fullpath))
                            {
                                originalWidth = imgBmp.Width;
                                originalHeight = imgBmp.Height;
                            } 
                        });



                        #region 云图片保存
                        List<ImageModel> imagelist = new List<ImageModel>();
                        imagelist.Add(new ImageModel()
                        {
                            suffix = sFullExtension.StartsWith(".") ? sFullExtension.Substring(1) : sFullExtension,
                            name = sFileNameNoExt,
                            size = new FXH.Common.Upload.Size() { width = originalWidth, height = originalHeight }
                        });
                        imagelist.Add(new ImageModel()
                        {
                            suffix = "webp",
                            name = sFileNameNoExt,
                            size = new FXH.Common.Upload.Size() { width = originalWidth, height = originalHeight }
                        });

                        string remotePath = string.Format("{0}/{1}", ImagePathConsts.NEWSPATH, DateTime.Now.ToString("yyyyMMdd"));
                       
                        var base64string = FileToBase64(fullpath);
                        var model = await ImageUploadHelper.SavaImage("data:image/png;base64," + base64string, imagelist, _imageConfig.UploadImageAPI, remotePath); 
                        if (model.Status)
                        {
                            path = _imageConfig.PrefixUrl + model.Url[0].Url;

                        }
                        else
                        {
                            msg = "图片上传到服务器失败，请联系管理员!";
                        }

                        #endregion 

                        if (System.IO.File.Exists(fullpath))
                        {
                            System.IO.File.Delete(fullpath);
                        }
                    }
                    status = 1;
                    name = imgname;  
                    ext = sFullExtension;
                    msg = "上传成功!";
                }
                catch (Exception ex)
                {
                    msg = "图片上传失败：" + ex.Message;
                }
            }
            else
            {
                msg = "请上传一张图片";
            }

            return Content(JsonConvert.SerializeObject(new { status = status, msg = msg, name = name, path = path, thumb = thumb, size = size, ext = ext }), "text/plain");
        }

        #endregion

        #region Webuploader 上传文件
        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> DoWebuploaderFile()
        {
            var msg = "上传失败!";
            var status = 0;
            var name = "";
            var path = "";
            var thumb = "";
            var size = "";
            var ext = "";
            var data = Request.Form.Files[0];
            if (data != null)
            {
                string filepath = $"{_env.WebRootPath}\\{attach.AttachPatch}\\files\\";
                string thumbsFilePath = $"{_env.WebRootPath}\\{attach.AttachPatch}\\_thumbs\\files\\";
                //根据附件配置，设置上传图片目录
                string imgPath = DateTime.Now.Year.ToString();//默认按年
                switch (attach.SaveType)
                {
                    case 1://按月份
                        imgPath = $"{DateTime.Now.Year.ToString()}\\{DateTime.Now.ToString("MM")}";
                        break;
                    case 2:
                        imgPath = $"{DateTime.Now.Year.ToString()}\\{DateTime.Now.ToString("MM")}\\{DateTime.Now.ToString("dd")}";
                        break;
                }
                filepath += imgPath;//存放路径
                thumbsFilePath += imgPath;//缩略图路径
                string sFileNameNoExt = CommonUtils.GetFileNameWithoutExtension(data.FileName);//文件名字，不带扩展名
                string sFullExtension = CommonUtils.GetFileExtName(data.FileName);//扩展名

                //判断是否是允许文件扩展名
                string sAllowedExtensions = _attachsetting.FileAllowedExtensions;
                List<string> listAllowedExtensions = new List<string>();
                string[] arrAllowedExtensions = sAllowedExtensions.Split(new string[] { "," }, StringSplitOptions.None);
                if (arrAllowedExtensions != null && arrAllowedExtensions.Length > 0)
                {
                    foreach (var s in arrAllowedExtensions)
                    {
                        listAllowedExtensions.Add(s);
                    }
                }
                if (listAllowedExtensions.Find(x => x == sFullExtension.ToLower().Replace(".", "")) == null)
                {
                    msg = $"{sFullExtension}的文件类型，不允许上传！";
                    return Content(JsonConvert.SerializeObject(new { status = status, msg = msg, name = name, path = path, thumb = thumb, size = size, ext = ext }), "text/plain");
                }

                //图片名字
                string imgname = CommonUtils.GetOrderNum() + CommonUtils.GetFileExtName(data.FileName);

                switch (attach.IsRandomFileName)
                {
                    case 0://不随机
                        imgname = data.FileName;
                        //判断是否存在
                        if (System.IO.File.Exists(Path.Combine(filepath, imgname)))
                        {
                            imgname = sFileNameNoExt + "(1)" + sFullExtension;
                        }
                        break;
                    case 1://随机字符串
                        imgname = CommonUtils.GetShortGUId() + sFullExtension;
                        break;
                    case 2://时间
                        imgname = CommonUtils.GetOrderNum() + sFullExtension;
                        break;
                }
                string fullpath = Path.Combine(filepath, imgname);//图片
                string fullThumbPath = Path.Combine(thumbsFilePath, imgname);//缩略图
                try
                {
                    //判断路径
                    if (!Directory.Exists(filepath))
                        Directory.CreateDirectory(filepath);
                    //缩略图路径
                    if (!Directory.Exists(thumbsFilePath))
                        Directory.CreateDirectory(thumbsFilePath);
                    if (data != null)
                    {
                        await Task.Run(() =>
                        {
                            using (FileStream fs = new FileStream(fullpath, FileMode.Create))
                            {
                                data.CopyTo(fs);
                            }
                        });
                    }
                    status = 1;
                    name = imgname;
                    path = $"/{attach.AttachPatch}/files/{imgPath.Replace("\\", "/")}/" + imgname;
                    thumb = $"/{attach.AttachPatch}/_thumbs/files/{imgPath.Replace("\\", "/")}/" + imgname;
                    ext = sFullExtension;
                    msg = "上传成功!";
                }
                catch (Exception ex)
                {
                    msg = "图片上传失败：" + ex.Message;
                }
            }
            else
            {
                msg = "请上传一个文件";
            }

            return Content(JsonConvert.SerializeObject(new { status = status, msg = msg, name = name, path = path, thumb = thumb, size = size, ext = ext }), "text/plain");
        }

        #endregion

        #region AilOSS上传文件
        /// <summary>
        /// AilOSS上传文件获取签名信息
        /// <para name="userDirPrefix">上传路径前缀</para>
        /// </summary>
        /// <returns></returns>
        public PostJson AliOSSUploadSignature(string userDirPrefix = "test/")
        {
            var res = _aliOssService.CalculatePostSignature(userDirPrefix);
            return res;
        }
        /// <summary>
        /// AilOSS上传文件获取签名信息
        /// </summary>
        /// <returns></returns>
        public  PostJson CusAliOSSUploadSignature(string userDirPrefix = "", AliOssCallback callback = null)
        {
            //var res = _aliOssService.CalculatePostSignature();
            DateTime ex = DateTime.Now.AddSeconds(3600);
            PolicyConditions policyConda = new PolicyConditions();
            policyConda.AddConditionItem("content-length-range", 0L, 1048576000L);
            policyConda.AddConditionItem(MatchMode.StartWith, PolicyConditions.CondKey, userDirPrefix);

            var postPolicy = _ossClient.GeneratePostPolicy(ex, policyConda);
            byte[] binaryData = Encoding.UTF8.GetBytes(postPolicy);
            var encodedPolicy = Convert.ToBase64String(binaryData);

            var hmac = new HMACSHA1(Encoding.UTF8.GetBytes("xxxxxx"));
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(encodedPolicy));
            var signature = Convert.ToBase64String(hashBytes);
            var callbackStr = "";
            if (callback != null)
            {
                var json = JsonConvert.SerializeObject(callback);
                callbackStr = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
            }

            return new PostJson()
            {
                AccessId = "xxx",
                Policy = encodedPolicy,
                Signature = signature,
                UserDirPrefix = userDirPrefix,
                Host = $"https://fxh-sz.oss-cn-shenzhen.aliyuncs.com",
                Expire = ex,
                Callback = callbackStr
            };
        }
        /// <summary>
        /// AilOSS上传文件列表
        /// </summary>
        /// <returns></returns>
        public ObjectListing AliOSSUploadList(string userDirPrefix = "test/")
        {
            var res = _aliOssService.ListObjects(userDirPrefix);
            return res;
        }
        #endregion

        #region 帮助方法
        /// <summary>
        /// 文件转换为Base64二进制流
        /// </summary>
        /// <param name="fileStream"></param>
        /// <returns></returns>
        public string FileToBase64(string filePath)
        {
            var result = string.Empty;
            try
            {
                using (Stream fs = new FileStream(filePath, FileMode.Open))
                {
                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    fs.Close();
                    result = Convert.ToBase64String(buffer);
                }
            }
            catch (Exception)
            {
            }
            return result;
        }

        public byte[] ConvertToBinary(Stream stream)
        {
            byte[] byData = new byte[stream.Length];// 保存为byte[] 
            stream.Read(byData, 0, byData.Length);
            stream.Dispose();
            return byData;
        }
        /// <summary>
        /// 二进制转图片
        /// </summary>
        /// <param name="streamByte"></param>
        /// <returns></returns>
        public Image ReturnPic(byte[] streamByte)
        {
            MemoryStream ms = new MemoryStream(streamByte);
            Image img = Image.FromStream(ms);
            return img;
        }
        /// <summary>
        /// 文件类型转后缀
        /// </summary>
        /// <param name="dataType">文件类型</param>
        /// <returns>后缀</returns>
        public string DataTypeToSuffix(string dataType)
        {
            var suffix = ".png";
            switch (dataType)
            {
                case "image/pjpeg":
                    suffix = ".jpg";
                    break;
                case "image/png":
                    suffix = ".png";
                    break;
                case "image/x-png":
                    suffix = ".png";
                    break;
                case "image/gif":
                    suffix = ".gif";
                    break;
                case "image/bmp":
                    suffix = ".bmp";
                    break;
                case "image/jpeg":
                    suffix = ".jpeg";
                    break;
                case "image/webp":
                    suffix = ".webp";
                    break;
                default:
                    break;
            }
            return suffix;
        }
        
        #endregion
    }
}