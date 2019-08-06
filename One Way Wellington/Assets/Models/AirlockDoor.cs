using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AirlockDoor : MonoBehaviour
{
    void OnTriggerStay(Collider other)
    {

        if (other.gameObject.tag == "Character")
        {
            // Disable collision 
            gameObject.GetComponent<NavMeshObstacle>().enabled = false;

            // Open door texure
            RuleTile t = Resources.Load<RuleTile>("TileSets/Furniture/Airlock_Open");
            FurnitureSpriteController.Instance.tilemap.SetTile(new Vector3Int((int) gameObject.transform.position.x, (int) gameObject.transform.position.y, 0), t);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Character")
        {
            // Enable collision 
            gameObject.GetComponent<NavMeshObstacle>().enabled = true;

            // Close door texure
            RuleTile t = Resources.Load<RuleTile>("TileSets/Furniture/Airlock");
            FurnitureSpriteController.Instance.tilemap.SetTile(new Vector3Int((int)gameObject.transform.position.x, (int)gameObject.transform.position.y, 0), t);
        }
    }
}
