using UnityEngine;

public class enemy : MonoBehaviour
{
    [Tooltip("khoảng cách tối thiểu giữa jumper va enemy")]
    public float dis;   

    [HideInInspector]
    public static float fixY; // khoảng cách Y giữa land và enemy    
    [HideInInspector]
    public gamemanager _gamemanager;

    float enemyWidth; // chiều rộng enemy 

    [HideInInspector]
    public float jumperWidth; // chiều rộng jumper

    void Start()
    {
        _gamemanager = FindObjectOfType<gamemanager>();
        enemyWidth = porn.SpriteWidth(_gamemanager.objects[(int)OBJECT_TYPE.ENEMY_OBJECT].obj);
        jumperWidth = porn.SpriteWidth(_gamemanager.objects[(int)OBJECT_TYPE.JUMPER_OBJECT].obj);
        fixY = (porn.SpriteHeight(_gamemanager.objects[(int)OBJECT_TYPE.ENEMY_OBJECT].obj) + porn.SpriteHeight(_gamemanager.objects[(int)OBJECT_TYPE.LAND_OBJECT].obj)) / 2;
    }
    
    // true nếu tạo thành công
    public bool Create(GameObject land)
    {
        float landWidth = porn.SpriteWidth(land);

        // land phải đủ dài để tạo
        if (landWidth < jumperWidth + 2 * dis + enemyWidth * 2)
            return false;

        float range = landWidth / 2 - dis - jumperWidth / 2 - enemyWidth;
        float rand = Random.Range(0, range); 

        GameObject obj = (GameObject)Instantiate(_gamemanager.objects[(int)OBJECT_TYPE.ENEMY_OBJECT].obj, 

            new Vector3(rand + (porn.RandomBool() ? (land.transform.position.x - landWidth / 2 + enemyWidth / 2) : (land.transform.position.x + dis + enemyWidth / 2)),
                                                                                                                                   
            fixY + land.transform.position.y), 
            
            
            Quaternion.identity);                            

        enemysub sub = obj.GetComponent<enemysub>();
        sub.landFollow = land;

        land.GetComponent<landcore>().thenemy = obj;

        return true;
    }
}
