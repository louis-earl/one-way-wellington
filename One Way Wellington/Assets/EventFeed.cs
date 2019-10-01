using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventFeed : MonoBehaviour
{

    public void DestroyAllChildren()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
