﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staff : Character
{
    // Staff needs 
    protected float energy;

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

        energy = Mathf.Clamp(energy - (1 * Time.deltaTime), 0, 100);

        if (energy <= 0)
        {
            // Enter a low power mode (staff never die from zero energy) 
            spriteRenderer.color = Color.red;

            if (targetJob?.GetJobType() != "UseChargingPad")
            {
                FindCharger();
            }
        }
        else if (health < 100)
        {
            if (targetJob?.GetJobType() != "Use3DPrinter")
            {
                Find3DPrinter();
            }
        }
    }

    private void FindCharger()
    {
        DoJobAtFurnitureTile("Charging Pad", "UseChargingPad", delegate () { UseChargingPad(); }, 5 );
    }

    private void Find3DPrinter()
    {
        DoJobAtFurnitureTile("3D Printer", "Use3DPrinter", delegate () { Use3DPrinter(); }, 10);
    }

    public void UseChargingPad()
    {
        energy = 100;
        spriteRenderer.color = Color.white;
    }

    public void Use3DPrinter()
    {
        health = 100;
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
