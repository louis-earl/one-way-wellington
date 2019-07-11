using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SaveFile
{
    public int bankBalance;
    public float timeOWW;
    public World world;

    // Staff Data 
    // On a per index basis for the following fields: 
    public string[] staffTypes;
    public float[] staffPosX;
    public float[] staffPosY;
    public float[] staffEnergy;
    public float[] staffHealth;

    // Jobs 
    public List<JobSerializable> buildersJobQueue;
    public List<JobSerializable> guardsJobQueue;
    public List<JobSerializable> passengersJobQueue;

    // TODO: Passenger Data 


    // TODO: Planets
    public List<PlanetData> planets;

    // TODO: Journey
    public float shipPosX;
    public float shipPosY;
}