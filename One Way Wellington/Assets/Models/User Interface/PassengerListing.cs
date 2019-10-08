using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PassengerListing : MonoBehaviour
{

    public Toggle toggle_BoardPassenger;
    public TextMeshProUGUI text_PassengerName;
    public TextMeshProUGUI text_PassengerFare;

    private PotentialPassenger potentialPassenger;
    private Planet planet;

    // Passenger visuals 
    public Image skin;
    public Image shades;
    public Image pants;
    public Image shirt;
    public Image shoes;
    public Image hair;
    public Image decal;



    // Initialisation 
    public void InitPassengerListing(PotentialPassenger potentialPassenger, Planet planet)
    {
        this.text_PassengerName.SetText(potentialPassenger.GetPassengerFirstName() + " " + potentialPassenger.GetPassengerLastName() + ".");
        this.planet = planet;
        this.potentialPassenger = potentialPassenger;

        int distance = (int) Vector2.Distance(planet.GetPlanetCoordinates(), Vector2.zero);
        text_PassengerFare.SetText("$" + (100 + distance * 5).ToString());

        // Load sprite resources 
        hair.sprite = Resources.Load<Sprite>("Images/Characters/Passengers/Hair/Hair" + potentialPassenger.hair.ToString());
        skin.sprite = Resources.Load<Sprite>("Images/Characters/Passengers/Skin/Skin" + potentialPassenger.skin.ToString());
        decal.sprite = Resources.Load<Sprite>("Images/Characters/Passengers/Decal/Decal" + potentialPassenger.decal.ToString());
        shirt.sprite = Resources.Load<Sprite>("Images/Characters/Passengers/Shirt/Shirt" + potentialPassenger.shirt.ToString());
        pants.sprite = Resources.Load<Sprite>("Images/Characters/Passengers/Pants/Pants" + potentialPassenger.pants.ToString());
        shoes.sprite = Resources.Load<Sprite>("Images/Characters/Passengers/Shoes/Shoes" + potentialPassenger.shoes.ToString());
        shades.sprite = Resources.Load<Sprite>("Images/Characters/Passengers/Shades/Shades" + potentialPassenger.shades.ToString());

        if (hair.sprite == null) hair.color = Color.clear;
        if (skin.sprite == null) skin.color = Color.clear;
        if (decal.sprite == null) decal.color = Color.clear;
        if (shirt.sprite == null) shirt.color = Color.clear;
        if (pants.sprite == null) pants.color = Color.clear;
        if (shoes.sprite == null) shoes.color = Color.clear;
        if (shades.sprite == null) shades.color = Color.clear;
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
