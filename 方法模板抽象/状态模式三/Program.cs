namespace 状态模式三
{
    internal class Program
    {
        static void Main(string[] args)
        { 
            Console.WriteLine("Hello, World!");
        }
    }

    public enum State { LOCKED,UNLOCKED}
    public enum Event { COIN,PASS}
    public interface TurnstileController
    {
        void Lock();
        void Unlock();
        void ThankYou();
        void Alarm();
    }
    public class Turnstile
    {
        internal State state = State.LOCKED;
        private TurnstileController turnstileController;
        public Turnstile(TurnstileController turnstileController)
        {
            this.turnstileController = turnstileController;
        }
        public void HandleEvent(Event e)
        {
            switch (state)
            {
                case State.LOCKED:
                    switch (e)
                    {
                        case Event.COIN:
                            state = State.UNLOCKED;
                            turnstileController.Unlock();
                            break;
                            case Event.PASS:
                            turnstileController.Alarm();
                            break;
                    }
                    break;
                    case State.UNLOCKED:
                    switch (e)
                    {
                        case Event.COIN:
                            turnstileController.ThankYou();
                            break;
                            case Event.PASS:
                            turnstileController.Lock();
                            break;
                    }
                    break;
            }
        }
    }
    
    public class TurnstileTest
    {
        private Turnstile turnstile;
        private TurnstileControllerSoof controllerSpoof;
        private class TurnstileControllerSoof : TurnstileController
        {
            public bool lockCalled = false;
            public bool unlockCalled = false;
            public bool alarmCalled = false;
            public bool thankyouCalled = false;

            public void Alarm()
            {
                alarmCalled= true;
            }

            public void Lock()
            {
                lockCalled= true;
            }

            public void ThankYou()
            {
                thankyouCalled= true;
            }

            public void Unlock()
            {
                unlockCalled = true;
            }
        }

        public void SetUp()
        {
            controllerSpoof= new TurnstileControllerSoof();
            turnstile = new Turnstile(controllerSpoof);
        }
        public void InitialConditions()
        {
            Console.WriteLine($"{State.LOCKED==turnstile.state}");
        }
        public void CoinInLockedState()
        {
            turnstile.state = State.LOCKED;
            turnstile.HandleEvent(Event.COIN);
            Console.WriteLine($"{State.UNLOCKED== turnstile.state}");
            Console.WriteLine($"{controllerSpoof.unlockCalled}");
        }
        public void CoinInUnlockedState()
        {
            turnstile.state = State.UNLOCKED;
            turnstile.HandleEvent(Event.COIN);
            Console.WriteLine($"{State.UNLOCKED == turnstile.state}");
            Console.WriteLine($"{controllerSpoof.thankyouCalled}");
        }

        public void PassInLockedState()
        {
            turnstile.state = State.LOCKED;
            turnstile.HandleEvent(Event.PASS);
            Console.WriteLine($"{State.LOCKED == turnstile.state}");
            Console.WriteLine($"{controllerSpoof.alarmCalled}");
        }

        public void PassInUnlockedState()
        {
            turnstile.state = State.UNLOCKED;
            turnstile.HandleEvent(Event.PASS);
            Console.WriteLine($"{State.LOCKED == turnstile.state}");
            Console.WriteLine($"{controllerSpoof.lockCalled}");
        }
    }
}