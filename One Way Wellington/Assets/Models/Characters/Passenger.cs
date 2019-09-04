using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passenger : Character
{
    // Passenger information 
    protected string passengerName;
    protected string career;
    protected int fare; 
    

    // Passenger needs 
    protected float nourishment;
    protected float oxygen;
    protected float bladder;
    protected float hygiene;
    protected float energy;

    // Interface
    public static GameObject passengerUIInstance;

    protected override void Init()
    {
        // Call from superclass
        base.Init();

        // Setup from here onwards!
        jobQueue = JobQueueController.PassengersJobQueue;

        // Default needs 
        energy = 100f;
        health = 100f;
        nourishment = 100f;
        oxygen = 100f;
        bladder = 100f;
        hygiene = 100f;

    }


    public void OnMouseDown()
    {
        if (Staff.staffUIInstance != null) Destroy(Staff.staffUIInstance);
        if (passengerUIInstance != null) Destroy(passengerUIInstance);

        passengerUIInstance = Instantiate(UserInterfaceController.Instance.passengerUIPrefab);
        passengerUIInstance.transform.position = new Vector3(currentX, currentY, 0);
        passengerUIInstance.transform.localScale = Vector3.one / 500;
        passengerUIInstance.GetComponent<CharacterInterface>().character = this;

    }

    public void SetPassengerInformation(string name, string career, int fare)
    {
        this.name = name;
        this.career = career;
        this.fare = fare;
    }

    protected override void Refresh()
    {
        // Call from superclass
        base.Refresh();

        // Program from here onwards!

        // Needs
        energy = Mathf.Clamp(energy - (1 * Time.deltaTime), 0, 100);
        hygiene = Mathf.Clamp(hygiene - (1 * Time.deltaTime), 0, 100);
        bladder = Mathf.Clamp(bladder - (1 * Time.deltaTime), 0, 100);
        nourishment = Mathf.Clamp(nourishment - (1 * Time.deltaTime), 0, 100);
        oxygen = Mathf.Clamp(oxygen - (0.02f * Time.deltaTime), 0, 1);


        // Restore oxygen
        TileOWW currentTile = WorldController.Instance.GetWorld().GetTileAt((int)currentX, (int)currentY);
        float oxygenDeficit = 1 - oxygen;
        float maxRegen = currentTile.oxygenLevel / 10;

        // Oxygen intake limited to 1/10th of a tiles oxygen level 
        if (oxygenDeficit < maxRegen)
        {
            oxygen += oxygenDeficit;
            currentTile.oxygenLevel -= oxygenDeficit;
        }
        else
        {
            oxygen += maxRegen;
            currentTile.oxygenLevel -= maxRegen;
        }

        SolveNeeds();

        // Take oxygen damage 
        if (oxygen < 0.5f)
        {
            Debug.Log("Oxygen 0");
            health -= (-2 * oxygen) + 1;
        }


        // Idle 
        if (targetJob == null)
        {
            targetJob = jobQueue.GetNextJob(new Vector2(currentX, currentY), failedJobs);
            if (targetJob == null)
            {
                // We are idle
                // Random movement 
                if (!navMeshAgent.hasPath) navMeshAgent.SetDestination(new Vector3(UnityEngine.Random.Range(0, 100), UnityEngine.Random.Range(0, 100), 0));
            }
        }
    }




    
    protected void SolveNeeds()
    {
        // Ordered needs by priority

        // TODO: Find room with oxygen?
        
        if (bladder == 0)
        {
            if (SetJobAtFurnitureTile("Toilet Stall", "Use Toilet Stall", 5, delegate () { UseToiletStall(); })) return;
        }

        if (nourishment == 0)
        {
            if (SetJobAtFurnitureTile("Cafe", "Use Cafe", 5, delegate () { UseCafe(); })) return;
        }

        if (hygiene == 0)
        {
            if (SetJobAtFurnitureTile("Shower Stall", "Use Shower Stall", 10, delegate () { UseShowerStall(); })) return;
        }

        if (energy == 0)
        {
            if (SetJobAtFurnitureTile("Bed", "Use Bed", 20, delegate () { UseBed(); })) return;
        }
    }
    

    public void UseToiletStall()
    {
        bladder = 100;
    }

    public void UseCafe()
    {
        nourishment = 100;
    }

    public void UseShowerStall()
    {
        hygiene = 100;
    }

    public void UseBed()
    {
        energy = 100;
    }

    public int GetPassengerFare()
    {
        return fare;
    }


    protected bool SetJobAtFurnitureTile(string furnitureType, string jobType, float jobTime, Action action)
    {
        if (BuildModeController.Instance.furnitureTileOWWMap.ContainsKey(furnitureType))
        {
            // Loop through all 
            for (int i = 0; i < BuildModeController.Instance.furnitureTileOWWMap[furnitureType].Count; i++)
            {
                TileOWW tileCharger = BuildModeController.Instance.furnitureTileOWWMap[furnitureType][i];
                if (tileCharger.currentJobType == null)
                {

                    // Clear existing job
                    if (targetJob?.GetJobType() != jobType)
                    {
                        targetJob = currentJob = null;
                    }

                    targetJob = new Job(action, tileCharger, jobTime, jobType);
                    return true;
                }
            }
        }
        return false;
    }

}
