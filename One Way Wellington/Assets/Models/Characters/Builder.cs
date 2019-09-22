using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Builder : Staff
{

    protected override void Init()
    {
        // Call from superclass
        base.Init();

        // Setup from here onwards
        jobQueue = JobQueueController.BuildersJobQueue;
    }

    protected override void Refresh()
    {
        // Call from superclass
        base.Refresh();

        // Program from here onwards


        // Find and set a target/current jobs

        if (targetJob == null || targetJob?.GetJobType() == "Wander")
        {
            targetJob = jobQueue.GetNextJob(new Vector2(currentX, currentY), failedJobs);
            if (targetJob == null)
            {

                // We are idle
                // TODO: Go to storage and enter low power mode 

                // Get random hull tile 
                TileOWW randomHullTile = WorldController.Instance.GetWorld().GetRandomHullTile();
                if (randomHullTile != null)
                {
                    targetJob = new Job(delegate () { }, randomHullTile, 1, "Wander", JobPriority.Low, tileExcludeOtherJobs: false);
                }
            }

 
        }
        
    }
}
