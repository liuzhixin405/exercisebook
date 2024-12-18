using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class BqExchangeRecord
    {
        public uint Id { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int FUserId { get; set; }
        /// <summary>
        /// 源币种id
        /// </summary>
        public short OriCurrencyTypeId { get; set; }
        /// <summary>
        /// 目标币种id
        /// </summary>
        public short TargetCurrencyTypeId { get; set; }
        /// <summary>
        /// 源币种名称
        /// </summary>
        public string OriCurrencyName { get; set; } = null!;
        /// <summary>
        /// 目标币种名称
        /// </summary>
        public string TargetCurrencyName { get; set; } = null!;
        /// <summary>
        /// 冻结幅度
        /// </summary>
        public decimal FrozenRate { get; set; }
        /// <summary>
        /// 冻结量
        /// </summary>
        public decimal FrozenAmount { get; set; }
        public decimal FrozenAmountBack { get; set; }
        /// <summary>
        /// 源币量
        /// </summary>
        public decimal OriAmount { get; set; }
        /// <summary>
        /// 目标币量
        /// </summary>
        public decimal TargetAmount { get; set; }
        /// <summary>
        /// 现价
        /// </summary>
        public decimal PresentPrice { get; set; }
        /// <summary>
        /// 价格调整幅度
        /// </summary>
        public decimal PriceRange { get; set; }
        /// <summary>
        /// 对冲方向（1、买入；2、卖出）
        /// </summary>
        public short Direction { get; set; }
        /// <summary>
        /// 手续费率
        /// </summary>
        public decimal FeeRate { get; set; }
        /// <summary>
        /// 手续费
        /// </summary>
        public decimal Fee { get; set; }
        /// <summary>
        /// 实际获得的目标币量
        /// </summary>
        public decimal ObtainAmount { get; set; }
        /// <summary>
        /// 对冲尝试次数
        /// </summary>
        public short HedgingTimes { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderId { get; set; } = null!;
        /// <summary>
        /// 状态（0、已提交，1、已审核，2、转人工审核，3、自动处理中，4、已处理，5、已取消）
        /// </summary>
        public short State { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; } = null!;
        /// <summary>
        /// 记录时间
        /// </summary>
        public int RecordTime { get; set; }
        /// <summary>
        /// 处理时间
        /// </summary>
        public int OperTime { get; set; }
        /// <summary>
        /// 下单价
        /// </summary>
        public decimal OrderPrice { get; set; }
        /// <summary>
        /// 成交均价
        /// </summary>
        public decimal AvgPrice { get; set; }
        /// <summary>
        /// 成交量
        /// </summary>
        public decimal DealAmount { get; set; }
        /// <summary>
        /// 成交额
        /// </summary>
        public decimal DealCash { get; set; }
    }
}
