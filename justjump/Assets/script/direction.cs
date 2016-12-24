using UnityEngine;
using System.Collections.Generic;

public class direction : MonoBehaviour
{
    public float vTrim; // vận tốc khỏi vẽ
    public float dis; // khoảng cách giữa các dir
    public GameObject expectedVoPrefab; // prefab khi có expectedVo
    public GameObject dirPrefab; // vừa là prefab vừa là kích thước đầu của dir    
    public GameObject endSize; // kích thước cuối cùng của dir
    public float trim; // bỏ đoạn đầu    
    public float finalTrim; // bỏ đoạn cuối
    List<GameObject> list; // list dir
    jumper _jumper;
    float step; // bước duyệt
    float bottomCamera; // điểm dừng của chuỗi dir

    [HideInInspector]
    public bool active; // trạng thái dir

    [HideInInspector]
    public Vector3 jumperPos; // tọa độ bắt đầu cho chuỗi dir

    void Start()
    {
        list = new List<GameObject>();
        _jumper = FindObjectOfType<jumper>();
        active = false;
        trim += porn.SpriteHeight(_jumper.gameObject) / 2;
        finalTrim += porn.SpriteHeight(FindObjectOfType<landcore>().gameObject) / 2;
        step = 0.001f;
        bottomCamera = -FindObjectOfType<Camera>().orthographicSize;
    }

    void Update()
    {
        if (active && _jumper.nowLandCore && helper.JumpButtonHolding() && _jumper.nowLandCore.rig.velocity.y < 0)
        {
            ClearList(); // xóa đường cũ  
            float v0 = (_jumper.nowLandCore.oriPos - _jumper.nowLandCore.transform.position).y / _jumper.vel2Dis * _jumper.game2realVel; // tính v0

            if (v0 <= vTrim) // bằng 0 thì khỏi vẽ
                return;

            // tọa độ x, y để tạo
            float _x = step;
            float _y = 0;

            // bỏ phần đầu
            for (; Vector3.Distance(new Vector3(_x - step, _y), Vector3.zero) < trim; _x += step)
                _y = (float)porn.NemXieng_YFromX(v0, _jumper.angle, _x, true);
            _x -= step; // fix for        

            // expectedVo
            _jumper.haveDirections = _jumper.PerfectLandPressing();
            GameObject prefab = _jumper.haveDirections ? expectedVoPrefab : dirPrefab; // gần expectedVo thì đổi màu

            // tạo dir            
            Vector3 preDir = new Vector3(_x, _y, 0);
            list.Add((GameObject)Instantiate(prefab, jumperPos + preDir, Quaternion.identity));
            for (;;) // lặp cho đến khi tới end point
            {
                for (; ; _x += step) // lặp đến điểm dir tiếp theo
                {
                    _y = (float)porn.NemXieng_YFromX(v0, _jumper.angle, _x, true);
                    if (Vector3.Distance(new Vector3(_x, _y, 0), preDir) >= dis)
                    {
                        preDir = new Vector3(_x, _y, 0);
                        list.Add((GameObject)Instantiate(prefab, jumperPos + preDir, Quaternion.identity));
                        break;
                    }
                }

                // kiểm tra đến end dir chưa
                float minXNextLand = _jumper.nowLandCore.nextLand.transform.position.x - porn.SpriteWidth(_jumper.nowLandCore.nextLand) / 2;
                float maxXNextLand = _jumper.nowLandCore.nextLand.transform.position.x + porn.SpriteWidth(_jumper.nowLandCore.nextLand) / 2;
                float maxXNowLand = _jumper.nowLandCore.transform.position.x + porn.SpriteWidth(_jumper.nowLandCore.gameObject) / 2;
                if (((jumperPos + preDir).y - bottomCamera < 2) || // chạm đáy camera
                   ((jumperPos + preDir).x >= minXNextLand && (jumperPos + preDir).x <= maxXNextLand && (jumperPos + preDir).y <= _jumper.nowLandCore.nextLand.transform.position.y + finalTrim) || // chạm next land
                   ((jumperPos + preDir).x <= maxXNowLand && (jumperPos + preDir).y <= _jumper.nowLandCore.transform.position.y + finalTrim)) // chạm land hiện tại
                {
                    // tính lại scale
                    float discout = (prefab.transform.localScale.x - endSize.transform.localScale.x) / list.Count;
                    for (int i = 0; i < list.Count; i++)
                        list[i].transform.localScale -= new Vector3(i * discout, i * discout);
                    
                    break; // kết thúc list
                }
            }
        }
        else
            ClearList();
    }

    public void ClearList()
    {
        if (list.Count < 1)
            return;

        foreach (GameObject obj in list)
            Destroy(obj);
        list.Clear();
    }

    public void ActiveDirection(bool status)
    {
        active = status;

        if (!active)
            ClearList();
    }

    public void UpdateParams()
    {
        if (active)
            jumperPos = new Vector3(_jumper.transform.position.x, _jumper.transform.position.y);
    }
}
