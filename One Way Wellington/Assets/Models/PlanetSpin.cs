using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetSpin : MonoBehaviour
{
    public GameObject[] surfaces;
    public float surfaceOffset = 0f;
    public float surfaceSpeed = 1f;

    public GameObject[] clouds;
    public float cloudOffset = 0.5f;
    public float cloudSpeed = 1.5f;

    public void InitSprites(Sprite surface, Sprite cloud)
    {
        gameObject.GetComponent<Planet>().SetSurface(surface);
        gameObject.GetComponent<Planet>().SetClouds(cloud);
        for (int i = 0; i < surfaces.Length; i++)
        {
            SpriteRenderer sr = surfaces[i].GetComponent<SpriteRenderer>();
            sr.sprite = surface;
            surfaces[i].transform.localPosition = new Vector3((i * sr.size.x) - surfaceOffset, surfaces[i].transform.localPosition.y, surfaces[i].transform.localPosition.z);       
        }
        for (int i = 0; i < clouds.Length; i++)
        {
            SpriteRenderer sr = clouds[i].GetComponent<SpriteRenderer>();
            sr.sprite = cloud;
            clouds[i].transform.localPosition = new Vector3((i * sr.size.x) - cloudOffset, clouds[i].transform.localPosition.y, clouds[i].transform.localPosition.z);
        }
    }

    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < surfaces.Length; i++)
        {
            GameObject texture = surfaces[i];
            SpriteRenderer sr = texture.GetComponent<SpriteRenderer>();

            texture.transform.localPosition = new Vector3(
                texture.transform.localPosition.x - (surfaceSpeed * Time.deltaTime),
                texture.transform.localPosition.y,
                texture.transform.localPosition.z);
            if (texture.transform.localPosition.x < -sr.size.x)
            {
                
                texture.transform.localPosition = new Vector3(sr.size.x - surfaceOffset, texture.transform.localPosition.y, texture.transform.localPosition.z);
            }
        }

        foreach (GameObject texture in clouds)
        {
            SpriteRenderer sr = texture.GetComponent<SpriteRenderer>();

            texture.transform.localPosition = new Vector3(
                texture.transform.localPosition.x - (cloudSpeed * Time.deltaTime),
                texture.transform.localPosition.y,
                texture.transform.localPosition.z);
            if (texture.transform.localPosition.x < -sr.size.x)
            {
                texture.transform.localPosition = new Vector3(sr.size.x - cloudOffset, texture.transform.localPosition.y, texture.transform.localPosition.z);
            }
        }

    }
}
