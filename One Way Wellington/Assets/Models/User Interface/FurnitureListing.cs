using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FurnitureListing : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Image image;
    public Button button;


    public void InputFurnitureType(FurnitureType furniture)
    {
        // Set Text 
        text.text = furniture.title;

        // Set Image 
        image.sprite = Resources.Load<Sprite>("Images/Furniture/" + furniture.title.ToLower());

        // Set OnClick Action 
        button.onClick.AddListener(() => InputController.Instance.ToggleMode_Furniture(furniture));
    } 
}
