using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Orc : Enemy
{
    
    protected override void Init()
    {
        // Call from superclass
        base.Init();

        // Setup from here onwards
        jobQueue = JobQueueController.OrcsJobQueue;
    }

    protected override void Refresh()
    {
        // Call from superclass
        base.Refresh();

        // Program from here onwards


        // Find and set a target/current jobs
        if (targetJob == null || targetJob?.GetJobType() == "wander")
        {
            // Not required to use global job queue yet 
            // targetJob = jobQueue.GetNextJob(new Vector2(currentX, currentY), failedJobs);

            DoJobAtVisibleCharacter("Staff", "Passenger");

            if (targetJob == null)
            {
                // We are idle, wander the ship 
                Action attackAction = delegate () { };
                targetJob = new Job(attackAction, WorldController.Instance.GetWorld().GetRandomHullTile(), 1f, "wander");
            }
        }
        
    }

    
}
