using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SeckillPro.Com.Model.ApiModel;
using SeckillPro.Com.Tool;
using SeckillPro.Com.Model.CrmModel;
using SeckillPro.Com.Model;
using SeckillPro.Com.Common;
using System.IO;

namespace SeckillPro.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    public class OrderController : Controller
    {
        //redis引用
        private StackRedis _redis = StackRedis.Current;

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        /// <summary>
        /// 抢单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<MoQiangGouRp> SubmitQiangGouOrder()
        {
            var rp = new MoQiangGouRp { RpMsg = EnumHelper.EmOrderStatus.抢购失败.ToString() };
            try
            {
                var strRq = string.Empty;
                using (var stream = Request.Body)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        strRq = await reader.ReadToEndAsync();
                    }
                }
                if (string.IsNullOrWhiteSpace(strRq)) { return rp; }

                var rq = JsonConvert.DeserializeObject<MoQiangGouRq>(strRq);
                if (rq.ShoppingId <= 0 || rq.MemberRq == null) { return rp; }

                #region 验证

                //登录验证
                if (string.IsNullOrWhiteSpace(rq.MemberRq.Token))
                {
                    rp.RpMsg = $"未登录，请登录后重试。";
                    return rp;
                }
                var sessionData = await _redis.Get<MoUserInfo>(rq.MemberRq.Token);
                if (sessionData == null)
                {
                    rp.RpMsg = $"登录失效，请重新登录！";
                    return rp;
                }

                //库存验证
                var shopsKey = EnumHelper.EmDataKey.ShoppingHash.ToString();
                var shop = await _redis.GetHashField<MoShopping>(shopsKey, rq.ShoppingId.ToString());
                if (shop == null) { return rp; }
                else if (shop.MaxNum <= 0)
                {
                    rp.RpMsg = $"你太慢了，商品：{shop.Name}，已经被抢完了！";
                    return rp;
                }
                else if (shop.MaxNum < rq.Num)
                {
                    rp.RpMsg = $"库存不足，商品：{shop.Name}，只剩{shop.MaxNum}了！";
                    return rp;
                }
                else if (shop.MaxGouNum < rq.Num)
                {
                    rp.RpMsg = $"一个账号每次最多只能抢购【{shop.Name}】{shop.MaxGouNum}件。";
                    return rp;
                }
                #endregion

                #region 加入订单list中

                var orderInfo = new MoOrderInfo();
                orderInfo.OrderId = await DataKeyHelper.Current.GetKeyId(EnumHelper.EmDataKey.OrderId);
                orderInfo.OrderStatus = (int)EnumHelper.EmOrderStatus.排队抢购中;
                orderInfo.CreatTime = DateTime.Now;
                orderInfo.Num = rq.Num;
                orderInfo.ShoppingId = rq.ShoppingId;
                orderInfo.UserId = sessionData.UserId;
                orderInfo.MoShopping = shop;
                //记录所有订单

                //记录会员名下的订单
                var isAddOrder = await _redis.SetOrUpdateHashsField<MoOrderInfo>($"User_{orderInfo.UserId}", orderInfo.OrderId.ToString(), orderInfo);
                if (isAddOrder <= 0)
                {
                    return rp;
                }
                rp.OrderId = orderInfo.OrderId;
                rp.OrderStatus = orderInfo.OrderStatus;
                rp.CreatTime = orderInfo.CreatTime;
                rp.RpStatus = 1;
                rp.RpMsg = EnumHelper.EmOrderStatus.排队抢购中.ToString();
                #endregion

                #region 各种验证无误后，加入抢购队列，并返回抢购中...

                var isAddQiangQueue = await _redis.SetList<MoOrderInfo>($"{EnumHelper.EmDataKey.QiangOrderEqueue.ToString()}_{orderInfo.ShoppingId}", orderInfo);

                #endregion
            }
            catch (Exception ex)
            {
                rp.RpMsg = "抢购活动正在高峰期，请稍后重试";
            }
            return rp;
        }

        [HttpPost]
        public async Task<MoOrderDetailRp> GetOrderDetail()
        {
            var rp = new MoOrderDetailRp();
            try
            {
                //获取请求参数
                var strRq = string.Empty;
                using (var stream = Request.Body)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        strRq = await reader.ReadToEndAsync();
                    }
                }
                if (string.IsNullOrWhiteSpace(strRq)) { return rp; }

                var rq = JsonConvert.DeserializeObject<MoOrderDetailRq>(strRq);

                #region 验证

                #region 登录验证
                if (rq.MemberRq == null || string.IsNullOrWhiteSpace(rq.MemberRq.Token))
                {
                    rp.RpMsg = $"未登录，请登录后重试。";
                    return rp;
                }
                var sessionData = await _redis.Get<MoUserInfo>(rq.MemberRq.Token);
                if (sessionData == null)
                {
                    rp.RpMsg = $"登录失效，请重新登录！";
                    return rp;
                }
                #endregion

                if (rq.OrderId <= 0)
                {
                    rp.RpMsg = $"参数不正确！";
                    return rp;
                }
                #endregion

                var orderDetail = await _redis.GetHashField<MoOrderInfo>($"User_{sessionData.UserId}", rq.OrderId.ToString());
                if (orderDetail == null)
                {
                    rp.RpMsg = $"订单查询失败，无此订单！";
                    return rp;
                }

                rp.OrderId = orderDetail.OrderId;
                rp.Num = orderDetail.Num;
                rp.OrderStatus = orderDetail.OrderStatus;
                rp.PayOutTime = orderDetail.PayOutTime;
                rp.CreatTime = orderDetail.CreatTime;
                rp.ShoppingId = orderDetail.ShoppingId;

                var shop = await _redis.GetHashField<MoShopping>(EnumHelper.EmDataKey.ShoppingHash.ToString(), rp.ShoppingId.ToString());
                if (shop == null)
                {
                    rp.RpMsg = $"订单查询失败，请稍后重试！";
                    return rp;
                }
                rp.MoShopping = shop;
                rp.RpStatus = 1;
                rp.RpMsg = "订单查询成功";
            }
            catch (Exception ex)
            {
                rp.RpMsg = "查询超时，请稍后重试";
            }
            return rp;
        }
    }
}