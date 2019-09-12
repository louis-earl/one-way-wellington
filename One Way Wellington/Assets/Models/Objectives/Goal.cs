using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Goal
{
    public string title;
    public int goalAmount;

    public abstract bool CheckComplete();
}
