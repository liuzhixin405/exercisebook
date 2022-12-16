using System;
using System.Collections.Generic;

namespace LiveScoreEs.Backend.ReadModel
{
    public class MatchHistory
    {
        public static MatchHistory New(String id)
        {
            return new MatchHistory(id);
        }

        public MatchHistory(String id)
        {
            Id = id;
            Records = new List<String>();
        }

        public String Id { get; private set; }
        public IList<String> Records { get; private set; }
    }
}