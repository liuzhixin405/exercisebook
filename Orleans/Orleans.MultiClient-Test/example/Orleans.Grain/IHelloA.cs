using Orleans.Concurrency;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Orleans.Grains
{
    /// <summary>
    /// Orleans grain communication interface IHello
    /// </summary>
    //public interface IHelloA : Orleans.IGrainWithIntegerKey
     
    public interface IHelloA : Orleans.IGrainWithIntegerKey
    {
        [AlwaysInterleave]
        Task<string> SayHello(string greeting);
    }
}
