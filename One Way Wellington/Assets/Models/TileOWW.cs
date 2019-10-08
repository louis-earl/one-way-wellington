﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileOWW 
{
    private int x;
    private int y;

    private string tileType;

    private InstalledFurniture installedFurniture;
    public int ?installedFurnitureAltX; // Used if this tile is apart of a multi-tile furniture object, 
    public int ?installedFurnitureAltY; // and links to the furniture origin tile.

    private string roomType;

    private bool isWalkable;

    public string currentJobType;

    public float oxygenLevel;

    public LooseItem looseItem;

    public TileOWW(int x, int y, string tileType)
    {
        this.x = x;
        this.y = y;
        this.tileType = tileType;
        
    }

    public int GetX()
    {
        return x;
    }

    public int GetY()
    {
        return y;
    }

    public void SetTileType(string tileType)
    {
		this.tileType = tileType;
        TileSpriteController.Instance.UpdateTile(this);

    }

    public string GetTileType()
    {
        return tileType;
    }

    public void SetInstalledFurniture(InstalledFurniture installedFurniture)
    {
        if (this.installedFurniture != null) Debug.LogWarning("Warning: Trying to change installedFurniture without removing existing first.");
        this.installedFurniture = installedFurniture;
        if (BuildModeController.Instance.furnitureTypes.ContainsKey(installedFurniture.GetFurnitureType()))
        {
            // If tile is not walkable 
            if (BuildModeController.Instance.furnitureTypes[installedFurniture.GetFurnitureType()].walkable == false) {
                isWalkable = false;
                roomType = null;
                RoomSpriteController.Instance.UpdateRoom(this);
            }
            else
            {
                isWalkable = true;
            }
        }
        else
        {
            isWalkable = true;
        }

        FurnitureSpriteController.Instance.UpdateFurniture(this);
    }

    public InstalledFurniture GetInstalledFurniture()
    {
        return installedFurniture;
    }

    public InstalledFurniture RemoveInstalledFurniture()
    {
        // Should only be accessed by a Builder after their RemoveFurniture job is complete.
        // Use BuildModeController to handle the RemoveFurniture job.

        InstalledFurniture temp = installedFurniture;
        installedFurniture = null;
        isWalkable = true;
        FurnitureSpriteController.Instance.UpdateFurniture(this);
        return temp;
    }

    public bool GetIsWalkable()
    {
        return isWalkable;
    }

    public void SetIsWalkable(bool isWalkable)
    {
        this.isWalkable = isWalkable;
    }

    public string GetRoomType()
    {
        return roomType;
    }

    public void SetRoomType(string roomType)
    {
        this.roomType = roomType;
        RoomSpriteController.Instance.UpdateRoom(this);

    }

    public override string ToString()
    {
        return ("Tile: (" + x + ", " + y + "); TileType: " + tileType + "; InstalledFurniture: " + installedFurniture?.GetFurnitureType());
    }

    // Job-less and furniture-less
    public bool IsTileEmpty()
    {
        if (currentJobType != null)
        {
            // Debug.Log(currentJobType);
            return false;
        }
        if (installedFurniture != null)
        {
            // Debug.Log(installedFurniture);
            return false;
        }
        if (installedFurnitureAltX != null)
        {
            // Debug.Log(installedFurnitureAltX);
            return false;
        }
        if (installedFurnitureAltY != null)
        {
            // Debug.Log(installedFurnitureAltY);
            return false;
        }

        return true;
    }

	public void CollectCargo(int quantity)
	{
        // Another character bet us to the job?
        if (looseItem == null) return;


        // This means to collect all 
        if (quantity == -1)
        {
            quantity = looseItem.quantity;
            Debug.Log("Collecting all " + looseItem.itemType + " cargo: " + quantity);

        }

        looseItem.quantity -= quantity;

		// Global stock reference 
		CargoController.Instance.unusedShipStock[looseItem.itemType] -= quantity;
		if (CargoController.Instance.unusedShipStock[looseItem.itemType] < 0)
		{
			Debug.LogWarning("Ship stock went below 0 for " + looseItem.itemType);
		}

		if (looseItem.quantity == 0)
		{
			// Type check 
			if (installedFurniture.GetFurnitureType() == "Cargo")
			{
				CargoController.Instance.shipStockLocations[looseItem.itemType].Remove(this);
				looseItem = null;
                BuildModeController.Instance.RemoveFurniture(this);
			}
			else
			{
				Debug.LogWarning("Trying to remove cargo but installed furniture type didn't match.");
			}
			FurnitureSpriteController.Instance.UpdateFurniture(this);
		}
        else if (looseItem.quantity < 0)
        {
            Debug.LogError("Quantity of item is below 0!");
        }
	}

}
