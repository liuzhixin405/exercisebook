using DecoratorPattern;
using System.Drawing;

namespace DecoratorPattern2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IText text = new TextObject();
            text = new BoldDecorator(text);
            text = new ColorDecorator(text);
            ColorState newClorState = new ColorState();
            newClorState.Color = Color.Red;
            IDecorator root = (IDecorator)text;
            root.Refresh<ColorDecorator>(newClorState);

            BoldState newBoldState = new BoldState();
            newBoldState.IsBold = true;
            root.Refresh<BoldDecorator>(newBoldState);

            //build
            IText tn = new TextObject();
            tn = (new DecoratorBuilder()).BuilUp(tn);
            Console.Write($"tn=>{tn.Content}");
        }
    }
}