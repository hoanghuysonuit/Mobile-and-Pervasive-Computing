using UnityEngine;
using System.Collections.Generic;

public class landcore : MonoBehaviour
{
    [HideInInspector]    
    public GameObject nextLand; // land sau liền kề
    [HideInInspector]
    public GameObject preLand; // land trước liền kề
    [HideInInspector]
    public GameObject thenemy; // có enemy nào k

    [HideInInspector]
    public bool isNowLand; // có phải là land hiện tại đang điều khiển hay không

    [HideInInspector]
    public Rigidbody2D rig;

    [HideInInspector]
    public Vector3 oriPos; // tọa độ ban đầu

    jumper _jumper;

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        isNowLand = false;
        _jumper = FindObjectOfType<jumper>();
        oriPos = transform.position;
    }

    void Update()
    {
        if (isNowLand) // đang xử lý
        {
            if (_jumper.isAuto) // auto
            {
                if (rig.velocity.y < 0) // đang xuống
                {
                    if (transform.position.y < _jumper.bottomPos || OverMaxHeight())
                    {
                        rig.velocity *= -1;
                        _jumper.v0 = (oriPos.y - transform.position.y) / _jumper.vel2Dis * _jumper.game2realVel;
                    }
                }
                else // đang lên
                {
                    if (transform.position.y > oriPos.y)
                        _jumper.SetFly();
                }
            }
            else // manual
            {
                if (!_jumper.isNotStillPress) // giai đoạn còn nhấn
                {
                    // bắt đầu k còn nhấn 
                    if (!helper.JumpButtonHolding() || // k nhấn phím nữa
                        transform.position.y < _jumper.maxMovePos.position.y || // vượt quá độ sâu cho phép của land
                        OverMaxHeight())   // khi nhấn quá độ cao tối đa cho phép thì hủy nhấn
                        SetManualFly();
                }
                else // giai đoạn k còn nhấn
                {
                    if (transform.position.y > oriPos.y) // land đã lên tới đỉnh
                        _jumper.SetFly();
                }
            }
        }
    }

    void SetManualFly()
    {
        _jumper.isNotStillPress = true;
        rig.velocity = new Vector2(0, _jumper.speedLandUp);
        _jumper.v0 = (oriPos.y - transform.position.y) / _jumper.vel2Dis * _jumper.game2realVel;
    }

    // true nếu độ cao sẽ bay vượt qua max height
    bool OverMaxHeight()
    {
        float dis = oriPos.y - transform.position.y;
        float v0 = dis / _jumper.vel2Dis * _jumper.game2realVel;
        float h = (float)porn.NemXieng_Max_H(v0, _jumper.angle);

        if (h > _jumper.maxHeightFly.position.y - oriPos.y)
            return true;

        return false;
    }
}
