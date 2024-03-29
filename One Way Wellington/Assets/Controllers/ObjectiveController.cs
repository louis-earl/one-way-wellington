﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ObjectiveController : MonoBehaviour
{
    public static ObjectiveController Instance;

    public GameObject objectiveUIParent;
    public GameObject objectiveUIPrefab;
    public GameObject goalUIPrefab;
    public GameObject toggle_Objectives;

    private List<Objective> currentObjectives;
    public List<string> completedObjectives;

    private Dictionary<string, Objective> allObjectives;

    // audio 
    public AudioSource audio_ObjectiveComplete;
    public AudioSource audio_ObjectiveRemove;


    public void AddObjective(Objective objective)
    {
        currentObjectives.Add(objective);

        // Create new ObjectiveGO for UI
        GameObject objectiveGO = Instantiate(objectiveUIPrefab);
        objectiveGO.transform.SetParent(objectiveUIParent.transform);
		objectiveGO.transform.localScale = Vector3.one;
        objectiveGO.GetComponent<ObjectiveUI>().text_Title.text = objective.title;

        // reward text 
        if (objective.reward == 0)
        {
            objectiveGO.GetComponent<ObjectiveUI>().text_OnComplete.text = "Complete in any order";
        }
        else
        {
            objectiveGO.GetComponent<ObjectiveUI>().text_OnComplete.text = string.Format("{0:C} when completed", objective.reward);
        }
        objectiveGO.GetComponent<ObjectiveUI>().buttonClose.GetComponent<Button>().onClick.AddListener(delegate () { CloseObjective(objectiveGO); });
        objectiveGO.GetComponent<ObjectiveUI>().objectiveReference = objective;

        // Create goals 
        foreach (Goal goal in objective.goals)
        {
            GameObject goalGO = Instantiate(goalUIPrefab);
            goalGO.transform.SetParent(objectiveGO.GetComponent<ObjectiveUI>().goalsParent);
			goalGO.transform.localScale = Vector3.one;
			goalGO.GetComponentInChildren<TextMeshProUGUI>().text = goal.title;
            goalGO.GetComponentInChildren<Toggle>().isOn = false;
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
                if (objective.title.Equals(objectiveTransform.GetComponent<ObjectiveUI>().text_Title.text))
                {
                    objectiveGO = objectiveTransform.gameObject;
                    break;
                }
            }

            if (objective.CheckComplete(objectiveGO))
            {
                // Turn on close button
                objectiveGO.GetComponent<ObjectiveUI>().buttonClose.SetActive(true);

                objectiveGO.GetComponent<ObjectiveUI>().isComplete = true;
                objectiveGO.GetComponent<ObjectiveUI>().StartBlinking();
                if (objective.reward == 0)
                {
                    objectiveGO.GetComponent<ObjectiveUI>().text_OnComplete.text = "Click to progress";
                }
                else
                {
                    objectiveGO.GetComponent<ObjectiveUI>().text_OnComplete.text = string.Format("Click to claim {0:C} reward", objective.reward);
                }
                objectiveGO.GetComponent<Button>().onClick.AddListener(delegate () { CloseObjective(objectiveGO); });

                objectivesToRemove.Add(objective);

                // Play sound 
                audio_ObjectiveComplete.Play();

                // UI pop-up if Objectives panel not already open 
                if (!objectiveUIParent.activeInHierarchy)
                {
                    NotificationController.Instance.CreateNotification("Objective Complete! (" + objective.title + ")", UrgencyLevel.Low, true, false, new List<string>() { "Open Objectives Panel" }, new List<System.Action>() { new System.Action(delegate () { toggle_Objectives.GetComponent<Toggle>().isOn = true; }) });
                    StartBlinking();
                }
            }
        }

        foreach (Objective completedObjective in objectivesToRemove)
        {
            currentObjectives.Remove(completedObjective);
        }
    }

    public void CloseObjective(GameObject objectiveGO)
    {

        // Check completion group 
        bool isGroupComplete = true;
        if (objectiveGO.GetComponent<ObjectiveUI>().objectiveReference.completionGroup != null)
        {
            foreach (string siblingObjective in objectiveGO.GetComponent<ObjectiveUI>().objectiveReference.completionGroup)
            {
                if (!completedObjectives.Contains(siblingObjective))
                {
                    isGroupComplete = false;
                }
            }
        }
        // Enable next objectives 
        if (isGroupComplete)
        {
            foreach (string nextObjective in objectiveGO.GetComponent<ObjectiveUI>().objectiveReference.nextObjectives)
            {
                AddObjective(allObjectives[nextObjective]);
            }
        }
        // Play sound 
        audio_ObjectiveRemove.timeSamples = 5000;
        audio_ObjectiveRemove.Play();

        completedObjectives.Add(objectiveGO.GetComponent<ObjectiveUI>().objectiveReference.title);
        CurrencyController.Instance.AddBankBalance(objectiveGO.GetComponent<ObjectiveUI>().objectiveReference.reward);
        Destroy(objectiveGO);
    }

    IEnumerator Blink()
    {
        while (true)
        {
            if (toggle_Objectives.GetComponent<Toggle>().colors.normalColor.r != 1)
            {
                ColorBlock colorBlock= toggle_Objectives.GetComponent<Toggle>().colors;
                colorBlock.normalColor = new Color32(255, 221, 0, 255);
                toggle_Objectives.GetComponent<Toggle>().colors = colorBlock;

                yield return new WaitForSecondsRealtime(0.5f);
            }
            else
            {
                ColorBlock colorBlock = toggle_Objectives.GetComponent<Toggle>().colors;
                colorBlock.normalColor = new Color32(59, 39, 186, 255);
                toggle_Objectives.GetComponent<Toggle>().colors = colorBlock;

                yield return new WaitForSecondsRealtime(0.5f);
            }
        }
    }

    public void StartBlinking()
    {
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine("Blink");
        }
    }

    public void StopBlinking()
    {
        StopAllCoroutines();
        ColorBlock colorBlock = toggle_Objectives.GetComponent<Toggle>().colors;
        colorBlock.normalColor = new Color32(59, 39, 186, 255);
        toggle_Objectives.GetComponent<Toggle>().colors = colorBlock;
    }

    public IEnumerator InitialNotificationDelayed()
    {
        yield return new WaitForSeconds(1.5f);
        NotificationController.Instance.CreateNotification("Welcome to One Way Wellington! Get started by opening your objectives panel.", UrgencyLevel.Low, true, false, new List<string>() { "Open Objectives Panel" }, new List<System.Action>() { new System.Action(delegate () { toggle_Objectives.GetComponent<Toggle>().isOn = true; }) });

    }

    private void Start()
    {
        if (Instance == null) Instance = this;

        StartBlinking();

        allObjectives = new Dictionary<string, Objective>();
        currentObjectives = new List<Objective>();

        StartCoroutine(InitialNotificationDelayed());

        if (SceneManager.GetActiveScene().name == "MainNew")
        {
            AddObjectivesForNewGame();
        }
        else
        {
            AddObjectivesForLoadGame();
        }


    }

    private void AddObjectivesForNewGame()
    {

        allObjectives.Add("Wellywood Cinematographer",
            new Objective(
                "Wellywood Cinematographer",
                new List<Goal>
                {
                    new Goal_CameraPan("Hold the right mouse button and drag to pan the camera", new Vector2(Camera.main.transform.position.x, Camera.main.transform.position.y), 10),
                    new Goal_CameraSize("Use the mouse scroll wheel to zoom the camera", Camera.main.orthographicSize, 15)
                },
                0,
                new List<string>
                {
                    "The Hull"
                }
         ));

        allObjectives.Add("The Hull",
            new Objective(
                "The Hull",
                new List<Goal>
                {
                    new Goal_Staff("Hire at least 1 Builder", 1, "Builder"),
                    new Goal_Hull("Build at least 30 Hull tiles", 30),
                    new Goal_Furniture("Build at least 1 Airlock", 1, "Airlock")
                },
                1000,
                new List<string>
                {
                    "Staff Basics",
                    "Interesting Public Transport",
                    "Fresh Air"
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
                new List<string>
                {
                    "Milestone I"
                    //,
                    //"Passenger Needs"
                },
                new List<string>
                {
                    "Interesting Public Transport",
                    "Fresh Air",
                }
         ));


        allObjectives.Add("Interesting Public Transport",
            new Objective(
                "Interesting Public Transport",
                new List<Goal>
                {
                    new Goal_Furniture("Build an Engine", 1, "Engine"),
                    new Goal_Furniture("Build at least 6 Fuel Tanks", 1, "Fuel Tank"),
                    new Goal_Furniture("Build the Ship Controls", 1, "Ship Controls"),
                    new Goal_Furniture("Build the Stairwell", 1, "Stairwell"),
                },
                2000,
                new List<string>
                {
                    "Milestone I"
                    //,           
                    //"Passenger Needs"
                },
                new List<string>
                {
                    "Staff Basics",
                    "Fresh Air",
                }
         ));

        allObjectives.Add("Fresh Air",
            new Objective(
                "Fresh Air",
                new List<Goal>
                {
                    new Goal_Furniture("Build an Oxygen Vent", 1, "Oxygen Vent"),
                    new Goal_Furniture("Build at least 2 Oxygen Tanks", 1, "Oxygen Tank"),
                },
                1000,
                new List<string>
                {
                    "Milestone I"
                    //,
                    //"Passenger Needs"
                },
                new List<string>
                {
                    "Staff Basics",
                    "Interesting Public Transport"
                }
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

        /*
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

    */

        allObjectives.Add("Milestone I",
           new Objective(
               "Milestone I",
               new List<Goal>
               {
                    new Goal_Passenger("Transport 10 passengers to Wellington in one journey", 10),
               },
               2000,
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



        // Initial Objective 
        AddObjective(allObjectives["Wellywood Cinematographer"]);
        
    }

    private void AddObjectivesForLoadGame()
    {

        allObjectives.Add("Wellywood Cinematographer",
            new Objective(
                "Wellywood Cinematographer",
                new List<Goal>
                {
                    new Goal_CameraPan("Hold the right mouse button and drag to pan the camera", new Vector2(Camera.main.transform.position.x, Camera.main.transform.position.y), 10),
                    new Goal_CameraSize("Use the mouse scroll wheel to zoom the camera", Camera.main.orthographicSize, 15)
                },
                0,
                new List<string>
                {
                    "Milestone I"
                }
         ));





        allObjectives.Add("Milestone I",
           new Objective(
               "Milestone I",
               new List<Goal>
               {
                    new Goal_Passenger("Transport 10 passengers to Wellington in one journey", 10),
               },
               2000,
               new List<string>
               {
                    "Milestone II"
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



        // Initial Objective 
        AddObjective(allObjectives["Wellywood Cinematographer"]);

    }

}
