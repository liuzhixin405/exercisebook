using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;

namespace PluginApi
{
    internal class Startup
    {
        private readonly WebApiPluginManager _webApiPluginManager;
        public Startup(IConfiguration configuration)
        {
            _webApiPluginManager = new WebApiPluginManager();
            _webApiPluginManager.LoadPlugins("pluginFolder");
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            //services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "main API", Version = "v1" }));
            _webApiPluginManager.ConfigureServices(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                //app.UseSwagger();
                //app.UseSwaggerUI(c =>
                //{
                //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Main API");
                //    c.RoutePrefix = "swagger";
                //});
            }
            app.UseRouting();
            app.UseHttpsRedirection();

            app.UseAuthorization();
          
            _webApiPluginManager.Configure(app, env);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}