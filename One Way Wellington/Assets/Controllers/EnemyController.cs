using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    public static EnemyController Instance;

    public GameObject enemyParent;

    public GameObject orcPrefab;
    public GameObject piratePrefab;
    public GameObject piranhaPrefab;
    public GameObject ghostPrefab;


    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null) Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnEnemy()
    {

        // Find stairwell
        Vector3 stairwellPos = Vector3.zero;
        
        if (BuildModeController.Instance.furnitureTileOWWMap.ContainsKey("Stairwell"))
        {
            stairwellPos = new Vector3(BuildModeController.Instance.furnitureTileOWWMap["Stairwell"][0].GetX(), BuildModeController.Instance.furnitureTileOWWMap["Stairwell"][0].GetY(), 0);
        }
        else
        {
            Debug.LogWarning("Couldn't find a stairwell!!");
        }

        GameObject enemyGO = Instantiate(orcPrefab);
        enemyGO.transform.parent = enemyParent.transform;

        enemyGO.transform.position = stairwellPos;


    }

}
