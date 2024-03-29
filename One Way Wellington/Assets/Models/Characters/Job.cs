﻿using System;
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
    protected JobPriority jobPriority;
    public bool tileExcludeOtherJobs;

    // If a job location is different from the actual tile being worked on
    protected int jobPosX;
    protected int jobPosY;

    public Job(Action action, TileOWW tileOWW, float jobTime, string jobType, JobPriority jobPriority, Job prerequisiteJob = null, bool tileExcludeOtherJobs = true)
    {
        this.action = action;
        this.tileOWW = tileOWW;
        this.jobTime = jobTime;
        this.jobType = jobType;
        this.prerequisiteJob = prerequisiteJob;
        this.tileExcludeOtherJobs = tileExcludeOtherJobs;
        this.jobPriority = jobPriority;

        // The tile won't hold a reference to this job. Likely because a character is wandering.
        if (tileExcludeOtherJobs == true)
        {
            tileOWW.currentJobType = jobType;
        }

        // Set default job pos
        jobPosX = tileOWW.GetX();
        jobPosY = tileOWW.GetY();

        if (jobTime < 0.1f) Debug.LogWarning("Job time is too short! Prerequisites may be skipped");
    }

    // Chase jobs not restricted to a single tileOWW 
    public Job(Action action, Character character, float jobTime, string jobType, JobPriority jobPriority, Job prerequisiteJob = null)
    {
        this.action = action;
        this.character = character;
        this.jobTime = jobTime;
        this.jobType = jobType;
        this.prerequisiteJob = prerequisiteJob;
        this.jobPriority = jobPriority;

        tileExcludeOtherJobs = false;

    }


    // Constructor used when loading game, because actions can't be serialized 
    public Job(TileOWW tileOWW, float jobTime, string jobType, bool tileExcludeOtherJobs, JobPriority jobPriority, Job prerequisiteJob = null)
    {
        Action actionNew = null;
		if (jobType == "Build Hull")
		{
			actionNew = delegate () { BuildModeController.Instance.PlaceHull(tileOWW); };
		}
        else if (jobType == "Build Wall")
        {
            actionNew = delegate () { BuildModeController.Instance.PlaceFurniture(tileOWW, "Wall"); };
        }
        else if (BuildModeController.Instance.furnitureTypes.ContainsKey(jobType) && jobType.Contains("Build"))
        {
            actionNew = delegate () { BuildModeController.Instance.PlaceFurniture(tileOWW, jobType); };
        }
		else if (jobType == "Destroy Hull")
		{
			actionNew = delegate () { BuildModeController.Instance.RemoveHull(tileOWW); };
		}
		else if (BuildModeController.Instance.furnitureTypes.ContainsKey(jobType) &&  jobType.Contains("Remove"))
        {
            actionNew = delegate () { BuildModeController.Instance.RemoveFurniture(tileOWW); };
        }  
        else
        {
            Debug.LogWarning("The job type: " + jobType + " loaded is not present in the Job constructor!!");
        }
        this.action = actionNew;
        this.tileOWW = tileOWW;
        this.jobTime = jobTime;
        this.jobType = jobType;
        this.tileExcludeOtherJobs = tileExcludeOtherJobs;
        this.prerequisiteJob = prerequisiteJob;
        this.jobPriority = jobPriority;
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

    public JobPriority GetJobPriority()
    {
        return jobPriority;
    }

    public bool DoJob(float deltaTime, Character character)
    {
        jobTime -= deltaTime;
        if (jobTime <= 0)
        {
            action?.Invoke();

            // Prevent wander jobs removing build jobs 
            if (tileExcludeOtherJobs)
            {
                
                if (tileOWW != null)
                {
                    if (tileOWW.currentJobType != null)
                    {
                        // Prevent opening the tile to other jobs if the prerequisite job was completed, 
                        // there's still more jobs scheduled for this tile! 
                        if (tileOWW.currentJobType.Equals(jobType))
                        {
                            tileOWW.currentJobType = null;
                        }
                    }
                }
            }
            // Update the job sprite 
            JobSpriteController.Instance.UpdateJob(tileOWW);
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

    public void SetPrerequisiteJob(Job prerequisiteJob)
    {
        this.prerequisiteJob = prerequisiteJob;
    }

    // Saving Job objects not supported! Use JobSerializable instead
    public JobSerializable ToJobSerializable()
    {
        JobSerializable p = null;
        if (prerequisiteJob != null)
        {
            p = prerequisiteJob.ToJobSerializable();
        }
        return new JobSerializable(tileOWW.GetX(), tileOWW.GetY(), jobPosX, jobPosY, jobTime, jobType, jobPriority, tileExcludeOtherJobs, p); 
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
