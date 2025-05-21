using Microsoft.AspNetCore.Mvc;
using Pandora.Cigfi.Models.Cigfi.Product;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Pandora.Cigfi.IServices.Cigfi;
using System.Collections;
using Microsoft.Extensions.Logging;
using FXH.Common.Oss.AliOss.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Pandora.Cigfi.Common;
using System.IO;

namespace Pandora.Cigfi.Web.Areas.AdminCP.Controllers
{
    [Area("AdminCP")]
    public class ProductController : AdminBaseController
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger<IProductService> _logger;
        private readonly IAliOssService _aliOssService;
        private readonly ImageConfig _imageConfig;
        
        public ProductController(IProductService productService, ICategoryService categoryService, IServiceProvider serviceProvider, ILogger<IProductService> logger, IAliOssService aliOssService, IOptions<ImageConfig> imageConfig)
        : base(serviceProvider)
        {
            _productService = productService;
            _categoryService = categoryService;
            _logger = logger;
            _aliOssService = aliOssService;
            _imageConfig = imageConfig.Value;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Add()
        {
            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories.ToList();
            ViewBag.ImagePrefix = _imageConfig.PrefixUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Product model, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    string imageUrl = await UploadImageAsync(imageFile);
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        model.ImageUrl = imageUrl;
                    }
                    else
                    {
                        ModelState.AddModelError("", "图片上传失败");
                        var categories = await _categoryService.GetAllAsync();
                        ViewBag.Categories = categories.ToList();
                        ViewBag.ImagePrefix = _imageConfig.PrefixUrl;
                        return View(model);
                    }
                }
                var success = await _productService.AddAsync(model);
                if (success)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "添加失败");
            }
            var categories2 = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories2.ToList();
            ViewBag.ImagePrefix = _imageConfig.PrefixUrl;
            return View(model);
        }

        /// <summary>
        /// 上传图片到OSS
        /// </summary>
        private async Task<string> UploadImageAsync(IFormFile imageFile)
        {
            try
            {
                List<string> imgtypelist = new List<string> { "image/pjpeg", "image/png", "image/x-png", "image/gif", "image/bmp", "image/jpeg" };
                if (imgtypelist.FindIndex(x => x == imageFile.ContentType) == -1)
                {
                    return null;
                }
                string sFileNameNoExt = Guid.NewGuid().ToString("N");
                string sFullExtension = Path.GetExtension(imageFile.FileName);
                string relativePath = "/cigfi_product/" + DateTime.Now.ToString("yyyyMMdd") + "/" + CommonUtils.MD5(sFileNameNoExt) + sFullExtension;
                var result = _aliOssService.PutObject(relativePath, imageFile.OpenReadStream());
                if (result.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    return relativePath;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts(int limit = 25, int page = 1, string keyword = "")
        {
            var queryHt = new Hashtable();
            if (!string.IsNullOrWhiteSpace(keyword))
                queryHt["keyword"] = keyword;
            var result = await _productService.GetPageListAsync(queryHt, page, limit);
            return Json(new { total = result.Total, rows = result.Items });
        }


        public async Task<IActionResult> Edit(int id)
        {
            var vm = await _productService.GetByIdAsync(id); // 假设返回 ProductViewModel
            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories.ToList();
            ViewBag.ImagePrefix = _imageConfig.PrefixUrl;
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = new Product
                {
                    Id = model.Id,
                    Name = model.Name,
                    Price = model.Price,
                    Stock = model.Stock,
                    SoldCount = model.SoldCount,
                    CategoryId = model.CategoryId,
                    ImageUrl = model.ImageUrl,
                    ThumbnailUrls = model.ThumbnailUrls,
                    Description = model.Description,
                    CreatedAt = model.CreatedAt
                };
                // 兼容标准表单提交和AJAX方式
                if (Request.Form.Files != null && Request.Form.Files.Count > 0)
                {
                    var file = Request.Form.Files["imageFile"] ?? Request.Form.Files[0];
                    if (file != null && file.Length > 0)
                    {
                        string imageUrl = await UploadImageAsync(file);
                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            entity.ImageUrl = imageUrl;
                        }
                        else
                        {
                            ModelState.AddModelError("", "图片上传失败");
                            var categories = await _categoryService.GetAllAsync();
                            ViewBag.Categories = categories.ToList();
                            ViewBag.ImagePrefix = _imageConfig.PrefixUrl;
                            return View(model);
                        }
                    }
                }
                await _productService.UpdateAsync(entity);
                return RedirectToAction("Index");
            }
            var categories2 = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories2.ToList();
            ViewBag.ImagePrefix = _imageConfig.PrefixUrl;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var ids = id.Split(',').Select(int.Parse).ToList();
                await _productService.DeleteAsync(ids);
                return Json(new { status = 1, message = "删除成功" });
            }
            return Json(new { status = 0, message = "请选择要删除的记录" });
        }
    }
}
