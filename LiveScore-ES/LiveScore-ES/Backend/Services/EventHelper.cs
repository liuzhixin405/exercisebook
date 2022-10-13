using LiveScore_ES.Framework.Events;
using LiveScore_ES.Framework;
using LiveScore_ES.Backend.ReadModel;

namespace LiveScore_ES.Backend.Services
{
    public class EventHelper
    {
        public static Match PlayEvents(String id, IList<DomainEvent> events)
        {
            var match = new Match(id);

            foreach (var e in events)
            {
                if (e == null)
                    return match;

                if (e is MatchStartedEvent)
                    match.Start();

                //var name = e.EventName.ToUpper();
                //switch (name)
                //{
                //    case "START":
                //        match.Start();
                //        break;
                //    case "END":
                //        match.Finish();
                //        break;
                //    case "NEWPERIOD":
                //        match.StartPeriod();
                //        break;
                //    case "ENDPERIOD":
                //        match.EndPeriod();
                //        break;
                //    case "GOAL1":
                //        match.Goal(TeamId.Home);
                //        break;
                //    case "GOAL2":
                //        match.Goal(TeamId.Visitors);
                //        break;
                //}
            }

            return match;
        }
    }
}
