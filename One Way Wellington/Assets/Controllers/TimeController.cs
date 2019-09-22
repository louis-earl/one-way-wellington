using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeController : MonoBehaviour
{
    public static TimeController Instance;
    public float timeOWW;

    // Colours
    private static Color colourPressed = new Color(1f, 0.8666666f, 0f);
    private static Color colourNormal = new Color(0.2313726f, 0.1529412f, 0.7294118f);


    // Button images 
    public Toggle buttonPause;
    public Toggle buttonPlay;
    public Toggle buttonFF;
    public Toggle buttonFFF;

    // Time Display Panels
    public Image[] imagesTimeDisplay;


    // Time text 
    public TextMeshProUGUI timeDisplayHour10;
    public TextMeshProUGUI timeDisplayHour01;
    public TextMeshProUGUI timeDisplayMinute10;
    public TextMeshProUGUI timeDisplayMinute01;


    // Start is called before the first frame update
    void Start()
    {
        timeOWW = 0;
        if (Instance == null) Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        timeOWW += Time.deltaTime;
        if (timeOWW >= 1440) timeOWW = 0;
        //timeDisplay.text = ((int) timeOWW / 60).ToString("D2") + ":" + ((int) (timeOWW % 60)).ToString("D2");
        timeDisplayHour10.text = ((int)timeOWW / 60).ToString("D2").Substring(0,1);
        timeDisplayHour01.text = ((int)timeOWW / 60).ToString("D2").Substring(1,1);
        timeDisplayMinute10.text = ((int)timeOWW % 60).ToString("D2").Substring(0,1);
        timeDisplayMinute01.text = ((int)timeOWW % 60).ToString("D2").Substring(1,1);

        // Pause or resume with spacebar
        if (Input.GetButtonDown("Jump"))
        {
            if (Time.timeScale == 0)
            {
                buttonPlay.isOn = true;
            }
            else
            {
                buttonPause.isOn = true;
            }
        }
        // Time speed setting with alpha numeric keys
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            buttonPause.isOn = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            buttonPlay.isOn = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            buttonFF.isOn = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            buttonFFF.isOn = true;
        }
    }


    public void PauseTime()
    {
        StartBlinking();
        Time.timeScale = 0; 
    }

    public void ResumeTime()
    {
        StopBlinking();
        Time.timeScale = 1;
    }

    public void DoubleTime()
    {
        StopBlinking();
        Time.timeScale = 2;
    }

    public void QuadrupleTime()
    {
        StopBlinking();
        Time.timeScale = 4;
    }

    IEnumerator Blink()
    {
        while (true)
        {
            if (imagesTimeDisplay[0].color.g == 1)
            {
                foreach (Image image in imagesTimeDisplay)
                {
                    image.color = new Color(1, 0.8666667f, 0, 1);
                    
                }
                Debug.Log("yellow");
                //Play sound
                yield return new WaitForSecondsRealtime(0.5f);
            }
            else
            {
                foreach (Image image in imagesTimeDisplay)
                {
                    image.color = new Color(1, 1, 1, 1);
                    
                }
                Debug.Log("white");

                //Play sound
                yield return new WaitForSecondsRealtime(0.5f);
            }
        }
    }

    public void StartBlinking()
    {
        Debug.Log("start blinking");
        StartCoroutine("Blink");
    }

    public void StopBlinking()
    {
        StopCoroutine("Blink");
        foreach (Image image in imagesTimeDisplay)
        {
            image.color = new Color(1, 1, 1, 1);
        }
    }
}
