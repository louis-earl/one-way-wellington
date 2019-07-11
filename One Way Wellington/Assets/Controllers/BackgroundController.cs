using UnityEngine;
using System.Collections;

public class BackgroundController : MonoBehaviour
{
    public static BackgroundController Instance;

    public FreeParallax parallax;


    // Use this for initialization
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }        
    }

    private void Update()
    {
        // Don't move the background in map mode, but the ship may continue to travel independently 
        if (TransitionController.Instance.isMapMode) parallax.Speed = 0;
        else parallax.Speed = -JourneyController.Instance.GetShipSpeedCurrent() * parallax.transform.localScale.x;
    }



}
