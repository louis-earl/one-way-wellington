using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Job
{
    protected Job prerequisiteJob;
    protected TileOWW tileOWW;
    protected float jobTime;
    protected string jobType;
    protected Action action;

    public Job(Action action, TileOWW tileOWW, float jobTime, string jobType, Job prerequisiteJob = null)
    {
        this.action = action;
        this.tileOWW = tileOWW;
        this.jobTime = jobTime;
        this.jobType = jobType;
        this.prerequisiteJob = prerequisiteJob;
        tileOWW.currentJobType = jobType;
    }

    // Constructor used when loading game, because actions can't be serialized 
    public Job(TileOWW tileOWW, float jobTime, string jobType, Job prerequisiteJob = null)
    {
        Action actionNew = null;
        if (jobType == "Hull")
        {
            actionNew = delegate () { BuildModeController.Instance.PlaceHull(tileOWW); };
        }
        else if (BuildModeController.Instance.furnitureTypes.ContainsKey(jobType))
        {
            actionNew = delegate () { BuildModeController.Instance.PlaceFurniture(tileOWW, jobType); };
        }
        else if (jobType == "removeFurniture")
        {
            actionNew = delegate () { BuildModeController.Instance.RemoveFurniture(tileOWW); };
        }
        else if (jobType == "removeHull")
        {
            actionNew = delegate () { BuildModeController.Instance.RemoveHull(tileOWW); };
        }
        else
        {
            Debug.LogWarning("The job type: " + jobType + " loaded is not present in the Job constructor!!");
        }
        this.action = actionNew;
        this.tileOWW = tileOWW;
        this.jobTime = jobTime;
        this.jobType = jobType;
        this.prerequisiteJob = prerequisiteJob;
        tileOWW.currentJobType = jobType;
    } 

    public Vector3 GetLocation()
    {
        return new Vector3(tileOWW.GetX() + 0.5f, tileOWW.GetY() + 0.5f, 0);
    }

    public TileOWW GetTileOWW()
    {
        return tileOWW;
    }

    public bool DoJob(float deltaTime)
    {
        jobTime -= deltaTime;
        if (jobTime <= 0)
        {
            // tileOWW.currentJob = null;
            JobSpriteController.Instance.UpdateJob(tileOWW);
            action?.Invoke();
            tileOWW.currentJobType = null;
            return true;
        }
        return false;
    }

    public string GetJobType()
    {
        return jobType;
    }

    public Job GetPrerequisiteJob()
    {
        if (prerequisiteJob == null || prerequisiteJob.jobTime <= 0) return null;
        else if (prerequisiteJob.GetPrerequisiteJob() == null) return prerequisiteJob;
        else return prerequisiteJob.GetPrerequisiteJob();
 
    }

    // Saving Job objects not supported! Use JobSerializable instead
    public JobSerializable ToJobSerializable()
    {
        JobSerializable p = null;
        if (prerequisiteJob != null)
        {
            p = prerequisiteJob.ToJobSerializable();
        }
        return new JobSerializable(tileOWW, jobTime, jobType, p); 
    }
}
