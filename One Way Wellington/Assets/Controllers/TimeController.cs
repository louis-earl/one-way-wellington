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
                ResumeTime();
                buttonPlay.isOn = true;
            }
            else
            {
                PauseTime();
                buttonPause.isOn = true;
            }
        }
        // Time speed setting with alpha numeric keys
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            PauseTime();
            buttonPause.isOn = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ResumeTime();
            buttonPlay.isOn = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            DoubleTime();
            buttonFF.isOn = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            QuadrupleTime();
            buttonFFF.isOn = true;
        }
    }


    public void PauseTime()
    {
        Time.timeScale = 0;

    }

    public void ResumeTime()
    {
        Time.timeScale = 1;
    }

    public void DoubleTime()
    {
        Time.timeScale = 2;
    }

    public void QuadrupleTime()
    {
        Time.timeScale = 4;

    }
}
