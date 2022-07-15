using UnityEngine;
using System.Collections.Generic;

namespace TestProject
{
    public class VerticalMatchDetector : MatchDetector
    {
        /// <summary>
        /// Returns the list of detected matches
        /// </summary>
        /// <param name="GM">The game board.</param>
        public override List<Match> DetectMatches(GameManager GM)
        {
            List<Match> matches = new List<Match>();

            for (int i = 0; i < GM.level.width; i++)
            {
                for (int j = 0; j < GM.level.height - 2;)
                {
                    GameObject tile = GM.GetTile(i, j);
                    if (tile != null && tile.GetComponent<Drop>() != null)
                    {
                        DropColor color = tile.GetComponent<Drop>().color;
                        if (GM.GetTile(i, j + 1) != null && GM.GetTile(i, j + 1).GetComponent<Drop>() != null &&
                            GM.GetTile(i, j + 1).GetComponent<Drop>().color == color &&
                            GM.GetTile(i, j + 2) != null && GM.GetTile(i, j + 2).GetComponent<Drop>() != null &&
                            GM.GetTile(i, j + 2).GetComponent<Drop>().color == color)
                        {
                            Match match = new Match();
                            match.type = MatchType.Vertical;
                            do
                            {
                                match.AddTile(GM.GetTile(i, j));
                                j += 1;
                            } while (j < GM.level.height && GM.GetTile(i, j) != null &&
                                     GM.GetTile(i, j).GetComponent<Drop>() != null &&
                                     GM.GetTile(i, j).GetComponent<Drop>().color == color);

                            matches.Add(match);
                            continue;
                        }
                    }

                    j += 1;
                }
            }

            return matches;
        }
    }
}
