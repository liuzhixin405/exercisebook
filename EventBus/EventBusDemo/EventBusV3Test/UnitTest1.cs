using EventBusV3;
using System;
using System.Threading;
using Xunit;

namespace EventBusV3Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            FishingRod fishingRod = new FishingRod();
            FishingMan man = new FishingMan("张三");
            man.FishingRod = fishingRod;
            /*
             ldftn     instance void [EventBusV2]EventBusV2.FishingMan::Update(valuetype [EventBusV2]EventBusV2.FishType)
             newobj    instance void [EventBusV2]EventBusV2.FishingRod/FishingHandler::.ctor(object, native int)
             callvirt  instance void [EventBusV2]EventBusV2.FishingRod::add_FishingEvent(class [EventBusV2]EventBusV2.FishingRod/FishingHandler)
             
             */
            //fishingRod.FishingEvent += new FishingEventHandler().HandleEvent; fishingRod构造函数已实现
            while (man.FishCount < 5)
            {
                man.Fishing();
                Console.WriteLine("-------------------------");
                Thread.Sleep(3000);
            }
            Assert.Equal(5, man.FishCount);
        }
    }
}
