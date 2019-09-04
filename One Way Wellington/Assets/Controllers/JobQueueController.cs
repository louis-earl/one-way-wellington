using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobQueueController : MonoBehaviour
{
    public static JobQueueController Instance;

    public static JobQueue BuildersJobQueue;
    public static JobQueue GuardsJobQueue;
    public static JobQueue PassengersJobQueue;
    public static JobQueue OrcsJobQueue;

    private void Start()
    {
        if (Instance == null) Instance = this;
        if (BuildersJobQueue == null) BuildersJobQueue = new JobQueue();
        if (GuardsJobQueue == null) GuardsJobQueue = new JobQueue();
        if (PassengersJobQueue == null) PassengersJobQueue = new JobQueue();
        if (OrcsJobQueue == null) OrcsJobQueue = new JobQueue();
    }

    public void ClearAllJobs()
    {
        BuildersJobQueue = new JobQueue();
        GuardsJobQueue = new JobQueue();
        PassengersJobQueue = new JobQueue();
        OrcsJobQueue = new JobQueue();
    }


	public string ConvertJobTypeToFurnitureType(string jobType)
	{
		string furnitureType = string.Empty;
		if (jobType.Contains("Build"))
		{
			furnitureType = jobType.Replace("Build ", string.Empty);
			// Debug.Log(tileOWW.currentJobType + " -> " + currentTileType);
		}
		else
		{
			Debug.LogWarning(jobType + " couldn't be converted to a furnitureType.");
			return null;
		}

		return furnitureType;
	}
}
