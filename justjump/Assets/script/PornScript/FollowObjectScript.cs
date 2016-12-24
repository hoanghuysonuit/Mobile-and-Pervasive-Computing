using UnityEngine;

public class FollowObjectScript : MonoBehaviour
{

    [Tooltip("Object sẽ đi theo")]
    public GameObject Object;

    [Tooltip("Chệnh lệch vị trí")]
    public Vector3 offset;

    [Tooltip("Chệnh lệch vị trí mặc định?")]
    public bool defaultOffset;

    void Start()
    {
        if (defaultOffset)
            offset = transform.position - Object.transform.position;
    }

	void Update ()
    {
        transform.position = Object.transform.position + offset;
	}
}
