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

    public int hairStyle;
    public Color hairColor;
    public int skin;
    public int pantStyle;
    public float pantColor;
    public int decal;
    public int shades;
    public int shirtStyle;
    public Color shirtColor;
    public int shoeStyle;

    private List<Color> hairColors = new List<Color>()
    {
        new Color(0.847f, 0.753f, 0.470f), new Color(0.89f, 0.8f,0.533f), new Color(0.949f,0.854f, 0.569f), new Color(0.949f, 0.882f, 0.682f), new Color(0.949f, 0.906f, 0.78f),
        new Color(0.4f, 0.31f, 0.235f),new Color(0.549f,0.408f,0.29f ),new Color(0.2f, 0.165f, 0.1333f),
        new Color(0.26f,0.14f,0.60f),new Color(0,0.53f, 0.81f),new Color(0.74f,0.13f,0.58f),new Color(0.96f,0.82f,0.08f),new Color(0.25f,0.64f,0.22f), new Color(0.92f,0.58f,0.12f),new Color(0.87f, 0.87f, 0.87f),
        new Color(1,0.97f,0.78f),new Color(0.61f,0.58f,0.39f),new Color(),new Color(1,0.91f,0.78f),new Color(0.31f,0.27f,0.21f),
        new Color(1,1,1), new Color(0.87f, 0.87f, 0.87f), new Color(0.5f,0.5f,0.5f), new Color(0.14f, 0.14f, 0.14f), new Color(0, 0, 0)
    };
    private List<Color> shirtColors = new List<Color>
    {
        new Color(0.26f,0.14f,0.60f), new Color(0.42f,0.30f,0.78f), new Color(0.16f,0.03f,0.54f), new Color(0.08f,0.01f,0.27f),
        new Color(0,0.53f, 0.81f), new Color(0.69f,0.89f,1), new Color(0.23f,0.73f,1), new Color(0,0.36f,0.55f), new Color(0,0.14f,0.21f),
        new Color(0.74f,0.13f,0.58f), new Color(0.87f,0.11f,0.73f), new Color(0.46f,0,0.35f),new Color(0.23f,0,0.17f),
        new Color(0.96f,0.82f,0.08f), new Color(1,0.89f,0.32f), new Color(0.74f,0.62f,0),
        new Color(0.25f,0.64f,0.22f), new Color(0.68f,0.96f,0.67f), new Color(0.43f,0.79f,0.40f), new Color(0.05f,0.49f,0.01f), new Color(0.09f,0.31f,0.06f),
        new Color(0.92f,0.58f,0.12f), new Color(1,0.69f,0.28f), new Color(0.74f,0.42f,0),
        new Color(0.87f, 0.87f, 0.87f), new Color(0.5f,0.5f,0.5f), new Color(0.14f, 0.14f, 0.14f)
    };


    // TODO: Reference to sprites used for passenger preview  

    public PotentialPassenger(string passengerFirstName, string passengerLastName, string occupation)
    {
        this.passengerFirstName = passengerFirstName;
        this.passengerLastName = passengerLastName;
        this.occupation = occupation;

        // Random generate appearance 
        hairStyle = Random.Range(0, 9);
        hairColor = hairColors[Random.Range(0, hairColors.Count)];
        skin = Random.Range(1, 7);
        pantStyle = Random.Range(1, 2);
        pantColor = Random.Range(0.1f, 1f);
        decal = Random.Range(0, 6);
        shades = Random.Range(0, 13);
        shirtStyle = Random.Range(1, 1);
        shirtColor = shirtColors[Random.Range(0, shirtColors.Count)];
        shoeStyle = Random.Range(1, 2);
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
