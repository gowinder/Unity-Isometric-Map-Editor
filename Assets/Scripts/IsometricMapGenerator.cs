using UnityEngine;
using System.Collections;
using System.Collections.Generic; // For the tuple
using System.Linq;

// For Saving and Loading.
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class IsometricMapGenerator : MonoBehaviour {

    // The size of the square part of each tile
    [SerializeField] float TileWidth  = 128f;
    [SerializeField] float TileHeight = 64f;

    public Sprite[] tiles;    // An array of sprites to load the tiles folder into

    // Level editor specific stuff
    public int selectedSprite; // Selected tile to change tile with

    // Some minor needless preoptimisation
    float TileWidthHalf;
    float TileHeightHalf;

    int[][] basicMap; // The map to load
    
    private GameObject[][] GeneratedMap;

	void Start () {
        TileWidthHalf  = TileWidth * 0.5f;
        TileHeightHalf = TileHeight * 0.5f;
        selectedSprite = 0;

        // Set a basic map
        ResetMap();
        GeneratedMap = new GameObject[basicMap.Length][];

        // Load the tile sprites
        tiles = (Sprite[])Resources.LoadAll<Sprite>("Sprites/Landscape")
                .Concat((Sprite[])Resources.LoadAll<Sprite>("Sprites/Details")).ToArray();

        GenerateMap(basicMap);
	}
	
	void Update () {
        if (Input.GetKeyDown("space"))   NewMap();       
        
        selectTile(); // Get the mouse to grid point and "select" that tile
        if (Input.GetMouseButton(0)) ChangeTile();
	}

    /// <summary> Sets the map back to the flat grass </summary>
    void ResetMap() {
        basicMap = new int[][] {
              new int[] {28,28,28,28,28,28,28,28,28,28},
              new int[] {28,28,28,28,28,28,28,28,28,28},
              new int[] {28,28,28,28,28,28,28,28,28,28},
              new int[] {28,28,28,28,28,28,28,28,28,28},
              new int[] {28,28,28,28,28,28,28,28,28,28},
              new int[] {28,28,28,28,28,28,28,28,28,28},
              new int[] {28,28,28,28,28,28,28,28,28,28},
              new int[] {28,28,28,28,28,28,28,28,28,28},
              new int[] {28,28,28,28,28,28,28,28,28,28},
              new int[] {28,28,28,28,28,28,28,28,28,28}
        };
    }

    void NewMap() {
        // Destory previous map
        foreach (Transform child in transform)
            GameObject.Destroy(child.gameObject);
        // Gen a new one
        ResetMap();
        GenerateMap(basicMap);
    }

    void NewMap(int[][] map) {
        foreach (Transform child in transform)
            GameObject.Destroy(child.gameObject);
        GenerateMap(map);
    }

    void GenerateMap(int[][] map) {
        // Loop through the rows and columns of the map
        // And place the sprites in the correct place
        for (int i = 0; i < map.Length; i++) {

            // Fill out the array to store the current map in
            GeneratedMap[i] = new GameObject[map[i].Length];

            // Reverse the drawing of the X axis,
            // So that tiles are rendered in the correct order
            for (int j = map[i].Length - 1; j >= 0; j--) {
                // Create a new tile and set it's image
                GameObject tile = new GameObject();
                tile.name = "Tile " + i + "-" + j;
                tile.transform.SetParent(transform);
                tile.AddComponent<SpriteRenderer>();
                tile.GetComponent<SpriteRenderer>().sprite = tiles[map[i][j]];

                // Attach the tile movement script to move it later
                tile.AddComponent<TileMovement>();
                var tMove = tile.GetComponent<TileMovement>();

                // Place the tile in the correct position
                float x = (j * TileWidth  / 2f)  + (i * TileWidth  / 2f);
                float y = (i * TileHeight / 2f)  - (j * TileHeight / 2f);

                tMove.gridPosition = new Vector3(x, y, 0);
                tile.transform.position = new Vector3(x, y + 5f, 0);

                // Finally add it to the GeneratedMap array to play with it later
                GeneratedMap[i][j] = tile;
            }
        }

    }
    
    void selectTile() {
        // Gets the mouse point and tells the tile under it that it's selected
        Vector2 m = mouseGridCoOrds();
        // Make sure the mouse is actually on grid
        if(m.x < GeneratedMap[0].Length && m.x >= 0 &&
           m.y < GeneratedMap.Length && m.y >= 0)
            GeneratedMap[(int)m.x][(int)m.y].GetComponent<TileMovement>().selected = true;
    }

    void ChangeTile() {
        // Changes tile under mouse cursor
        Vector2 m = mouseGridCoOrds();
        if (m.x < GeneratedMap[0].Length && m.x >= 0 &&
           m.y < GeneratedMap.Length && m.y >= 0) {
            GeneratedMap[(int)m.x][(int)m.y].GetComponent<SpriteRenderer>().sprite = tiles[selectedSprite];
            basicMap[(int)m.x][(int)m.y] = selectedSprite;
        }
    }

    Vector2 mouseGridCoOrds() {
        // Takes the mouse position reletive to screen and outputs the tile hit
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float x = ((mouse.x) / TileWidthHalf  +  (mouse.y / TileHeightHalf)) / 2f;
        float y = ((mouse.y) / TileHeightHalf - (mouse.x / TileWidthHalf))  / 2f;

        // Get rid of pesky negatives and decimals
        // Also adjust the mouse position x-- and y++,
        // With tiles having a pivot of "bottom" instead of "center" the math feels off
        // This adjustment fixes it

        // TODO: Fix this returning a grid point if the mouse isn't actually on the grid (Abs() is making negetive numbers seem like it is)
        x = Mathf.Round(--x);
        x = Mathf.Abs(x);        
        y = Mathf.Round(--y);
        y = Mathf.Abs(y);

        return new Vector2(x, y);
    }

    Vector2 pointToIso(float x, float y) {
        Vector2 isoPoint = new Vector2();
        isoPoint.x = x - y;
        isoPoint.y = (x + y) / 2;
        return isoPoint;
    }

    /// <summary> Saves the current map to disk </summary>
    /// <param name="name"></param>
    public void SaveMap(string name) {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + name + ".dat", FileMode.Create);
        MapData data = new MapData();
        data.map = basicMap;
        bf.Serialize(file, data);
        file.Close();
    }

    /// <summary> Loads a map from persistantData path with given name </summary>
    /// <param name="name"></param>
    public void LoadMap(string name) {
        if (File.Exists(Application.persistentDataPath + name + ".dat")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + name + ".dat", FileMode.Open);
            MapData data = (MapData)bf.Deserialize(file);
            file.Close();

            NewMap(data.map);
        }
    }
}

/// <summary> MapData that can be stored to the disk </summary>
[Serializable]
class MapData {
    public int[][] map;
}