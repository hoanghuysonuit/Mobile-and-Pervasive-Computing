using UnityEngine;

public class ShakeScript : MonoBehaviour
{
    [Tooltip("Độ lệch vị trí theo x")]
    public float maxX;

    [Tooltip("Độ lệch vị trí theo y")]
    public float maxY;

    [Tooltip("Thời gian để rung")]
    public float time;

    [Tooltip("tọa độ lúc đầu")]
    Vector3 oriPos;

    float tick;

    void Start()
    {
        oriPos = transform.position;
        tick = 0;
    }

    void Update()
    {
        tick += Time.deltaTime;

        if (tick >= time)
        {
            transform.position = oriPos + new Vector3(Random.Range(0, maxX) * porn.Random1(), Random.Range(0, maxY) * porn.Random1());
            tick = 0;
        }
    }
}
