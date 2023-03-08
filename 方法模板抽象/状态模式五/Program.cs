namespace 状态模式五
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
    public interface TurnstileState
    {
        void Coin(Turnstile t);
        void Pass(Turnstile t);
    }
    internal class LockedTurnstileState : TurnstileState
    {
        public void Coin(Turnstile t)
        {
            t.SetUnlocked();
            t.Unlock();
        }

        public void Pass(Turnstile t)
        {
            t.Alarm();
        }
    }
    internal class UnlockedTurnstileState : TurnstileState
    {
        public void Coin(Turnstile t)
        {
            t.Thankyou();
        }

        public void Pass(Turnstile t)
        {
            t.SetLocked();
            t.Lock();
        }
    }
    public class Turnstile
    {
        internal static TurnstileState lockedState = new LockedTurnstileState();

        internal static TurnstileState unlockedState = new UnlockedTurnstileState();
        private TurnstileController turnstileController;
        internal TurnstileState state = unlockedState;
        public Turnstile(TurnstileController controller)
        {
            turnstileController = controller;
        }
        public void Coin()
        {
            state.Coin(this);
        }
        public void Pass()
        {
            state.Pass(this);
        }

        public void SetLocked()
        {
            state = lockedState;
        }

        public void SetUnlocked()
        {
            state = unlockedState;
        }
        public bool IsLocked()
        {
            return state == lockedState;
        }

        public bool IsUnlocked()
        {
            return state == unlockedState;
        }

        internal void Thankyou()
        {
            turnstileController.ThankYou();
        }
        internal void Alarm()
        {
            turnstileController.Alarm();
        }
        internal void Lock()
        {
            turnstileController.Lock();
        }
        internal void Unlock()
        {
            turnstileController.Unlock();
        }
    }
}