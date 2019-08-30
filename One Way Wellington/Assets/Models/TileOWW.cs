using System.Collections;
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
        if (this.installedFurniture != null) Debug.LogError("Warning: Trying to change installedFurniture without removing existing first.");
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
        return ("Tile: (" + x + ", " + y + "); TileType: " + tileType + "; InstalledFurniture: " + installedFurniture);
    }

    // Job-less and furniture-less
    public bool IsTileEmpty()
    {
        if (currentJobType != null) return false;
        if (installedFurniture != null) return false;
        if (installedFurnitureAltX != null) return false;
        if (installedFurnitureAltY != null) return false;


        return true;
    }

}
