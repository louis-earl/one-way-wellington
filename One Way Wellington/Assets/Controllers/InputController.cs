using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InputController : MonoBehaviour
{
    public static InputController Instance;

    // Background 
    public GameObject backgroundGO_Ship;
    public GameObject backgroundGO_Map;

    // Build measure / price interface
    public GameObject buildMeasurePrefabX;
    public GameObject buildMeasurePrefabY;
    public GameObject buildPricePrefab;

    private GameObject buildMeasureInstanceX;
    private GameObject buildMeasureInstanceY;
    private GameObject buildPriceInstance;

    public int currentBuildPrice;


    // Mouse positions 
    private Vector3 lastFramePosition;
    private Vector3 currFramePosition;
    private Vector3 dragStartPosition;

    // Build modes 
    private bool isMode_Plopable;
    private bool isMode_Dragable;

    private bool isMode_Hull;
    private bool isMode_HullNoWalls;
    private bool isMode_RemoveHull;
    private bool isMode_Wall;
    private bool isMode_RemoveWall;
    private bool isMode_Staff;
    private bool isMode_Furniture;
    private bool isMode_FurnitureMulti;
    private bool isMode_RemoveFurniture;
    private bool isMode_Rooms;




    private FurnitureType furnitureTypeInUse;
    private string roomTypeInUse;

    private GameObject staff;

    // Background scale
    private float orthoOrg;
    private float orthoCurr;
    private Vector3 scaleOrg;
    private Vector3 posOrg;


    // Camera 
    public float desiredCameraZoom;
    public Vector3 desiredCameraPos;

    public bool cameraZoomEnabled;
    public float cameraSizeMin;
    public float cameraSizeMax;
    public float cameraPosZ;

    public float cameraBoundMin;
    public float cameraBoundMax;

    // Tile Interface
    public static GameObject tileInterfaceGO;
    public GameObject tileInterfacePrefab;


    // Audio 
    public AudioSource audio_Button_SelectBuild;
    public AudioSource audio_Button_CloseBuild;
    public GameObject audio_Blueprint_Drag; // Gameobject because we want multiple instances playing at once 
    private float timeOfPlay_BlueprintDrag = 0f;
    public AudioSource audio_Blueprint_DragComplete;
    public GameObject audio_Blueprint_Plop;
    private float timeOfPlay_BlueprintPlop = 0f;
    public AudioSource audio_Blueprint_PlopComplete;



    

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null) Instance = this;

        // Background scale
        orthoOrg = Camera.main.orthographicSize;
        orthoCurr = orthoOrg;
        // scaleOrg = backgroundGO.transform.localScale;
        // posOrg = Camera.main.WorldToViewportPoint(backgroundGO.transform.position);

        cameraZoomEnabled = true;
        cameraSizeMin = 3f;
        cameraSizeMax = 50f;
        cameraPosZ = -10;
        cameraBoundMin = -20;
        cameraBoundMax = 120;

        desiredCameraZoom = Camera.main.orthographicSize;
        desiredCameraPos = Camera.main.transform.position;

        // Switch background GO 
        InputController.Instance.backgroundGO_Ship.SetActive(true);
        InputController.Instance.backgroundGO_Map.SetActive(false);
    }

    private bool isCanceled = false;

    // Update is called once per frame
    void Update()
    {
        // Camera zoom 
        currFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currFramePosition.z = 0;

        if (Input.GetMouseButtonDown(0))
        {
            ClearUI();
        }

        // We are building something and only want to cancel that 
        if (Input.GetButtonDown("Cancel") && Input.GetMouseButton(0))
        {       
            BuildModeController.Instance.ClearPreviews();
            Destroy(buildMeasureInstanceX);
            Destroy(buildMeasureInstanceY);
            Destroy(buildPriceInstance);
            ClearUI();
            isCanceled = true;
            UserInterfaceController.Instance.tooltipInstance.GetComponentInChildren<TextMeshProUGUI>().text = UserInterfaceController.Instance.toolTipText;
            audio_Button_CloseBuild.Play();
        }
        // We want to stop building selected 
        else if (Input.GetButtonDown("Cancel") && (isMode_Dragable || isMode_Plopable))
        {
            BuildModeController.Instance.SetAllListingTogglesOff();
            SetMode_None();
            ClearUI();
            audio_Button_CloseBuild.Play();

        }
        // We want to exit build mode 
        else if (Input.GetButtonDown("Cancel"))
        {
            UserInterfaceController.Instance.CloseAllBuilding();
            UserInterfaceController.Instance.panel_Building.GetComponent<ToggleGroup>().SetAllTogglesOff(false);
            audio_Button_CloseBuild.Play();

        }
        // Allow building 
        else if (!isCanceled)
        {
            UpdateDragging();          
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isCanceled = false;
            UserInterfaceController.Instance.tooltipInstance.GetComponentInChildren<TextMeshProUGUI>().text = UserInterfaceController.Instance.toolTipText;

        }

        UpdateCameraMovement();
        UpdateTileInterface();

        lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lastFramePosition.z = 0;


    }

    void UpdateTileInterface()
    {
        // Abort if in any building mode
        if (isMode_Plopable || isMode_Dragable)
        {
            return;
        }

        if (Input.GetMouseButtonUp(0)) // LMB
        {
            // Check if player clicked on something other than a tile 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.tag != "NavigationPlane")
                {
                    return;
                }
            }

            // Get tile clicked on 
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            TileOWW tileOWW = WorldController.Instance.GetWorld().GetTileAt((int)mousePos.x, (int)mousePos.y);

            // Null checks
            if (tileOWW == null)
            {
                return;
            }
            if (tileOWW.GetTileType() == "Empty" && tileOWW.currentJobType == null)
            {
                return;
            }

            // Create interface 
            tileInterfaceGO = Instantiate(tileInterfacePrefab);
            tileInterfaceGO.GetComponent<TileInterface>().tile = tileOWW;
            tileInterfaceGO.GetComponent<TileInterface>().tileType.text = tileOWW.GetTileType();
        }
    }


    void UpdateCameraMovement()
    {
        

        float currentCameraZoom = Camera.main.orthographicSize;

        if (cameraZoomEnabled)
        {
            // If mouse is NOT over UI element 
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                // Zoom camera 
                desiredCameraZoom -= desiredCameraZoom * Input.GetAxis("Mouse ScrollWheel") * 1.25f;                
            }           
        }
        desiredCameraZoom = Mathf.Clamp(desiredCameraZoom, cameraSizeMin, cameraSizeMax);
        Camera.main.orthographicSize = Mathf.Lerp(desiredCameraZoom, currentCameraZoom, 0.90f);

        // zoom into mouse 
        
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            desiredCameraPos = new Vector3(currFramePosition.x, currFramePosition.y, Camera.main.transform.position.z);
        }
        
        // Handle screen panning
        // Right or Middle Mouse Button
        if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            Camera.main.transform.Translate(lastFramePosition - currFramePosition);
            desiredCameraPos = Camera.main.transform.position;

        }
        else
        {
            
            float melerp = 1 - ((currentCameraZoom - desiredCameraZoom) / (100 * (currentCameraZoom/5)));
            Camera.main.transform.position = Vector3.Lerp(desiredCameraPos, Camera.main.transform.position, melerp);
        }

        // Camera bounds 
        if (Camera.main.transform.position.x < cameraBoundMin + currentCameraZoom) Camera.main.transform.position = new Vector3(cameraBoundMin + currentCameraZoom, Camera.main.transform.position.y, cameraPosZ);
        else if (Camera.main.transform.position.x > cameraBoundMax - currentCameraZoom) Camera.main.transform.position = new Vector3(cameraBoundMax - currentCameraZoom, Camera.main.transform.position.y, cameraPosZ);

        // Not contained in the same block because x and y camera bounds could be breached at the same time. 
        if (Camera.main.transform.position.y < cameraBoundMin + currentCameraZoom) Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, cameraBoundMin + currentCameraZoom, cameraPosZ);
        else if (Camera.main.transform.position.y > cameraBoundMax - currentCameraZoom) Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, cameraBoundMax - currentCameraZoom, cameraPosZ);



        // Always update background scale, camera orthographic size may be controlled elsewhere 
        // UpdateBackgroundScale();

        // Check objectives
        ObjectiveController.Instance.CheckObjectives();
    }

    public void SetMode_None()
    {
        isMode_Plopable = false;
        isMode_Dragable = false;
        isMode_Hull = false;
        isMode_HullNoWalls = false;
        isMode_RemoveHull = false;
        isMode_Wall = false;
        isMode_RemoveWall = false;
        isMode_Staff = false;
        staff = null;
        isMode_Furniture = false;
        isMode_FurnitureMulti = false;
        furnitureTypeInUse = null;
        isMode_RemoveFurniture = false;
        isMode_Rooms = false;
        roomTypeInUse = null;

        BuildModeController.Instance.SetGridVisible(false);
        BuildModeController.Instance.roomsTilemap.SetActive(false);
        BuildModeController.Instance.ClearPreviews();

        currentBuildPrice = 0;
        Destroy(buildPriceInstance);
    }

    public void ToggleMode_Hull()
    {
        bool temp = isMode_Hull;
        SetMode_None();
        isMode_Hull = !temp;
        isMode_Dragable = !temp;
        if (isMode_Hull) audio_Button_SelectBuild.Play();

        BuildModeController.Instance.SetGridVisible(true);
    }

    public void ToggleMode_HullNoWalls()
    {
        bool temp = isMode_HullNoWalls;
        SetMode_None();
        isMode_HullNoWalls = !temp;
        isMode_Dragable = !temp;
        if (isMode_HullNoWalls) audio_Button_SelectBuild.Play();

        BuildModeController.Instance.SetGridVisible(true);
    }

    public void ToggleMode_RemoveHull()
    {
        bool temp = isMode_RemoveHull;
        SetMode_None();
        isMode_RemoveHull = !temp;
        isMode_Dragable = !temp;
        if (isMode_RemoveHull) audio_Button_SelectBuild.Play();

        BuildModeController.Instance.SetGridVisible(true);
    }

    public void ToggleMode_Wall()
    {
        bool temp = isMode_Wall;
        SetMode_None();
        isMode_Wall = !temp;
        isMode_Dragable = !temp;
        if (isMode_Wall) audio_Button_SelectBuild.Play();

        BuildModeController.Instance.SetGridVisible(true);
    }

    public void ToggleMode_RemoveWall()
    {
        bool temp = isMode_RemoveWall;
        SetMode_None();
        isMode_RemoveWall = !temp;
        isMode_Dragable = !temp;
        if (isMode_RemoveWall) audio_Button_SelectBuild.Play();

        BuildModeController.Instance.SetGridVisible(true);
    }

    public void ToggleMode_Staff(GameObject staff)
    {
        bool temp = isMode_Staff;
        SetMode_None();
        isMode_Staff = !temp;
        isMode_Plopable = !temp;
        if (isMode_Staff) audio_Button_SelectBuild.Play();

        this.staff = staff;
        BuildModeController.Instance.SetGridVisible(true);
    }

    public void ToggleMode_Furniture(FurnitureType furnitureType)
    {
        if (furnitureType.multiSize)
        {
            ToggleMode_FurnitureMulti(furnitureType);
        }
        else
        {
            bool temp = isMode_Furniture;
            SetMode_None();
            isMode_Furniture = !temp;
            furnitureTypeInUse = furnitureType;
            isMode_Plopable = !temp;
            if (isMode_Furniture) audio_Button_SelectBuild.Play();

            BuildModeController.Instance.SetGridVisible(true);
        }
    }

    private void ToggleMode_FurnitureMulti(FurnitureType furnitureType)
    {
        bool temp = isMode_FurnitureMulti;
        SetMode_None();
        isMode_FurnitureMulti = !temp;
        furnitureTypeInUse = furnitureType;
        isMode_Dragable = !temp;
        if (isMode_FurnitureMulti) audio_Button_SelectBuild.Play();

        BuildModeController.Instance.SetGridVisible(true);

    }

    public void ToggleMode_RemoveFurniture()
    {
        bool temp = isMode_RemoveFurniture;
        SetMode_None();
        isMode_RemoveFurniture = !temp;
        isMode_Dragable = !temp;
        if (isMode_RemoveFurniture) audio_Button_SelectBuild.Play();

        BuildModeController.Instance.SetGridVisible(true);

    }

    public void ToggleMode_Rooms(string roomType)
    {
        bool temp = isMode_Rooms;
        SetMode_None();
        isMode_Rooms = !temp;
        roomTypeInUse = roomType;
        isMode_Dragable = !temp;
        if (isMode_Rooms) audio_Button_SelectBuild.Play();

        BuildModeController.Instance.roomsTilemap.SetActive(true);
        BuildModeController.Instance.SetGridVisible(true);
    }

    void ClearUI()
    {
        if (Passenger.passengerUIInstance != null) Destroy(Passenger.passengerUIInstance);
        if (Staff.staffUIInstance != null) Destroy(Staff.staffUIInstance);
        Character.currentSelectedCharacter = null;
        if (Planet.planetUI != null)
        {
            if (!Planet.planetUI.GetComponent<PlanetInterface>().isMouseOver)
            {
                Destroy(Planet.planetUI);
            }
        }
        if (tileInterfaceGO != null) Destroy(tileInterfaceGO);
    }

    void UpdateDragging()
    {
        // If we're over a UI element, then bail out from this.
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        // Start Drag
        if (Input.GetMouseButtonDown(0))
        {
            dragStartPosition = currFramePosition;
            UserInterfaceController.Instance.tooltipInstance.GetComponentInChildren<TextMeshProUGUI>().text = "Press ESC to cancel.";
        }

        int start_x = Mathf.FloorToInt(dragStartPosition.x);
        int end_x = Mathf.FloorToInt(currFramePosition.x);
        int start_y = Mathf.FloorToInt(dragStartPosition.y);
        int end_y = Mathf.FloorToInt(currFramePosition.y);

        // Allows us to preview a single tile (where the mouse is) of a drag-able input mode 
        if (!Input.GetMouseButton(0) && !Input.GetMouseButtonUp(0))
        {
            start_x = end_x;
            start_y = end_y;
        }

        // We may be dragging in the "wrong" direction, so flip things if needed.
        if (end_x < start_x)
        {
            int tmp = end_x;
            end_x = start_x;
            start_x = tmp;
        }
        if (end_y < start_y)
        {
            int tmp = end_y;
            end_y = start_y;
            start_y = tmp;
        }



        // Previews for drag-able modes 

        if (isMode_Hull) BuildModeController.Instance.PreviewHull(start_x, end_x, start_y, end_y, true);
        else if (isMode_HullNoWalls) BuildModeController.Instance.PreviewHull(start_x, end_x, start_y, end_y, false);
        else if (isMode_RemoveHull) BuildModeController.Instance.PreviewRemoveHull(start_x, end_x, start_y, end_y);
        else if (isMode_Wall) BuildModeController.Instance.PreviewWall(start_x, end_x, start_y, end_y);
        else if (isMode_RemoveWall) BuildModeController.Instance.PreviewRemoveWall(start_x, end_x, start_y, end_y);
        else if (isMode_FurnitureMulti) BuildModeController.Instance.PreviewFurniture(furnitureTypeInUse, start_x, end_x, start_y, end_y);
        else if (isMode_RemoveFurniture) BuildModeController.Instance.PreviewRemoveFurniture(start_x, end_x, start_y, end_y);
        else if (isMode_Rooms) BuildModeController.Instance.PreviewRoom(roomTypeInUse, start_x, end_x, start_y, end_y);

        // Previews for plop-able modes 

        if (isMode_Furniture) BuildModeController.Instance.PreviewFurniture(furnitureTypeInUse, (int)currFramePosition.x, (int)currFramePosition.y);
        else if (isMode_Staff) BuildModeController.Instance.PreviewStaff("builder", (int)currFramePosition.x, (int)currFramePosition.y);

        if (isMode_Hull || isMode_HullNoWalls || isMode_Wall || isMode_FurnitureMulti)
        {
            // Size indicator UI (X Axis)
            if (start_x != end_x)
            {
                if (buildMeasureInstanceX == null) buildMeasureInstanceX = Instantiate(buildMeasurePrefabX);
                float xPos = ((start_x + end_x) / 2);
                xPos += ((end_x - start_x) % 2 == 0) ? 0.5f : 1f;

                buildMeasureInstanceX.transform.position = new Vector3(xPos, start_y);
                buildMeasureInstanceX.transform.localScale = Vector3.one / 60;
                buildMeasureInstanceX.GetComponentInChildren<Image>().GetComponent<RectTransform>().sizeDelta = new Vector2((end_x - start_x + 1.13f) * 60, 100);
                buildMeasureInstanceX.GetComponentInChildren<TextMeshProUGUI>().text = (end_x - start_x + 1).ToString() + "m";
            }
            else
            {
                Destroy(buildMeasureInstanceX);
            }
            // Size indicator UI (Y Axis)
            if (start_y != end_y)
            {
                if (buildMeasureInstanceY == null) buildMeasureInstanceY = Instantiate(buildMeasurePrefabY);
                float yPos = ((start_y + end_y) / 2);
                yPos += ((end_y - start_y) % 2 == 0) ? 0.5f : 1f;

                buildMeasureInstanceY.transform.position = new Vector3(start_x, yPos);
                buildMeasureInstanceY.transform.localScale = Vector3.one / 60;
                buildMeasureInstanceY.GetComponentInChildren<Image>().GetComponent<RectTransform>().sizeDelta = new Vector2((end_y - start_y + 1.13f) * 60, 100);
                buildMeasureInstanceY.GetComponentInChildren<TextMeshProUGUI>().text = (end_y - start_y + 1).ToString() + "m";
            }
            else
            {
                Destroy(buildMeasureInstanceY);
            }

            // Dragging audio
            if (Input.GetMouseButton(0))
            {
                if ((int)currFramePosition.x != (int)lastFramePosition.x || (int)currFramePosition.y != (int)lastFramePosition.y)
                {
                    if (timeOfPlay_BlueprintDrag + 0.1f < Time.unscaledTime)
                    {
                        GameObject audioGO = Instantiate(audio_Blueprint_Drag);
                        audioGO.GetComponent<AudioSource>().pitch = UnityEngine.Random.Range(0.85f, 1.3f);
                        timeOfPlay_BlueprintDrag = Time.unscaledTime;
                        audioGO.GetComponent<AudioSource>().Play();
                        Destroy(audioGO, audioGO.GetComponent<AudioSource>().clip.length);
                    }
                }
            }

            // Price indicator UI 
            if (currentBuildPrice > 0)
            {
                if (buildPriceInstance == null) buildPriceInstance = Instantiate(buildPricePrefab);
                float pricePosX = ((start_x + end_x) / 2);
                pricePosX += ((end_x - start_x) % 2 == 0) ? 0.5f : 1f;
                float pricePosY = ((start_y + end_y) / 2);
                pricePosY += ((end_y - start_y) % 2 == 0) ? 0.5f : 1f;

                // if not fitting within axis graphics 
                if (end_y - start_y == 0 && end_x - start_x < 3)
                {
                    pricePosY += 1;
                }
                else if (end_y - start_y == 1 && end_x - start_x < 3)
                {
                    pricePosY += 0.5f;
                    pricePosX += 1;
                }
                else if (end_x - start_x == 2)
                {
                    pricePosX += 0.5f;
                }
                else if (end_x - start_x == 1)
                {
                    pricePosX += 1;
                }
                else if (end_x - start_x == 0)
                {
                    pricePosX += 1.5f;
                }

                buildPriceInstance.transform.position = new Vector3(pricePosX, pricePosY);
                buildPriceInstance.transform.localScale = Vector3.one / 60;
                buildPriceInstance.GetComponentInChildren<TextMeshProUGUI>().text = string.Format("{0:C}", currentBuildPrice);
            }
            else Destroy(buildPriceInstance);
        }
        else if (isMode_Plopable)
        {
            // play sound 
            if ((int)currFramePosition.x != (int)lastFramePosition.x || (int)currFramePosition.y != (int)lastFramePosition.y)
            {
                if (timeOfPlay_BlueprintPlop + 0.075f < Time.unscaledTime)
                {
                    GameObject audioGO = Instantiate(audio_Blueprint_Plop);
                    audioGO.GetComponent<AudioSource>().pitch = UnityEngine.Random.Range(0.85f, 1.3f);
                    timeOfPlay_BlueprintPlop = Time.unscaledTime;
                    audioGO.GetComponent<AudioSource>().Play();
                    Destroy(audioGO, audioGO.GetComponent<AudioSource>().clip.length);
                }
            }

            // update price graphic 
            if (currentBuildPrice > 0)
            {
                if (buildPriceInstance == null) buildPriceInstance = Instantiate(buildPricePrefab);
                buildPriceInstance.transform.position = new Vector3((int)currFramePosition.x + 0.5f, (int)currFramePosition.y + 1.5f);
                buildPriceInstance.transform.localScale = Vector3.one / 60;
                buildPriceInstance.GetComponentInChildren<TextMeshProUGUI>().text = string.Format("{0:C}", currentBuildPrice);
            }
        }

        // End Drag
        if (Input.GetMouseButtonUp(0))
        {
            UserInterfaceController.Instance.tooltipInstance.GetComponentInChildren<TextMeshProUGUI>().text = UserInterfaceController.Instance.toolTipText;

            if (isMode_Hull)
            {
                Tuple<List<TileOWW>, List<TileOWW>, List<TileOWW>> hull_tuple;
                hull_tuple = BuildModeController.Instance.PreviewHull(start_x, end_x, start_y, end_y, true);
                if (hull_tuple != null)
                {
                    BuildModeController.Instance.PlanHull(hull_tuple.Item1, hull_tuple.Item2, hull_tuple.Item3);
                }
            }
            if (isMode_HullNoWalls)
            {
                Tuple<List<TileOWW>, List<TileOWW>, List<TileOWW>> hull_tuple;
                hull_tuple = BuildModeController.Instance.PreviewHull(start_x, end_x, start_y, end_y, false);
                if (hull_tuple != null)
                {
                    BuildModeController.Instance.PlanHull(hull_tuple.Item1, hull_tuple.Item2, hull_tuple.Item3);
                }
            }
            else if (isMode_RemoveHull)
            {
                List<TileOWW> hull_tiles;
                hull_tiles = BuildModeController.Instance.PreviewRemoveHull(start_x, end_x, start_y, end_y);
                BuildModeController.Instance.PlanRemoveHull(hull_tiles);

            }
            else if (isMode_Wall)
            {
                List<TileOWW> wall_tiles = BuildModeController.Instance.PreviewWall(start_x, end_x, start_y, end_y);
                BuildModeController.Instance.PlanFurniture(wall_tiles, BuildModeController.Instance.furnitureTypes["Wall"]);
            }
            else if (isMode_RemoveWall)
            {
                List<TileOWW> wall_tiles = BuildModeController.Instance.PreviewRemoveWall(start_x, end_x, start_y, end_y);
                BuildModeController.Instance.PlanRemoveFurniture(wall_tiles);
            }
            else if (isMode_Staff)
            {
                // Generate an unused staff name 
                int staffNumber = 0;
                bool staffNumberCollides = true;
                while (staffNumberCollides == true)
                {
                    staffNumber = UnityEngine.Random.Range(1, 999);
                    if (WorldController.Instance.staff.Count == 0)
                    {
                        staffNumberCollides = false;
                    }
                    foreach (GameObject staffGO in WorldController.Instance.staff)
                    {
                        if (staffGO.name.Contains(staffNumber.ToString()))
                        {
                            staffNumberCollides = true;
                            break;
                        }
                        staffNumberCollides = false;
                    }
                }

                string staffName = staff.tag + staffNumber.ToString();

                BuildModeController.Instance.PlaceStaff((int)currFramePosition.x, (int)currFramePosition.y, staff, staffName);
            }
            else if (isMode_Furniture)
            {
                TileOWW furniture_tile = BuildModeController.Instance.PreviewFurniture(furnitureTypeInUse, (int)currFramePosition.x, (int)currFramePosition.y);
                BuildModeController.Instance.PlanFurniture(furniture_tile, furnitureTypeInUse);
            }
            else if (isMode_FurnitureMulti)
            {
                List<TileOWW> furnitureTiles = BuildModeController.Instance.PreviewFurniture(furnitureTypeInUse, start_x, end_x, start_y, end_y);
                BuildModeController.Instance.PlanFurniture(furnitureTiles, furnitureTypeInUse);
            }
            else if (isMode_RemoveFurniture)
            {
                List<TileOWW> wall_tiles = BuildModeController.Instance.PreviewRemoveFurniture(start_x, end_x, start_y, end_y);
                BuildModeController.Instance.PlanRemoveFurniture(wall_tiles);
            }
            else if (isMode_Rooms)
            {
                List<TileOWW> roomTiles = BuildModeController.Instance.PreviewRoom(roomTypeInUse, start_x, end_x, start_y, end_y);
                BuildModeController.Instance.PlaceRoom(roomTiles, roomTypeInUse);
            }

            // audio 
            if (isMode_Plopable)
            {
                audio_Blueprint_PlopComplete.Play();
            }
            else if (isMode_Dragable)
            {
                audio_Blueprint_DragComplete.Play();
            }

            BuildModeController.Instance.ClearPreviews();
        }
    }

    /*
    public void UpdateBackgroundScale()
    {
        var osize = Camera.main.orthographicSize;
        if (orthoCurr != osize)
        {
            backgroundGO.transform.localScale = scaleOrg * osize / orthoOrg;
            orthoCurr = osize;
            backgroundGO.transform.position = Camera.main.ViewportToWorldPoint(posOrg);
        }
    }
    */ 

    public IEnumerator MoveCameraTo(float cameraPosX, float cameraPosY, float cameraZoom = -1)
    {
        if (cameraZoom == -1) cameraZoom = Camera.main.orthographicSize;

        Vector3 newCameraPos = new Vector3(cameraPosX, cameraPosY, cameraPosZ);
        float t = 0;
        while (t < 0.4f)
        {
            Camera.main.transform.position = Vector3.Lerp(newCameraPos, Camera.main.transform.position, 0.85f);
            Camera.main.orthographicSize = desiredCameraZoom = Mathf.Lerp(cameraZoom, Camera.main.orthographicSize, 0.85f);
            // Camera bounds 
            if (Camera.main.transform.position.x < cameraBoundMin + desiredCameraZoom) Camera.main.transform.position = new Vector3(cameraBoundMin + desiredCameraZoom, Camera.main.transform.position.y, cameraPosZ);
            else if (Camera.main.transform.position.x > cameraBoundMax - desiredCameraZoom) Camera.main.transform.position = new Vector3(cameraBoundMax - desiredCameraZoom, Camera.main.transform.position.y, cameraPosZ);

            // Not contained in the same block because x and y camera bounds could be breached at the same time. 
            if (Camera.main.transform.position.y < cameraBoundMin + desiredCameraZoom) Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, cameraBoundMin + desiredCameraZoom, cameraPosZ);
            else if (Camera.main.transform.position.y > cameraBoundMax - desiredCameraZoom) Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, cameraBoundMax - desiredCameraZoom, cameraPosZ);

            t += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        
    }


    
}
