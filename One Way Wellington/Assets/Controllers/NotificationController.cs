using System;
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

    //private List<GameObject> notifications;

    // Start is called before the first frame update
    void Start()
    {
        List<Action> actions = new List<Action>
        {
            delegate () { EnemyController.Instance.SpawnEnemy(); }
        };

        CreateNotification("Testing!", UrgencyLevel.Low, actions);
    }

    public void CreateNotification(string description, UrgencyLevel urgencyLevel, List<Action> buttonActions)
    {

        GameObject notificationGO = Instantiate(notificationPrefab);

        //notifications.Add(notificationGO);

        notificationGO.transform.parent = notificationParent.transform;
        Notification notification = notificationGO.GetComponent<Notification>();
        notification.descriptionGO.text = description;

        foreach(Action action in buttonActions)
        {
            GameObject buttonGO = Instantiate(buttonPrefab);
            buttonGO.transform.parent = notification.buttonParent.transform;
            buttonGO.GetComponent<Button>().onClick.AddListener(() => action.Invoke());
        }
    }



}
