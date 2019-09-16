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

    }


    public void PauseTime(bool value)
    {
        Time.timeScale = 0;

    }

    public void ResumeTime(bool value)
    {
        Time.timeScale = 1;
    }

    public void DoubleTime(bool value)
    {
        Time.timeScale = 2;
    }

    public void QuadrupleTime(bool value)
    {
        Time.timeScale = 4;

    }
}
