using UnityEngine;

public class background : MonoBehaviour
{
    public GameObject bgrPrejab;
    GameObject lastBackground;
    float halfWidthCamera; // nửa chiều rộng màn hình
    Camera _camera;
    float halfWidthBg;

    void Start ()
    {
        lastBackground = null;
        porn.FitSize(bgrPrejab, 0, FindObjectOfType<Camera>().orthographicSize * 2, true);
        _camera = FindObjectOfType<Camera>();
        halfWidthCamera = _camera.aspect * _camera.orthographicSize;
        halfWidthBg = porn.SpriteWidth(bgrPrejab) / 2;
    }
	
	void Update ()
    {
        // tạo background mới
        if (!lastBackground || lastBackground.transform.position.x + halfWidthBg < _camera.transform.position.x + halfWidthCamera +  2)
        {
            GameObject newobj;

            if (!lastBackground)
                newobj = (GameObject)Instantiate(bgrPrejab, new Vector3(), Quaternion.identity);
            else
                newobj = (GameObject)Instantiate(bgrPrejab, lastBackground.transform.position + new Vector3(halfWidthBg * 2, 0), Quaternion.identity);

            lastBackground = newobj;
        }
	}
}
