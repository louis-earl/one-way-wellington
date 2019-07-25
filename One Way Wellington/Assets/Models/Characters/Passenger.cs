using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passenger : Character
{
    // Passenger information 
    protected string name;
    protected string career;
    protected int fare; 
    

    // Passenger needs 
    protected float nourishment;
    protected float oxygen;
    protected float bladder;
    protected float hygiene;
    protected float energy;
    protected float health;

    // Interface
    private static GameObject passengerUIInstance;

    protected override void Init()
    {
        // Call from superclass
        base.Init();

        // Setup from here onwards!
        energy = 100f;
        health = 100f;
        nourishment = 100f;
        oxygen = 100f;
        bladder = 100f;
        hygiene = 100f;

    }


    public void OnMouseDown()
    {
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
        oxygen = Mathf.Clamp(oxygen - (1 * Time.deltaTime), 0, 100);

        // Order needs by priority

        if (bladder == 0)
        {
            Debug.Log("bladder 0");
            if (BuildModeController.Instance.furnitureTileOWWMap.ContainsKey("Toilet Stall"))
            {
                // Loop through all 
                for (int i = 0; i < BuildModeController.Instance.furnitureTileOWWMap["Toilet Stall"].Count; i++)
                {
                    TileOWW tileCharger = BuildModeController.Instance.furnitureTileOWWMap["Toilet Stall"][i];
                    if (tileCharger.currentJobType == null)
                    {
                        if (targetJob != null)
                        {

                            if (targetJob.GetJobType() != "UseToiletStall")
                            {
                                JobQueueController.BuildersJobQueue.AddJob(targetJob);
                                targetJob = currentJob = null;
                            }
                            Action rechargeAction = delegate () { UseToiletStall(); };
                            targetJob = new Job(UseToiletStall, tileCharger, 5, "UseToiletStall");
                            return;

                        }
                        else
                        {
                            Action rechargeAction = delegate () { UseToiletStall(); };
                            targetJob = new Job(UseToiletStall, tileCharger, 5, "UseToiletStall");
                            return;
                        }
                    }
                }
            }
        }

        else if (hygiene == 0)
        {
            Debug.Log("hygiene 0");
            if (BuildModeController.Instance.furnitureTileOWWMap.ContainsKey("Shower Stall"))
            {
                // Loop through all 
                for (int i = 0; i < BuildModeController.Instance.furnitureTileOWWMap["Shower Stall"].Count; i++)
                {
                    TileOWW tileCharger = BuildModeController.Instance.furnitureTileOWWMap["Shower Stall"][i];
                    if (tileCharger.currentJobType == null)
                    {
                        if (targetJob != null)
                        {

                            if (targetJob.GetJobType() != "UseShowerStall")
                            {
                                JobQueueController.BuildersJobQueue.AddJob(targetJob);
                                targetJob = currentJob = null;
                            }
                            Action rechargeAction = delegate () { UseShowerStall(); };
                            targetJob = new Job(UseShowerStall, tileCharger, 5, "UseShowerStall");
                            return;

                        }
                        else
                        {
                            Action rechargeAction = delegate () { UseShowerStall(); };
                            targetJob = new Job(UseShowerStall, tileCharger, 5, "UseShowerStall");
                            return;
                        }
                    }
                }
            }
        }



        if (targetJob == null)
        {
            targetJob = JobQueueController.PassengersJobQueue.GetNextJob(new Vector2(currentX, currentY), failedJobs);
            if (targetJob == null)
            {
                // We are idle
                // Random movement 
                if (!navMeshAgent.hasPath) navMeshAgent.SetDestination(new Vector3(UnityEngine.Random.Range(0, 100), UnityEngine.Random.Range(0, 100), 0));
            }
        }
    }


    public void UseToiletStall()
    {
        bladder = 100;
    }

    public void UseShowerStall()
    {
        hygiene = 100;
    }

    public int GetPassengerFare()
    {
        return fare;
    }
}
