
using Microsoft.AspNetCore.Mvc;
using Pandora.Cigfi.Models.Consts.Sys;
using Pandora.Cigfi.Models.Consts;
using Pandora.Cigfi.Models.Sys;
using Pandora.Cigfi.Web.Common;
using System.Collections;
using System.Threading.Tasks;
using System;
using Pandora.Cigfi.Models.Cigfi.Product;
using System.Linq;
using Pandora.Cigfi.IServices.Cigfi;
using Newtonsoft.Json;
using Pandora.Cigfi.Models.Cigfi.Invitation;
using Pandora.Cigfi.Models.ResponseMsg;

namespace Pandora.Cigfi.Web.Areas.AdminCP.Controllers.Invitation
{
    public class InvitationController : AdminBaseController
    {
        private readonly IInvitationService _invitationService;
        public InvitationController(IServiceProvider serviceProvider, IInvitationService invitationService) : base(serviceProvider)
        {
            _invitationService = invitationService;
        }

        [MyAuthorize(EventKeyConsts.VIEWLIST, MenuKeyConsts.INVITATION)]
        public IActionResult Index()
        {
            return View();
        }
        [MyAuthorize(EventKeyConsts.VIEWLIST, MenuKeyConsts.INVITATION, ReturnTypeConsts.JSON)]
        public async Task<IActionResult> GetPageListAsync(string UserID, string sortKey, string sort, int page = 1, int limit = 20)
        {
            var pageResult = new PagedResult<CigfiMemberViewModel>();

            Hashtable queryHt = new Hashtable();

            
            if (!string.IsNullOrEmpty(UserID))
            {
                queryHt.Add("UserID", UserID);
            }
            if (!string.IsNullOrEmpty(sortKey))
            {
                queryHt.Add("sortKey", sortKey);
            }
            if (!string.IsNullOrEmpty(sort))
            {
                queryHt.Add("sort", sort);
            }
           
            pageResult = await _invitationService.GetPageListAsync(queryHt, page, limit);
            return Json(new
            {
                total = pageResult.Total,
                rows = pageResult.Items
            });
        }

        public async Task<IActionResult> Add()
        {
            //var categories = await _categoryService.GetAllAsync();
            //ViewBag.Categories = categories.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(CigfiMember model)
        {
            if (ModelState.IsValid)
            {
                var success = await _invitationService.AddAsync(model);
                if (success)
                {
                    return Json(new { code = 200, msg = "添加成功", data = new { returnUrl = "closeandreload" } });
                }
                return Json(new { code = 500, msg = "添加失败" });
            }
            return Json(new { code = 400, msg = "参数错误" });
        }

        public async Task<IActionResult> Edit(long id)
        {
            var vm = await _invitationService.GetByIdAsync(id); // 假设返回 
            //var categories = await _invitationService.GetAllAsync();
            //ViewBag.Categories = categories.ToList();
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(CigfiMemberViewModel model)
        {
            if (ModelState.IsValid)
            {
                // ViewModel 映射到实体
                var entity = new CigfiMember
                {
                    UserId = model.UserId,
                    WalletAddress = model.WalletAddress,
                    IsVip = model.IsVip,
                    InviteCode = model.InviteCode,
                    InvitedBy = model.InvitedBy,
                    RebateAmount = model.RebateAmount,
                    CreatedAt = model.CreatedAt,
                    UpdatedAt = model.UpdatedAt
                };
                await _invitationService.UpdateAsync(entity);
                return Json(new { status = 1, message = "修改成功", returnUrl = "closeandreload" });
            }
            return Json(new { status = 0, message = "参数错误" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var ids = id.Split(',').Select(long.Parse).ToList();
                await _invitationService.DeleteAsync(ids);
                return Json(new { status = 1, message = "删除成功" });
            }
            return Json(new { status = 0, message = "请选择要删除的记录" });
        }
    }
}
