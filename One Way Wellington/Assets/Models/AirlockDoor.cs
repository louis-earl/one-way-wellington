using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AirlockDoor : MonoBehaviour
{
    TileOWW tileOWW;

    private void Start()
    {
        tileOWW = WorldController.Instance.GetWorld().GetTileAt((int)gameObject.transform.position.x, (int)gameObject.transform.position.y);
    }

    void OnTriggerStay2D(Collider2D other)
    {

        if (other.gameObject.tag == "Passenger" || other.gameObject.tag == "Guard" || other.gameObject.tag == "Builder" || other.gameObject.tag == "Captain")
        {
            // Incase the installed furniture has changed
            if (tileOWW.GetInstalledFurniture()?.GetFurnitureType() == "Airlock")
            {

                // Disable collision 
                gameObject.GetComponent<NavMeshObstacle>().enabled = false;

                // Open door texure
                RuleTile t = Resources.Load<RuleTile>("TileSets/Furniture/Airlock Open");
                FurnitureSpriteController.Instance.tilemap.SetTile(new Vector3Int((int)gameObject.transform.position.x, (int)gameObject.transform.position.y, 0), t);

                // Allow oxygen flow 
                tileOWW.GetInstalledFurniture().SetFurnitureType("Airlock Open");
            }
            else if (tileOWW.GetInstalledFurniture()?.GetFurnitureType() == "Airlock Open")
            {
                // Do nothing 
            }
            else
            {
                Destroy(gameObject);
            }
        }

    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Passenger" || other.gameObject.tag == "Guard" || other.gameObject.tag == "Builder" || other.gameObject.tag == "Captain")
        {
            // Incase the installed furniture has changed 
            if (tileOWW.GetInstalledFurniture()?.GetFurnitureType() == "Airlock Open")
            {
                // Enable collision 
                gameObject.GetComponent<NavMeshObstacle>().enabled = true;

                // Close door texure
                RuleTile t = Resources.Load<RuleTile>("TileSets/Furniture/Airlock");
                FurnitureSpriteController.Instance.tilemap.SetTile(new Vector3Int((int)gameObject.transform.position.x, (int)gameObject.transform.position.y, 0), t);

                // Block oxygen flow 
                tileOWW.GetInstalledFurniture().SetFurnitureType("Airlock");
            }
            else if (tileOWW.GetInstalledFurniture()?.GetFurnitureType() == "Airlock")
            {
                // Do nothing 
            }
            else
            {
                Destroy(gameObject);
            }

        }
        
    }
}
