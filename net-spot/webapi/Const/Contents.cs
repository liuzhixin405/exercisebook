namespace webapi.Const
{
    public class Contents
    {
        public enum InfoType
        {
            info, error
        }

        public readonly String s = "";

        /** 方向 买1 */
        public static readonly int DIRECTION_BUY = 1;
        /** 方向 卖2 */
        public static readonly int DIRECTION_SELL = 2;
        /** 成交类型(1:多单开仓,买多 */
        public static readonly int TRADE_TYPE_BUY = 1;
        /** 成交类型(2:空头开仓,卖空 */
        public static readonly int TRADE_TYPE_SELL = 2;

        /** 操作类型1多单开仓 */
        public static readonly int CZLX_DDKC = 1;
        /** 操作类型2多单平仓 */
        public static readonly int CZLX_DDPC = 2;
        /** 操作类型3空头开仓 */
        public static readonly int CZLX_KTKC = 3;
        /** 操作类型4空头平仓 */
        public static readonly int CZLX_KTPC = 4;
        /** 多单爆仓 */
        public static readonly int CZLX_DDBC = 5;
        /** 空头爆仓 */
        public static readonly int CZLX_KTBC = 6;
        /** 多单交割 */
        public static readonly int CZLX_DDJG = 7;
        /** 空头交割 */
        public static readonly int CZLX_KTJG = 8;

        /** 优先级开仓1 */
        public static readonly int PRIORITY_KC = 1;
        /** 优先级平仓2 */
        public static readonly int PRIORITY_PC = 2;
        /** 优先级爆仓3 */
        public static readonly int PRIORITY_BC = 3;
        /** 优先级交割4 */
        public static readonly int PRIORITY_JG = 4;

        /** PC设备 */
        public static readonly int DEVICE_PC = 0;
        /** App设备 */
        public static readonly int DEVICE_APP = 1;
        /** Api设备 */
        public static readonly int DEVICE_API = 2;
        /** 后台-管理员 */
        public static readonly int DEVICE_ADMIN = 3;
        /** 服务器-强平 */
        public static readonly int DEVICE_SERVER_QP = 4;
        /** 服务器-强撮 */
        public static readonly int DEVICE_SERVER_QC = 5;
        /** 服务器-计划委托 */
        public static readonly int DEVICE_SERVER_PLAN = 6;
        /** 服务器-止盈止损 */
        public static readonly int DEVICE_SERVER_SLW = 7;
        /** 服务器-成交保证金不够，部分撤销 */
        public static readonly int DEVICE_SERVER_BFC = 8;
        /** 服务器-深度自成交 */
        public static readonly int DEVICE_SERVER_DEPTH = 9;
        /** 服务器-监控用户 */
        public static readonly int DEVICE_SERVER_MONITOR = 10;
        /** 服务器-出错委托 */
        public static readonly int DEVICE_SERVER_ERROR = 11;
        /** 服务器-交割强平 */
        public static readonly int DEVICE_SERVER_DELIVERY = 12;


        /** 合约状态正常1 */
        public static readonly int TRANSACTION_STATE_ZC = 1;
        /** 合约状态清算中2 */
        public static readonly int TRANSACTION_STATE_QSZ = 2;
        /** 合约状态关闭3 */
        public static readonly int TRANSACTION_STATE_GB = 3;

        /** 交易对状态正常1 */
        public static readonly int COINPAIR_STATE_NORMAL = 1;
        /** 合约状态关闭2 */
        public static readonly int COINPAIR_STATE_CLOSE = 2;

        /** 市场类型 正式盘 1 */
        public static readonly int MARKET_TYPE_CONTRACT = 1;
        /** 市场类型 模拟盘 2 */
        public static readonly int MARKET_TYPE_VCONTRACT = 2;
        /** 市场类型 币币交易 3 */
        public static readonly int MARKET_TYPE_SPOT = 3;
        /** 市场类型 交割合约 4 */
        public static readonly int MARKET_TYPE_DELIVERY = 4;

        /** 挂单日志 动作 初始状态 */
        public static readonly int PENDING_LOG_ACTION_CS = 0;
        /** 挂单日志 动作 成交1 */
        public static readonly int PENDING_LOG_ACTION_CJ = 1;
        /** 挂单日志 动作 结算2 */
        public static readonly int PENDING_LOG_ACTION_JS = 2;
        /** 挂单日志 动作 强平3 */
        public static readonly int PENDING_LOG_ACTION_QP = 3;
        /** 挂单日志 动作 撤单4 */
        public static readonly int PENDING_LOG_ACTION_CD = 4;

        /** 计划委托 状态 未生效0 */
        public static readonly int PLAN_PENDING_STATUS_INEFFECTIVE = 0;
        /** 计划委托 状态 生效1 */
        public static readonly int PLAN_PENDING_STATUS_EFFECTIVE = 1;
        /** 计划委托 状态 失败2 */
        public static readonly int PLAN_PENDING_STATUS_FAIL = 2;
        /** 计划委托 状态 已撤销3 */
        public static readonly int PLAN_PENDING_STATUS_CANCEL = 3;

        /** 计划委托 类型 现价大于触发价1 */
        public static readonly int PLAN_PENDING_TYPE_GT = 1;
        /** 计划委托 类型 现价小于触发价2 */
        public static readonly int PLAN_PENDING_TYPE_LT = 2;

        /** 交易类型标识 1普通交易 */
        public static readonly int TRADE_FLAG_JY = 1;
        /** 交易类型标识 2交割 */
        public static readonly int TRADE_FLAG_JG = 2;
        /** 交易类型标识 3强平 */
        public static readonly int TRADE_FLAG_QP = 3;

        /** 个人财务流水 交易类型 充值：1 */
        public static readonly int TRANS_TYPE_CZ = 1;
        /** 个人财务流水 交易类型 提现：2 */
        public static readonly int TRANS_TYPE_TX = 2;
        /** 个人财务流水 交易类型 交易：3 */
        public static readonly int TRANS_TYPE_JY = 3;
        /** 个人财务流水 交易类型 追加保证金：4 */
        public static readonly int TRANS_TYPE_APBZJ = 4;
        /** 个人财务流水 交易类型 购买积分：5 */
        public static readonly int TRANS_TYPE_BUYCredits = 5;
        /** 个人财务流水 交易类型 奖励：6 */
        public static readonly int TRANS_TYPE_AWARD = 6;
        /** 个人财务流水 交易类型 转入：7 */
        public static readonly int TRANS_TYPE_TO = 7;
        /** 个人财务流水 交易类型 转出：8 */
        public static readonly int TRANS_TYPE_OUT = 8;
        /** 个人财务流水 交易类型 兑换：9 */
        public static readonly int TRANS_TYPE_EXCHANGE = 9;
        /** 母账户转入子账号：10 */
        public static readonly int TRANS_TYPE_TOSUB = 10;
        /** 子账号转出到母账户：11 */
        public static readonly int TRANS_TYPE_FROMSUB = 11;
        /** OTC资产转入：12 */
        public static readonly int TRANS_TYPE_TOOTC = 12;
        /** 转出到OTC资产：13 */
        public static readonly int TRANS_TYPE_OUTOTC = 13;

        /** 交易记录数量：50 */
        public static readonly int TRADE_JSON = 50;

        /** 系统用户id */
        public static readonly long BQMEMBER_ADMIN = 1l;

        /** 自由经纪人:1 */
        public static readonly int BQMEMBER_APPLY_BROKER = 1;
        /** 高级经纪人:3 */
        public static readonly int BQMEMBER_APPLY_ADVANCEDBROKER = 3;
        /** 区域代理商:2 */
        public static readonly int BQMEMBER_APPLY_AREA = 2;

        /** 待审:0 */
        public static readonly int BQMEMBER_APPLY_WAITE = 0;
        /** 通过审核:1 */
        public static readonly int BQMEMBER_APPLY_SUCC = 1;
        /** 拒绝:-1 */
        public static readonly int BQMEMBER_APPLY_REFUSE = -1;

        /** 充值提现表 充值类型 普通充值1 */
        public static readonly int RECHARGE_FLAG_NORMAL = 1;
        /** 充值提现表 充值类型 管理员充值2 */
        public static readonly int RECHARGE_FLAG_ADMIN = 2;
        /** 充值提现表 充值类型 管理员奖励操作3 */
        public static readonly int RECHARGE_FLAG_ADMIN_AWARD = 3;

        /** 充值提现表 充值1 */
        public static readonly int RECHARGE_TYPE_CZ = 1;
        /** 充值提现表 提现2 */
        public static readonly int RECHARGE_TYPE_TX = 2;
        /** 奖励 */
        public static readonly int RECHARGE_TYPE_CZ_AWARD = 5;
        /** 扣除奖励 */
        public static readonly int RECHARGE_TYPE_TX_AWARD = 6;
        /** 充值提现表 转入3 */
        public static readonly int RECHARGE_TYPE_ZR = 3;
        /** 充值提现表 转出4 */
        public static readonly int RECHARGE_TYPE_ZC = 4;

        /** 母子账户转入转出表 转入子账号：1 */
        public static readonly int SUB_TRANSFER_IN = 1;
        /** 母子账户转入转出表 子账号转出到母账户：2 */
        public static readonly int SUB_TRANSFER_OUT = 2;


        /** 充值提现表 状态 未处理0 */
        public static readonly int RECHARGE_STATE_ING = 0;
        /** 充值提现表 状态 确认1 */
        public static readonly int RECHARGE_STATE_ED = 1;
        /** 充值提现表 状态 拒绝2 */
        public static readonly int RECHARGE_STATE_NOT = 2;
        /** 充值提现表 状态 自动处理中3 */
        public static readonly int RECHARGE_STATE_PROC = 3;
        /** 充值提现表 状态  转人工处理4 */
        public static readonly int RECHARGE_STATE_MANUAL = 4;
        /** 充值提现表 状态 自动处理，人工已审核5 */
        public static readonly int RECHARGE_STATE_CHECKED = 5;

        /** 积分日志表 状态 注册1 */
        public static readonly int CREDIT_TYPE_REGISTER = 1;
        /** 积分日志表 状态 验证邮箱2 */
        public static readonly int CREDIT_TYPE_SUREEMAIL = 2;
        /** 积分日志表 状态 绑定手机3 */
        public static readonly int CREDIT_TYPE_BINDPHONE = 3;
        /** 积分日志表 状态 绑定Google4 */
        public static readonly int CREDIT_TYPE_BINDGOOGLE = 4;
        /** 积分日志表 状态 首次充值5 */
        public static readonly int CREDIT_TYPE_FIRST_RECHARGE = 5;
        /** 积分日志表 状态 交易6 */
        public static readonly int CREDIT_TYPE_TRADE = 6;
        /** 积分日志表 状态 购买7 */
        public static readonly int CREDIT_TYPE_BUY = 7;
        /** 积分日志表 状态 奖励8 */
        public static readonly int CREDIT_TYPE_AWARD = 8;
        /** 积分日志表 状态 首次交易9 */
        public static readonly int CREDIT_TYPE_FIRST_TRADE = 9;
        /** 积分日志表 状态 账户存额10 */
        public static readonly int CREDIT_TYPE_ACCOUNT_BALANCE = 10;
        /** 积分日志表 状态 扣除11 */
        public static readonly int CREDIT_TYPE_SUBTRACT = 11;

        /** 止盈 */
        public static readonly int STOP_WIN = 1;
        /** 止损 */
        public static readonly int STOP_LOSS = 2;

        /** 止盈止损日志表 动作更新-添加(止盈止损由用户正常添加) */
        public static readonly int STOP_LOSS_LOG_ACTION_CS = 0;
        /** 止盈止损日志表 动作 止盈止损TimerTask触发挂单成功1 */
        public static readonly int STOP_LOSS_LOG_ACTION_CG = 1;
        /** 止盈止损日志表 动作 止盈止损TimerTask触发挂单失败2 */
        public static readonly int STOP_LOSS_LOG_ACTION_SB = 2;
        /** 止盈止损日志表 动作 强平-撤单3(爆仓TimerTaskOverFlow：处理达到强平价格的持单，撤掉止盈止损单) */
        public static readonly int STOP_LOSS_LOG_ACTION_QP = 3;
        /** 止盈止损日志表  动作更新-撤单4(止盈止损由用户正常撤销) */
        public static readonly int STOP_LOSS_LOG_ACTION_CD = 4;
        /** 止盈止损日志表  动作平仓成交-撤单5 */
        public static readonly int STOP_LOSS_LOG_ACTION_CJ = 5;
        /** 止盈止损日志表  动作删除-触发成功或失败后都删除 */
        public static readonly int STOP_LOSS_LOG_ACTION_SC = 6;

        /** 止盈止损日志表 标识 正常 */
        public static readonly int STOP_LOSS_FLAG_NORMAL = 0;
        /** 止盈止损日志表 标识 异常 */
        public static readonly int STOP_LOSS_FLAG_ERROR = 1;


        /** api获取市场深度更新时间 */
        public static readonly int APIDEPTH_SECOND = 2;

        /** 货币汇率使用状态 使用1 */
        public static readonly int COIN_STATE_ONUSE = 1;
        /** 货币汇率使用状态 未使用0 */
        public static readonly int COIN_STATE_NOUSE = 0;

        public static readonly String BQTRANS_CODE_SZ = "sz";

        public static readonly int BQTRANS_TYPE_SZ = 3;

        public static readonly int TRADE_CURRENCY_TYPE_ID = 2;     // USDT

        //	/** 货币类型1 比特币 */
        //	public static readonly int COIN_CURRENCY_BTC = 1;
        //	/** 货币类型2 莱特币 */
        //	public static readonly int COIN_CURRENCY_LTC = 2;
        //	/** 货币类型3 上证指数 */
        //	public static readonly int COIN_CURRENCY_SZI = 3;
        //	
        //	/** 合约类型 日合约 */
        //	public static readonly String BQCODE_BR = "br";
        //	/** 合约类型 周合约 */
        //	public static readonly String BQCODE_BZ = "bz";
        //	/** 合约类型 月合约 */
        //	public static readonly String BQCODE_BTC = "btc";
        //	/** 合约类型 上证合约 */
        //	public static readonly String BQCODE_SZ = "sz";
        //	
        //	/** 合约类型 btc合约 */
        //	public static readonly int BQTRAN_TYPE_BTC = 1;
        //	/** 合约类型 SZ合约 */
        //	public static readonly int BQTRAN_TYPE_SZ = 3;

        /** 合约的默认杠杆系数 */
        public static readonly String DEFAULT_MULTIPLE = "1,5,10,20,50";

        /** 合约的默认杠杆系数状态 */
        public static readonly String DEFAULT_MULTIPLE_STATE = "1,1,1,1,1";

        /** 合约的默认杠杆系数 */
        public static readonly decimal DEFAULT_TRADE_COEFFICIENT = new decimal(1);

        /** 注册送积分 */
        public static readonly String REGISTE_BIND_SUCC = "注册送积分";

        /** btc100flag */
        public static readonly int OTHERTRADEDATAS_100 = 1;
        /** okcoinflag */
        public static readonly int OTHERTRADEDATAS_OKCOIN = 2;
        /** huobiflag */
        public static readonly int OTHERTRADEDATAS_HUOBI = 3;

        /** countAccount平账1 */
        public static readonly int COUNTACCOUNT_PINGZHANG = 1;
        /** countAccount已交割手续费2 */
        public static readonly int COUNTACCOUNT_FEE = 2;

        public static readonly int TEL_CODE_LIMIT = 30;
        public static readonly int TEL_CODE_ERROR_LIMIT = 6;

        public static readonly int TRANSFER_OTC_STATE_MANUAL = 1;
        public static readonly int TRANSFER_OTC_STATE_SUCCESS = 2;
        public static readonly int TRANSFER_OTC_STATE_CANCELLED = 3;

        public static readonly int EXCHANGE_STATE_SUBMIT = 0;
        public static readonly int EXCHANGE_STATE_CHECKED = 1;
        public static readonly int EXCHANGE_STATE_MANUAL = 2;
        public static readonly int EXCHANGE_STATE_PROCESSING = 3;
        public static readonly int EXCHANGE_STATE_SUCCESS = 4;
        public static readonly int EXCHANGE_STATE_CANCELLED = 5;

        public static readonly int REWARD_REASON_LOGIN = 1;
        public static readonly int REWARD_REASON_INVITE = 2;
        public static readonly int REWARD_REASON_ACCOUNT = 3;
        public static readonly int REWARD_REASON_BFX = 4;
        public static readonly int REWARD_REASON_REGISTER = 5;
        public static readonly int REWARD_REASON_OLD_REGISTER = 6;

        public static readonly int API_ERROR_UNKNOW = 10000;
        public static readonly int API_ERROR_PARAM = 10001;
        public static readonly int API_ERROR_APPKEY = 10002;
        public static readonly int API_ERROR_OVERTIME = 10003;
        public static readonly int API_ERROR_IPNOTALLOW = 10004;
        public static readonly int API_ERROR_SIGN = 10005;
        public static readonly int API_ERROR_APPKEYDISABLED = 10006;
        public static readonly int API_ERROR_NOTRANSACTION = 10007;
        public static readonly int API_ERROR_NOMULTI = 10008;
        public static readonly int API_ERROR_TRANSSUSPEND = 10009;
        public static readonly int API_ERROR_PRECISION = 10010;
        public static readonly int API_ERROR_LOWBALANCE = 10011;
        public static readonly int API_ERROR_RISK = 10012;
        public static readonly int API_ERROR_NOORDER = 10013;
        public static readonly int API_ERROR_NOPOSITION = 10014;
        public static readonly int API_ERROR_EXCHANGEMIN = 10015;
        public static readonly int API_ERROR_INSUFFICIENTBALANCE = 10016;
        public static readonly int API_ERROR_ORDER_IN_PROCESS = 10017;

        public static readonly int API_ERROR_RELOGIN = 10020;

        public static readonly String PENDING_STATE_SUBMITTING = "submitting";
        public static readonly String PENDING_STATE_SUBMITTED = "submitted";
        public static readonly String PENDING_STATE_FILLED = "filled";
        public static readonly String PENDING_STATE_CANCELED = "canceled";

        public static readonly String EXCHANGE_STATE_DONE = "success";
        public static readonly String EXCHANGE_STATE_CANCEL = "canceled";
        public static readonly String EXCHANGE_STATE_PROCESS = "processing";

        public static readonly String RECHARGE_STATE_SUBMITTING = "submitting";
        public static readonly String RECHARGE_STATE_PROCESS = "processing";
        public static readonly String RECHARGE_STATE_DONE = "success";
        public static readonly String RECHARGE_STATE_REFUSE = "refuse";
        public static readonly String RECHARGE_STATE_MANUAL_PROCESS = "manual";
        public static readonly String RECHARGE_STATE_MANUAL_CHECKED = "checked";

        public static readonly String BASE_UPLOAD_PATH = "/work/coinbase";
        public static readonly String TEMP_PATH = "/tmp/btcq";
        public static readonly String BASE_UPLOAD_URL = "localhost:5000";

    }

}