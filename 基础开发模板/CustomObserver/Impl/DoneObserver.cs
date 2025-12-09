using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomObserver.Impl
{
    internal class DoneObserver : AbsObserver<Data>
    {
       
        public DoneObserver(string typee) : base(typee)
        {
        }

        public override Task UpdateAsync(Data data)
        {
            Console.WriteLine($"DoneObserver done message: {data.Message}, {_type}");
            return Task.CompletedTask;
        }
    }
}
