using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PanelAlpha : MonoBehaviour
{

    Image image;
    [Range(0,1)]
    public float alpha;
    private List<Transform> currentChildren;

    // Start is called before the first frame update
    void Start()
    {
        image = gameObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (image.color.a != alpha)
        {
            currentChildren = new List<Transform>();
            GetChildren(transform);
            SetAlpha();
        }

    }

    private void GetChildren(Transform transform)
    {
        currentChildren.Add(transform);
        foreach (Transform child in transform)
        {
            GetChildren(child);
        }
    }

    public void SetAlpha()
    {
        foreach(Transform child in currentChildren)
        {
            if (child.gameObject.TryGetComponent<Image>(out Image image))
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
            }
        }
    }
}
