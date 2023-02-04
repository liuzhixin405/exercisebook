using System;
using System.Linq;
using System.Text;
using AspnetRun.Api.Application.Middlewares;
using AspnetRun.Core.Configuration;
using AspnetRun.Core.Entities;
using AspnetRun.Infrastructure.Data;
using AspnetRun.Infrastructure.IoC;
using AspnetRun.Infrastructure.Misc;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NSwag;

namespace AspnetRun.Api
{
    public class Startup
    {

        public Startup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
            AspnetRunSettings = configuration.Get<AspnetRunSettings>();
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment HostingEnvironment { get; }
        public AspnetRunSettings AspnetRunSettings { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return
            services
                .AddCustomMvc()
                .AddCustomDbContext(AspnetRunSettings)
                .AddCustomIdentity()
                .AddCustomSwagger()
                .AddCustomConfiguration(Configuration)
                .AddCustomAuthentication(AspnetRunSettings)
                .AddCustomIntegrations(HostingEnvironment);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("CorsPolicy");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

         

            app.UseMiddleware<LoggingMiddleware>();
            app.UseRouting();
            app.UseAuthentication()
              .UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                var builder = endpoints.MapControllers();
                builder.RequireAuthorization();
            });
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseOpenApi();
            app.UseSwaggerUi3();
        }
    }

    static class CustomExtensionsMethods
    {
        public static IServiceCollection AddCustomMvc(this IServiceCollection services)
        {
            // Add framework services.
            services
                .AddControllers()
               .AddNewtonsoftJson(options =>
               {
                   options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                   options.SerializerSettings.Converters.Add(new DecimalJsonConverter());
               })
                //.AddControllersAsServices()
                ;

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .SetIsOriginAllowed((host) => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            return services;
        }

        public static IServiceCollection AddCustomDbContext(this IServiceCollection services, AspnetRunSettings aspnetRunSettings)
        {
            // use in-memory database
            //services.AddDbContext<AspnetRunContext>(c => c.UseInMemoryDatabase("AspnetRun"));

            // Add AspnetRun DbContext
            services
                .AddEntityFrameworkSqlServer()
                .AddDbContext<AspnetRunContext>(options =>
                        options.UseSqlServer(aspnetRunSettings.ConnectionString,
                        sqlOptions =>
                        {
                            sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                        }
                    ),
                    ServiceLifetime.Scoped
                 );

            return services;
        }

        public static IServiceCollection AddCustomIdentity(this IServiceCollection services)
        {
            var sp = services.BuildServiceProvider();
            using (var scope = sp.CreateScope())
            {
                var existingUserManager = scope.ServiceProvider.GetService<UserManager<AspnetRunUser>>();

                if (existingUserManager == null)
                {
                    services.AddIdentity<AspnetRunUser, AspnetRunRole>(
                        cfg =>
                        {
                            cfg.User.RequireUniqueEmail = true;
                        })
                        .AddEntityFrameworkStores<AspnetRunContext>()
                        .AddDefaultTokenProviders();
                }
            }

            return services;
        }

        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {
            //services.AddSwaggerDocument(config =>
            //{
            //    config.PostProcess = document =>
            //    {
            //        document.Info.Version = "v1";
            //        document.Info.Title = "AspnetRun HTTP API";
            //        document.Info.Description = "The AspnetRun Service HTTP API";
            //        document.Info.TermsOfService = "Terms Of Service";
            //        document.Info.Contact = new NSwag.OpenApiContact
            //        {
            //            Name = "AspnetRun",
            //            Email = string.Empty,
            //            Url = string.Empty
            //        };
            //        document.Info.License = new NSwag.OpenApiLicense
            //        {
            //            Name = "Use under LICX",
            //            Url = "https://example.com/license"
            //        };
            //    };
            //});//swagger不提供登录

            services.AddOpenApiDocument(settings =>
            {
                settings.Title = "AspNetRun.Api";
                settings.AllowReferencesWithProperties = true;
                settings.AddSecurity("身份认证Token", Enumerable.Empty<string>(), new OpenApiSecurityScheme()
                {
                    Scheme = "bearer",
                    Description = "Authorization:Bearer {your JWT token}<br/><b>授权地址:/api/account/createtoken</b>(email password文件ContextSeed中)",
                    Name = "Authorization",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Type = OpenApiSecuritySchemeType.Http
                });
            });

            return services;
        }

        public static IServiceCollection AddCustomConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<AspnetRunSettings>(configuration);

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Instance = context.HttpContext.Request.Path,
                        Status = StatusCodes.Status400BadRequest,
                        Detail = "Please refer to the errors property for additional details."
                    };

                    return new BadRequestObjectResult(problemDetails)
                    {
                        ContentTypes = { "application/problem+json", "application/problem+xml" }
                    };
                };
            });

            return services;
        }

        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, AspnetRunSettings aspnetRunSettings)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
              .AddJwtBearer(options =>
              {
                  options.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuer = true,
                      ValidateAudience = true,
                      ValidateLifetime = true,
                      ValidateIssuerSigningKey = true,

                      ValidIssuer = aspnetRunSettings.Tokens.Issuer,
                      ValidAudience = aspnetRunSettings.Tokens.Audience,
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(aspnetRunSettings.Tokens.Key))
                  };
              });

            return services;
        }

        public static IServiceProvider AddCustomIntegrations(this IServiceCollection services, IWebHostEnvironment hostingEnvironment)
        {
            services.AddHttpContextAccessor();

            var fileProvider = new AppFileProvider(hostingEnvironment.ContentRootPath);
            var typeFinder = new WebAppTypeFinder(fileProvider);

            //configure autofac
            var containerBuilder = new ContainerBuilder();

            //register type finder
            containerBuilder.RegisterInstance(fileProvider).As<IAppFileProvider>().SingleInstance();
            containerBuilder.RegisterInstance(typeFinder).As<ITypeFinder>().SingleInstance();

            //populate Autofac container builder with the set of registered service descriptors
            containerBuilder.Populate(services);

            //find dependency registrars provided by other assemblies
            var dependencyRegistrars = typeFinder.FindClassesOfType<IDependencyRegistrar>();

            //create and sort instances of dependency registrars
            var instances = dependencyRegistrars
                .Select(dependencyRegistrar => (IDependencyRegistrar)Activator.CreateInstance(dependencyRegistrar))
                .OrderBy(dependencyRegistrar => dependencyRegistrar.Order);

            //register all provided dependencies
            foreach (var dependencyRegistrar in instances)
                dependencyRegistrar.Register(containerBuilder, typeFinder);

            return new AutofacServiceProvider(containerBuilder.Build());
        }
    }

    public class DecimalJsonConverter : JsonConverter
    {
        public DecimalJsonConverter()
        {
        }

        public override bool CanRead
        {
            get
            {
                return false;
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
        }

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(decimal) || objectType == typeof(float) || objectType == typeof(double));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (DecimalJsonConverter.IsWholeValue(value))
            {
                writer.WriteRawValue(JsonConvert.ToString(Convert.ToInt64(value)));
            }
            else if (value is decimal)
            {
                var buffer = ((decimal)value).ToString("#0.####################################");
                writer.WriteRawValue(JsonConvert.ToString(Convert.ToDecimal(buffer)));
            }
            else
            {
                writer.WriteRawValue(JsonConvert.ToString(value));
            }
        }

        private static bool IsWholeValue(object value)
        {
            if (value is decimal)
            {
                decimal decimalValue = (decimal)value;
                if (decimalValue - Convert.ToInt64(decimalValue) == 0m)
                    return true;
                int precision = (Decimal.GetBits(decimalValue)[3] >> 16) & 0x000000FF;
                return precision == 0;
            }
            else if (value is float || value is double)
            {
                double doubleValue = (double)value;
                return doubleValue == Math.Truncate(doubleValue);
            }
            return false;
        }
    }

}
