using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoController : MonoBehaviour
{
    public static CargoController Instance;
    private Dictionary<string, int> undeliveredStock;

    public Dictionary<string, int> stocktake;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null) Instance = this;
        stocktake = new Dictionary<string, int>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void DeliverItems()
    {
        Debug.LogError("Method incomplete");
    }

    // Assumes the payment has already been made !
    public void PlaceOrder(string itemType, int quantity)
    {
        if (undeliveredStock.ContainsKey(itemType))
        {
            undeliveredStock[itemType] += quantity;
        }
        else
        {
            undeliveredStock.Add(itemType, quantity);
        }
    }
}
