using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StaffInterface : MonoBehaviour
{

    public Staff staff;

    public TextMeshProUGUI staffName;
    public TextMeshProUGUI currentJob;
    public Slider sliderEnergy;
    public Slider sliderHealth;

    public TextMeshProUGUI inventoryItemType;
    public TextMeshProUGUI inventoryQuantity;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Move the UI with the camera zoom level 

        transform.localScale = Vector3.Lerp(new Vector3(Camera.main.orthographicSize / 500, Camera.main.orthographicSize / 500, 1), transform.localScale, 0.9f);
        transform.localPosition = Vector3.Lerp(new Vector3(staff.gameObject.transform.position.x + 0.5f + (Camera.main.orthographicSize / 2.75f), staff.gameObject.transform.position.y + 0.35f, 0), transform.position, 0.9f);


        // Update the UI elements
        if (staff.currentJob == null || staff.currentJob.GetJobType() == "Wander")
        {
            currentJob.text = "Not doing anything right now.";
        }
        else
        {
            currentJob.text = "Going to " + staff.currentJob.GetJobType();
        }

        sliderEnergy.value = staff.GetEnergy();
        sliderHealth.value = staff.GetHealth();

        if (staff.GetInventory() != null)
        {
            inventoryItemType.text = staff.GetInventory().itemType;
            inventoryQuantity.text = staff.GetInventory().quantity.ToString();
        }
        else
        {
            inventoryItemType.text = "";
            inventoryQuantity.text = "";
        }
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
