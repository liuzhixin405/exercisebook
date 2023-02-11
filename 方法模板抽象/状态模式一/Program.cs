namespace 状态模式一
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TrafficLight trafficLight = new TrafficLight(new GreenLightState());
            trafficLight.Show();
            trafficLight.SetState(new YellowLightState());
            trafficLight.Show();
            trafficLight.SetState(new RedLightState());
            trafficLight.Show();
        }
    }

    /// <summary>
    /// 定义状态的抽象类
    /// </summary>
    public abstract class TrafficLighState
    {
        public abstract void Display();
    }

    #region 具体实现
    public class RedLightState : TrafficLighState
    {
        public override void Display()
        {
            Console.WriteLine("RedLight");
        }
    }
    public class GreenLightState : TrafficLighState
    {
        public override void Display()
        {
            Console.WriteLine("GreenLight");
        }
    }
    public class YellowLightState : TrafficLighState
    {
        public override void Display()
        {
            Console.WriteLine("YellowLight");
        }
    } 
    #endregion

    /// <summary>
    /// 状态控制类
    /// </summary>
    public class TrafficLight
    {
        private TrafficLighState trafficLighState;
        public TrafficLight(TrafficLighState trafficLighState)
        {
            this.trafficLighState = trafficLighState;
        }
        public void SetState(TrafficLighState state)
        {
            trafficLighState = state;
        }
        public void Show()
        {
            trafficLighState.Display();
        }
    }
}