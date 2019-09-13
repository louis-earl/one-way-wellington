using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PassengerInterface : MonoBehaviour
{

    public Passenger passenger;

    public TextMeshProUGUI passengerName;
    public TextMeshProUGUI currentJob;

    public Slider sliderEnergy;
    public Slider sliderNourishment;
    public Slider sliderOxygen;
    public Slider sliderBladder;
    public Slider sliderHappiness;
    public Slider sliderHygiene;
    public Slider sliderHealth;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Move the UI with the camera zoom level 

        transform.localScale = Vector3.Lerp(new Vector3(Camera.main.orthographicSize / 500, Camera.main.orthographicSize / 500, 1), transform.localScale, 0.8f);
        transform.localPosition = new Vector3(passenger.GetXPos() + 0.5f + (Camera.main.orthographicSize / 2.75f), passenger.GetYPos() + 0.35f, 0);

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
