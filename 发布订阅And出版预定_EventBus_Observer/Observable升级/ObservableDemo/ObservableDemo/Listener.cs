using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservableDemo
{
    internal class Listener : IObserver<Messager>
    {
        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(Messager value)
        {
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(value));
        }
    }
}
