namespace ECommerce.API.Application
{
    // 订单组件接口
    public interface IOrderComponent
    {
        decimal GetTotal();
        void Display(int indent = 0);

    }
}
