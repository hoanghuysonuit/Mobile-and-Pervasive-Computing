using UnityEngine;

public class StarMove : MonoBehaviour
{

    public float speed;

	void Start ()
    {
       
	}
	
	void Update ()
    {
        transform.localScale += new Vector3(speed, 0, 0);
	}
}
