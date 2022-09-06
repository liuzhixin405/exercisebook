using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSourcingDemo
{
    internal interface IEvent { }
    internal record ProductShipped(String Sku,int Quantity,DateTime DateTime):IEvent;           //发货
    internal record ProductReceived(String Sku, int Quantity, DateTime DateTime) : IEvent;       //收货
    internal record InventoryAdjusted(String Sku, int Quantity, string Resason,DateTime DateTime) : IEvent;    //调整库存
}
