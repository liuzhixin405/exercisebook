using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace 状态模式二
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var lamborghini = new Car();

            Display(lamborghini.CurrentState);
            lamborghini.TaskAction(Action.Start);

            Display(lamborghini.CurrentState);
            lamborghini.TaskAction(Action.Start);

            Display(lamborghini.CurrentState);
            lamborghini.TaskAction(Action.Accelerate); //加速

            Display(lamborghini.CurrentState);
            lamborghini.TaskAction(Action.Stop);

            Display(lamborghini.CurrentState);
            /* result
                车辆当前状态: Stopped
                车辆当前状态:Started
                车辆当前状态:Started
                车辆当前状态:Running
                车辆当前状态:Stopped
            */
        }
        static void Display(State state)
        {
            Console.WriteLine($"车辆当前状态:{state}");
        }
    }
    /// <summary>
    /// 模拟汽车的状态和驾驶员的动作判断车辆当前的状态
    /// </summary>
    public class Car
    {
        private State state = State.Stopped;
        public State CurrentState { get { return state; } }

        public void TaskAction(Action action)
        {
            state = (state, action) switch
            {
                (State.Stopped, Action.Start) => State.Started,
                (State.Started, Action.Accelerate) => State.Running,
                (State.Started, Action.Stop) => State.Stopped,
                (State.Running, Action.Stop) => State.Stopped,
                _ => state
            };
        }
    }
    public enum State { Stopped, Started, Running }
    public enum Action { Stop, Start, Accelerate }
}