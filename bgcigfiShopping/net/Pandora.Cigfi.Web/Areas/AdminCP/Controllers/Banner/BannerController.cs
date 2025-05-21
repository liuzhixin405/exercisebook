using Microsoft.AspNetCore.Mvc;
using Pandora.Cigfi.Models.Cigfi;
using Pandora.Cigfi.IServices.Cigfi;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;
using FXH.Common.Oss.AliOss.Interface;
using Pandora.Cigfi.Common;
using Microsoft.Extensions.Options;
using Pandora.Cigfi.Common.Images;
using System.IO;

namespace Pandora.Cigfi.Web.Areas.AdminCP.Controllers
{
    [Area("AdminCP")]
    public class BannerController : Controller
    {
        private readonly IBannerService _bannerService;
        private readonly IAliOssService _aliOssService;
        private readonly ImageConfig _imageConfig;
        
        public BannerController(IBannerService bannerService, IAliOssService aliOssService, IOptions<ImageConfig> imageConfig)
        {
            _bannerService = bannerService;
            _aliOssService = aliOssService;
            _imageConfig = imageConfig.Value;
        }

        public async Task<IActionResult> Index()
        {
            var banners = await _bannerService.GetAllAsync();
            foreach (var banner in banners)
            {
                banner.ImageUrl = _imageConfig.PrefixUrl+ banner.ImageUrl + "?x-oss-process=image/quality,q_30";
            }
            return View(banners);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Banner model, IFormFile imageFile)
        {
            if (ModelState.IsValid && imageFile != null)
            {
                // 处理图片上传
                string imageUrl = await UploadImageAsync(imageFile);
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    model.ImageUrl = imageUrl;
                    model.CreatedAt = DateTime.Now;
                    model.UpdatedAt = DateTime.Now;
                    await _bannerService.AddAsync(model);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "图片上传失败！");
                }
            }
            return View(model);
        }
        
        /// <summary>
        /// 上传图片到OSS
        /// </summary>
        private async Task<string> UploadImageAsync(IFormFile imageFile)
        {
            try
            {
                //判断是否是图片类型
                List<string> imgtypelist = new List<string> { "image/pjpeg", "image/png", "image/x-png", "image/gif", "image/bmp", "image/jpeg" };
                if (imgtypelist.FindIndex(x => x == imageFile.ContentType) == -1)
                {
                    return null;
                }
                
                string sFileNameNoExt = Guid.NewGuid().ToString("N"); //文件名，不带扫展名
                string sFullExtension = Path.GetExtension(imageFile.FileName);//扩展名
                string relativePath = "/cigfi_banner/" + DateTime.Now.ToString("yyyyMMdd") + "/" + CommonUtils.MD5(sFileNameNoExt) + sFullExtension;
                
                var result = _aliOssService.PutObject(relativePath, imageFile.OpenReadStream());
                if (result.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    return relativePath;
                }
            }
            catch (Exception ex)
            {
                // 处理异常
                Console.WriteLine(ex.Message);
            }
            
            return null;
        }

        public async Task<IActionResult> Edit(long id)
        {
            var banner = await _bannerService.GetByIdAsync(id);
            if (banner != null)
            {
                banner.ImageUrl = _imageConfig.PrefixUrl + banner.ImageUrl + "?x-oss-process=image/quality,q_30";
            }
            else
            {
                return NotFound();
            }
            return View(banner);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Banner model, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                // 如果有新上传的图片，则更新图片URL
                if (imageFile != null)
                {
                    string imageUrl = await UploadImageAsync(imageFile);
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        model.ImageUrl = imageUrl;
                    }
                    else
                    {
                        ModelState.AddModelError("", "图片上传失败！");
                        return View(model);
                    }
                }
                
                model.UpdatedAt = DateTime.Now;
                await _bannerService.UpdateAsync(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(long id)
        {
            await _bannerService.DeleteAsync(new List<long> { id });
            return RedirectToAction("Index");
        }
    }
}