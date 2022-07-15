using UnityEngine;
using System.Collections.Generic;

namespace TestProject
{
    public class LshapedMatchDetector : MatchDetector
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
                            GM.GetTile(i + 2, j).GetComponent<Drop>().color == color &&
                            GM.GetTile(i, j + 1) != null && GM.GetTile(i, j + 1).GetComponent<Drop>() != null &&
                            GM.GetTile(i, j + 1).GetComponent<Drop>().color == color &&
                            GM.GetTile(i, j + 2) != null && GM.GetTile(i, j + 2).GetComponent<Drop>() != null &&
                            GM.GetTile(i, j + 2).GetComponent<Drop>().color == color)
                        {
                            Match match = new Match();
                            match.type = MatchType.LShaped;
                            match.AddTile(GM.GetTile(i, j));
                            match.AddTile(GM.GetTile(i + 1, j));
                            match.AddTile(GM.GetTile(i + 2, j));
                            match.AddTile(GM.GetTile(i, j + 1));
                            match.AddTile(GM.GetTile(i, j + 2));
                            matches.Add(match);

                            int k = i + 3;
                            while (k < GM.level.width && GM.GetTile(k, j) != null &&
                                   GM.GetTile(k, j).GetComponent<Drop>() != null &&
                                   GM.GetTile(k, j).GetComponent<Drop>().color == color)
                            {
                                match.AddTile(GM.GetTile(k, j));
                                k += 1;
                            }

                            k = j + 3;
                            while (k < GM.level.height && GM.GetTile(i, k) != null &&
                                   GM.GetTile(i, k).GetComponent<Drop>() != null &&
                                   GM.GetTile(i, k).GetComponent<Drop>().color == color)
                            {
                                match.AddTile(GM.GetTile(i, k));
                                k += 1;
                            }
                        }

                        if (GM.GetTile(i + 1, j) != null && GM.GetTile(i + 1, j).GetComponent<Drop>() != null &&
                            GM.GetTile(i + 1, j).GetComponent<Drop>().color == color &&
                            GM.GetTile(i + 2, j) != null && GM.GetTile(i + 2, j).GetComponent<Drop>() != null &&
                            GM.GetTile(i + 2, j).GetComponent<Drop>().color == color &&
                            GM.GetTile(i, j - 1) != null && GM.GetTile(i, j - 1).GetComponent<Drop>() != null &&
                            GM.GetTile(i, j - 1).GetComponent<Drop>().color == color &&
                            GM.GetTile(i, j - 2) != null && GM.GetTile(i, j - 2).GetComponent<Drop>() != null &&
                            GM.GetTile(i, j - 2).GetComponent<Drop>().color == color)
                        {
                            Match match = new Match();
                            match.type = MatchType.LShaped;
                            match.AddTile(GM.GetTile(i, j));
                            match.AddTile(GM.GetTile(i + 1, j));
                            match.AddTile(GM.GetTile(i + 2, j));
                            match.AddTile(GM.GetTile(i, j - 1));
                            match.AddTile(GM.GetTile(i, j - 2));
                            matches.Add(match);

                            int k = i + 3;
                            while (k < GM.level.width && GM.GetTile(k, j) != null &&
                                   GM.GetTile(k, j).GetComponent<Drop>() != null &&
                                   GM.GetTile(k, j).GetComponent<Drop>().color == color)
                            {
                                match.AddTile(GM.GetTile(k, j));
                                k += 1;
                            }

                            k = j - 3;
                            while (k >= 0 && GM.GetTile(i, k) != null && GM.GetTile(i, k).GetComponent<Drop>() != null &&
                                   GM.GetTile(i, k).GetComponent<Drop>().color == color)
                            {
                                match.AddTile(GM.GetTile(i, k));
                                k -= 1;
                            }
                        }

                        if (GM.GetTile(i - 1, j) != null && GM.GetTile(i - 1, j).GetComponent<Drop>() != null &&
                            GM.GetTile(i - 1, j).GetComponent<Drop>().color == color &&
                            GM.GetTile(i - 2, j) != null && GM.GetTile(i - 2, j).GetComponent<Drop>() != null &&
                            GM.GetTile(i - 2, j).GetComponent<Drop>().color == color &&
                            GM.GetTile(i, j + 1) != null && GM.GetTile(i, j + 1).GetComponent<Drop>() != null &&
                            GM.GetTile(i, j + 1).GetComponent<Drop>().color == color &&
                            GM.GetTile(i, j + 2) != null && GM.GetTile(i, j + 2).GetComponent<Drop>() != null &&
                            GM.GetTile(i, j + 2).GetComponent<Drop>().color == color)
                        {
                            Match match = new Match();
                            match.type = MatchType.LShaped;
                            match.AddTile(GM.GetTile(i, j));
                            match.AddTile(GM.GetTile(i - 1, j));
                            match.AddTile(GM.GetTile(i - 2, j));
                            match.AddTile(GM.GetTile(i, j + 1));
                            match.AddTile(GM.GetTile(i, j + 2));
                            matches.Add(match);

                            int k = i - 3;
                            while (k >= 0 && GM.GetTile(k, j) != null && GM.GetTile(k, j).GetComponent<Drop>() != null &&
                                   GM.GetTile(k, j).GetComponent<Drop>().color == color)
                            {
                                match.AddTile(GM.GetTile(k, j));
                                k -= 1;
                            }

                            k = j + 3;
                            while (k < GM.level.height && GM.GetTile(i, k) != null &&
                                   GM.GetTile(i, k).GetComponent<Drop>() != null &&
                                   GM.GetTile(i, k).GetComponent<Drop>().color == color)
                            {
                                match.AddTile(GM.GetTile(i, k));
                                k += 1;
                            }
                        }

                        if (GM.GetTile(i - 1, j) != null && GM.GetTile(i - 1, j).GetComponent<Drop>() != null &&
                            GM.GetTile(i - 1, j).GetComponent<Drop>().color == color &&
                            GM.GetTile(i - 2, j) != null && GM.GetTile(i - 2, j).GetComponent<Drop>() != null &&
                            GM.GetTile(i - 2, j).GetComponent<Drop>().color == color &&
                            GM.GetTile(i, j - 1) != null && GM.GetTile(i, j - 1).GetComponent<Drop>() != null &&
                            GM.GetTile(i, j - 1).GetComponent<Drop>().color == color &&
                            GM.GetTile(i, j - 2) != null && GM.GetTile(i, j - 2).GetComponent<Drop>() != null &&
                            GM.GetTile(i, j - 2).GetComponent<Drop>().color == color)
                        {
                            Match match = new Match();
                            match.type = MatchType.LShaped;
                            match.AddTile(GM.GetTile(i, j));
                            match.AddTile(GM.GetTile(i - 1, j));
                            match.AddTile(GM.GetTile(i - 2, j));
                            match.AddTile(GM.GetTile(i, j - 1));
                            match.AddTile(GM.GetTile(i, j - 2));
                            matches.Add(match);

                            int k = i - 3;
                            while (k >= 0 && GM.GetTile(k, j) != null && GM.GetTile(k, j).GetComponent<Drop>() != null &&
                                   GM.GetTile(k, j).GetComponent<Drop>().color == color)
                            {
                                match.AddTile(GM.GetTile(k, j));
                                k -= 1;
                            }

                            k = j - 3;
                            while (k >= 0 && GM.GetTile(i, k) != null && GM.GetTile(i, k).GetComponent<Drop>() != null &&
                                   GM.GetTile(i, k).GetComponent<Drop>().color == color)
                            {
                                match.AddTile(GM.GetTile(i, k));
                                k -= 1;
                            }
                        }
                    }

                    i += 1;
                }
            }

            return matches;
        }
    }
}
