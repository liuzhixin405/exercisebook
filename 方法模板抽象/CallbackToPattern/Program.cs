namespace CallbackToPattern
{
    #region callback
    //internal class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        ServiceA serviceA = new ServiceA();
    //        serviceA.DoWorkA(() =>
    //        {
    //            ServiceB serviceB = new ServiceB();
    //            serviceB.DoWorkB(() =>
    //            {
    //                ServiceC serviceC = new ServiceC();
    //                serviceC.DoWorkC(() =>
    //                {
    //                    Console.WriteLine("All work done!");
    //                });
    //            });
    //        });
    //    }
    //}

    //class ServiceA
    //{
    //    public void DoWorkA(Action callback)
    //    {
    //        Console.WriteLine("Service A is working...");
    //        callback();
    //    }
    //}
    //class ServiceB
    //{
    //    public void DoWorkB(Action callback)
    //    {
    //        Console.WriteLine("Service B is working...");
    //        callback();
    //    }
    //}
    //class ServiceC
    //{
    //    public void DoWorkC(Action callback)
    //    {
    //        Console.WriteLine("Service C is working...");
    //        callback();
    //    }
    //} 
    #endregion
    
    internal class Program
    {
        static void Main(string[] ages)
        {
            ServiceA serviceA = new ServiceA();
            serviceA.WorkCompleted += ServiceA_WorkCompleted;
            serviceA.DoWorkA();
        }

        private static void ServiceA_WorkCompleted(object? sender, EventArgs e)
        {
            ServiceB serviceB = new ServiceB();
            serviceB.WorkCompleted += ServiceB_WorkCompleted;
            serviceB.DoWorkB();
        }

        private static void ServiceB_WorkCompleted(object? sender, EventArgs e)
        {
            ServiceC serviceC = new ServiceC();
            serviceC.WorkCompleted += ServiceC_WorkCompleted;
            serviceC.DoWorkC();
        }

        private static void ServiceC_WorkCompleted(object? sender, EventArgs e)
        {
            Console.WriteLine("All work done!");
        }
    }
   
    class ServiceA
    {
        public event EventHandler WorkCompleted;

        public void DoWorkA()
        {
            Console.WriteLine("Service A is working...");
            OnWorkCompleted();
        }

        protected virtual void OnWorkCompleted()
        {
            WorkCompleted?.Invoke(this, EventArgs.Empty);
        }


    }

    class ServiceB
    {
        public event EventHandler WorkCompleted;

        public void DoWorkB()
        {
            Console.WriteLine("Service B is working...");
            OnWorkCompleted();
        }

        protected virtual void OnWorkCompleted()
        {
            WorkCompleted?.Invoke(this, EventArgs.Empty);
        }


    }

    class ServiceC
    {
        public event EventHandler WorkCompleted;

        public void DoWorkC()
        {
            Console.WriteLine("Service C is working...");
            OnWorkCompleted();
        }

        protected virtual void OnWorkCompleted()
        {
            WorkCompleted?.Invoke(this, EventArgs.Empty);
        }


    }
}
