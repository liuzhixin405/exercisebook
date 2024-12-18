using DapperDal.Expressions;
using DapperDal.Predicate;
using System.Text;
using webapi.Const;
using webapi.Dals.Impls;
using webapi.Utils;

namespace webapi.Services.Impl
{
    public class BqAccountAmountLogServiceImpl : BqBaseService<BqAccountAmountLogDalImpl>
    {
        public BqAccountAmountLogServiceImpl(IConfiguration configuration) : base(configuration)
        {
        }

        protected override BqAccountAmountLogDalImpl CreateInstance(string config)
        {
            return new BqAccountAmountLogDalImpl(config);
        }

        public void SaveAmountLog(BqAccountAmountLog accountAmountLog)
        {
            instance.Insert(accountAmountLog);
        }


        public Pagination<BqAccountAmountLog> ReadListRechargeWithdraw(long id)
        {
            StringBuilder sbBuilder = new StringBuilder();
            var result = instance.GetList(x => x.LogId == id).OrderByDescending(x => x.LogId).ToList();

            return new Pagination<BqAccountAmountLog>(result,1,10);
        }

        public List<BqAccountAmountLog> ReadByUserAndTime(long uid, long from, String direct, long startTime, long endTime,
                int limit)
        {
            var predicate = PredicateBuilder.True<BqAccountAmountLog>();
           
          
           
            if (uid > 0)
            {
               predicate = predicate.And(x => x.LogId == uid);
            }

            if (startTime > 0)
            {
                predicate = predicate.And(x => x.Dateline >= startTime);
            }

            if (endTime > 0)
            {
                predicate = predicate.And(x => x.Dateline < endTime);
            }
            if (from >= 0 && "prev".Equals(direct))
            {
                predicate = predicate.And(x => x.LogId >= from);
            }
            else if (from >= 0 && "next".Equals(direct))
            {
                predicate = predicate.And(x => x.LogId <= from);
            }
            var sort = new Sort { PropertyName = "LogId", Ascending = false };
            List<BqAccountAmountLog> result = instance.GetList(predicate,sort).ToList();

            return result;
        }

        
        public void CreateAccountAmountLogBak()
        {
            instance.Execute("create table if not exists bq_account_amount_log_bak like bq_account_amount_log;");
        }

      
        public void insertAccountAmountLogBak(long endTime)
        {
          
            instance.Execute("insert into bq_account_amount_log_bak select * from bq_account_amount_log\"\r\n\t\t\t\t\t\t+ \" where operation <> " + Contents.TRANS_TYPE_TO + " and operation <> " + Contents.TRANS_TYPE_OUT + " and dateline < " + endTime);
            instance.Execute("DELETE FROM bq_account_amount_log WHERE operation <> " + Contents.TRANS_TYPE_TO + " AND operation <> " + Contents.TRANS_TYPE_OUT + " AND dateline < {0}",endTime);
        }

    public Pagination<BqAccountAmountLog> ReadOTCRecords(long uid, int currencyId, int operation)
        {
            var predicate = PredicateBuilder.True<BqAccountAmountLog>();
            predicate=predicate.And(x => x.CurrencyTypeId == currencyId);
  
            if (uid > 0)
            {
                predicate=predicate.And(x=>x.LogId == uid);
            }

            if (operation == 1)
            {
              
                predicate = predicate.And(x => x.Operation == Contents.TRANS_TYPE_TOOTC);
            }
            else if (operation == 2)
            {
                predicate = predicate.And(x => x.Operation == Contents.TRANS_TYPE_OUTOTC);
            }
            else
            {
              
                predicate = predicate.And(x => x.Operation == Contents.TRANS_TYPE_OUTOTC || x.Operation== Contents.TRANS_TYPE_TOOTC);
            }

            var sort = new Sort { PropertyName = "LogId", Ascending = false };
            List<BqAccountAmountLog> result = instance.GetList(predicate, sort).ToList();
            return new Pagination<BqAccountAmountLog>(result,1,10);
        }
    }
}
