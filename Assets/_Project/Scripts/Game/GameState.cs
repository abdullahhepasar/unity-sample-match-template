using System;
using System.Collections.Generic;

namespace TestProject
{
    /// <summary>
    /// This class stores the state of game
    /// </summary>
    public class GameState
    {
        public int score;
        public Dictionary<DropColor, int> collectedDrops = new Dictionary<DropColor, int>();

        /// <summary>
        /// Resets the game state to its initial state
        /// </summary>
        public void Reset()
        {
            score = 0;

            collectedDrops.Clear();
            foreach (DropColor value in Enum.GetValues(typeof(DropColor)))
            {
                collectedDrops.Add(value, 0);
            }
        }

        public void AddDrop(DropColor drop)
        {
            collectedDrops[drop] += 1;
        }
    }
}
