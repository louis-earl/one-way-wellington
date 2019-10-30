using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class WorldController : MonoBehaviour
{
    public static WorldController Instance { get; protected set; }
    private World world;

    private List<GameObject> planets; // 0 = planet wellington

    public GameObject mapGO;
    public GameObject emptyPlanetPrefab;

    public List<GameObject> staff;
    // NOTE: Passengers are stored in journey controller 

    private string[] planetSuffixes = new string[] {" a", " b", " c", " d", " e", " f"};
    private string[] planetNames = new string[] {"Berenices", "Andromedae", "Herculis", "Cygni", "Delphini", "Arietis", "Draconis", "Majoris", "Eridani", "Pegasi", "Cancri", "Lyncis", "Ceti", "Aquarii", "Pictoris", "Tauri", "Leonis", "Tucanae", "Epsilon", "Cephei", "Librae" };



    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        // Create world
        world = new World(100, 100);  

        // Staff Lists 
        staff = new List<GameObject>();

    }

    private void Start()
    {
        // Create tile GameObjects
        TileSpriteController.Instance.InstantiateTiles(world);

        // Generate planets 
        GeneratePlanets(75);
        mapGO.SetActive(false);
    }


    public World GetWorld()
    {
        return world;
    }

    public void SetWorld(World world)
    {
        this.world = world;
        UpdateAllTiles();
    }

    private void UpdateAllTiles()
    {
        foreach (TileOWW tile in world.GetAllTiles())
        {
            // Build references to furniture locations
            if (tile.GetInstalledFurniture() != null)
            {
                if (!BuildModeController.Instance.furnitureTileOWWMap.ContainsKey(tile.GetInstalledFurniture().GetFurnitureType()))
                {
                    BuildModeController.Instance.furnitureTileOWWMap.Add(tile.GetInstalledFurniture().GetFurnitureType(), new List<TileOWW>());
                    BuildModeController.Instance.furnitureTileOWWMap[tile.GetInstalledFurniture().GetFurnitureType()].Add(tile);
                }
                else if (!BuildModeController.Instance.furnitureTileOWWMap[tile.GetInstalledFurniture().GetFurnitureType()].Contains(tile))
                {
                    BuildModeController.Instance.furnitureTileOWWMap[tile.GetInstalledFurniture().GetFurnitureType()].Add(tile);
                }
            }

            // Reset furniture to their default state
            if (tile.GetInstalledFurniture()?.GetFurnitureType() == "Airlock Open")
            {
                tile.SetInstalledFurniture(new InstalledFurniture("Airlock"));
            }
            else
            {              
                FurnitureSpriteController.Instance.UpdateFurniture(tile);
            }        

            TileSpriteController.Instance.UpdateTile(tile);
            JobSpriteController.Instance.UpdateJob(tile);
            RoomSpriteController.Instance.UpdateRoom(tile);
        }
    }

    public void NewGame()
    {
        SceneManager.LoadScene("Main");
    }

    // Generate n number of planets in the world 
    private void GeneratePlanets(int n)
    {

        planets = new List<GameObject>();
        planets.Add(InstantiatePlanet(new Vector2(0, 0), "Wellington"));
        JourneyController.Instance.planetWellington = planets[0].GetComponent<Planet>();

        for (int i = 1; i < n; i++)
        {
            bool hasPlaced = false;
            while (hasPlaced == false)
            {

                Vector2 temp = new Vector2(Random.Range(-600, 600), Random.Range(-400, 400));
                bool canPlace = true;
                for (int j = 0; j < i; j++)
                {
                    if (planets[j] != null)
                    {
                        if (Vector2.Distance(planets[j].GetComponent<Planet>().GetPlanetCoordinates(), temp) < 40)
                        {
                            canPlace = false;
                        }
                    }
                }
                if (canPlace)
                {


                    planets.Add(InstantiatePlanet(temp, GeneratePlanetName()));
                    hasPlaced = true;
                }
            }
        }
    }

    private string GeneratePlanetName()
    {
        // Ensures no name collisions 
        string nameAttempt = planetNames[Random.Range(0, planetNames.Length)] + planetSuffixes[Random.Range(0, planetSuffixes.Length)];

        bool nameAlreadyTaken = false;
        foreach (GameObject planetGO in planets)
        {
            if (planetGO.GetComponent<Planet>().GetPlanetName() == nameAttempt)
            {
                nameAlreadyTaken = true;
                break;
            }
        }
        if (nameAlreadyTaken)
        {
            return GeneratePlanetName();
        }
        return nameAttempt;
    }


    private GameObject InstantiatePlanet(Vector2 temp, string name)
    {
        GameObject planet = Instantiate(emptyPlanetPrefab);
        planet.transform.parent = mapGO.transform;
        planet.GetComponent<Planet>().planetCoordinates = temp;
        planet.transform.position = new Vector3(temp.x, temp.y, 1020);
        planet.name = name;
        planet.GetComponent<Planet>().SetPlanetName(name);


        // Initialise sprites

        int planetGraphicSuffix;
        if (name == "Wellington")
        {
            planetGraphicSuffix = 0;
            planet.GetComponent<Planet>().planetScale = 1.9f;


        }
        else
        {
            planetGraphicSuffix = Random.Range(1, 5);
            planet.GetComponent<Planet>().planetScale = Random.Range(1.4f, 2.2f);

        }
        planet.transform.localScale = new Vector3(planet.GetComponent<Planet>().planetScale, planet.GetComponent<Planet>().planetScale, 1);

        planet.GetComponent<Planet>().SetPlanetGraphicSuffix(planetGraphicSuffix);
        planet.GetComponent<Planet>().SetSurface(Resources.Load < Sprite > ("Images/Planets/Surfaces/Surface" + planetGraphicSuffix.ToString()));
        planet.GetComponent<Planet>().SetClouds(Resources.Load < Sprite > ("Images/Planets/Clouds/Cloud" + planetGraphicSuffix.ToString())); 

        return planet;
    }

    public List<GameObject> GetPlanets()
    {
        return planets;
    }

    public void InsertPlanets(List<PlanetData> planetData)
    {
        planets = new List<GameObject>();
        for (int i = 0; i < planetData.Count; i++)
        {
            GameObject planet = Instantiate(emptyPlanetPrefab);
            planet.transform.parent = mapGO.transform;
            planet.GetComponent<Planet>().planetCoordinates = new Vector2(planetData[i].planetPosX, planetData[i].planetPosY);
            planet.transform.position = new Vector3(planetData[i].planetPosX, planetData[i].planetPosY, 1020);
            planet.name = planetData[i].planetName;
            planet.GetComponent<Planet>().SetPlanetName(planetData[i].planetName);
            planet.GetComponent<Planet>().planetScale = planetData[i].planetScale;
            planet.transform.localScale = new Vector3(planet.GetComponent<Planet>().planetScale, planet.GetComponent<Planet>().planetScale, 1);


            // Initialise sprites
            planet.GetComponent<Planet>().SetSurface(Resources.Load<Sprite>("Images/Planets/Surfaces/Surface" + planetData[i].planetGraphic.ToString()));
            planet.GetComponent<Planet>().SetClouds(Resources.Load<Sprite>("Images/Planets/Clouds/Cloud" + planetData[i].planetGraphic.ToString()));

            planets.Add(planet);

        }
        for (int i = 0; i < planetData.Count; i++)
        {
            if (planetData[i].nextPlanet != null)
            {

                for (int j = 0; j < planets.Count; j++)
                {

                    if (planetData[i].nextPlanet.planetName == planets[j].GetComponent<Planet>().GetPlanetName())
                    {
                        planets[i].GetComponent<Planet>().nextPlanet = planets[j].GetComponent<Planet>();
                    }
                }
            }
        }
    }

    public void ClearAllStaff()
    {
        foreach (GameObject staffGO in staff) 
        {
            Destroy(staffGO);
        }
        staff.Clear();
    }

    public void ClearAllPlanets()
    {
        foreach (GameObject planetGO in planets)
        {
            Destroy(planetGO.GetComponent<Planet>().linkLine);
            Destroy(planetGO);
        }
        planets = new List<GameObject>();
    }
}
