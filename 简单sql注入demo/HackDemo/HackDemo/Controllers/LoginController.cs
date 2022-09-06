using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HackDemo.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace HackDemo.Controllers
{
    public class LoginController : Controller
    {
        private IConfiguration _configuration;
        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Index(string name, string password)
        {
            var conn = _configuration.GetSection("SqlServer")["SqlServerConnection"].ToString();
            SqlHelper helper = new SqlHelper();
            //bool isChecked = helper.CheckUserByPars(conn, name, password);
            bool isChecked = helper.CheckUser(conn, name, password);
            //sql注入  
            //1. https://localhost:44323/Login/index?name=test007'--&password=1234511111111
            //2.  select count(1) from TUser where name='test007' ; insert into TUser(name,password) values('abcd','1234')--' and password='12345678' ;
            if (isChecked)
            {
                string session = base.HttpContext.Session.GetString("currentUser");
                if (string.IsNullOrEmpty(session))
                {
                    base.HttpContext.Session.SetString("currentUser", "hello world");
                }
                ViewBag.SessionString = base.HttpContext.Session.GetString("currentUser");
                return View();
            }
            else
            {
                return new RedirectToActionResult("Error", "Login",null);
            }
        }

        public IActionResult Error()
        {
            return View();
        }

    }
}
