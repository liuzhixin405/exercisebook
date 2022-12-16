using System;
using System.Diagnostics.Contracts;
using LiveScoreEs.Framework.Utils;

namespace NoSqlEvents.Backend.ReadModel
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

        public String Id { get; private set; }
        public String Team1 { get { return "Home"; } }
        public String Team2 { get { return "Visitors"; } }
        public Score CurrentScore { get; private set; }
        public Boolean IsBallInPlay { get; private set; }
        public Int32 CurrentPeriod { get; private set; }
        public MatchState State { get; private set; }
        public String Venue { get; set; }
        public DateTime Day { get; set; }   // deserves further thinking in relationship with Score/State

        #region Informational
        public Boolean IsInProgress()
        {
            return State == MatchState.InProgress;
        }
        public Boolean IsFinished()
        {
            return State == MatchState.Finished;
        }
        public Boolean IsScheduled()
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
        /// <summary>
        /// Starts the match
        /// </summary>
        /// <returns>this</returns>
        public Match Start()
        {
            State = MatchState.InProgress;
            return this;
        }

        /// <summary>
        /// Ends the match
        /// </summary>
        /// <returns></returns>
        public void Finish()
        {
            State = MatchState.Finished;
        }

        /// <summary>
        /// Scores a goal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Match Goal(TeamId id)
        {
            if (id == TeamId.Home)
            {
                CurrentScore = new Score(CurrentScore.TotalGoals1.Increment(),
                                         CurrentScore.TotalGoals2);
            }
            if (id == TeamId.Visitors)
            {
                CurrentScore = new Score(CurrentScore.TotalGoals1,
                                         CurrentScore.TotalGoals2.Increment());
            }

            //IsBallInPlay = false;
            return this;
        }

        /// <summary>
        /// Starts next period
        /// </summary>
        /// <returns>this</returns>
        public Match StartPeriod()
        {
            CurrentPeriod = CurrentPeriod.Increment(4);
            IsBallInPlay = true;
            return this;
        }

        /// <summary>
        /// Starts next period
        /// </summary>
        /// <returns>this</returns>
        public Match EndPeriod()
        {
            IsBallInPlay = false;

            if (CurrentPeriod == 4)
                Finish();

            return this;
        }

        /// <summary>
        /// Aborts the match 
        /// </summary>
        /// <returns>this</returns>
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
        /// <returns>Boolean</returns>
        [Pure]
        public Boolean CanFinishMatch()
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