using LiveScore_ES.Framework;

namespace LiveScore_ES.Backend.DAL
{
    public class EventWrapper
    {
        public EventWrapper(DomainEvent theEvent)
        {
            TheEvent = theEvent;
        }
        public DomainEvent TheEvent { get; private set; }
    }
}
