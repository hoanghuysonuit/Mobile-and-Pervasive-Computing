using UnityEngine;

public class landmover : MonoBehaviour
{
    [HideInInspector]
    public Transform lastLand; // land cuối của list

    [HideInInspector]
    public float nowLandSpeed; // tốc độ di chuyển của land hiện tại

    public int lenghtLandPercent; // % tạo ra land dài
    public float emptySpace; // khoảng cách tối thiểu giữa 2 land
    public float landSpeed; // tốc độ di chuyển của land khi đang xử lý  
    public Transform maxLandPos; // vị trí thấp nhất của land
    public Transform minLandPos; // vị trí cao nhất của land
    public Transform maxEmptySpace; // khoảng cách tối đa giữa 2 land
    public Transform maxLandWidth; // độ dài tối đa của land
    public Transform rightSide; // top của viewport
    jumper _jumper;
    gamemanager _gamemanager;
    coin _coin;    
    public GameObject landPrefab;
    public Transform minLandWidthNow; // độ dài nhỏ nhất hiện tại được phép của land
    float maxDis, maxH, v0; // ba cái biến tạm xài cho HeadCalc() á mà

    void Start()
    {
        _jumper = FindObjectOfType<jumper>();
        lastLand = FindObjectOfType<landcore>().transform;
        _gamemanager = FindObjectOfType<gamemanager>();
        _coin = FindObjectOfType<coin>();
    }

    void Update()
    {
        if (lastLand.position.x < rightSide.position.x * 2 + _jumper.transform.position.x)
            NewLand();
    }

    float NewLandWidth(float maxWidth)
    {
        if (maxWidth <= minLandWidthNow.localScale.x)
            return maxWidth;
        else if (maxWidth > maxLandWidth.transform.localScale.x)
            maxWidth = maxLandWidth.transform.localScale.x;

        int rand = porn.RandomInt((int)(minLandWidthNow.localScale.x * 10), (int)(maxWidth * 10), lenghtLandPercent);

        return rand / 10.0f;
    }

    void HeadCalc()
    {
        maxDis = lastLand.transform.position.y - _jumper.maxMovePos.position.y; // khoảng cách tối đa land có thể xuống
        v0 = maxDis / _jumper.vel2Dis * _jumper.game2realVel; // vận tốc tối đa
        maxH = (float)porn.NemXieng_Max_H(v0, _jumper.angle); // độ cao tối đa (parabol)

        //if (maxH > (topSide.position.y - lastLand.position.y)) // nếu max h quá cao thì max h bằng top của viewport
        //{
        //    v0 = Mathf.Sqrt((float)((topSide.position.y - lastLand.position.y) * 2 * 9.8) / ((Mathf.Sin(Mathf.Deg2Rad * _jumper.angle) * Mathf.Sin(Mathf.Deg2Rad * _jumper.angle))));

        //    // Debug.Log("Max H quá cao !");
        //}
    }

    bool NewLand()
    {
        if (lastLand == null || _jumper == null) // ngoại lệ
        {
            Debug.LogError("96228");
            return false;            
        }

        HeadCalc();

        // Debug.Log("v0 " + v0);

        Vector3 nghiem = porn.NghiemBacHaiMotAn((float)-9.8 / (float)(2 * v0 * v0 * Mathf.Cos(Mathf.Deg2Rad * _jumper.angle) * Mathf.Cos(Mathf.Deg2Rad * _jumper.angle)), Mathf.Tan(Mathf.Deg2Rad * _jumper.angle), lastLand.position.y - maxLandPos.position.y); // tìm điểm giao max X

        if (nghiem.z < 2) // phải 2 ngiệm mới đúng
        {
            Debug.LogError("8524 " + nghiem.z);
            return false;
        }

        float lastLandEdge = lastLand.position.x + porn.SpriteWidth(lastLand.gameObject) / 2; // vị trí cuối cùng của last land
        float maxX = nghiem.y + lastLand.position.x; // max x (jumper có thể bay đến)
        float minX = lastLandEdge + porn.SpriteWidth(_jumper.gameObject) + emptySpace; //  min x (jumper có thể bay đến)
        float maxLandLenght = maxX - minX;
        float newLandWidth = NewLandWidth(maxLandLenght);

        // Debug.Log("max x " + maxX);
        // Debug.Log("min x " + minX);
        // Debug.Log("new land width " + newLandWidth);

        maxX = maxX - newLandWidth / 2; // max x (của land)
        minX = minX + newLandWidth / 2; // min x (của land)

        // Debug.Log("max x land " + maxX);
        // Debug.Log("min x land " + minX);

        if (maxX - newLandWidth / 2 - lastLandEdge > porn.SpriteWidth(maxEmptySpace.gameObject)) // giới hạn khoảng cách tối đa giữa 2 land
        {
            maxX = lastLandEdge + porn.SpriteWidth(maxEmptySpace.gameObject) + newLandWidth / 2;
        }

        float posX; // tọa độ x của new land
        if (maxX - minX <= 0) // ngoại lệ
            posX = minX;
        else
            posX = Random.Range(minX, maxX); // random posX

        // Debug.Log("pos X " + posX);

        float maxY = (float) porn.NemXieng_YFromX(v0, _jumper.angle, posX - lastLand.position.x, true) - porn.SpriteHeight(lastLand.gameObject); // max y ảo

        // Debug.Log("max y " + maxY);

        if (maxY + lastLand.position.y > minLandPos.position.y) // nếu maxY ảo cao hơn vị trí cao nhất thì set lại
        {
            maxY = minLandPos.position.y - lastLand.position.y;
        }

        for (; maxY > maxLandPos.position.y - lastLand.position.y; maxY -= 0.01f) // lặp tìm maxY ảo có thể chứa land
        {
            nghiem = porn.NghiemBacHaiMotAn((float)-9.8 / (float)(2 * v0 * v0 * Mathf.Cos(Mathf.Deg2Rad * _jumper.angle) * Mathf.Cos(Mathf.Deg2Rad * _jumper.angle)), Mathf.Tan(Mathf.Deg2Rad * _jumper.angle), -maxY);

            if (nghiem.z < 2) // phải 2 ngiệm mới đúng
            {
                Debug.LogError("963258 " + nghiem.z);
                return false;
            }

            if (Vector2.Distance(new Vector2(posX - lastLand.position.x, maxY), new Vector2(nghiem.y, maxY)) > newLandWidth / 2)
                break; 
        }        

        maxY += lastLand.position.y; // top Y thật

        if (maxY < maxLandPos.position.y) // define lại maxY
            maxY = maxLandPos.position.y;

        // Debug.Log("max y real " + maxY);

        float posY = Random.Range(maxLandPos.position.y, maxY); // tọa độ x của new land     

        // Debug.Log("pos y " + posY);

        if (posY > minLandPos.position.y || posY < maxLandPos.position.y)
        {
            Debug.LogError("75342 Lỗi lố vị trí");
            return false;
        }        

        GameObject newLandObject = (GameObject) Instantiate(landPrefab, new Vector3(posX, posY, 0), Quaternion.identity); // tạo land mới nè        

        lastLand.gameObject.GetComponent<landcore>().nextLand = newLandObject; // ghi next land         
        newLandObject.GetComponent<landcore>().preLand = lastLand.gameObject;
        lastLand = newLandObject.transform; // cập nhật lại last land        

        // tính lại scale
        HeadCalc(); // gọi để tính lại scale cho phù hợp
        nghiem = porn.NghiemBacHaiMotAn((float)-9.8 / (float)(2 * v0 * v0 * Mathf.Cos(Mathf.Deg2Rad * _jumper.angle) * Mathf.Cos(Mathf.Deg2Rad * _jumper.angle)), Mathf.Tan(Mathf.Deg2Rad * _jumper.angle), 0); // tìm điểm giao max X để tính lại scale
        if (nghiem.y - porn.SpriteWidth(_jumper.gameObject) - emptySpace < newLandWidth / 2) // nếu land mới quá dài 
        {
            newLandWidth = (nghiem.y - porn.SpriteWidth(_jumper.gameObject) - emptySpace) * 2;
            return false;
        }
        newLandObject.transform.localScale = new Vector3(newLandWidth, 1, 0); // độ dài cho land mới

        // tạo enemy
        _gamemanager.CreateEnemy(newLandObject);

        return true;
    }
}
