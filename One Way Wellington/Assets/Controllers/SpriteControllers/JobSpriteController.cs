using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class JobSpriteController : MonoBehaviour
{
    public static JobSpriteController Instance;

    public Tilemap tilemap;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    public void UpdateJob(TileOWW tileOWW)
    {
        RuleTile t;
        if (tileOWW.currentJobType != null)
        {
            // Create job graphics 
            t = Resources.Load<RuleTile>("TileSets/Furniture/" + tileOWW.currentJobType);
            tilemap.SetTile(new Vector3Int(tileOWW.GetX(), tileOWW.GetY(), 0), t);
            tilemap.RefreshTile(new Vector3Int(tileOWW.GetX(), tileOWW.GetY(), 0));
        }
        else
        {
            // Remove tile graphics 
            t = null;
            tilemap.SetTile(new Vector3Int(tileOWW.GetX(), tileOWW.GetY(), 0), t);
            tilemap.RefreshTile(new Vector3Int(tileOWW.GetX(), tileOWW.GetY(), 0));
        }
    }
}
