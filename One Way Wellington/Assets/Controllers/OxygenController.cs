using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class OxygenController : MonoBehaviour
{
    public Tilemap tilemap;


    Queue<TileOWW> oxygenDecayQueue = new Queue<TileOWW>();


    Queue<TileOWW> tilesToCheck;
    Dictionary<TileOWW, float> oxygenLevels;
    Dictionary<TileOWW, float> oxygenDifferences;



    public void FixedUpdate()
    {
        // Clear dictionaries
        tilesToCheck = new Queue<TileOWW>();
        oxygenLevels = new Dictionary<TileOWW, float>();
        oxygenDifferences = new Dictionary<TileOWW, float>();


        // Oxygen decay 
        while (oxygenDecayQueue.Count > 0)
        {
            TileOWW decayTile = oxygenDecayQueue.Dequeue();
            if (decayTile.GetTileType() != "Hull")
            {
                decayTile.oxygenLevel = 0;
                UpdateOxygenSprite(decayTile, 0);
                Debug.Log("Oxygen escaped at this tile: " + decayTile.ToString());
            }

        }

        // Add oxygen at each Oxygen Vent 
        VentOxygen();

        // Put all tiles with oxygen into the queue tilesToCheck
        // And dictionary oxygenCurrent
        foreach (TileOWW tileOWW in WorldController.Instance.GetWorld().GetTiles())
        {
            if (tileOWW.GetTileType() == "Hull")
            {
                tilesToCheck.Enqueue(tileOWW);
                oxygenLevels.Add(tileOWW, tileOWW.oxygenLevel);
            }
        }

        StartCoroutine(UpdateOxygen());
        



    }

    IEnumerator UpdateOxygen()
    {
        // Go through all tiles in tilesToCheck
        while (tilesToCheck.Count > 0)
        {

            TileOWW nextTile = tilesToCheck.Dequeue();
            TileOxygen(nextTile);

        }

        // Apply all of the oxygen changes 
        foreach (KeyValuePair<TileOWW, float> keyValuePair in oxygenDifferences)
        {
            TileOWW currentTile = keyValuePair.Key;

            // Debug.Log("Applying this: " + keyValuePair.Value + " to tile: " + keyValuePair.Key);

            currentTile.oxygenLevel += keyValuePair.Value;

            UpdateOxygenSprite(currentTile, keyValuePair.Value);

            // Check for oxygen escape! 
            if (currentTile.GetTileType() != "Hull")
            {
                currentTile.oxygenLevel = 0;
                if (!oxygenDecayQueue.Contains(currentTile))
                {
                    oxygenDecayQueue.Enqueue(currentTile);
                }
            }
        }
        yield return null;
    }


    // DEBUG FUNCTION 
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

    public void VentOxygen()
    {
        if (BuildModeController.Instance.furnitureTileOWWMap.ContainsKey("Oxygen Vent"))
        {

            foreach (TileOWW tile in BuildModeController.Instance.furnitureTileOWWMap["Oxygen Vent"])
            {
                // Check oxygen vent tile first 
                if (tile.oxygenLevel < 1)
                {
                    tile.oxygenLevel += 0.25f;
                    // TODO: Remove from oxygen tank supply 
                }
            }
        }
    }



    private void TileOxygen(TileOWW tile)
    {

        if (oxygenLevels[tile] < 0.01)
        {
            return;
        }
     

        List<TileOWW> tileSpreads = new List<TileOWW>(); // Neighbour tiles that can accept oxygen.
        float oxygenTotal = oxygenLevels[tile]; // Total oxygen of this tile and of neughour tiles that can accept oxygen 

        TileOWW tileNorth = WorldController.Instance.GetWorld().GetTileAt(tile.GetX(), tile.GetY() + 1);
        TileOWW tileEast = WorldController.Instance.GetWorld().GetTileAt(tile.GetX() + 1, tile.GetY());
        TileOWW tileSouth = WorldController.Instance.GetWorld().GetTileAt(tile.GetX(), tile.GetY() - 1);
        TileOWW tileWest = WorldController.Instance.GetWorld().GetTileAt(tile.GetX() - 1, tile.GetY());

        

        if (tileNorth != null)
        {
            if (oxygenLevels.ContainsKey(tileNorth))
            {
                if (oxygenLevels[tileNorth] < oxygenLevels[tile] && tileNorth.GetInstalledFurniture()?.GetFurnitureType() != "Wall" && tileNorth.GetInstalledFurniture()?.GetFurnitureType() != "Airlock")
                {

                    tileSpreads.Add(tileNorth);
                    oxygenTotal += oxygenLevels[tileNorth];
                    // Debug.Log("We can operate on north tile. Oxygen total is now: " + oxygenTotal);
                }
            }
            else
            {
                oxygenLevels.Add(tileNorth, 0);
                tileSpreads.Add(tileNorth);
            }
        }
        if (tileEast != null)
        {
            if (oxygenLevels.ContainsKey(tileEast))
            {
                if (oxygenLevels[tileEast] < oxygenLevels[tile] && tileEast.GetInstalledFurniture()?.GetFurnitureType() != "Wall" && tileEast.GetInstalledFurniture()?.GetFurnitureType() != "Airlock")
                {
                    tileSpreads.Add(tileEast);
                    oxygenTotal += oxygenLevels[tileEast];
                    // Debug.Log("We can operate on east tile. Oxygen total is now: " + oxygenTotal);

                }
            }
            else
            {
                oxygenLevels.Add(tileEast, 0);

                tileSpreads.Add(tileEast);
            }
        }
        if (tileSouth != null)
        {
            if (oxygenLevels.ContainsKey(tileSouth))
            {
                if (oxygenLevels[tileSouth] < oxygenLevels[tile] && tileSouth.GetInstalledFurniture()?.GetFurnitureType() != "Wall" && tileSouth.GetInstalledFurniture()?.GetFurnitureType() != "Airlock")
                {
                    tileSpreads.Add(tileSouth);
                    oxygenTotal += oxygenLevels[tileSouth];
                    // Debug.Log("We can operate on south tile. Oxygen total is now: " + oxygenTotal);

                }
            }
            else
            {
                oxygenLevels.Add(tileSouth, 0);
                tileSpreads.Add(tileSouth);
            }
        }
        if (tileWest != null)
        {
            if (oxygenLevels.ContainsKey(tileWest))
            {
                if (oxygenLevels[tileWest] < oxygenLevels[tile] && tileWest.GetInstalledFurniture()?.GetFurnitureType() != "Wall" && tileWest.GetInstalledFurniture()?.GetFurnitureType() != "Airlock")
                {
                    tileSpreads.Add(tileWest);
                    oxygenTotal += oxygenLevels[tileWest];
                    // Debug.Log("We can operate on west tile. Oxygen total is now: " + oxygenTotal);

                }
            }
            else
            {
                oxygenLevels.Add(tileWest, 0);
                tileSpreads.Add(tileWest);
            }
        }
        float oxygenOutput = oxygenTotal / (tileSpreads.Count + 1); // New oxygen level of this tile and neighour tiles 

        float tileOxygenDifference = oxygenOutput - oxygenLevels[tile];

        // Debug.Log("Oxygen output per tile is: " + oxygenOutput);

        // Debug.Log("Old: " + oxygenLevels[tile] + " New: " + oxygenOutput + " Difference: " + tileOxygenDifference);

        if (oxygenDifferences.ContainsKey(tile))
        {
            oxygenDifferences[tile] += tileOxygenDifference;
        }
        else
        {
            oxygenDifferences.Add(tile, tileOxygenDifference);
        }

        foreach (TileOWW tileSpread in tileSpreads)
        {

            float tileSpreadOxygenDifference = oxygenOutput - oxygenLevels[tileSpread];

            if (oxygenDifferences.ContainsKey(tileSpread))
            {
                oxygenDifferences[tileSpread] += tileSpreadOxygenDifference;            
            }
            else
            {
                oxygenDifferences.Add(tileSpread, tileSpreadOxygenDifference);
            }          
        }      
    }




    private void UpdateOxygenSprite(TileOWW tileOWW, float oxygenLevel)
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
