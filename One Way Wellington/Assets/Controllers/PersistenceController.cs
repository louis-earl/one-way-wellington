﻿
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


// Game save and load operations 

public class PersistenceController : MonoBehaviour
{

    public static PersistenceController Instance;


    public GameObject BuilderPrefab;
    public GameObject GuardPrefab;
    public GameObject PassengerPrefab;

    public Image blackScreen;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null) Instance = this;

        blackScreen.gameObject.SetActive(true);
        blackScreen.enabled = true;
        blackScreen.color = Color.black;

        if (SceneManager.GetActiveScene().name == "MainLoad")
        {
            StartCoroutine(LoadShipAfterSceneLoad());
        }
        else
        {

            StartCoroutine(FadeIntoScene());

        }
    }

    private bool IsAllControllersLoaded ()
    {
        if (BuildModeController.Instance != null
            && CurrencyController.Instance != null
            && EnemyController.Instance != null
            && InputController.Instance != null
            && JobQueueController.Instance != null
            && JourneyController.Instance != null
            && NotificationController.Instance != null
            && ObjectiveController.Instance != null
            && OxygenController.Instance != null
            && RoomController.Instance != null
            && TileSpriteController.Instance != null
            && FurnitureSpriteController.Instance != null
            && JobSpriteController.Instance != null
            && RoomSpriteController.Instance != null
            && TimeController.Instance != null
            && TransitionController.Instance != null
            && UserInterfaceController.Instance != null
            && WorldController.Instance != null)
        {
            return true;
        }
        return false;
    }

    private IEnumerator LoadShipAfterSceneLoad()
    {
        bool isSceneLoaded = false;
        while (isSceneLoaded == false)
        {
            if (IsAllControllersLoaded())
            {
                LoadGame("save1");
                isSceneLoaded = true;
                StartCoroutine(FadeIntoScene());
            }
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    private IEnumerator FadeIntoScene()
    {
        bool isControllersLoaded = false;
        while (isControllersLoaded == false)
        {
            if (IsAllControllersLoaded())
            {
                isControllersLoaded = true;
            }
            yield return new WaitForEndOfFrame();
        }

        while (blackScreen.color.a >= 0.03)
        {
            blackScreen.color = new Color(0, 0, 0, (blackScreen.color.a - Time.deltaTime));
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    public void SaveGame(string saveName)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fileStream = File.Create(Application.persistentDataPath + "/" + saveName);
        SaveFile saveFile = new SaveFile();

        // SAVE
        saveFile.bankBalance = CurrencyController.Instance.GetBankBalance();
        saveFile.timeOWW = 0;
        saveFile.world = WorldController.Instance.GetWorld();

        // STAFF
        int numberOfStaff = WorldController.Instance.staff.Count;

        saveFile.staffTypes = new string[numberOfStaff];
        for (int i = 0; i < numberOfStaff; i++)
        {
            saveFile.staffTypes[i] = WorldController.Instance.staff[i].tag;
        }

        saveFile.staffNames = new string[numberOfStaff];
        for (int i = 0; i < numberOfStaff; i++)
        {
            saveFile.staffNames[i] = WorldController.Instance.staff[i].name;
        }

        saveFile.staffPosX = new float[numberOfStaff];
        for (int i = 0; i < numberOfStaff; i++)
        {
            saveFile.staffPosX[i] = WorldController.Instance.staff[i].GetComponent<Character>().GetXPos();
        }

        saveFile.staffPosY = new float[numberOfStaff];
        for (int i = 0; i < numberOfStaff; i++)
        {
            saveFile.staffPosY[i] = WorldController.Instance.staff[i].GetComponent<Character>().GetYPos();
        }

        saveFile.staffEnergy = new float[numberOfStaff];
        for (int i = 0; i < numberOfStaff; i++)
        {
            saveFile.staffEnergy[i] = WorldController.Instance.staff[i].GetComponent<Staff>().GetEnergy();
        }

        saveFile.staffHealth = new float[numberOfStaff];
        for (int i = 0; i < numberOfStaff; i++)
        {
            saveFile.staffHealth[i] = WorldController.Instance.staff[i].GetComponent<Staff>().GetHealth();
        }

        

        // JOBS
        saveFile.buildersJobQueue = JobQueueController.BuildersJobQueue.GetSerializedJobs();
        saveFile.guardsJobQueue = JobQueueController.GuardsJobQueue.GetSerializedJobs();
        saveFile.passengersJobQueue = JobQueueController.PassengersJobQueue.GetSerializedJobs();

        foreach (GameObject staffGO in WorldController.Instance.staff)
        {
            Job targetJob;
            if (staffGO.tag == "Builder")
            {
                targetJob = staffGO.GetComponent<Builder>().targetJob;
                if (targetJob?.ToJobSerializable() != null)
                {
                    if (targetJob.tileExcludeOtherJobs)
                    {
                        saveFile.buildersJobQueue.Add(targetJob.ToJobSerializable());
                    }
                }
            }
            else if (staffGO.tag == "Guard")
            {
                targetJob = staffGO.GetComponent<Guard>().targetJob;
                if (targetJob?.ToJobSerializable() != null)
                {
                    if (targetJob.tileExcludeOtherJobs)
                    {
                        saveFile.guardsJobQueue.Add(targetJob.ToJobSerializable());
                    }
                }
            }
            else if (staffGO.tag == "Captain")
            {
                // Do nothing 
            }
            else
            {
                Debug.LogError("Staff type not specified!!");
            }

        }
          
            
        

        // PLANETS
        saveFile.planets = new List<PlanetData>();
        foreach (GameObject planetGO in WorldController.Instance.GetPlanets())
        {
            Planet planet = planetGO.GetComponent<Planet>();
            PlanetData planetData = planet.ToPlanetData();
            saveFile.planets.Add(planetData);
        }


        bf.Serialize(fileStream, saveFile);
        fileStream.Close();
    }


    public void GoToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Start");

    }

    public void LoadNewScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainLoad");
    }

    public void NewScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainNew");

    }

    public void UnloadGame()
    {
        JobQueueController.Instance.ClearAllJobs();
        WorldController.Instance.ClearAllStaff();
        WorldController.Instance.ClearAllPlanets();
        BuildModeController.Instance.furnitureTileOWWMap.Clear();
        BuildModeController.Instance.emptyHullTiles = new List<TileOWW>();
        BuildModeController.Instance.allHullTiles = new List<TileOWW>();
        JourneyController.Instance.shipCoordinates = Vector2.zero;

        WorldController.Instance.SetWorld(new World(100, 100));


        // DEBUG ONLY 
        // Update all sprites 
        FurnitureSpriteController.Instance.UpdateAllFurniture();
        JobSpriteController.Instance.UpdateAllJob();
        RoomSpriteController.Instance.UpdateAllRoom();
        TileSpriteController.Instance.UpdateAllTile();
    }


    public void LoadGame(string saveName)
    {
        if (File.Exists(Application.persistentDataPath + "/" + saveName))
        {

            BinaryFormatter bf = new BinaryFormatter();
            FileStream fileStream = File.Open(Application.persistentDataPath + "/" + saveName, FileMode.Open);
            SaveFile saveFile = (SaveFile)bf.Deserialize(fileStream);

            // UNLOAD 
            UnloadGame();


            // LOAD
            CurrencyController.Instance.SetBankBalance(saveFile.bankBalance);
            TimeController.Instance.timeOWW = saveFile.timeOWW;
            WorldController.Instance.SetWorld(saveFile.world);

            // Find all current hull tiles 
            foreach (TileOWW tile in WorldController.Instance.GetWorld().GetAllTiles())
            {
                if (tile.GetTileType() == "Hull")
                {
                    BuildModeController.Instance.allHullTiles.Add(tile);

                    if (tile.GetInstalledFurniture() == null && tile.installedFurnitureAltX == null && tile.installedFurnitureAltY == null)
                    {
                        BuildModeController.Instance.emptyHullTiles.Add(tile);
                    }
                }
            }
            

            // STAFF
            for (int i = 0; i < saveFile.staffTypes.Length; i++)
            {
                if (saveFile.staffTypes[i] == "Builder")
                {
                    BuildModeController.Instance.PlaceStaff(saveFile.staffPosX[i],
                        saveFile.staffPosY[i], BuilderPrefab, saveFile.staffNames[i],
                        saveFile.staffEnergy[i], saveFile.staffHealth[i]);

                }
                else if (saveFile.staffTypes[i] == "Guard")
                {
                    BuildModeController.Instance.PlaceStaff(saveFile.staffPosX[i],
                        saveFile.staffPosY[i], GuardPrefab, saveFile.staffNames[i],
                        saveFile.staffEnergy[i], saveFile.staffHealth[i]);

                }
                else
                {
                    Debug.LogWarning("Staff type (" + saveFile.staffTypes[i] + ") not specified in PersistenceController!");
                }


            }


            // JOBS
            JobQueueController.BuildersJobQueue.SetJobsFromSerialized(saveFile.buildersJobQueue);
            JobQueueController.GuardsJobQueue.SetJobsFromSerialized(saveFile.guardsJobQueue);
            JobQueueController.PassengersJobQueue.SetJobsFromSerialized(saveFile.passengersJobQueue);


            // PLANETS
            WorldController.Instance.InsertPlanets(saveFile.planets);
            JourneyController.Instance.planetWellington = WorldController.Instance.GetPlanets()[0].GetComponent<Planet>();


            // SHIP SPEED
            if (BuildModeController.Instance.furnitureTileOWWMap.ContainsKey("Engine"))
            {
                JourneyController.Instance.shipSpeedMax = BuildModeController.Instance.furnitureTileOWWMap["Engine"].Count * 5;
            }

            fileStream.Close();
        }
    }

    
}
