using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectiveUI : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI onComplete;
    public Transform goalsParent;
    public GameObject buttonClose;

    public Objective objectiveReference;


    private void Start()
    {
        buttonClose.SetActive(false);
    }
}
