using System;
using System.Collections.Generic;
using LiveScoreEs.Framework;
using LiveScoreEs.Framework.Events;
using NoSqlEvents.Backend.ReadModel;

namespace LiveScoreEs.Backend.Services
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