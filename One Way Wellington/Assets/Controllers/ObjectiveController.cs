using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectiveController : MonoBehaviour
{

    public GameObject objectiveUIParent;
    public GameObject objectiveUIPrefab;
    public GameObject goalUIPrefab;

    private List<Objective> currentObjectives;

    private Dictionary<string, Objective> allObjectives;

    public void AddObjective(Objective objective)
    {
        currentObjectives.Add(objective);

        // Create new ObjectiveGO for UI
        GameObject objectiveGO = Instantiate(objectiveUIPrefab);
        objectiveGO.transform.SetParent(objectiveUIParent.transform);
        objectiveGO.GetComponent<ObjectiveUI>().title.text = objective.title;

        foreach (Goal goal in objective.goals)
        {
            GameObject goalGO = Instantiate(goalUIPrefab);
            goalGO.transform.SetParent(objectiveGO.GetComponent<ObjectiveUI>().goalsParent);
            goalGO.GetComponentInChildren<TextMeshProUGUI>().text = goal.title;
        }
    }

    private void Start()
    {
        allObjectives = new Dictionary<string, Objective>();
        currentObjectives = new List<Objective>();

        List<Goal> goals = new List<Goal>
        {
            new Goal_Furniture("Build at least 30 hull tiles", 30, "Hull")
        };

        List<string> nextObjectives = new List<string>
        {
            "Staff Basics",
            "Get Moving",
            "Habitable"
        };

        // The Hull
        allObjectives.Add("The Hull",
            new Objective(
                "The Hull",
                new List<Goal>
                {
                    new Goal_Hull("Build at least 30 hull tiles", 30),
                    new Goal_Staff("Hire a Builder", 1, "Builder")
                },
                0,
                new List<string>
                {
                    "Staff Basics",
                    "Get Moving",
                    "Habitable"
                }
         ));


        allObjectives.Add("Staff Basics",
            new Objective(
                "Staff Basics",
                new List<Goal>
                {
                    new Goal_Staff("Hire a Captain", 1, "Captain"),
                    new Goal_Staff("Hire a Guard", 1, "Guard"),
                    new Goal_Furniture("Build a Charging Pad", 1, "Charging Pad"),
                    new Goal_Furniture("Build a Battery", 1, "Battery"),

                },
                0,
                new List<string>()                              
         ));



        // DEBUG
        AddObjective(allObjectives["The Hull"]);
        AddObjective(allObjectives["Staff Basics"]);
    }

}
