using UnityEngine;

public class flyenemy : MonoBehaviour
{
    [Tooltip("khoảng cách flyenemy so với top camera")]
    public float dis;

    [HideInInspector]
    public gamemanager _gamemanager;

    float flyenemyWidth; // chiều rộng flyenemy 
    float flyenemyHeight; // chiều cao flyenemy 
    float maxY; // độ cao tối đa của flyenemy
    float landHeight; 

    void Start()
    {
        _gamemanager = FindObjectOfType<gamemanager>();
        flyenemyWidth = porn.SpriteWidth(_gamemanager.objects[(int)OBJECT_TYPE.FLYENEMY_OBJECT].obj);
        flyenemyHeight = porn.SpriteHeight(_gamemanager.objects[(int)OBJECT_TYPE.FLYENEMY_OBJECT].obj);
        maxY = FindObjectOfType<Camera>().orthographicSize - dis - flyenemyHeight / 2;
        landHeight = porn.SpriteHeight(FindObjectOfType<landcore>().gameObject);
    }

    // true nếu tạo thành công
    public bool Create(GameObject land)
    {
        float landWidth = porn.SpriteWidth(land);
        float minY = Mathf.Max(land.GetComponent<landcore>().preLand.transform.position.y, land.transform.position.y);
        minY += dis + flyenemyHeight / 2 + landHeight;

        if (minY > maxY) // k đủ đều kiện tạo flyenemy
            return false;

        float y = Random.Range(minY, maxY);
        float x = Random.Range(land.transform.position.x - landWidth / 2 + flyenemyWidth / 2, land.transform.position.x + landWidth / 2 - flyenemyWidth / 2);

        GameObject obj = (GameObject)Instantiate(_gamemanager.objects[(int)OBJECT_TYPE.FLYENEMY_OBJECT].obj, new Vector3(x, y), Quaternion.identity);

        flyenemysub sub = obj.GetComponent<flyenemysub>();
        sub.landFollow = land;

        land.GetComponent<landcore>().thenemy = obj;

        return true;
    }
}
