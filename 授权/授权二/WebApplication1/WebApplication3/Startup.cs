using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using WebApplication3.Models;

namespace WebApplication3
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            #region jwtУ��  HS
            JwtTokenOptions tokenOptions = new JwtTokenOptions();
            Configuration.Bind("JwtTokenOptions", tokenOptions);

            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)//Scheme
            //.AddJwtBearer(options =>
            //{
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        //JWT��һЩĬ�ϵ����ԣ����Ǹ���Ȩʱ�Ϳ���ɸѡ��
            //        ValidateIssuer = true,//�Ƿ���֤Issuer
            //        ValidateAudience = true,//�Ƿ���֤Audience
            //        ValidateLifetime = true,//�Ƿ���֤ʧЧʱ��
            //        ValidateIssuerSigningKey = true,//�Ƿ���֤SecurityKey
            //        ValidAudience = tokenOptions.Audience,//
            //        ValidIssuer = tokenOptions.Issuer,//Issuer���������ǰ��ǩ��jwt������һ��
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SecurityKey)),//�õ�SecurityKey
            //        //AudienceValidator = (m, n, z) =>
            //        //{
            //        //    //��ͬ��ȥ��չ����Audience��У�����---��Ȩ
            //        //    return m != null && m.FirstOrDefault().Equals(this.Configuration["audience"]);
            //        //},
            //        //LifetimeValidator = (notBefore, expires, securityToken, validationParameters) =>
            //        //{
            //        //    return notBefore <= DateTime.Now
            //        //    && expires >= DateTime.Now;
            //        //    //&& validationParameters
            //        //}//�Զ���У�����
            //    };
            //});
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("AdminPolicy",
            //        policyBuilder => policyBuilder
            //        .RequireRole("Admin")//Claim��Role��Admin
            //        .RequireUserName("lzx")//Claim��Name��Eleven
            //        .RequireClaim("EMail")//������ĳ��Cliam
            //         //.RequireClaim("Account")
            //        //.Combine(qqEmailPolicy)
            //        //.AddRequirements(new CustomExtendRequirement())
            //        );//����

            //    //options.AddPolicy("QQEmail", policyBuilder => policyBuilder.Requirements.Add(new QQEmailRequirement()));
            //    //options.AddPolicy("DoubleEmail", policyBuilder => policyBuilder
            //    //.AddRequirements(new CustomExtendRequirement())
            //    //.Requirements.Add(new DoubleEmailRequirement()));
            //});
            #endregion

            #region �ǶԳƼ��� Rs
            #region ��ȡ��Կ
            string path = Path.Combine(Directory.GetCurrentDirectory(), "key.public.json");
            string key = File.ReadAllText(path);//this.Configuration["SecurityKey"];
            Console.WriteLine($"KeyPath:{path}");

            var keyParams = JsonConvert.DeserializeObject<RSAParameters>(key);
            var credentials = new SigningCredentials(new RsaSecurityKey(keyParams), SecurityAlgorithms.RsaSha256Signature);
            #endregion
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,//�Ƿ���֤Issuer
                        ValidateAudience = true,//�Ƿ���֤Audience
                        ValidateLifetime = true,//�Ƿ���֤ʧЧʱ��
                        ValidateIssuerSigningKey = true,//�Ƿ���֤SecurityKey
                        ValidAudience = this.Configuration["JwtTokenOptions:Audience"],//Audience
                        ValidIssuer = this.Configuration["JwtTokenOptions:Issuer"],//Issuer���������ǰ��ǩ��jwt������һ��
                        IssuerSigningKey = new RsaSecurityKey(keyParams)
                    };
                });

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();     //token
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
