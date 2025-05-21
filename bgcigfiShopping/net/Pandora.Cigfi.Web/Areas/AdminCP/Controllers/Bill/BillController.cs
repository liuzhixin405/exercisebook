using Microsoft.AspNetCore.Mvc;
using Pandora.Cigfi.Models.Cigfi;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pandora.Cigfi.Web.Areas.AdminCP.Controllers
{
    [Area("AdminCP")]
    public class BillController : Controller
    {
        // TODO: 注入IBillService
        public async Task<IActionResult> Index()
        {
            // TODO: 从服务获取账单列表
            var bills = new List<Bill>(); // await _billService.GetAllAsync();
            return View(bills);
        }
    }
}
