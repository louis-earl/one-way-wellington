using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staff : Character
{
    // Staff needs 
    protected float energy;
    protected float health;

    // Interface
    public static GameObject staffUIInstance;
    

    protected override void Init()
    {
        // Call from superclass
        base.Init();

        // Setup from here onwards
        //energy = 100f;
        //health = 100f;

    }

    public void OnMouseDown()
    {
        if (Passenger.passengerUIInstance != null) Destroy(Passenger.passengerUIInstance);
        if (staffUIInstance != null) Destroy(staffUIInstance);

        staffUIInstance = Instantiate(UserInterfaceController.Instance.staffUIPrefab);
        staffUIInstance.transform.position = new Vector3(currentX, currentY, 0);
        staffUIInstance.transform.localScale = Vector3.one / 500;
        staffUIInstance.GetComponent<CharacterInterface>().character = this;

    }

    protected override void Refresh()
    {
        base.Refresh();

        if (energy > 0)
        {
            energy -= 1 * Time.deltaTime;
            spriteRenderer.color = Color.white;
        }
        else
        {
            // Enter a low power mode (staff never die from zero energy) 
            spriteRenderer.color = Color.red;

            if (targetJob?.GetJobType() != "UseChargingPad")
            {
                FindCharger();
            }
        }
    }

    private void FindCharger()
    {
        // Does a charger exist?
        if (BuildModeController.Instance.furnitureTileOWWMap.ContainsKey("Charging Pad"))
        {
            // Loop through all chargers
            for (int i = 0; i < BuildModeController.Instance.furnitureTileOWWMap["Charging Pad"].Count; i++)
            {
                TileOWW tileCharger = BuildModeController.Instance.furnitureTileOWWMap["Charging Pad"][i];
                if (tileCharger.currentJobType == null)
                {
                    if (targetJob != null)
                    {

                        if (targetJob.GetJobType() != "UseChargingPad")
                        {
                            jobQueue.AddJob(targetJob);
                            targetJob = currentJob = null;
                        }
                        Action rechargeAction = delegate () { UseChargingPad(); };
                        targetJob = new Job(UseChargingPad, tileCharger, 5, "UseChargingPad");
                        return;

                    }
                    else
                    {
                        Action rechargeAction = delegate () { UseChargingPad(); };
                        targetJob = new Job(UseChargingPad, tileCharger, 5, "UseChargingPad");
                        return;
                    }
                }
            }
        }
    }

    public void UseChargingPad()
    {
        energy = 100;
        spriteRenderer.color = Color.white;
    }

    public float GetEnergy()
    {
        return energy;
    }

    public void SetEnergy(float energy)
    {
        this.energy = energy;

    }

    public float GetHealth()
    {
        return health;
    }

    public void SetHealth(float health)
    {
        this.health = health;
    }

}
