using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileType
{
    private static Sprite empty;
    private static Sprite hull;
    private static Sprite grid;



    public static Sprite Empty { 
        get {
            if (empty == null)
            {
                empty = Resources.Load<Sprite>("Images/Tiles/empty");
            }
            return empty;
        } 
    }

    public static Sprite Hull
    {
        get
        {
            if (hull == null)
            {
                hull = Resources.Load<Sprite>("Images/Tiles/hull");
            }
            return hull;
        }
    }

    public static Sprite Grid
    {
        get
        {
            if (grid == null)
            {
                grid = Resources.Load<Sprite>("Images/Tiles/grid");
            }
            return grid;
        }
    }
}
