using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PassengerInterface : MonoBehaviour
{

    private Passenger passenger;

    public TextMeshProUGUI passengerName;
    public TextMeshProUGUI passengerOccupation;
    public TextMeshProUGUI passengerHomePlanet;
    public TextMeshProUGUI currentJob;

    public Slider sliderEnergy;
    public Slider sliderNourishment;
    public Slider sliderOxygen;
    public Slider sliderBladder;
    public Slider sliderHappiness;
    public Slider sliderHygiene;
    public Slider sliderHealth;


    public void InitPassengerUI(Passenger passenger)
    {
        this.passenger = passenger;
        passengerName.text = passenger.name;
        passengerOccupation.text = passenger.occupation;
        passengerHomePlanet.text = passenger.homePlanet;

    }

    // Update is called once per frame
    void Update()
    {
        // Move the UI with the camera zoom level 
        if (passenger == null)
        {
            Destroy(gameObject);
            return;
        }
        transform.localScale = Vector3.Lerp(new Vector3(Camera.main.orthographicSize / 500, Camera.main.orthographicSize / 500, 1), transform.localScale, 0.9f);
        transform.localPosition = Vector3.Lerp(new Vector3(passenger.gameObject.transform.position.x + 0.5f + (Camera.main.orthographicSize / 2.75f), passenger.gameObject.transform.position.y + 0.35f, 0), transform.position, 0.9f);

        if (passenger.currentJob == null || passenger.currentJob.GetJobType() == "Wander")
        {
            currentJob.text = "Not doing anything right now.";
        }
        else
        {
            currentJob.text = passenger.currentJob.GetJobType();
        }

        sliderHealth.value = passenger.GetHealth();
        sliderEnergy.value = passenger.GetEnergy();
        sliderBladder.value = passenger.GetBladder();
        // sliderHappiness.value = pas
        sliderHygiene.value = passenger.GetHygiene();
        sliderNourishment.value = passenger.GetNourishment();
        sliderOxygen.value = passenger.GetOxygen();
    }

    private void OnMouseEnter()
    {
        InputController.Instance.cameraZoomEnabled = false;
    }

    private void OnMouseExit()
    {
        InputController.Instance.cameraZoomEnabled = true;
    }


}
