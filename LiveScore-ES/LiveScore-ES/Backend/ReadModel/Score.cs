namespace LiveScore_ES.Backend.ReadModel
{
    public sealed class Score
    {
        private const int MaxGoals = 99;
        public Score():this(0,0)
        {

        }
        public Score(int goals1=0,int goals2=0)
        {
            TotalGoals1 = goals1;
            TotalGoals2 = goals2;
        }
        public int TotalGoals1 { get; private set; }
        public int TotalGoals2 { get; private set; }

        #region Informational
        /// <summary>
        /// 比分
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return String.Format("{0} - {1}", TotalGoals1, TotalGoals2);
        }
        /// <summary>
        /// 是否领先
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Boolean IsLeading(TeamId id)
        {
            if (id == TeamId.Home)
                return TotalGoals1 > TotalGoals2;
            if (id == TeamId.Visitors)
                return TotalGoals2 > TotalGoals1;
            return false;
        }
        #endregion

        public override bool Equals(object? obj)
        {
            var otherScore = obj as Score;
            if(otherScore==null) return false;
            return otherScore.TotalGoals1==TotalGoals1 && otherScore.TotalGoals2==TotalGoals2;
        }
        public override int GetHashCode()
        {
            return (TotalGoals1 ^ TotalGoals2).GetHashCode();
        }
    }
}
