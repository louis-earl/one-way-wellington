using UnityEngine;
using UnityEngine.UI;

public class UserInterfaceController : MonoBehaviour
{
    public GameObject[] subPanels;
    public GameObject panel_Building;
    public GameObject panel_GoToMap;
    public GameObject panel_GoToShip;
    public GameObject panel_LaunchJourney;
    public GameObject panel_LandShip;

    public Button button_Hull;
    public Button button_Wall;
    public Button button_Storage;
    public Button button_Furniture;
    public Button button_Rooms;
    public Button button_Staff;

    public GameObject passengerUIPrefab;
    public GameObject staffUIPrefab;

    public GameObject canvas_Main;

    public GameObject pricePopUpPrefab;
    public GameObject pricePopUpInstance;

    public GameObject tooltipInstance;

    public static UserInterfaceController Instance;

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
    }

    private void CloseAllBuilding()
    {
        foreach (GameObject g in subPanels)
        {
            g.SetActive(false);
        }
        button_Hull.image.color = Color.white;
        button_Wall.image.color = Color.white;
        button_Storage.image.color = Color.white;
        button_Furniture.image.color = Color.white;
        button_Staff.image.color = Color.white;
        BuildModeController.Instance.roomsTilemap.SetActive(false);
        tooltipInstance.SetActive(false);

    }

    private void CloseOtherBuilding(int ignore)
    {
        foreach (GameObject g in subPanels)
        {
            if (g != subPanels[ignore]) g.SetActive(false);
        }
        button_Hull.image.color = Color.white;
        button_Wall.image.color = Color.white;
        button_Storage.image.color = Color.white;
        button_Furniture.image.color = Color.white;
        button_Rooms.image.color = Color.white;
        button_Staff.image.color = Color.white;
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
            button_Hull.image.color = new Color(1f, 0.73f, 0.94f);
        }
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
            button_Wall.image.color = new Color(1f, 0.73f, 0.94f);
        }
    }

    public void ToggleStoragePanel()
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
            button_Storage.image.color = new Color(1f, 0.73f, 0.94f);
        }
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
            button_Furniture.image.color = new Color(1f, 0.73f, 0.94f);
        }
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
            button_Rooms.image.color = new Color(1f, 0.73f, 0.94f);
            BuildModeController.Instance.roomsTilemap.SetActive(true);
        }
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
            button_Staff.image.color = new Color(1f, 0.73f, 0.94f);
        }
    }

    public void ShowMapUI()
    {
        CloseAllBuilding(); // Might cause error if GameObject is disabled 
        panel_Building.SetActive(false);
        panel_GoToMap.SetActive(false);
        panel_LandShip.SetActive(false);      
        panel_GoToShip.SetActive(true);

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
