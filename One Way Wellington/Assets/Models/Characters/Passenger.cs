using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passenger : Character
{
    // Passenger information 
    public string occupation;
    public string homePlanet;
    
    protected int fare;

    // Sprite direction 
    private float lastPosX;

    // Appearance 
    protected int hairStyle;
    protected Color hairColor;
    public SpriteRenderer spriteRenderer_Hair;

    protected int skin;
    public SpriteRenderer spriteRenderer_Skin;

    protected int pantStyle;
    protected Color pantColor;
    public SpriteRenderer spriteRenderer_Pants;

    protected int decal;
    public SpriteRenderer spriteRenderer_Decal;

    protected int shades;
    public SpriteRenderer spriteRenderer_Shades;

    protected int shirtStyle;
    protected Color shirtColor;
    public SpriteRenderer spriteRenderer_Shirt;

    protected int shoeStyle;
    public SpriteRenderer spriteRenderer_Shoes;


    // Passenger needs 
    protected float nourishment;
    protected float oxygen;
    protected float oxygenUsageMultiplier;
    protected float bladder;
    protected float hygiene;
    protected float energy;


    // Notifications 
    bool hasWarnedOxygen;

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
        SetHealth(100);
        nourishment = 100f;
        oxygen = 100f;
        oxygenUsageMultiplier = UnityEngine.Random.Range(0.85f, 1.15f);
        bladder = 100f;
        hygiene = 100f;


        // Notifications
        hasWarnedOxygen = false;
    }


    public void OnMouseUpAsButton()
    {
        if (Staff.staffUIInstance != null) Destroy(Staff.staffUIInstance);
        if (passengerUIInstance != null) Destroy(passengerUIInstance);

        passengerUIInstance = Instantiate(UserInterfaceController.Instance.passengerUIPrefab);
        passengerUIInstance.transform.position = new Vector3(currentX, currentY, 0);
        passengerUIInstance.transform.localScale = Vector3.one / 500;
        passengerUIInstance.GetComponent<PassengerInterface>().InitPassengerUI(this);

    }

    public void SetPassengerInformation(string name, string occupation, string homePlanet, int fare, 
        int hairStyle, Color hairColor, 
        int skin, int decal, 
        int shirtStyle, Color shirtColor, 
        int pantStyle, Color pantColor, 
        int shoeStyle, int shades)
    {
        this.name = name;
        this.occupation = occupation;
        this.homePlanet = homePlanet;
        this.fare = fare;
        this.hairStyle = hairStyle;
        this.hairColor = hairColor;
        this.skin = skin;
        this.decal = decal;
        this.shirtStyle = shirtStyle;
        this.shirtColor = shirtColor;
        this.shirtColor = shirtColor;
        this.pantStyle = pantStyle;
        this.pantColor = pantColor;
        this.shoeStyle = shoeStyle;
        this.shades = shades;


        // Load sprite resources 
        spriteRenderer_Hair.sprite = Resources.Load<Sprite>("Images/Characters/Passengers/Hair/Hair" + hairStyle.ToString());
        spriteRenderer_Hair.color = hairColor;
        spriteRenderer_Skin.sprite = Resources.Load<Sprite>("Images/Characters/Passengers/Skin/Skin" + skin.ToString());
        spriteRenderer_Decal.sprite = Resources.Load<Sprite>("Images/Characters/Passengers/Decal/Decal" + decal.ToString());
        spriteRenderer_Shirt.sprite = Resources.Load<Sprite>("Images/Characters/Passengers/Shirt/Shirt" + shirtStyle.ToString());
        spriteRenderer_Shirt.color = shirtColor;
        spriteRenderer_Pants.sprite = Resources.Load<Sprite>("Images/Characters/Passengers/Pants/Pants" + pantStyle.ToString());
        spriteRenderer_Pants.color = pantColor;
        spriteRenderer_Shoes.sprite = Resources.Load<Sprite>("Images/Characters/Passengers/Shoes/Shoes" + shoeStyle.ToString());
        spriteRenderer_Shades.sprite = Resources.Load<Sprite>("Images/Characters/Passengers/Shades/Shades" + shades.ToString());
    }

    protected override void Refresh()
    {
        // Call from superclass
        base.Refresh();

        // Program from here onwards!

        // Sprite 
        if (lastPosX < transform.position.x)
        {
            transform.GetChild(0).transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.GetChild(0).transform.localScale = new Vector3(-1, 1, 1);
        }
        lastPosX = transform.position.x;

        // Needs
        energy = Mathf.Clamp(energy - (1 * Time.deltaTime), 0, 100);
        hygiene = Mathf.Clamp(hygiene - (1 * Time.deltaTime), 0, 100);
        bladder = Mathf.Clamp(bladder - (1 * Time.deltaTime), 0, 100);
        nourishment = Mathf.Clamp(nourishment - (1 * Time.deltaTime), 0, 100);
        oxygen = Mathf.Clamp(oxygen - (5 * Time.deltaTime * oxygenUsageMultiplier), 0, 100);


        // Restore oxygen
        TileOWW currentTile = WorldController.Instance.GetWorld().GetTileAt((int)currentX, (int)currentY);
        float oxygenDeficit = 100 - oxygen; // how much oxygen can be filled up 
        float maxRegen = currentTile.oxygenLevel;

        // Oxygen intake limited to 1/10th of a tiles oxygen level 
        if (oxygenDeficit < maxRegen)
        {
            oxygen += oxygenDeficit; // Just take as much as needed 
            currentTile.oxygenLevel -= oxygenDeficit;
        }
        else
        {
            oxygen += maxRegen; // Take as much as possible 
            currentTile.oxygenLevel -= maxRegen;
        }

        SolveNeeds();



        // Take oxygen damage 
        if (oxygen < 50f)
        {
            if (!hasWarnedOxygen)
            {
                hasWarnedOxygen = true;
                NotificationController.Instance.CreateNotification(name + " is suffocating! Quick, get some oxygen to them!", UrgencyLevel.High, true, true, new List<string> { "Go to " + name },
                    new List<Action> { delegate () { StartCoroutine(InputController.Instance.MoveCameraTo(transform.position.x, transform.position.y)); } });
            }
            TakeDamage(((-0.133333f * oxygen) + 6.666666f) * Time.deltaTime);
        }
        else if (oxygen > 98)
        {
            // Reset warning notification 
            hasWarnedOxygen = false;
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
