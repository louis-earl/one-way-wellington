﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class JobSpriteController : MonoBehaviour
{
    public static JobSpriteController Instance;

    public Tilemap tilemap;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }


    public void UpdateAllJob()
    {
        foreach (TileOWW tileOWW in WorldController.Instance.GetWorld().GetAllTiles())
        {
            UpdateJob(tileOWW);
        }
    }

    public void UpdateJob(TileOWW tileOWW)
    {
        if (tileOWW == null) return;

        RuleTile t;
        if (tileOWW.currentJobType != null && // Has a job
            ((tileOWW.installedFurnitureAltX == null && tileOWW.installedFurnitureAltY == null) || // Is the origin tile of job
            (tileOWW.installedFurnitureAltX == tileOWW.GetX() && tileOWW.installedFurnitureAltY == tileOWW.GetY()))) // Is the origin tile of job
        {			

			// Create job graphics 
			t = Resources.Load<RuleTile>("TileSets/Furniture/" + JobQueueController.Instance.ConvertJobTypeToFurnitureType(tileOWW.currentJobType));
            tilemap.SetTile(new Vector3Int(tileOWW.GetX(), tileOWW.GetY(), 0), t);
            tilemap.RefreshTile(new Vector3Int(tileOWW.GetX(), tileOWW.GetY(), 0));
        }
        else
        {
            // Remove tile graphics 
            t = null;
            tilemap.SetTile(new Vector3Int(tileOWW.GetX(), tileOWW.GetY(), 0), t);
            tilemap.RefreshTile(new Vector3Int(tileOWW.GetX(), tileOWW.GetY(), 0));
        }
    }
}
