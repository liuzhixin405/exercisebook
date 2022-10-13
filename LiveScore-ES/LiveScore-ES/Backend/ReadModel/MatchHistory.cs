namespace LiveScore_ES.Backend.ReadModel
{
    public class MatchHistory
    {
        public static MatchHistory New(String id)
        {
            return new MatchHistory(id);
        }
        public MatchHistory(string id)
        {
            Id = id;
            Records = new List<string>();
        }
        public string Id { get; private set; }
        public IList<String> Records { get; private set; }
    }
}
