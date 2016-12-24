using UnityEngine;

public class movenemysub : MonoBehaviour
{
    [HideInInspector]
    public GameObject landFollow;
    Vector3 preLandPos;
    [HideInInspector]
    public float minPos, maxPos;
    Rigidbody2D rig;

    void Start()
    {
        movenemy _movenemy = FindObjectOfType<movenemy>();
        rig = GetComponent<Rigidbody2D>();
        rig.velocity = new Vector2(Random.Range(_movenemy.minSpeed, _movenemy.maxSpeed), 0);
        transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    void Update()
    {
        // di chuyển theo land
        if (landFollow)
        {
            if (preLandPos != landFollow.transform.position) // y theo land
            {
                transform.transform.position = new Vector3(transform.position.x, landFollow.transform.position.y + movenemy.fixY);
                preLandPos = landFollow.transform.position;
            }

            // move qua lại
            if (transform.position.x < minPos)
            {
                rig.velocity = new Vector2(Mathf.Abs(rig.velocity.x), 0);
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (transform.position.x > maxPos)
            {
                rig.velocity = new Vector2(-Mathf.Abs(rig.velocity.x), 0);
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }
}
