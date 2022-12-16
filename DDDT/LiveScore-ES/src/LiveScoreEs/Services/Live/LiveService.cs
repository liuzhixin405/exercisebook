using System;
using System.Linq;
using LiveScoreEs.Backend.DAL;
using NoSqlEvents.Backend.ReadModel.Dto;

namespace LiveScoreEs.Services.Live
{
    public class LiveService
    {
        public LiveMatch GetLiveMatch(String matchId)
        {
            using (var db = new WaterpoloContext())
            {
                var lm = (from m in db.Matches where m.Id == matchId select m).FirstOrDefault();
                if (lm == null)
                    return new LiveMatch() {Id = matchId};
                return lm;
            }
        }

    }
}