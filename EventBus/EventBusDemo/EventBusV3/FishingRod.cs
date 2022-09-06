using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EventBusV3
{
    /// <summary>
    /// 被观察者 
    /// </summary>
    public class FishingRod
    {
        //public delegate void FishingHandler(FishingEventData eventData);
        //public event FishingHandler FishingEvent;
        public FishingRod()
        {
            //Assembly assembly = Assembly.GetExecutingAssembly();
            //foreach (var type in assembly.GetTypes())
            //{
            //    if (typeof(IEventHandler).IsAssignableFrom(type)&&!type.Name.Equals("IEventHandler")&& !type.Name.Equals("IEventHandler`1"))
            //    {
            //        Type handlerInterface = type.GetInterface("IEventHandler`1");                  
            //            Type eventDataType = handlerInterface.GetGenericArguments()[0];
                   
            //        if (eventDataType.Equals(typeof(FishingEventData)))
            //        {
            //            var handler = Activator.CreateInstance(type) as IEventHandler<FishingEventData>;
            //            FishingEvent += handler.HandleEvent;
            //        }
            //    }
            //}
        }
        public void ThrowHook(FishingMan man)
        {
            Console.WriteLine("开始下钩！");
            if(new Random().Next() % 2 == 0)
            {
                var type = (FishType)new Random().Next(0,5);
                FishingEventData data = new FishingEventData(); //事件源
                data.FishingMan = man;
                data.EventTime = DateTime.Now;
                data.FishType = type;
                Console.WriteLine("铃铛：叮叮叮，鱼儿咬钩了");
                //if(FishingEvent!=null)
                //FishingEvent(data);               
                //事件总线模式
                EventBus.Default.Trigger<FishingEventData>(data);
            }
        }
    }
}
