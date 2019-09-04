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
        if (targetJob == null || targetJob?.GetJobType() == "Wander")
        {
            // Not required to use global job queue yet 
            // targetJob = jobQueue.GetNextJob(new Vector2(currentX, currentY), failedJobs);

            DoJobAtVisibleCharacter("Guard", "Passenger", "Builder");

            if (targetJob == null)
            {
                // We are idle, wander the ship 
                Action idleAction = delegate () { };
                targetJob = new Job(idleAction, WorldController.Instance.GetWorld().GetRandomHullTile(), 1f, "Wander", tileExcludeOtherJobs: false);
            }
        }
        
    }

    
}
