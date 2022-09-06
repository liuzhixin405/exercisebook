using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcPattern.Level2
{
    public class Demo
    {
       
    }

    public interface IView
    {
        void Print(string data);
    }

    public interface IModel
    {
        int[] Data { get; }
    }

    public class Controller
    {
        //v m

        private IList<IView> views = new List<IView>();

        private IModel model;

        public IModel Model { get => model; set => model = value; }

        public void Process()
        {
            if (views.Count == 0) return;
            if (model == null || model.Data.Length == 0) throw new ArgumentNullException($"{nameof(IModel)}未实现或者 空 ");
            string data =string.Join(",", Array.ConvertAll<int, string>(model.Data, x => x.ToString()));
            foreach (var view in views)
            {
                view.Print(data);
            }
        }

        public static Controller operator +(Controller controller,IView view)
        {
            if (view == null) throw new ArgumentNullException("view");
            controller.views.Add(view);
            return controller;
        }
        public static Controller operator -(Controller controller, IView view)
        {
            if (view == null) throw new ArgumentNullException("view");
            controller.views.Remove(view);
            return controller;
        }
    }
}
