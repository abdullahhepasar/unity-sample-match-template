using UnityEngine;
using System.Collections.Generic;

namespace TestProject
{
    /// <summary>
    /// The available limit types
    /// </summary>
    public enum LimitType
    {
        Moves
    }

    public enum ElementType
    {
        None
    }

    /// <summary>
    /// This class stores the settings of a game level
    /// </summary>
    public class Level : MonoBehaviour
    {
        public int id;

        public int width;
        public int height;
        public List<LevelTile> tiles = new List<LevelTile>();

        public LimitType limitType;
        public int limit;

        public List<DropColor> availableColors = new List<DropColor>();

        //The state of not falling again from the top of the drops in the column
        public List<int> columnRespawnLimits = new List<int>();
    }

    public class LevelTile
    {
        public ElementType elementType;
    }

    public class DropTile : LevelTile
    {
        public DropType type;
    }

    public class HoleTile : LevelTile
    {

    }
}
