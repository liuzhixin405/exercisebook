namespace CommandPattern01
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Receiver receiver = new Receiver();
            ICommand commandSetName = new SetNameCommand();
            ICommand commandSetAddress = new SetAddressCommand();
            commandSetAddress.Recceiver = receiver;
            commandSetName.Recceiver = receiver;

            Invoker invoker = new Invoker();
            invoker.AddCommand(commandSetAddress);
            invoker.AddCommand(commandSetName);
            invoker.Run();
        }
    }
}