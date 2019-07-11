using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobQueue
{
   private List<Job> jobs = new List<Job>();

    public Job GetNextJob(Vector2 characterPos, List<Job> failedJobs)
    {
        if (jobs.Count != 0)
        {
            int closestIndex = 0;
            int shortestDistance = 999;
            for (int i = 0; i < jobs.Count; i++)
            {
                if (!failedJobs.Contains(jobs[i]))
                {
                    int d = (int)Vector2.Distance(characterPos, new Vector2(jobs[i].GetLocation().x, jobs[i].GetLocation().y));
                    if (d < shortestDistance)
                    {
                        closestIndex = i;
                        shortestDistance = d;
                    }
                }
            }
            Job temp = jobs[closestIndex];
            jobs.RemoveAt(closestIndex);
            return temp;
        }
        else return null;
    }

    public void AddJob(Job job)
    {
        if (job != null)
        {
            if (job.GetJobType() != "recharge")
            {
                jobs.Add(job);
                JobSpriteController.Instance.UpdateJob(job.GetTileOWW());
            }
        }
    }

    public List<JobSerializable> GetSerializedJobs()
    {
        List<JobSerializable> serializedJobs = new List<JobSerializable>();
        foreach (Job j in jobs)
        {
            serializedJobs.Add(j.ToJobSerializable());
        }
        return serializedJobs;
    }

    public void SetJobsFromSerialized(List<JobSerializable> serializedJobs)
    {
        List<Job> jobsNew = new List<Job>();
        foreach (JobSerializable j in serializedJobs)
        {
            jobsNew.Add(j.ToJob());
        }
        this.jobs = jobsNew;
    }

}
