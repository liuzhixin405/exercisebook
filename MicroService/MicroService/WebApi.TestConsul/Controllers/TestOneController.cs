using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgilityFramework.ConsulClientExtend.ConsulClientExtend;
using Consul;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WebApi.TestConsul.Models;
using WebApi.TestConsul.Utility;

namespace WebApi.TestConsul.Controllers
{
    public class TestOneController : Controller
    {
        private ILogger<TestOneController> _logger;
        private IConsulDispatcher _consulDispatcher;
        public TestOneController(ILogger<TestOneController> logger, IConsulDispatcher consulDispatcher)
        {
            _logger = logger;
            _consulDispatcher = consulDispatcher;
        }
        #region http请求
        /// <summary>
        /// http请求获取用户信息
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            string url = "http://127.0.0.1:5007/Api/users/get?id=1";

            string result = HttpHelper.InvokeApi(url);

            User user = JsonConvert.DeserializeObject<User>(result);
            base.ViewBag.User = user;
            base.ViewBag.Url = url;
            ViewBag.Message = "OK";
            return View();
        }

        /// <summary>
        /// http请求获取用户列表
        /// </summary>
        /// <returns></returns>
        public IActionResult UserList()
        {
            string url = "http://127.0.0.1:5007/Api/users/All";

            string result = HttpHelper.InvokeApi(url);

            List<User> users = JsonConvert.DeserializeObject<List<User>>(result);
            base.ViewBag.User = users;
            base.ViewBag.Url = url;
            ViewBag.Message = "OK";
            return View(users);
        }
        #endregion



        public ActionResult GetAllConsulService()
        {
            using (ConsulClient client = new ConsulClient(c =>
            {
                c.Address = new Uri("http://localhost:8500/");
                c.Datacenter = "dc1";
            }))
            {
                var dictionary = client.Agent.Services().Result.Response;
                string message = "";
                foreach (var keyValuePair in dictionary)
                {
                    AgentService agentService = keyValuePair.Value;
                    this._logger.LogWarning($"{agentService.Address}:{agentService.Port} {agentService.ID} {agentService.Service}");//找的是全部服务 全部实例  其实可以通过ServiceName筛选
                    message += $"{agentService.Address}:{agentService.Port};";
                }
                //获取当前consul的全部服务
                base.ViewBag.Message = message;
            }

            return View();
        }

        public ActionResult GetUserInConsul()
        {
            string url = "http://test007_consul/api/users/get?id=1";      //test007_consul  consul设置的组名
            
            //consul解决使用服务名字 转换IP:Port----DNS
            string requestUrl = string.Empty;    //最终请求地址
            Uri uri = new Uri(url);
            string groupName = uri.Host;    //组名=test007_consul
            User user = new User();
            //策略  由starup注册 轮询还是权重
            var ipPort = _consulDispatcher.ChooseAddress(groupName);
            requestUrl = $"{uri.Scheme}://{ipPort}{uri.PathAndQuery}";
            string result = HttpHelper.InvokeApi(requestUrl);
            user = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(result);

            base.ViewBag.User = user;
            base.ViewBag.Url = requestUrl;
            return View();
        }

        public ActionResult GetListInConsul()
        {
            string url = "http://test007_consul/api/users/all";      //test007_consul  consul设置的组名

            //consul解决使用服务名字 转换IP:Port----DNS
            string requestUrl = string.Empty;    //最终请求地址
            Uri uri = new Uri(url);
            string groupName = uri.Host;    //组名=test007_consul
            List<User> userList = new List<User>();
            //策略  由starup注册 轮询还是权重
            var ipPort = _consulDispatcher.ChooseAddress(groupName);
            requestUrl = $"{uri.Scheme}://{ipPort}{uri.PathAndQuery}";
            string result = HttpHelper.InvokeApi(requestUrl);
            userList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<User>>(result);
            base.ViewBag.Url = requestUrl;
            return View(userList);
        }
    }

}