using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal_CameraPan : Goal
{
    private Vector2 cameraPosInitial;
    private bool isComplete = false;

    public Goal_CameraPan(string title, Vector2 cameraPosInitial, int goalAmount)
    {
        this.title = title;
        this.cameraPosInitial = cameraPosInitial;
        this.goalAmount = goalAmount;
    }

    public override bool CheckComplete()
    {
        if (Vector2.Distance(new Vector2(Camera.main.transform.position.x, Camera.main.transform.position.y), cameraPosInitial) > goalAmount)
        {
            isComplete = true;
        }
        return isComplete;
    }
}
