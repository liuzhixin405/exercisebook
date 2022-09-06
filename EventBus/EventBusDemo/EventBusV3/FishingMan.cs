using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBusV3
{
    /// <summary>
    /// 观察者
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
        /// 
        /// </summary>
        public void Fishing()
        {
            this.FishingRod.ThrowHook(this);
        }
        //该方法移除到类FishingEventHandler中
        //public void Update(FishingEventData eventData)
        //{
        //    FishCount++;
        //    Console.WriteLine("{0}：钓到一条[{2}]，已经钓到{1}条鱼了！", Name, FishCount, eventData.FishType);
        //}
    }
}
