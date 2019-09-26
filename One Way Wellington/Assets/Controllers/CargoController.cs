using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoController : MonoBehaviour
{
    public static CargoController Instance;
    private Dictionary<string, int> undeliveredStock;

    public Dictionary<string, int> unusedShipStock;
    public Dictionary<string, List<TileOWW>> shipStockLocations;
    public Queue<TileOWW> tempStockLocations; // Cargo that is not located to a hull tile and needs to be moved
    public Dictionary<TileOWW, LooseItem> stockInTransit; // TileOWW is the target tile for the LooseItem to get to 


    // For tutorial 
    private bool hasOrderedBefore;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null) Instance = this;
        unusedShipStock = new Dictionary<string, int>();
        undeliveredStock = new Dictionary<string, int>();
        shipStockLocations = new Dictionary<string, List<TileOWW>>();
        stockInTransit = new Dictionary<TileOWW, LooseItem>();
        tempStockLocations = new Queue<TileOWW>();

        // TODO: Save/load this 
        hasOrderedBefore = false;
    }



    IEnumerator DeliveryDelay()
    {
        float delay = 5f;
        while (delay > 0)
        {
            delay -= Time.deltaTime;
            yield return null;
        }
        DeliverItems();
    }

    // Attempt to find placement for cargo that is not located to a hull tile (floating out in space) 
    public void CheckTempStockLocations()
    {
        if (tempStockLocations.Count > 0)
        {
            TileOWW tileOWW = tempStockLocations.Peek();

            if (tileOWW.GetTileType() == "Empty")
            {
                // Find appropriate place to place cargo first 
                // Remember GetRandomHullTile() does validity checks 
                TileOWW dropTile = WorldController.Instance.GetWorld().GetRandomHullTile(avoidJobs: true);
                if (dropTile != null)
                {
                    Job collectJob = new Job(delegate () { CollectAllCargoFromTile(tileOWW, dropTile); }, tileOWW, 0.5f, "Collect All Cargo", JobPriority.High, tileExcludeOtherJobs: false);

                    // Store reference - what stock is going where 
                    Job dropJob = new Job(delegate ()
                    {
                        if (dropTile != null)
                        {
                            // If key is missing, the cargo has hopefully already been delivered 
                            if (stockInTransit.ContainsKey(dropTile))
                            {
                                DropCargo(dropTile, stockInTransit[dropTile].itemType, stockInTransit[dropTile].quantity);
                            }
                        }

                    },
                    dropTile, 0.5f, "Drop Cargo", JobPriority.High, collectJob, tileExcludeOtherJobs: true);


                    JobQueueController.BuildersJobQueue.AddJob(dropJob);
                    tempStockLocations.Dequeue();
                    Debug.Log("Move job added for cargo on empty tile");
                }
            }
        }
    }

    public void DeliverItems()
    {
        string orderSummary = "";

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
                Job collectJob = new Job(delegate () { CollectAllCargoFromStairs(); }, stairwellTile, 0.5f, "Collect Cargo", JobPriority.High, tileExcludeOtherJobs: false);
                TileOWW dropTile = WorldController.Instance.GetWorld().GetRandomHullTile(avoidJobs: true);
                Job dropJob = new Job(delegate () { DropCargo(dropTile, cargoTypeQuantityPair.Key, cargoTypeQuantityPair.Value); }, dropTile, 0.5f, "Drop Cargo", JobPriority.High, collectJob, tileExcludeOtherJobs: false);
                JobQueueController.BuildersJobQueue.AddJob(dropJob);
            }
            else
            {
                // Debug.Log("Couldn't find a stairwell!!");            
                TileOWW randomEmptyTile = WorldController.Instance.GetWorld().GetRandomEmptyTile();
                DropCargo(randomEmptyTile, cargoTypeQuantityPair.Key, cargoTypeQuantityPair.Value);
                tempStockLocations.Enqueue(randomEmptyTile);
            }

            orderSummary += "  - " + cargoTypeQuantityPair.Key + " X" + cargoTypeQuantityPair.Value + "\n";

            

            
        }

		// All stock delivered 
		undeliveredStock = new Dictionary<string, int>();

        // Order summary notification 
        NotificationController.Instance.CreateNotification("Your order for the following has been delivered: \n" + orderSummary, UrgencyLevel.Low, false);

        // Stairwell notification last 
        if (!BuildModeController.Instance.furnitureTileOWWMap.ContainsKey("Stairwell"))
        {
            NotificationController.Instance.CreateNotification("Your ship needs a stairwell to accept deliveries properly. Your materials have been placed on a random exterior tile instead.", UrgencyLevel.Medium, true, null);
        }
    }

    public void CollectAllCargoFromStairs()
    {
        Debug.Log("Cargo collected from stairs!");
    }

    // Call all related methods when taking cargo from a tile 
    // Return false if failed 
    public bool CollectAllCargoFromTile(TileOWW pickUpTile, TileOWW dropTile)
    {
        if (pickUpTile.looseItem == null) return false;

        stockInTransit.Add(dropTile, new LooseItem(pickUpTile.looseItem.itemType, pickUpTile.looseItem.quantity));
        pickUpTile.CollectCargo(-1);
        return true;
    }

    public void DropCargo(TileOWW tile, string cargoType, int quantity, bool clearFailedJobs = true)
    {
        // Does tile already have cargo? 
        if (tile.looseItem?.itemType == cargoType)
        {
            tile.looseItem.quantity += quantity;
        }
        else
        {
            // Add to tile
            BuildModeController.Instance.PlaceFurniture(tile, "Cargo");
            tile.looseItem = new LooseItem(cargoType, quantity);
        }
        // Add to ship inventory 
        if (unusedShipStock.ContainsKey(cargoType))
        {
            unusedShipStock[cargoType] += quantity;
        }
        else
        {
            unusedShipStock.Add(cargoType, quantity);
        }

        // Save reference of tile location 
        if (!shipStockLocations.ContainsKey(cargoType))
        {     
            shipStockLocations.Add(cargoType, new List<TileOWW>());           
        }
        shipStockLocations[cargoType].Add(tile);

        // Clear all failed jobs 
        if (clearFailedJobs)
        {
            foreach (GameObject characterGO in WorldController.Instance.staff)
            {
                characterGO.GetComponent<Character>().failedJobs = new List<Job>();
            }
        }

        // Remove transit reference 
        if (stockInTransit.ContainsKey(tile))
        {
            stockInTransit.Remove(tile);
        }
        else
        {
            Debug.Log("Couldn't find tile in stock in transit: " + tile);
        }
    }


    // Assumes the payment has already been made !
    public void PlaceOrder(string itemType, int quantity)
    {
        if (quantity == 0)
        {
            Debug.LogWarning("Just made an order with quantity of 0");
            return;
        }

        // Tutorial message 
        if (!hasOrderedBefore)
        {
            hasOrderedBefore = true;
            NotificationController.Instance.CreateNotification("You've just placed your first order for construction materials! Orders are delivered while your ship is stationed at a planet, so you won't receive anything while travelling.", UrgencyLevel.Medium, true);
        }

        if (undeliveredStock.ContainsKey(itemType))
        {
            undeliveredStock[itemType] += quantity;
        }
        else
        {
            undeliveredStock.Add(itemType, quantity);
        }
        // NotificationController.Instance.CreateNotification("Your order for " + quantity + " " + itemType + " has been placed!", UrgencyLevel.Low, false);
        // Debug.Log("Order placed for " + itemType + " x" + quantity);

        // if at a planet stop:
        // Deliver items

        // TODO: better way to track if we are at a planet 
        if (JourneyController.Instance.isJourneyEditMode)
        {
                StopCoroutine("DeliveryDelay");
                StartCoroutine("DeliveryDelay");
        }
        
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
                if (tileOWW.looseItem == null)
                {
                    Debug.LogWarning("The location of this stock has not been removed properly. " + cargoType + " " + tileOWW);
                    shipStockLocations[cargoType].Remove(tileOWW);
                }
                else if (tileOWW.looseItem.quantity < smallestQuantity)
                {
                    smallestQuantity = tileOWW.looseItem.quantity;
                    smallestQuantityTile = tileOWW;
                }
            }

            return smallestQuantityTile;
        }
        return null;
    }

    // DEBUG function 
    public void ShowCargoSummary()
    {
        string output = "CARGO SUMMARY \n Click this message to read \n";

        output += "Undelivered Stock: \n";
        foreach (KeyValuePair<string, int> keyValuePair in undeliveredStock)
        {
            output += " - " + keyValuePair.Key + " " + keyValuePair.Value + "\n";
        }
        output += "\n";

        output += "Ship Stock \n";
        foreach (KeyValuePair<string, int> keyValuePair in unusedShipStock)
        {
            output += " - " + keyValuePair.Key + " " + keyValuePair.Value + "\n";
        }
        output += "\n";

        output += "Ship Stock Locations \n";
        foreach (KeyValuePair<string, List<TileOWW>> keyValuePair in shipStockLocations)
        {
            output += " - " + keyValuePair.Key + "\n";
            foreach (TileOWW tileOWW in keyValuePair.Value)
            {
                output += "    - " + tileOWW.ToString() + "\n";
            }
        }
        output += "\n";

        output += "Temp Stock Locations \n";
        foreach (TileOWW tileOWW in tempStockLocations)
        {
            output += " - " + tileOWW.ToString() + "\n";
        }
        output += "\n";

        output += "Stock In Transit";
        foreach (KeyValuePair<TileOWW, LooseItem> keyValuePair in stockInTransit)
        {
            output += " - " + keyValuePair.Key.ToString() + "\n";
            output += "    - " + keyValuePair.Value.itemType + " " + keyValuePair.Value.quantity + "\n";
        }
        output += "\n";


        Debug.Log(output);
    }
}
