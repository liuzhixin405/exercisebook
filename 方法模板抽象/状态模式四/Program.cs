using System.Collections;
using System.Transactions;

namespace 状态模式四
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }
    }
    public interface TurnstileController
    {
        void Lock();
        void Unlock();
        void ThankYou();
        void Alarm();
    }
    public enum State { LOCKED, UNLOCKED }
    public enum Event { COIN, PASS }
    public class Turnstile
    {
        internal State state = State.LOCKED;
        private IList transitions = new ArrayList();
        private delegate void Action();
        public Turnstile(TurnstileController controller)
        {
            Action unlock = new Action(controller.Unlock);
            Action alarm = new Action(controller.Alarm);
            Action thankyou = new Action(controller.ThankYou);
            Action lockAction = new Action(controller.Lock);
            AddTransition(State.LOCKED, Event.COIN, State.UNLOCKED, unlock);
            AddTransition(State.LOCKED, Event.PASS, State.LOCKED, alarm);
            AddTransition(State.UNLOCKED, Event.COIN, State.UNLOCKED, thankyou);
            AddTransition(State.UNLOCKED, Event.PASS, State.LOCKED, lockAction);
        }
        public void HandleEvent(Event e)
        {
            foreach (Transition transition in transitions)
            {
                if(state== transition.startState && e == transition.trigger)
                {
                    state = transition.endState;
                    transition.action();
                }
            }
        }
        private void AddTransition(State start,Event e,State end,Action action)
        {
            transitions.Add(new Transition(start,e,end,action));
        }
        private class Transition
        {
            public State startState;
            public Event trigger;
            public State endState;
            public Action action;
            public Transition(State startState, Event trigger, State endState, Action action)
            {
                this.startState = startState;
                this.trigger = trigger;
                this.endState = endState;
                this.action = action;
            }
        }
    }
}