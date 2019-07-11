using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The world contains all information of the ship tiles 
// TODO: Rename to 'ship' 
[System.Serializable]
public class World
{
    private int width;
    private int height;

    private TileOWW[,] tiles;

    public World(int width, int height)
    {

        // Tile array
        tiles = new TileOWW[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                tiles[i, j] = new TileOWW(i, j, "Empty");
            }
        }
    }

    public TileOWW GetTileAt(int x, int y)
    {
        if (0 <= x && x < 100 && 0 <= y && y < 100)
        {
            return tiles[x, y];
        }
        else return null;
    }

    public TileOWW[,] GetTiles()
    {
        return tiles;
    }
}
