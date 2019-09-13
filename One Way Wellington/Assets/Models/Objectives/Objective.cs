﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    public bool CheckComplete(GameObject objectiveGO)
    {

        bool isAllGoalsComplete = true;

        foreach (Goal goal in goals)
        {
            // Find matching GameObject 
            GameObject goalGO = null;

            for (int i = 0; i < goals.Count; i++)
            {
                Transform goalTransform = objectiveGO.GetComponent<ObjectiveUI>().goalsParent.GetChild(i);
                if (goal.title.Equals(goalTransform.GetComponentInChildren<TextMeshProUGUI>().text))
                {
                    goalGO = goalTransform.gameObject;
                    break;
                }
            }

            if (goal.CheckComplete())
            {
                // Update graphic               
                goalGO.GetComponent<Image>().color = new Color(0.254902f, 0.6431373f, 0.2196078f);
            }
            else
            {
                // Update graphic (in case the goal changed back to incomplete) 
                goalGO.GetComponent<Image>().color = new Color(0f, 0.5254902f, 0.8117647f);

                isAllGoalsComplete = false;
            }
        }
         
        return isAllGoalsComplete;
    }


}