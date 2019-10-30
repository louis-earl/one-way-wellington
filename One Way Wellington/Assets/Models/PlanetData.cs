using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlanetData
{
    public string planetName;

    public int planetGraphic;
    public PlanetData nextPlanet; // further from wellington 
    public float planetScale;
    public float planetPosX;
    public float planetPosY;

    public List<PotentialPassenger> potentialPassengers;

    public PlanetData(string planetName, int planetGraphic, PlanetData nextPlanet, float planetScale, float planetPosX, float planetPosY)
    {
        this.planetName = planetName;
        this.planetGraphic = planetGraphic;
        this.nextPlanet = nextPlanet;
        this.planetScale = planetScale;
        this.planetPosX = planetPosX;
        this.planetPosY = planetPosY;
    }


}
