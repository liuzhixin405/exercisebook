namespace MementoPattern04
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Originator originator = new Originator();
            originator.IncreaseY();
            originator.DecreaseX();
            originator.SaveCheckpoint(1);
            originator.DecreaseX();
            originator.SaveCheckpoint(2);

            Originator originator2 = new Originator();
            originator2.UpdateX(12);
            originator2.SaveCheckpoint(3);

            originator.Undo(1);
            originator2.Undo(3);
            //todo
        }
    }
}