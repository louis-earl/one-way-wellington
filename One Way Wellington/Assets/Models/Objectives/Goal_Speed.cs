using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal_Speed : Goal
{

    public int shipSpeed;

    public Goal_Speed(string title, int goalAmount)
    {
        this.title = title;
        this.goalAmount = goalAmount;
    }

    public override bool CheckComplete()
    {
        Debug.LogWarning("Goal checker not working properly!!");
        return false;
    }
}
