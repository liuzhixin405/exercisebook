using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveScore_ES.Backend.ReadModel.Dto
{
    [Table("LiveMatch")]
    public class LiveMatch
    {
        public LiveMatch()
        {
            Id = Guid.NewGuid().ToString();
            State = MatchState.ToBePlayed;
            CurrentScore = new Score();
            IsBallInPlay = false;
            CurrentPeriod = 0;
        }
        [Key]
        public string Id { get; set; }
        public string Team1 { get; set; }
        public string Team2 { get; set; }
        [NotMapped]
        public Score CurrentScore { get; set; }
        public bool IsBallInPlay { get; set; }
        public int CurrentPeriod { get; set; }
        public int TimeInPeriod { get; set; }
        public MatchState State { get; set; }
    }
}
