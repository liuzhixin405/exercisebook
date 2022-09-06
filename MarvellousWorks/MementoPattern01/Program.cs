using MementoPattern01.Service;
using System.Diagnostics;

namespace MementoPattern01
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IMemento<Position> memento = new Memento();
            Originator originator = new Originator();
<<<<<<< HEAD
            IMemento<Position> m1 = originator.Memento;

            originator.IncreaseY();
            originator.DecreaseX();
            originator.Memento = m1;
=======
            memento = originator.Memento;
            originator.IncX(1);
            originator.UpdateY(4);
            Console.WriteLine($"updated postion.X={originator.Memento.State.X} , position.Y={originator.Memento.State.Y}");

            originator.Memento = memento;
            Console.WriteLine($"reset postion.X={originator.Memento.State.X} , position.Y={originator.Memento.State.Y}");
>>>>>>> d179c7caab6f3422775cc62668a0b86288252a6d
        }
    }
}