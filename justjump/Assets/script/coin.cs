using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coin : MonoBehaviour
{
    public float fixCenter; // làm cho coin đầu và cuối ngay giữa
    public float dis; // khoảng cách giữa các coin
    public float appro; // sai số giữa điểm đến và coin cuối
    jumper _jumper;
    gamemanager _gamemanager;
    public float trim; // khoảng cách bỏ hai đầu để tránh coin trùng vs player
    public GameObject coinPrefab; // các loại coin để prefab
    [HideInInspector]
    public float step; // biến dùng cho việc quét
    float topCamera; // coin cao quá thì khỏi vẽ
    float halfHeightCoin;
    landcore preLandCore;
    List<GameObject> coinList, preCoinList;

    void Start()
    {
        _jumper = FindObjectOfType<jumper>();
        _gamemanager = FindObjectOfType<gamemanager>();
        trim += porn.SpriteHeight(_jumper.gameObject) + porn.SpriteHeight(FindObjectOfType<landcore>().gameObject) / 2;
        step = 0.001f;
        topCamera = FindObjectOfType<Camera>().orthographicSize;
        halfHeightCoin = porn.SpriteHeight(coinPrefab) / 2;
        coinList = new List<GameObject>();
        preCoinList = new List<GameObject>();
    }

    bool CheckPreviosBonuses(float _x, float _y, Vector3 start)
    {
        foreach (GameObject ele in preCoinList)
        {
            if (ele)
                if (Vector3.Distance(new Vector3(_x, _y) + start, ele.transform.position) < halfHeightCoin + dis + porn.SpriteHeight(ele) / 2)
                    return false;
        }

        return true;
    }

    public IEnumerator MakeCoin(Vector3 start, Vector3 end)
    {
        MiniSoundManager.PlaySound(SOUND_NAME.SOUND_MAKE_COIN);

        float v0 = porn.NemXieng_FindVo(_jumper.angle, end.x - start.x + fixCenter * 2, end.y - start.y, true);

        // fix center
        start -= new Vector3(fixCenter, 0);
        end += new Vector3(fixCenter, 0);

        float disNow = halfHeightCoin * 2 + dis; // dis cho từng loại coin       

        // bỏ phần đầu
        float _x = 0, _y = 0;
        for (; _y < trim; _x += step)
            _y = (float)porn.NemXieng_YFromX(v0, _jumper.angle, _x, true);
        _x -= step; // fix for    

        coinList.Clear();

        // tạo coin
        Vector3 preCoin = new Vector3(_x, _y);
        if (CheckPreviosBonuses(_x, _y, start))
        {
            GameObject obj = (GameObject)Instantiate(coinPrefab, start + preCoin, Quaternion.identity);
            coinList.Add(obj);

        }
        for (int i = 0; i < 50; i++) // lặp cho đến khi tới end point
        {
            for (int ii = 0; ii < 2000; ii++, _x += step) // lặp đến điểm coin tiếp theo
            {
                _y = (float)porn.NemXieng_YFromX(v0, _jumper.angle, _x, true);

                // coin cao quá thì khỏi vẽ
                if (_y + start.y > topCamera - halfHeightCoin)
                    continue;

                // check coi nextLand có enemy fly hay ko, có thì bỏ
                var thenemy = _jumper.nowLandCore.nextLand.GetComponent<landcore>().thenemy;
                if (thenemy && thenemy.GetComponent<flyenemysub>()) // có fly enemy
                {
                    if (Vector3.Distance(new Vector3(_x, _y) + start, thenemy.transform.position) >= dis + halfHeightCoin + porn.SpriteWidth(thenemy))
                        Destroy(thenemy);
                }

                // cách coin trước 1 khoảng nhất định mới vẽ
                if (Vector3.Distance(new Vector3(_x, _y, 0), preCoin) >= disNow && CheckPreviosBonuses(_x, _y, start))
                {
                    preCoin = new Vector3(_x, _y, 0);
                    GameObject obj = (GameObject)Instantiate(coinPrefab, start + preCoin, Quaternion.identity);
                    coinList.Add(obj);
                    break;
                }
            }

            if (Vector3.Distance(end, start + preCoin) <= trim * 1.5f) // đã đến end point
            {
                // ghi index các kiểu cho từng coin
                int index = Random.Range(1, 100) * Random.Range(1, 100);
                int count = coinList.Count;
                foreach (GameObject ele in coinList)
                {
                    coinsub cs = ele.GetComponent<coinsub>();
                    cs.listIndex = index;
                    cs.listCount = count;
                }

                // định lại list coin
                preCoinList.Clear();
                foreach (GameObject ele in coinList)
                    preCoinList.Add(ele);

                break;
            }

            yield return null;
        }
    }
}
