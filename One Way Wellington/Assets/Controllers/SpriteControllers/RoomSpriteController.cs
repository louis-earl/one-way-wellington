using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomSpriteController : MonoBehaviour
{
    public static RoomSpriteController Instance;

    public Tilemap tilemap;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    public void UpdateAllRoom()
    {
        foreach (TileOWW tileOWW in WorldController.Instance.GetWorld().GetAllTiles())
        {
            UpdateRoom(tileOWW);
        }
    }

    public void UpdateRoom(TileOWW tileOWW)
    {
        RuleTile t;
        if (tileOWW.GetRoomType() != null)
        {
            // Create room graphics 
            t = Resources.Load<RuleTile>("TileSets/Rooms/" + tileOWW.GetRoomType());
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
