using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal_Room : Goal
{

    public string roomType;

    public Goal_Room(string title, int goalAmount, string roomType)
    {
        this.title = title;
        this.goalAmount = goalAmount;
        this.roomType = roomType;
    }

    public override bool CheckComplete()
    {
        Debug.LogWarning("Goal checker not working properly!!");
        return false;
    }
}
