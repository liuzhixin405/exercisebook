using SeckillPro.Com.Model;
using SeckillPro.Com.Model.CrmModel;
using SeckillPro.Com.Tool;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

namespace SeckillPro.Server
{
    class Program
    {
        //redis引用
        private static StackRedis _redis = StackRedis.Current;
        //控制重复任务
        private static Dictionary<long, long> _dicTask = new Dictionary<long, long>();
        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Console.OutputEncoding = Encoding.GetEncoding("GB2312");

            try
            {
                Console.WriteLine("是否开启处理抢购队列（Y）：");
                Console.ReadLine();
                Console.WriteLine($"开启抢购任务监控中...");
                var shopsKey = EnumHelper.EmDataKey.ShoppingHash.ToString();
                while (true)
                {
                    //匹配出QiangOrderEqueue_xxx格式的抢单队列keys
                    var matchKey = $"{EnumHelper.EmDataKey.QiangOrderEqueue.ToString()}_*";
                    var matches = _redis.MatchKeys(matchKey).Result;
                    var matchLen = matches.Count;
                    if (matchLen <= 0) { continue; }

                    //根据key获取对应的商品，并加载对应的商品抢单任务处理
                    foreach (var item in matches)
                    {
                        var itemArr = item.Split('_');
                        if (itemArr.Length <= 1) { continue; }
                        var shopId = itemArr[1];
                        var shop = _redis.GetHashField<MoShopping>(shopsKey, shopId).Result;
                        if (shop == null || _dicTask.ContainsKey(shop.Id)) { continue; }

                        //加入排重任务dic
                        _dicTask.Add(shop.Id, shop.Id);

                        Task.Factory.StartNew(async b =>
                        {
                            var equeueShop = b as MoShopping;
                            var equeueKey = $"{EnumHelper.EmDataKey.QiangOrderEqueue.ToString()}_{equeueShop.Id}";
                            var sbTaskLog = new StringBuilder(string.Empty);
                            try
                            {
                                sbTaskLog.AppendFormat("商品【{0}-{1}】，开启抢购队列处理；", equeueShop.Name, equeueShop.Id);
                                Console.WriteLine(sbTaskLog);

                                //监控队列key是否存在
                                while (await _redis.KeyExists(equeueKey))
                                {
                                    //获取队列
                                    var qiangOrder = await _redis.GetListAndPop<MoOrderInfo>(equeueKey);
                                    if (qiangOrder == null) { continue; }

                                    //获取真实剩余库存
                                    var equShop = await _redis.GetHashField<MoShopping>(shopsKey, equeueShop.Id.ToString());
                                    if (equShop == null) { continue; }

                                    var sbLog = new StringBuilder(string.Empty);
                                    Stopwatch watch = new Stopwatch();
                                    watch.Start();
                                    try
                                    {
                                        #region 逻辑处理库存

                                        sbLog.AppendFormat("用户：{0}抢购商品【{1}-{4}】当前库存：{2}件，抢购数：{3}件，",
                                                                    qiangOrder.UserId,
                                                                    equShop.Name,
                                                                    equShop.MaxNum,
                                                                    qiangOrder.Num,
                                                                    equShop.Id);
                                        if (equShop.MaxNum <= 0)
                                        {
                                            //无库存，直接抢购失败
                                            qiangOrder.OrderStatus = (int)EnumHelper.EmOrderStatus.抢购失败;
                                        }
                                        else if (equShop.MaxNum < qiangOrder.Num)
                                        {
                                            //剩余库存小于抢购数量
                                            qiangOrder.OrderStatus = (int)EnumHelper.EmOrderStatus.抢购失败;
                                        }
                                        else if (equShop.MaxGouNum < qiangOrder.Num)
                                        {
                                            //最大允许抢购数量小于抢购申请数量
                                            qiangOrder.OrderStatus = (int)EnumHelper.EmOrderStatus.抢购失败;
                                        }
                                        else
                                        {
                                            //库存充足
                                            equShop.MaxNum = equShop.MaxNum - qiangOrder.Num;
                                            //扣除当前抢购数量后，更新库存
                                            var isOk = await _redis.SetOrUpdateHashsField<MoShopping>(shopsKey, equShop.Id.ToString(), equShop, false) > 0;
                                            if (!isOk)
                                            {
                                                qiangOrder.OrderStatus = (int)EnumHelper.EmOrderStatus.抢购失败;
                                            }
                                            else
                                            {
                                                qiangOrder.OrderStatus = (int)EnumHelper.EmOrderStatus.抢购成功;
                                            }
                                        }
                                        #endregion
                                    }
                                    catch (Exception ex)
                                    {
                                        sbLog.AppendFormat("异常信息：{0},", ex.Message);
                                    }
                                    finally
                                    {
                                        sbLog.AppendFormat("库存剩余：{0}件，抢购订单状态：{1}，",
                                            equeueShop.MaxNum,
                                            Enum.GetName(typeof(EnumHelper.EmOrderStatus), qiangOrder.OrderStatus));

                                        //更新当前订单抢购状态
                                        var isQiangOrder = await _redis.SetOrUpdateHashsField<MoOrderInfo>($"User_{qiangOrder.UserId}", qiangOrder.OrderId.ToString(), qiangOrder, false);
                                        watch.Stop();
                                        sbLog.AppendFormat("更新订单状态：{0}；处理总耗时：{1}ms。", isQiangOrder > 0, watch.ElapsedMilliseconds);
                                        Console.WriteLine(sbLog);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                sbTaskLog.AppendFormat("异常信息：{0}；", ex.Message);
                            }
                            finally
                            {
                                //任务结束时去掉排重任务dic记录
                                if (_dicTask.ContainsKey(equeueShop.Id)) { _dicTask.Remove(equeueShop.Id); }

                                sbTaskLog.Append("处理抢购订单队列结束。");
                                Console.WriteLine(sbTaskLog);
                            }
                        }, shop);
                        //Console.WriteLine($"商品【{shop.Name}-{shop.Id}】开启处理订单队列任务;");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("全局异常信息：" + ex.Message);
            }
            Console.WriteLine("温馨提示：按住任意键即可退出！");
            Console.ReadLine();
        }
    }
}