using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogLibrary
{
    public class LogClient : IObserver<KeyValuePair<string,object?>>
    {
        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }


        public void OnNext(KeyValuePair<string, object?> value)
        {
           
            Console.WriteLine($"接收的消息为: {System.Text.Json.JsonSerializer.Serialize(value.Value)}");
        }
    }
}
