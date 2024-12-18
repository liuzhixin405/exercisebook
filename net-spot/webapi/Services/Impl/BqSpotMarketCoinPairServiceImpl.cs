using DapperDal.Expressions;
using DapperDal.Predicate;
using webapi.Dals.Impls;
using webapi.Utils;

namespace webapi.Services.Impl
{
    public class BqSpotMarketCoinPairServiceImpl : BqBaseService<BqSpotMarketCoinPairDalImpl>, IBqSpotMarketCoinPairService
    {
        public BqSpotMarketCoinPairServiceImpl(IConfiguration configuration) : base(configuration)
        {
        }

        protected override BqSpotMarketCoinPairDalImpl CreateInstance(string config)
        {
            return new BqSpotMarketCoinPairDalImpl(config);
        }

        public void createTable(string coinPairName)
        {
            String trade_sql = "CREATE TABLE IF NOT EXISTS " + coinPairName + "_member_trade (" +
               "id int(10) unsigned NOT NULL AUTO_INCREMENT COMMENT '会员与交易对应关系ID'," +
               "f_user_id int(10) unsigned NOT NULL COMMENT '用户ID'," +
               "trade_id int(12) unsigned NOT NULL DEFAULT '0' COMMENT '交易ID'," +
               "dateline int(10) unsigned NOT NULL DEFAULT '0' COMMENT '交易时间'," +
               "  PRIMARY KEY (id)," +
               "  UNIQUE KEY uniq_user_trade (f_user_id,trade_id,dateline) USING BTREE," +
               "  KEY idx_dateline (dateline)" +
               ") ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='会员交易记录关系表(多对多),一个交易，需要将进行这个交易的双方与交易的对应关系都写入此表一份';";

            String trade_detail_sql = "CREATE TABLE IF NOT EXISTS " + coinPairName + "_member_trade_detail (" +
           "id int(12) unsigned NOT NULL AUTO_INCREMENT," +
           "f_buy_trade_id int(12) unsigned NOT NULL COMMENT '买方对应的交易ID'," +
           "f_buy_uid int(10) unsigned NOT NULL DEFAULT '0' COMMENT '买家uid'," +
           "f_buy_user_email char(40) NOT NULL DEFAULT '' COMMENT '买家用户邮箱'," +
           "f_sell_uid int(10) unsigned NOT NULL DEFAULT '0' COMMENT '卖家uid'," +
           "f_sell_user_email char(40) NOT NULL DEFAULT '' COMMENT '卖家用户邮箱'," +
           "f_sell_trade_id int(10) unsigned NOT NULL COMMENT '卖方对应的交易id'," +
           "trade_flag smallint(5) NOT NULL DEFAULT '1' COMMENT '交易类型标识(1:普通交易, 2:交割 3:强平)'," +
           "trade_trans_dirction smallint(5) NOT NULL DEFAULT '1' COMMENT '这笔交易的成交方向(1:买 2:卖)'," +
           "trade_price decimal(18,8) NOT NULL DEFAULT '0.00' COMMENT '成交价格'," +
           "trade_count decimal(18,8) NOT NULL DEFAULT '0' COMMENT '成交数量'," +
           "trade_time int(10) unsigned NOT NULL COMMENT '成交时间,unix时间格式'," +
           "coin_pair_id smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '交易对id'," +
           "coin_pair_name varchar(10) NOT NULL DEFAULT '' COMMENT '交易对名称'," +
           " PRIMARY KEY (id)," +
           " KEY idx_tradetime (trade_time) USING BTREE," +
           " KEY idx_fbuyuid_fselluid_tradetime (f_buy_uid, f_sell_uid, trade_time) USING BTREE" +
           ") ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='交易详情表';";

            String delegation_sql = "CREATE TABLE IF NOT EXISTS " + coinPairName + "_pending_delegation (" +
           "delegation_id int(10) unsigned NOT NULL AUTO_INCREMENT COMMENT '委托编号'," +
           "pending_no bigint(19) NOT NULL COMMENT '挂单流水号'," +
           "delegation_time int(10) unsigned NOT NULL DEFAULT '0' COMMENT '委托时间,unix时间格式'," +
           "coin_pair_id smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '交易对id'," +
           "coin_pair_name varchar(10) NOT NULL DEFAULT '' COMMENT '交易对名称'," +
           "direction smallint(2) NOT NULL DEFAULT '0' COMMENT '操作方向(1:买 2:卖)'," +
           "delegation_count decimal(18,8) unsigned NOT NULL DEFAULT '0.00' COMMENT '委托数量'," +
           "delegation_price decimal(18,8) unsigned NOT NULL DEFAULT '0.00' COMMENT '委托价格'," +
           "deal_count decimal(18,8) NOT NULL DEFAULT '0.00' COMMENT '已成交数量'," +
           "undeal_count decimal(18,8) NOT NULL DEFAULT '0.00' COMMENT '未成交数量'," +
           "commission_rate decimal(18,8) NOT NULL DEFAULT '0.00' COMMENT '佣金比例'," +
           "f_user_id int(10) unsigned NOT NULL DEFAULT '0' COMMENT '用户ID'," +
           "flag smallint(5) NOT NULL DEFAULT '0' COMMENT '0默认，1交易中，2撤单中（其他）'," +
           "`f_frozenremain` decimal(18,8) NOT NULL COMMENT '冻结备用金'," +
           " PRIMARY KEY (`delegation_id`), KEY `idx_f_user_id` (`f_user_id`), KEY `idx_direction` (`direction`) USING BTREE" +
           ") ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='委托挂单表（非全部成交）';";

            String delegation_cache_sql = "CREATE TABLE IF NOT EXISTS " + coinPairName + "_pending_delegation_cache (" +
           "delegation_id int(10) unsigned NOT NULL AUTO_INCREMENT COMMENT '委托编号'," +
           "pending_no bigint(19) NOT NULL COMMENT '挂单流水号'," +
           "delegation_time int(10) unsigned NOT NULL COMMENT '委托时间,unix时间格式'," +
           "coin_pair_id smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '交易对id'," +
           "coin_pair_name varchar(10) NOT NULL DEFAULT '' COMMENT '交易对名称'," +
           "direction smallint(2) NOT NULL DEFAULT '0' COMMENT '操作方向(1:买 2:卖)'," +
           "delegation_count decimal(18,8) unsigned NOT NULL DEFAULT '0.00' COMMENT '委托数量'," +
           "delegation_price decimal(18,8) unsigned NOT NULL DEFAULT '0' COMMENT '委托价格'," +
           "deal_count decimal(18,8) NOT NULL DEFAULT '0.00' COMMENT '已成交数量'," +
           "undeal_count decimal(18,8) NOT NULL DEFAULT '0.00' COMMENT '未成交数量'," +
           "commission_rate decimal(18,8) NOT NULL DEFAULT '0.00' COMMENT '佣金比例'," +
           "f_user_id int(10) unsigned NOT NULL DEFAULT '0' COMMENT '用户ID'," +
           "flag smallint(5) NOT NULL DEFAULT '0' COMMENT '0默认，1交易中，2撤单中（其他）'," +
           "`f_frozenremain` decimal(18,8) NOT NULL COMMENT '冻结备用金'," +
           " PRIMARY KEY (delegation_id)," +
           "KEY idx_f_user_id (f_user_id)" +
           ") ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='委托挂单表（非全部成交）';";

            String plan_delegation_sql = "CREATE TABLE IF NOT EXISTS " + coinPairName + "_plan_pending_delegation (" +
             "id INT(11) NOT NULL AUTO_INCREMENT," +
             "f_code VARCHAR(45) NOT NULL DEFAULT '' COMMENT '合约号'," +
             "f_user_id INT(11) NOT NULL DEFAULT '0' COMMENT '用户ID'," +
             "tri_price DECIMAL(18,8) NOT NULL DEFAULT '0.00000000' COMMENT '触发价格'," +
             "delegation_price DECIMAL(18,8) NOT NULL DEFAULT '0.00000000' COMMENT '委托价格'," +
             "delegation_count DECIMAL(18,8) NOT NULL DEFAULT '0.00000000' COMMENT '委托数量'," +
             "delegation_time INT(10) NOT NULL DEFAULT 0 COMMENT '委托时间'," +
             "multiple SMALLINT(4) NOT NULL DEFAULT '1' COMMENT '杆杠倍数'," +
             "direction SMALLINT(4) NOT NULL DEFAULT '0' COMMENT '操作方向'," +
             "type SMALLINT(4) NOT NULL DEFAULT '1' COMMENT '类型（1-现价大于触发价时进行委托，2-现价低于触发价时进行委托）'," +
             "status SMALLINT(4) NOT NULL DEFAULT '0' COMMENT '状态（0-未生效，1-已生效，2-失败，3-已撤销）'," +
             "note VARCHAR(64) NOT NULL DEFAULT '' COMMENT '备注'," +
             "PRIMARY KEY (id), " +
             "KEY idx_fuserid_delegationtime (f_user_id,delegation_time) USING BTREE, " +
             "KEY idx_triprice_type (tri_price,type) USING BTREE " +
           ") ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT = '计划委托表';";

            String pending_log_sql = "CREATE TABLE IF NOT EXISTS " + coinPairName + "_pending_log (" +
           "log_id int(12) unsigned NOT NULL AUTO_INCREMENT," +
           "delegation_id int(10) unsigned NOT NULL DEFAULT '0' COMMENT '委托编号'," +
           "pending_no bigint(19) NOT NULL COMMENT '挂单流水号'," +
           "delegation_time int(10) unsigned NOT NULL COMMENT '委托时间,unix时间格式'," +
           "coin_pair_id smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '交易对id'," +
           "coin_pair_name varchar(10) NOT NULL DEFAULT '' COMMENT '交易对名称'," +
           "direction smallint(2) NOT NULL DEFAULT '0' COMMENT '操作方向(1:买/2:卖/)'," +
           "delegation_count decimal(18,8) unsigned NOT NULL DEFAULT '0.00' COMMENT '委托数量'," +
           "delegation_price decimal(18,8) unsigned NOT NULL DEFAULT '0.00' COMMENT '委托价格'," +
           "deal_count decimal(18,8) unsigned NOT NULL DEFAULT '0.00' COMMENT '已成交数量'," +
           "undeal_count decimal(18,8) NOT NULL DEFAULT '0.00' COMMENT '未成交数量'," +
           "action smallint(2) NOT NULL DEFAULT '1' COMMENT '1:成交/2:结算/3:强平/4:撤单'," +
           "dateline int(10) unsigned NOT NULL COMMENT '操作时间,unix时间格式'," +
           "f_user_id int(10) unsigned NOT NULL DEFAULT '0' COMMENT '用户ID'," +
           "avg_price decimal(18,8) unsigned NOT NULL DEFAULT '0.00' COMMENT '成交均价'," +
           "income decimal(18,8) NOT NULL DEFAULT '0.00' COMMENT '盈亏'," +
           "fee decimal(18,8) NOT NULL DEFAULT '0.00' COMMENT '手续费'," +
           "bfx_fee decimal(18,8) NOT NULL DEFAULT '0.00' COMMENT 'bfx抵扣手续费'," +
           "device smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '下单设备（0-PC，1-App，2-Api）'," +
           "ip_addr char(15) NOT NULL DEFAULT '' COMMENT '下单IP'," +
           "flag smallint(5) NOT NULL DEFAULT '0' COMMENT '0表示正常挂单日志，1标识交易异常时转过来的挂单'," +
           "  PRIMARY KEY (log_id)," +
           "  KEY idx_fuserid_action_dateline (f_user_id, action, dateline)," +
           "  KEY idx_pendingno_fuserid_dateline (pending_no,f_user_id,dateline) USING BTREE" +
           ") ENGINE=InnoDB DEFAULT CHARSET=utf8;";

            String trade_log_sql = "CREATE TABLE IF NOT EXISTS " + coinPairName + "_trade_log (" +
           "trade_id int(12) unsigned NOT NULL AUTO_INCREMENT COMMENT '交易ID'," +
           "coin_pair_id smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '交易对id'," +
           "coin_pair_name varchar(10) NOT NULL DEFAULT '' COMMENT '交易对名称'," +
           "trade_trans_dirction smallint(2) unsigned NOT NULL DEFAULT '0' COMMENT '这笔交易的成交方向'," +
           "trade_average_price decimal(18,8) unsigned NOT NULL DEFAULT '0.000' COMMENT '成交均价'," +
           "trade_count decimal(18,8) unsigned NOT NULL DEFAULT '0.000' COMMENT '成交数量'," +
           "trade_fee decimal(18,8) unsigned NOT NULL DEFAULT '0.000000' COMMENT '手续费'," +
           "trade_bfx_fee decimal(18,8) unsigned NOT NULL DEFAULT '0.000000' COMMENT 'bfx抵扣手续费'," +
           "f_user_id int(10) unsigned NOT NULL DEFAULT '0' COMMENT '用户ID'," +
           "coin_id smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '交易货币ID'," +
           "market_coin_id smallint(5) unsigned NOT NULL DEFAULT '0' COMMENT '交易资金币种ID'," +
           "trade_amount decimal(18,8) NOT NULL DEFAULT '0.000000' COMMENT '交易金额'," +
           "trade_time int(10) unsigned NOT NULL DEFAULT '0' COMMENT '成交时间,unix时间格式'," +
           "pending_no bigint(19) unsigned NOT NULL DEFAULT '0' COMMENT '对应挂单流水号'," +
           " PRIMARY KEY (trade_id)," +
           " KEY idx_trade_time (trade_time) USING BTREE," +
           " KEY idx_fuserid_tradetime (f_user_id,trade_time) USING BTREE" +
           ") ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8 COMMENT='交易记录日志表。一个交易会产生两个用户对这个交易的操作记录，基本一份是更新，另一份是写入新记录';";

            String account_log_sql = "CREATE TABLE IF NOT EXISTS " + coinPairName + "_account_log (" +
             "log_id int(10) unsigned NOT NULL AUTO_INCREMENT," +
             "title varchar(100) NOT NULL DEFAULT '' COMMENT '描述标题'," +
             "currency_type_id tinyint(11) unsigned NOT NULL DEFAULT '0' COMMENT '币种（btc/ltc,gtc)1:btc 2:ltc 3...(后期可添加币种)'," +
             "operation tinyint(2) unsigned NOT NULL DEFAULT '0' COMMENT '操作类型(0:其它/1:转入/2:转出/3:交易)'," +
             "direction tinyint(1) NOT NULL DEFAULT '0' COMMENT '1:加款 -1:减款 '," +
             "current_amount decimal(18,8) NOT NULL DEFAULT '0.00000000' COMMENT '当前账户余额(bq_acount:btc/bq_account:ltc)'," +
             "change_amount decimal(18,8) unsigned NOT NULL DEFAULT '0.00000000' COMMENT '增减'," +
             "lastest_amount decimal(18,8) NOT NULL COMMENT '变更后的金额'," +
             "before_frozen decimal(18,8) NOT NULL COMMENT '变更前的冻结'," +
             "after_frozen decimal(18,8) NOT NULL COMMENT '变更后的冻结'," +
             "dateline int(10) unsigned NOT NULL DEFAULT '0' COMMENT '操作时间,unix时间格式'," +
             "f_user_id int(10) unsigned NOT NULL DEFAULT '0' COMMENT '用户ID'," +
             "trans_no char(20) NOT NULL DEFAULT '' COMMENT '交易流水号'," +
             "record_id int(10) unsigned NOT NULL DEFAULT '0' COMMENT '交易分表id'," +
             "f_coin_pair_name varchar(10) NOT NULL COMMENT '交易对名称'," +
             "PRIMARY KEY (`log_id`)," +
             "KEY `idx_f_user_id` (`f_user_id`,`currency_type_id`) USING BTREE" +
           ") ENGINE=InnoDB AUTO_INCREMENT=132946 DEFAULT CHARSET=utf8 COMMENT='用户提现/充值记录,主要记录合约中用户金额变更(个人财务流水）';";

            String ksql1 = "CREATE TABLE IF NOT EXISTS ";
            String ksql2 = " (id int(12) unsigned NOT NULL AUTO_INCREMENT,"
                   + "code varchar(10) NOT NULL DEFAULT '' COMMENT '交易对名称',"
                   + "high decimal(18,8) unsigned NOT NULL DEFAULT '0.00000000' COMMENT '最高价',"
                   + "low decimal(18,8) unsigned NOT NULL DEFAULT '0.00000000' COMMENT '最低价',"
                   + "first decimal(18,8) unsigned NOT NULL DEFAULT '0.00000000' COMMENT '开盘价',"
                   + "last decimal(18,8) unsigned NOT NULL DEFAULT '0.00000000' COMMENT '收盘价',"
                   + "vol decimal(18,8) unsigned NOT NULL DEFAULT '0.00000000' COMMENT '成交数量',"
                   + "time int(10) unsigned NOT NULL DEFAULT '0' COMMENT '时间,unix时间格式',"
                   + "PRIMARY KEY (id),"
                   + "KEY idx_time (time) USING BTREE"
                   + ") ENGINE=InnoDB DEFAULT CHARSET=utf8;";
            var sql = "";
            sql += ksql1 + coinPairName + "_one_minute_trade" + ksql2;
            sql += ksql1 + coinPairName + "_three_minutes_trade" + ksql2;
            sql += ksql1 + coinPairName + "_fifteen_minutes_trade" + ksql2;
            sql += ksql1 + coinPairName + "_half_hour_trade" + ksql2;
            sql += ksql1 + coinPairName + "_one_hour_trade" + ksql2;
            sql += ksql1 + coinPairName + "_two_hours_trade" + ksql2;
            sql += ksql1 + coinPairName + "_four_hours_trade" + ksql2;
            sql += ksql1 + coinPairName + "_six_hours_trade" + ksql2;
            sql += ksql1 + coinPairName + "_twelve_hours_trade" + ksql2;
            sql += ksql1 + coinPairName + "_five_minutes_trade" + ksql2;
            sql += ksql1 + coinPairName + "_one_day_trade" + ksql2;
            sql += ksql1 + coinPairName + "_one_week_trade" + ksql2;
            sql += ksql1 + coinPairName + "_one_month_trade" + ksql2;
            instance.Execute(sql);
        }

        public bool AddMarketCoinPair(BqSpotMarketCoinPair coinPair)
        {
            return instance.Insert(coinPair) > 0;
        }

        public void Update(BqSpotMarketCoinPair coinPair)
        {
            instance.Update(coinPair);
        }

        public long Time()
        {
            return instance.ExecuteScalar<long>("select UNIX_TIMESTAMP() ");
        }

        public List<BqSpotMarketCoinPair> FindByStateNew(int state)
        {
           return instance.Query<BqSpotMarketCoinPair>("select * from BQSpotMarketCoinPair where state = @State",new{ State=state}).ToList();
        }

       public Pagination<BqSpotMarketCoinPair> ReadByStatePM(string coinPairName,int state,int pageIndex=1,int pageSize=10)
        {
            var predicate = PredicateBuilder.True<BqSpotMarketCoinPair>();
            if (state != 0)
            {
                predicate = predicate.And(x=>x.State==state);
            }
            if (string.IsNullOrWhiteSpace(coinPairName))
            {
                predicate = predicate.And(x=>x.CoinPairName == coinPairName);
            }
            var sort = new Sort { PropertyName = "sort", Ascending = true };
            var query = instance.GetListPaged(predicate, sort, pageIndex, pageSize);
            return new Pagination<BqSpotMarketCoinPair>(query, pageIndex, pageSize);    
        }


    }
}
