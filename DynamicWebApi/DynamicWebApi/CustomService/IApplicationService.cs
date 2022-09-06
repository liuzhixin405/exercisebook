using Panda.DynamicWebApi;
using Panda.DynamicWebApi.Attributes;

namespace DynamicWebApi.CustomService
{
    [DynamicWebApi]
    public interface IApplicationService:IDynamicWebApi
    {
        //Dynamicwebapi方便业务层代码之间的调用生成的文档，相当于业务层对外开放接口，增加了controoler控制器，效果等同于swagger，只是替我们写好了控制器。
        //使用说明：
        /*
         1.新增通用接口，iapplicationservice，
         2.新增业务接口istudentservice继承iapplicationservice
         3.实现业务接口
         4.安装dynamicwebapi组件
         5.addcontroller之后添加builder.Services.AddDynamicWebApi(),顺序不能乱
         6.改造swagger
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "WebApi", Version = "v1" });

                options.DocInclusionPredicate((docName, description) => true);

            });
         app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "dynamic WebApi");
        });
        7.运行看效果
         */
    }
}
