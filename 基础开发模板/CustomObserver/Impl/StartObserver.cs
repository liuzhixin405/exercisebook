using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomObserver.Impl
{
    internal class StartObserver : AbsObserver<Data>
    {
        public StartObserver(string typee) : base(typee)
        {
        }

        public override Task UpdateAsync(Data data)
        {
            Console.WriteLine($"StartObserver received data: {data.Message} ,{_type}");
            return Task.CompletedTask;
        }
    }
}
