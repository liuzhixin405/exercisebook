using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crypto_exchange_engine_simple.Engine
{
    public class Trade
    {
        public string TakerOrderId { get; set; }
        public string MakerOrderId { get; set; }
        public decimal Amount { get; set; }
        public decimal Price { get; set; }
        public override string ToString()
        {
            return $"TaskerOrderId:{TakerOrderId},MakerOrderId{MakerOrderId},Amount:{Amount},Price:{Price}";
        }
    }
}
