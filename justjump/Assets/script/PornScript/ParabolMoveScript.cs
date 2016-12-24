using UnityEngine;

public class ParabolMoveScript : MonoBehaviour
{
    [Tooltip("Có đang bay?")]
    bool isFlying;

    [Tooltip("Rig")]
    Rigidbody2D rig;

    [Tooltip("Tọa độ trước khi bay để làm gốc")]
    Vector2 beforeFlyingPos;

    [Tooltip("Vận tốc ban đầu (10)"), Range(0, 100)]
    public float v0;

    [Tooltip("Quy đổi từ khoảng cách di chuyển thành vận tốc để set veloc (50)")]
    public float dis2Vel;

    [Tooltip("Góc bay (65)")]
    public float angle;

    [Tooltip("Bay qua phải?")]
    public bool rightFly;

    // Nếu check VelByVo thì điền dividedVel, khỏi cần điền flyingVel
    // Nếu k thì ngược lại
    [Space(10), Tooltip("Vận tốc bay phụ thuộc vào Vo?")]
    public bool VelByVo;
    [Tooltip("Tỉ lệ vận tốc (60)")]
    public float dividedVel;
    [Tooltip("Tốc độ bay (Chỉ khi VelByVo ko check, 0.1)")]
    public float flyingVel;

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        isFlying = false;
        if (VelByVo)
            flyingVel = v0 / dividedVel;
    }

    void Update()
    {
        if (isFlying) // đang bay
        {
            Vector2 virNewPos = new Vector2(Mathf.Abs(transform.position.x - beforeFlyingPos.x) + flyingVel, 0);
            virNewPos.y = (float)porn.NemXieng_YFromX(v0, angle, virNewPos.x, true);

            Vector2 newDis;
            if (!rightFly)
                newDis = beforeFlyingPos + virNewPos - new Vector2(2 * beforeFlyingPos.x - transform.position.x, transform.position.y);
            else
                newDis = beforeFlyingPos + virNewPos - new Vector2(transform.position.x, transform.position.y);

            rig.velocity = newDis * dis2Vel;

            if (!rightFly)
                rig.velocity = new Vector2(-rig.velocity.x, rig.velocity.y);
        }
    }

    public void SetFly()
    {
        beforeFlyingPos = new Vector2(transform.position.x, transform.position.y);
        isFlying = true;
        rig.velocity = Vector2.zero;
    }

    public void SetFly(bool _rightFly)
    {
        SetFly();
        rightFly = _rightFly;
    }
}
