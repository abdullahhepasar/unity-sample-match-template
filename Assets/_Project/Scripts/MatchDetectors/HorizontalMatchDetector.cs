using UnityEngine;
using System.Collections.Generic;

namespace TestProject
{
    public class HorizontalMatchDetector : MatchDetector
    {
        /// <summary>
        /// Returns the list of detected matches
        /// </summary>
        /// <param name="GM">The game board.</param>
        public override List<Match> DetectMatches(GameManager GM)
        {
            List<Match> matches = new List<Match>();

            for (int j = 0; j < GM.level.height; j++)
            {
                for (int i = 0; i < GM.level.width - 2;)
                {
                    GameObject tile = GM.GetTile(i, j);
                    if (tile != null && tile.GetComponent<Drop>() != null)
                    {
                        DropColor color = tile.GetComponent<Drop>().color;
                        if (GM.GetTile(i + 1, j) != null && GM.GetTile(i + 1, j).GetComponent<Drop>() != null &&
                            GM.GetTile(i + 1, j).GetComponent<Drop>().color == color &&
                            GM.GetTile(i + 2, j) != null && GM.GetTile(i + 2, j).GetComponent<Drop>() != null &&
                            GM.GetTile(i + 2, j).GetComponent<Drop>().color == color)
                        {
                            Match match = new Match();
                            match.type = MatchType.Horizontal;
                            do
                            {
                                match.AddTile(GM.GetTile(i, j));
                                i += 1;
                            } while (i < GM.level.width && GM.GetTile(i, j) != null &&
                                     GM.GetTile(i, j).GetComponent<Drop>() != null &&
                                     GM.GetTile(i, j).GetComponent<Drop>().color == color);

                            matches.Add(match);
                            continue;
                        }
                    }

                    i += 1;
                }
            }

            return matches;
        }
    }
}
