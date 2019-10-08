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

    public int hair;
    public int skin;
    public int pants;
    public int decal;
    public int shades;
    public int shirt;
    public int shoes;

    // TODO: Reference to sprites used for passenger preview  

    public PotentialPassenger(string passengerFirstName, string passengerLastName, string occupation)
    {
        this.passengerFirstName = passengerFirstName;
        this.passengerLastName = passengerLastName;
        this.occupation = occupation;

        // Random generate appearance 
        hair = Random.Range(0, 9);
        skin = Random.Range(1, 7);
        pants = Random.Range(1, 2);
        decal = Random.Range(0, 4);
        shades = Random.Range(0, 5);
        shirt = Random.Range(1, 1);
        shoes = Random.Range(1, 2);
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
