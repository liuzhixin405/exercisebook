using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class BqExchangeBfcRecord
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
        /// 冻结量
        /// </summary>
        public decimal FrozenAmount { get; set; }
        /// <summary>
        /// 源币量
        /// </summary>
        public decimal OriAmount { get; set; }
        /// <summary>
        /// 目标币量
        /// </summary>
        public decimal TargetAmount { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; } = null!;
        /// <summary>
        /// 价格
        /// </summary>
        public decimal OrderPrice { get; set; }
        /// <summary>
        /// 状态（0、已提交，1、已审核，2、转人工审核，3、自动处理中，4、已处理，5、已取消）
        /// </summary>
        public short State { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public int OperTime { get; set; }
        /// <summary>
        /// 提交记录时间
        /// </summary>
        public int RecordTime { get; set; }
    }
}
