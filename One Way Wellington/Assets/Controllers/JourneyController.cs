﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JourneyController : MonoBehaviour
{

    public static JourneyController Instance;

    public Planet planetWellington;
    public Planet lastPlanetVisit;
    public Planet nextPlanetVisit;
    public Planet currentPlanetVisit;
    public bool isAtOriginPlanet;
    public Vector2 shipCoordinates = Vector2.zero;
    public float shipSpeedMax; 
    public float shipSpeedCurrent;
    public bool isJourneyEditMode; // TODO: set ONLY after Wellington arrival

    public GameObject distanceLinePrefab;
    public GameObject distanceLineTeleportPrefab;
    public GameObject distanceLineTeleportInstance;
    public GameObject progressMarkerPrefab;
    public GameObject PlanetUIPrefab;

    public GameObject distanceRingPrefab;
    public GameObject distanceRingInstance;

    public List<GameObject> currentPassengers;

    public float fuelRemaining;
    public float fuelUsed;
    public static readonly float FUEL_COST = 2.30f;

    public GameObject passengerParent;

    public int passengersDeliveredLastJourney;
    public int passengersDeliveredTotal;

    // Fuel UI
    public GameObject panel_FuelCost;
    public TextMeshProUGUI text_FuelCost;

    // Location UI 
    public TextMeshProUGUI text_CurrentLocation;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null) Instance = this;
        progressMarkerPrefab = Instantiate(progressMarkerPrefab);
        progressMarkerPrefab.transform.position = new Vector3(0, 0, 1020);

        currentPassengers = new List<GameObject>();
        panel_FuelCost.SetActive(false);

        text_CurrentLocation.text = "Stationed: Planet Wellington";
    }

    // Update is called once per frame
    void Update()
    {
        
        if (nextPlanetVisit != null && !isJourneyEditMode)
        {
            // If ship is at planet location 
            if (Vector2.Distance(shipCoordinates, nextPlanetVisit.GetPlanetCoordinates()) < 0.1)
            {
                // We have arrived at a planet! 
                currentPlanetVisit = nextPlanetVisit;

                isJourneyEditMode = true;
                StartCoroutine(TransitionController.Instance.TransitionArrival(nextPlanetVisit));
                currentPlanetVisit = nextPlanetVisit;

                // set text
                text_CurrentLocation.text = "Arrived: Planet " + currentPlanetVisit.GetPlanetName();
            }
            else
            {
                // Move towards the planet 

                shipCoordinates = Vector2.MoveTowards(shipCoordinates, nextPlanetVisit.GetPlanetCoordinates(), (shipSpeedCurrent / 5) * Time.deltaTime);
                if (shipCoordinates.x < 100000) // not infinity
                {
                    progressMarkerPrefab.transform.position = new Vector3(shipCoordinates.x, shipCoordinates.y, 1020);
                }
                currentPlanetVisit = null;
            }
        }
    }

    public void AddNextPlanet(Planet planet)
    {
        Debug.Log("Adding next planet to journey: " + planet.name);

        if (isJourneyEditMode)
        {
            
            Planet lastPlanet = planetWellington.GetNextPlanet();
            if (fuelRemaining > Vector3.Distance(planet.transform.position, lastPlanet.transform.position))
            {
                if (planetWellington.SetNextPlanet(planet, planetWellington)) // If successful 
                {
                    lastPlanetVisit = planet;
                    nextPlanetVisit = lastPlanet;

                    // Switch UI buttons
                    UserInterfaceController.Instance.panel_LaunchJourney.SetActive(true);
                    UserInterfaceController.Instance.panel_GoToShip.SetActive(false);

                    // Draw new distance line between planets 
                    GameObject line = Instantiate(distanceLinePrefab);
                    line.transform.position = Vector3.Lerp(planet.transform.position, lastPlanet.transform.position, 0.5f);
                    line.transform.localScale = new Vector3(12, 12, 1);
                    line.GetComponent<SpriteRenderer>().size = new Vector2(Vector3.Distance(planet.transform.position, lastPlanet.transform.position)/12, 0.31f);
                    line.transform.LookAt(planet.transform.position);
                    line.transform.Rotate(new Vector3(0, 90, 0));
                    line.transform.parent = TransitionController.Instance.mapGO.transform;
                    line.GetComponentInChildren<Canvas>().GetComponentInChildren<TextMeshProUGUI>().text = string.Format("{0:C}", (int)Vector3.Distance(planet.transform.position, lastPlanet.transform.position) * FUEL_COST);

                    // Rotate fuel cost text 
                    Debug.Log("line.transform.localRotation.eulerAngles.y): " + line.transform.localRotation.eulerAngles.y);
                    if (Mathf.Abs(line.transform.localRotation.eulerAngles.y % 360) >= 90)
                    {
                        line.GetComponentInChildren<Canvas>().GetComponentInChildren<RectTransform>().localRotation = Quaternion.Euler(new Vector3(0, -180, 0));
                    }

                    nextPlanetVisit.linkLine = line;


                    // Update existing teleport line 
                    if (distanceLineTeleportInstance == null) distanceLineTeleportInstance = Instantiate(distanceLineTeleportPrefab);
                    distanceLineTeleportInstance.transform.position = Vector3.Lerp(planetWellington.transform.position, lastPlanetVisit.transform.position, 0.5f);
                    distanceLineTeleportInstance.transform.localScale = new Vector3(12, 12, 1);
                    distanceLineTeleportInstance.GetComponent<SpriteRenderer>().size = new Vector2(Vector3.Distance(planetWellington.transform.position, lastPlanetVisit.transform.position) / 12, 0.31f);
                    distanceLineTeleportInstance.transform.LookAt(planetWellington.transform.position);
                    distanceLineTeleportInstance.transform.Rotate(new Vector3(0, 90, 0));
                    distanceLineTeleportInstance.transform.parent = TransitionController.Instance.mapGO.transform;

                    // Rotate fuel cost text 
                    if (Mathf.Abs(distanceLineTeleportInstance.transform.localRotation.eulerAngles.y % 360) >= 90)
                    {
                        distanceLineTeleportInstance.GetComponentInChildren<Canvas>().GetComponentInChildren<RectTransform>().localRotation = Quaternion.Euler(new Vector3(0, -180, 0));
                    }

                    // Offset if journey is only 1 planet
                    if (lastPlanet == planetWellington)
                    {
                        line.transform.Translate(Vector3.up * 2, line.transform);
                        distanceLineTeleportInstance.transform.Translate(Vector3.down * 2, distanceLineTeleportInstance.transform);
                    }
                    else
                    {
                        planetWellington.GetComponent<Planet>().linkLine.transform.position = Vector3.Lerp(planetWellington.GetNextPlanet(false).transform.position, planetWellington.transform.position, 0.5f);

                    }


                    // Fuel usage
                    fuelRemaining -= Vector3.Distance(planet.transform.position, lastPlanet.transform.position);
                    fuelUsed += Vector3.Distance(planet.transform.position, lastPlanet.transform.position);
                    text_FuelCost.text = string.Format("{0:C}", (int) fuelUsed * FUEL_COST);

                    // Strange Unity glitch requires UI force update
                    Canvas.ForceUpdateCanvases();
                    text_FuelCost.transform.parent.GetComponent<ContentSizeFitter>().enabled = false;
                    text_FuelCost.transform.parent.GetComponent<ContentSizeFitter>().enabled = true;

                    // Draw circle around planet 
                    Destroy(distanceRingInstance);
                    distanceRingInstance = Instantiate(distanceRingPrefab);
                    distanceRingInstance.transform.position = new Vector3(planet.transform.position.x, planet.transform.position.y, 1015);
                    if (BuildModeController.Instance.furnitureTileOWWMap.ContainsKey("Fuel Tank"))
                    {
                        distanceRingInstance.GetComponent<DistanceRing>().xradius = fuelRemaining;
                        distanceRingInstance.GetComponent<DistanceRing>().yradius = fuelRemaining;
                        distanceRingInstance.GetComponent<DistanceRing>().CreatePoints();
                    }
                }
                else
                {
                    Debug.LogWarning("Failed to add planet to journey");
                }
            }
            else
            {
                NotificationController.Instance.CreateNotification("Add more fuel tanks to your ship before travelling to " + planet.name, UrgencyLevel.Medium, true, false, new List<string> { "Return to ship" }, new List<System.Action> { delegate () { TransitionController.Instance.StartTransitionToMain(); } });
            }
        }
        else
        {
            Debug.Log("Not in journey edit mode");
        }
    }

    public void OnMapMode()
    {
        if (BuildModeController.Instance.furnitureTileOWWMap.ContainsKey("Fuel Tank"))
        {
            fuelRemaining = BuildModeController.Instance.furnitureTileOWWMap["Fuel Tank"].Count * 10;
            // Draw circle around planet 
            Destroy(distanceRingInstance);
            distanceRingInstance = Instantiate(distanceRingPrefab);
            distanceRingInstance.transform.position = new Vector3(planetWellington.transform.position.x, planetWellington.transform.position.y, 1015);

            distanceRingInstance.GetComponent<DistanceRing>().xradius = fuelRemaining;
            distanceRingInstance.GetComponent<DistanceRing>().yradius = fuelRemaining;
            distanceRingInstance.GetComponent<DistanceRing>().CreatePoints();
        }
        // Fuel cost UI 
        if (isJourneyEditMode)
        {
            panel_FuelCost.SetActive(true);
            text_FuelCost.text = string.Format("{0:C}", 0);

            // Strange Unity glitch requires UI force update
            Canvas.ForceUpdateCanvases();
            text_FuelCost.transform.parent.GetComponent<ContentSizeFitter>().enabled = false;
            text_FuelCost.transform.parent.GetComponent<ContentSizeFitter>().enabled = true;
            
        }
    }

    public float GetShipSpeedCurrent()
    {
        return shipSpeedCurrent;
    }

    // Used by UI button 
    public void LaunchJourney()
    {
        if (nextPlanetVisit != null) 
        {
            Destroy(distanceRingInstance);
            shipCoordinates = planetWellington.GetNextPlanet().GetPlanetCoordinates();
            //shipSpeedCurrent = shipSpeedMax;
            //TransitionController.Instance.StartTransitionToMain();
            StartCoroutine(TransitionController.Instance.TransitionWormhole(lastPlanetVisit));
            isJourneyEditMode = false;
            isAtOriginPlanet = true;
            panel_FuelCost.SetActive(false);

            // Charge for fuel 
            CurrencyController.Instance.DeductBankBalance((int)(fuelUsed * FUEL_COST));
            fuelUsed = 0;

            // Refill oxygen
            OxygenController.Instance.RestockOxygen();

            UserInterfaceController.Instance.panel_LaunchJourney.SetActive(false);

            // set text
            text_CurrentLocation.text = "Traveling: Planet " + lastPlanetVisit.GetPlanetName();

        }
        else
        {
            // Can't launch without setting a destination!! 
        }
    }

    public Vector2 GetShipCoordinates()
    {
        return shipCoordinates;
    }

    public Vector2 GetNextPlanetVist()
    {
        if (nextPlanetVisit != null) return nextPlanetVisit.transform.position;
        else return Vector2.zero;
    }

    // Called from UI Button after arrival
    public void Land()
    {
        if (TransitionController.Instance.isMapMode) StartCoroutine(TransitionController.Instance.TransitionLandingWithoutZoom());
        else StartCoroutine(TransitionController.Instance.TransitionLanding());
        UserInterfaceController.Instance.panel_LandShip.SetActive(false);
    }

    // Called when the above coroutine is completed
    public void OnLandComplete()
    {
       
        if (currentPlanetVisit != null)
        {
            if (currentPlanetVisit == planetWellington)
            {
                EndJourney();
            }
            else SpawnPlanetInterfaceGO();

        }
        else
        {
            currentPlanetVisit = lastPlanetVisit;
            SpawnPlanetInterfaceGO();
        }
    }

    // For selecting passengers to board
    private void SpawnPlanetInterfaceGO()
    {
        GameObject planetInterfaceGO = Instantiate(PlanetUIPrefab);
        planetInterfaceGO.GetComponent<PlanetInterface>().SetMoveWithPlanet(false);

        planetInterfaceGO.transform.SetParent(UserInterfaceController.Instance.canvas_Main.transform);
        planetInterfaceGO.transform.localPosition = new Vector3(0, 50, 0);
        planetInterfaceGO.transform.localScale = Vector3.one;
        planetInterfaceGO.GetComponent<PlanetInterface>().SetPlanet(currentPlanetVisit, true);
        planetInterfaceGO.GetComponent<PlanetInterface>().button_AddToJourney.gameObject.SetActive(false);

        InputController.Instance.cameraZoomEnabled = false;
        planetInterfaceGO.GetComponent<RectTransform>().sizeDelta = new Vector2(600, 600);

    }

    // After landing and boarding passengers 
    public void ContinueJourney(Planet planet)
    {

        // Return planet to map
        GameObject planetLandGO = planet.gameObject;
        TransitionController.Instance.mapGO.SetActive(true);
        planetLandGO.transform.parent = TransitionController.Instance.mapGO.transform;
        planetLandGO.transform.localScale = new Vector3(planetLandGO.GetComponent<Planet>().planetScale, planetLandGO.GetComponent<Planet>().planetScale, 1);
        planetLandGO.transform.localPosition = new Vector3(planetLandGO.GetComponent<Planet>().GetPlanetCoordinates().x, planetLandGO.GetComponent<Planet>().GetPlanetCoordinates().y, 0);
        TransitionController.Instance.mapGO.SetActive(false);
        planet.allowClick = true;

        Debug.Log("Put back planet: " + planet.name);

        // Set the next destination
        UserInterfaceController.Instance.ShowMainUI();

        InputController.Instance.cameraZoomEnabled = true;


        lastPlanetVisit = nextPlanetVisit;
        nextPlanetVisit = nextPlanetVisit.GetPreviousPlanet();
        lastPlanetVisit.ClearLinkedPlanets();
        Destroy(lastPlanetVisit.linkLine);


        shipSpeedCurrent = shipSpeedMax;
        isJourneyEditMode = false;
        StartCoroutine(TransitionController.Instance.TransitionDeparture());


        // set text
        text_CurrentLocation.text = "Traveling: Planet " + nextPlanetVisit.GetPlanetName();
    }

    public void EndJourney()
    {
        // put planet wellington back
        // Return planet to map
        GameObject planetLandGO = planetWellington.gameObject;
        TransitionController.Instance.mapGO.SetActive(true);
        planetLandGO.transform.parent = TransitionController.Instance.mapGO.transform;
        planetLandGO.transform.localScale = new Vector3(planetLandGO.GetComponent<Planet>().planetScale, planetLandGO.GetComponent<Planet>().planetScale, 1);
        planetLandGO.transform.localPosition = new Vector3(planetLandGO.GetComponent<Planet>().GetPlanetCoordinates().x, planetLandGO.GetComponent<Planet>().GetPlanetCoordinates().y, 0);
        TransitionController.Instance.mapGO.SetActive(false);

        lastPlanetVisit.ClearLinkedPlanets();
        planetWellington.ClearLinkedPlanets();
        Destroy(planetWellington.linkLine);
        nextPlanetVisit = null;
        lastPlanetVisit = null;

        isJourneyEditMode = true;
        StartCoroutine(TransitionController.Instance.TransitionInMap());

        passengersDeliveredLastJourney = 0;

        foreach (GameObject passengerGO in currentPassengers)
        {
            int payment = passengerGO.GetComponent<Passenger>().GetPassengerFare();
            CurrencyController.Instance.AddBankBalance(payment);
            passengersDeliveredLastJourney++;
            passengersDeliveredTotal++;
        }
        foreach (GameObject passengerGO in currentPassengers)
        {
            if (passengerGO != null)
            {
                
                Destroy(passengerGO);
            }
        }
        currentPassengers.Clear();

        // Set text
        text_CurrentLocation.text = "Stationed: Planet Wellington";

        // Generate new potential passengers for all planets 
       foreach(GameObject planetGO in WorldController.Instance.GetPlanets())
        {
            planetGO.GetComponent<Planet>().GeneratePotentialPassengers();
        }


        // Check Objectives 
        ObjectiveController.Instance.CheckObjectives();
    }

}
