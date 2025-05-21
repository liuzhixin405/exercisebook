using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Pandora.Cigfi.Common.Security;
using FXH.Common.Oss.AliOss.Interface;
using FXH.Common.Upload;

namespace Pandora.Cigfi.Common.Images
{
    /// <summary>
    /// oss文件上传返回结果
    /// </summary>
    public struct OssUploadResault
    {
        public string msg { get; set; }
        public string path { get; set; }
        public bool status { get; set; }
    }
    public  class ImageHelper
    {

        /// <summary>
        /// oss图片上传
        /// </summary>
        /// <param name="imagebase64"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static OssUploadResault ImageOssUploads(IAliOssService aliOssService, string imagebase64, string fileName, string file)
        {
            var res = new OssUploadResault();
            try
            {
                var filePath = file + CommonUtils.CusMD5Name(fileName, imagebase64);
                var NcRes = aliOssService.PutObject_Base64(filePath, CommonUtils.StandardBase64(imagebase64));
                res.path = "/" + filePath + "?v=" + DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                res.msg = "上传成功";
                res.status = true;
            }
            catch (Exception ex)
            {
                res.msg = $"图片上传失败, {ex.Message}";
                res.status = false;
            }
            return res;
        }
        /// <summary>
        /// 图片base64编码的头部
        /// </summary>
        private const string imageBase64RegPattern = @"data:image/(.+);base64";

        /// <summary>
        /// 是否是图片的base64编码
        /// </summary>
        /// <param name="base64string"></param>
        /// <returns></returns>
        public static bool IsBase64Image(string base64string)
        {
            return Regex.IsMatch(base64string, imageBase64RegPattern, RegexOptions.IgnoreCase);
        }
        /// <summary>
        /// 根据base64编码判断上传文件格式
        /// </summary>
        /// <param name="base64string"></param>
        /// <returns></returns>
        public static string GetFileSuffix(string base64string)
        {
            var result = Regex.Match(base64string, @"data:image/(.+);base64");
            if (result.Length != 0)
            {
                if (result.Groups[1].Value.IndexOf(".") == 0)
                {
                    return result.Groups[1].Value;
                }
                else
                {
                    return "." + result.Groups[1].Value;
                }
            }
            if (base64string.Contains("png", StringComparison.OrdinalIgnoreCase))
                return ".png";
            if (base64string.Contains("bmp", StringComparison.OrdinalIgnoreCase))
                return ".bmp";
            if (base64string.Contains("gif", StringComparison.OrdinalIgnoreCase))
                return ".gif";
            if (base64string.Contains("icon", StringComparison.OrdinalIgnoreCase))
                return ".icon";
            return ".jpg";
        }
        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="name">文件名</param>
        /// <param name="base64string">图片base64</param>
        /// <param name="storagePath">保存路径</param>
        /// <param name="width">图宽</param>
        /// <param name="height">图高</param>
        /// <param name="flag">是否自定义设置图片宽高</param>
        /// <returns></returns>
        public static async Task<ImageUpLoadModel> UpLoadImageAsync(string name, string base64string, string storagePath, string api = null, int width=0,int height=0)
        {
            ImageUpLoadModel resultModel = new ImageUpLoadModel();
            if (!ImageHelper.IsBase64Image(base64string))
            {
                resultModel.Msg = "提交的图片非base64编码";
                resultModel.IsSuc = false;
                return resultModel;
            }
            string fileName = DataCrypto.MD5(name);

            string suffix = ImageHelper.GetFileSuffix(base64string);
            List<ImageModel> imagelist = new List<ImageModel>();

            imagelist.Add(new ImageModel()
            {
                suffix = suffix,
                name = fileName,
                size = new FXH.Common.Upload.Size() { width = width, height = height }
            });
            
            string path = string.Format("{0}/{1}", storagePath, DateTime.Now.ToString("yyyyMMdd"));
            var model = await ImageUploadHelper.SavaImage(base64string, imagelist, api, path);
            if (model.Status && model.Url.Count > 0)
            {
                resultModel.FilePath = model.Url[0].Url.Replace("\\", "/");
                resultModel.IsSuc = true;
            }
            else
            {
                resultModel.Msg = model.Message;
                resultModel.IsSuc = false;
            }
            return resultModel;
        }
        /// <summary>
        /// 图片转成 Base64编码 url
        /// </summary>
        public static string EncodeImageToBase64Url(Bitmap bitmap, ImageFormat imageFormat)
        {
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, imageFormat == ImageFormat.Gif ? ImageFormat.Gif : ImageFormat.Jpeg);
                var byt = ms.ToArray();
                string contentType = imageFormat == ImageFormat.Gif ? "image/gif" : "image/jpeg";//写死contentType
                string base64AvatarImg = $"data:{contentType};base64,{Convert.ToBase64String(byt)}";
                return base64AvatarImg;
            }
        }

        /// <summary>
        /// 图片上传 Base64解码
        /// </summary>
        /// <param name="dataURL">Base64数据</param>
        /// <returns>返回一个Bitmap</returns>
        public static Bitmap DecodeBase64ToBitmap(string dataURL)
        {

            String base64;
            int index = dataURL.IndexOf(",");
            if (index != -1)
            {
                base64 = dataURL.Substring(index + 1);      //将‘，’以前的多余字符串删除
            }
            else
            {
                base64 = dataURL;
            }
            Bitmap bitmap = null;//定义一个Bitmap对象，接收转换完成的图片


            byte[] arr = Convert.FromBase64String(base64);//将纯净资源Base64转换成等效的8位无符号整形数组

            System.IO.MemoryStream ms = new System.IO.MemoryStream(arr);//转换成无法调整大小的MemoryStream对象
            bitmap = new Bitmap(ms);//将MemoryStream对象转换成Bitmap对象


            return bitmap;//返回相对路径
        }


        /// <summary>
        /// 图片上传 Base64解码
        /// </summary>
        /// <param name="dataURL">Base64数据</param>
        /// <returns>返回一个Bitmap</returns>
        public static Bitmap DecodeUrlToBitmap(string URL)
        {
            using (WebClient wc = new WebClient())
            {
                using (var st = wc.OpenRead(URL))
                {
                    using (System.IO.MemoryStream ms = new MemoryStream())
                    {
                        st.CopyTo(ms);//转换成无法调整大小的MemoryStream对象
                        var bitmap = new Bitmap(ms);//将MemoryStream对象转换成Bitmap对象
                        return bitmap;//返回相对路径
                    }
                }
            }
        }
        /// <summary>
        /// 重置图片大小
        /// </summary>
        /// <param name="imgToResize"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Bitmap ResizeImage(Bitmap imgToResize, System.Drawing.Size size)
        {
            //获取图片宽度
            int sourceWidth = imgToResize.Width;
            //获取图片高度
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;
            //计算宽度的缩放比例
            nPercentW = ((float)size.Width / (float)sourceWidth);
            //计算高度的缩放比例
            nPercentH = ((float)size.Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;
            //期望的宽度
            int destWidth = (int)(sourceWidth * nPercent);
            //期望的高度
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage(b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //绘制图像
            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();
            return b;
        }/// <summary>
         /// 加水印
         /// </summary>
         /// <param name="sourceImage"></param>
         /// <param name="waterImage"></param>
         /// <param name="x"></param>
         /// <param name="y"></param>
         /// <param name="w"></param>
         /// <param name="h"></param>
         /// <param name="picFormat"></param>
         /// <param name="alpha"></param>
         /// <returns></returns>
        public static Bitmap DrawImage(Bitmap sourceImage, Bitmap waterImage, int x, int y, int w, int h, float alpha = 1.0f)
        {
            //
            // 将需要加上水印的图片装载到Image对象中
            //
            Bitmap imgPhoto = sourceImage;

            //
            // 确定其长宽
            //
            int phWidth = imgPhoto.Width;
            int phHeight = imgPhoto.Height;

            //
            // 封装 GDI+ 位图，此位图由图形图像及其属性的像素数据组成。
            //
            var pixelFormat = imgPhoto.PixelFormat;
            if (!Enum.IsDefined(typeof(PixelFormat), imgPhoto.PixelFormat))
            {
                pixelFormat = PixelFormat.Format24bppRgb;
            }
            Bitmap bmPhoto = new Bitmap(phWidth, phHeight, pixelFormat);//phWidth, phHeight, PixelFormat.Format24bppRgb  imgPhoto
                                                                        // if()
                                                                        //
                                                                        // 设定分辨率
                                                                        // 
                                                                        //bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            //
            // 定义一个绘图画面用来装载位图
            //
            Graphics grPhoto = Graphics.FromImage(bmPhoto);

            //
            //同样，由于水印是图片，我们也需要定义一个Image来装载它
            //
            Image imgWatermark = waterImage;

            //
            // 获取水印图片的高度和宽度
            //
            int wmWidth = imgWatermark.Width;
            int wmHeight = imgWatermark.Height;

            //SmoothingMode：指定是否将平滑处理（消除锯齿）应用于直线、曲线和已填充区域的边缘。
            // 成员名称  说明 
            // AntiAlias   指定消除锯齿的呈现。 
            // Default    指定不消除锯齿。

            // HighQuality 指定高质量、低速度呈现。 
            // HighSpeed  指定高速度、低质量呈现。 
            // Invalid    指定一个无效模式。 
            // None     指定不消除锯齿。 
            grPhoto.SmoothingMode = SmoothingMode.AntiAlias;

            //
            // 第一次描绘，将我们的底图描绘在绘图画面上
            //
            grPhoto.DrawImage(imgPhoto,
             new Rectangle(0, 0, phWidth, phHeight),
             0,
             0,
             phWidth,
             phHeight,
             GraphicsUnit.Pixel);

            //
            // 与底图一样，我们需要一个位图来装载水印图片。并设定其分辨率
            //
            Bitmap bmWatermark = new Bitmap(bmPhoto);
            //bmWatermark.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            //
            // 继续，将水印图片装载到一个绘图画面grWatermark
            //
            Graphics grWatermark = Graphics.FromImage(bmWatermark);

            //
            //ImageAttributes 对象包含有关在呈现时如何操作位图和图元文件颜色的信息。
            //   

            ImageAttributes imageAttributes = new ImageAttributes();

            //
            //Colormap: 定义转换颜色的映射
            //
            ColorMap colorMap = new ColorMap();

            //
            //我的水印图被定义成拥有绿色背景色的图片被替换成透明
            //
            colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
            colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);

            ColorMap[] remapTable = { colorMap };

            imageAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);

            float[][] colorMatrixElements = {
        new float[] {1.0f, 0.0f, 0.0f, 0.0f, 0.0f}, //red红色
                new float[] {0.0f, 1.0f, 0.0f, 0.0f, 0.0f}, //green绿色
                new float[] {0.0f, 0.0f, 1.0f, 0.0f, 0.0f}, //blue蓝色    
                new float[] {0.0f, 0.0f, 0.0f, alpha, 0.0f},//透明度   
                new float[] {0.0f, 0.0f, 0.0f, 0.0f, 1.0f}};//

            // ColorMatrix:定义包含 RGBA 空间坐标的 5 x 5 矩阵。
            // ImageAttributes 类的若干方法通过使用颜色矩阵调整图像颜色。
            ColorMatrix wmColorMatrix = new ColorMatrix(colorMatrixElements);

            imageAttributes.SetColorMatrix(wmColorMatrix, ColorMatrixFlag.Default,
            ColorAdjustType.Bitmap);

            //
            //上面设置完颜色，下面开始设置位置
            //

            imgPhoto.Dispose();//释放底图，解决图片保存时 “GDI+ 中发生一般性错误。”
                               // 第二次绘图，把水印印上去
                               //
            grWatermark.DrawImage(imgWatermark,
      new Rectangle(x,
           y,
           w,
           h),
           0,
           0,
           wmWidth,
           wmHeight,
           GraphicsUnit.Pixel,
           imageAttributes);

            imgPhoto = bmWatermark;
            grPhoto.Dispose();
            grWatermark.Dispose();

            //try
            //{
            //    imgPhoto.Save(destImagePath, picFormat);
            //}
            //catch (Exception e)
            //{
            //    LogManager.LogManager.Instance.Write("DrawImage:" + e.Message, MsgType.Error);
            //    return false;
            //}

            //imgPhoto.Dispose();
            imgWatermark.Dispose();
            return imgPhoto;
        }
        /// <summary>
        /// 图片上传 Base64解码
        /// </summary>
        /// <param name="dataURL"></param>
        /// <returns></returns>
        public static MemoryStream DecodeBase64ToImageMemoryStream(string dataURL)
        {
            String base64 = dataURL.Substring(dataURL.IndexOf(",") + 1);      //将‘，’以前的多余字符串删除
            Bitmap bitmap = null;//定义一个Bitmap对象，接收转换完成的图片
            byte[] arr = Convert.FromBase64String(base64);//将纯净资源Base64转换成等效的8位无符号整形数组
            MemoryStream ms = new MemoryStream(arr);//转换成无法调整大小的MemoryStream对象
            bitmap = new Bitmap(ms);//将MemoryStream对象转换成Bitmap对象
            MemoryStream rms = new MemoryStream();
            bitmap.Save(rms, ImageFormat.Jpeg);//保存到服务器路径
            ms.Close();//关闭当前流，并释放所有与之关联的资源
            bitmap.Dispose();
            return rms;
        }
    }

    public class ImageUpLoadModel
{
        public ImageUpLoadModel()
        {
            Msg = FilePath = string.Empty;
        }
        public bool IsSuc
        {
            get;
            set;
        }
        public string Msg
        {
            get;
            set;
        }
        public string FilePath
        {
            get;
            set;
        }
}

}

