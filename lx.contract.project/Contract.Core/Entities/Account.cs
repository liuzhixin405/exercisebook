using Contract.SharedKernel;
using Contract.SharedKernel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.Core.Entities
{
    public class Account: EntityBase, IAggregateRoot
    {
        public Account(decimal balance)
        {
            Id = Guid.NewGuid().ToString();
            Balance = balance;
        }
        public Account() : this(0)
        {
        }
        public Account(string id, decimal balance)
        {
            Id = id;
            Balance = balance;
        }
        public string Id { get; set; }

        public decimal Balance { get; set; }
        private List<DomainEventBase> events = new List<DomainEventBase>();
        public IEnumerable<DomainEventBase> Events => events.AsReadOnly();
        public Account Create(decimal balance)
        {
            return this;
        }
        public void Witchdraw(decimal balance)
        {
            if (balance > Balance)
                throw new ArgumentNullException("没有这么多钱");
            Balance -= balance;

        }

        public Account Deposited(string id, decimal balance)
        {
            Balance += balance;
 
            return this;
        }

        public void Transfer(decimal balance, Account toAccount)
        {
            Balance -= balance;
            toAccount.Balance += balance;
        }
    }
}
