using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
// Job objects must be converted to an instance of this class to be saved to file!!
public class JobSerializable
{
    protected JobSerializable prerequisiteJob;
    protected int tilePosX;
    protected int tilePosY;
    protected float jobTime;
    protected string jobType;
    protected bool tileExcludeOtherJobs;
    protected int jobPosX;
    protected int jobPosY;
    protected JobPriority jobPriority;

    public JobSerializable(int tilePosX, int tilePosY, int jobPosX, int jobPosY, float jobTime, string jobType, JobPriority jobPriority, bool tileExcludeOtherJobs = false, JobSerializable prerequisiteJob = null)
    {
        this.tilePosX = tilePosX;
        this.tilePosY = tilePosY;
        this.jobPosX = jobPosX;
        this.jobPosY = jobPosY;
        this.jobTime = jobTime;
        this.jobType = jobType;
        this.tileExcludeOtherJobs = tileExcludeOtherJobs;
        this.prerequisiteJob = prerequisiteJob;
        this.jobPriority = jobPriority;
    }

    public Job ToJob()
    {
        Job p = null;
        if (prerequisiteJob != null)
        {
            p = prerequisiteJob.ToJob();
        }

        TileOWW tile = WorldController.Instance.GetWorld().GetTileAt(tilePosX, tilePosY);

        if (tile == null)
        {
            Debug.LogWarning("Just made a job with no tile");
        }

        Job job = new Job(tile, jobTime, jobType, tileExcludeOtherJobs, jobPriority, p);
        job.SetAltPosition(jobPosX, jobPosY);

        return job;
    }


   
}
