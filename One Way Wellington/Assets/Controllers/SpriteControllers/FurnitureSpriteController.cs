using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.AI;
using UnityEngine.U2D;

public class FurnitureSpriteController : MonoBehaviour
{

    public static FurnitureSpriteController Instance;

    public Tilemap tilemap;

    //Dictionary<TileOWW, RuleTile> furnitureObjectMap = new Dictionary<TileOWW, RuleTile>();
    Dictionary<TileOWW, GameObject> tileNavBlockMap = new Dictionary<TileOWW, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    public void UpdateAllFurniture()
    {
        foreach (TileOWW tileOWW in WorldController.Instance.GetWorld().GetAllTiles())
        {
            UpdateFurniture(tileOWW);
        }
    }

    
    public void UpdateFurniture(TileOWW tileOWW)
    {
        RuleTile t;
        if (tileOWW.GetInstalledFurniture() != null)
        {
            // Create tile graphics 
            t = Resources.Load<RuleTile>("TileSets/Furniture/" + tileOWW.GetInstalledFurniture().GetFurnitureType());
            tilemap.SetTile(new Vector3Int(tileOWW.GetX(), tileOWW.GetY(), 0), t);
            tilemap.RefreshTile(new Vector3Int(tileOWW.GetX(), tileOWW.GetY(), 0));

            // Pathfinding 
            if (!tileOWW.GetIsWalkable())
            {
                // Create NavMeshObstacle GameObject 
                GameObject go = new GameObject();
                go.name = "NavMeshBlocking: (" + tileOWW.GetX() + " ," + tileOWW.GetY() + ")";
                go.transform.parent = Instance.transform;
                go.transform.position = new Vector3(tileOWW.GetX(), tileOWW.GetY(), 0);
                NavMeshObstacle nma = go.AddComponent<NavMeshObstacle>();
                nma.center = new Vector3(0.5f, 0.5f);
                
                // Set parameters depending on the furniture type 
                if (tileOWW.GetInstalledFurniture().GetFurnitureType() == "Wall")
                {
                    nma.carving = true;
                    nma.size = new Vector3(0.7f, 0.7f, 1f);
                }
                else if (tileOWW.GetInstalledFurniture().GetFurnitureType() == "Airlock")
                {
                    nma.size = new Vector3(1f, 1f, 1f);
                    go.AddComponent<AirlockDoor>();
                    BoxCollider2D boxCollider = go.AddComponent<BoxCollider2D>();
                    boxCollider.isTrigger = true;
                    boxCollider.offset = new Vector2(0.5f, 0.5f);
                    Rigidbody2D rigidbody = go.AddComponent<Rigidbody2D>();
                    rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;

                }
                tileNavBlockMap.Add(tileOWW, go);
            }
        }
        else
        {
            // Remove tile graphics 
            t = null;
            tilemap.SetTile(new Vector3Int(tileOWW.GetX(), tileOWW.GetY(), 0), t);
            tilemap.RefreshTile(new Vector3Int(tileOWW.GetX(), tileOWW.GetY(), 0));
            if (tileNavBlockMap.ContainsKey(tileOWW))
            {
                if (tileNavBlockMap[tileOWW] != null)
                {
                    // Remove NavMeshObstacle GameObject 
                    GameObject go = tileNavBlockMap[tileOWW];
                    tileNavBlockMap.Remove(tileOWW);
                    Destroy(go);

                }
            }
        }
    }
}
