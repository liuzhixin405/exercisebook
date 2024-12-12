namespace EF.GraphQL.Two.Models.Context
{
    public class SampleContext
    {
        public List<Person> Persons { get; set; }
        public List<Account> Accounts { get; set; }
        public SampleContext()
        {
            var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };

            Accounts = new List<Account>
            {
                new Account { Id=Guid.NewGuid(), Type= TypeOfAccount.Junior,Description="初代会员",PersonId=ids[0] },
                new Account { Id=Guid.NewGuid(), Type= TypeOfAccount.Intermediate,Description="中代会员",PersonId=ids[1]  },
                  new Account { Id=Guid.NewGuid(), Type= TypeOfAccount.Free,Description="无会员",PersonId=ids[1]  }
            };

            Persons = new List<Person>
            {
                new Person { Id =ids[0], Name = "张三", Address = "武穴市" },
                new Person { Id = ids[1], Name = "老六", Address = "大马士革" }
            };
        }
    }
}
