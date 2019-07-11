using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InstalledFurniture
{
    private string furnitureType;

    public InstalledFurniture(string furnitureType)
    {
        this.furnitureType = furnitureType;
    }

    public void SetFurnitureType(string furnitureType)
    {
        this.furnitureType = furnitureType;
    }

    public string GetFurnitureType ()
    {
        return furnitureType;
    }
}
