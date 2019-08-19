using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Job
{
    protected Job prerequisiteJob;
    protected TileOWW tileOWW;
    protected Character character;
    protected float jobTime;
    protected string jobType;
    protected Action action;
    public bool tileExcludeOtherJobs;

    // If a job location is different from the actual tile being worked on
    protected int jobPosX;
    protected int jobPosY;

    public Job(Action action, TileOWW tileOWW, float jobTime, string jobType, Job prerequisiteJob = null, bool tileExcludeOtherJobs = true)
    {
        this.action = action;
        this.tileOWW = tileOWW;
        this.jobTime = jobTime;
        this.jobType = jobType;
        this.prerequisiteJob = prerequisiteJob;
        this.tileExcludeOtherJobs = tileExcludeOtherJobs;

        // The tile won't hold a reference to this job. Likely because a character is wandering.
        if (tileExcludeOtherJobs == true)
        {
            tileOWW.currentJobType = jobType;
        }

        // Set default job pos
        jobPosX = tileOWW.GetX();
        jobPosY = tileOWW.GetY();
    }

    // Chase jobs not restricted to a single tileOWW 
    public Job(Action action, Character character, float jobTime, string jobType, Job prerequisiteJob = null)
    {
        this.action = action;
        this.character = character;
        this.jobTime = jobTime;
        this.jobType = jobType;
        this.prerequisiteJob = prerequisiteJob;

        tileExcludeOtherJobs = false;

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
        if (tileOWW != null)
        {
            return new Vector3(jobPosX + 0.5f, jobPosY + 0.5f, 0);
        }
        else
        {
            return character.transform.position;
        }
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
            if (tileOWW != null)
            {
                tileOWW.currentJobType = null;
            }
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

    public void SetAltPosition(int x, int y)
    {
        jobPosX = x;
        jobPosY = y;
    }

    public int GetJobPosX()
    {
        return jobPosX;

    }

    public int GetJobPosY()
    {
        return jobPosY;
    }

    public Character GetCharacter()
    {
        return character;
    }

}
