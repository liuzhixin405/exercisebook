using Contract.Core.Enum;

namespace Contract.Web.ApiModel
{
    public class OrderDTO
    {
        //public OrderDTO(string contractId, decimal price, decimal size, int marginMode)
        //{
        //    //unitMode 判断下单数量是U还是币种
        //    ContractId = contractId;
        //    Price = price;
        //    CumQty = size;
        //    MarginMode = (MarginMode)marginMode;
        //}
        public OrderDTO()
        {

        }
        /// <summary>
        /// 合约id
        /// </summary>

        public String ContractId { get; set; }

        /// <summary>
        /// 价格
        /// </summary>

        public Decimal Price { get; set; }
        /// <summary>
        /// 已成交数量
        /// </summary>

        public Decimal CumQty { get; set; }
        /// <summary>
        /// 保证金模式
        /// </summary>
        public MarginMode MarginMode { set; get; }
        public OrderStatus OrderStatus { get; set; }
    }
}
