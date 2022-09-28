using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SeckillPro.Com.Tool;
using SeckillPro.Com.Model.CrmModel;
using SeckillPro.Com.Model;
using SeckillPro.Com.Common;
using System.Net.Http;
using SeckillPro.Com.Model.ApiModel;
using Newtonsoft.Json;

namespace SeckillPro.Web.Controllers
{
    public class HomeController : Controller
    {
        //redis引用
        private StackRedis _redis = StackRedis.Current;

        /// <summary>
        /// 用户抢购商品列表
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            #region  使用ip模拟登录账号
            var token = "Sid_" + HttpContext.Connection.RemoteIpAddress.ToString();
            var sessionData = await _redis.Get<MoUserInfo>(token);
            if (sessionData == null || sessionData.UserId <= 0)
            {
                //用户基本信息
                var moUser = new MoUserInfo();
                moUser.UserId = await DataKeyHelper.Current.GetKeyId(EnumHelper.EmDataKey.UserId);
                moUser.NickName = token;
                //redis存储session，默认30分钟失效
                var isLogin = await _redis.Set<MoUserInfo>(token, moUser, 30);
                if (isLogin)
                {
                    ViewData["MoUser"] = moUser;
                    //加入cookie
                    Response.Cookies.Append(EnumHelper.EmDataKey.SessionKey.ToString(), token, new Microsoft.AspNetCore.Http.CookieOptions
                    {
                        Expires = DateTime.Now.AddMinutes(30),
                        HttpOnly = true
                    });
                }
            }
            else
            {
                ViewData["MoUser"] = sessionData;
                //已经是登陆状态，需要重新设置失效时间
                var isLogin = await _redis.Set<MoUserInfo>(token, sessionData, 30);
                Response.Cookies.Append(EnumHelper.EmDataKey.SessionKey.ToString(), token, new Microsoft.AspNetCore.Http.CookieOptions
                {
                    Expires = DateTime.Now.AddMinutes(30),
                    HttpOnly = true
                });
            }
            #endregion

            //商品列表
            var shoppings = await _redis.GetHashsToList<MoShopping>(EnumHelper.EmDataKey.ShoppingHash.ToString());
            Response.Headers.Add("PageCache-Time", $"{60 * 2}");  //2分钟
            return View(shoppings);
        }

        [HttpGet]
        public async Task<IActionResult> Qiang(int? id)
        {
            if (id == null) { return BadRequest(); }

            var shop = await _redis.GetHashField<MoShopping>(EnumHelper.EmDataKey.ShoppingHash.ToString(), id.ToString());
            if (shop == null) { return NotFound(); }
            return View(shop);
        }

        [HttpPost]
        public async Task<IActionResult> Qiang(MoKaiQiang qiang)
        {

            #region 基础验证
            if (qiang == null || qiang.ShopId <= 0) { return RedirectToAction("Error", new { msg = "操作太快，请稍后重试。" }); }

            if (!Request.Cookies.TryGetValue(EnumHelper.EmDataKey.SessionKey.ToString(), out string token))
            {
                return RedirectToAction("Error", new { msg = "请先去登录。" });
            }
            var sessionData = await _redis.Get<MoUserInfo>(token);
            if (sessionData == null || sessionData.UserId <= 0) { return RedirectToAction("Error", new { msg = "请先去登录！" }); }

            var shop = await _redis.GetHashField<MoShopping>(EnumHelper.EmDataKey.ShoppingHash.ToString(), qiang.ShopId.ToString());
            if (shop == null) { return NotFound(); }
            else if (shop.MaxNum <= 0)
            {
                return RedirectToAction("Error", new { msg = $"你太慢了，商品：{shop.Name}，已经被抢完了！" });
            }
            else if (shop.MaxNum < qiang.Num)
            {
                return RedirectToAction("Error", new { msg = $"库存不足，商品：{shop.Name}，只剩{shop.MaxNum}了！" });
            }
            else if (shop.MaxGouNum < qiang.Num)
            {
                return RedirectToAction("Error", new { msg = $"一个账号每次最多只能抢购【{shop.Name}】{shop.MaxGouNum}件。" });
            }

            #endregion

            #region 请求抢购商品的分布式接口
            var rq = new MoQiangGouRq();
            rq.Num = qiang.Num;
            rq.ShoppingId = qiang.ShopId;
            rq.MemberRq = new MoMemberRq
            {
                Ip = HttpContext.Connection.RemoteIpAddress.ToString(),  //用户Ip
                RqSource = (int)EnumHelper.EmRqSource.Web,
                Token = token
            };
            var strRq = JsonConvert.SerializeObject(rq);
            var content = new StringContent(strRq, System.Text.Encoding.UTF8, "application/json");

            //基础接口地址
           // string apiBaseUrl = $"http://{HttpContext.Connection.LocalIpAddress}:4545";
            string apiBaseUrl = $"http://localhost:4545";
            var qiangApiUrl = $"{apiBaseUrl}/api/order/SubmitQiangGouOrder";
            var strRp = await HttpTool.HttpPostAsync(qiangApiUrl, content, 30);
            if (string.IsNullOrWhiteSpace(strRp))
            {
                return RedirectToAction("Error", new { msg = $"抢单超时，请查看你的订单列表是否抢单成功。" });
            }
            var rp = JsonConvert.DeserializeObject<MoQiangGouRp>(strRp);
            if (rp == null)
            {
                return RedirectToAction("Error", new { msg = $"抢单超时，请查看你的订单列表是否抢单成功。" });
            }
            else if (rp.RpStatus != 1)
            {
                return Error(rp.RpMsg);
            }
            #endregion

            return RedirectToAction("QiangResult", new { id = rp.OrderId });
        }

        /// <summary>
        /// 抢购结果
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> QiangResult(long? id)
        {
            if (id == null) { return BadRequest(); }
            if (!Request.Cookies.TryGetValue(EnumHelper.EmDataKey.SessionKey.ToString(), out string token))
            {
                return RedirectToAction("Error", new { msg = $"请先去登录" });
            }

            var rq = new MoOrderDetailRq();
            rq.OrderId = Convert.ToInt64(id);
            rq.MemberRq = new MoMemberRq { Token = token };

            var strRq = JsonConvert.SerializeObject(rq);
            var content = new StringContent(strRq, System.Text.Encoding.UTF8, "application/json");

            //基础接口地址
            string apiBaseUrl = $"http://localhost:4545";
            var qiangApiUrl = $"{apiBaseUrl}/api/order/GetOrderDetail";
            var strRp = await HttpTool.HttpPostAsync(qiangApiUrl, content, 30);
            if (string.IsNullOrWhiteSpace(strRp))
            {
                return RedirectToAction("Error", new { msg = "查询失败，请稍后重试。" });
            }
            var rp = JsonConvert.DeserializeObject<MoOrderDetailRp>(strRp);
            if (rp == null) { return RedirectToAction("Error", new { msg = $"查询失败，请稍后重试！" }); }
            else if (rp.RpStatus != 1)
            {
                return RedirectToAction("Error", new { msg = rp.RpMsg });
            }
            return View(rp);
        }

        /// <summary>
        /// 订单列表
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OrderList()
        {
            if (!Request.Cookies.TryGetValue(EnumHelper.EmDataKey.SessionKey.ToString(), out string token))
            {
                return RedirectToAction("Error", new { msg = "请先去登录。" });
            }
            var sessionData = await _redis.Get<MoUserInfo>(token);
            if (sessionData == null || sessionData.UserId <= 0) { return RedirectToAction("Error", new { msg = "请先去登录！" }); }

            var orderList = await _redis.GetHashsToList<MoOrderInfo>($"User_{sessionData.UserId}");
            return View(orderList);
        }

        #region 商品后台管理
        /// <summary>
        /// 商品后台管理列表（后台人员设置）
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> ShoppingManage()
        {
            //获取原有商品
            var shoppings = await _redis.GetHashsToList<MoShopping>(EnumHelper.EmDataKey.ShoppingHash.ToString());
            return View(shoppings);
        }

        #region 添加商品
        [HttpGet]
        public IActionResult ShoppingAdd()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ShoppingAdd(MoShopping shopping)
        {

            //加入商品列表
            shopping.Id = await DataKeyHelper.Current.GetKeyId(EnumHelper.EmDataKey.ShoppingId);
            shopping.MaxNum = shopping.MaxNum < 0 ? 0 : shopping.MaxNum;
            shopping.MaxGouNum = shopping.MaxGouNum <= 0 ? 1 : shopping.MaxGouNum;
            shopping.Url = string.IsNullOrWhiteSpace(shopping.Url) ? "https://ss1.bdstatic.com/70cFvXSh_Q1YnxGkpoWK1HF6hhy/it/u=112998166,256272819&fm=117&gp=0.jpg" : shopping.Url;
            var result = await _redis.SetOrUpdateHashsField<MoShopping>(EnumHelper.EmDataKey.ShoppingHash.ToString(), shopping.Id.ToString(), shopping);
            if (result > 0) { return RedirectToAction("ShoppingManage"); }
            return View(shopping);
        }
        #endregion

        #endregion

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error(string msg = "怎么就迷路了呢。")
        {
            ViewData["msg"] = msg;
            return View();
        }
    }

    public class MoKaiQiang
    {
        public long ShopId { get; set; }
        public int Num { get; set; }
    }
}
