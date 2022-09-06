using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBusV2
{
    /// <summary>
    /// 被观察者 
    /// </summary>
    public class FishingRod
    {
        public delegate void FishingHandler(FishType type);
        public event FishingHandler FishingEvent;

        public void ThrowHook(FishingMan man)
        {
            Console.WriteLine("开始下钩！");
            if(new Random().Next() % 2 == 0)
            {
                var type = (FishType)new Random().Next(0,5);
                Console.WriteLine("铃铛：叮叮叮，鱼儿咬钩了");
                if(FishingEvent!=null)
                FishingEvent(type);
            }
        }
    }
}
