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

    public JobSerializable(TileOWW tileOWW, int jobPosX, int jobPosY, float jobTime, string jobType, JobSerializable prerequisiteJob = null)
    {
        this.tilePosX = tileOWW.GetX();
        this.tilePosY = tileOWW.GetY();
        this.jobPosX = jobPosX;
        this.jobPosY = jobPosY;
        this.jobTime = jobTime;
        this.jobType = jobType;
        this.prerequisiteJob = prerequisiteJob;
    }

    public Job ToJob()
    {
        Job p = null;
        if (prerequisiteJob != null)
        {
            p = prerequisiteJob.ToJob();
        }

        Job job = new Job(WorldController.Instance.GetWorld().GetTileAt(tilePosX, tilePosY), jobTime, jobType, p);
        job.SetAltPosition(jobPosX, jobPosY);

        return job;
    }

   
}
