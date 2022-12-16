using System;

namespace LiveScoreEs.Framework.Events
{
    public class MatchStartedEvent : DomainEvent
    {
        public MatchStartedEvent(String id)
        {
            MatchId = id;
            
            // Command class has SagaId property used to find saga the message relates to. 
            // (Simpler schema than more general ConfigureHowToFindSaga method in NServiceBus)
            SagaId = id;
        }

        public String MatchId { get; private set; }
    }
}