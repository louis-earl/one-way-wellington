using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal_Hull : Goal
{

    public Goal_Hull(string title, int goalAmount)
    {
        this.title = title;
        this.goalAmount = goalAmount;
    }

    public override bool CheckComplete()
    {
        if (BuildModeController.Instance.emptyHullTiles.Count >= goalAmount)
        {
            return true;
        }
        return false;
    }
}
