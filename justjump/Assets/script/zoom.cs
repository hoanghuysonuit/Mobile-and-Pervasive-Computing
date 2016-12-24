using UnityEngine;

public class zoom : MonoBehaviour
{
    [HideInInspector]
    public GameObject zoomLand;

    [Tooltip("khoảng cách tối thiểu giữa 2 land")]
    public float dis;

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Return))
        //    Zoom();
    }

    public void Zoom()
    {
        if (zoomLand)
        {
            float minX, maxX;

            landcore landCore = zoomLand.GetComponent<landcore>();
            if (landCore)
            {
                if (landCore.preLand) // zoom qua trái
                    minX = landCore.preLand.transform.position.x + porn.SpriteWidth(landCore.preLand) / 2 + dis;
                else
                    minX = landCore.transform.position.x - porn.SpriteWidth(landCore.gameObject) / 2;

                if (landCore.nextLand) // zoom qua phải
                {
                    maxX = landCore.nextLand.transform.position.x - porn.SpriteWidth(landCore.nextLand) / 2 - dis;

                    // tiến hành zoom
                    float size = maxX - minX;
                    zoomLand.transform.position = new Vector3(minX + size / 2, zoomLand.transform.position.y);
                    zoomLand.transform.localScale = new Vector3(size, 1, 1);
                }
                else // k có land next thì k thực hiện được
                    return;
            }
        }
    }
}