namespace PubSubConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            for (int i = 1; i < 10; i++)
            {
                new Publisher().Notification($"来自第{i}条消息,{DateTime.Now}");
            }
            Console.ReadLine(); 
        }
    }
}