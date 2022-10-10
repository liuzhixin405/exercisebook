using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Embed.WebApi
{
    public class InitTest : IInitTest
    {
        //private static readonly string AssemblyName = typeof(InitTest).Assembly.GetName().Name??throw new Exception("dll加载失败");//来自本当前类库
        //private static readonly string AssemblyName = Assembly.LoadFile("C:\\Users\\l\\Desktop\\Service\\EmbedWebApi\\Embed.Controller\\bin\\Debug\\net6.0\\Embed.Controller.dll").GetName().Name ?? throw new Exception("dll加载失败");
        public void Init()
        {
            var builder = WebApplication.CreateBuilder();
            //builder.Services.AddControllers();
            //builder.Services.AddControllers().AddApplicationPart(Assembly.Load(new AssemblyName(AssemblyName))); //被[assembly: ApplicationPart("Embed.WebApi")]替代
            builder.Services.AddControllers().AddApplicationPart(Assembly.LoadFile("C:\\Users\\l\\Desktop\\Service\\EmbedWebApi\\Embed.Controller\\bin\\Debug\\net6.0\\Embed.Controller.dll")); //来自类库
            var app = builder.Build();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });

            app.Run();
        }
    }

    public interface IInitTest
    {
        void Init();
    }
}
