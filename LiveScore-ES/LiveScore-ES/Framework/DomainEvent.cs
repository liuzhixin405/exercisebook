namespace LiveScore_ES.Framework
{
    public class DomainEvent:Message
    {
        public DateTime TimeStamp { get; private set; }
        public DomainEvent()
        {
            TimeStamp = DateTime.Now;
        }
    }
}
