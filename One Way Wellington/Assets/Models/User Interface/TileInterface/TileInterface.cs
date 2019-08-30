using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TileInterface : MonoBehaviour
{
    public TileOWW tile;

    public TextMeshProUGUI tileType;
    public GameObject parentGO;


    public GameObject furniturePanelPrefab;
    private GameObject furniturePanelGO;

    public GameObject jobPanelPrefab;
    private GameObject jobPanelGO;

    public GameObject looseItemPrefab;
    private GameObject looseItemGO;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = Vector3.zero;

        if (tile.GetInstalledFurniture() != null)
        {
            furniturePanelGO = Instantiate(furniturePanelPrefab, parentGO.transform);
            furniturePanelGO.GetComponent<FurniturePanel>().furnitureType.text = tile.GetInstalledFurniture().GetFurnitureType();
        }

        if (tile.currentJobType != null)
        {
            jobPanelGO = Instantiate(jobPanelPrefab, parentGO.transform);
            jobPanelGO.GetComponent<JobPanel>().jobType.text = tile.currentJobType;
        }

        if (tile.looseItem != null)
        {
            looseItemGO = Instantiate(looseItemPrefab, parentGO.transform);
            looseItemGO.GetComponent<LooseItemPanel>().cargoType.text = tile.looseItem.itemType;
            looseItemGO.GetComponent<LooseItemPanel>().quantity.text = tile.looseItem.quantity.ToString();
        }

    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.Lerp(new Vector3(Camera.main.orthographicSize / 500, Camera.main.orthographicSize / 500, 1), transform.localScale, 0.8f);
        transform.localPosition = new Vector3(tile.GetX() + 0.85f, tile.GetY() + 0.5f, 0);
    }
}
