using Microsoft.AspNetCore.Mvc;
using Pandora;
using Pandora.Cigfi;
using Pandora.Cigfi.IServices.Cigfi;
using Pandora.Cigfi.Models.Cigfi.Invitation;
using Pandora.Cigfi.Models.Cigfi.Rebate;
using Pandora.Cigfi.Models.Consts.Sys;
using Pandora.Cigfi.Models.Consts;
using Pandora.Cigfi.Services.Cigfi.Invitation;
using Pandora.Cigfi.Web;
using Pandora.Cigfi.Web.Areas;
using Pandora.Cigfi.Web.Areas.AdminCP;
using Pandora.Cigfi.Web.Areas.AdminCP.Controllers;
using Pandora.Cigfi.Web.Areas.AdminCP.Controllers.Invitation;
using Pandora.Cigfi.Web.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pandora.Cigfi.Models.ResponseMsg;
using Newtonsoft.Json;

namespace Pandora.Cigfi.Web.Areas.AdminCP.Controllers.RebateRatio
{
    [Area("AdminCP")]
    [Route("AdminCP/[controller]/[action]")]
    public class RebateRatioController : AdminBaseController
    {
        private readonly IRebateService _rebateservice;
        public RebateRatioController(IServiceProvider serviceProvider, IRebateService rebateservice) : base(serviceProvider)
        {
            _rebateservice=rebateservice;
        }

        public IActionResult Index()
        {
            return View();
        }

        //[HttpGet("GetListAsync")]
        //public async Task<IActionResult> GetListAsync(int offset = 0, int limit = 20)
        //{
        //    var (items, total) = await _rebateservice.GetPageListAsync(offset, limit);
        //    return Json(new
        //    {
        //        total,
        //        rows = items
        //    });
        //}
        //[HttpGet("GetListAsync")]
        [MyAuthorize(EventKeyConsts.VIEWLIST, MenuKeyConsts.REBATE, ReturnTypeConsts.JSON)]
        public async Task<IActionResult> GetPageListAsync(  int page = 1, int limit = 20)
        {
            var pageResult = new PagedResult<RebateViewModel>();

            Hashtable queryHt = new Hashtable();


            //if (!string.IsNullOrEmpty(UserID))
            //{
            //    queryHt.Add("UserID", UserID);
            //}
            //if (!string.IsNullOrEmpty(sortKey))
            //{
            //    queryHt.Add("sortKey", sortKey);
            //}
            //if (!string.IsNullOrEmpty(sort))
            //{
            //    queryHt.Add("sort", sort);
            //}

            pageResult = await _rebateservice.GetPageListAsync(queryHt, page, limit);
            return Json(new
            {
                total = pageResult.Total,
                rows = pageResult.Items
            });
            //return Content(JsonConvert.SerializeObject(pageResult), "text/plain");
        }


        public async Task<IActionResult> GetById(int id)
        {
            var data = await _rebateservice.GetByIdAsync(id);
            if (data == null)
                return Json(new { success = false, message = "数据不存在" });

            return Json(new { success = true, data });
        }

        //[HttpPost("Add")]
        public async Task<IActionResult> Add([FromForm] Rebate dto)
        {
            var success = await _rebateservice.AddAsync(dto);
            if (success)
                return Json(new { success = true });
            else
                return Json(new { success = false, message = "添加失败" });
        }

        public async Task<IActionResult> Edit([FromForm] Rebate dto)
        {
            var success = await _rebateservice.UpdateAsync(dto);
            if (success)
                return Json(new { success = true });
            else
                return Json(new { success = false, message = "更新失败" });
        }

        public async Task<IActionResult> Delete(List<int> id)
        {
            var success = await _rebateservice.DeleteAsync(id);
            if (success)
                return Json(new { success = true });
            else
                return Json(new { success = false, message = "删除失败" });
        }
    }
}
