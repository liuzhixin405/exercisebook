using cat.Domains;

namespace cat.Events
{
    public class ContractCreatedEvent:DomainEventBase
    {
    
        public string ContractName { get; set; }
        public ContractCreatedEvent( string contractName)
        {
            ContractName = contractName;
        }
    }
}
