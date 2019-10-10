using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class OxygenController : MonoBehaviour
{
    public static OxygenController Instance;

    public Tilemap tilemap;

    // Oxygen usage
    public float shipOxygenLevel;

    HashSet<TileOWW> uncheckedTiles;
    Queue<TileOWW> tilesToCheck;
    Dictionary<TileOWW, List<TileOWW>> oxygenTileGroups;

	private static float OXYGEN_COST = 0.00585f;

    private static readonly float VENT_FLOW = 20f; // Flow from Oxygen Vent per FixedUpdate 

    private void Start()
    {
        if (Instance == null) Instance = this;
    }

    public void FixedUpdate()
    {
        // Clear dictionaries
        tilesToCheck = new Queue<TileOWW>();
        oxygenTileGroups = new Dictionary<TileOWW, List<TileOWW>>();
        uncheckedTiles = new HashSet<TileOWW>();


        // Put all tiles with oxygen into the queue tilesToCheck
        foreach (TileOWW tileOWW in WorldController.Instance.GetWorld().GetAllTiles())
        {
            if (tileOWW.GetTileType() == "Hull" && tileOWW.GetInstalledFurniture()?.GetFurnitureType() != "Wall" && tileOWW.GetInstalledFurniture()?.GetFurnitureType() != "Airlock") 
            {
                tilesToCheck.Enqueue(tileOWW);
             }

            // We still want to check empty tiles for oxygen loss
            uncheckedTiles.Add(tileOWW);

        }

        // Add oxygen via Vents 
        VentOxygen();

        // Go through all tiles in tilesToCheck
        while (tilesToCheck.Count > 0)
        {
            // Origin tile is used as a key in oxygenTileGroups
            TileOWW originTile = tilesToCheck.Dequeue();
            if (uncheckedTiles.Contains(originTile))
            {
                oxygenTileGroups.Add(originTile, new List<TileOWW>());
                TileOxygen(originTile, originTile);
            }

        }

        // Go through all tile groups 
        foreach(TileOWW originTile in oxygenTileGroups.Keys)
        {
            // Get total amount of oxygen in group 
            float oxygenTotal = 0;
            foreach(TileOWW tile in oxygenTileGroups[originTile])
            {
                if (tile.GetTileType() == "Empty")
                {
                    oxygenTotal = 0;
                    break;
                }
                else
                {
                    oxygenTotal += tile.oxygenLevel;
                }
            }

            float oxygenOutput = oxygenTotal / oxygenTileGroups[originTile].Count;

            // Set all tiles in group average oxygen value 

            foreach (TileOWW tile in oxygenTileGroups[originTile])
            {
                tile.oxygenLevel = oxygenOutput;
                UpdateOxygenSprite(tile, oxygenOutput);
            }

        }

    }


    // DEBUG FUNCTION 
    // Gets the total amount of oxygen from every tile 
    public void GetOxygenCount()
    {
        float total = 0;
        foreach (TileOWW tileOWW in WorldController.Instance.GetWorld().GetAllTiles())
        {
            total += tileOWW.oxygenLevel;
        }
        Debug.Log("Current oxygen level is: " + total);
    }


    public void VentOxygen()
    {
        if (BuildModeController.Instance.furnitureTileOWWMap.ContainsKey("Oxygen Vent"))
        {
            // Is there any oxygen to use
            if (shipOxygenLevel >= 0.5f)
            {
                foreach (TileOWW tile in BuildModeController.Instance.furnitureTileOWWMap["Oxygen Vent"])
                {
                    // Check oxygen vent tile first 
                    if (tile.oxygenLevel < 100)
                    {
                        // Remove from oxygen tank supply
                        shipOxygenLevel -= VENT_FLOW;

                        // Add oxygen to ship
                        tile.oxygenLevel += VENT_FLOW;
                    }
                }
            }
        }
    }



    private void TileOxygen(TileOWW originTile, TileOWW nextTile)
    {
        // Base case #1 
        if (nextTile.GetInstalledFurniture()?.GetFurnitureType() == "Wall" || nextTile.GetInstalledFurniture()?.GetFurnitureType() == "Airlock")
        {
            return;
        }

        // Base case #2 
        if (!uncheckedTiles.Contains(nextTile))
        {
            return;
        }

        // Main
        oxygenTileGroups[originTile].Add(nextTile);
        if (uncheckedTiles.Contains(nextTile)) uncheckedTiles.Remove(nextTile);

        // Base case #3 
        // Note we still add this tile to the group without spreading to other tiles 
        if (nextTile.GetTileType() == "Empty")
        {
            return;
        }

        // Spread to next tiles 
        TileOWW tileNorth = WorldController.Instance.GetWorld().GetTileAt(nextTile.GetX(), nextTile.GetY() + 1);
        TileOWW tileEast = WorldController.Instance.GetWorld().GetTileAt(nextTile.GetX() + 1, nextTile.GetY());
        TileOWW tileSouth = WorldController.Instance.GetWorld().GetTileAt(nextTile.GetX(), nextTile.GetY() - 1);
        TileOWW tileWest = WorldController.Instance.GetWorld().GetTileAt(nextTile.GetX() - 1, nextTile.GetY());
      
        if (tileNorth != null)
        {
            {
                TileOxygen(originTile, tileNorth);
            }
        }
        if (tileEast != null)
        {
            {
                TileOxygen(originTile, tileEast);
            }
        }
        if (tileSouth != null)
        {
            {
                TileOxygen(originTile, tileSouth);
            }
        }
        if (tileWest != null)
        {
            {
                TileOxygen(originTile, tileWest);
            }
        }


   
    }


    public void RestockOxygen()
    {
        float currentOxygenLevel = shipOxygenLevel;
        float maxOxygenLevel = 0;
        if (BuildModeController.Instance.furnitureTileOWWMap.ContainsKey("Oxygen Tank"))
        {
            maxOxygenLevel = BuildModeController.Instance.furnitureTileOWWMap["Oxygen Tank"].Count * 10000;
            Debug.Log("Found oxygen tanks: " + BuildModeController.Instance.furnitureTileOWWMap["Oxygen Tank"].Count);

        }


        shipOxygenLevel = maxOxygenLevel;
        CurrencyController.Instance.DeductBankBalance((int)((maxOxygenLevel - currentOxygenLevel) * OXYGEN_COST));

        Debug.Log("Charged " + ((maxOxygenLevel - currentOxygenLevel) * OXYGEN_COST) + " for oxygen."); 
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
                t.color = new Color(0.227451f, 0.7294118f, 0.9960784f, tileOWW.oxygenLevel/50);
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
