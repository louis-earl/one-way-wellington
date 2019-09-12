using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotentialPassenger
{
    // Passenger data stored while they are at a planet 
    // Converted into a passenger upon boarding your ship

    private string passengerFirstName;
    private string passengerLastName;
    private string occupation;

    
    // TODO: Reference to sprites used for passenger preview  

    public PotentialPassenger(string passengerFirstName, string passengerLastName, string occupation)
    {
        this.passengerFirstName = passengerFirstName;
        this.passengerLastName = passengerLastName;
        this.occupation = occupation;
    }

    public string GetPassengerFirstName()
    {
        return passengerFirstName;
    }

    public string GetPassengerLastName()
    {
        return passengerLastName;
    }

    public string GetPassengerOccupation()
    {
        return occupation;
    }

}
