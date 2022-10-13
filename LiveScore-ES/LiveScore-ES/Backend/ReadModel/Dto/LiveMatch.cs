namespace LiveScore_ES.Backend.ReadModel.Dto
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
        public string Id { get; set; }
        public string Team1 { get; set; }
        public string Team2 { get; set; }
        public Score CurrentScore { get; set; }
        public bool IsBallInPlay { get; set; }
        public int CurrentPeriod { get; set; }
        public int TimeInPeriod { get; set; }
        public MatchState State { get; set; }
    }
}
