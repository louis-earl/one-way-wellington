﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staff : Character
{
    // Staff needs 
    protected float energy;
    protected float health;

    

    protected override void Init()
    {
        // Call from superclass
        base.Init();

        // Setup from here onwards
        //energy = 100f;
        //health = 100f;

        

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
                                JobQueueController.BuildersJobQueue.AddJob(targetJob);
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