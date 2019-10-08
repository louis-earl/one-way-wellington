using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Notification : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image iconGO;
    public TextMeshProUGUI descriptionGO;
    public GameObject buttonParent;
    public Button buttonClose;

    private float lifeSpan;

    public void Destroy(float lifeSpan)
    {
        this.lifeSpan = lifeSpan;
        StartCoroutine("DestroyCountdown");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines();
        GetComponent<PanelAlpha>().alpha = 1;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        GetComponent<PanelAlpha>().alpha = 1;

        if (lifeSpan > 0)
        {
            StartCoroutine("DestroyCountdown");
        }
    }

    IEnumerator DestroyCountdown()
    {

        float t = 0 ;
        while (t < lifeSpan)
        {
            t += Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine("FadeAndDestroy");
    }

    IEnumerator FadeAndDestroy()
    {
        PanelAlpha panelAlpha = GetComponent<PanelAlpha>();
        while (panelAlpha.alpha > 0)
        {
            panelAlpha.alpha -= Time.unscaledDeltaTime * 0.5f;
            yield return new WaitForEndOfFrame();
        }

        Destroy(gameObject);
    }

}
