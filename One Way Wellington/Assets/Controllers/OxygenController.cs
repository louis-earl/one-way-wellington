using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class OxygenController : MonoBehaviour
{
    public Tilemap tilemap;

    Queue<Tuple<TileOWW, float>> oxygenTileQueue;
    HashSet<TileOWW> checkedTiles;

    // TODO: Working recursion so Unity doesn't crash!
    public void DoOxygenTick()
    {
        if (BuildModeController.Instance.furnitureTileOWWMap.ContainsKey("Oxygen Vent")) {
            foreach (TileOWW tile in BuildModeController.Instance.furnitureTileOWWMap["Oxygen Vent"])
            {
                oxygenTileQueue = new Queue<Tuple<TileOWW, float>>();
                checkedTiles = new HashSet<TileOWW>();

                // Check oxygen vent tile first 
                if (tile.oxygenLevel < 1)
                {
                    TileOxygen(tile, 1);
                    // TODO: Remove from oxygen tank supply 
                }

                // Check other tiles added in queue 
                while (oxygenTileQueue.Count > 0)
                {
                    Tuple<TileOWW, float> tuple = oxygenTileQueue.Dequeue();
                    // If tile not already checked 
                    if (!checkedTiles.Contains(tuple.Item1))
                    {
                        TileOxygen(tuple.Item1, tuple.Item2);
                    }
                }
            }
        }
    }

    // Gets the total amount of oxygen from every tile 
    public void GetOxygenCount()
    {
        float total = 0;
        foreach (TileOWW tileOWW in WorldController.Instance.GetWorld().GetTiles())
        {
            total += tileOWW.oxygenLevel;
        }
        Debug.Log("Current oxygen level is: " + total);
    }

    private void TileOxygen(TileOWW tile, float oxygenInput)
    {

        if (oxygenInput < 0.01)
        {
            // Debug.Log("Cancelled because oxygen was less than 0.01");
            tile.oxygenLevel += oxygenInput;
            return;
        }

        

        List<TileOWW> tileSpreads = new List<TileOWW>(); // Neighbour tiles that can accept oxygen.
        float oxygenTotal = oxygenInput; // Total oxygen of this tile and of neughour tiles that can accept oxygen 

        TileOWW tileNorth = WorldController.Instance.GetWorld().GetTileAt(tile.GetX(), tile.GetY() + 1);
        TileOWW tileEast = WorldController.Instance.GetWorld().GetTileAt(tile.GetX() + 1, tile.GetY());
        TileOWW tileSouth = WorldController.Instance.GetWorld().GetTileAt(tile.GetX(), tile.GetY() - 1);
        TileOWW tileWest = WorldController.Instance.GetWorld().GetTileAt(tile.GetX() - 1, tile.GetY());

        

        if (tileNorth != null)
        {
            if (tileNorth.oxygenLevel < oxygenInput && tileNorth.GetInstalledFurniture()?.GetFurnitureType() != "Wall" && tileNorth.GetInstalledFurniture()?.GetFurnitureType() != "Airlock")
            {
                
                tileSpreads.Add(tileNorth);
                oxygenTotal += tileNorth.oxygenLevel;
                // Debug.Log("We can operate on north tile. Oxygen total is now: " + oxygenTotal);
            }
        }
        if (tileEast != null)
        {
            if (tileEast.oxygenLevel < oxygenInput && tileEast.GetInstalledFurniture()?.GetFurnitureType() != "Wall" && tileEast.GetInstalledFurniture()?.GetFurnitureType() != "Airlock")
            {
                tileSpreads.Add(tileEast);
                oxygenTotal += tileEast.oxygenLevel;
                // Debug.Log("We can operate on east tile. Oxygen total is now: " + oxygenTotal);

            }
        }
        if (tileSouth != null)
        {
            if (tileSouth.oxygenLevel < oxygenInput && tileSouth.GetInstalledFurniture()?.GetFurnitureType() != "Wall" && tileSouth.GetInstalledFurniture()?.GetFurnitureType() != "Airlock")
            {
                tileSpreads.Add(tileSouth);
                oxygenTotal += tileSouth.oxygenLevel;
                // Debug.Log("We can operate on south tile. Oxygen total is now: " + oxygenTotal);

            }
        }
        if (tileWest != null)
        {
            if (tileWest.oxygenLevel < oxygenInput && tileWest.GetInstalledFurniture()?.GetFurnitureType() != "Wall" && tileWest.GetInstalledFurniture()?.GetFurnitureType() != "Airlock")
            {
                tileSpreads.Add(tileWest);
                oxygenTotal += tileWest.oxygenLevel;
                // Debug.Log("We can operate on west tile. Oxygen total is now: " + oxygenTotal);

            }
        }
        float oxygenOutput = oxygenTotal / (tileSpreads.Count + 1); // New oxygen level of this tile and neighour tiles 

        // Debug.Log("Oxygen output per tile is: " + oxygenOutput);

        tile.oxygenLevel = oxygenOutput;
        if (oxygenOutput > 1)
        {
            Debug.LogWarning("Tiles oxygen level is > 1: " + tile.ToString());
        }

        checkedTiles.Add(tile);

        foreach (TileOWW tileSpread in tileSpreads)
        {
            tileSpread.oxygenLevel = oxygenOutput;
            Tuple<TileOWW, float> tuple = new Tuple<TileOWW, float>(tileSpread, oxygenOutput);
            foreach (Tuple <TileOWW, float> existingTuple in oxygenTileQueue)
            {
                if (existingTuple.Item1 == tileSpread)
                {
                    
                }

            }
            oxygenTileQueue.Enqueue(tuple);
        }
        Debug.Log(tile.ToString() + "|| Input: " + oxygenInput + " Total: " + oxygenTotal + " Output: " + oxygenOutput);
        GetOxygenCount();
        UpdateOxygenSprite(tile);
    }


    private void UpdateOxygenSprite(TileOWW tileOWW)
    {
        Tile t;
        if (tileOWW.oxygenLevel > 0)
        {
            // Create room graphics 
            t = Resources.Load<Tile>("TileSets/Rooms/Oxygen");
            t.color = new Color(0, 1, 1, tileOWW.oxygenLevel);

            tilemap.SetTile(new Vector3Int(tileOWW.GetX(), tileOWW.GetY(), 0), t);
            tilemap.RefreshTile(new Vector3Int(tileOWW.GetX(), tileOWW.GetY(), 0));
        }
        else
        {
            // Remove tile graphics 
            t = null;
            tilemap.SetTile(new Vector3Int(tileOWW.GetX(), tileOWW.GetY(), 0), t);
            tilemap.RefreshTile(new Vector3Int(tileOWW.GetX(), tileOWW.GetY(), 0));
        }
    }

}
