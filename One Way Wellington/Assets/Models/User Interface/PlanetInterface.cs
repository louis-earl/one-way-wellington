﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlanetInterface : MonoBehaviour
{
    private Vector3 homePosition;
    private Planet planet;

    // Passenger characters to instantiate 
    public GameObject passengerPrefab;

    // Passenger listings
    public GameObject passengerListingPrefab;
    public GameObject passengerListingContentParent;

    // GameObject fields 
    public Button button_AddToJourney;
    public Button button_ContinueJourney;

    public TextMeshProUGUI tmp_PlanetName;
    public TextMeshProUGUI tmp_PlanetDistance;

    public bool moveWithPlanet = true;

    public bool isMouseOver;


    // Start is called before the first frame update
    void Start()
    {
        homePosition = transform.position;
        if (!JourneyController.Instance.isJourneyEditMode)
        {
            button_AddToJourney.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Move the UI with the camera zoom level 
        if (moveWithPlanet)
        {
            transform.localScale = Vector3.Lerp(new Vector3(Camera.main.orthographicSize / 500, Camera.main.orthographicSize / 500, 1), transform.localScale, 0.8f);
            transform.localPosition = new Vector3(homePosition.x + 10f + (Camera.main.orthographicSize / 2.75f), homePosition.y, homePosition.z);
        }
    }

    private void OnMouseEnter()
    {
        isMouseOver = true;
    }

    private void OnMouseExit()
    {
        isMouseOver = false;
    }

    // Activated by UI Button 
    public void AddToJourney(Planet planet)
    {
        JourneyController.Instance.AddNextPlanet(planet);
        InputController.Instance.cameraZoomEnabled = true;
        Destroy(gameObject);
    }

    // Activated by UI button
    public void ContinueJourney()
    {
        StartCoroutine(ContinueJourneyEnum());
    }

    public IEnumerator ContinueJourneyEnum()
    {
        StartCoroutine(TransitionController.Instance.QuickFadeToBlack());
        yield return new WaitForSecondsRealtime(0.5f);

        Vector3 stairwellPos = Vector3.zero;

        // Get position of stairwell 
        if (BuildModeController.Instance.furnitureTileOWWMap.ContainsKey("Stairwell"))
        {
            stairwellPos = new Vector3(BuildModeController.Instance.furnitureTileOWWMap["Stairwell"][0].GetX(), BuildModeController.Instance.furnitureTileOWWMap["Stairwell"][0].GetY(), 0);
        }
        else
        {
            Debug.LogWarning("Couldn't find a stairwell!!");
        }

        // Instantiate passengers 
        foreach (PotentialPassenger potentialPassenger in planet.selectedPassengers)
        {

            GameObject passengerGO = Instantiate(passengerPrefab, stairwellPos, Quaternion.identity);
            int distance = (int)Vector2.Distance(planet.GetPlanetCoordinates(), Vector2.zero);

            passengerGO.transform.parent = JourneyController.Instance.passengerParent.transform;

            passengerGO.GetComponent<Passenger>().SetPassengerInformation(
                potentialPassenger.GetPassengerFirstName() + " " + potentialPassenger.GetPassengerLastName() + ".",
                potentialPassenger.GetPassengerOccupation(),
                planet.name,
                (100 + distance * 5),
                potentialPassenger.hairStyle,
                potentialPassenger.hairColor,
                potentialPassenger.skin,
                potentialPassenger.decal,
                potentialPassenger.shirtStyle,
                potentialPassenger.shirtColor,
                potentialPassenger.pantStyle,
                potentialPassenger.pantColor,
                potentialPassenger.shoeStyle,
                potentialPassenger.shades);

            JourneyController.Instance.currentPassengers.Add(passengerGO);

            // Remove passenger as potential passneger from planet 
            planet.RemovePotentialPassenger(potentialPassenger);

        }

        planet.selectedPassengers.Clear();

        Destroy(gameObject);

        JourneyController.Instance.ContinueJourney(planet);
    }

    // An initialisation function 
    public void SetPlanet(Planet planet, bool isAtPlanet)
    {
        this.planet = planet;

        button_AddToJourney.onClick.AddListener(() => AddToJourney(this.planet));
        button_ContinueJourney.onClick.AddListener(() => ContinueJourney());
        tmp_PlanetName.GetComponent<TextMeshProUGUI>().SetText(this.planet.GetPlanetName());
        int distance = (int) Vector2.Distance(planet.GetPlanetCoordinates(), Vector2.zero);
        tmp_PlanetDistance.SetText(distance + " light-years from wellington");

        foreach (PotentialPassenger passenger in planet.GetPotentialPassengers()) 
        {
            GameObject listing = Instantiate(passengerListingPrefab);
            listing.transform.SetParent(passengerListingContentParent.transform);
            listing.transform.localPosition = new Vector3(listing.transform.localPosition.x, listing.transform.localPosition.y, 0);
            listing.transform.localScale = Vector3.one;

            // TODO: Listing info
            listing.GetComponent<PassengerListing>().InitPassengerListing(passenger, planet);

            if (!isAtPlanet)
            {
                listing.GetComponent<PassengerListing>().SetBoardButtonActive(false);
            }
        }
        passengerListingContentParent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 50 * passengerListingContentParent.transform.childCount);

    }

    public void SetMoveWithPlanet(bool moveWithPlanet)
    {
        this.moveWithPlanet = moveWithPlanet;
    }

}
