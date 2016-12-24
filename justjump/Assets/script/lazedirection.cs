using UnityEngine;

public class lazedirection : MonoBehaviour
{
    public GameObject lazePrefab;
    public GameObject expectedVolazePrefab;
    GameObject laze, expectedLaze, nowLaze;
    jumper _jumper;
    float bottomCamera; // điểm dừng của chuỗi dir

    [HideInInspector]
    public bool active; // trạng thái laze direction

    [HideInInspector]
    public Vector3 jumperPos; // tọa độ bắt đầu cho chuỗi dir

    void Start()
    {
        _jumper = FindObjectOfType<jumper>();
        bottomCamera = -porn.CameraHeightHalf(); 
        porn.FitSize(lazePrefab, 0, porn.CameraHeightHalf(2));
        porn.FitSize(expectedVolazePrefab, 0, porn.CameraHeightHalf(2));
        SetActive(false);
    }

    void Update()
    {
        if (active && _jumper.nowLandCore && helper.JumpButtonHolding() && _jumper.nowLandCore.rig.velocity.y <= 0)
        {
            float v0 = (_jumper.nowLandCore.oriPos - _jumper.nowLandCore.transform.position).y / _jumper.vel2Dis * _jumper.game2realVel; // tính v0

            if (v0 < 1)
                return;

            // Tìm X cho tranform

            _jumper.haveDirections = _jumper.PerfectLandPressing();
            if (_jumper.haveDirections)
            {
                nowLaze = expectedLaze;
                laze.SetActive(false);
            }
            else
            {
                nowLaze = laze;
                expectedLaze.SetActive(false);
            }
            nowLaze.SetActive(true);

            Vector3 findX = porn.NemXieng_XFromY(v0, _jumper.angle, bottomCamera - _jumper.nowLandCore.transform.position.y);

            if (findX.z == 2)
                nowLaze.transform.position = new Vector3(findX.y + jumperPos.x, 0, 0);
            else if (findX.z == 1)
                nowLaze.transform.position = new Vector3(findX.x + jumperPos.x, 0, 0);          
        }
        else if (active && nowLaze)
            nowLaze.SetActive(false);
    }

    public void SetActive(bool status)
    {
        active = status;

        if (active)
        {
            laze = (GameObject)Instantiate(lazePrefab, _jumper.transform.position, Quaternion.identity);
            expectedLaze = (GameObject)Instantiate(expectedVolazePrefab, _jumper.transform.position, Quaternion.identity);
        }
        else if (laze || expectedLaze)
        {
            Destroy(laze);
            Destroy(expectedLaze);
        }
    }

    public void UpdateParams()
    {
        if (active)
            jumperPos = new Vector3(_jumper.transform.position.x, _jumper.transform.position.y);
    }
}
