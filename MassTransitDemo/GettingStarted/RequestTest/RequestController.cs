using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GettingStarted.RequestTest
{
    internal class RequestController:Controller
    {
        IRequestClient<CheckOrderStatus> _client;
        public RequestController(IRequestClient<CheckOrderStatus> client)
        {
            _client = client;
        }

        public async Task<OrderStatusResult>  Get(string id)
        {
            var response = await _client.GetResponse<OrderStatusResult>(new { OrderId = 2 });
            return response.Message;
        }
    }
}
