using trnasfer;

namespace transfer;
public class Program
{
    static void Main(String[] args)
    {
        Account account1 = new Account(1, 12);
        account1.ChangedEvent += DepositEvent;
        Account account2 = new Account(2, 22);
        account2.ChangedEvent += DepositEvent;
        account1.ADeposit(1);
        account1.Transfer(2, account2);
        account1.AWithdraw(5);

    }

    private static void DepositEvent(object sender, IEvent args)
    {
        if (args is DepositArgs)
            Console.WriteLine($"id={((DepositArgs)args).Id},deposit=>{((DepositArgs)args).Amount}");
        else
            Console.WriteLine($"id={((WithdrawArgs)args).Id},withdraw=>{((WithdrawArgs)args).Amount}");
    }
}

