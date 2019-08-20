
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


// Game save and load operations 

public class PersistenceController : MonoBehaviour
{

    public static PersistenceController Instance;


    public GameObject BuilderPrefab;
    public GameObject GuardPrefab;
    public GameObject PassengerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null) Instance = this;
    }

    public void SaveGame(string saveName)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fileStream = File.Create(Application.persistentDataPath + "/" + saveName);
        SaveFile saveFile = new SaveFile();

        // SAVE
        saveFile.bankBalance = CurrencyController.Instance.GetBankBalance();
        saveFile.timeOWW = TimeController.Instance.timeOWW;
        saveFile.world = WorldController.Instance.GetWorld();

        // STAFF
        int numberOfStaff = WorldController.Instance.staff.Count;

        saveFile.staffTypes = new string[numberOfStaff];
        for (int i = 0; i < numberOfStaff; i++)
        {
            saveFile.staffTypes[i] = WorldController.Instance.staff[i].tag;
        }

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
            if (staffGO.name == "Builder")
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
            else if (staffGO.name == "Guard")
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

    public void LoadGame(string saveName)
    {
        if (File.Exists(Application.persistentDataPath + "/" + saveName))
        {

            BinaryFormatter bf = new BinaryFormatter();
            FileStream fileStream = File.Open(Application.persistentDataPath + "/" + saveName, FileMode.Open);
            SaveFile saveFile = (SaveFile)bf.Deserialize(fileStream);

            // UNLOAD 
            JobQueueController.Instance.ClearAllJobs();

            WorldController.Instance.ClearAllStaff();
            WorldController.Instance.ClearAllPlanets();

            BuildModeController.Instance.furnitureTileOWWMap.Clear();
            BuildModeController.Instance.emptyHullTiles = new List<TileOWW>();

            JourneyController.Instance.shipCoordinates = Vector2.zero;


            // LOAD
            CurrencyController.Instance.SetBankBalance(saveFile.bankBalance);
            TimeController.Instance.timeOWW = saveFile.timeOWW;
            WorldController.Instance.SetWorld(saveFile.world);

            // Find all current hull tiles 
            foreach (TileOWW tile in WorldController.Instance.GetWorld().GetAllTiles())
            {
                if (tile.GetTileType() == "Hull")
                {
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
            JourneyController.Instance.earth = WorldController.Instance.GetPlanets()[0].GetComponent<Planet>();


            // SHIP SPEED
            if (BuildModeController.Instance.furnitureTileOWWMap.ContainsKey("Engine"))
            {
                JourneyController.Instance.shipSpeedMax = BuildModeController.Instance.furnitureTileOWWMap["Engine"].Count * 5;
            }

            fileStream.Close();
        }
    }

    
}
