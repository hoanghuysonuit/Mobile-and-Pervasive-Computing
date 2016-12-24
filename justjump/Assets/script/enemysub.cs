using UnityEngine;

public class enemysub : MonoBehaviour
{
    [HideInInspector]
    public GameObject landFollow;
    Vector3 preLandPos;

	void Update ()
    {
        // di chuyển theo land
        if (landFollow) 
        {
            if (preLandPos != landFollow.transform.position)
            {
                transform.position = new Vector3(transform.position.x, landFollow.transform.position.y + enemy.fixY);
                preLandPos = landFollow.transform.position;
            }
        }
        //else
        //    Destroy(gameObject);
	}
}
