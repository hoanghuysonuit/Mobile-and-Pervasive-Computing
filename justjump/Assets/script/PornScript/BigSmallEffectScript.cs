using UnityEngine;

public class BigSmallEffectScript : MonoBehaviour
{
    [Tooltip("Có active hay không?")]
    public bool active;

    [Tooltip("Có update liên tục hay không?")]
    public bool updateValues;

    [Tooltip("Phần trăm scale nhỏ. Càng lớn chệnh lệch size càng lớn")]
    public uint percent4SmallScale;

    [Tooltip("Phần trăm scale lớn. Càng lớn chệnh lệch size càng lớn")]
    public uint percent4BigScale;

    [Tooltip("Phần trăm dựt. Càng lớn càng như chơi thuốc, lol"), Range(0, 100)]
    public uint percentEffect;

    float minX, maxX; // min, max scale theo x
    Vector3 originScale;

    void Start()
    {
        originScale = transform.localScale;
        minX = originScale.x * percent4SmallScale / 100;
        maxX = originScale.x * percent4BigScale / 100;
    }

    void Update()
    {
        if (active)
        {
            if (updateValues)
            {
                minX = originScale.x * percent4SmallScale / 100;
                maxX = originScale.x * percent4BigScale / 100;
            }

            if (porn.RandomBool((int)percentEffect))
            {
                float newXScale;

                if (porn.RandomBool()) // to smaller
                    newXScale = Random.Range(-minX, 0);
                else // to bigger
                    newXScale = Random.Range(0, maxX);

                float newYScale = newXScale * originScale.y / originScale.x;
                float newZScale = newXScale * originScale.z / originScale.x;

                transform.localScale = originScale + new Vector3(newXScale, newYScale, newZScale);
            }
        }
        else if (transform.localScale != originScale)
            transform.localScale = originScale;
    }
}
