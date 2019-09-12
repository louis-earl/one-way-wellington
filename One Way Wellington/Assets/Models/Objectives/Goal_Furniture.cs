using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal_Furniture : Goal
{

    public string furnitureType;

    public Goal_Furniture(string title, int goalAmount, string furnitureType)
    {
        this.title = title;
        this.goalAmount = goalAmount;
        this.furnitureType = furnitureType;
    }

    public override bool CheckComplete()
    {
        if (BuildModeController.Instance.furnitureTileOWWMap.ContainsKey(furnitureType))
        {
            if (BuildModeController.Instance.furnitureTileOWWMap[furnitureType].Count >= goalAmount)
            {
                return true;
            }
        }
        return false;
    }
}
