using UnityEngine;

// 22.11.2016

public class TwinklingScript : MonoBehaviour
{
    TimeElapsed tick;
    SpriteRenderer spriteRenderer;
    MeshRenderer meshRenderer;
    bool is2D;

    [Tooltip("tốc độ nhấp nháy")]
    public float twinklingSpeed;

    void Start()
    {
        tick = new TimeElapsed(twinklingSpeed);

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (!spriteRenderer)
        {
            meshRenderer = gameObject.GetComponent<MeshRenderer>();
            is2D = false;
        }
        else
            is2D = true;
    }

    void Update()
    {
        twinklingSpeed = tick.SetTime(twinklingSpeed);

        if (tick.PingPong())
        {
            if (is2D)
                spriteRenderer.enabled = !spriteRenderer.enabled;
            else
                meshRenderer.enabled = !meshRenderer.enabled;
        }
    }
}
