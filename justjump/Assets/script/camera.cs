using UnityEngine;

public class camera : MonoBehaviour
{
    Rigidbody2D rig;
    landmover landMover;

    void Start ()
    {
        rig = GetComponent<Rigidbody2D>();
        landMover = FindObjectOfType<landmover>();
    }
	
	void Update ()
    {
        if (rig.velocity.x != landMover.nowLandSpeed) // di chuyển land
            rig.velocity = new Vector2(landMover.nowLandSpeed, 0);
    }
}
