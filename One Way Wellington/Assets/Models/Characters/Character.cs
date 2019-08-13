using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{
    protected float currentX;
    protected float currentY;
    protected Sprite sprite;

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
            if (Vector3.Distance(new Vector3(currentX, currentY), new Vector3(currentJob.GetJobPosX(), currentJob.GetJobPosY())) < 2)
            {
                // Do job until finished 
                if (currentJob.DoJob(Time.fixedDeltaTime))
                {
                    if (currentJob == targetJob)
                    {
                        currentJob.GetTileOWW().currentJobType = null;
                        JobSpriteController.Instance.UpdateJob(currentJob.GetTileOWW());
                        currentJob = targetJob = null;
                        navMeshAgent.SetDestination(new Vector3(currentX, currentY, 0));
                        failedJobs.Clear();
                    }
                    else currentJob = null;
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

}
