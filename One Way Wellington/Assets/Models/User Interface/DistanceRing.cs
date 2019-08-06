using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceRing : MonoBehaviour
{


    public int segments = 100;

    public float xradius = 5;

    public float yradius = 5;
    LineRenderer line;

    void Start()
    {

        
    }

    public void CreatePoints()
    {
        line = gameObject.GetComponent<LineRenderer>();
        line.positionCount = segments + 1;
        line.useWorldSpace = false;



        float x;
        float z;

        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * xradius;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * yradius;

            line.SetPosition(i, new Vector3(x, 0, z));

            angle += (360f / segments);
        }
    }
}

