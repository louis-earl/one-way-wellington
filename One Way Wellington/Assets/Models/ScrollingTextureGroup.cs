using UnityEngine;

public class ScrollingTextureGroup : MonoBehaviour
{
    public static ScrollingTextureGroup Instance;

    public float scrollSpeed;
    public SpriteRenderer[] spriteRenderers;



    // Use this for initialization
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        spriteRenderers = new SpriteRenderer[transform.childCount];
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i] = transform.GetChild(i).GetComponent<SpriteRenderer>();
        }
    }

    private void Update()
    {
        // Don't move the background in map mode, but the ship may continue to travel independently 
        if (TransitionController.Instance.isMapMode) SetSpeed(0);
        else SetSpeed(JourneyController.Instance.GetShipSpeedCurrent());

        for (int i = 1; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].transform.localPosition = new Vector3(Camera.main.transform.position.x * i * 0.05f, Camera.main.transform.position.y * i * 0.05f, 0);
        }
    }

    public void SetSpeed(float scrollSpeed)
    {
        this.scrollSpeed = scrollSpeed;

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].material.SetVector("_ScrollSpeed", new Vector2(scrollSpeed, 0) * i * 0.1f);
        }
    }
}
