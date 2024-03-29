﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;


// Handle the creation of building jobs
public class BuildModeController : MonoBehaviour
{
    public static BuildModeController Instance;

    public GameObject tilemap;
    public GameObject roomsTilemap;

    public GameObject validPreviewPrefab;
    public GameObject invalidPreviewPrefab;
    public GameObject removePreviewPrefab;

    public GameObject furniturePreviewPrefab;

    // Content groups 
    public GameObject hullListingParent;
    public GameObject wallListingParent;
    public GameObject utilityListingParent;
    public GameObject furnitureListingParent;
    public GameObject roomsListingParent;
    public GameObject staffListingParent;

    List<GameObject> dragPreviewGameObjects;

    public Dictionary<string, List<TileOWW>> furnitureTileOWWMap;
    public Dictionary<string, FurnitureType> furnitureTypes;

    public Dictionary<string, List<TileOWW>> roomsTileOWWMap;

    public List<TileOWW> emptyHullTiles;
    public List<TileOWW> allHullTiles;

    public GameObject staffParent;

    // Static values 
    private static readonly int HULL_COST = 300;
    private static readonly int WALL_COST = 100;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        dragPreviewGameObjects = new List<GameObject>();

        InstantiateFurnitureTypes();
        furnitureTileOWWMap = new Dictionary<string, List<TileOWW>>();

        roomsTileOWWMap = new Dictionary<string, List<TileOWW>>();

        roomsTilemap.SetActive(false);
        // SetGridVisible(true);
    }

    // Create build jobs for Hull 
    public Tuple<List<TileOWW>, List<TileOWW>, List<TileOWW>> PreviewHull(int start_x, int end_x, int start_y, int end_y, bool includeWalls)
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
                    // if (t.GetTileType() == "Empty") hull_tiles.Add(t);

                    // Check outlines if including walls 
                    if (includeWalls && (t.GetX() == start_x || t.GetX() == end_x || t.GetY() == start_y || t.GetY() == end_y))
                    {
                        // Check valid 
                        if (t.GetInstalledFurniture() == null && 
                            t.installedFurnitureAltX == null && // not apart of a multi-tile furniture
                            t.installedFurnitureAltY == null)
                        {
                            wall_tiles.Add(t);
                            CreatePreview(validPreviewPrefab, "Wall", x, y);
                            estimatedCost += WALL_COST;
                            estimatedCost += HULL_COST;
                        }
                        else CreatePreview(invalidPreviewPrefab, "Wall", x, y);
                    }
                    else
                    {
                        if (t.GetTileType() == "Empty")
                        {
                            floor_tiles.Add(t);
                            CreatePreview(validPreviewPrefab, "Hull", x, y);
                            estimatedCost += HULL_COST;
                        }
                        else CreatePreview(invalidPreviewPrefab, "Hull", x, y);
                    }

                }
            }
        }

        // Consider materials already in stock 
        /*
        if (CargoController.Instance.unusedShipStock.ContainsKey("Hull")) {
            estimatedCost -= (CargoController.Instance.unusedShipStock["Hull"] * HULL_COST);
        }

        if (CargoController.Instance.unusedShipStock.ContainsKey("Wall"))
        {
            estimatedCost -= (CargoController.Instance.unusedShipStock["Wall"] * WALL_COST);
        }
        */

        // Display price UI
        InputController.Instance.currentBuildPrice = estimatedCost;

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
                        if (includeWalls && (t.GetX() == start_x || t.GetX() == end_x || t.GetY() == start_y || t.GetY() == end_y))
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
                        if (t.GetInstalledFurniture() == null && t.GetTileType() == "Hull"  && 
                            t.installedFurnitureAltX == null && // not apart of a multi-tile furniture
                            t.installedFurnitureAltY == null)
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

        // Display price UI
        InputController.Instance.currentBuildPrice = estimatedCost;

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


    public bool CheckFurniturePreviewValid(TileOWW tile, FurnitureType furnitureType)
    {    
        if (furnitureType.cost > CurrencyController.Instance.GetBankBalance())
        {
            // Debug.Log("Couldn't affoard.");
            return false;
        }

        // Check all tiles of multi-tile furniture 
        for (int i = 0; i < furnitureType.sizeX; i++)
        {
            for (int j = 0; j < furnitureType.sizeY; j++)
            {
                TileOWW t = WorldController.Instance.GetWorld().GetTileAt(tile.GetX() + i, tile.GetY() + j);
                if ((t.GetTileType() == "Hull" && furnitureType.exteriorOnly) || (t.GetTileType() == "Empty" && !furnitureType.exteriorOnly))
                {
                    // Debug.Log("Interior / exterior check failed.");
                    return false;
                }
                if (t.installedFurnitureAltX != null || t.installedFurnitureAltY != null)
                {
                    //Allow certain furniture to overwrite others 
                    if (t.GetInstalledFurniture()?.GetFurnitureType() == "Wall" && furnitureType.title == "Airlock")
                    {
                        return true;
                    }
                    // Debug.Log("Multi-tile furniture check failed.");
                    return false;
                }
                if (t.GetInstalledFurniture() != null)
                {
                    //Allow certain furniture to overwrite others 
                    if (t.GetInstalledFurniture().GetFurnitureType() == "Wall" && furnitureType.title == "Airlock")
                    {
                        return true;
                    }
                    // Debug.Log("Installed furniture check failed");
                    return false;
                }
                if (t.currentJobType != null)
                {
                    // Debug.Log("Existing job check failed.");
                    return false;
                }
            }
        }
        
        // All checks passed, furniture is valid.
        return true;
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
                    if (CheckFurniturePreviewValid(t, furnitureType))
                    {
                        furniture_tiles.Add(t);
                        CreatePreview(validPreviewPrefab, furnitureType.title, x, y);
                        estimatedCost += furnitureType.cost;
                    }
                    else CreatePreview(invalidPreviewPrefab, furnitureType.title, x, y);

                }
            }
        }
        // Display price UI
        InputController.Instance.currentBuildPrice = estimatedCost;

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
            if (CheckFurniturePreviewValid(furnitureTile, furnitureType))
            {
                CreatePreview(validPreviewPrefab, furnitureType.title, posX, posY);
                furnitureTileReturn = furnitureTile;
            }
            else CreatePreview(invalidPreviewPrefab, furnitureType.title, posX, posY);

        }
        // Display price UI
        InputController.Instance.currentBuildPrice = furnitureType.cost;

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
                TileOWW tileOWW = WorldController.Instance.GetWorld().GetTileAt(x, y);

                // Check valid - remove installed hull OR job
                if (tileOWW != null)
                {
                    if (tileOWW.GetTileType() == "Hull")
                    {
                        hull_tiles.Add(tileOWW);
                        CreatePreview(removePreviewPrefab, "Hull", x, y);
                    }
                    // Can remove any build job 
                    else if (tileOWW.currentJobType != null)
                    {
                        if (tileOWW.currentJobType.Contains("Build"))
                        {
                            hull_tiles.Add(tileOWW);
                            CreatePreview(removePreviewPrefab, "Hull", x, y);
                        }
                    }
                }
            }
        }
        return hull_tiles;
    }

    public TileOWW PreviewRemoveFurniture(int posX, int posY)
    {
        ClearPreviews();

        TileOWW tileOWW = WorldController.Instance.GetWorld().GetTileAt(posX, posY);

        // Check valid 
        if (tileOWW?.GetInstalledFurniture()?.GetFurnitureType() == "Wall" || tileOWW?.currentJobType == "Build Wall")
        {
            CreatePreview(removePreviewPrefab, "Wall", posX, posY);
            return tileOWW;
        }
        return null;
    }

    public List<TileOWW> PreviewRemoveFurniture(int start_x, int end_x, int start_y, int end_y)
    {
        List<TileOWW> furniture_tiles = new List<TileOWW>();

        ClearPreviews();

        // Loop through all the tiles
        for (int x = start_x; x <= end_x; x++)
        {
            for (int y = start_y; y <= end_y; y++)
            {
                TileOWW tileOWW = WorldController.Instance.GetWorld().GetTileAt(x, y);

                // Check valid 
                if (tileOWW != null)
                {
                    if (tileOWW.GetInstalledFurniture() != null)
                    {
                        if (tileOWW.GetInstalledFurniture().GetFurnitureType() != "Wall") // Do NOT remove wall furniture items! 
                        {
                            furniture_tiles.Add(tileOWW);
                            CreatePreview(removePreviewPrefab, "Hull", x, y);
                        }
                    }
                    // Allow removal of build jobs that are not walls
                    else if (tileOWW.currentJobType != null)
                    {
                        if (tileOWW.currentJobType.Contains("Build") && !tileOWW.currentJobType.Contains("Wall") && !tileOWW.currentJobType.Contains("Hull"))
                        {
                            furniture_tiles.Add(tileOWW);
                            CreatePreview(removePreviewPrefab, "Hull", x, y);
                        }
                    }
                }
            }
        }
        return furniture_tiles;
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
                TileOWW tileOWW = WorldController.Instance.GetWorld().GetTileAt(x, y);

                // Check valid 
                if (tileOWW?.GetInstalledFurniture()?.GetFurnitureType() == "Wall" || tileOWW?.currentJobType == "Build Wall") // Only remove wall furniture items! 
                {
                    wall_tiles.Add(tileOWW);
                    CreatePreview(removePreviewPrefab, "Wall", x, y);
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

        // Display price UI
        InputController.Instance.currentBuildPrice = 1000;
    }

    public void PlanHull(List<TileOWW> hull_tiles, List<TileOWW> furniture_tiles, List<TileOWW> floor_tiles)
    {
        int hullOrder = 0;
        int wallOrder = 0;

        foreach (TileOWW tile in floor_tiles)
        {
            // We only want to plan a hull tile if nothing else is already planned 
            if (tile.currentJobType == null)
            {
                Action placeHullAction = delegate () { PlaceHull(tile); };
                Job j = new Job(placeHullAction, tile, 3, "Build Hull", JobPriority.Medium);
                tile.currentJobType = j.GetJobType();
                JobQueueController.BuildersJobQueue.AddJob(j);

                hullOrder ++;
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
                Job j = new Job(placeFurnitureAction, tile, 3, "Build Wall", JobPriority.Medium, new Job(placeHullAction, tile, 3, "Build Hull", JobPriority.Medium));
                tile.currentJobType = j.GetJobType();
                JobQueueController.BuildersJobQueue.AddJob(j);

				hullOrder ++;
                wallOrder ++;

            }
            else Debug.Log("There's already a job here! Remove it first. " + tile.ToString());
        }

        // Consider materials already in stock 
        // TODO - BROKEN!
        /*
        if (CargoController.Instance.unusedShipStock.ContainsKey("Hull"))
        {
            hullOrder -= (CargoController.Instance.unusedShipStock["Hull"]);
        }

        if (CargoController.Instance.unusedShipStock.ContainsKey("Wall"))
        {
            wallOrder -= (CargoController.Instance.unusedShipStock["Wall"]);
        }
        */

        // Invoice 
        CurrencyController.Instance.DeductBankBalance(hullOrder * HULL_COST);
        CurrencyController.Instance.DeductBankBalance(wallOrder * WALL_COST);
    }

    // Overloaded method for planing multiple furniture jobs at once (e.g. walls, tanks, batteries, etc.)
    public void PlanFurniture(List<TileOWW> furniture_tiles, FurnitureType furnitureType)
    {
        foreach (TileOWW tile in furniture_tiles)
        {
            PlanFurniture(tile, furnitureType);
        }
    }

    // Overloaded method for planing single furniture jobs at once
    public void PlanFurniture(TileOWW furnitureTile, FurnitureType furnitureType)
    {
        if (furnitureTile != null)
        {
            // Check validity for multi-tile 
            for (int i = 0; i < furnitureType.sizeX; i++)
            {
                for (int j = 0; j < furnitureType.sizeY; j++)
                {
                    TileOWW temp = WorldController.Instance.GetWorld().GetTileAt(furnitureTile.GetX() + i, furnitureTile.GetY() + j);
                    if (temp.currentJobType != null)
                    {
                        Debug.Log("Unable to place job baceause tile already had an existing job: " + temp.currentJobType);
                        return; // Failed to place 
                    }
                }
            }

            Action placeFurnitureAction = delegate () { PlaceFurniture(furnitureTile, furnitureType.title); };
            Job job;

            // Allow certain furniture to overwrite others by creating a prerequisite removal job
            if (furnitureTile.GetInstalledFurniture()?.GetFurnitureType() == "Wall" && furnitureType.title == "Airlock")
            {
                Action removeExistingFurnitureAction = delegate () { RemoveFurniture(furnitureTile); };
                Job prerequisiteJob = new Job(removeExistingFurnitureAction, furnitureTile, 2, "Remove Wall", JobPriority.Medium);
                job = new Job(placeFurnitureAction, furnitureTile, furnitureType.installTime, "Build " + furnitureType.title, JobPriority.Medium, prerequisiteJob);
            }
            else
            {
                job = new Job(placeFurnitureAction, furnitureTile, furnitureType.installTime, "Build " + furnitureType.title, JobPriority.Medium);
            }
            
            furnitureTile.currentJobType = job.GetJobType();
            JobQueueController.BuildersJobQueue.AddJob(job);

			// Invoice 
            CurrencyController.Instance.DeductBankBalance(furnitureType.cost);

			// Multi-tile furniture items 
			for (int i = 0; i < furnitureType.sizeX; i++)
            {
                for (int j = 0; j < furnitureType.sizeY; j++)
                {
                    TileOWW temp = WorldController.Instance.GetWorld().GetTileAt(furnitureTile.GetX() + i, furnitureTile.GetY() + j);
                    temp.currentJobType = furnitureTile.currentJobType;
                    temp.installedFurnitureAltX = furnitureTile.GetX();
                    temp.installedFurnitureAltY = furnitureTile.GetY();
                }
            }
        }

    }

    public void PlanRemoveHull(List<TileOWW> hull_tiles)
    {
        foreach (TileOWW tileOWW in hull_tiles)
        {
            if (tileOWW.currentJobType == null)
            {
                Action removeHullAction = delegate () { RemoveHull(tileOWW); };

                // Make a prerequisite job to remove furniture first!
                if (tileOWW.GetInstalledFurniture() != null)
                {
                    Action removeFurnitureAction = delegate () { RemoveFurniture(tileOWW); };
                    Job j = new Job(removeHullAction, tileOWW, 2, "Destroy Hull", JobPriority.Medium, new Job(removeFurnitureAction, tileOWW, 2, "Remove " + tileOWW.GetInstalledFurniture().GetFurnitureType(), JobPriority.Medium));
                    tileOWW.currentJobType = j.GetJobType();
                    JobQueueController.BuildersJobQueue.AddJob(j);
                }             
                else
                {
                    Job j = new Job(removeHullAction, tileOWW, 2, "Destroy Hull", JobPriority.Medium);
                    tileOWW.currentJobType = j.GetJobType();
                    JobQueueController.BuildersJobQueue.AddJob(j);
                }
            }
            // Remove build jobs if applicable 
            else if (tileOWW.currentJobType != null)
            {
                if (tileOWW.currentJobType.Contains("Build"))
                {
                    // Find the job and remove it
                    if (!JobQueueController.BuildersJobQueue.TryDestroyJob(tileOWW, JobPriority.Medium))
                    {
                        Debug.LogWarning("Failed to remove job!!");
                    }
                }
            }
        }
    }

    public void PlanRemoveFurniture(List<TileOWW> furniture_tiles)
    {
        foreach (TileOWW tileOWW in furniture_tiles)
        {
            if (tileOWW.currentJobType == null || tileOWW.currentJobType.Contains("Use"))
            {
                // If tile is not walkable, the job will always fail!
                if (tileOWW.GetInstalledFurniture()?.GetFurnitureType() == "Wall")
                {
                    // Find a neighbour tile that is not blocked
                    if (WorldController.Instance.GetWorld().GetTileAt(tileOWW.GetX() + 1, tileOWW.GetY()).GetInstalledFurniture()?.GetFurnitureType() != "Wall")
                    {
                        CreateRemoveFurnitureAction(tileOWW, 1, 0);
                    }
                    else if (WorldController.Instance.GetWorld().GetTileAt(tileOWW.GetX() - 1, tileOWW.GetY()).GetInstalledFurniture()?.GetFurnitureType() != "Wall")
                    {
                        CreateRemoveFurnitureAction(tileOWW, -1, 0);
                    }
                    else if (WorldController.Instance.GetWorld().GetTileAt(tileOWW.GetX(), tileOWW.GetY() + 1).GetInstalledFurniture()?.GetFurnitureType() != "Wall")
                    {
                        CreateRemoveFurnitureAction(tileOWW, 0, 1);
                    }
                    else if (WorldController.Instance.GetWorld().GetTileAt(tileOWW.GetX(), tileOWW.GetY() - 1).GetInstalledFurniture()?.GetFurnitureType() != "Wall")
                    {
                        CreateRemoveFurnitureAction(tileOWW, 0, -1);
                    }
                    else
                    {
                        Debug.LogWarning("Job isn't reachable, send help!!");
                    }

                }
                else
                {
                    CreateRemoveFurnitureAction(tileOWW);
                }
            }
            // Try remove install job 
            else if (tileOWW.currentJobType != null)
            {
                if (tileOWW.currentJobType.Contains("Build") && !tileOWW.currentJobType.Contains("Hull"))
                {
                    // Find the job and remove it
                    if (!JobQueueController.BuildersJobQueue.TryDestroyJob(tileOWW, JobPriority.Medium))
                    {
                        Debug.LogWarning("Failed to remove job!!");
                    }
                }
            }
        }

    }

    private void CreateRemoveFurnitureAction(TileOWW tile, int offsetX = 0, int offsetY = 0)
    {
        Action removeFurnitureAction = delegate () { RemoveFurniture(tile); };
        Job j = new Job(removeFurnitureAction, tile, 2, "Remove " + tile.GetInstalledFurniture().GetFurnitureType(), JobPriority.Medium);
        j.SetAltPosition(tile.GetX() + offsetX, tile.GetY() + offsetY);
        tile.currentJobType = j.GetJobType();
        JobQueueController.BuildersJobQueue.AddJob(j);
    }

    public void PlaceHull(TileOWW tile)
    {
        tile.SetTileType("Hull");
        emptyHullTiles.Add(tile);
        allHullTiles.Add(tile);

        // Event checks 
        ObjectiveController.Instance.CheckObjectives();
    }

    public void RemoveHull(TileOWW tile)
    {
        tile.SetTileType("Empty");
        emptyHullTiles.Remove(tile);
        allHullTiles.Add(tile);
        ObjectiveController.Instance.CheckObjectives();

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

        // Multi-tile references 

        if (!furnitureTypes.ContainsKey(furnitureType))
        {
            Debug.LogError("This key wasnt found: " + furnitureType);
        }
        for (int i = 0; i < furnitureTypes[furnitureType].sizeX; i++)
        {
            for (int j = 0; j < furnitureTypes[furnitureType].sizeY; j++)
            {
                TileOWW temp = WorldController.Instance.GetWorld().GetTileAt(tile.GetX() + i, tile.GetY() + j);
                temp.currentJobType = null;
                temp.installedFurnitureAltX = tile.GetX();
                temp.installedFurnitureAltY = tile.GetY();
                emptyHullTiles.Remove(tile);
            }
        }

        ObjectiveController.Instance.CheckObjectives();
    }

    public void RemoveFurniture(TileOWW tile)
    {
        string furnitureType = tile.GetInstalledFurniture()?.GetFurnitureType();

        // 'Airlock' may be in 'Airlock Open' state
        if (furnitureType == "Airlock Open")
        {
            furnitureType = "Airlock";
        }

        // Update pathfinding 
        if (furnitureType == "Wall")
        {
            // TODO: reset all failed jobs 
        }
        
        tile.RemoveInstalledFurniture();

        // Multi-tile references 
        for (int i = 0; i < furnitureTypes[furnitureType].sizeX; i++)
        {
            for (int j = 0; j < furnitureTypes[furnitureType].sizeY; j++)
            {
                TileOWW temp = WorldController.Instance.GetWorld().GetTileAt(tile.GetX() + i, tile.GetY() + j);
                temp.installedFurnitureAltX = null;
                temp.installedFurnitureAltY = null;

                if (tile.GetTileType() == "Hull")
                {
                    emptyHullTiles.Add(tile);
                }
            }
        }

        // Remove from map of furnitureType TileOWW        
        while (furnitureTileOWWMap[furnitureType].Contains(tile))
        {
            furnitureTileOWWMap[furnitureType].Remove(tile);
        }

        ObjectiveController.Instance.CheckObjectives();
    }

    public void PlaceRoom(List<TileOWW> room_tiles, string roomType)
    {
        foreach (TileOWW tile in room_tiles)
        {
            // check if we need to remove existing room tiles - they'll proably be assigned multiple times in the dictionary 
            if (tile.GetRoomType() != null)
            {
                if (roomsTileOWWMap.ContainsKey(tile.GetRoomType())) // TODO: This check shouldn't be necessary once the maps get set up properly on load 
                {
                    roomsTileOWWMap[tile.GetRoomType()].Remove(tile);
                }
            }

            tile.SetRoomType(roomType);

            if (!roomsTileOWWMap.ContainsKey(roomType))
            {
                roomsTileOWWMap.Add(roomType, new List<TileOWW>());
            }
            roomsTileOWWMap[roomType].Add(tile);
        }

        RoomController.Instance.DoRoomChecks();

        ObjectiveController.Instance.CheckObjectives();
    }

    public void PlaceStaff(float x, float y, GameObject staff, string staffName, float energy = 100, float health = 100)
    {
        if (CurrencyController.Instance.GetBankBalance() >= 500)
        {
            GameObject staffGO = Instantiate(staff, new Vector3(x, y, 0), Quaternion.identity);

            staffGO.name = staffName;
            staffGO.GetComponent<Staff>().SetEnergy(energy);
            staffGO.GetComponent<Staff>().SetHealth(health);
            staffGO.transform.parent = staffParent.transform;
            WorldController.Instance.staff.Add(staffGO);

            // TODO: Invoice depend on staff type
            CurrencyController.Instance.DeductBankBalance(1000);
        }

        ObjectiveController.Instance.CheckObjectives();
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
        foreach (TileOWW tile in world.GetAllTiles())
        {
            UnityEngine.Tilemaps.Tile t = ScriptableObject.CreateInstance<UnityEngine.Tilemaps.Tile>();
            t.sprite = TileType.Grid;
            t.name = tile.GetX() + "_" + tile.GetY();
            tilemap.GetComponent<Tilemap>().SetTile(new Vector3Int(tile.GetX(), tile.GetY(), 0), t);
        }
    }

    public void SetGridVisible(bool isVisible)
    {
        tilemap.SetActive(isVisible);
        if (tilemap.activeInHierarchy)
        {
            if (tilemap.GetComponent<Tilemap>().GetUsedTilesCount() == 0) InstantiateGridTiles(WorldController.Instance.GetWorld());
        }
    }

    private void InstantiateFurnitureTypes()
    {
        int furnitureIndex = 0;
        int utilityIndex = 0;
        int hullIndex = 1;
        int wallIndex = 1;

        furnitureTypes = FurnitureType.InstantiateFurnitureTypes();
        foreach (KeyValuePair<string, FurnitureType> furnitureEntry in furnitureTypes)
        {
            if (furnitureEntry.Value.category == FurnitureType.BuildCategory.Furniture)
            {
                GameObject furnitureEntryGO = Instantiate(furniturePreviewPrefab, furnitureListingParent.transform);
                furnitureEntryGO.transform.SetSiblingIndex(furnitureIndex);
                furnitureIndex++;
                furnitureEntryGO.GetComponent<FurnitureListing>().InputFurnitureType(furnitureEntry.Value);
            }
            else if (furnitureEntry.Value.category == FurnitureType.BuildCategory.Utility)
            {
                GameObject furnitureEntryGO = Instantiate(furniturePreviewPrefab, utilityListingParent.transform);
                furnitureEntryGO.transform.SetSiblingIndex(utilityIndex);
                utilityIndex++;
                furnitureEntryGO.GetComponent<FurnitureListing>().InputFurnitureType(furnitureEntry.Value);
            }
            else if (furnitureEntry.Value.category == FurnitureType.BuildCategory.Hull) 
            {
                GameObject furnitureEntryGO = Instantiate(furniturePreviewPrefab, hullListingParent.transform);
                furnitureEntryGO.transform.SetSiblingIndex(hullIndex);
                hullIndex++;
                furnitureEntryGO.GetComponent<FurnitureListing>().InputFurnitureType(furnitureEntry.Value);
            }
            else if (furnitureEntry.Value.category == FurnitureType.BuildCategory.Wall)
            {
                GameObject furnitureEntryGO = Instantiate(furniturePreviewPrefab, wallListingParent.transform);
                furnitureEntryGO.transform.SetSiblingIndex(wallIndex);
                wallIndex++;
                furnitureEntryGO.GetComponent<FurnitureListing>().InputFurnitureType(furnitureEntry.Value);
            }

        }
    }

    public void SetAllListingTogglesOff()
    {
        hullListingParent.GetComponent<ToggleGroup>().SetAllTogglesOff();
        wallListingParent.GetComponent<ToggleGroup>().SetAllTogglesOff();
        utilityListingParent.GetComponent<ToggleGroup>().SetAllTogglesOff();
        furnitureListingParent.GetComponent<ToggleGroup>().SetAllTogglesOff();
        roomsListingParent.GetComponent<ToggleGroup>().SetAllTogglesOff();
        staffListingParent.GetComponent<ToggleGroup>().SetAllTogglesOff();
    }

}
