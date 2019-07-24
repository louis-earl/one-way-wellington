using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{

    public static RoomController Instance;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null) Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DoRoomChecks()
    {

        foreach (KeyValuePair<string, List<TileOWW>> entry in BuildModeController.Instance.roomsTileOWWMap)
        {
            Debug.Log("Looking for " + entry.Key);
            List<TileOWW> tilesToCheck = new List<TileOWW>(entry.Value);
            foreach (TileOWW tile in entry.Value)
            {
                if (tilesToCheck.Count == 0)
                {
                    break;
                }
                if (tilesToCheck.Contains(tile))
                {
                    if (CheckTile(tile, tilesToCheck))
                    {
                        Debug.Log("Found a charger!!");
                    }
                    else
                    {
                        Debug.Log("check for charger failed");
                    }

                }
            }

        }
    }


    // TODO: Return true before marking off the other tiles as checked!! 
    private bool CheckTile(TileOWW tile, List<TileOWW> tilesToCheck)
    {
        if (tilesToCheck.Contains(tile))
        {

            tilesToCheck.Remove(tile);

            bool isSuccessful = false;

            if (CheckTile(WorldController.Instance.GetWorld().GetTileAt(tile.GetX(), tile.GetY() + 1), tilesToCheck)) isSuccessful = true;
            if (CheckTile(WorldController.Instance.GetWorld().GetTileAt(tile.GetX(), tile.GetY() - 1), tilesToCheck)) isSuccessful = true;
            if (CheckTile(WorldController.Instance.GetWorld().GetTileAt(tile.GetX() + 1, tile.GetY()), tilesToCheck)) isSuccessful = true;
            if (CheckTile(WorldController.Instance.GetWorld().GetTileAt(tile.GetX() - 1, tile.GetY()), tilesToCheck)) isSuccessful = true;

            if (isSuccessful)
            {
                return true;
            }

            if (tile.GetInstalledFurniture()?.GetFurnitureType() == "Charging Pad") // TODO: The validity will be horrible 
            {
                return true;
            }
            
            return false;
        }
        else
        {
            return false;
        }
    }
}
