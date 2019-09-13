using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{
    protected float currentX;
    protected float currentY;
    protected Sprite sprite;

    protected float health;

	protected LooseItem inventory;

    // Jobs 
    public Job targetJob; // The base job object
    public Job currentJob; // Any prerequisite jobs of the base object must be completed first 
    public List<Job> failedJobs;

    protected NavMeshAgent navMeshAgent;
    protected SpriteRenderer spriteRenderer;

    protected JobQueue jobQueue;

    private void Start()
    {
        Init();

        failedJobs = new List<Job>();
    }

    protected virtual void Init()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.radius = 0.15f;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.transform.localPosition = new Vector3(0f, 0f, 0.25f);
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
		if (currentJob != null) {
			if (currentJob.GetJobType().Contains("Build"))
			{
				string furnitureType = JobQueueController.Instance.ConvertJobTypeToFurnitureType(currentJob.GetJobType());

				// Check if character already has stock 
				if (inventory?.itemType != furnitureType)
				{


					if (CargoController.Instance.shipStock.ContainsKey(furnitureType))
					{

						if (CargoController.Instance.shipStock[furnitureType] == 0)
						{
							// item is not in stock 
							ReturnFailedJob();
							return;
						}
						else if (CargoController.Instance.shipStock[furnitureType] < 0)
						{
							Debug.LogError(furnitureType + " stock is a negative value");
						}
						else
						{
							// Item is in stock! Character must go get it first 
							TileOWW cargoTile = CargoController.Instance.FindCargo(furnitureType);
                            if (cargoTile == null)
                            {
                                Debug.LogError("No location found for item with stock count of: " + CargoController.Instance.shipStock[furnitureType]);
                                currentJob = null;
                                return;
                            }
							currentJob.SetPrerequisiteJob(new Job(delegate () { PickUpCargo(furnitureType, 1); }, cargoTile, 0.5f, "Pickup " + furnitureType + " Cargo", JobPriority.Medium));
							currentJob = null;
							return;
						}
					}
					else
					{
						// Item is not in stock 
						ReturnFailedJob();
						return;

					}
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
                ReturnFailedJob();
                return;
            }
        }

        // Check if at job location
        if (currentJob != null)
        {
            // For tile based job
            if (Vector3.Distance(new Vector3(currentX, currentY), new Vector3(currentJob.GetJobPosX(), currentJob.GetJobPosY())) < 1.2)
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
    }

    public void ReturnFailedJob()
    {
        failedJobs.Add(targetJob);
        jobQueue.AddJob(targetJob);
        targetJob = currentJob = null;
    }

    // Do job until finished 
    public void DoJobTick()
    {
		// If job is complete 
        if (currentJob.DoJob(Time.fixedDeltaTime))
        {
            if (currentJob != null)
            {
				// Remove inventory if it was a build job
				if (currentJob.GetJobType().Contains("Build"))
				{
					// Check item built is item in inventory 
					if (inventory.itemType == JobQueueController.Instance.ConvertJobTypeToFurnitureType(currentJob.GetJobType())) {
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
        Debug.Log("Taking damage: " + damage);
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
                NotificationController.Instance.CreateNotification("Your " + gameObject.tag + ", '" + gameObject.name + "' has died!", UrgencyLevel.High, null);
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
                NotificationController.Instance.CreateNotification("Your " + gameObject.tag + ", '" + gameObject.name + "' is low on health!", UrgencyLevel.Medium, actions);
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
		TileOWW cargoTile = CargoController.Instance.FindCargo(cargoType);
		if (cargoTile != null)
		{
			cargoTile.CollectCargo(quantity);
			this.inventory = new LooseItem(cargoType, quantity);
		}
		else
		{
			ReturnFailedJob();
		}
	}
}
