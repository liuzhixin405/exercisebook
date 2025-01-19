using System.Data.SqlClient;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webapi.Const;
using webapi.Dals.Impls;
using webapi.Exceptions;
using webapi.Helper;
using webapi.Utils;

namespace webapi.Services.Impl
{
    public class BQNotradetimeServiceImpl
    {
        private readonly BqNoTradetimeDalImpl _bqNoTradetimeDalImpl;
        private readonly BqStockSettingDalImpl _bqStockSettingDalImpl;
        public BQNotradetimeServiceImpl(BqNoTradetimeDalImpl bqNoTradetimeDalImpl, BqStockSettingDalImpl bqStockSettingDalImpl)
        {
            _bqNoTradetimeDalImpl = bqNoTradetimeDalImpl;
            _bqStockSettingDalImpl = bqStockSettingDalImpl;
        }



        private async Task<bool> TradeTime(long transactionId)
        {
            long currTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            long time = TimestampHelper.GetTimesMorning(currTime);

            bool sfzc = CacheHelper.Get<bool>(BTCQMemecacheKeys.Trade_Normal + "_" + transactionId + "_" + time);
            if (sfzc == null)
            {
                bool zcm = true;
                List<BqNotradetime> lstNotradetimes =(await _bqNoTradetimeDalImpl.GetList(x => x.HTime == time && transactionId == transactionId)).ToList();
                if (lstNotradetimes == null || lstNotradetimes.Count <= -0)
                {
                    zcm = true;
                }
                else
                {
                    zcm = false;
                }
                sfzc = zcm;
                CacheHelper.Set(BTCQMemecacheKeys.Trade_Normal + "_" + transactionId + "_" + time, sfzc, TimeSpan.FromSeconds(BTCQMemecacheKeys.TIMES.ONE_MIN));

            }
            if (!sfzc)
            {
                return false;
            }
            // 分钟开始
            long minuteStart = currTime / 1000 / 60 * 60;
            bool tradeTime = CacheHelper.Get<bool>(BTCQMemecacheKeys.STOCK_TRADE_TIME + transactionId + "_" + minuteStart);
            if (tradeTime != null)
            {
                return tradeTime;
            }

            BqStockSetting setting = CacheHelper.Get<BqStockSetting>(BTCQMemecacheKeys.STOCK_SETTING + "_" + transactionId);
            if (setting == null)
            {

                List<BqStockSetting> list =(await _bqStockSettingDalImpl.GetList(x => x.TransactionId == transactionId)).ToList();

                if (list != null && list.Count > 0)
                {
                    setting = list.First();
                }

                if (setting != null)
                    CacheHelper.Set(BTCQMemecacheKeys.STOCK_SETTING + "_" + transactionId, setting, TimeSpan.FromSeconds(BTCQMemecacheKeys.TIMES.ONE_MIN));
            }
            if (setting == null) return false;
            bool isTradeDay = false;
            DayOfWeek currentDayofWeek = DateTime.UtcNow.DayOfWeek;
            string tradeTimeStr = "";
            switch (currentDayofWeek)
            {
                case DayOfWeek.Sunday:
                    isTradeDay = setting.SunState;
                    tradeTimeStr = setting.SunTradeTime;
                    break;
                case DayOfWeek.Monday:
                    isTradeDay = setting.MonState;
                    tradeTimeStr = setting.MonTradeTime;
                    break;
                case DayOfWeek.Tuesday:
                    isTradeDay = setting.TueState;
                    tradeTimeStr = setting.TueTradeTime;
                    break;
                case DayOfWeek.Wednesday:
                    isTradeDay = setting.WedState;
                    tradeTimeStr = setting.WedTradeTime;
                    break;
                case DayOfWeek.Thursday:
                    isTradeDay = setting.ThuState;
                    tradeTimeStr = setting.ThuTradeTime;
                    break;
                case DayOfWeek.Friday:
                    isTradeDay = setting.FriState;
                    tradeTimeStr = setting.FriTradeTime;
                    break;
                case DayOfWeek.Saturday:
                    isTradeDay = setting.SatState;
                    tradeTimeStr = setting.SatTradeTime;
                    break;
                default:
                    throw new ParamsErrorException("未能获取正确星期");

            }
            sfzc = isTradeDay;
            if (!sfzc || string.IsNullOrWhiteSpace(tradeTimeStr))
            {
                CacheHelper.Set(BTCQMemecacheKeys.STOCK_TRADE_TIME + transactionId + "_" + minuteStart, false, TimeSpan.FromSeconds(BTCQMemecacheKeys.TIMES.TWO_MIN));
                return false;
            }
            string[] tradeTimArr = tradeTimeStr.Split(',');
            bool isTradeTime = false;
            foreach (string trTime in tradeTimArr)
            {
                isTradeTime = CheckTradeTime(trTime, minuteStart - time);
                if (isTradeTime) break;
            }
            CacheHelper.Set(BTCQMemecacheKeys.STOCK_TRADE_TIME + transactionId + "_" + minuteStart, isTradeTime, TimeSpan.FromSeconds(BTCQMemecacheKeys.TIMES.TWO_MIN));

            return isTradeTime;
        }

        private bool CheckTradeTime(string tradeTime, long todayTime)
        {
            bool isTrade = false;
            if (string.IsNullOrWhiteSpace(tradeTime))
            {
                return isTrade;
            }
            if (!tradeTime.Contains("-") || !tradeTime.Contains(":"))
                return isTrade;

            String startStr = tradeTime.Substring(0, tradeTime.IndexOf("-"));
            String endStr = tradeTime.Substring(tradeTime.IndexOf("-") + 1, tradeTime.Length);

            String[] startHourMinute = startStr.Split(":");
            if (startHourMinute.Length != 2)
            {
                return isTrade;
            }

            int hour = int.Parse(startHourMinute[0]);
            int minute = int.Parse(startHourMinute[1]);

            if (hour < 0 || hour > 24 || minute < 0 || minute > 60)
            {
                return isTrade;
            }

            long startTime = hour * 3600 + minute * 60;

            String[] endHourMinute = endStr.Split(":");
            if (endHourMinute.Length != 2)
            {
                return isTrade;
            }

            hour = int.Parse(endHourMinute[0]);
            minute = int.Parse(endHourMinute[1]);

            if (hour < 0 || hour > 24 || minute < 0 || minute > 60)
            {
                return isTrade;
            }

            long endTime = hour * 3600 + minute * 60;

            if (todayTime >= startTime && todayTime <= endTime)
            {
                isTrade = true;
            }

            return isTrade;
        }


        public Task<bool> IsTradeTime(long transactionId)
        {
            return TradeTime(transactionId);
        }

        public Task<bool> readTradeTime(long transactionId)
        {
            return TradeTime(transactionId);
        }

        public Task add(BqNotradetime noTrade)
        {
          return  _bqNoTradetimeDalImpl.Insert(noTrade);
        }

        public Task<BqNotradetime> getById(long id)
        {
            return _bqNoTradetimeDalImpl.Get(id);
        }

        public Task deleteById(long id)
        {

           return _bqNoTradetimeDalImpl.SoftDelete(id);
        }

        public async Task updateById(long id, long time)
        {
            var data =await _bqNoTradetimeDalImpl.Get(id);
            data.HTime = time;
           await _bqNoTradetimeDalImpl.Update(data);
        }

        public Task addList(List<BqNotradetime> lstNotrades)
        {
           return _bqNoTradetimeDalImpl.Insert(lstNotrades);
        }

        public async Task<Pagination<BqNotradetime>> findAllpage(long time, int type, long transactionId,int pageIndex=1,int pageSize=10)
        {
            Predicate<BqNotradetime> predicate = x => x.HType == type;
            var hasType = false;
            if (type!=null&&type > 0)
            {
                predicate = x => x.HType == type && x.HTime == time;
            }
            if(transactionId!=null && transactionId > 0)
            {
                if(hasType)
                predicate = x => x.HType == type && x.HTime == time&&transactionId==x.TransactionId;
                else
                    predicate = x => x.HType == type && transactionId == x.TransactionId;
            }

            var sort = new { HId = SortOrder.Descending };
            var result =(await _bqNoTradetimeDalImpl.GetListPaged(predicate, sort, pageIndex, pageSize)).ToList();
            return new Pagination<BqNotradetime>(result,pageIndex,pageSize);
        }

    }
}
