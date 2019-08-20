using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective 
{
    public string title;
    public List<Goal> goals;
    public int reward;
    public List<string> nextObjectives;

    public Objective(string title, List<Goal> goals, int reward, List<string> nextObjectives)
    {
        this.title = title;
        this.goals = goals;
        this.reward = reward;
        this.nextObjectives = nextObjectives;
    }


}
