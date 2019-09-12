using System;
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
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.Lerp(new Vector3(Camera.main.orthographicSize / 500, Camera.main.orthographicSize / 500, 1), transform.localScale, 0.8f);
        transform.localPosition = new Vector3(tile.GetX() + 0.85f, tile.GetY() + 0.5f, 0);

        tileType.text = tile.GetTileType();

        // Installed furniture 
        if (tile.GetInstalledFurniture() != null)
        {
            if (furniturePanelGO == null)
            {
                furniturePanelGO = Instantiate(furniturePanelPrefab, parentGO.transform);
            }
            furniturePanelGO.GetComponent<FurniturePanel>().furnitureType.text = tile.GetInstalledFurniture().GetFurnitureType();
        }
        /*
        // Installed furniture alt position 
        else if (tile.installedFurnitureAltX != null && tile.installedFurnitureAltY != null)
        {
            if (tile.installedFurnitureAltX != tile.GetX() || tile.installedFurnitureAltY != tile.GetY())
            {
                if (furniturePanelGO == null)
                {
                    furniturePanelGO = Instantiate(furniturePanelPrefab, parentGO.transform);
                }
                TileOWW furnitureTile = WorldController.Instance.GetWorld().GetTileAt((int)tile.installedFurnitureAltX, (int)tile.installedFurnitureAltY);
                furniturePanelGO.GetComponent<FurniturePanel>().furnitureType.text = furnitureTile.GetInstalledFurniture().GetFurnitureType() + " (from alt tile)";
            }
        }
        */
        else
        {
            if (furniturePanelGO != null)
            {
                Destroy(furniturePanelGO);
            }
        }

        // Current job
        if (tile.currentJobType != null)
        {
            if (jobPanelGO == null)
            {
                jobPanelGO = Instantiate(jobPanelPrefab, parentGO.transform);
            }
            jobPanelGO.GetComponent<JobPanel>().jobType.text = tile.currentJobType;
        }
        else
        {
            if (jobPanelGO != null)
            {
                Destroy(jobPanelGO);
            }
        }

        // Cargo 
        if (tile.looseItem != null)
        {
            if (looseItemGO == null)
            {
                looseItemGO = Instantiate(looseItemPrefab, parentGO.transform);
            }
            looseItemGO.GetComponent<LooseItemPanel>().cargoType.text = tile.looseItem.itemType;
            looseItemGO.GetComponent<LooseItemPanel>().quantity.text = tile.looseItem.quantity.ToString();
        }
        else
        {
            if (looseItemGO != null)
            {
                Destroy(looseItemGO);
            }
        }
    }
}
