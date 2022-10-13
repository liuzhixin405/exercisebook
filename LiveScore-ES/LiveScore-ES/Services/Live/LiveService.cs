using LiveScore_ES.Backend.DAL;
using LiveScore_ES.Backend.ReadModel.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace LiveScore_ES.Services.Live
{
    public class LiveService
    {
        private readonly WaterPoloContext _db;
        public LiveService(WaterPoloContext db)
        {
            _db = db;
        }
        public LiveMatch GetLiveMatch(string matchId)
        {
            using(var db = _db)
            {
                var lm = (from m in db.Matches where m.Id==matchId select m).FirstOrDefault();
                if(lm==null)
                    return new LiveMatch() { Id=matchId};
                return lm;
            }
        }
    }
}
