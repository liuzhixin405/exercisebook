using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trnasfer
{
    public class Account
    {
        public Account(int id, decimal balance)
        {
            Id = id;
            Balance = balance;
        }
        public int Id { get; set; }
        public decimal Balance { get; set; }

        public bool AWithdraw(decimal amount)
        {
            Balance -= amount;
            ChangedEvent.Invoke(this, new WithdrawArgs(Id, amount));
            return true;
        }
        public bool ADeposit(decimal amount)
        {
            Balance += amount;
            ChangedEvent.Invoke(this, new DepositArgs(Id, amount));
            return true;
        }

        public event EventHandler<IEvent> ChangedEvent;
    }

    public static class Extensions
    {
        public static void Transfer(this Account account, decimal amount, Account accountTo)
        {
            account.AWithdraw(amount);
            accountTo.ADeposit(amount);
        }
    }
    public record IEvent { }
    public record WithdrawArgs(int Id, decimal Amount) : IEvent;
    public record DepositArgs(int Id, decimal Amount) : IEvent;
    public record TransfersArgs(decimal amount, Account Account) : IEvent;
}
