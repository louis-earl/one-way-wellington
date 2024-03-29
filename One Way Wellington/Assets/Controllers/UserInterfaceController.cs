﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UserInterfaceController : MonoBehaviour
{
    // Build panels 
    public GameObject[] subPanels;
    public GameObject panel_Building;
    public GameObject panel_GoToMap;
    public GameObject panel_GoToShip;
    public GameObject panel_LaunchJourney;
    public GameObject panel_LandShip;

    // Prefabs 
    public GameObject passengerUIPrefab;
    public GameObject staffUIPrefab;


    // Reference to the canvas 
    public GameObject canvas_Main;


    // Tooltips 
    public GameObject pricePopUpPrefab;
    public GameObject pricePopUpInstance;
    public GameObject tooltipInstance;
    public string toolTipText;

    // Audio 
    public AudioSource audio_OpenBuildPanel;
    public AudioSource audio_SelectBuildItem;

    public static UserInterfaceController Instance;

    public GameObject NotificationPanel;
    public GameObject ObjectivesPanel;

    // Start is called before the first frame update
    void Start()
    {
        panel_Building.SetActive(true);
        panel_GoToMap.SetActive(true);

        // TODO: Sometimes when we start the game, we want to go to the right state

        panel_GoToShip.SetActive(false);
        panel_LaunchJourney.SetActive(false);
        panel_LandShip.SetActive(false);

        CloseAllBuilding();
        if (Instance == null) Instance = this;

        tooltipInstance.SetActive(false);

        NotificationPanel.SetActive(false);
        ObjectivesPanel.SetActive(false);
    }

    public void CloseAllBuilding()
    {
        BuildModeController.Instance.SetAllListingTogglesOff();

        foreach (GameObject g in subPanels)
        {
            g.SetActive(false);
        }
        BuildModeController.Instance.roomsTilemap.SetActive(false);
        tooltipInstance.SetActive(false);

    }

    private void CloseOtherBuilding(int ignore)
    {

        audio_SelectBuildItem.Play();
        audio_OpenBuildPanel.PlayDelayed(0.04f);
        
        BuildModeController.Instance.SetAllListingTogglesOff();

        foreach (GameObject g in subPanels)
        {
            if (g != subPanels[ignore]) g.SetActive(false);
        }
        BuildModeController.Instance.roomsTilemap.SetActive(false);
        tooltipInstance.SetActive(false);


    }



    public void ToggleHullPanel()
    {
        CloseOtherBuilding(0);
        subPanels[0].SetActive(!subPanels[0].activeInHierarchy);
        if (!subPanels[0].activeInHierarchy)
        {
            BuildModeController.Instance.roomsTilemap.SetActive(false);
            InputController.Instance.SetMode_None();
        }
        else
        {
            tooltipInstance.SetActive(true);       
        }
        toolTipText = "The hull is the foundation of your spaceship. All interior objects must be placed on a hull tile. Hover over an item for more information.";
        tooltipInstance.GetComponentInChildren<TextMeshProUGUI>().text = toolTipText;

    }


    public void ToggleWallPanel()
    {
        CloseOtherBuilding(1);
        subPanels[1].SetActive(!subPanels[1].activeInHierarchy);
        if (!subPanels[1].activeInHierarchy)
        {
            BuildModeController.Instance.roomsTilemap.SetActive(false);
            InputController.Instance.SetMode_None();
        }
        else
        {
            tooltipInstance.SetActive(true);
        }
        toolTipText = "Walls don't let oxygen past, so they are a necessity to enclose the exterior of your ship. Hover over an item for more information.";
        tooltipInstance.GetComponentInChildren<TextMeshProUGUI>().text = toolTipText;

    }

    public void ToggleUtilityPanel()
    {
        CloseOtherBuilding(2);
        subPanels[2].SetActive(!subPanels[2].activeInHierarchy);
        if (!subPanels[2].activeInHierarchy)
        {
            BuildModeController.Instance.roomsTilemap.SetActive(false);
            InputController.Instance.SetMode_None();
        }
        else
        {
            tooltipInstance.SetActive(true);
        }
        toolTipText = "Utility items are necessities to get your ship up and running. Hover over an item for more information.";
        tooltipInstance.GetComponentInChildren<TextMeshProUGUI>().text = toolTipText;

    }

    public void ToggleFurniturePanel()
    {
        CloseOtherBuilding(3);
        subPanels[3].SetActive(!subPanels[3].activeInHierarchy);
        if (!subPanels[3].activeInHierarchy)
        {
            BuildModeController.Instance.roomsTilemap.SetActive(false);
            InputController.Instance.SetMode_None();
        }
        else
        {
            tooltipInstance.SetActive(true);
        }
        toolTipText = "Furniture objects add functionality to your ship. Hover over an item for more information.";
        tooltipInstance.GetComponentInChildren<TextMeshProUGUI>().text = toolTipText;
        
    }

    public void ToggleRoomsPanel()
    {
        CloseOtherBuilding(4);
        subPanels[4].SetActive(!subPanels[4].activeInHierarchy);
        if (!subPanels[4].activeInHierarchy)
        {
            BuildModeController.Instance.roomsTilemap.SetActive(false);
            InputController.Instance.SetMode_None();
        }
        else
        {
            tooltipInstance.SetActive(true);
            BuildModeController.Instance.roomsTilemap.SetActive(true);
        }
        toolTipText = "Rooms allow you to designate an area for a particular purpose. Hover over an item for more information.";
        tooltipInstance.GetComponentInChildren<TextMeshProUGUI>().text = toolTipText;
    }

    public void ToggleStaffPanel()
    {
        CloseOtherBuilding(5);
        subPanels[5].SetActive(!subPanels[5].activeInHierarchy);
        if (!subPanels[5].activeInHierarchy)
        {
            BuildModeController.Instance.roomsTilemap.SetActive(false);
            InputController.Instance.SetMode_None();
        }
        else
        {
            tooltipInstance.SetActive(true);
        }
        toolTipText = "Hire staff-bots to maintain ship operations. They run on energised coffee and recharge at the charging pad. Hover over an item for more information.";
        tooltipInstance.GetComponentInChildren<TextMeshProUGUI>().text = toolTipText;
    }

    public void ShowMapUI()
    {
        CloseAllBuilding(); // Might cause error if GameObject is disabled 
        panel_Building.SetActive(false);
        panel_GoToMap.SetActive(false);
        panel_LandShip.SetActive(false);      
        panel_GoToShip.SetActive(true);

    }

    public void HideBuildUI()
    {
        CloseAllBuilding(); // Might cause error if GameObject is disabled 
        panel_Building.SetActive(false);
    }

    public void ShowBuildUI()
    {
        CloseAllBuilding(); // Might cause error if GameObject is disabled 
        panel_Building.SetActive(true);
    }

    public void ShowMainUI()
    {
        CloseAllBuilding(); // Might cause error if GameObject is disabled 
        panel_Building.SetActive(true);
        panel_GoToShip.SetActive(false);
        panel_LaunchJourney.SetActive(false);
        panel_GoToMap.SetActive(true);
        panel_LandShip.SetActive(false);

    }

    public void ShowLandUI()
    {
        panel_GoToMap.SetActive(false);
        panel_LandShip.SetActive(true);
    }
}
