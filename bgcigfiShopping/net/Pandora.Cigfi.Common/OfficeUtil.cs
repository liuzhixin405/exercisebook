using System;
using System.IO;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;
using Pandora.Cigfi.Common.Images;
using Microsoft.AspNetCore.Http;
using OpenXmlPowerTools;

namespace Pandora.Cigfi.Common
{
    /// <summary>
    /// office 工具
    /// </summary>
    public class OfficeUtil
    {
        /// <summary>
        /// 转换word为html 并返回
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public static string ConvertToHtml(IFormFile wordFile, string imagePrefixUrl, string CkImagePath)
        {
            OfficeUtil officeUtil = new OfficeUtil();
            using (var fs = wordFile.OpenReadStream())
            {
                string html = officeUtil.ConvertToHtml(fs, imagePrefixUrl, CkImagePath);
                string body = officeUtil.GetHtmlBody(html);
                string fiterbody = officeUtil.FilterHtml(body);
                return fiterbody;
            }
        }
        /// <summary>
        /// 获取html 的body 部分
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public string GetHtmlBody(string html)
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();

            doc.LoadXml(html);
            var nodes = doc.GetElementsByTagName("body");
            return nodes[0].InnerXml;
        }

        /// <summary>
        /// 过滤html内容
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public string FilterHtml(string html)
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.LoadXml(html);
            ClearXmlStyleAttr(doc.DocumentElement);
            return doc.OuterXml;
        }
        /// <summary>
        /// 移除样式属性
        /// </summary>
        /// <param name="node"></param>
        private void ClearXmlStyleAttr(System.Xml.XmlNode node)
        {
            string[] atts = { "style", "class" };
            foreach (var attname in atts)
            {
                var styleattr = node.Attributes?[attname];
                if (styleattr != null)
                {
                    node.Attributes.Remove(styleattr);
                }
            }
            foreach (System.Xml.XmlNode cnode in node.ChildNodes)
            {
                ClearXmlStyleAttr(cnode);
            }
        }
        /// <summary>
        /// 转换word为html
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
        public string ConvertToHtml(Stream fs, string imagePrefixUrl, string CkImagePath)
        {
            using (var ms = new MemoryStream())
            {
                fs.CopyTo(ms);
                using (WordprocessingDocument doc = WordprocessingDocument.Open(ms, true))
                {
                    HtmlConverterSettings settings = new HtmlConverterSettings()
                    {
                        PageTitle = "My Page Title",
                        ImageHandler = imageInfo =>
                        {
                            string imageFileName = "";
                            try
                            {
                                ImageInfo imageInfo1 = new ImageInfo();
                                //imageInfo.Bitmap.Save(imageFileName, imageFormat);
                                imageFileName = ImageHelper.EncodeImageToBase64Url(imageInfo.Bitmap, imageInfo.Bitmap.RawFormat);
                                if (!string.IsNullOrEmpty(imageFileName) && ImageHelper.IsBase64Image(imageFileName)) 
                                {
                                    var imgModel = ImageHelper.UpLoadImageAsync(imageInfo.AltText, imageFileName, CkImagePath);
                                    if (imgModel.Result.IsSuc)
                                    {
                                        imageFileName = imagePrefixUrl + imgModel.Result.FilePath;
                                    }
                                    else 
                                    {
                                        imageFileName = string.Empty;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                return XElement.Parse(ex.Message);
                            }
                            XElement img = new XElement(Xhtml.img,
                                new XAttribute(NoNamespace.src, imageFileName),
                                imageInfo.ImgStyleAttribute,
                                imageInfo.AltText != null ?
                                    new XAttribute(NoNamespace.alt, imageInfo.AltText) : null);
                            return img;
                        }
                    };
                    XElement html = HtmlConverter.ConvertToHtml(doc, settings);

                    // Note: the XHTML returned by ConvertToHtmlTransform contains objects of type
                    // XEntity. PtOpenXmlUtil.cs defines the XEntity class. See
                    // http://blogs.msdn.com/ericwhite/archive/2010/01/21/writing-entity-references-using-linq-to-xml.aspx
                    // for detailed explanation.
                    //
                    // If you further transform the XML tree returned by ConvertToHtmlTransform, you
                    // must do it correctly, or entities do not serialize properly.
                    return html.ToStringNewLineOnAttributes();
                }
            }
        }
    }

}
