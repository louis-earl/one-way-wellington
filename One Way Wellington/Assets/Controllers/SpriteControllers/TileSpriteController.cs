using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class TileSpriteController : MonoBehaviour
{
    public static TileSpriteController Instance;

    public Tilemap tilemap;

    Dictionary<TileOWW, UnityEngine.Tilemaps.Tile> tileObjectMap = new Dictionary<TileOWW, UnityEngine.Tilemaps.Tile>();


    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InstantiateTiles(World world)
    {
        foreach (TileOWW tile in world.GetTiles())
        {
            Tile t = ScriptableObject.CreateInstance<Tile>();
            t.sprite = TileType.Empty;
            t.name = tile.GetX() + "_" + tile.GetY();
            tilemap.SetTile(new Vector3Int(tile.GetX(), tile.GetY(), 0), t);
            tileObjectMap.Add(tile, t);
        }
    }

    public void UpdateTile(TileOWW tile)
    {
        Tile t;
        if (!tileObjectMap.ContainsKey(tile))
        {
            // The tile might not exist when loading a game from file
            t = ScriptableObject.CreateInstance<Tile>();
            t.name = tile.GetX() + "_" + tile.GetY();
            t.sprite = Resources.Load<Sprite>("Images/Tiles/" + tile.GetTileType());
            tilemap.SetTile(new Vector3Int(tile.GetX(), tile.GetY(), 0), t);
            tileObjectMap.Add(tile, t);
        }
        else
        {
            t = tileObjectMap[tile];
            t.sprite = Resources.Load<Sprite>("Images/Tiles/" + tile.GetTileType());
            tilemap.RefreshTile(new Vector3Int(tile.GetX(), tile.GetY(), 0));
        }



    }
}
