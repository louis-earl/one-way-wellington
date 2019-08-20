using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal_Staff : Goal
{

    public string staffType;

    public Goal_Staff(string title, int goalAmount, string staffType)
    {
        this.title = title;
        this.goalAmount = goalAmount;
        this.staffType = staffType;
    }

    public override bool CheckComplete()
    {
        int staffCount = 0;

        foreach (GameObject staffGO in WorldController.Instance.staff)
        {
            if (staffGO.CompareTag(staffType))
            {
                staffCount++;
            }
        }

        if (staffCount >= goalAmount)
        {
            return true;
        }

        return false;
    }
}
