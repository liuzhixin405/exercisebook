using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pandora.Cigfi.IServices.Sys;
using Pandora.Cigfi.Models;
using Pandora.Cigfi.Models.Consts;
using Pandora.Cigfi.Models.Consts.Sys;
using Pandora.Cigfi.Models.Sys;
using Pandora.Cigfi.Web.Common;
using NewLife;
using FXH.Common.Orm;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Pandora.Cigfi.Web.Areas.AdminCP.Controllers.Sys
{
    public class OperationLogController : AdminBaseController
    {
        private readonly IOperationLogViewService _operationLogService;
        private readonly IReviewLogService _operationLog;
        public OperationLogController(IServiceProvider serviceProvider, IReviewLogService operationLog, IOperationLogViewService operationLogService) : base(serviceProvider)
        {
            _operationLogService = operationLogService;
            _operationLog = operationLog;
        }

        [MyAuthorize(EventKeyConsts.VIEWLIST, MenuKeyConsts.OPERATIONLOG)]
        public IActionResult Index()
        {

            return View();
        }

        [MyAuthorize(EventKeyConsts.VIEWLIST, MenuKeyConsts.OPERATIONLOG, ReturnTypeConsts.JSON)]
        public async Task<IActionResult> GetPageListAsync(string operationCode, string operationObject, string operationPeople, string operationType, string isAccurate, string sortKey, string sort, int page = 1, int limit = 20)
        {
            var pageResult = new PagedResult<Sys_ReviewLogViewModel>();

            Hashtable queryHt = new Hashtable();

            if (!string.IsNullOrEmpty(isAccurate))
            {
                queryHt.Add("isAccurate", isAccurate);
            }
            if (!string.IsNullOrEmpty(sortKey))
            {
                queryHt.Add("sortKey", sortKey);
            }
            if (!string.IsNullOrEmpty(sort))
            {
                queryHt.Add("sort", sort);
            }
            if (!string.IsNullOrEmpty(operationCode))
            {
                queryHt.Add("operationCode", operationCode.Trim());
            }
            if (!string.IsNullOrEmpty(operationObject))
            {
                queryHt.Add("operationObject", operationObject);
            }
            if (!string.IsNullOrEmpty(operationPeople))
            {
                queryHt.Add("operationPeople", operationPeople);
            }
            if (!string.IsNullOrEmpty(operationType))
            {
                queryHt.Add("operationType", operationType);
            }
            pageResult.rows = await _operationLogService.GetPageListAsync(queryHt, page, limit);
            pageResult.total = await _operationLogService.CountAsync(queryHt);
            return Content(JsonConvert.SerializeObject(pageResult), "text/plain");
        }


        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [MyAuthorize(EventKeyConsts.VIEWLIST, MenuKeyConsts.OPERATIONLOG, ReturnTypeConsts.JSON)]
        public async Task<IActionResult> GetAllUserAsync()
        {
            var list = new List<Sys_AdminModel>();
            var model = await _adminService.GetAllAsync();
            foreach (var item in model)
            {
                list.Add(new Sys_AdminModel()
                {
                    Id = item.Id,
                    RealName = item.RealName
                });
            }

            return Json(list);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [MyAuthorize(EventKeyConsts.DEL, MenuKeyConsts.OPERATIONLOG, ReturnTypeConsts.JSON)]
        public async Task<IActionResult> Del(string id = "")
        {
            tip.Status = JsonTip.SUCCESS;
            tip.Message = "删除成功";

            if (!string.IsNullOrEmpty(id))
            {
                List<Sys_ReviewLogModel> list = new List<Sys_ReviewLogModel>();
                string[] ids = id.Split(',');
                if (ids.Length > 0)
                {
                    for (int i = 0; i < ids.Length; i++)
                    {
                        list.Add(new Sys_ReviewLogModel() { ID = Convert.ToInt32(ids[i]) });
                    }
                }
                await _operationLog.DeleteAsync(list);

                await base.WriteLog("删除操作日志，成功，ID：" + id, AdminLogType.DEL, AdminLogLevel.NORMAL);
            }
            else
            {
                tip.Status = JsonTip.ERROR;
                tip.Message = "请选择需要删除的记录";
            }
            return Json(tip);
        }

        /// <summary>
        /// 查看详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [MyAuthorize(EventKeyConsts.VIEW, MenuKeyConsts.OPERATIONLOG)]
        public async Task<IActionResult> Look(string id = "")
        {
            var model = new Sys_ReviewLogViewModel();
            if (!string.IsNullOrEmpty(id))
                model = await _operationLogService.GetModelById(Convert.ToInt32(id));

            if (model == null)
                model = new Sys_ReviewLogViewModel();

            return View(model);
        }
    }
}