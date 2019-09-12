using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
// Job objects must be converted to an instance of this class to be saved to file!!
public class JobSerializable
{
    protected JobSerializable prerequisiteJob;
    protected int posX;
    protected int posY;
    protected float jobTime;
    protected string jobType;

    public JobSerializable(TileOWW tileOWW, float jobTime, string jobType, JobSerializable prerequisiteJob = null)
    {
        this.posX = tileOWW.GetX();
        this.posY = tileOWW.GetY();
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

        return new Job(WorldController.Instance.GetWorld().GetTileAt(posX, posY), jobTime, jobType, p);
    }

   
}
