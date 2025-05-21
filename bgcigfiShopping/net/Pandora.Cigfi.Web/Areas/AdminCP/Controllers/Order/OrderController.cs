using Microsoft.AspNetCore.Mvc;
using Pandora.Cigfi.IServices.Cigfi;
using Pandora.Cigfi.Models.Cigfi.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pandora.Cigfi.Web.Areas.AdminCP.Controllers
{
    [Area("AdminCP")]
    public class OrderController : AdminBaseController
    {
        // 假设有IOrderService注入，实际请替换为你的服务
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService, IServiceProvider serviceProvider)
        : base(serviceProvider)
        {
            _orderService = orderService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders(int limit = 25, int page = 1, string orderno = "", string userid = "")
        {
            var queryHt = new System.Collections.Hashtable();
            if (!string.IsNullOrWhiteSpace(orderno))
                queryHt["orderno"] = orderno;
            if (!string.IsNullOrWhiteSpace(userid))
                queryHt["userid"] = userid;
            var result = await _orderService.GetPageListAsync(queryHt, page, limit);
            return Json(new { total = result.Total, rows = result.Items });
        }

        
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(OrderViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _orderService.UpdateAsync(model);
                return Json(new { status = 1, message = "修改成功", returnUrl = "closeandreload" });
            }
            return Json(new { status = 0, message = "参数错误" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var ids = id.Split(',').Select(int.Parse).ToList();
                await _orderService.DeleteAsync(ids);
                return Json(new { status = 1, message = "删除成功" });
            }
            return Json(new { status = 0, message = "请选择要删除的记录" });
        }
    }
}
