using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using FullSerializer;

namespace TestProject
{
    /// <summary>
    /// The base class of Gameplay System
    /// </summary>
    public class GameManager : MonoBehaviour
    {        
        [Header("Pool Manager Object")]
        [SerializeField] private TilePool tilePool;
        
        [Header("Offset Setting For The Playground")]
        [SerializeField] private Vector3 gameCenter = Vector3.zero;

        //Current Level Data
        [HideInInspector] public Level level;

        [HideInInspector] public GameState gameState = new GameState();

        
        private List<GameObject> tiles; //All Grid List
        private readonly List<Vector3> tilePositions = new List<Vector3>(); //All Grid Positions List
        private float tileW;    //Tile Width
        private float tileH;    //Tile Height

        private class Swap
        {
            public GameObject tileA;
            public GameObject tileB;
        }
        private List<Swap> possibleSwaps = new List<Swap>();

        private bool gameStarted;
        private bool inputLocked;   //Input Lock system. To lock the Input system in animation states
        private bool currentlySwapping; //Drop swapping system. To lock the Input system in animation states

        private bool drag;  //Screen touch resume detection
        private GameObject selectedTile; //Current Touched Drop

        //Returns the list of detected matches.
        private readonly MatchDetector horizontalMatchDetector = new HorizontalMatchDetector();
        private readonly MatchDetector verticalMatchDetector = new VerticalMatchDetector();
        private readonly MatchDetector tShapedMatchDetector = new TshapedMatchDetector();
        private readonly MatchDetector lShapedMatchDetector = new LshapedMatchDetector();

        private string ResourceFolderLevels = "Levels";
        private int CurrentLevelID = 1;

        private int AddScore = 10; //Add Score Points For Each Explosion

        void Start()
        {
            LoadLevel();
            StartGame();
        }

        void Update()
        {
            if (!gameStarted)
                return;

            HandleInput();
        }

        /// <summary>
        /// Handles the player's input
        /// </summary>
        public void HandleInput()
        {
            if (inputLocked || currentlySwapping)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                drag = true;
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null && hit.collider.gameObject.CompareTag("Tile"))
                {   
                    selectedTile = hit.collider.gameObject;
                    selectedTile.GetComponent<Animator>().SetTrigger("Pressed");
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                drag = false;

                if (selectedTile != null && selectedTile.GetComponent<Animator>() != null && selectedTile.gameObject.activeSelf)
                    selectedTile.GetComponent<Animator>().SetTrigger("Unpressed");
            }

            if (drag && selectedTile != null)
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null && hit.collider.gameObject != selectedTile)
                {
                    if (selectedTile.GetComponent<Animator>() != null && selectedTile.gameObject.activeSelf)
                        selectedTile.GetComponent<Animator>().SetTrigger("Unpressed");

                    int idxSelected = tiles.FindIndex(x => x == selectedTile);
                    int xSelected = idxSelected % level.width;
                    int ySelected = idxSelected / level.width;
                    int idxNew = tiles.FindIndex(x => x == hit.collider.gameObject);
                    int xNew = idxNew % level.width;
                    int yNew = idxNew / level.width;
                    if (Math.Abs(xSelected - xNew) > 1 || Math.Abs(ySelected - yNew) > 1)
                        return;

                    if (possibleSwaps.Find(x => x.tileA == hit.collider.gameObject && x.tileB == selectedTile) != null ||
                        possibleSwaps.Find(x => x.tileB == hit.collider.gameObject && x.tileA == selectedTile) != null)
                    {
                        GameObject selectedTileCopy = selectedTile;
                        selectedTile.GetComponent<SpriteRenderer>().sortingOrder = 1;
                        currentlySwapping = true;
                        LeanTween.move(selectedTile, hit.collider.gameObject.transform.position, 0.25f).setOnComplete(
                            () =>
                            {
                                currentlySwapping = false;
                                selectedTileCopy.GetComponent<SpriteRenderer>().sortingOrder = 0;
                                HandleMatches(true);
                            });
                        LeanTween.move(hit.collider.gameObject, selectedTile.transform.position, 0.25f);

                        GameObject tileA = hit.collider.gameObject;
                        GameObject tileB = selectedTile;
                        int idxA = tiles.FindIndex(x => x == tileA);
                        int idxB = tiles.FindIndex(x => x == tileB);
                        tiles[idxA] = tileB;
                        tiles[idxB] = tileA;

                        tileA.GetComponent<Tile>().x = idxB % level.width;
                        tileA.GetComponent<Tile>().y = idxB / level.width;
                        tileB.GetComponent<Tile>().x = idxA % level.width;
                        tileB.GetComponent<Tile>().y = idxA / level.width;

                        selectedTile = null;

                        possibleSwaps = DetectPossibleSwaps();
                    }
                    else
                    {
                        GameObject selectedTileCopy = selectedTile;
                        GameObject hitTileCopy = hit.collider.gameObject;
                        selectedTile.GetComponent<SpriteRenderer>().sortingOrder = 1;

                        Vector3 selectedTilePos = selectedTile.transform.position;
                        Vector3 hitTilePos = hit.collider.gameObject.transform.position;

                        GameObject tileA = hit.collider.gameObject;
                        GameObject tileB = selectedTile;
                        if (!(tileA.GetComponent<Tile>().x != tileB.GetComponent<Tile>().x &&
                              tileA.GetComponent<Tile>().y != tileB.GetComponent<Tile>().y))
                        {
                            currentlySwapping = true;
                            LeanTween.move(selectedTile, hitTilePos, 0.2f);
                            LeanTween.move(hit.collider.gameObject, selectedTilePos, 0.2f).setOnComplete(() =>
                            {
                                LeanTween.move(selectedTileCopy, selectedTilePos, 0.2f).setOnComplete(() =>
                                {
                                    currentlySwapping = false;
                                    selectedTileCopy.GetComponent<SpriteRenderer>().sortingOrder = 0;
                                });
                                LeanTween.move(hitTileCopy, hitTilePos, 0.2f);
                            });
                        }

                        selectedTile = null;
                    }
                }
            }
        }

        /// <summary>
        /// Loads the current level
        /// </summary>
        public void LoadLevel()
        {
            ResetLevelData();
        }

        /// <summary>
        /// Start game.
        /// </summary>
        public void StartGame()
        {
            gameStarted = true;
        }

        /// <summary>
        /// Resets the current level data
        /// </summary>
        public void ResetLevelData()
        {
            fsSerializer serializer = new fsSerializer();
            level = FileUtils.LoadJsonFile<Level>(serializer, ResourceFolderLevels + "/" + CurrentLevelID);

            tiles = new List<GameObject>(level.width * level.height);

            gameState.Reset();

            foreach (ObjectPool pool in tilePool.GetComponentsInChildren<ObjectPool>())
            {
                pool.Reset();
            }

            tilePositions.Clear();
            possibleSwaps.Clear();

            CreateMapTiles();

            possibleSwaps = DetectPossibleSwaps();
        }

        #region CREATE MAP
        /// <summary>
        /// Create Current Level
        /// </summary>
        private void CreateMapTiles()
        {
            DebugLog("1-CreateMapTiles", Color.yellow);

            const float horizontalSpacing = 0.0f;
            const float verticalSpacing = 0.0f;

            for (int j = 0; j < level.height; j++)
            {
                for (int i = 0; i < level.width; i++)
                {
                    LevelTile levelTile = level.tiles[i + (j * level.width)];
                    GameObject tile = CreateTileFromLevel(levelTile, i, j);
                    if (tile != null)
                    {
                        SpriteRenderer spriteRenderer = tile.GetComponent<SpriteRenderer>();
                        tileW = spriteRenderer.bounds.size.x;
                        tileH = spriteRenderer.bounds.size.y;
                        tile.transform.position = new Vector2(i * (tileW + horizontalSpacing), -j * (tileH + verticalSpacing));
                    }

                    tiles.Add(tile);
                }
            }

            float totalWidth = (level.width - 1) * (tileW + horizontalSpacing);
            float totalHeight = (level.height - 1) * (tileH + verticalSpacing);
            for (int j = 0; j < level.height; j++)
            {
                for (int i = 0; i < level.width; i++)
                {
                    Vector2 tilePos = new Vector2(i * (tileW + horizontalSpacing), -j * (tileH + verticalSpacing));
                    Vector2 newPos = tilePos;
                    newPos.x -= totalWidth / 2;
                    newPos.y += totalHeight / 2;
                    newPos.y += gameCenter.y;
                    GameObject tile = tiles[i + (j * level.width)];
                    if (tile != null)
                    {
                        tile.transform.position = newPos;
                    }

                    tilePositions.Add(newPos);

                    LevelTile levelTile = level.tiles[i + (j * level.width)];
                    if (!(levelTile is HoleTile))
                    {
                        GameObject bgTile;
                        if (j % 2 == 0)
                        {
                            bgTile = i % 2 == 0
                                ? tilePool.TileBGDarkPool.GetObject()
                                : tilePool.TileBGLightPool.GetObject();
                        }
                        else
                        {
                            bgTile = i % 2 == 0
                                ? tilePool.TileBGLightPool.GetObject()
                                : tilePool.TileBGDarkPool.GetObject();
                        }

                        bgTile.transform.position = newPos;
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new tile from the current level data
        /// </summary>
        /// <param name="levelTile">The level tile</param>
        /// <param name="x">The x-coordinate of the tile</param>
        /// <param name="y">The y-coordinate of the tile</param>
        private GameObject CreateTileFromLevel(LevelTile levelTile, int x, int y)
        {
            DebugLog("2-CreateTileFromLevel", Color.yellow);

            if (levelTile is DropTile)
            {
                DropTile dropTile = (DropTile)levelTile;
                if (dropTile.type == DropType.RandomDrop)
                {
                    return CreateTile(x, y, false);
                }
                else
                {
                    GameObject tile = tilePool.GetDropPool((DropColor)((int)dropTile.type)).GetObject();
                    tile.GetComponent<Tile>().GM = this;
                    tile.GetComponent<Tile>().x = x;
                    tile.GetComponent<Tile>().y = y;
                    return tile;
                }
            }

            return null;
        }

        /// <summary>
        /// Creates a new
        /// </summary>
        /// <param name="x">The x-coordinate of the tile.</param>
        /// <param name="y">The y-coordinate of the tile.</param>
        /// <param name="runtime">True if this tile is created during a game; false otherwise.</param>
        private GameObject CreateTile(int x, int y, bool runtime)
        {
            DebugLog("3-CreateTile", Color.yellow);

            List<DropColor> suitableTiles = new List<DropColor>();
            suitableTiles.AddRange(level.availableColors);

            GameObject leftTile1 = GetTile(x - 1, y);
            GameObject leftTile2 = GetTile(x - 2, y);
            if (leftTile1 != null && leftTile2 != null &&
                leftTile1.GetComponent<Drop>() != null && leftTile2.GetComponent<Drop>() != null &&
                leftTile1.GetComponent<Drop>().color == leftTile2.GetComponent<Drop>().color)
            {
                DropColor tileToRemove = suitableTiles.Find(t => t == leftTile1.GetComponent<Drop>().color);
                suitableTiles.Remove(tileToRemove);
            }

            GameObject topTile1 = GetTile(x, y - 1);
            GameObject topTile2 = GetTile(x, y - 2);
            if (topTile1 != null && topTile2 != null &&
                topTile1.GetComponent<Drop>() != null && topTile2.GetComponent<Drop>() != null &&
                topTile1.GetComponent<Drop>().color == topTile2.GetComponent<Drop>().color)
            {
                DropColor tileToRemove = suitableTiles.Find(t => t == topTile1.GetComponent<Drop>().color);
                suitableTiles.Remove(tileToRemove);
            }

            GameObject tile = tilePool.GetDropPool(suitableTiles[UnityEngine.Random.Range(0, suitableTiles.Count)]).GetObject();

            tile.GetComponent<Tile>().GM = this;
            tile.GetComponent<Tile>().x = x;
            tile.GetComponent<Tile>().y = y;
            return tile;
        }
        #endregion

        /// <summary>
        /// Returns the tile at coordinates (x, y)
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <returns>The tile at coordinates (x, y).</returns>
        public GameObject GetTile(int x, int y)
        {
            if (x >= 0 && x < level.width && y >= 0 && y < level.height)
                return tiles[x + (y * level.width)];

            return null;
        }

        /// <summary>
        /// Detects all the possible tile swaps in the current level
        /// </summary>
        private List<Swap> DetectPossibleSwaps()
        {
            List<Swap> swaps = new List<Swap>();

            for (int j = 0; j < level.height; j++)
            {
                for (int i = 0; i < level.width; i++)
                {
                    GameObject tile = GetTile(i, j);
                    if (tile != null)
                    {
                        if (i < level.width - 1)
                        {
                            GameObject other = GetTile(i + 1, j);
                            if (other != null)
                            {
                                SetTile(other, i, j);
                                SetTile(tile, i + 1, j);

                                if (HasMatch(i, j) || HasMatch(i + 1, j))
                                {
                                    Swap swap = new Swap { tileA = tile, tileB = other };
                                    swaps.Add(swap);
                                }
                            }

                            SetTile(tile, i, j);
                            SetTile(other, i + 1, j);
                        }

                        if (j < level.height - 1)
                        {
                            GameObject other = GetTile(i, j + 1);
                            if (other != null)
                            {
                                SetTile(other, i, j);
                                SetTile(tile, i, j + 1);

                                if (HasMatch(i, j) || HasMatch(i, j + 1))
                                {
                                    Swap swap = new Swap { tileA = tile, tileB = other };
                                    swaps.Add(swap);
                                }
                            }

                            SetTile(tile, i, j);
                            SetTile(other, i, j + 1);
                        }
                    }
                }
            }

            return swaps;
        }

        /// <summary>
        /// Replaces the tile at coordinates (x, y)
        /// </summary>
        /// <param name="tile">The new tile.</param>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        private void SetTile(GameObject tile, int x, int y)
        {
            if (x >= 0 && x < level.width && y >= 0 && y < level.height)
                tiles[x + (y * level.width)] = tile;
        }

        /// <summary>
        /// Returns true if the tile at (x, y) has a match and false otherwise
        /// </summary>
        /// <param name="x">The x-coordinate</param>
        /// <param name="y">The y-coordinate</param>
        private bool HasMatch(int x, int y)
        {
            return HasHorizontalMatch(x, y) || HasVerticalMatch(x, y);
        }

        /// <summary>
        /// Returns true if the tile at (x, y) has a horizontal match and false otherwise
        /// </summary>
        /// <param name="x">The x-coordinate</param>
        /// <param name="y">The y-coordinate</param>
        /// <returns>True if the tile at (x, y) has a horizontal match; false otherwise.</returns>
        private bool HasHorizontalMatch(int x, int y)
        {
            GameObject tile = GetTile(x, y);
            if (tile.GetComponent<Drop>() != null)
            {
                int horzLen = 1;
                for (int i = x - 1;
                    i >= 0 && GetTile(i, y) != null && GetTile(i, y).GetComponent<Drop>() != null &&
                    GetTile(i, y).GetComponent<Drop>().color == tile.GetComponent<Drop>().color;
                    i--, horzLen++) ;
                for (int i = x + 1;
                    i < level.width && GetTile(i, y) != null && GetTile(i, y).GetComponent<Drop>() != null &&
                    GetTile(i, y).GetComponent<Drop>().color == tile.GetComponent<Drop>().color;
                    i++, horzLen++) ;
                if (horzLen >= 3) return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if the tile at (x, y) has a vertical match and false otherwise.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <returns>True if the tile at (x, y) has a vertical match; false otherwise.</returns>
        private bool HasVerticalMatch(int x, int y)
        {
            GameObject tile = GetTile(x, y);
            if (tile.GetComponent<Drop>() != null)
            {
                int vertLen = 1;
                for (int j = y - 1;
                    j >= 0 && GetTile(x, j) != null && GetTile(x, j).GetComponent<Drop>() != null &&
                    GetTile(x, j).GetComponent<Drop>().color == tile.GetComponent<Drop>().color;
                    j--, vertLen++) ;
                for (int j = y + 1;
                    j < level.height && GetTile(x, j) != null && GetTile(x, j).GetComponent<Drop>() != null &&
                    GetTile(x, j).GetComponent<Drop>().color == tile.GetComponent<Drop>().color;
                    j++, vertLen++) ;
                if (vertLen >= 3) return true;
            }

            return false;
        }

        #region  MATCHES & EXPLODES & GRAVITY

        /// <summary>
        /// Resolves all the matches in the current level
        /// </summary>
        /// <param name="isPlayerMatch">True if the match was caused by a player and false otherwise</param>
        private bool HandleMatches(bool isPlayerMatch)
        {
            List<Match> matches = new List<Match>();
            List<Match> horizontalMatches = horizontalMatchDetector.DetectMatches(this);
            List<Match> verticalMatches = verticalMatchDetector.DetectMatches(this);
            List<Match> tShapedMatches = tShapedMatchDetector.DetectMatches(this);
            List<Match> lShapedMatches = lShapedMatchDetector.DetectMatches(this);

            if (tShapedMatches.Count > 0)
            {
                matches.AddRange(tShapedMatches);
                foreach (Match match in horizontalMatches)
                {
                    bool found = false;
                    foreach (Match match2 in tShapedMatches)
                    {
                        if (match.tiles.Find(x => match2.tiles.Contains(x)) != null)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        matches.Add(match);
                    }
                }

                foreach (Match match in verticalMatches)
                {
                    bool found = false;
                    foreach (Match match2 in tShapedMatches)
                    {
                        if (match.tiles.Find(x => match2.tiles.Contains(x)) != null)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        matches.Add(match);
                    }
                }
            }
            else if (lShapedMatches.Count > 0)
            {
                matches.AddRange(lShapedMatches);
                foreach (Match match in horizontalMatches)
                {
                    bool found = false;
                    foreach (Match match2 in lShapedMatches)
                    {
                        if (match.tiles.Find(x => match2.tiles.Contains(x)) != null)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        matches.Add(match);
                    }
                }

                foreach (Match match in verticalMatches)
                {
                    bool found = false;
                    foreach (Match match2 in lShapedMatches)
                    {
                        if (match.tiles.Find(x => match2.tiles.Contains(x)) != null)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        matches.Add(match);
                    }
                }
            }
            else if (horizontalMatches.Count > 0)
            {
                matches.AddRange(horizontalMatches);
                foreach (Match match in verticalMatches)
                {
                    bool found = false;
                    foreach (Match match2 in horizontalMatches)
                    {
                        if (match.tiles.Find(x => match2.tiles.Contains(x)) != null)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        matches.Add(match);
                    }
                }
            }
            else
            {
                matches.AddRange(horizontalMatches);
                matches.AddRange(verticalMatches);
            }

            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    foreach (GameObject tile in match.tiles)
                    {
                        ExplodeTile(tile);
                    }
                }

                StartCoroutine(ApplyGravityAsync(0.0f));
                return true;
            }
            else
            {
                return false;
            }
        }

        #region EXPLODES
        /// <summary>
        /// Explodes the specified tile
        /// </summary>
        /// <param name="tile">The tile to explode</param>
        public void ExplodeTile(GameObject tile)
        {
            List<GameObject> explodedTiles = new List<GameObject>();
            ExplodeTileRecursive(tile, explodedTiles);
            int score = 0;
            foreach (GameObject explodedTile in explodedTiles)
            {
                int idx = tiles.FindIndex(x => x == explodedTile);
                if (idx != -1)
                {
                    explodedTile.GetComponent<Tile>().UpdateGameState(gameState);
                    score += AddScore;
                    explodedTile.GetComponent<PooledObject>().pool.ReturnObject(explodedTile);
                    tiles[idx] = null;
                }
            }

            UpdateScore(score);
        }

        /// <summary>
        /// Explodes the specified tile recursively
        /// </summary>
        /// <param name="tile">The tile to explode</param>
        /// <param name="explodedTiles">The list of the exploded tiles so far</param>
        private void ExplodeTileRecursive(GameObject tile, List<GameObject> explodedTiles)
        {
            if (tile != null && tile.GetComponent<Tile>() != null)
            {
                List<GameObject> newTilesToExplode = tile.GetComponent<Tile>().Explode();

                explodedTiles.Add(tile);

                foreach (GameObject t in newTilesToExplode)
                {
                    if (t != null && t.GetComponent<Tile>() != null && t.GetComponent<Tile>().destructable &&
                        !explodedTiles.Contains(t))
                    {
                        explodedTiles.Add(t);
                        ExplodeTileRecursive(t, explodedTiles);
                    }
                }

                foreach (GameObject t in newTilesToExplode)
                {
                    if (!newTilesToExplode.Contains(t))
                    {
                        newTilesToExplode.Add(t);
                    }
                }
            }
        }
        #endregion

        #region GRAVITY
        /// <summary>
        /// The coroutine that applies the gravity to the current level.
        /// </summary>
        private IEnumerator ApplyGravityAsync(float delay)
        {
            inputLocked = true;
            yield return new WaitForSeconds(delay);
            ApplyGravityInternal();
            possibleSwaps = DetectPossibleSwaps();
            yield return new WaitForSeconds(1.0f);

            if (!HandleMatches(false))
                inputLocked = false;
        }

        /// <summary>
        /// Internal method that actually applies the gravity to the current level
        /// </summary>
        private void ApplyGravityInternal()
        {
            for (int i = 0; i < level.width; i++)
            {
                for (int j = level.height - 1; j >= 0; j--)
                {
                    int tileIndex = i + (j * level.width);

                    // Find bottom.
                    int bottom = -1;
                    for (int k = j; k < level.height; k++)
                    {
                        int idx = i + (k * level.width);
                        if (tiles[idx] == null && !(level.tiles[idx] is HoleTile))
                        {
                            bottom = k;
                        }
                    }

                    if (bottom != -1)
                    {
                        GameObject tile = GetTile(i, j);
                        if (tile != null)
                        {
                            int numTilesToFall = bottom - j;
                            tiles[tileIndex + (numTilesToFall * level.width)] = tiles[tileIndex];
                            LTDescr tween = LeanTween.move(tile,
                                tilePositions[tileIndex + level.width * numTilesToFall],
                                0.5f);
                            tween.setEase(LeanTweenType.easeInQuad);
                            tween.setOnComplete(() =>
                            {
                                if (tile.GetComponent<Tile>() != null)
                                {
                                    tile.GetComponent<Tile>().y += numTilesToFall;
                                    if (tile.activeSelf && tile.GetComponent<Animator>() != null)
                                    {
                                        tile.GetComponent<Animator>().SetTrigger("Falling");
                                    }
                                }
                            });
                            tiles[tileIndex] = null;
                        }
                    }
                }
            }

            for (int i = 0; i < level.width; i++)
            {
                int numEmpties = 0;
                for (int j = 0; j < level.height; j++)
                {
                    int idx = i + (j * level.width);
                    if (tiles[idx] == null && !(level.tiles[idx] is HoleTile))
                    {
                        numEmpties += 1;
                    }
                }

                if (numEmpties > 0)
                {
                    for (int j = 0; j < level.height; j++)
                    {
                        int tileIndex = i + (j * level.width);
                        bool isHole = level.tiles[tileIndex] is HoleTile;

                        bool isColumnLimitActive = false;
                        foreach (int item in level.columnRespawnLimits)
                        {
                            if (item == i)
                            {
                                isColumnLimitActive = true;
                                break;
                            }
                        }

                        if (tiles[tileIndex] == null && !isHole && !isColumnLimitActive)
                        {
                            GameObject tile = CreateTile(i, j, true);
                            Vector3 sourcePos = tilePositions[i];
                            Vector3 targetPos = tilePositions[tileIndex];
                            Vector3 pos = sourcePos;
                            pos.y = tilePositions[i].y + (numEmpties * (tileH));
                            --numEmpties;
                            tile.transform.position = pos;
                            LTDescr tween = LeanTween.move(tile, targetPos, 0.5f);
                            tween.setEase(LeanTweenType.easeInQuad);
                            tween.setOnComplete(() =>
                            {
                                if (tile.activeSelf && tile.GetComponent<Animator>() != null)
                                {
                                    tile.GetComponent<Animator>().SetTrigger("Falling");
                                }
                            });
                            tiles[tileIndex] = tile;
                        }
                    }
                }
            }
        }
        #endregion

        #endregion

        /// <summary>
        /// Updates the score of the current game
        /// </summary>
        /// <param name="score"></param>
        void UpdateScore(int score)
        {
            gameState.score += score;
            DebugLog("SCORE->" + gameState.score, Color.green);
        }

        #region TOOLS
        public void DebugLog(string text, Color color)
        {
            Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{text}</color>");
        }
        #endregion

    }
}
