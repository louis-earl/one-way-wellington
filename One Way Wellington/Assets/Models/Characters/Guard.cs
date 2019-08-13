using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Guard : Staff
{
    
    protected override void Init()
    {
        // Call from superclass
        base.Init();

        // Setup from here onwards
        jobQueue = JobQueueController.GuardsJobQueue;
    }

    protected override void Refresh()
    {
        // Call from superclass
        base.Refresh();

        // Program from here onwards




        /*
         * Find and set a target/current jobs
         * We don't want global jobs for Guards at this stage
        if (targetJob == null)
        {
            targetJob = jobQueue.GetNextJob(new Vector2(currentX, currentY), failedJobs);
            if (targetJob == null)
            {
                // We are idle
                // Go to storage and enter low power mode 
            }
        }

        */


        // Find enemy 


    }
}
