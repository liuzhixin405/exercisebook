using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeMaker
{
    /// <summary>
    /// c++提供的硬件接口
    /// </summary>
   public enum WarmerPlateStatus
    {
        WARMER_EMPTY,
        POT_EMPTY,
        POT_NOT_EMPTY
    }
    public enum BoilerStatus
    {
        EMPTY,NOT_EMPTY
    }
    public enum BrewButtonStatus
    {
        PUSHED,NOT_PUSHED
    }
    public enum BoilerState
    {
        ON,OFF
    }
    public enum WarmerState
    {
        ON,OFF
    }
    public enum IndicatorState
    {
        ON,OFF
    }
    public enum ReliefValveState
    {
        OPEN,CLOSED
    }
    public interface CoffeeMakerAPI
    {
        /// <summary>
        /// 返回保温盘传感器的状态 传感器检测有没有壶，壶中有没有水
        /// </summary>
        /// <returns></returns>
        WarmerPlateStatus GetWarmerPlayeStatus();
        /// <summary>
        /// 返回加热器开关的状态加热器开关是浮动的，确保加热器中至少有半杯水
        /// </summary>
        /// <returns></returns>
        BoilerStatus GetBoilerStatus();
        /// <summary>
        /// 返回冲煮按钮的状态
        /// 冲煮按钮是一个瞬时开关，可以记住自己的状态
        /// 每次调用都会返回记忆的状态
        /// 然后将该状态重置为NOT_PUSHED
        /// 
        /// 因此，即使以非常慢的速度轮询这个函数，仍然可以在冲煮按钮被按下时检测到
        /// </summary>
        /// <returns></returns>
        BrewButtonStatus GetBrewButtonStatus();

        /// <summary>
        /// 负责开、关加热器中的加热元件
        /// </summary>
        /// <param name="s"></param>
        void SetBolierState(BoilerState s);
        /// <summary>
        /// 负责开、关加热器中的加热元件
        /// </summary>
        /// <param name="s"></param>
        void SetWarmerState(WarmerState s);
        /// <summary>
        /// 负责开、关指示灯
        /// 冲煮完成时灯亮；用户按下冲煮按钮时灯灭
        /// </summary>
        /// <param name="s"></param>
        void SetIndicatorState(IndicatorState s);
        /// <summary>
        /// 负责开、关减压阀
        /// 关闭减压阀时，加热器中的压力使热水喷洒在咖啡过滤器上
        /// 打开减压阀时,加热器中的蒸汽排到外部
        /// 加热器中的水不会再喷到过滤器上
        /// </summary>
        void SetReliefValveState(ReliefValveState s);
    }

    public class M4CoffeeMakerAPI : CoffeeMakerAPI
    {
        private readonly int random;
        public M4CoffeeMakerAPI()
        {
            random = Random.Shared.Next();
        }
        public BoilerStatus GetBoilerStatus()
        {
            if (random % 2 == 0)
            {
                return BoilerStatus.NOT_EMPTY;
            }
            return BoilerStatus.EMPTY;
        }

        public BrewButtonStatus GetBrewButtonStatus()
        {
            if (random % 2 == 0)
            {
                return BrewButtonStatus.NOT_PUSHED;
            }
            return BrewButtonStatus.PUSHED;
        }

        public WarmerPlateStatus GetWarmerPlayeStatus()
        {
            if (random % 3 == 0)
            {
                return  WarmerPlateStatus.POT_EMPTY;
            }else if(random % 3 == 1)
            {
               return WarmerPlateStatus.WARMER_EMPTY;
            }
            else
            {
                return WarmerPlateStatus.POT_NOT_EMPTY;
            }
   
        }

        public void SetBolierState(BoilerState s)
        {
            Console.WriteLine(" BoilerState is set");
        }

        public void SetIndicatorState(IndicatorState s)
        {
            Console.WriteLine(" IndicatorState is set");
        }

        public void SetReliefValveState(ReliefValveState s)
        {
            Console.WriteLine(" ReliefValveState is set");
        }

        public void SetWarmerState(WarmerState s)
        {
            Console.WriteLine(" WarmerState is set");
        }
    }
}
