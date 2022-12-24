using cat.Events;
using System.Runtime.CompilerServices;

namespace cat.Models
{
    public class Contract:Entity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }

        
        public static Contract CreateNew(string name)
        {
            return new Contract(name);
        }
        private Contract(string name)
        {
            Name = name;
            CreateTime = DateTime.Now;
            AddDomainEvent(new ContractCreatedEvent(name));
        }
    }
}
