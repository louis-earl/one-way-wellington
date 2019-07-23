using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


// Handle the creation of building jobs
public class BuildModeController : MonoBehaviour
{
    public static BuildModeController Instance;

    public GameObject tilemap;

    public GameObject validPreviewPrefab;
    public GameObject invalidPreviewPrefab;
    public GameObject removePreviewPrefab;

    public GameObject furniturePreviewPrefab;
    public GameObject hullParent;
    public GameObject furnitureParent;
    public GameObject utilityParent;

    List<GameObject> dragPreviewGameObjects;

    public Dictionary<string, List<TileOWW>> furnitureTileOWWMap;

    public Dictionary<string, FurnitureType> furnitureTypes;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        dragPreviewGameObjects = new List<GameObject>();

        InstantiateFurnitureTypes();
        furnitureTileOWWMap = new Dictionary<string, List<TileOWW>>(); 
    }


    // Create build jobs for Hull 
    public Tuple<List<TileOWW>, List<TileOWW>, List<TileOWW>> PreviewHull(int start_x, int end_x, int start_y, int end_y)
    {
        List<TileOWW> hull_tiles = new List<TileOWW>();
        List<TileOWW> wall_tiles = new List<TileOWW>();
        List<TileOWW> floor_tiles = new List<TileOWW>();

        ClearPreviews();

        int estimatedCost = 0;

        // Loop through all the tiles
        for (int x = start_x; x <= end_x; x++)
        {
            for (int y = start_y; y <= end_y; y++)
            {
                TileOWW t = WorldController.Instance.GetWorld().GetTileAt(x, y);

                if (t != null)
                {

                    // Check valid 
                    if (t.GetTileType() == "Empty") hull_tiles.Add(t);

                    // Check outlines
                    if (t.GetX() == start_x || t.GetX() == end_x || t.GetY() == start_y || t.GetY() == end_y)
                    {
                        // Check valid 
                        if (t.GetInstalledFurniture() == null)
                        {
                            wall_tiles.Add(t);
                            CreatePreview(validPreviewPrefab, "Wall", x, y);
                            estimatedCost += 250;
                        }
                        else CreatePreview(invalidPreviewPrefab, "Wall", x, y);
                    }
                    else
                    {
                        if (t.GetTileType() == "Empty")
                        {
                            floor_tiles.Add(t);
                            CreatePreview(validPreviewPrefab, "Hull", x, y);
                            estimatedCost += 200;
                        }
                        else CreatePreview(invalidPreviewPrefab, "Hull", x, y);
                    }

                }
            }
        }
        // Check funds 
        if (estimatedCost > CurrencyController.Instance.GetBankBalance())
        {
            // reset all previews invalid 
            ClearPreviews();
            for (int x = start_x; x <= end_x; x++)
            {
                for (int y = start_y; y <= end_y; y++)
                {
                    TileOWW t = WorldController.Instance.GetWorld().GetTileAt(x, y);
                    if (t != null)
                    {
                        // Check outlines
                        if (t.GetX() == start_x || t.GetX() == end_x || t.GetY() == start_y || t.GetY() == end_y)
                        {
                            CreatePreview(invalidPreviewPrefab, "Wall", x, y);
                        }
                        else
                        {
                            CreatePreview(invalidPreviewPrefab, "Hull", x, y);
                        }
                    }
                }
            }
            // Can NOT affoard this, so return NO valid tiles 
            return null;
        }
        // Can affoard this, so return valid tiles 
        else return Tuple.Create(hull_tiles, wall_tiles, floor_tiles);

    }

    public List<TileOWW> PreviewWall(int start_x, int end_x, int start_y, int end_y)
    {
        List<TileOWW> wall_tiles = new List<TileOWW>();

        ClearPreviews();

        int estimatedCost = 0;

        for (int x = start_x; x <= end_x; x++)
        {
            for (int y = start_y; y <= end_y; y++)
            {
                TileOWW t = WorldController.Instance.GetWorld().GetTileAt(x, y);

                if (t != null)
                {
                    // Check outlines
                    if (t.GetX() == start_x || t.GetX() == end_x || t.GetY() == start_y || t.GetY() == end_y)
                    {
                        // Check valid 
                        if (t.GetInstalledFurniture() == null && t.GetTileType() == "Hull")
                        {
                            wall_tiles.Add(t);
                            CreatePreview(validPreviewPrefab, "Wall", x, y);
                            estimatedCost += 50;
                        }
                        else CreatePreview(invalidPreviewPrefab, "Wall", x, y);
                    }
                }
            }
        }
        if (estimatedCost > CurrencyController.Instance.GetBankBalance())
        {
            ClearPreviews();
            for (int x = start_x; x <= end_x; x++)
            {
                for (int y = start_y; y <= end_y; y++)
                {
                    TileOWW t = WorldController.Instance.GetWorld().GetTileAt(x, y);

                    if (t != null)
                    {
                        // Check outlines
                        if (t.GetX() == start_x || t.GetX() == end_x || t.GetY() == start_y || t.GetY() == end_y)
                        {
                            CreatePreview(invalidPreviewPrefab, "Wall", x, y);
                        }
                    }
                }
            }
            return null;
        }
        else return wall_tiles;
    }

    public List<TileOWW> PreviewFurniture(FurnitureType furnitureType, int start_x, int end_x, int start_y, int end_y)
    {
        List<TileOWW> furniture_tiles = new List<TileOWW>();

        ClearPreviews();

        int estimatedCost = 0;

        for (int x = start_x; x <= end_x; x++)
        {
            for (int y = start_y; y <= end_y; y++)
            {
                TileOWW t = WorldController.Instance.GetWorld().GetTileAt(x, y);

                if (t != null)
                {

                    // Check valid 
                    if ((t.GetInstalledFurniture() == null && t.GetTileType() == "Hull" && !furnitureType.exteriorOnly) ||
                        (t.GetInstalledFurniture() == null && t.GetTileType() == "Empty" && furnitureType.exteriorOnly))
                    {
                        furniture_tiles.Add(t);
                        CreatePreview(validPreviewPrefab, furnitureType.title, x, y);
                        estimatedCost += furnitureType.cost;
                    }
                    else CreatePreview(invalidPreviewPrefab, furnitureType.title, x, y);

                }
            }
        }
        if (estimatedCost > CurrencyController.Instance.GetBankBalance())
        {
            ClearPreviews();

            for (int x = start_x; x <= end_x; x++)
            {
                for (int y = start_y; y <= end_y; y++)
                {
                    TileOWW t = WorldController.Instance.GetWorld().GetTileAt(x, y);

                    if (t != null)
                    {

                        CreatePreview(invalidPreviewPrefab, furnitureType.title, x, y);

                    }
                }
            }
            return null;
        }
        else return furniture_tiles;
    }

    public TileOWW PreviewFurniture(FurnitureType furnitureType, int posX, int posY)
    {

        ClearPreviews();


        TileOWW furnitureTile = WorldController.Instance.GetWorld().GetTileAt(posX, posY);
        TileOWW furnitureTileReturn = null;

        if (furnitureTile != null)
        {

            // Check valid 
            if (((furnitureTile.GetInstalledFurniture() == null && furnitureTile.GetTileType() == "Hull" && !furnitureType.exteriorOnly) ||
                (furnitureTile.GetInstalledFurniture() == null && furnitureTile.GetTileType() == "Empty" && furnitureType.exteriorOnly)) &&
                furnitureType.cost < CurrencyController.Instance.GetBankBalance())
            {
                CreatePreview(validPreviewPrefab, furnitureType.title, posX, posY);
                furnitureTileReturn = furnitureTile;
            }
            else CreatePreview(invalidPreviewPrefab, furnitureType.title, posX, posY);

        }

        return furnitureTileReturn;
    }

    public List<TileOWW> PreviewRemoveHull(int start_x, int end_x, int start_y, int end_y)
    {
        List<TileOWW> hull_tiles = new List<TileOWW>();

        ClearPreviews();

        // Loop through all the tiles
        for (int x = start_x; x <= end_x; x++)
        {
            for (int y = start_y; y <= end_y; y++)
            {
                TileOWW t = WorldController.Instance.GetWorld().GetTileAt(x, y);

                if (t != null)
                {
                    // Check valid 
                    if (t.GetTileType() == "Hull")
                    {
                        hull_tiles.Add(t);
                        CreatePreview(removePreviewPrefab, "Hull", x, y);
                    }
                }
            }
        }
        return hull_tiles;
    }

    public List<TileOWW> PreviewRemoveWall(int start_x, int end_x, int start_y, int end_y)
    {
        List<TileOWW> wall_tiles = new List<TileOWW>();

        ClearPreviews();

        // Loop through all the tiles
        for (int x = start_x; x <= end_x; x++)
        {
            for (int y = start_y; y <= end_y; y++)
            {
                TileOWW t = WorldController.Instance.GetWorld().GetTileAt(x, y);

                if (t != null)
                {
                    // Check valid 
                    if (t.GetInstalledFurniture() != null)
                    {
                        if (t.GetInstalledFurniture().GetFurnitureType() == "Wall")
                        {
                            wall_tiles.Add(t);
                            CreatePreview(removePreviewPrefab, "Wall", x, y);
                        }
                    }
                }
            }
        }
        return wall_tiles;
    }

    public List<TileOWW> PreviewRoom(string roomType, int start_x, int end_x, int start_y, int end_y)
    {
        List<TileOWW> room_tiles = new List<TileOWW>();

        ClearPreviews();

        for (int x = start_x; x <= end_x; x++)
        {
            for (int y = start_y; y <= end_y; y++)
            {
                TileOWW t = WorldController.Instance.GetWorld().GetTileAt(x, y);

                if (t != null)
                {

                    // Check valid 
                    if (t.GetInstalledFurniture()?.GetFurnitureType() != "Wall" && t.GetTileType() == "Hull")
                    {
                        room_tiles.Add(t);
                        CreatePreview(validPreviewPrefab, "Hull", x, y); // TODO: Different looks per room?
                    }
                    else CreatePreview(invalidPreviewPrefab, "Hull", x, y); // TODO: Different looks per room?

                }
            }
        }
        
        return room_tiles;
    }

    public void PreviewStaff(string staffType, int posX, int posY)
    {
        ClearPreviews();

        TileOWW staffTile = WorldController.Instance.GetWorld().GetTileAt(posX, posY);

        if (CurrencyController.Instance.GetBankBalance() >= 500)
        {
            CreatePreview(validPreviewPrefab, staffType, posX, posY);
        }
        else
        {
            CreatePreview(invalidPreviewPrefab, staffType, posX, posY);
        }
    }

    public void PlanHull(List<TileOWW> hull_tiles, List<TileOWW> furniture_tiles, List<TileOWW> floor_tiles)
    {
        foreach (TileOWW tile in floor_tiles)
        {
            // We only want to plan a hull tile if nothing else is already planned 
            if (tile.currentJobType == null)
            {
                Action placeHullAction = delegate () { PlaceHull(tile); };
                Job j = new Job(placeHullAction, tile, 3, "Hull");
                tile.currentJobType = j.GetJobType();
                JobQueueController.BuildersJobQueue.AddJob(j);

                // Invoice 
                CurrencyController.Instance.ChangeBankBalance(-200);
            }
            else Debug.Log("There's already a job here! Remove it first. " + tile.ToString());
        }
        foreach (TileOWW tile in furniture_tiles)
        {
            if (tile.currentJobType == null)
            {
                Action placeFurnitureAction = delegate () { PlaceFurniture(tile, "Wall"); };
                Action placeHullAction = delegate () { PlaceHull(tile); };

                // This wall job has a prerequisite job, which is to build the hull first.
                Job j = new Job(placeFurnitureAction, tile, 3, "Wall", new Job(placeHullAction, tile, 3, "Hull"));
                tile.currentJobType = j.GetJobType();
                JobQueueController.BuildersJobQueue.AddJob(j);

                // Invoice 
                CurrencyController.Instance.ChangeBankBalance(-250);

            }
            else Debug.Log("There's already a job here! Remove it first. " + tile.ToString());
        }
    }

    // Overloaded method for planing multiple furniture jobs at once (e.g. walls, tanks, batteries, etc.)
    public void PlanFurniture(List<TileOWW> furniture_tiles, FurnitureType furnitureType)
    {
        foreach (TileOWW tile in furniture_tiles)
        {
            if (tile.currentJobType == null)
            {
                Action placeFurnitureAction = delegate () { PlaceFurniture(tile, furnitureType.title); };
                Job j = new Job(placeFurnitureAction, tile, furnitureType.installTime, furnitureType.title);
                tile.currentJobType = j.GetJobType();
                JobQueueController.BuildersJobQueue.AddJob(j);

                CurrencyController.Instance.ChangeBankBalance(-furnitureType.cost);

            }
            else Debug.Log("There's already a job here! Remove it first. " + tile.ToString());
        }
    }

    // Overloaded method for planing single furniture jobs at once
    public void PlanFurniture(TileOWW furniture_tile, FurnitureType furnitureType)
    {
        if (furniture_tile != null)
        {
            if (furniture_tile.currentJobType == null)
            {
                Action placeFurnitureAction = delegate () { PlaceFurniture(furniture_tile, furnitureType.title); };
                Job j = new Job(placeFurnitureAction, furniture_tile, furnitureType.installTime, furnitureType.title);
                furniture_tile.currentJobType = j.GetJobType();
                JobQueueController.BuildersJobQueue.AddJob(j);

                CurrencyController.Instance.ChangeBankBalance(-furnitureType.cost);
            }
            else Debug.Log("There's already a job here! Remove it first. " + furniture_tile.ToString());

        }
    }

    public void PlanRemoveHull(List<TileOWW> hull_tiles)
    {
        foreach (TileOWW tile in hull_tiles)
        {
            if (tile.currentJobType == null)
            {
                Action removeHullAction = delegate () { RemoveHull(tile); };

                // Make a prerequisite job to remove furniture first!
                if (tile.GetInstalledFurniture() != null)
                {
                    Action removeFurnitureAction = delegate () { RemoveFurniture(tile); };
                    Job j = new Job(removeHullAction, tile, 2, "removeHull", new Job(removeFurnitureAction, tile, 2, "removeFurniture"));
                    tile.currentJobType = j.GetJobType();
                    JobQueueController.BuildersJobQueue.AddJob(j);
                }
                else
                {
                    Job j = new Job(removeHullAction, tile, 2, "removeHull");
                    tile.currentJobType = j.GetJobType();
                    JobQueueController.BuildersJobQueue.AddJob(j);
                }
            }
        }
    }

    public void PlanRemoveFurniture(List<TileOWW> furniture_tiles)
    {
        foreach (TileOWW tile in furniture_tiles)
        {
            if (tile.currentJobType == null)
            {
                Action removeFurnitureAction = delegate () { RemoveFurniture(tile); };
                Job j = new Job(removeFurnitureAction, tile, 2, "removeFurniture");
                tile.currentJobType = j.GetJobType();
                JobQueueController.BuildersJobQueue.AddJob(j);
            }
        }

    }

    public void PlaceHull(TileOWW tile)
    {
        tile.SetTileType("Hull");
    }

    public void RemoveHull(TileOWW tile)
    {
        tile.SetTileType("Empty");
    }

    public void PlaceFurniture(TileOWW tile, string furnitureType)
    {
        tile.SetInstalledFurniture(new InstalledFurniture(furnitureType));

        // Add to map of furnitureType TileOWW
        if (!furnitureTileOWWMap.ContainsKey(furnitureType))
        {
            furnitureTileOWWMap.Add(furnitureType, new List<TileOWW>());
        }
        furnitureTileOWWMap[furnitureType].Add(tile);

        // If engines built, update ship speed 
        // TODO: Different types of engines 
        if (furnitureType == "Engine")
        {
            // TODO: Different speeds per engine 
            JourneyController.Instance.shipSpeedMax = furnitureTileOWWMap[furnitureType].Count * 5;
        }
    }

    public void RemoveFurniture(TileOWW tile)
    {
        string furnitureType = tile.GetInstalledFurniture().GetFurnitureType();
        tile.RemoveInstalledFurniture();

        // Remove from map of furnitureType TileOWW
        furnitureTileOWWMap[furnitureType].Remove(tile);
    }

    public void PlaceRoom(TileOWW tile, string roomType)
    {
        // set the TileOWW's room type
        // uhhhh
    }

    public void PlaceStaff(float x, float y, GameObject staff, float energy = 100, float health = 100)
    {
        if (CurrencyController.Instance.GetBankBalance() >= 500)
        {
            GameObject staffGO = Instantiate(staff, new Vector3(x, y, 0), Quaternion.identity);
            staffGO.name = "Builder";
            staffGO.GetComponent<Staff>().SetEnergy(energy);
            staffGO.GetComponent<Staff>().SetHealth(health);

            WorldController.Instance.staff.Add(staffGO);

            // TODO: Invoice depend on staff type
            CurrencyController.Instance.ChangeBankBalance(-500);
        }
    }

    private void CreatePreview(GameObject prefab, string preview, int x, int y)
    {
        GameObject go = SimplePool.Spawn(prefab, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity);
        if (go != null)
        {
            go.transform.SetParent(this.transform, true);
            go.GetComponent<SpriteRenderer>().sortingOrder = 25;
            go.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/Furniture/" + preview);
            dragPreviewGameObjects.Add(go);
        }
    }

    public void ClearPreviews()
    {
        // Clean up old drag previews
        while (dragPreviewGameObjects.Count > 0)
        {
            GameObject go = dragPreviewGameObjects[0];
            dragPreviewGameObjects.RemoveAt(0);
            SimplePool.Despawn(go);
        }
    }

    private void InstantiateGridTiles(World world)
    {
        foreach (TileOWW tile in world.GetTiles())
        {
            UnityEngine.Tilemaps.Tile t = ScriptableObject.CreateInstance<UnityEngine.Tilemaps.Tile>();
            t.sprite = TileType.Grid;
            t.name = tile.GetX() + "_" + tile.GetY();
            tilemap.GetComponent<Tilemap>().SetTile(new Vector3Int(tile.GetX(), tile.GetY(), 0), t);
        }
    }

    public void SetGridVisible(bool isVisible)
    {
        if (tilemap.GetComponent<Tilemap>().GetUsedTilesCount() == 0) InstantiateGridTiles(WorldController.Instance.GetWorld());
        tilemap.SetActive(isVisible);
    }

    private void InstantiateFurnitureTypes()
    {
        furnitureTypes = FurnitureType.InstantiateFurnitureTypes();
        foreach (KeyValuePair<string, FurnitureType> furnitureEntry in furnitureTypes)
        {
            if (furnitureEntry.Value.category == FurnitureType.BuildCategory.Furniture)
            {
                GameObject furnitureEntryGO = Instantiate(furniturePreviewPrefab, furnitureParent.transform);
                furnitureEntryGO.GetComponent<FurnitureListing>().InputFurnitureType(furnitureEntry.Value);
            }
            else if (furnitureEntry.Value.category == FurnitureType.BuildCategory.Utility)
            {
                GameObject furnitureEntryGO = Instantiate(furniturePreviewPrefab, utilityParent.transform);
                furnitureEntryGO.GetComponent<FurnitureListing>().InputFurnitureType(furnitureEntry.Value);
            }
            else if (furnitureEntry.Value.category == FurnitureType.BuildCategory.Hull) 
                    {
                GameObject furnitureEntryGO = Instantiate(furniturePreviewPrefab, hullParent.transform);
                furnitureEntryGO.GetComponent<FurnitureListing>().InputFurnitureType(furnitureEntry.Value);
            }
           
        }
    }

}
