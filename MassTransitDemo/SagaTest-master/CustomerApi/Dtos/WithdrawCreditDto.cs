using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerApi.Dtos
{
    /// <summary>
    /// 消费信用
    /// </summary>
    public class WithdrawCreditDto
    {
        public int CustomerId { get; set; }
        public decimal Credit { get; set; }
    }
}
