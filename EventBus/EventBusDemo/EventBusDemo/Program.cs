using System;
using System.Threading;

namespace EventBusDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var fishingRod = new FishingRod (); //鱼竿
            var jeff = new FishingMan("姜太公");
            jeff.FishingRod = fishingRod; //鱼竿分配
            //注册观察者
            fishingRod.FishingEvent += jeff.Update;
            //循环钓鱼
            while(jeff.FishCount < 5)
            {
                jeff.Fishing();
                Console.WriteLine("----------------------");
                Thread.Sleep(5000);
            }
            Console.ReadLine();
        }
    }

    public enum FishType
    {
        鲫鱼,
        鲤鱼,
        黑鱼,
        青鱼,
        草鱼,
        鲈鱼
    }

    /// <summary>
    /// 姜太公
    /// </summary>
    public class FishingMan
    {
        public string Name { get; set; }
        public int FishCount { get; set; }
        public FishingMan(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// 鱼竿
        /// </summary>
        public FishingRod FishingRod { get; set; }

        /// <summary>
        /// 垂钓
        /// </summary>
        public void Fishing()
        {
            this.FishingRod.ThrowHook(this);
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="fishType"></param>
        public void Update(FishType fishType)
        {
            FishCount++;
            Console.WriteLine("{0} : 钓到一条[{2}],已经都钓到{1}条鱼了!", Name, FishCount, fishType);
        }
    }
    public class FishingRod
    {
        public delegate void FishingHandler(FishType type);
        public event FishingHandler FishingEvent;

        /// <summary>
        /// 钓竿工作->钓鱼
        /// </summary>
        /// <param name="man"></param>
        public void ThrowHook(FishingMan man)
        {
            Console.WriteLine("开始下钩! ");

            if(new Random().Next() % 2 == 0)
            {
                var type = (FishType)new Random().Next(0, 5);
                Console.WriteLine("铃铛：叮叮叮，鱼儿咬钩了");
                if (FishingEvent != null)
                    FishingEvent(type);
            }
        }
    }
}
