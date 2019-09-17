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

    public static NotificationController Instance;

    private List<GameObject> notifications;

    // Start is called before the first frame update
    void Start()
    { 
        if (Instance == null) Instance = this;

        notifications = new List<GameObject>();
    }

    public void CreateNotification(string description, UrgencyLevel urgencyLevel, bool destroyExisting, List<Action> buttonActions = null)
    {
        // Remove older occurances of the same notification 
        if (destroyExisting)
        {
            CloseNotification(FindNotificationGO(description));
        }


        GameObject notificationGO = Instantiate(notificationPrefab);

        notifications.Add(notificationGO);

        notificationGO.transform.SetParent(notificationParent.transform);
        notificationGO.transform.SetSiblingIndex(0);
		notificationGO.transform.localScale = Vector3.one;
        Notification notification = notificationGO.GetComponent<Notification>();
        notification.descriptionGO.text = description;
        if (buttonActions != null)
        {
            foreach (Action action in buttonActions)
            {
                GameObject buttonGO = Instantiate(buttonPrefab);
                buttonGO.transform.SetParent(notification.buttonParent.transform);
				buttonGO.transform.localScale = Vector3.one;
                buttonGO.GetComponent<Button>().onClick.AddListener(() => action.Invoke());
            }
        }

        // Colour based on urgency 
        if (urgencyLevel == UrgencyLevel.High)
        {
            notificationGO.GetComponent<Image>().color = new Color(1, 0.5679187f, 0.06666666f);
        }
        else if (urgencyLevel == UrgencyLevel.Medium)
        {
            notificationGO.GetComponent<Image>().color = new Color(1, 0.8666667f, 0.06666667f);
        }
        else if (urgencyLevel == UrgencyLevel.Low)
        {
            notificationGO.GetComponent<Image>().color = Color.white;
        }

        notification.buttonClose.onClick.AddListener(delegate () { CloseNotification(notificationGO); });
    }

    public void CloseNotification(GameObject notificationGO)
    {
        notifications.Remove(notificationGO);
        Destroy(notificationGO);
    }

    public GameObject FindNotificationGO(string notificationDescription)
    {
        foreach (GameObject notificationGO in notifications)
        {
            if (notificationGO.GetComponent<Notification>().descriptionGO.text.Equals(notificationDescription))
            {
                return notificationGO;
            }
        }
        return null;
    }



}
