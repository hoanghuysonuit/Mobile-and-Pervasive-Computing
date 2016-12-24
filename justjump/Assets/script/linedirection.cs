using UnityEngine;
using System.Collections.Generic;

public class linedirection : MonoBehaviour
{
    public GameObject linePrefab;
    List<GameObject> nowLine;
    jumper _jumper;
    float halfHightLand, pos;
    coin _coin;
    bool isTwinkling;
    float halfHightLine;

    [Tooltip("khoảng cách giữa now line và land để now line nhấp nháy")]
    public float twinklingDis;

    [Tooltip("tọa độ Y trừ hao")]
    public float addY;

    [Tooltip("tốc độ nhấp nháy lúc land sắp trùng")]
    public float twinklingSpeed;

    [HideInInspector]
    public bool active; // trạng thái line direction    

    void Start()
    {
        active = false;
        _jumper = FindObjectOfType<jumper>();
        halfHightLand = porn.SpriteHeight(FindObjectOfType<landcore>().gameObject) / 2;
        
        linePrefab.GetComponent<TwinklingScript>().twinklingSpeed = twinklingSpeed;
        _coin = FindObjectOfType<coin>();
        nowLine = new List<GameObject>();
        halfHightLine = porn.SpriteHeight(linePrefab) / 2;
    }

    void Update()
    {
        if (active)
        {
            // gần land thì rung

            if (_jumper.nowLandCore && nowLine.Count > 0) // đk để set
            {
                if (Mathf.Abs(_jumper.nowLandCore.transform.transform.position.y - pos) < twinklingDis) // gần land rồi nè
                {
                    if (!isTwinkling) // chưa rung
                    {
                        foreach (GameObject ele in nowLine)
                        {
                            ele.GetComponent<TwinklingScript>().enabled = true;
                        }

                        isTwinkling = true;
                        _jumper.haveDirections = true;
                    }
                }
                else // còn xa land
                {
                    if (isTwinkling) // đang rung
                    {
                        foreach (GameObject ele in nowLine)
                        {
                            ele.GetComponent<TwinklingScript>().enabled = false;
                        }

                        isTwinkling = false;
                        _jumper.haveDirections = false;
                    }
                }
            }

            if (!helper.JumpButtonHolding())
                DestroyLines();
        }
    }

    void DestroyLines()
    {
        foreach (GameObject ele in nowLine)
            Destroy(ele);

        nowLine.Clear();
    }

    public void SetActive(bool status)
    {
        active = status;

        if (nowLine.Count > 0)
            DestroyLines();
    }

    public void CreateLineDirection()
    {
        if (active)
        {
            float v0 = porn.NemXieng_FindVo(_jumper.angle, _jumper.nowLandCore.nextLand.transform.position.x - _jumper.transform.position.x, _jumper.nowLandCore.nextLand.transform.position.y - _jumper.transform.position.y, true);
            pos = _jumper.nowLandCore.oriPos.y - _jumper.vel2Dis * v0 + addY;
            float halfWidth = porn.SpriteWidth(_jumper.nowLandCore.gameObject) / 2;

            // tạo khung line direction

            // left
            nowLine.Add((GameObject)Instantiate(linePrefab, new Vector3(_jumper.nowLandCore.transform.position.x - halfWidth + halfHightLine, pos, 0), Quaternion.identity));
            nowLine[0].transform.Rotate(new Vector3(0, 0, -90));
            porn.FitSize(nowLine[0], halfHightLand * 2);

            // right
            nowLine.Add((GameObject)Instantiate(linePrefab, new Vector3(_jumper.nowLandCore.transform.position.x + halfWidth - halfHightLine, pos, 0), Quaternion.identity));
            nowLine[1].transform.Rotate(new Vector3(0, 0, -90));
            porn.FitSize(nowLine[1], halfHightLand * 2);

            // top
            nowLine.Add((GameObject)Instantiate(linePrefab, new Vector3(_jumper.nowLandCore.transform.position.x, pos + halfHightLand - halfHightLine, 0), Quaternion.identity));
            porn.FitSize(nowLine[2], halfWidth * 2);

            // bottom
            nowLine.Add((GameObject)Instantiate(linePrefab, new Vector3(_jumper.nowLandCore.transform.position.x, pos - halfHightLand + halfHightLine, 0), Quaternion.identity));
            porn.FitSize(nowLine[3], halfWidth * 2);
        }
    }
}
