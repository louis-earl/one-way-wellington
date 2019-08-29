﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectiveController : MonoBehaviour
{
    public static ObjectiveController Instance;

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
        objectiveGO.GetComponent<ObjectiveUI>().onComplete.text = string.Format("{0:C} when completed", objective.reward);
        objectiveGO.GetComponent<ObjectiveUI>().buttonClose.GetComponent<Button>().onClick.AddListener(delegate () { CloseObjective(objectiveGO); });
        objectiveGO.GetComponent<ObjectiveUI>().objectiveReference = objective;

        foreach (Goal goal in objective.goals)
        {
            GameObject goalGO = Instantiate(goalUIPrefab);
            goalGO.transform.SetParent(objectiveGO.GetComponent<ObjectiveUI>().goalsParent);
            goalGO.GetComponentInChildren<TextMeshProUGUI>().text = goal.title;
        }
    }

    public void CheckObjectives()
    {

        List<Objective> objectivesToRemove = new List<Objective>(); // Temp store completed objectives as to not remove while iterating 

        foreach (Objective objective in currentObjectives)
        {
            // Find matching GameObject 
            GameObject objectiveGO = null;

            for (int i = 0; i < objectiveUIParent.transform.childCount; i++)
            {
                Transform objectiveTransform = objectiveUIParent.transform.GetChild(i);
                if (objective.title.Equals(objectiveTransform.GetComponent<ObjectiveUI>().title.text))
                {
                    objectiveGO = objectiveTransform.gameObject;
                    break;
                }
            }

            if (objective.CheckComplete(objectiveGO))
            {
                // Turn on close button
                objectiveGO.GetComponent<ObjectiveUI>().buttonClose.SetActive(true);

                

                objectiveGO.GetComponent<ObjectiveUI>().onComplete.text = "Click to claim reward!";
                objectiveGO.GetComponent<Button>().onClick.AddListener(delegate () { CloseObjective(objectiveGO); });

                objectivesToRemove.Add(objective);               
            }
        }

        foreach (Objective completedObjective in objectivesToRemove)
        {
            currentObjectives.Remove(completedObjective);
        }
    }

    public void CloseObjective(GameObject objectiveGO)
    {
        // Enable next objectives 
        foreach (string nextObjective in objectiveGO.GetComponent<ObjectiveUI>().objectiveReference.nextObjectives)
        {
            AddObjective(allObjectives[nextObjective]);
        }

        CurrencyController.Instance.ChangeBankBalance(objectiveGO.GetComponent<ObjectiveUI>().objectiveReference.reward);
        Destroy(objectiveGO);
    }

    private void Start()
    {
        if (Instance == null) Instance = this;


        allObjectives = new Dictionary<string, Objective>();
        currentObjectives = new List<Objective>();

        List<Goal> goals = new List<Goal>
        {
            new Goal_Furniture("Build at least 32 hull tiles", 32, "Hull")
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
                1000,
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
                1500,
                new List<string>()                              
         ));


        allObjectives.Add("Get Moving",
            new Objective(
                "Get Moving",
                new List<Goal>
                {
                    new Goal_Furniture("Build an Engine", 1, "Engine"),
                    new Goal_Furniture("Build at least 6 Fuel Tanks", 1, "Fuel Tank"),
                    new Goal_Furniture("Build the Ship Controls", 1, "Ship Controls"),
                    new Goal_Furniture("Build the Staircase", 1, "Staircase"),
                },
                2000,
                new List<string>()
         ));

        allObjectives.Add("Habitable",
            new Objective(
                "Habitable",
                new List<Goal>
                {
                    new Goal_Furniture("Build an Oxygen Vent", 1, "Oxygen Vent"),
                    new Goal_Furniture("Build at least 2 Oxygen Tanks", 1, "Oxygen Tank"),
                },
                1000,
                new List<string>()
         ));


        allObjectives.Add("Wellingtonian I",
           new Objective(
               "Wellingtonian I",
               new List<Goal>
               {
                    new Goal_VIP("Get musician, Grayson Gilmour to Wellington", "Grayson Gilmour"),
               },
               5000,
               new List<string>
               {
                    "Wellingtonian II"
               }
        ));

        allObjectives.Add("Passenger Needs",
           new Objective(
               "Passenger Needs",
               new List<Goal>
               {
                    new Goal_Room("Have a working bathroom", 1, "Bathroom"),
                    new Goal_Room("Have a working canteen", 1, "Canteen"),
                    new Goal_Room("Have 10 living quarters", 10, "Living Quarters"),
               },
               3000,
               new List<string>
               {
                    "Passenger Leisure"
               }
        ));

        allObjectives.Add("Milestone I",
           new Objective(
               "Milestone I",
               new List<Goal>
               {
                    new Goal_Passenger("Transport 10 passengers to Wellington in one journey", 10),
               },
               1000,
               new List<string>
               {
                    "Milestone II"
               }
        ));



       allObjectives.Add("Passenger Leisure",
           new Objective(
               "Passenger Leisure",
               new List<Goal>
               {
                    new Goal_Room("Have a working art studio", 1, "Art Studio"),
                    new Goal_Room("Have a working leisure room", 1, "Leisure Room"),
               },
               4000,
               new List<string>
               {
                    "Faster Than Light"
               }
        ));

        allObjectives.Add("Milestone II",
           new Objective(
               "Milestone II",
               new List<Goal>
               {
                    new Goal_Passenger("Transport 25 passengers to Wellington in one journey", 25),
               },
               2500,
               new List<string>
               {
                    "Milestone III"
               }
        ));

        allObjectives.Add("Faster Than Light",
           new Objective(
               "Faster Than Light",
               new List<Goal>
               {
                    new Goal_Speed("Achieve a ship speed of 100", 100),
               },
               4000,
               new List<string>
               {
                    
               }
        ));

        allObjectives.Add("Milestone III",
           new Objective(
               "Milestone III",
               new List<Goal>
               {
                    new Goal_Passenger("Transport 50 passengers to Wellington in one journey", 50),
               },
               5000,
               new List<string>
               {
                    "Milestone IV"
               }
        ));


        allObjectives.Add("Milestone IV",
           new Objective(
               "Milestone IV",
               new List<Goal>
               {
                    new Goal_Passenger("Transport 100 passengers to Wellington in one journey", 100),
               },
               5000,
               new List<string>
               {

               }
        ));



        // DEBUG
        AddObjective(allObjectives["The Hull"]);
        
    }

}