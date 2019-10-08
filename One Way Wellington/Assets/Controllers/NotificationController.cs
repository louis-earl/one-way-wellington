using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


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

    public GameObject eventFeedParent;

    public Toggle toggle_Notification;

    public static NotificationController Instance;

    private List<GameObject> notifications;

    // Start is called before the first frame update
    void Start()
    { 
        if (Instance == null) Instance = this;

        notifications = new List<GameObject>();
    }

    private void Update()
    {
        if (eventFeedParent.transform.childCount == 0)
        {
            StopBlinking();
        }
    }

    public void CreateNotification(string description, UrgencyLevel urgencyLevel, bool destroyExisting, bool saveToNotifications = true, List<string> buttonTitles = null, List<Action> buttonActions = null)
    {
        // Remove older occurances of the same notification 
        if (destroyExisting)
        {
            CloseNotification(FindNotificationGO(description));
        }

        if (saveToNotifications)
        {
            GameObject notificationGO = CreateNotificationGO(notificationParent.transform, false, description, urgencyLevel, destroyExisting, buttonTitles, buttonActions);
            notifications.Add(notificationGO);
        }
        // Add to event feed if notifications panel not already open 
        if (!notificationParent.activeInHierarchy && !ObjectiveController.Instance.objectiveUIParent.activeInHierarchy)
        {
            GameObject eventFeedGO = CreateNotificationGO(eventFeedParent.transform, true, description, urgencyLevel, destroyExisting, buttonTitles, buttonActions);
            Canvas.ForceUpdateCanvases();
            eventFeedGO.transform.parent.GetComponent<ContentSizeFitter>().enabled = false;
            eventFeedGO.transform.parent.GetComponent<ContentSizeFitter>().enabled = true;
            eventFeedGO.GetComponent<Notification>().Destroy(5f);
            if (saveToNotifications)
            {
                StartBlinking();
            }
        }
    }

    private GameObject CreateNotificationGO(Transform parent, bool destroyOnClick, string description, UrgencyLevel urgencyLevel, bool destroyExisting, List<string> buttonTitles, List<Action> buttonActions)
    {
        GameObject notificationGO = Instantiate(notificationPrefab);

        notificationGO.transform.SetParent(parent);
        notificationGO.transform.SetSiblingIndex(0);
        notificationGO.transform.localScale = Vector3.one;
        Notification notification = notificationGO.GetComponent<Notification>();
        notification.descriptionGO.text = description;
        if (buttonActions != null)
        {
            if (buttonActions.Count != buttonTitles.Count)
            {
                Debug.LogError("Unmatched number of button titles to actions!");
            }
            foreach (Action action in buttonActions)
            {
                GameObject buttonGO = Instantiate(buttonPrefab);
                buttonGO.transform.SetParent(notification.buttonParent.transform);
                buttonGO.transform.localScale = Vector3.one;
                if (destroyOnClick)
                {
                    buttonGO.GetComponent<Button>().onClick.AddListener(() => action.Invoke());
                    buttonGO.GetComponent<Button>().onClick.AddListener(() => Destroy(notificationGO));
                }
                else
                {
                    buttonGO.GetComponent<Button>().onClick.AddListener(() => action.Invoke());
                }

                // Button texts
                buttonGO.GetComponentInChildren<TextMeshProUGUI>().text = buttonTitles[buttonActions.IndexOf(action)];

                
            }
        }

        // Colour based on urgency 
        if (urgencyLevel == UrgencyLevel.High)
        {
            notificationGO.GetComponent<Image>().color = new Color(1, 0.5679187f, 0.06666666f);
            notification.descriptionGO.color = Color.white;
        }
        else if (urgencyLevel == UrgencyLevel.Medium)
        {
            notificationGO.GetComponent<Image>().color = new Color(1, 0.8666667f, 0.06666667f);
            notification.descriptionGO.color = Color.black;

        }
        else if (urgencyLevel == UrgencyLevel.Low)
        {
            notificationGO.GetComponent<Image>().color = new Color(0.2313726f, 0.1529412f, 0.7294118f);
            notification.descriptionGO.color = Color.white;

        }

        notification.buttonClose.onClick.AddListener(delegate () { CloseNotification(notificationGO); });

        return notificationGO;
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

    IEnumerator Blink()
    {
        while (true)
        {
            if (toggle_Notification.colors.normalColor.r != 1)
            {
                ColorBlock colorBlock = toggle_Notification.colors;
                colorBlock.normalColor = new Color32(255, 221, 0, 255);
                toggle_Notification.colors = colorBlock;

                yield return new WaitForSecondsRealtime(0.5f);
            }
            else
            {
                ColorBlock colorBlock = toggle_Notification.colors;
                colorBlock.normalColor = new Color32(59, 39, 186, 255);
                toggle_Notification.colors = colorBlock;

                yield return new WaitForSecondsRealtime(0.5f);
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
        ColorBlock colorBlock = toggle_Notification.colors;
        colorBlock.normalColor = new Color32(59, 39, 186, 255);
        toggle_Notification.colors = colorBlock;
    }

}
