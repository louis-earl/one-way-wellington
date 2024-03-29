﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectiveUI : MonoBehaviour
{
    public TextMeshProUGUI text_Title;
    public TextMeshProUGUI text_OnComplete;

    public Image panel_OnComplete;

    public Transform goalsParent;
    public GameObject buttonClose;

    public Objective objectiveReference;

    public bool isComplete;

    private void OnEnable()
    {
        panel_OnComplete.color = new Color(panel_OnComplete.color.r, panel_OnComplete.color.g, panel_OnComplete.color.b, 0);
        buttonClose.SetActive(isComplete);

        if (isComplete)
        {
            StartBlinking();
        }
    }

    private void Start()
    {
        buttonClose.SetActive(isComplete);
        panel_OnComplete.color = new Color(panel_OnComplete.color.r, panel_OnComplete.color.g, panel_OnComplete.color.b, 0);
    }

    IEnumerator Blink()
    {
        while (true)
        {
            switch (panel_OnComplete.color.a.ToString())
            {
                case "0":
                    panel_OnComplete.color = new Color(panel_OnComplete.color.r, panel_OnComplete.color.g, panel_OnComplete.color.b, 1);
                    //Play sound
                    yield return new WaitForSecondsRealtime(0.5f);
                    break;
                case "1":
                    panel_OnComplete.color = new Color(panel_OnComplete.color.r, panel_OnComplete.color.g, panel_OnComplete.color.b, 0);
                    //Play sound
                    yield return new WaitForSecondsRealtime(0.5f);
                    break;
            }
        }
    }

    public void StartBlinking()
    {
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine("Blink");
        }
    }

    public void StopBlinking()
    {
        StopAllCoroutines();
    }
}
