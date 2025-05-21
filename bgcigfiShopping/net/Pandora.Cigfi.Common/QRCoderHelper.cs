using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using QRCoder;
/// <summary>
/// 二维码生成帮助类
/// </summary>
public class QRCoderHelper
{
    private QRCoderHelper() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="url">存储内容</param>
    /// <param name="pixel">像素大小</param>
    /// <returns></returns>
    public static Bitmap GetQRCodeImg(string url, int pixel)
    {
        QRCodeGenerator generator = new QRCodeGenerator();
        QRCodeData codeData = generator.CreateQrCode(url, QRCodeGenerator.ECCLevel.M, true);
        QRCoder.QRCode qrcode = new QRCoder.QRCode(codeData);

        //水印图标
        Bitmap qrImage = qrcode.GetGraphic(pixel, Color.Black, Color.White, true);

        return qrImage;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="url">存储内容</param>
    /// <param name="pixel">像素大小</param>
    /// <param name="icon">水印图标</param>
    /// <returns></returns>
    public static Bitmap GetIconQRCodeImg(string url, int pixel, Bitmap icon)
    {
        QRCodeGenerator generator = new QRCodeGenerator();
        QRCodeData codeData = generator.CreateQrCode(url, QRCodeGenerator.ECCLevel.M, true);
        QRCoder.QRCode qrcode = new QRCoder.QRCode(codeData);

        //水印图标
        Bitmap qrImage = qrcode.GetGraphic(pixel, Color.Black, Color.White, icon, 20, 15, false);

        return qrImage;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="url">存储内容</param>
    /// <param name="pixel">像素大小</param>
    /// <param name="contentType">图片类型</param>
    /// <param name="iconUrl">水印图标</param>
    public static string GetIconQRCode(string url, int pixel, string contentType, byte[] imageByte)
    {
        MemoryStream mStream = new MemoryStream(imageByte);
        Bitmap icon = (Bitmap)Image.FromStream(mStream);
        mStream.Close();
        Bitmap bitmap = GetIconQRCodeImg(url, pixel, icon);

        try
        {
            var ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Jpeg);
            var arr = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(arr, 0, (int)ms.Length);
            ms.Close();
            return string.Format("data:{0};base64,{1}", contentType, Convert.ToBase64String(arr));
        }
        catch
        {
            return "";
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="url">存储内容</param>
    /// <param name="pixel">像素大小</param>
    /// <param name="contentType">图片类型</param>
    public static string GetQRCode(string url, int pixel, string contentType)
    {
        Bitmap bitmap = GetQRCodeImg(url, pixel);
        try
        {
            var ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Jpeg);
            var arr = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(arr, 0, (int)ms.Length);
            ms.Close();
            return string.Format("data:{0};base64,{1}", contentType, Convert.ToBase64String(arr));
        }
        catch
        {
            return "";
        }
    }
}

    public class ImageType
    {
        public const string JPEG = "image/jpeg";
        //public const string JPG = "image/jpeg";
        //public const string PNG = "image/jpeg";
    }