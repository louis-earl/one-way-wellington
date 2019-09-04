using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Captain : Staff
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

        if (targetJob == null || targetJob?.GetJobType() == "Wander")
        {

            DoJobAtFurnitureTile("ShipControls", "Use Ship Controls", delegate () { }, 10);


            if (targetJob == null)
            {
                // We are idle, wander the ship 
                Action attackAction = delegate () { };
                targetJob = new Job(attackAction, WorldController.Instance.GetWorld().GetRandomHullTile(), 1f, "Wander", tileExcludeOtherJobs: false);
            }
        }

    }

}
