using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TileInterface : MonoBehaviour
{
    public TileOWW tile;

    public TextMeshProUGUI cargoType;
    public TextMeshProUGUI quantity;

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
    }
}
