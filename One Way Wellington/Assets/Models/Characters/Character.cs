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
        if (targetJob.GetJobType() != "Wander")
        {
            Debug.Log("Returning failed job: " + targetJob.GetJobType());

            failedJobs.Add(targetJob);
            jobQueue.AddJob(targetJob);
        }

        // Clear the job 
        targetJob = currentJob = null;      
    }

    // Do job until finished 
    public void DoJobTick()
    {
		// If job is complete 
        if (currentJob.DoJob(Time.fixedDeltaTime, this))
        {
            if (currentJob != null)
            {				
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
}
