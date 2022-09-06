using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcPattern.Level3
{
    public class Demo
    {
    }

    public class ModelEventArgs : EventArgs
    {
        private string content;
        public string Context { get { return this.content; } }

        public ModelEventArgs(int[] data)
        {
            this.content = string.Join(",", Array.ConvertAll<int, string>(data, x => x.ToString()));
        }
    }
    public interface IModel
    {
        event EventHandler<ModelEventArgs> DataChanged;
        int this[int index] { get;set; }
    }

    public interface IView
    {
        EventHandler<ModelEventArgs> Handler { get; }
        void Print(string data);
    }

    public class Controller
    {
        private IModel model;

        public virtual IModel Model { get => model; set => model = value; }
        public static Controller operator +(Controller controller,IView view)
        {
            if (view == null) throw new ArgumentNullException("view");
            controller.Model.DataChanged += view.Handler;
            return controller;
        }
        public static Controller operator -(Controller controller, IView view)
        {
            if (view == null) throw new ArgumentNullException("view");
            controller.Model.DataChanged -= view.Handler;
            return controller;
        }
    }
}
