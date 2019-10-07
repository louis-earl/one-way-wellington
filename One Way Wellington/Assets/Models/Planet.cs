using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Planet : MonoBehaviour
{
    private string planetName;

    private int planetGraphicSuffix;
    public SpriteRenderer surface;
    public SpriteRenderer clouds;
    public Planet nextPlanet; // further from earth 
    public Planet previousPlanet; // closer to earth 
    public float planetScale;
    public Vector2 planetCoordinates;

    public static GameObject planetUI;

    public GameObject linkLine;

    private List<PotentialPassenger> potentialPassengers;
    public List<PotentialPassenger> selectedPassengers;

    public bool allowClick;

    private void Start()
    {
        potentialPassengers = new List<PotentialPassenger>();
        selectedPassengers = new List<PotentialPassenger>();
        GeneratePotentialPassengers();
        allowClick = true;
    }

    private void OnMouseUpAsButton()
    {
        if (allowClick)
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
            StartCoroutine(InputController.Instance.MoveCameraTo(transform.position.x, transform.position.y));
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
            if (this.nextPlanet == null)
            {
                Debug.Log("setting " + this + " next planet to " + nextPlanet + " AND setting " + this.nextPlanet + " sprevious planet to " + this);
                this.nextPlanet = nextPlanet;
                this.nextPlanet.previousPlanet = this;
            }
            else
            {
                if (nextPlanet.previousPlanet == null)
                {
                    Debug.Log("setting " + nextPlanet + " sprevious planet to " + GetNextPlanet());
                    nextPlanet.previousPlanet = GetNextPlanet();
                }
                return this.nextPlanet.SetNextPlanet(nextPlanet, this);
            }
            return true;
        }
        return false;
    }

    public void ClearLinkedPlanets()
    {
        nextPlanet = null;
        previousPlanet = null;
    }

    public Planet GetNextPlanet(bool recurse = true)
    {
        if (nextPlanet == null) return this;
        if (recurse) return nextPlanet.GetNextPlanet(recurse);
        else return nextPlanet;
    }


    public void SetPlanetGraphicSuffix (int planetGraphicSuffix)
    {
        this.planetGraphicSuffix = planetGraphicSuffix;
    }

    public Planet GetPreviousPlanet()
    {
        return previousPlanet;
    }

    public Sprite GetSurface()
    {
        return surface.sprite;
    }

    public void SetSurface(Sprite surface)
    {
        this.surface.sprite = surface;
    }

    public Sprite GetClouds()
    {
        return clouds.sprite;
    }

    public void SetClouds(Sprite clouds)
    {
        this.clouds.sprite = clouds;
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
            //Debug.Log(planetName + " -> " + this.nextPlanet.planetName);
            nextPlanet = this.nextPlanet.ToPlanetData();
            
        }

        return new PlanetData(planetName, planetGraphicSuffix, nextPlanet, planetScale, planetCoordinates.x, planetCoordinates.y);
    }

}
