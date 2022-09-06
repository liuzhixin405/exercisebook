using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSourcingDemoTwo
{
    internal record Event
    {
        public int Id { get; set; }
        public string Name => GetType().Name;
    }
    internal record AddedCart(string UserId):Event;
    internal record AddedItemToCart(int ProductId,int Qty):Event;
    internal record RemovedItemFromCart(int ProductId,int Qty):Event;
    internal record AddedShippingInformationCart(string Address,string PhoneNumber):Event;
}
