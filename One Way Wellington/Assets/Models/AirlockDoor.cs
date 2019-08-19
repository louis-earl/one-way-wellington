using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AirlockDoor : MonoBehaviour
{
    void OnTriggerStay2D(Collider2D other)
    {

        if (other.gameObject.tag == "Passenger" || other.gameObject.tag == "Guard" || other.gameObject.tag == "Builder")
        {
            // Disable collision 
            gameObject.GetComponent<NavMeshObstacle>().enabled = false;

            // Open door texure
            RuleTile t = Resources.Load<RuleTile>("TileSets/Furniture/Airlock_Open");
            FurnitureSpriteController.Instance.tilemap.SetTile(new Vector3Int((int) gameObject.transform.position.x, (int) gameObject.transform.position.y, 0), t);

            // Allow oxygen flow 
            WorldController.Instance.GetWorld().GetTileAt((int)gameObject.transform.position.x, (int)gameObject.transform.position.y).GetInstalledFurniture().SetFurnitureType("Airlock_Open");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Passenger" || other.gameObject.tag == "Staff")
        {
            // Enable collision 
            gameObject.GetComponent<NavMeshObstacle>().enabled = true;

            // Close door texure
            RuleTile t = Resources.Load<RuleTile>("TileSets/Furniture/Airlock");
            FurnitureSpriteController.Instance.tilemap.SetTile(new Vector3Int((int)gameObject.transform.position.x, (int)gameObject.transform.position.y, 0), t);

            // Block oxygen flow 
            WorldController.Instance.GetWorld().GetTileAt((int)gameObject.transform.position.x, (int)gameObject.transform.position.y).GetInstalledFurniture().SetFurnitureType("Airlock");
        }
    }
}
