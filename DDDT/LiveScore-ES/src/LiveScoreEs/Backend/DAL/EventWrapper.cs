using LiveScoreEs.Framework;

namespace LiveScoreEs.Backend.DAL
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