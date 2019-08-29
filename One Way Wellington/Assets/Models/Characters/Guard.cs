﻿using System;
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

        if (targetJob == null || targetJob?.GetJobType() == "wander")
        {
            // Not required to use global job queue yet 
            // targetJob = jobQueue.GetNextJob(new Vector2(currentX, currentY), failedJobs);

            DoJobAtVisibleCharacter("Enemy");

            if (targetJob == null)
            {
                // We are idle, wander the ship 
                Action attackAction = delegate () { };
                targetJob = new Job(attackAction, WorldController.Instance.GetWorld().GetRandomHullTile(), 1f, "wander", tileExcludeOtherJobs: false);
            }
        }

    }

}