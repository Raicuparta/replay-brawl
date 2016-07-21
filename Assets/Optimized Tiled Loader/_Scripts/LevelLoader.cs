using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEditor;
using System.IO;

public class LevelLoader : MonoBehaviour
{
    public TextAsset level_JSON;
    public float scale = 1;

    GameObject _tiles;
    GameObject _boundaries;

    GameObject[,] levelTiles;
    int[,] levelIndexes;
    List<GameObject> realTiles;

    public void Start()
    {
        Clear();

        // Make a new empty gameobject where all the tiles in the level will be placed under.
        if (_tiles == null)
        {
            _tiles = new GameObject();
            _tiles.transform.SetParent(transform);
            _tiles.transform.localPosition = Vector2.zero;
            _tiles.name = "_Tiles";
        }

        // Loads the JSON file.
        LoadFile(level_JSON);
        // Creates the blocks from the loaded file.
        CreateLevel();
        // Merges chunks of the same blocks together to reduce the amount of instances.
        MergeBlocks();
        // Makes a single image from the loaded blocks.
        CreateOverlay();
    }

    /// <summary>
    /// Load a tiled level file.
    /// </summary>
    /// <param name="levelFile">The .json file we would like to load from.</param>
    public void LoadFile(TextAsset levelFile)
    {
        // Check if the levelFile isn't null.
        if (levelFile == null)
        {
            throw new Exception("Level File is null!");
        }

        string levelFilePath = AssetDatabase.GetAssetPath(levelFile);

        // Check if the levelFile is in fact a JSON file.
        if (levelFilePath.Substring(levelFilePath.Length - 5, 5) != ".json")
        {
            throw new Exception(levelFile.name + " is not a JSON file!");
        }

        // Load the raw JSON from the tiled file and store it in a level object.
        var level = JsonUtility.FromJson<Level>(levelFile.text);
        TiledLevel = level;

        levelFilePath = AssetDatabase.GetAssetPath(levelFile) + "/../";
        /*
        Unity won't support going to parent folder using ../ in the file path. 
        Have to do use System.IO to get the correct absolute file path
        */
        string absolutePath = Path.GetFullPath(levelFilePath + TiledLevel.tilesets[0].image);

        /* 
        But we don't want the absolute path, we want the path from the unity root folder.
        To get it, I remove the datapath of the project from the absolute path (which unfortunately includes the "assets" folder).
        After adding assets again, this gives us the correct relative path from the root folder.
        God damn it Unity, why can't I use ../ to go up a folder. Look at what you made me do :/     
        */
        string tilesetPath = "assets" + absolutePath.Remove(0, Application.dataPath.Length);

        // Load the tileset used for the tiles.
        Tileset = AssetDatabase.LoadAssetAtPath<Texture2D>(tilesetPath);

        if(Tileset == null)
        {
            throw new Exception("The tileset could not be found at: " + tilesetPath);
        }

        // Set the width and height of the level.
        LevelGridSize = new Vector2(TiledLevel.width, TiledLevel.height);
        TilePixelSize = new Vector2(TiledLevel.tilewidth, TiledLevel.tileheight);
        // Determine the size the tiles should have in Unity.
        // Experimentally found out that this is correct. It probably makes sense mathematically, since it always holds true. Or magic, probably magic.
        TileUnitySize = new Vector2(scale * 10f / TiledLevel.width, scale * 10f / TiledLevel.width * (TilePixelSize.y / TilePixelSize.x));
        LevelUnitySize = new Vector2(TileUnitySize.x * LevelGridSize.x, TileUnitySize.y * LevelGridSize.y);

        // Create an array to hold int values for the different block types.
        levelIndexes = new int[(int)LevelGridSize.y, (int)LevelGridSize.x];

        // Fill the level indexes with the data from the tiled level file.
        for (int i = 0; i < TiledLevel.layers[0].data.Length; i++)
        {
            levelIndexes[i / (int)LevelGridSize.x, i % (int)LevelGridSize.x] = TiledLevel.layers[0].data[i];
        }
    }

    /// <summary>
    /// Place blocks inside the 2D world based on the level loaded.
    /// </summary>
    public void CreateLevel()
    {
        // Check if level is loaded.
        if (levelIndexes == null)
            throw new Exception("Level not loaded!");

        // The grid of tiles in the level.
        levelTiles = new GameObject[(int)LevelGridSize.y, (int)LevelGridSize.x];

        // Place the tiles inside the level.
        for (int y = 0; y < (int)LevelGridSize.y; y++)
            for (int x = 0; x < (int)LevelGridSize.x; x++)
            {
                int tileType = levelIndexes[y, x];
                // This is where you attach logic to different tileTypes.
                switch (tileType)
                {
                    case 0:
                        break;
                    default:
                        LoadDefaultTile(x, y, tileType);
                        break;
                }
            }

        // Add the boundaries to the 2D level.
        AddBoundaries(LevelUnitySize.x, LevelUnitySize.y);
    }

    #region Tile Loaders

    /// <summary>
    /// Load a solid block the player cannot pass through.
    /// </summary>
    /// <param name="xIndex">The x-index of the tile.</param>
    /// <param name="yIndex">The y-index of the tile.</param>
    void LoadDefaultTile(int xIndex, int yIndex, int tileType)
    {
        // Instantiate a new tile
        GameObject newTile = new GameObject();
        newTile.name = "Tile";
        newTile.AddComponent<BoxCollider2D>();
        // Set the size of the boxcollider to the correct unity-unit tilesize.
        newTile.GetComponent<BoxCollider2D>().size = TileUnitySize;
        newTile.transform.position = new Vector2(xIndex * TileUnitySize.x, yIndex * TileUnitySize.y);

        // Put the new tile under our _tiles folder.
        newTile.transform.SetParent(_tiles.transform);
        // Place it at the correct position.
        newTile.transform.localPosition = new Vector2(TileUnitySize.x * xIndex + TileUnitySize.x / 2, TileUnitySize.y * -yIndex - TileUnitySize.y / 2);

        newTile.AddComponent<Block>();
        newTile.GetComponent<Block>().Initialize(xIndex, yIndex, false, tileType);
        levelTiles[yIndex, xIndex] = newTile;
    }

    #endregion

    /// <summary>
    /// Add the boundaries of the level.
    /// </summary>
    /// <param name="levelWidth">The width of the level.</param>
    /// <param name="levelHeight">The height of the level.</param>
    void AddBoundaries(float levelWidth, float levelHeight)
    {
        // Make a new empty gameObject.
        _boundaries = new GameObject();
        // Make the boundaries our child and set its name correctly.
        _boundaries.transform.SetParent(transform);
        _boundaries.name = "_Boundaries";

        // Add the required components.
        _boundaries.AddComponent<BoxCollider2D>();

        // Determine the correct size and offset for the level boundaries.
        _boundaries.GetComponent<BoxCollider2D>().size = new Vector2(levelWidth, levelHeight);
        _boundaries.GetComponent<BoxCollider2D>().isTrigger = true;
        _boundaries.transform.localPosition = new Vector2(levelWidth / 2, -levelHeight / 2);
    }

    /// <summary>
    /// Clear the 2D level.
    /// </summary>
    public void Clear()
    {
        List<GameObject> allChildren = new List<GameObject>();
        foreach (Transform t in transform) allChildren.Add(t.gameObject);
        allChildren.ForEach(child => DestroyImmediate(child));
    }

    #region Optimization

    /// <summary>
    /// Remove redundant blocks, gather chunks of blocks together.
    /// </summary>
    public void MergeBlocks()
    {
        // Merge rows of same-sized blocks together into one block.
        for (int y = 0; y < LevelGridSize.y; y++)
            for (int x = 0; x < LevelGridSize.x; x++)
            {
                bool done = false;
                int xIndex = 1;

                while (!done)
                {
                    done = true;

                    if (levelTiles[y, x] == null)
                        continue;

                    if (CanMergeRight(levelTiles[y, x].GetComponent<Block>()))
                    {
                        Merge(levelTiles[y, x], levelTiles[y, x + xIndex]);
                        xIndex++;
                        done = false;
                    }
                }
            }

        // Merge columns of same-sized blocks together into one block.
        for (int y = 0; y < LevelGridSize.y; y++)
            for (int x = 0; x < LevelGridSize.x; x++)
            {
                bool done = false;
                int yOffset = 1;

                while (!done)
                {
                    done = true;
                    if (levelTiles[y, x] == null)
                        continue;
                    if (CanMergeUnder(levelTiles[y, x].GetComponent<Block>()))
                    {
                        Merge(levelTiles[y, x], levelTiles[y + yOffset, x]);
                        yOffset++;
                        done = false;
                    }
                }
            }
    }

    /// <summary>
    /// Merge two blocks together into one big block.
    /// </summary>
    /// <param name="a">First block.</param>
    /// <param name="b">Second block.</param>
    public void Merge(GameObject a, GameObject b)
    {
        Block aBlock;
        Block bBlock;

        // If either block is null, return.
        if (a == null || b == null || a.GetComponent<Block>() == null || b.GetComponent<Block>() == null)
        {
            return;
        }

        aBlock = a.GetComponent<Block>();
        bBlock = b.GetComponent<Block>();

        // If either block is background, don't merge.
        if (aBlock.backGround || bBlock.backGround)
            return;

        // Create a new block that will become the merged block of aBlock and bBlock.
        GameObject mergedBlock = new GameObject();
        mergedBlock.transform.SetParent(_tiles.transform);

        // Add the right components to the block.
        mergedBlock.AddComponent<Block>();
        mergedBlock.GetComponent<Block>().Initialize((int)aBlock.index.x, (int)aBlock.index.y, false, aBlock.tileType);
        mergedBlock.AddComponent<BoxCollider2D>();
        mergedBlock.name = "Tile";

        // Case for doing a horizontal merge.
        if (a.transform.position.y == b.transform.position.y)
        {
            // Add the gridsize of the 2 blocks to our size.
            mergedBlock.GetComponent<Block>().GridSize = new Vector2(
                aBlock.GridSize.x + bBlock.GridSize.x,
                aBlock.GridSize.y);

            // Add the actual size of the 2 blocks to our size.
            mergedBlock.GetComponent<BoxCollider2D>().size = new Vector2(a.GetComponent<BoxCollider2D>().size.x + b.GetComponent<BoxCollider2D>().size.x, a.GetComponent<BoxCollider2D>().size.y);
        }
        // Case for doing a vertical merge.
        else if (a.transform.position.x == b.transform.position.x)
        {
            // Add the gridsize of the 2 blocks to our size.
            mergedBlock.GetComponent<Block>().GridSize = new Vector2(
                aBlock.GridSize.x,
                aBlock.GridSize.y + bBlock.GridSize.y);

            // Add the actual size of the 2 blocks to our size.
            mergedBlock.GetComponent<BoxCollider2D>().size = new Vector2(a.GetComponent<BoxCollider2D>().size.x, a.GetComponent<BoxCollider2D>().size.y + b.GetComponent<BoxCollider2D>().size.y);
        }

        // Add all the positions of a and b to the merged block.
        mergedBlock.GetComponent<Block>().positions.AddRange(aBlock.positions);
        mergedBlock.GetComponent<Block>().positions.AddRange(bBlock.positions);

        // Get the position weighted with the gridSize of block A and block B.
        Vector2 weightedA = new Vector3(a.transform.position.x * aBlock.GridSize.x, a.transform.position.y * aBlock.GridSize.y);
        Vector2 weightedB = new Vector3(b.transform.position.x * bBlock.GridSize.x, b.transform.position.y * bBlock.GridSize.y);
        Vector2 weightedM = weightedA + weightedB;

        // Use the weighted position to put the merged block on the position exactly between A and B.
        mergedBlock.transform.position = new Vector3(weightedM.x / (aBlock.GridSize.x + bBlock.GridSize.x), weightedM.y / (aBlock.GridSize.y + bBlock.GridSize.y));

        // Fill the right positions in the grid with the merged block.
        foreach (Vector2 p in aBlock.positions)
        {
            levelTiles[(int)p.y, (int)p.x] = mergedBlock;
        }

        foreach (Vector2 p in bBlock.positions)
        {
            levelTiles[(int)p.y, (int)p.x] = mergedBlock;
        }

        // Destroy the old 2 tiles
        DestroyImmediate(a.gameObject);
        DestroyImmediate(b.gameObject);
    }

    /// <summary>
    /// Check if there is a block with the same height to our right.
    /// </summary>
    /// <param name="block">The block we would like to check from.</param>
    /// <returns>Whether there is a block to our right with the same height.</returns>
    bool CanMergeRight(Block block)
    {
        // If the current block is a background block, do nothing.
        if (block.backGround)
            return false;
        // If the block is at the edge of the level, do nothing.
        if ((int)block.index.y >= LevelGridSize.y || (int)block.index.x + block.GridSize.x >= LevelGridSize.x)
            return false;

        // Get the neighbouring object to our right.
        GameObject nbr = levelTiles[(int)block.index.y, (int)block.index.x + (int)block.GridSize.x];

        // If there was no neighbour or the neighbour is of a different type, return false.
        if (nbr == null || block.tileType != nbr.GetComponent<Block>().tileType)
            return false;
        else
        {
            Block nbrBlock = nbr.GetComponent<Block>();
            // If our neighbour has the same ySize and the neighbour isn't in the background, return true.
            if (block.GridSize.y == nbrBlock.GridSize.y && !nbrBlock.backGround)
            {
                return true;
            }
            else
                return false;
        }
    }

    /// <summary>
    /// Check if there is a block with the same width underneath us.
    /// </summary>
    /// <param name="block">The block we would like to check from.</param>
    /// <returns>Whether there is a block underneath us with the same height.</returns>
    bool CanMergeUnder(Block block)
    {
        // If the current block is a background block, do nothing.
        if (block.backGround)
            return false;
        // If the block is at the edge of the level, do nothing.
        if ((int)block.index.y + block.GridSize.y >= LevelGridSize.y || (int)block.index.x >= LevelGridSize.x)
            return false;

        // Get the neighbouring object underneath us.
        GameObject nbr = levelTiles[(int)block.index.y + (int)block.GridSize.y, (int)block.index.x];

        // If there was no neighbour or the neighbour is of a different type, return false.
        if (nbr == null || block.tileType != nbr.GetComponent<Block>().tileType)
            return false;
        else
        {
            Block nbrBlock = nbr.GetComponent<Block>();
            // If our neighbour has the same xSize and the neighbour isn't in the background, return true.
            if (block.GridSize.x == nbrBlock.GridSize.x && !nbrBlock.backGround)
            {
                return true;
            }
            else
                return false;
        }
    }

    #endregion

    #region Level Drawing

    /// <summary>
    /// Create a single texture for all the blocks placed in the level to save processing power.
    /// </summary>
    public void CreateOverlay()
    {
        // Create a new texture to be used for th level overlay.
        Texture2D overlay = new Texture2D((int)(TilePixelSize.x * LevelGridSize.x), (int)(TilePixelSize.y * LevelGridSize.y));

        // Fill the texture with the right tiles.
        for (int x = 0; x < LevelGridSize.x; x++)
            for (int y = 0; y < LevelGridSize.y; y++)
            {
                int tileType = levelIndexes[y, x];

                overlay.SetPixels((int)((LevelGridSize.x - 1) * TilePixelSize.x) - (int)(x * TilePixelSize.x), (int)(y * TilePixelSize.y), (int)TilePixelSize.x, (int)TilePixelSize.y, GetTilePixels(tileType));
            }

        // Correct the settings and apply the changes.
        overlay.filterMode = FilterMode.Point;
        overlay.wrapMode = TextureWrapMode.Clamp;
        overlay.Apply();

        // Make a plane to project the texture onto.
        GameObject overlayObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        overlayObject.transform.SetParent(transform);
        overlayObject.transform.localPosition = Vector3.zero;
        // Rotate the plane to face the 2D camera.
        overlayObject.transform.localEulerAngles = new Vector3(-90, 0, 0);
        overlayObject.name = "_Level Image";

        // Apply the texture to the mesh of the plane.
        MeshRenderer meshRenderer = overlayObject.GetComponent<MeshRenderer>();
        Material material = new Material(Shader.Find("Unlit/Transparent"));
        material.mainTexture = overlay;
        meshRenderer.material = material;

        // Correct the scale and position of the mesh.
        meshRenderer.gameObject.transform.localScale = new Vector3(scale, 1, scale * ((LevelGridSize.y * TilePixelSize.y) / (LevelGridSize.x * TilePixelSize.x)));
        meshRenderer.gameObject.transform.localPosition = _boundaries.transform.localPosition;
    }

    /// <summary>
    /// Gets the pixels of the correct tile in the tileset based on the tiletype.
    /// </summary>
    /// <param name="tileType">The tiletype the tile has in tiled.</param>
    /// <returns></returns>
    Color[] GetTilePixels(int tileType)
    {
        if (tileType == 0)
        {
            // tiletype 0 means we have a background color. Return an empty color array.
            return new Color[(int)TilePixelSize.x * (int)TilePixelSize.y];
        }
        else
        {
            int xIndex, yIndex;
            try {
                xIndex = (tileType - 1) % TiledLevel.tilesets[0].columns;
                yIndex = (tileType - 1) / TiledLevel.tilesets[0].columns;
            } catch (DivideByZeroException e) {
                xIndex = yIndex = 0;
            }

            return Tileset.GetPixels(xIndex * (int)TilePixelSize.x, (int)TilePixelSize.y - (yIndex * (int)TilePixelSize.y), (int)TilePixelSize.x, (int)TilePixelSize.y);
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// The size of a single tile converted to Unity's size units.
    /// </summary>
    public Vector2 TileUnitySize
    {
        get;
        set;
    }

    /// <summary>
    /// The size of a single tile in pixels.
    /// </summary>
    public Vector2 TilePixelSize
    {
        get;
        set;
    }

    /// <summary>
    /// The size of the grid the tiles are placed in.
    /// </summary>
    public Vector2 LevelGridSize
    {
        get;
        set;
    }

    /// <summary>
    /// The size of the level in Unity units.
    /// </summary>
    public Vector2 LevelUnitySize
    {
        get;
        set;
    }

    /// <summary>
    /// The data from the level imported from tiled.
    /// </summary>
    public Level TiledLevel
    {
        get;
        set;
    }

    /// <summary>
    /// Tiles used to build the tile texture.
    /// </summary>
    public Texture2D Tileset
    {
        get;
        set;
    }

    #endregion
}
