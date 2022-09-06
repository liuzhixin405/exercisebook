namespace MementoPattern03
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Originator originator = new Originator();

            originator.SaveCheckpoint(); //初始化
            originator.IncreaseY();
            originator.DecreaseX();
            originator.SaveCheckpoint();
            originator.IncreaseY();
            originator.SaveCheckpoint();
            originator.Undo(); //回复
            originator.Undo(); //回复
            originator.Undo(); //回复
        }
    }
}