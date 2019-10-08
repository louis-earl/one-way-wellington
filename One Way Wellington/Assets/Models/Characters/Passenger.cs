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

    // Appearance 
    protected int hair;
    public SpriteRenderer spriteRenderer_Hair;

    protected int skin;
    public SpriteRenderer spriteRenderer_Skin;

    protected int pants;
    public SpriteRenderer spriteRenderer_Pants;

    protected int decal;
    public SpriteRenderer spriteRenderer_Decal;

    protected int shades;
    public SpriteRenderer spriteRenderer_Shades;

    protected int shirt;
    public SpriteRenderer spriteRenderer_Shirt;

    protected int shoes;
    public SpriteRenderer spriteRenderer_Shoes;


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


    public void OnMouseUpAsButton()
    {
        if (Staff.staffUIInstance != null) Destroy(Staff.staffUIInstance);
        if (passengerUIInstance != null) Destroy(passengerUIInstance);

        passengerUIInstance = Instantiate(UserInterfaceController.Instance.passengerUIPrefab);
        passengerUIInstance.transform.position = new Vector3(currentX, currentY, 0);
        passengerUIInstance.transform.localScale = Vector3.one / 500;
        passengerUIInstance.GetComponent<PassengerInterface>().passenger = this;

    }

    public void SetPassengerInformation(string name, string career, int fare, int hair, int skin, int decal, int shirt, int pants, int shoes, int shades)
    {
        this.name = name;
        this.career = career;
        this.fare = fare;
        this.hair = hair;
        this.skin = skin;
        this.decal = decal;
        this.shirt = shirt;
        this.pants = pants;
        this.shoes = shoes;
        this.shades = shades;


        // Load sprite resources 
        spriteRenderer_Hair.sprite = Resources.Load<Sprite>("Images/Characters/Passengers/Hair/Hair" + hair.ToString());
        spriteRenderer_Skin.sprite = Resources.Load<Sprite>("Images/Characters/Passengers/Skin/Skin" + skin.ToString());
        spriteRenderer_Decal.sprite = Resources.Load<Sprite>("Images/Characters/Passengers/Decal/Decal" + decal.ToString());
        spriteRenderer_Shirt.sprite = Resources.Load<Sprite>("Images/Characters/Passengers/Shirt/Shirt" + shirt.ToString());
        spriteRenderer_Pants.sprite = Resources.Load<Sprite>("Images/Characters/Passengers/Pants/Pants" + pants.ToString());
        spriteRenderer_Shoes.sprite = Resources.Load<Sprite>("Images/Characters/Passengers/Shoes/Shoes" + shoes.ToString());
        spriteRenderer_Shades.sprite = Resources.Load<Sprite>("Images/Characters/Passengers/Shades/Shades" + shades.ToString());
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
                targetJob = new Job(delegate () { }, WorldController.Instance.GetWorld().GetRandomHullTile(), 1f, "Wander", JobPriority.Low, tileExcludeOtherJobs: false);
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

    public float GetNourishment()
    {
        return nourishment;
    }

    public float GetEnergy()
    {
        return energy;
    }

    public float GetBladder()
    {
        return bladder;
    }

    public float GetOxygen()
    {
        return oxygen;
    }

    public float GetHygiene()
    {
        return hygiene;
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

                    targetJob = new Job(action, tileCharger, jobTime, jobType, JobPriority.Medium);
                    return true;
                }
            }
        }
        return false;
    }

}
