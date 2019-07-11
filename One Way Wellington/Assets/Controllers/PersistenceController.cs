
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
            saveFile.staffTypes[i] = WorldController.Instance.staff[i].name;
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
            if (staffGO.name == "Builder")
            {
                if (staffGO.GetComponent<Builder>().targetJob != null)
                {
                    if (staffGO.GetComponent<Builder>().targetJob.ToJobSerializable() != null)
                    {
                        saveFile.buildersJobQueue.Add(staffGO.GetComponent<Builder>().targetJob.ToJobSerializable());
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

        // JOURNEY
        saveFile.shipPosX = JourneyController.Instance.shipCoordinates.x;
        saveFile.shipPosY = JourneyController.Instance.shipCoordinates.y;


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



            // LOAD
            CurrencyController.Instance.SetBankBalance(saveFile.bankBalance);
            TimeController.Instance.timeOWW = saveFile.timeOWW;
            WorldController.Instance.SetWorld(saveFile.world);


            // STAFF
            for (int i = 0; i < saveFile.staffTypes.Length; i++)
            {
                if (saveFile.staffTypes[i] == "Builder")
                {
                    BuildModeController.Instance.PlaceStaff(saveFile.staffPosX[i],
                        saveFile.staffPosY[i], BuilderPrefab,
                        saveFile.staffEnergy[i], saveFile.staffHealth[i]);

                }
                else
                {
                    Debug.LogWarning("Staff type not specified in PersistenceController!");
                }


            }


            // JOBS
            JobQueueController.BuildersJobQueue.SetJobsFromSerialized(saveFile.buildersJobQueue);
            JobQueueController.GuardsJobQueue.SetJobsFromSerialized(saveFile.guardsJobQueue);
            JobQueueController.PassengersJobQueue.SetJobsFromSerialized(saveFile.passengersJobQueue);


            // PLANETS
            WorldController.Instance.InsertPlanets(saveFile.planets);
            JourneyController.Instance.earth = WorldController.Instance.GetPlanets()[0].GetComponent<Planet>();

            // Journey
            JourneyController.Instance.fuelRemaining = BuildModeController.Instance.furnitureTileOWWMap["Fuel Tank"].Count * 10;

            JourneyController.Instance.shipCoordinates = new Vector2(saveFile.shipPosX, saveFile.shipPosY);
            JourneyController.Instance.FixPlanetLinks(JourneyController.Instance.earth);

            fileStream.Close();
        }
    }

    
}
