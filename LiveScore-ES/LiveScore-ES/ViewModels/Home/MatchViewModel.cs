using LiveScore_ES.Backend.ReadModel;

namespace LiveScore_ES.ViewModels.Home
{
    public class MatchViewModel
    {
        public MatchViewModel(Match currentMatchjG)
        {
            CurrentMatchjG = currentMatchjG;
        }

        public Match CurrentMatchjG { get; private set; }
    }
}
