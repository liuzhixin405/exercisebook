using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// AutoMapper 扩展
    /// </summary>
    public static class AutoMapperServiceCollectionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="includeDllNames">包含DLL的前缀，
        /// 比如 FXH.API.dll,写 FXH.
        /// 为空，默认包含 FXH.
        /// </param>
        /// <param name="excludeDllNames">需要过滤的dll后缀名称
        ///  例如：FXH.API.dll ，可以写 .API.dll
        ///  为空，不过滤
        /// </param>
        public static IServiceCollection AddAutoMapper(this IServiceCollection services, List<string> includeDllNames = null, List<string> excludeDllNames = null)
        {
            if (includeDllNames == null || includeDllNames.Count <= 0)
            {
                includeDllNames = new List<string> { "Pandora." };
            }
            //当前程序的目录
            var path = AppContext.BaseDirectory;
            //获取DLL 文件
            var files = Directory.GetFiles(path, "*.dll", SearchOption.TopDirectoryOnly).AsEnumerable();
            //排除不需要的文件
            if (excludeDllNames != null && excludeDllNames.Count > 0)
                excludeDllNames.ForEach(ic =>
                {
                    files = files.Where(x => !x.EndsWith(ic, StringComparison.OrdinalIgnoreCase)).ToList();
                });
            //得到需要的文件的名称
            var dllNames = new List<string>();
            foreach (var fileName in files)
            {
                var name = GetFileName(fileName, includeExtension: false);
                dllNames.Add(name);
            }
            includeDllNames.ForEach(ic =>
            {
                dllNames = dllNames.Where(x => x.StartsWith(ic, StringComparison.OrdinalIgnoreCase)).ToList();
            });
            //依赖注入
            services.AddAutoMapper(dllNames.ToArray());
            //
            return services;
        }

        /// <summary>
        /// 获取文件名称
        /// </summary>
        /// <param name="filePath">文件完整路径</param>
        /// <param name="includeExtension">是否包含文件扩展名
        /// 例如：fxh.api.dll ：true-是完整的，false-是 fxh.api
        /// </param>
        /// <returns></returns>
        private static string GetFileName(string filePath, bool includeExtension = true)
        {
            var file = new FileInfo(filePath);
            var fileName = file.Name;
            if (!includeExtension)
            {
                var lastIndex = file.Name.LastIndexOf('.');
                fileName = file.Name.Substring(0, lastIndex);
            }
            return fileName;
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="services"></param>
        /// <param name="dllNames">程序集名称，不包括.dll
        /// 例如：fxh.dd.dll,写 fxh.dd
        /// </param>
        public static IServiceCollection AddAutoMapper(this IServiceCollection services, params string[] dllNames)
        {
            if (dllNames != null)
            {
                var assemblies = new List<Assembly>();
                foreach (var dll in dllNames)
                {
                    assemblies.Add(Assembly.Load(dll));
                }
                services.AddAutoMapper(assemblies.ToArray());
            }
            return services;
        }
    }
}
