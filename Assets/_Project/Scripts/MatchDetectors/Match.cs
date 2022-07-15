using UnityEngine;
using System.Collections.Generic;

namespace TestProject
{
    public enum MatchType
    {
        Horizontal,
        Vertical,
        TShaped,
        LShaped
    }

    public class Match
    {
        public MatchType type;

        public readonly List<GameObject> tiles = new List<GameObject>();

        public void AddTile(GameObject tile)
        {
            tiles.Add(tile);
        }
    }
}
