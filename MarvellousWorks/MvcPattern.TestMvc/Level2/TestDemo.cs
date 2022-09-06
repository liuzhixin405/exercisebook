using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcPattern.Level2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcPattern.TestMvc.Level2
{
    [TestClass]
    public class TestDemo
    {

        [TestMethod]
        public void Test()
        {
            Controller controller = new Controller();
            controller.Model = new Randomizer();
            controller += new ConsoleView();
            controller += new TraceView();
            controller.Process();

        }

    }
    class Randomizer : IModel
    {
        public int[] Data
        {
            get
            {
                Random random = new Random();
                int[] result = new int[10];
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = random.Next() % 1023;
                }
                return result;
            }
        }
    }
    class ConsoleView : IView
    {
        public void Print(string data)
        {
            Console.WriteLine(data);
        }
    }

    class TraceView : IView
    {
        public void Print(string data)
        {
            Trace.WriteLine(data);
        }
    }
}
