using LiveScore_ES.Framework.Utils;
using System.Diagnostics.Contracts;

namespace LiveScore_ES.Backend.ReadModel
{
    public class Match
    {
        public Match(String id)
        {
            Id = id;
            State = MatchState.ToBePlayed;
            CurrentScore = new Score();
            Venue = String.Empty;
            Day = DateTime.Today;
            IsBallInPlay = false;
            CurrentPeriod = 0;
        }
        public string Id { get; private set; }
        public string Team1 => "Home";
        public string Team2 => "Visitors";
        public Score CurrentScore { get; private set; }
        public bool IsBallInPlay { get; private set; }
        public int CurrentPeriod { get; private set; }
        public MatchState State { get; private set; }
        public string Venue { get; set; } //场地
        public DateTime Day { get; set; } //值得进一步思考与分数/状态的关系

        #region informational
        public bool IsInProgress()
        {
            return State == MatchState.InProress;
        }
        public bool IsFinished()
        {
            return State == MatchState.Finished;
        }
        public bool IsScheduled()  //计划
        {
            return State == MatchState.ToBePlayed;
        }
        public override String ToString()
        {
            return IsScheduled()
                ? String.Format("{0} vs. {1}", Team1, Team2)
                : String.Format("{0} / {1}  {2}", Team1, Team2, CurrentScore);
        }
        #endregion

        #region Behavior
        public Match Start()
        {
            State = MatchState.InProress;
            return this;
        }

        public void Finish()
        {
            State = MatchState.Finished;
        }

        public Match Goal(TeamId id)
        {
           CurrentScore = new Score(id==TeamId.Home ? CurrentScore.TotalGoals1.Increment():CurrentScore.TotalGoals1,id== TeamId.Visitors? CurrentScore.TotalGoals2:CurrentScore.TotalGoals2.Increment());
            return this;
        }

        /// <summary>
        /// 开始下一轮
        /// </summary>
        /// <returns></returns>
        public Match StartPeriod()
        {
            CurrentPeriod = CurrentPeriod.Increment(4);
            IsBallInPlay = true;
            return this;

        }

        /// <summary>
        /// 结束下一轮
        /// </summary>
        /// <returns></returns>
        public Match EndPeriod()
        {
            IsBallInPlay = false;
            if (CurrentPeriod == 4)
                Finish();
            return this;
        }

        /// <summary>
        /// 暂停
        /// </summary>
        /// <returns></returns>
        public Match Abort()
        {
            EndPeriod();
            State = MatchState.Suspended;
            return this;
        }
        #endregion

        #region Private members
        /// <summary>
        /// Determines whether the match can be declared as finished. (Regular finish.)
        /// </summary>
        /// <returns></returns>
        [Pure]
        public bool CanFinishMatch()
        {
            if (State == MatchState.Finished || State == MatchState.Suspended)
                return true;

            if (IsInProgress() && CurrentPeriod == 4 && !IsBallInPlay)
                return true;

            return false;
        }
        #endregion
    }
}
