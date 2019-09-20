using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    private string planetName;

    public int planetGraphic;
    private Sprite surface;
    private Sprite clouds;
    public Planet nextPlanet; // further from earth 
    public Planet previousPlanet; // closer to earth 
    public float planetScale;
    public Vector2 planetCoordinates;

    public static GameObject planetUI;

    public GameObject linkLine;

    private List<PotentialPassenger> potentialPassengers;
    public List<PotentialPassenger> selectedPassengers;

    private void Start()
    {
        potentialPassengers = new List<PotentialPassenger>();
        selectedPassengers = new List<PotentialPassenger>();
        GeneratePotentialPassengers();
    }

    private void OnMouseUpAsButton()
    {
        if (planetName != "Earth")
        {
            if (planetUI != null) Destroy(planetUI);
            planetUI = Instantiate(JourneyController.Instance.PlanetUIPrefab);
            planetUI.transform.position = new Vector3(planetCoordinates.x, planetCoordinates.y, 1020);
            planetUI.transform.localScale = Vector3.one / 500;
            planetUI.GetComponent<PlanetInterface>().SetPlanet(this, false);
            planetUI.GetComponent<PlanetInterface>().button_ContinueJourney.gameObject.SetActive(false);
            planetUI.GetComponent<PlanetInterface>().SetMoveWithPlanet(true);
        }
    }

    public Vector2 GetPlanetCoordinates()
    {
        return planetCoordinates;
    }

    public bool SetNextPlanet(Planet nextPlanet, Planet previousPlanet)
    {
        this.previousPlanet = previousPlanet;
        Debug.Log(nextPlanet.name + " <- " + planetName + " <- " + this.previousPlanet.name);
        if (nextPlanet != this)
        {
            if (this.nextPlanet == null) this.nextPlanet = nextPlanet;
            else return this.nextPlanet.SetNextPlanet(nextPlanet, this);
            return true;
        }
        return false;
    }

    public void ClearLinkedPlanets()
    {
        nextPlanet = null;
        previousPlanet = null;
    }

    public Planet GetNextPlanet()
    {
        if (nextPlanet == null) return this;
        else return nextPlanet.GetNextPlanet();
    }

    public Planet GetPreviousPlanet()
    {
        return previousPlanet;
    }

    public Sprite GetSurface()
    {
        return surface;
    }

    public void SetSurface(Sprite surface)
    {
        this.surface = surface;
    }

    public Sprite GetClouds()
    {
        return clouds;
    }

    public void SetClouds(Sprite clouds)
    {
        this.clouds = clouds;
    }

    public string GetPlanetName()
    {
        return planetName;
    }

    public void SetPlanetName(string name)
    {
        this.planetName = name;
    }

    public void RemovePotentialPassenger(PotentialPassenger potentialPassenger) 
    {
        if (potentialPassengers.Contains(potentialPassenger))
        {
            potentialPassengers.Remove(potentialPassenger);
        }

    }

    public void GeneratePotentialPassengers()
    {
        potentialPassengers.Clear();
        int n = (int) Random.Range(0, 10);
        for (int i = 0; i < n; i++)
        {

            PassengerNames.FirstNames passengerFirstName = (PassengerNames.FirstNames)Random.Range(0, (int)PassengerNames.FirstNames.COUNT);
            PassengerNames.LastNames passengerLastName = (PassengerNames.LastNames)Random.Range(0, (int)PassengerNames.LastNames.COUNT);
            potentialPassengers.Add(new PotentialPassenger(passengerFirstName.ToString(), passengerLastName.ToString(), "Student"));
        }


    }

    public List<PotentialPassenger> GetPotentialPassengers()
    {
        return potentialPassengers;
    }

    public PlanetData ToPlanetData()
    {
        PlanetData nextPlanet = null;

        if (this.nextPlanet != null)
        {
            Debug.Log(planetName + " -> " + this.nextPlanet.planetName);
            nextPlanet = this.nextPlanet.ToPlanetData();
            
        }

        return new PlanetData(planetName, planetGraphic, nextPlanet, planetScale, planetCoordinates.x, planetCoordinates.y);
    }

}
