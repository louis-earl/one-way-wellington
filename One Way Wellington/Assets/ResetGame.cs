using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ResetGame : MonoBehaviour
{

   public TextMeshProUGUI countdown;
    public float startTime;
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.unscaledTime;
        transform.localScale = new Vector3(0.0001f, 0.0001f);
        transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z + 10);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey)
        {
            Destroy(gameObject);
        }
           
        if (15.5 - (Time.unscaledTime - startTime) < 0 )
        {
            SceneManager.LoadScene("Start");
            Destroy(gameObject);
        }
        
        countdown.text = ((int)(15.5 - (Time.unscaledTime - startTime))).ToString();


        transform.localScale = Vector3.Lerp(new Vector3(Camera.main.orthographicSize / 300, Camera.main.orthographicSize / 300, 1), transform.localScale, 0.9f);
        transform.position = Vector3.Lerp(new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z + 10), transform.position, 0.9f);

    }

}
