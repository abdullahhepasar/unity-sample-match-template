using UnityEngine;
using System.Collections.Generic;

namespace TestProject
{
    /// <summary>
    /// The available drop colors.
    /// </summary>
    public enum DropColor
    {
        Yellow,
        Blue,
        Green,
        Red
    }

    /// <summary>
    /// The Drop IDs
    /// </summary>
    public enum DropType
    {
        YellowDrop,
        BlueDrop,
        GreenDrop,
        RedDrop,
        RandomDrop
    }

    /// <summary>
    ///  The base class of Drops
    /// </summary>
    public class Drop : Tile
    {
        public DropColor color;

        /// <summary>
        /// Returns a list containing all the tiles destroyed when this tile explodes
        /// </summary>
        public override List<GameObject> Explode()
        {
            if (gameObject.activeSelf && GetComponent<Animator>() != null)
                GetComponent<Animator>().SetTrigger("Kill");

            return new List<GameObject> { gameObject };
        }

        /// <summary>
        /// Updates the game state when this tile explodes
        /// </summary>
        /// <param name="state"></param>
        public override void UpdateGameState(GameState state)
        {
            state.AddDrop(color);
        }
    }
}
