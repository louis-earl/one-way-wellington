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

    private bool isWalkable;

    public string currentJobType;

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
        if (installedFurniture.GetFurnitureType() == "Wall")
        {
            isWalkable = false;
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

    public override string ToString()
    {
        return ("Tile: (" + x + ", " + y + "); TileType: " + tileType + "; InstalledFurniture: " + installedFurniture);
    }

}
