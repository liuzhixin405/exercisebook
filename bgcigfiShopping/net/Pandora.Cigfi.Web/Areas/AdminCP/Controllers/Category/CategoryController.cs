using Microsoft.AspNetCore.Mvc;
using Pandora.Cigfi.Models.Cigfi;
using Pandora.Cigfi.IServices.Cigfi;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Pandora.Cigfi.Web.Areas.AdminCP.Controllers
{
    [Area("AdminCP")]
    public class CategoryController : AdminBaseController
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(IServiceProvider serviceProvider,ICategoryService categoryService) : base(serviceProvider)
        {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllAsync();
            return View(categories);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(CategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 映射 ViewModel 到实体
                var entity = new Category
                {
                    Id = model.Id,
                    Name = model.Name,
                    ParentId = model.ParentId,
                    SortOrder = model.SortOrder
                };
                await _categoryService.AddAsync(entity);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(long id)
        {
            var vm = await _categoryService.GetByIdAsync(id);
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 映射 ViewModel 到实体
                var entity = new Category
                {
                    Id = model.Id,
                    Name = model.Name,
                    ParentId = model.ParentId,
                    SortOrder = model.SortOrder
                };
                await _categoryService.UpdateAsync(entity);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(long id)
        {
            await _categoryService.DeleteAsync(new List<long> { id });
            return RedirectToAction("Index");
        }

       
    }
}
