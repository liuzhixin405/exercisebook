using System;

namespace NoSqlEvents.Backend.ReadModel.Dto
{
    public class LiveMatch  
    {
        public LiveMatch()
        {
            Id = "";
            State = MatchState.ToBePlayed;
            CurrentScore = new Score();
            IsBallInPlay = false;
            CurrentPeriod = 0;
        }

        public String Id { get; set; }
        public String Team1 { get; set; }
        public String Team2 { get; set; }
        public Score CurrentScore { get; set; }
        public Boolean IsBallInPlay { get; set; }
        public Int32 CurrentPeriod { get; set; }
        public Int32 TimeInPeriod { get; set; }
        public MatchState State { get; set; }
    }
}