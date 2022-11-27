namespace TemplateDesign
{
    internal class Program
    {
        static void Main(string[] args)
        {
            #region one
            //bool done = false;
            //while (!done)
            //{
            //    string fahrString = Console.In.ReadLine();
            //    if (fahrString == null || fahrString.Length == 0)
            //        done = true;
            //    else
            //    {
            //        double fahr = double.Parse(fahrString);
            //        double celcius = 5.0 / 9.0 * (fahr - 32);
            //        Console.Out.WriteLine($"F={fahr},C={celcius}");
            //    }
            //}
            //Console.Out.WriteLine("ftoc exit"); 
            #endregion

            #region two
            new FtoCTemplateMethod().Run();
            #endregion
        }
    }

    public abstract class Application
    {
        private bool isDone = false;
        protected abstract void Init();
        protected abstract void Idle();
        protected abstract void Cleanup();
        protected void SetDone()=>isDone = true;
        
        protected bool Done()=>isDone;
        public void Run()
        {
            Init();
            while (!Done())
                Idle();
            Cleanup();
        }

    }

    public class FtoCTemplateMethod : Application
    {
        private TextReader input;
        private TextWriter output;
        protected override void Cleanup()
        {
            output.WriteLine("ftoc exit");
        }

        protected override void Idle()
        {

            string fahrString = input.ReadLine();
            if (fahrString == null || fahrString.Length == 0)
                SetDone();
            else
            {
                double fahr = double.Parse(fahrString);
                double celcius = 5.0 / 9.0 * (fahr - 32);
                output.WriteLine($"F={fahr},C={celcius}");
            }

        }

        protected override void Init()
        {
            input = Console.In;
            output = Console.Out;
        }
    }
}