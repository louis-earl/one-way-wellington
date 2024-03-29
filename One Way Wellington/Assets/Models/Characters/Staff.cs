﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Staff : Character
{
    // Staff needs 
    protected float energy;

    // Interface
    public static GameObject staffUIInstance;

    public float timeClearFailedJobs;
    

    protected override void Init()
    {
        // Call from superclass
        base.Init();

        // Setup from here onwards
        //energy = 100f;
        //health = 100f;
        spriteRenderer.transform.localPosition = new Vector3(0f, 0f, 0.25f);

        timeClearFailedJobs = Time.time;
    }

    public void OnMouseUpAsButton()
    {
        if (Passenger.passengerUIInstance != null) Destroy(Passenger.passengerUIInstance);
        if (staffUIInstance != null) Destroy(staffUIInstance);

        staffUIInstance = Instantiate(UserInterfaceController.Instance.staffUIPrefab);
        staffUIInstance.transform.position = new Vector3(currentX, currentY, 0);
        staffUIInstance.transform.localScale = Vector3.one / 500;
        staffUIInstance.GetComponent<StaffInterface>().staff = this;

        currentSelectedCharacter = this;
    }

    protected override void Refresh()
    {
        base.Refresh();


        // Clear failed jobs every now and then 
        if (Time.time > timeClearFailedJobs)
        {
            timeClearFailedJobs = Time.time + 5f;
            failedJobs = new List<Job>();
        }


        energy = Mathf.Clamp(energy - (1 * Time.deltaTime), 0, 100);

        if (energy <= 0)
        {
            // Enter a low power mode (staff never die from zero energy) 
            // spriteRenderer.color = Color.red;

            if (targetJob?.GetJobType() != "Use Charging Pad")
            {
                FindCharger();
            }
        }
        else if (GetHealth() < 100)
        {
            if (targetJob?.GetJobType() != "Use 3D Printer")
            {
                Find3DPrinter();
            }
        }
    }

    private void FindCharger()
    {
        DoJobAtFurnitureTile("Charging Pad", "Use Charging Pad", delegate () { UseChargingPad(); }, 5 );
    }

    private void Find3DPrinter()
    {
        DoJobAtFurnitureTile("3D Printer", "Use 3D Printer", delegate () { Use3DPrinter(); }, 10);
    }

    public void UseChargingPad()
    {
        energy = 100;
        spriteRenderer.color = Color.white;
    }

    public void Use3DPrinter()
    {
        SetHealth(100);
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

    

}
