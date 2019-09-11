using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal_CameraSize : Goal
{
    private float cameraSizeInitial;
    private bool isComplete = false;

    public Goal_CameraSize(string title, float cameraSizeInitial, int goalAmount)
    {
        this.title = title;
        this.cameraSizeInitial = cameraSizeInitial;
        this.goalAmount = goalAmount;
    }

    public override bool CheckComplete()
    {
        if (Mathf.Abs(cameraSizeInitial - Camera.main.orthographicSize) > goalAmount)
        {
            isComplete = true;
        }
        return isComplete;
    }
}

