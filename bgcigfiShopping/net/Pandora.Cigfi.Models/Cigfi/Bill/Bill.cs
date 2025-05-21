using System;

namespace Pandora.Cigfi.Models.Cigfi
{
    /// <summary>
    /// 账单流水表 cigfi_bill
    /// </summary>
    public class Bill
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Type { get; set; } // 收入/支出/返佣/提现等
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
