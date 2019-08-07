using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class OxygenController : MonoBehaviour
{
    public Tilemap tilemap;

    // Start is called before the first frame update
    void FixedUpdate()
    {
        if (BuildModeController.Instance.furnitureTileOWWMap.ContainsKey("Oxygen Vent")) {
            foreach (TileOWW tile in BuildModeController.Instance.furnitureTileOWWMap["Oxygen Vent"])
            {
                if (tile.oxygenLevel < 1)
                {
                    RecurseOxygen(tile, 1);
                    // TODO: Remove from oxygen tank supply 
                }
            }
        }
    }

    private void RecurseOxygen(TileOWW tile, float oxygenInput)
    {
        tile.oxygenLevel += oxygenInput;

        if (tile.oxygenLevel > 1)
        {
            oxygenInput += (tile.oxygenLevel - 1);
        }

        if (oxygenInput < 0.01)
        {
            return;
        }

        int tileSpreads = 0; // Number of neighbour tiles that can accept oxygen. This includes the current tile 

        TileOWW tileNorth = WorldController.Instance.GetWorld().GetTileAt(tile.GetX(), tile.GetY() + 1);
        TileOWW tileEast = WorldController.Instance.GetWorld().GetTileAt(tile.GetX() + 1, tile.GetY());
        TileOWW tileSouth = WorldController.Instance.GetWorld().GetTileAt(tile.GetX(), tile.GetY() - 1);
        TileOWW tileWest = WorldController.Instance.GetWorld().GetTileAt(tile.GetX() - 1, tile.GetY()); 

        if (tileNorth != null)
        {
            if (tileNorth.oxygenLevel < tile.oxygenLevel && tileNorth.GetInstalledFurniture()?.GetFurnitureType() != "Wall" && tileNorth.GetInstalledFurniture()?.GetFurnitureType() != "Airlock")
            tileSpreads += 1;
        }
        if (tileEast != null)
        {
            if (tileEast.oxygenLevel < tile.oxygenLevel && tileEast.GetInstalledFurniture()?.GetFurnitureType() != "Wall" && tileEast.GetInstalledFurniture()?.GetFurnitureType() != "Airlock")
                tileSpreads += 1;
        }
        if (tileSouth != null)
        {
            if (tileSouth.oxygenLevel < tile.oxygenLevel && tileSouth.GetInstalledFurniture()?.GetFurnitureType() != "Wall" && tileSouth.GetInstalledFurniture()?.GetFurnitureType() != "Airlock")
                tileSpreads += 1;
        }
        if (tileWest != null)
        {
            if (tileWest.oxygenLevel < tile.oxygenLevel && tileWest.GetInstalledFurniture()?.GetFurnitureType() != "Wall" && tileWest.GetInstalledFurniture()?.GetFurnitureType() != "Airlock")
                tileSpreads += 1;
        }
        if (tileSpreads > 0)
        {
            float oxygenOutput = oxygenInput / tileSpreads;

            if (tileNorth != null)
            {
                if (tileNorth.oxygenLevel < tile.oxygenLevel + oxygenOutput && tileNorth.GetInstalledFurniture()?.GetFurnitureType() != "Wall" && tileNorth.GetInstalledFurniture()?.GetFurnitureType() != "Airlock")
                {
                    tile.oxygenLevel -= oxygenOutput;
                    RecurseOxygen(tileNorth, oxygenOutput);
                }
            }
            if (tileEast != null)
            {
                if (tileEast.oxygenLevel < tile.oxygenLevel + oxygenOutput && tileEast.GetInstalledFurniture()?.GetFurnitureType() != "Wall" && tileEast.GetInstalledFurniture()?.GetFurnitureType() != "Airlock")
                {
                    tile.oxygenLevel -= oxygenOutput;
                    RecurseOxygen(tileEast, oxygenOutput);
                }
            }
            if (tileSouth != null)
            {
                if (tileSouth.oxygenLevel < tile.oxygenLevel + oxygenOutput && tileSouth.GetInstalledFurniture()?.GetFurnitureType() != "Wall" && tileSouth.GetInstalledFurniture()?.GetFurnitureType() != "Airlock")
                {
                    tile.oxygenLevel -= oxygenOutput;
                    RecurseOxygen(tileSouth, oxygenOutput);
                }
            }
            if (tileWest != null)
            {
                if (tileWest.oxygenLevel < tile.oxygenLevel + oxygenOutput && tileWest.GetInstalledFurniture()?.GetFurnitureType() != "Wall" && tileWest.GetInstalledFurniture()?.GetFurnitureType() != "Airlock")
                {
                    tile.oxygenLevel -= oxygenOutput;
                    RecurseOxygen(tileWest, oxygenOutput);
                }
            }
        }
        
        UpdateOxygenSprite(tile, oxygenInput);
    }


    private void UpdateOxygenSprite(TileOWW tileOWW, float oxygenUnit)
    {
        RuleTile t;
        if (tileOWW.oxygenLevel > 0)
        {
            // Create room graphics 
            t = Resources.Load<RuleTile>("TileSets/Rooms/Command Centre");
           
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
