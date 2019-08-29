using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LooseItem
{
    public string itemType;
    public int quantity;

    public LooseItem(string itemType, int quantity)
    {
        this.itemType = itemType;
        this.quantity = quantity;
    }
}
