using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoController : MonoBehaviour
{
    public static CargoController Instance;
    private Dictionary<string, int> undeliveredStock;

    public Dictionary<string, int> stocktake;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null) Instance = this;
        stocktake = new Dictionary<string, int>();
        undeliveredStock = new Dictionary<string, int>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void DeliverItems()
    {
        
        foreach (KeyValuePair<string, int> keyValuePair in undeliveredStock)
        {
            // Add to ship inventory 
            if (stocktake.ContainsKey(keyValuePair.Key))
            {
                stocktake[keyValuePair.Key] += keyValuePair.Value;
            }
            else
            {
                stocktake.Add(keyValuePair.Key, keyValuePair.Value);
            }


            // Create jobs to move 

            // Find stairwell
            Vector3 stairwellPos = Vector3.zero;

            if (BuildModeController.Instance.furnitureTileOWWMap.ContainsKey("Stairwell"))
            {
                stairwellPos = new Vector3(BuildModeController.Instance.furnitureTileOWWMap["Stairwell"][0].GetX(), BuildModeController.Instance.furnitureTileOWWMap["Stairwell"][0].GetY(), 0);
            }
            else
            {
                Debug.LogWarning("Couldn't find a stairwell!!");
            }

            TileOWW stairwellTile = WorldController.Instance.GetWorld().GetTileAt((int)stairwellPos.x, (int)stairwellPos.y);

            Job collectJob = new Job(delegate () { CollectCargoFromStairs(); }, stairwellTile, 0.5f, "collectCargo", tileExcludeOtherJobs: false);


            TileOWW dropTile = WorldController.Instance.GetWorld().GetRandomHullTile();

            Job dropJob = new Job(delegate () { DropCargo(dropTile, keyValuePair.Key); }, dropTile, 0.5f, "dropCargo", collectJob, tileExcludeOtherJobs: false);


            JobQueueController.BuildersJobQueue.AddJob(dropJob);
        }
    }

    public void CollectCargoFromStairs()
    {
        Debug.Log("Cargo collected from stairs!");
    }

    public void CollectCargo()
    {
        Debug.Log("Cargo collected!");
    }

    public void DropCargo(TileOWW tile, string cargoType)
    {
        Debug.Log("Cargo dropped: " + cargoType);
        BuildModeController.Instance.PlaceFurniture(tile, "Cargo");
    }

    // Assumes the payment has already been made !
    public void PlaceOrder(string itemType, int quantity)
    {
        if (undeliveredStock.ContainsKey(itemType))
        {
            undeliveredStock[itemType] += quantity;
        }
        else
        {
            undeliveredStock.Add(itemType, quantity);
        }
    }
}
