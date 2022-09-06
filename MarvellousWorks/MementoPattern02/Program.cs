namespace MementoPattern02
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Originator originator = new Originator();

            originator.SaveCheckpoint(); //初始化
            originator.IncreaseY();
            originator.DecreaseX();
            originator.Undo(); //回复
        }
    }
}