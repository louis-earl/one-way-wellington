using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PassengerListing : MonoBehaviour
{

    public Image image_PassengerPreview;
    public Toggle toggle_BoardPassenger;
    public TextMeshProUGUI text_PassengerName;
    public TextMeshProUGUI text_PassengerFare;

    private PotentialPassenger potentialPassenger;
    private Planet planet;

    // Initialisation 
    public void InitPassengerListing(PotentialPassenger potentialPassenger, Planet planet)
    {
        this.text_PassengerName.SetText(potentialPassenger.GetPassengerFirstName() + " " + potentialPassenger.GetPassengerLastName() + ".");
        this.planet = planet;
        this.potentialPassenger = potentialPassenger;

        int distance = (int) Vector2.Distance(planet.GetPlanetCoordinates(), Vector2.zero);
        text_PassengerFare.SetText("$" + (100 + distance * 5).ToString());
    }

    // Board button pressed
    public void ToggleAddToSelectedPassengers()
    {
        if (planet.selectedPassengers.Contains(potentialPassenger))
        {
            // Remove passenger from selectedPassengers
            planet.selectedPassengers.Remove(potentialPassenger);
        }
        else
        {
            // Add passenger to selectedPassengers
            planet.selectedPassengers.Add(potentialPassenger);
        }
    }

    // Sow/hide board button 
    public void SetBoardButtonActive(bool isActive)
    {
        toggle_BoardPassenger.gameObject.SetActive(isActive);
    }
}
