using ECommerce.Application.Services;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerce.API.Controllers
{
    // 合并到 InventoryController 的 /api/inventory/transactions/... 路由下，此控制器已移除
    // 保留文件以提示迁移历史；若无需要，可安全删除该文件。
}