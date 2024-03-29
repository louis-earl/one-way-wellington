﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class FurnitureListing : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI text;
    public Image image;
    public Toggle toggle;
    public string description;
    public int price;


    public void InputFurnitureType(FurnitureType furniture)
    {
        // Set Text 
        text.text = furniture.title;

        // Set Image 
        image.sprite = Resources.Load<Sprite>("Images/Furniture/" + furniture.title.ToLower());

        // Set OnClick Action 
        toggle.group = gameObject.transform.parent.GetComponent<ToggleGroup>();
        toggle.onValueChanged.AddListener( delegate { InputController.Instance.ToggleMode_Furniture(furniture); });


        this.price = furniture.cost;
        this.description = furniture.description;
    }


    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        // Price 
        if (price > 0)
        {
            UserInterfaceController.Instance.pricePopUpInstance = Instantiate(UserInterfaceController.Instance.pricePopUpPrefab);
            UserInterfaceController.Instance.pricePopUpInstance.GetComponentInChildren<TextMeshProUGUI>().text = string.Format("{0:C}", price);
            UserInterfaceController.Instance.pricePopUpInstance.transform.SetParent(UserInterfaceController.Instance.canvas_Main.transform);
            UserInterfaceController.Instance.pricePopUpInstance.transform.position = gameObject.transform.position + new Vector3(0, 74 * UserInterfaceController.Instance.canvas_Main.transform.localScale.x);
            UserInterfaceController.Instance.pricePopUpInstance.transform.localScale = Vector3.one;
        }
        // Description should already be active 
        UserInterfaceController.Instance.tooltipInstance.GetComponentInChildren<TextMeshProUGUI>().text = description;
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        UserInterfaceController.Instance.tooltipInstance.GetComponentInChildren<TextMeshProUGUI>().text = UserInterfaceController.Instance.toolTipText;
        Destroy(UserInterfaceController.Instance.pricePopUpInstance);
    }
}
