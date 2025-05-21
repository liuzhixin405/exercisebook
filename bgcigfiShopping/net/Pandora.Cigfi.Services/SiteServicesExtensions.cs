using FXH.Redis.Extensions.Extensions;
using Pandora.Cigfi.IServices;
using Pandora.Cigfi.IServices.Cigfi;
using Pandora.Cigfi.IServices.Sys;
using Pandora.Cigfi.Services;
using Pandora.Cigfi.Services.Cigfi;
using Pandora.Cigfi.Services.Sys;
using System;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 仓储数据扩展类
    /// </summary>
    public static class SiteServicesExtensions
    {
        /// <summary>
        /// 添加数据仓储服务模块
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

      
            #region 系统及权限管理部分
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IAdminLogService, AdminLogService>();
            services.AddScoped<IAdminRolesService, AdminRolesService>();
            services.AddScoped<IAdminMenuService, AdminMenuService>();
            services.AddScoped<IAdminMenuEventService, AdminMenuEventService>();
            services.AddScoped<ITargetEventService, TargetEventService>();
            services.AddScoped<IAdminRightService, AdminRightService>();

            #endregion

            #region 商城
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IOrderService,OrderService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IBannerService, BannerService>();
            #endregion

            //操作日志视图
            services.AddScoped<IOperationLogViewService, OperationLogViewService>();
            //操作日志
            services.AddScoped<IReviewLogService, ReviewLogService>();

        
            return services;
        }
    }
}


