using System.Collections.Generic;

namespace TestProject
{
    public abstract class MatchDetector
    {
        public abstract List<Match> DetectMatches(GameManager GM);
    }
}
