using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{


    // Interface
    public static GameObject staffUIInstance;
    

    protected override void Init()
    {
        // Call from superclass
        base.Init();

        // Setup from here onwards
        health = 100f;

    }

    /*
     * Enemy interface not yet planned 
     * 
    public void OnMouseDown()
    {
        if (Passenger.passengerUIInstance != null) Destroy(Passenger.passengerUIInstance);
        if (staffUIInstance != null) Destroy(staffUIInstance);

        staffUIInstance = Instantiate(UserInterfaceController.Instance.staffUIPrefab);
        staffUIInstance.transform.position = new Vector3(currentX, currentY, 0);
        staffUIInstance.transform.localScale = Vector3.one / 500;
        staffUIInstance.GetComponent<CharacterInterface>().character = this;

    }
    */ 

    protected override void Refresh()
    {
        base.Refresh();

        
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
