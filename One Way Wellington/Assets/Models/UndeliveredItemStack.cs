using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndeliveredItemStack
{
  
    string itemType;
    int quantity;

    public UndeliveredItemStack(string itemType, int quantity)
    {
        this.itemType = itemType;
        this.quantity = quantity;
    }
}
