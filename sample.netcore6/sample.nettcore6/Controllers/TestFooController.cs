using IDCM.Contract.Business.Report;
using IDCM.Contract.Core;
using IDCM.Contract.Utility;
using IDCM.Contract.WebAdmin.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using NSwag.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace IDCM.Contract.WebAdmin.Controllers.Base_Manage
{

    [Route("api/[controller]/[action]")]
    [OpenApiTag("Test")]
    public class TestController : BaseBackController
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ApplicationPartManager _applicationPartManager;
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;
        private readonly SwaggerGenerator _swaggerGenerator;
        public TestController(IServiceProvider serviceProvider, ApplicationPartManager applicationPartManager,
            IActionDescriptorCollectionProvider actionDescriptorCollectionProvider
            , SwaggerGenerator swaggerGenerator)
        {
            _serviceProvider = serviceProvider;
            _applicationPartManager = applicationPartManager;
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
            _swaggerGenerator = swaggerGenerator;
        }

        [HttpGet]
        public IActionResult GetRouteBaz()
        {
            var controllerFeature = new ControllerFeature();
            _applicationPartManager.PopulateFeature(controllerFeature);

            var data = controllerFeature.Controllers.Select(x => new
            {
                Namespace = x.Namespace,
                Controller = x.FullName,
                ModuleName = x.Module.Name,
                Actions = x.DeclaredMethods.Where(m => m.IsPublic && !m.IsDefined(typeof(NonActionAttribute))).Select(y => new
                {
                    Name = y.Name,
                    ParameterCount = y.GetParameters().Length,
                    Parameters = y.GetParameters().Select(z => new
                    {
                        z.Name,
                        z.ParameterType.FullName,
                        z.Position
                    })
                })
            });
            return Ok(data);
        }


        [HttpGet]
        public IActionResult GetRouteBar()
        {
            var routes = _actionDescriptorCollectionProvider.ActionDescriptors.Items.Select(x => new
            {
                Action = x.RouteValues["Action"],
                Controller = x.RouteValues["Controller"],
                Name = x.AttributeRouteInfo.Name,
                Method = x.ActionConstraints?.OfType<HttpMethodActionConstraint>().FirstOrDefault()?.HttpMethods.First(),
                Template = x.AttributeRouteInfo.Template
            }).OrderBy(x=>x.Controller).ToList();
            return Ok(routes);
        }

        [HttpGet]
        public IActionResult GetRouteFoo()
        {
            var list = _swaggerGenerator.GetSwagger("v1");
            List<object> paths =new List<object>();
            foreach (var item in list.Paths)
            {
                if(item.Value.Operations !=null && item.Value.Operations.Count > 0)
                {
                    foreach (var operation in item.Value.Operations)
                    {
                        paths.Add(new
                        {
                            Controller = operation.Value.Tags.FirstOrDefault()?.Name,
                            Path = item.Key,
                            HttpMethod = operation.Key.ToString(),
                            Summary = operation.Value.Summary
                        });
                    }
                }
            }
            return Ok(paths);
        }
        ///// <summary>
        ///// 邮箱推送
        ///// </summary>
        ///// <param name="startTime"></param>
        ///// <param name="endTime"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public async Task SendReportEmail(DateTime? startTime, DateTime? endTime)
        //{
        //    //using var scope = _serviceProvider.CreateAsyncScope();
        //    //var reportBus = scope.ServiceProvider.GetService<IBusinessStatisticalReportBusiness>();
        //    //await reportBus.CreateFirstDailyReport(null, null);
        //    //using var sendReport = new SendReportImplement(_serviceProvider);
        //    //await sendReport.SendEmail(startTime, endTime);
        //    throw new NotImplementedException("测试中");
        //}


        ///// <summary>
        ///// 同步用户扩展历史数据
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //public async Task<bool> UpdateCustomerExtendHistory()
        //{
        //    //using var extendImplement = new CustomerExtendUpdateImplement(_serviceProvider);
        //    //return await extendImplement.UpdateExtendHistory();
        //    throw new NotImplementedException("测试中");
        //}

        ///// <summary>
        ///// 获取用户实时手续费、收益
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //public async Task<dynamic> GetInTimeCustomerExtendDetail(string[] ids)
        //{
        //    if (ids == null || ids.Length == 0)
        //        return new { };
        //    using var extendImplement = new CustomerExtendUpdateImplement(_serviceProvider);
        //    return await extendImplement.UpdateCustomerByIds(ids);
        //}

        ///// <summary>
        ///// 删除excel文件
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //public void DeleteExcel()
        //{
        //    if (System.IO.File.Exists($"Download/PreKlicklContract{DateTime.Now.ToString("yyyyMM")}.xlsx"))
        //    {
        //        System.IO.File.Delete($"Download/PreKlicklContract{DateTime.Now.ToString("yyyyMM")}.xlsx");
        //    }

        //    if (System.IO.File.Exists($"Download/KlicklContract{DateTime.Now.ToString("yyyyMM")}.xlsx"))
        //    {
        //        System.IO.File.Delete($"Download/KlicklContract{DateTime.Now.ToString("yyyyMM")}.xlsx");
        //    }

        //    if (System.IO.File.Exists($"Download/PreKlicklFutures{DateTime.Now.ToString("yyyyMM")}.xlsx"))
        //    {
        //        System.IO.File.Delete($"Download/PreKlicklFutures{DateTime.Now.ToString("yyyyMM")}.xlsx");
        //    }

        //    if (System.IO.File.Exists($"Download/KlicklFutures{DateTime.Now.ToString("yyyyMM")}.xlsx"))
        //    {
        //        System.IO.File.Delete($"Download/KlicklFutures{DateTime.Now.ToString("yyyyMM")}.xlsx");
        //    }
        //}
    }
}
