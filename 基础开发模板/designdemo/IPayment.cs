using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace designdemo
{
    internal interface IPayment<T> where T:IPaymentEntity
    {
        void Pay(T payment);
    }
}
