using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum JobPriority
{
    User, // Top priority 
    High, // No dist
    Medium, // Within 50 tiles 
    Low // Within 35 tiles 
}

public class JobQueue
{
   private List<Job> jobs_user = new List<Job>();
   private List<Job> jobs_high = new List<Job>();
   private List<Job> jobs_med = new List<Job>();
   private List<Job> jobs_low = new List<Job>();

    public Job GetNextJob(Vector2 characterPos, List<Job> failedJobs)
    {

        Job nextJob = null;


        // User defined jobs 
        if (jobs_user.Count > 0)
        {      
            nextJob = GetClosestJob(jobs_user, characterPos, failedJobs);
            if (nextJob != null) return nextJob;
        }


        // High priority jobs 
        if (jobs_high.Count > 0)
        {
            nextJob = GetClosestJob(jobs_high, characterPos, failedJobs);
            if (nextJob != null) return nextJob;
        }


        // Medium priority jobs
        if (jobs_med.Count > 0)
        {
            nextJob = GetClosestJob(jobs_med, characterPos, failedJobs);
            if (nextJob != null) return nextJob;
        }


        // Low priotity jobs 
        if (jobs_low.Count > 0)
        {
            nextJob = GetClosestJob(jobs_low, characterPos, failedJobs);
            if (nextJob != null) return nextJob;
        }


        return null;
    }

    private Job GetClosestJob(List<Job> jobs, Vector2 characterPos, List<Job> failedJobs)
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
            else
            {
                // Debug.Log("Failed job: " + jobs[i].GetJobType());
            }
        }

        // Ensuring Job with index 0 is not automatically assigned after the fail-checks
        if (!failedJobs.Contains(jobs[0]))
        {
            Job temp = jobs[closestIndex];
            jobs.RemoveAt(closestIndex);
            return temp;
        }
        return null;
    }

    public void AddJob(Job job)
    {
        if (job != null)
        {
            if (job.GetJobPriority() == JobPriority.User)
            {
                jobs_user.Add(job);
            }
            else if (job.GetJobPriority() == JobPriority.High)
            {
                jobs_high.Add(job);
            }
            else if (job.GetJobPriority() == JobPriority.Medium)
            {
                jobs_med.Add(job);
            }
            else if (job.GetJobPriority() == JobPriority.Low)
            {
                jobs_low.Add(job);
            }

            JobSpriteController.Instance.UpdateJob(job.GetTileOWW());
        }
    }

    public List<JobSerializable> GetSerializedJobs()
    {
        List<JobSerializable> serializedJobs = new List<JobSerializable>();
        foreach (Job j in jobs_user)
        {

            serializedJobs.Add(j.ToJobSerializable());
            
        }
        foreach (Job j in jobs_high)
        {

            serializedJobs.Add(j.ToJobSerializable());

        }
        foreach (Job j in jobs_med)
        {

            serializedJobs.Add(j.ToJobSerializable());

        }
        foreach (Job j in jobs_low)
        {

            serializedJobs.Add(j.ToJobSerializable());

        }
        return serializedJobs;
    }


    public void SetJobsFromSerialized(List<JobSerializable> serializedJobs)
    {
        foreach (JobSerializable j in serializedJobs)
        {
            AddJob(j.ToJob());
        }
    }

    public bool TryDestroyJob(TileOWW tileOWW, JobPriority jobPriority)
    {
        List<Job> jobList = new List<Job>();
        if (jobPriority == JobPriority.User) jobList = jobs_user;
        else if (jobPriority == JobPriority.High) jobList = jobs_high;
        else if (jobPriority == JobPriority.Medium) jobList = jobs_med;
        else if (jobPriority == JobPriority.Low) jobList = jobs_low;

        // Check the jobList first
        foreach (Job job in jobList)
        {
            if (job.GetTileOWW() == tileOWW)
            {
                jobList.Remove(job);
                tileOWW.currentJobType = null;
                JobSpriteController.Instance.UpdateJob(tileOWW);
                return true;
            }
        }
        // Now check all staff 
        foreach (GameObject staffGO in WorldController.Instance.staff)
        {
            if (staffGO.GetComponent<Staff>().currentJob != null)
            {
                if (staffGO.GetComponent<Staff>().currentJob.GetTileOWW() == tileOWW)
                {
                    staffGO.GetComponent<Staff>().currentJob = staffGO.GetComponent<Staff>().targetJob = null;
                    tileOWW.currentJobType = null;
                    JobSpriteController.Instance.UpdateJob(tileOWW);
                    return true;
                }
            }
            if (staffGO.GetComponent<Staff>().targetJob != null)
            {
                if (staffGO.GetComponent<Staff>().targetJob.GetTileOWW() == tileOWW)
                {
                    staffGO.GetComponent<Staff>().currentJob = staffGO.GetComponent<Staff>().targetJob = null;
                    tileOWW.currentJobType = null;
                    JobSpriteController.Instance.UpdateJob(tileOWW);
                    return true;
                }
            }
        }

        return false;
    }

}
