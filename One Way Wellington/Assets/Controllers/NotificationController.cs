﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum UrgencyLevel
{
    Low, 
    Medium,
    High
}

public class NotificationController : MonoBehaviour
{
    public GameObject notificationParent;
    public GameObject notificationPrefab;
    public GameObject buttonPrefab;

    public static NotificationController Instance;

    //private List<GameObject> notifications;

    // Start is called before the first frame update
    void Start()
    { 
        if (Instance == null) Instance = this;
    }

    public void CreateNotification(string description, UrgencyLevel urgencyLevel, List<Action> buttonActions)
    {

        GameObject notificationGO = Instantiate(notificationPrefab);

        //notifications.Add(notificationGO);

        notificationGO.transform.SetParent(notificationParent.transform);
        notificationGO.transform.SetSiblingIndex(0);
        Notification notification = notificationGO.GetComponent<Notification>();
        notification.descriptionGO.text = description;
        if (buttonActions != null)
        {
            foreach (Action action in buttonActions)
            {
                GameObject buttonGO = Instantiate(buttonPrefab);
                buttonGO.transform.SetParent(notification.buttonParent.transform);
                buttonGO.GetComponent<Button>().onClick.AddListener(() => action.Invoke());
            }
        }
    }



}
