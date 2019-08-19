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

        // Check if another character finished a duplicate chase job
        if (currentJob?.GetJobType() == "attack" && currentJob?.GetCharacter() == null)
        {
            targetJob = currentJob = null;
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
                failedJobs.Add(targetJob);
                jobQueue.AddJob(targetJob);
                targetJob = currentJob = null;
            }
        }

        // Check if at job location
        if (currentJob != null)
        {
            // For tile based job
            if (Vector3.Distance(new Vector3(currentX, currentY), new Vector3(currentJob.GetJobPosX(), currentJob.GetJobPosY())) < 1)
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

    // Do job until finished 
    public void DoJobTick()
    {
        if (currentJob.DoJob(Time.fixedDeltaTime))
        {
            if (currentJob == targetJob)
            {
                if (currentJob.GetTileOWW() != null)
                {
                    currentJob.GetTileOWW().currentJobType = null;
                }
                JobSpriteController.Instance.UpdateJob(currentJob.GetTileOWW());
                currentJob = targetJob = null;
                navMeshAgent.SetDestination(new Vector3(currentX, currentY, 0));
                failedJobs.Clear();
            }
            else currentJob = null;
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

    public void TakeDamage(float damage)
    {
        float startHealth = health;
        Debug.Log("Taking damage: " + damage);
        health -= damage;

        if (health < 0)
        {
            if (gameObject.CompareTag("Passenger") || gameObject.CompareTag("Builder") || gameObject.CompareTag("Guard"))
            {
                NotificationController.Instance.CreateNotification("Your " + gameObject.tag + ", '" + gameObject.name + "' has died!", UrgencyLevel.High, null);
            }
            Destroy(gameObject);
        }
        else if (health < 50 && startHealth > 50)
        {
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
                            targetJob = new Job(attackAction, target.GetComponentInParent<Character>(), 1f, "attack");
                            return;
                        }
                    }
                }
            }
        }

    }

}
