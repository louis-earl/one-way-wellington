using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoController : MonoBehaviour
{
    public static CargoController Instance;
    private Dictionary<string, int> undeliveredStock;

    public Dictionary<string, int> shipStock;
    public Dictionary<string, List<TileOWW>> shipStockLocations;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null) Instance = this;
        shipStock = new Dictionary<string, int>();
        undeliveredStock = new Dictionary<string, int>();
        shipStockLocations = new Dictionary<string, List<TileOWW>>();
    }



    public void DeliverItems()
    {
        
        foreach (KeyValuePair<string, int> cargoTypeQuantityPair in undeliveredStock)
        {          
            // Create jobs to move 

            // Find stairwell
            Vector3 stairwellPos = Vector3.zero;

            if (BuildModeController.Instance.furnitureTileOWWMap.ContainsKey("Stairwell"))
            {
                stairwellPos = new Vector3(BuildModeController.Instance.furnitureTileOWWMap["Stairwell"][0].GetX(), BuildModeController.Instance.furnitureTileOWWMap["Stairwell"][0].GetY(), 0);

				// Cargo can be picked up and placed properly
				TileOWW stairwellTile = WorldController.Instance.GetWorld().GetTileAt((int)stairwellPos.x, (int)stairwellPos.y);
				Job collectJob = new Job(delegate () { CollectCargoFromStairs(); }, stairwellTile, 0.5f, "Collect Cargo", tileExcludeOtherJobs: false);
				TileOWW dropTile = WorldController.Instance.GetWorld().GetRandomHullTile(avoidJobs: true);
				Job dropJob = new Job(delegate () { DropCargo(dropTile, cargoTypeQuantityPair.Key, cargoTypeQuantityPair.Value); }, dropTile, 0.5f, "Drop Cargo", collectJob, tileExcludeOtherJobs: false);
				JobQueueController.BuildersJobQueue.AddJob(dropJob);
			}
            else
            {
                Debug.Log("Couldn't find a stairwell!!");
                DropCargo(WorldController.Instance.GetWorld().GetRandomEmptyTile(), cargoTypeQuantityPair.Key, cargoTypeQuantityPair.Value);
            }                  
        }

		// All stock delivered 
		undeliveredStock = new Dictionary<string, int>();
    }

    public void CollectCargoFromStairs()
    {
        Debug.Log("Cargo collected from stairs!");
    }

    public void CollectCargo()
    {
        Debug.Log("Cargo collected!");
    }

    public void DropCargo(TileOWW tile, string cargoType, int quantity)
    {
        // Add to tile
        BuildModeController.Instance.PlaceFurniture(tile, "Cargo");
        tile.looseItem = new LooseItem(cargoType, quantity);

        // Add to ship inventory 
        if (shipStock.ContainsKey(cargoType))
        {
            shipStock[cargoType] += quantity;
        }
        else
        {
            shipStock.Add(cargoType, quantity);
        }

        // Save reference of tile location 
        if (!shipStockLocations.ContainsKey(cargoType))
        {     
            shipStockLocations.Add(cargoType, new List<TileOWW>());           
        }
        shipStockLocations[cargoType].Add(tile);

        // Clear all failed jobs 
        foreach (GameObject characterGO in WorldController.Instance.staff)
        {
            characterGO.GetComponent<Character>().failedJobs = new List<Job>();
        }
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

		Debug.Log("Order placed for " + itemType + " x" + quantity);

        // if at a planet stop:
        // Deliver items 
    }


    public TileOWW FindCargo(string cargoType)
    {
        if (shipStockLocations.ContainsKey(cargoType))
        {
            // Prefer to take from the smallest stack, so that space can be cleared 
            int smallestQuantity = int.MaxValue;
            TileOWW smallestQuantityTile = null;
            foreach (TileOWW tileOWW in shipStockLocations[cargoType])
            {
                if (tileOWW.looseItem.quantity < smallestQuantity)
                {
                    smallestQuantity = tileOWW.looseItem.quantity;
                    smallestQuantityTile = tileOWW;
                }
            }

            return smallestQuantityTile;
        }
        return null;
    }
}
