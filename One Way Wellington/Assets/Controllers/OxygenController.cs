using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class OxygenController : MonoBehaviour
{
    public Tilemap tilemap;

    Queue<TileOWW> oxygenTileQueue;
    HashSet<TileOWW> checkedTiles;
    Queue<TileOWW> oxygenDecayQueue = new Queue<TileOWW>();



    public void FixedUpdate()
    {
        if (BuildModeController.Instance.furnitureTileOWWMap.ContainsKey("Oxygen Vent")) {
            while (oxygenDecayQueue.Count > 0)
            {
                TileOWW decayTile = oxygenDecayQueue.Dequeue();
                if (decayTile.GetTileType() != "Hull")
                {
                    decayTile.oxygenLevel = 0;
                    UpdateOxygenSprite(decayTile);
                }

            }
            foreach (TileOWW tile in BuildModeController.Instance.furnitureTileOWWMap["Oxygen Vent"])
            {
                oxygenTileQueue = new Queue<TileOWW>();
                checkedTiles = new HashSet<TileOWW>();

                // Check oxygen vent tile first 
                if (tile.oxygenLevel < 1)
                {
                    tile.oxygenLevel += 1;

                    // TODO: Remove from oxygen tank supply 
                }

                TileOxygen(tile);

                // Check other tiles added in queue 
                while (oxygenTileQueue.Count > 0)
                {
                    TileOWW nextTile = oxygenTileQueue.Dequeue();
                    // If tile not already checked 
                    if (!checkedTiles.Contains(nextTile))
                    {
                        TileOxygen(nextTile);
                    }
                }


            }
        }
    }


    IEnumerator UpdateOxygen()
    {
        if (BuildModeController.Instance.furnitureTileOWWMap.ContainsKey("Oxygen Vent"))
        {
            foreach (TileOWW tile in BuildModeController.Instance.furnitureTileOWWMap["Oxygen Vent"])
            {
                oxygenTileQueue = new Queue<TileOWW>();
                checkedTiles = new HashSet<TileOWW>();

                // Check oxygen vent tile first 
                if (tile.oxygenLevel < 1)
                {
                    tile.oxygenLevel += 1;

                    // TODO: Remove from oxygen tank supply 
                }

                TileOxygen(tile);

                // Check other tiles added in queue 
                while (oxygenTileQueue.Count > 0)
                {
                    TileOWW nextTile = oxygenTileQueue.Dequeue();
                    // If tile not already checked 
                    if (!checkedTiles.Contains(nextTile))
                    {
                        TileOxygen(nextTile);
                    }
                    yield return null;
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

    private void TileOxygen(TileOWW tile)
    {


        if (tile.oxygenLevel < 0.01)
        {
            // Debug.Log("Cancelled because oxygen was less than 0.01");
            return;
        }

        


        List<TileOWW> tileSpreads = new List<TileOWW>(); // Neighbour tiles that can accept oxygen.
        float oxygenTotal = tile.oxygenLevel; // Total oxygen of this tile and of neughour tiles that can accept oxygen 

        TileOWW tileNorth = WorldController.Instance.GetWorld().GetTileAt(tile.GetX(), tile.GetY() + 1);
        TileOWW tileEast = WorldController.Instance.GetWorld().GetTileAt(tile.GetX() + 1, tile.GetY());
        TileOWW tileSouth = WorldController.Instance.GetWorld().GetTileAt(tile.GetX(), tile.GetY() - 1);
        TileOWW tileWest = WorldController.Instance.GetWorld().GetTileAt(tile.GetX() - 1, tile.GetY());

        

        if (tileNorth != null)
        {
            if (tileNorth.oxygenLevel < tile.oxygenLevel && tileNorth.GetInstalledFurniture()?.GetFurnitureType() != "Wall" && tileNorth.GetInstalledFurniture()?.GetFurnitureType() != "Airlock")
            {
                
                tileSpreads.Add(tileNorth);
                oxygenTotal += tileNorth.oxygenLevel;
                // Debug.Log("We can operate on north tile. Oxygen total is now: " + oxygenTotal);
            }
        }
        if (tileEast != null)
        {
            if (tileEast.oxygenLevel < tile.oxygenLevel && tileEast.GetInstalledFurniture()?.GetFurnitureType() != "Wall" && tileEast.GetInstalledFurniture()?.GetFurnitureType() != "Airlock")
            {
                tileSpreads.Add(tileEast);
                oxygenTotal += tileEast.oxygenLevel;
                // Debug.Log("We can operate on east tile. Oxygen total is now: " + oxygenTotal);

            }
        }
        if (tileSouth != null)
        {
            if (tileSouth.oxygenLevel < tile.oxygenLevel && tileSouth.GetInstalledFurniture()?.GetFurnitureType() != "Wall" && tileSouth.GetInstalledFurniture()?.GetFurnitureType() != "Airlock")
            {
                tileSpreads.Add(tileSouth);
                oxygenTotal += tileSouth.oxygenLevel;
                // Debug.Log("We can operate on south tile. Oxygen total is now: " + oxygenTotal);

            }
        }
        if (tileWest != null)
        {
            if (tileWest.oxygenLevel < tile.oxygenLevel && tileWest.GetInstalledFurniture()?.GetFurnitureType() != "Wall" && tileWest.GetInstalledFurniture()?.GetFurnitureType() != "Airlock")
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
            // Debug.LogWarning("Tiles oxygen level is > 1: " + tile.ToString());
        }

        checkedTiles.Add(tile);

        foreach (TileOWW tileSpread in tileSpreads)
        {

            tileSpread.oxygenLevel = oxygenOutput;
            if (tileSpread.GetTileType() != "Hull")
            {
                UpdateOxygenSprite(tileSpread);
                tileSpread.oxygenLevel = 0;
                if (!oxygenDecayQueue.Contains(tileSpread))
                {
                    oxygenDecayQueue.Enqueue(tileSpread);
                }
            }
            else
            {
                oxygenTileQueue.Enqueue(tileSpread);
            }
        }
        // Debug.Log(tile.ToString() + "|| Input: " + tile.oxygenLevel + " Total: " + oxygenTotal + " Output: " + oxygenOutput);
        // GetOxygenCount();
        UpdateOxygenSprite(tile);

        
    }


    private void UpdateOxygenSprite(TileOWW tileOWW)
    {
        Tile t;
        if (tileOWW.oxygenLevel > 0)
        {
            // Create room graphics 
            t = Resources.Load<Tile>("TileSets/Rooms/Oxygen");
            if (tileOWW.GetTileType() == "Hull")
            {
                t.color = new Color(0.227451f, 0.7294118f, 0.9960784f, tileOWW.oxygenLevel);
            }
            else
            {
                t.color = new Color(0.9215686f, 0.5764706f, 0.1176471f);
                // TODO: Notification 
            }

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
