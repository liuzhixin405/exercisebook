using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcPattern.Level3;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcPattern.TestMvc.Level3
{
    [TestClass]
    public class TestsDemo
    {
        [TestMethod]
        public void Test()
        {
            Controller controller = new Controller();
            IModel model = new Model();
            controller.Model = model;
            controller += new TraceView();
            controller += new ConsoleView();

            model[1] = 100;
            model[5] = 1000;
        }
    }

    class Model : IModel
    {
        public int this[int index]
        {
            get
            {
                return data[index];

            }
            set
            {
                this.data[index] = value;
                if (DataChanged != null)
                    DataChanged(this, new ModelEventArgs(data));
            }
        }
        public Model()
        {
            Random random = new Random();
            data = new int[10];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = random.Next() % 1023;
            }

        }
        public event EventHandler<ModelEventArgs> DataChanged;
        private int[] data;
    }
    abstract class ViewBase : IView
    {
        public EventHandler<ModelEventArgs> Handler => OnDataCHanged;

        public abstract void Print(string data);
        public virtual void OnDataCHanged(object sender, ModelEventArgs args)
        {
            Print(args.Context);
        }
    }

    class TraceView : ViewBase
    {
        public override void Print(string data)
        {
            Trace.WriteLine(data);
        }
    }
    class ConsoleView : ViewBase
    {
        public override void Print(string data)
        {
            Console.WriteLine(data);
        }
    }
}
