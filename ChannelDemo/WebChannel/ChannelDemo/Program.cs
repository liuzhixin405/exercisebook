for (int i = 0; i < 10; i++)
{
    Console.WriteLine(i);
    //SendNotification();
    Task.Run(() => SendNotification());

}
Console.Read();
void SendNotification()
{
    Task.Delay(1000).GetAwaiter().GetResult();
    Console.WriteLine("Send Complete");
}