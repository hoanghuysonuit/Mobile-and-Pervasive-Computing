using UnityEngine;

public class movenemy : MonoBehaviour
{
    [Tooltip("khoảng cách tối thiểu giữa jumper va enemy")]
    public float dis;
    [Tooltip("khoảng cách di chuyển nhỏ nhất để tạo movenemy")]
    public float minDisMove;
    [Header("tốc độ di chuyển các move enemy")]
    public float minSpeed;
    public float maxSpeed;

    [HideInInspector]
    public static float fixY; // khoảng cách Y giữa land và enemy    
    float movenemyWidth; // chiều rộng movenemy 
    gamemanager _gamemanager;
    [HideInInspector]
    public float jumperWidth; // chiều rộng jumper

    void Start()
    {
        _gamemanager = FindObjectOfType<gamemanager>();
        movenemyWidth = porn.SpriteWidth(_gamemanager.objects[(int)OBJECT_TYPE.MOVENEMY_OBJECT].obj);
        jumperWidth = porn.SpriteWidth(_gamemanager.objects[(int)OBJECT_TYPE.JUMPER_OBJECT].obj);
        fixY = (porn.SpriteHeight(_gamemanager.objects[(int)OBJECT_TYPE.MOVENEMY_OBJECT].obj) + porn.SpriteHeight(_gamemanager.objects[(int)OBJECT_TYPE.LAND_OBJECT].obj)) / 2;
    }

    // true nếu tạo thành công
    public bool Create(GameObject land)
    {
        float landWidth = porn.SpriteWidth(land);

        // land phải đủ dài để tạo
        if (landWidth <= jumperWidth + 2 * dis + movenemyWidth * 2)
            return false;

        float range = landWidth / 2 - dis - jumperWidth / 2 - movenemyWidth;
        if (range < minDisMove)
            return false;

        float rand = Random.Range(0, range);
        float min, max;

        if (porn.RandomBool()) // tạo ở trái land
        {
            min = land.transform.position.x - landWidth / 2 + movenemyWidth / 2;
            max = land.transform.position.x - jumperWidth / 2 - dis - movenemyWidth / 2;
        }
        else // bên phải
        {
            min = land.transform.position.x + dis + movenemyWidth / 2 + jumperWidth / 2;
            max = land.transform.position.x + landWidth / 2 - movenemyWidth / 2;
        }

        GameObject obj = (GameObject)Instantiate(_gamemanager.objects[(int)OBJECT_TYPE.MOVENEMY_OBJECT].obj, new Vector3(min + rand, land.transform.position.y + fixY), Quaternion.identity);
        movenemysub sub = obj.GetComponent<movenemysub>();
        sub.landFollow = land;
        sub.minPos = min;
        sub.maxPos = max;

        land.GetComponent<landcore>().thenemy = obj;

        return true;
    }
}
