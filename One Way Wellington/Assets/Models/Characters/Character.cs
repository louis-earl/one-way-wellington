using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{
    protected float currentX;
    protected float currentY;

    private float health;

	protected LooseItem inventory;

    // Jobs 
    public Job targetJob; // The base job object
    public Job currentJob; // Any prerequisite jobs of the base object must be completed first 
    public List<Job> failedJobs;
    protected JobQueue jobQueue;

    // Rendering 
    protected Sprite sprite;
    protected SpriteRenderer spriteRenderer;


    // Pathfinding 
    public static Vector3[] path = new Vector3[0];
    protected LineRenderer lineRenderer;
    protected NavMeshAgent navMeshAgent;
    public static Character currentSelectedCharacter;

    // Pathfinding stuck
    public Vector2 lastLocation;
    public float stuckCheckTime;


    private void Start()
    {
        Init();
        lineRenderer = GetComponent<LineRenderer>();
        failedJobs = new List<Job>();
        lastLocation = transform.position;
        stuckCheckTime = 5f;
    }

    private void Update()
    {
        // Draw path 
        if (currentSelectedCharacter == this)
        {
            lineRenderer.enabled = true;
            lineRenderer.material.SetTextureOffset("_MainTex", Vector2.right * Time.time);
            path = gameObject.GetComponent<NavMeshAgent>().path.corners;

            if (path != null && path.Length > 1)
            {
                lineRenderer.positionCount = path.Length;
                for (int i = 0; i < path.Length; i++)
                {
                    lineRenderer.SetPosition(i, path[i]);
                }
            }
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    protected virtual void Init()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.radius = 0.15f;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        Refresh();
    }

    protected virtual void Refresh()
    {
        currentX = transform.position.x;
        currentY = transform.position.y;
   
        if (currentJob == null && targetJob != null)
        {
            if (targetJob.GetPrerequisiteJob() == null) currentJob = targetJob;
            else currentJob = targetJob.GetPrerequisiteJob();
        }

        // If applicable, Check if in stock 
        if (currentJob != null)
        {
            if (currentJob.GetJobType().Contains("Build"))
            {
                string furnitureType = JobQueueController.Instance.ConvertJobTypeToFurnitureType(currentJob.GetJobType());

                // Check if character already has stock 
                if (inventory == null)
                {
                    if (CargoController.Instance.unusedShipStock.ContainsKey(furnitureType))
                    {

                        if (CargoController.Instance.unusedShipStock[furnitureType] == 0)
                        {
                            // item is not in stock 
                            Debug.Log("Item: " + furnitureType + " is not in stock");
                            ReturnFailedJob();
                            return;
                        }
                        else if (CargoController.Instance.unusedShipStock[furnitureType] < 0)
                        {
                            Debug.LogError(furnitureType + " stock is a negative value");
                        }
                        else
                        {
                            // Item is in stock! Character must go get it first 
                            TileOWW cargoTile = CargoController.Instance.FindCargo(furnitureType);
                            if (cargoTile == null)
                            {
                                Debug.LogError("No location found for item with stock count of: " + CargoController.Instance.unusedShipStock[furnitureType]);
                                currentJob = null;
                                return;
                            }
                            // Pick up new materials
                            currentJob.SetPrerequisiteJob(new Job(delegate () { if (inventory != null) { return; } PickUpCargo(furnitureType, 1); }, cargoTile, 0.5f, "Pickup " + furnitureType + " Cargo", JobPriority.Medium));


                            currentJob = null;
                            return;
                        }
                    }
                    else
                    {
                        // Item is not in stock 
                        Debug.Log("Item: " + furnitureType + " is not in stock");
                        ReturnFailedJob();
                        return;

                    }
                }
                else if (inventory.itemType != furnitureType)
                {
                    // Return existing inventory first ! 

                    Debug.Log("returning the incorrect inventory");

                    TileOWW returnTile = CargoController.Instance.FindCargo(inventory.itemType);
                    if (returnTile == null)
                    {
                        // Put cargo down at random tile
                        returnTile = WorldController.Instance.GetWorld().GetRandomHullTile(avoidJobs: true);
                    }

                    currentJob = 
                        new Job(delegate ()
                        {
                            if (inventory == null) return;
                            CargoController.Instance.DropCargo(returnTile, inventory.itemType, inventory.quantity, clearFailedJobs: false);
                            inventory = null;
                        },
                        returnTile, 0.5f, "Return Cargo", JobPriority.High, tileExcludeOtherJobs: false);
                }
            }
        }

		// Check if another character finished a duplicate chase job
		if (currentJob != null)
		{
			if (currentJob.GetJobType().Contains("Attack") && currentJob?.GetCharacter() == null)
			{
				targetJob = currentJob = null;
			}
		}


        // Check if at job location
        // Ordered before setting a path, incase already at location 
        if (currentJob != null)
        {
            // For tile based job
            if (Vector3.Distance(new Vector3(currentX, currentY), currentJob.GetLocation()) < 1.2)
            {
                DoJobTick();
                return;
            }
            // For character based job
            if (currentJob.GetCharacter() != null)
            {
                if (Vector3.Distance(transform.position, currentJob.GetCharacter().transform.position) < 1)
                {

                    DoJobTick();
                    return;
                }
            }
        }


        // Set path 
        if (currentJob != null)
        {

            NavMeshPath navpath = new NavMeshPath();
            NavMesh.CalculatePath(new Vector3(currentX, currentY, 0), currentJob.GetLocation(), -1, navpath);
            if (navpath.status == NavMeshPathStatus.PathComplete)
            {
                navMeshAgent.SetDestination(currentJob.GetLocation());
            }
            else
            {
                // Debug.Log("Couldn't make a path");
                ReturnFailedJob();
                return;
            }
        }

        // Stuck but path is complete
        if (currentJob != null)
        {
            // Check every 5 seconds 
            if (stuckCheckTime > 0)
            {
                stuckCheckTime -= Time.deltaTime;
            }
            // Due for stuck check 
            else
            {
                stuckCheckTime = 5f;
                // not at job location 
                if (Vector2.Distance(transform.position, currentJob.GetLocation()) > 1.2f)
                {
                    // Do nothing?
                }
                else if (Vector2.Distance(transform.position, lastLocation) < 0.667f)
                {
                    // We are stuck
                    // Fix 1: move to middle of tile
                    transform.position = new Vector3(Mathf.FloorToInt(transform.position.x) + 0.5f, Mathf.FloorToInt(transform.position.y) + 0.5f);
                    Debug.LogWarning("STUCK CHARACTER - Tried moving " + name + " to the center of the tile.");
                }
                lastLocation = transform.position;
            }
        }

    }

    public void ReturnFailedJob()
    {

        if (targetJob.GetJobType() == "Collect All Cargo")
        {
            // Prevent cargo being lost forever 
            inventory = CargoController.Instance.stockInTransit[targetJob.GetTileOWW()];
            CargoController.Instance.stockInTransit.Remove(targetJob.GetTileOWW());

            // Notification 
            // NotificationController.Instance.CreateNotification("A builder just got stuck trying to move some cargo, consider building more Airlock doors.", UrgencyLevel.Medium, true, buttonActions: null);

        }
        else if (targetJob.GetJobType() == "Collect Cargo")
        {
            // do nothing 
            Debug.Log("collect cargo job diverted from returning as failed job");
        }
        else if (targetJob.GetJobType() == "Return Cargo")
        {
            Debug.LogWarning("Failed to return the failed job");
            // NotificationController.Instance.CreateNotification("A builder just got stuck trying to move some cargo, consider building more Airlock doors.", UrgencyLevel.Medium, true, buttonActions: null);


        }
        else if (targetJob.GetJobType() != "Wander" && !targetJob.GetJobType().Contains("Drop"))
        {
            Debug.Log("Returning failed job: " + targetJob.GetJobType());

            failedJobs.Add(targetJob);
            jobQueue.AddJob(targetJob);
        }

        // Clear the job 
        targetJob = currentJob = null;

        if (inventory != null)
        {
            // Return cargo to existing stash if possible 
            TileOWW returnTile = CargoController.Instance.FindCargo(inventory.itemType);
            if (returnTile == null)
            {
                // Put cargo down at random tile
                returnTile = WorldController.Instance.GetWorld().GetRandomHullTile(avoidJobs: true);

            }
            navMeshAgent.ResetPath();
            targetJob = new Job(delegate () {
                if (inventory == null) return;
                CargoController.Instance.DropCargo(returnTile, inventory.itemType, inventory.quantity, clearFailedJobs: false);
                inventory = null;}, 
                returnTile, 0.5f, "Return Cargo", JobPriority.High, tileExcludeOtherJobs: false);

        }
    }

    // Do job until finished 
    public void DoJobTick()
    {
        // Checks inventory if build job, otherwise pass 
        if ((currentJob.GetJobType().Contains("Build") && currentJob.GetJobType().Contains(inventory.itemType) && inventory.quantity == 1) || !currentJob.GetJobType().Contains("Build"))
        {
            // If job is complete 
            if (currentJob.DoJob(Time.fixedDeltaTime, this))
            {
                if (currentJob != null)
                {
                    // Remove inventory if it was a build job

                    if (currentJob.GetJobType().Contains("Build"))
                    {
                        // Check item built is item in inventory 
                        if (inventory?.itemType == JobQueueController.Instance.ConvertJobTypeToFurnitureType(currentJob.GetJobType()))
                        {
                            inventory.quantity -= 1;
                            if (inventory.quantity == 0)
                            {
                                inventory = null;
                            }
                        }
                        else Debug.LogError("Item being built didn't match item in inventory!");
                    }

                    if (currentJob == targetJob)
                    {
                        currentJob = targetJob = null;
                        navMeshAgent.SetDestination(new Vector3(currentX, currentY, 0));
                        failedJobs.Clear();
                    }
                    else
                    {
                        currentJob = null;
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("Not valid to do job tick!");
            ReturnFailedJob();
        }
    }

    public float GetXPos()
    {
        return currentX;
    }

    public float GetYPos()
    {
        return currentY;
    }

    public LooseItem GetInventory()
    {
        return inventory;
    }


    public float GetHealth()
    {
        return health;
    }

    public void SetHealth(float health)
    {
        this.health = health;
    }

    public void TakeDamage(float damage)
    {
        float startHealth = health;
        // Debug.Log("Taking damage: " + damage);
        health -= damage;

        if (health < 0)
        {
           
            
            if (gameObject.CompareTag("Passenger") || gameObject.CompareTag("Builder") || gameObject.CompareTag("Guard"))
            {
                // Clear previous notification 
                GameObject previousNotificationGO = NotificationController.Instance.FindNotificationGO(("Your " + gameObject.tag + ", '" + gameObject.name + "' is low on health!"));
                if (previousNotificationGO != null)
                {
                    NotificationController.Instance.CloseNotification(previousNotificationGO);
                }

                // Create new notification
                NotificationController.Instance.CreateNotification("Your " + gameObject.tag + ", '" + gameObject.name + "' has died!", UrgencyLevel.Medium, false, buttonActions: null);
            }

            // Remove other references 
            if (gameObject.CompareTag("Builder") || gameObject.CompareTag("Guard"))
            {
                WorldController.Instance.staff.Remove(gameObject);
            }
            else if (gameObject.CompareTag("Passenger"))
            {
                JourneyController.Instance.currentPassengers.Remove(gameObject);
            }

            // Remove this character
            Destroy(gameObject);
        }
        else if (health < 50 && startHealth >= 50)
        {
            // Create new notification 
            if (gameObject.CompareTag("Passenger") || gameObject.CompareTag("Builder") || gameObject.CompareTag("Guard"))
            {
                List<Action> actions = new List<Action>()
                {
                    delegate () { StartCoroutine(InputController.Instance.MoveCameraTo(currentX, currentY)); }
                };
                NotificationController.Instance.CreateNotification("Your " + gameObject.tag + ", " + gameObject.name + " is low on health!", UrgencyLevel.High, false, buttonTitles: new List<string>() { "Go to " + name}, buttonActions: actions);
            }
        }
        
    }

    // Use 2D raycast to find character in view 
    // TODO: Return a character?
    protected void DoJobAtVisibleCharacter(params string[] characterTags)
    {
        // Get a list of potential targets 
        Collider2D[] potentialTargets = Physics2D.OverlapCircleAll(transform.position, 10);


        foreach (Collider2D target in potentialTargets)
        {
            // Type check
            if (target.transform.parent != null)
            {
                // Params can be ordered by priority 
                foreach (string characterTag in characterTags)
                {
                    // Debug.Log(string.Join("", characterTag) + " -> " + target.transform.parent.tag);
                    if (characterTag.Equals(target.transform.parent.tag))
                    {
                        // Visibility check 
                        RaycastHit2D hit = Physics2D.Raycast(transform.position, (target.transform.position - transform.position));
                        // Debug.DrawRay(transform.position, (target.transform.position - transform.position), Color.red);
                        if (characterTag.Equals(target.transform.parent.tag))
                        {
                            Action attackAction = delegate () { target.GetComponentInParent<Character>().TakeDamage(25); };
                            targetJob = new Job(attackAction, target.GetComponentInParent<Character>(), 1f, "Attack " + target.name, JobPriority.High);
                            return;
                        }
                    }
                }
            }
        }

    }

    protected void DoJobAtFurnitureTile(string furnitureType, string jobType, Action action, float jobTime)
    {
        // Does the furnitureType exist in the world?
        if (BuildModeController.Instance.furnitureTileOWWMap.ContainsKey(furnitureType))
        {
            // Loop through all chargers
            for (int i = 0; i < BuildModeController.Instance.furnitureTileOWWMap[furnitureType].Count; i++)
            {
                TileOWW tileOWW = BuildModeController.Instance.furnitureTileOWWMap[furnitureType][i];
                if (tileOWW.currentJobType == null)
                {
                    if (targetJob != null)
                    {

                        if (targetJob.GetJobType() != jobType)
                        {
                            jobQueue.AddJob(targetJob);
                            targetJob = currentJob = null;
                        }

                    }

                    targetJob = new Job(action, tileOWW, 5, jobType, JobPriority.Medium);
                    return;

                }
            }
        }
    }

	protected void PickUpCargo(string cargoType, int quantity)
	{
        if (this.inventory != null)
        {
            Debug.LogError("Replacing existing inventory!! New: " + cargoType + " " + quantity + " Existing: " + inventory.itemType + " " + inventory.quantity);
        }
		TileOWW cargoTile = CargoController.Instance.FindCargo(cargoType);
		if (cargoTile != null)
		{
			cargoTile.CollectCargo(quantity);
			this.inventory = new LooseItem(cargoType, quantity);
		}
		else
		{
            Debug.Log("Not sure why to return job here?");
			// ReturnFailedJob();
		}
	}

    public void SetInventory(LooseItem looseItem)
    {
        if (this.inventory != null)
        {
            Debug.LogError("Replacing existing inventory!! New: " + looseItem.itemType + " " + looseItem.quantity + " Existing: " + inventory.itemType + " " + inventory.quantity);
        }
        this.inventory = looseItem;
    }

}
