using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;

namespace TestProject
{
    /// <summary>
    /// This class stores the pools of the tiles used in the game
    /// </summary>
    public class TilePool : MonoBehaviour
    {
        public ObjectPool DropYellowPool;
        public ObjectPool DropBluePool;
        public ObjectPool DropGreenPool;
        public ObjectPool DropRedPool;

        public ObjectPool TileBGLightPool;
        public ObjectPool TileBGDarkPool;

        private readonly List<ObjectPool> drops = new List<ObjectPool>();


        private void Awake()
        {
            Assert.IsNotNull(DropYellowPool);
            Assert.IsNotNull(DropBluePool);
            Assert.IsNotNull(DropGreenPool);
            Assert.IsNotNull(DropRedPool);

            drops.Add(DropYellowPool);
            drops.Add(DropBluePool);
            drops.Add(DropGreenPool);
            drops.Add(DropRedPool);
        }

        /// <summary>
        /// Returns the pool of the drop color
        /// </summary>
        /// <param name="color">The drop color.</param>
        public ObjectPool GetDropPool(DropColor color)
        {
            return drops[(int)color];
        }
    }
}