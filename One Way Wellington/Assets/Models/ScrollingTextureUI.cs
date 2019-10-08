using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ScrollingTextureUI : MonoBehaviour
{

    public float scrollSpeed = 0.5f;
    Image image;

    void Start()
    {
        image = GetComponent<Image>();
    }

    void Update()
    {
        float offset = Time.time * scrollSpeed;
        image.material.SetTextureOffset("_MainTex", new Vector2(offset, 0));
    }
}
