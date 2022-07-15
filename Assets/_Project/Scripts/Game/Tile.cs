using UnityEngine;
using System.Collections.Generic;

namespace TestProject
{
    /// <summary>
    /// The base class used for the tiles in the game
    /// </summary>
    public abstract class Tile : MonoBehaviour
    {
        [HideInInspector] public GameManager GM;
        [HideInInspector] public int x;
        [HideInInspector] public int y;


        public bool destructable;

        public abstract List<GameObject> Explode();
        public abstract void UpdateGameState(GameState state);

        public virtual void OnEnable()
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                Color newColor = spriteRenderer.color;
                newColor.a = 1.0f;
                spriteRenderer.color = newColor;
            }

            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            transform.localRotation = Quaternion.identity;
        }

        public virtual void OnDisable()
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                Color newColor = spriteRenderer.color;
                newColor.a = 1.0f;
                spriteRenderer.color = newColor;
            }

            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            transform.localRotation = Quaternion.identity;
        }
    }
}
