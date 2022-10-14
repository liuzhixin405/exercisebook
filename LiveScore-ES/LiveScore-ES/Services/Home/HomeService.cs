using LiveScore_ES.Backend.DAL;
using LiveScore_ES.Backend.ReadModel.Dto;
using LiveScore_ES.Backend.Services;
using LiveScore_ES.Framework;
using LiveScore_ES.Framework.Events;
using LiveScore_ES.ViewModels.Home;

namespace LiveScore_ES.Services.Home
{
    public class HomeService
    {
        private readonly EventRepository _eventRepository;
        private readonly WaterPoloContext _dbContext;
        public HomeService(EventRepository eventRepository, WaterPoloContext waterPoloContext)
        {
            _eventRepository = eventRepository;
            _dbContext = waterPoloContext;
        }

        public void DispatchCommand(string matchId,string eventName)
        {
            //// Log event unless it is UNDO
            //var repo = new EventRepository();

            //if (eventName == "Zap")
            //{
            //    repo.Empty(matchId);
            //    ZapSnapshots(matchId);
            //    return;
            //}
            if (eventName == "start")
            {
                var domainEvent = new MatchStartedEvent(matchId);
                Bus.Send(domainEvent);
            }
            //if (eventName == "Undo")
            //    _eventRepository.UndoLastAction(matchId);
            //else
            //    _eventRepository.RecordEvent(matchId, eventName);
            //repo.Commit();

            // Update snapshot for live scoring
            UpdateSnapshots(matchId);
        }
        public MatchViewModel GetCurrentState(string matchId)
        {
            var events = _eventRepository.GetEventStreamFor(matchId);
            var matchInfo = EventHelper.PlayEvents(matchId, events.ToList());
            return new MatchViewModel(matchInfo);
        }
        public void UpdateSnapshots(string matchId)
        {
            var events = _eventRepository.GetEventStreamFor(matchId);
            var matchInfo = EventHelper.PlayEvents(matchId, events);
            var lm = (from m in _dbContext.Matches where m.Id==matchId select m).FirstOrDefault();
            if (lm == null)
            {
                var liveMatch = new LiveMatch
                {
                    Id = matchId,
                    Team1 = matchInfo.Team1,
                    Team2 = matchInfo.Team2,
                    State = matchInfo.State,
                    IsBallInPlay = matchInfo.IsBallInPlay,
                    CurrentScore = matchInfo.CurrentScore,
                    CurrentPeriod = matchInfo.CurrentPeriod,
                    TimeInPeriod = 0
                };
                _dbContext.Matches.Add(liveMatch);
            }
            else
            {
                lm.State = matchInfo.State;
                lm.IsBallInPlay = matchInfo.IsBallInPlay;
                lm.CurrentScore = matchInfo.CurrentScore;
                lm.CurrentPeriod = matchInfo.CurrentPeriod;
                lm.TimeInPeriod = 0;
            }
            _dbContext.SaveChanges();
        }

        private void ZapSnapshots(String matchId)
        {
            var lm = (from m in _dbContext.Matches where m.Id == matchId select m).FirstOrDefault();
            if (lm != null)
            {
                _dbContext.Matches.Remove(lm);
            }
            _dbContext.SaveChanges();
        }
    }
}
