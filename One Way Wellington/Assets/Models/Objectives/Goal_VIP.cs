using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal_VIP : Goal
{

    public string vipName;

    public Goal_VIP(string title, string vipName)
    {
        this.title = title;
        this.vipName = vipName;
    }

    public override bool CheckComplete()
    {

        Debug.LogWarning("Goal checker not working properly!!");
        return false;
    }
}
