using UnityEngine;
using System.IO;

// onequy (20/08/2016)

public enum NAVIGATION
{
    NAVI_NONE, LEFT, RIGHT, TOP, BOTTOM
};

public enum VECTOR3_CHANGE_VALUE
{
    CHANGE_X, CHANGE_Y, CHANGE_Z, CHANGE_XY, CHANGE_YZ, CHANGE_XZ, CHANGE_XYZ
}

// hệ thống đếm thông minh hihi
public struct CountSystem
{
    int count, limit;
    bool active;

    public CountSystem(int _limit, bool limitWhenStart = true)
    {
        count = 0;
        active = true;
        limit = _limit + (limitWhenStart ? 0 : 1);
    }

    public void SetActive(bool _active)
    {
        active = _active;

        if (_active)
            Reset();
    }

    public void SetLimit(int _limit, bool limitWhenStart = true)
    {
        if (!active)
            return;

        limit = _limit + (limitWhenStart ? 0 : 1);
    }

    public bool Count()
    {
        if (!active)
            return false;

        count++;

        if (limit == 0 || count == limit)
        {
            Reset();
            return true;
        }

        return false;
    }

    public void Reset()
    {
        if (!active)
            return;

        count = 0;
    }
}

// hệ thống tính giờ đã qua thông minh nữa nè hihi
// hàm PingPong phải được cập nhật mới tính giờ được !
public struct TimeElapsed
{
    float tick;
    float time;
    bool active;
    float defaulttime;

    public TimeElapsed(float _time = 1, float _defaultTime = 0.3f, bool _active = true)
    {
        if (_time < 0)
        {
            Debug.LogError("Lỗi time nhỏ hơn 0 ! Set về mặc định là 1 gồi !");
            _time = 1;
        }

        tick = 0;
        active = _active;
        time = _time;
        defaulttime = _defaultTime;
    }

    public bool PingPong()
    {
        if (!active)
            return false;

        tick += Time.deltaTime;

        if (tick >= time)
        {
            tick = 0;
            return true;
        }

        return false;
    }

    public void SetActive(bool _active)
    {
        active = _active;

        if (active)
            tick = 0;
    }

    public float SetTime(float _time)
    {
        if (_time < 0)
            _time = defaulttime;

        if (_time != time)
            time = _time;

        return _time;
    }

    public void Reset()
    {
        if (!active)
            return;

        tick = 0;
    }
}

public class porn : MonoBehaviour
{
    #region giải phương trình bậc hai một ẩn
    // x, y là giá trị nghiệm
    // z là số ngiệm
    // sắp xếp nghiệm tăng dần luôn
    // dùng đề tạo vị trí land mới
    public static Vector3 NghiemBacHaiMotAn(float a, float b, float c)
    {
        Vector3 retu = Vector3.zero;
        float delta = b * b - 4 * a * c;

        if (delta == 0) // 1 nghiệm
        {
            retu.z = 1;
            retu.x = (-b / 2 / a);
        }
        else if (delta > 0) // 2 nghiệm
        {
            retu.z = 2;
            delta = Mathf.Sqrt(delta);
            float i = ((-b - delta) / 2 / a);
            float j = ((-b + delta) / 2 / a);
            retu.x = Mathf.Min(i, j);
            retu.y = Mathf.Max(i, j);
        }
        else // vô nghiệm
            retu.z = 0;

        return retu;
    }

    #endregion

    #region parabol

    // đỉnh parabol từ 3 điểm mà parabol đi qua
    public static Vector2 CalcParabolaVertex(float x1, float y1, float x2, float y2, float x3, float y3)
    {
        float denom = (x1 - x2) * (x1 - x3) * (x2 - x3);
        float A = (x3 * (y2 - y1) + x2 * (y1 - y3) + x1 * (y3 - y2)) / denom;
        float B = (x3 * x3 * (y1 - y2) + x2 * x2 * (y3 - y1) + x1 * x1 * (y2 - y3)) / denom;
        float C = (x2 * x3 * (x2 - x3) * y1 + x3 * x1 * (x3 - x1) * y2 + x1 * x2 * (x1 - x2) * y3) / denom;

        return new Vector2(-B / (2 * A), C - B * B / (4 * A));
    }

    // công thức parabol từ 3 điểm nó đi qua
    public static Vector3 ParabolFormula(float x1, float y1, float x2, float y2, float x3, float y3)
    {
        float denom = (x1 - x2) * (x1 - x3) * (x2 - x3);
        float A = (x3 * (y2 - y1) + x2 * (y1 - y3) + x1 * (y3 - y2)) / denom;
        float B = (x3 * x3 * (y1 - y2) + x2 * x2 * (y3 - y1) + x1 * x1 * (y2 - y3)) / denom;
        float C = (x2 * x3 * (x2 - x3) * y1 + x3 * x1 * (x3 - x1) * y2 + x1 * x2 * (x1 - x2) * y3) / denom;

        return new Vector3(A, B, C);
    }

    // kiểm tra 1 điểm có thuộc parabol hay không
    public static bool OfParabol(Vector3 parabol, float x, float y)
    {
        if (parabol.x * x * x + parabol.y * x + parabol.z == y)
            return true;
        return false;
    }

    // tạo Parabol từ đỉnh và 1 điểm đi qua
    public static Vector3 MakeParabol(float xRoot, float yRoot, float x, float y)
    {
        float fPow = Mathf.Pow(x - xRoot, 2);
        return new Vector3((y - yRoot) / fPow,
         (-2 * xRoot * (x - yRoot) / fPow),
         ((yRoot * x * x - 2 * x * xRoot * yRoot + xRoot * xRoot * y) / fPow));
    }

    #endregion

    #region file

    public static void WriteFile(string path, FileMode fileMode, object data)
    {
        var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

        using (Stream filestream = File.Open(path, fileMode))
        {
            formatter.Serialize(filestream, data);
        }
    }

    public static object ReadFile(string path, FileMode fileMode)
    {
        var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        using (Stream filestream = File.Open("filename.dat", FileMode.Open))
        {
            return formatter.Deserialize(filestream);
        }
    }

    #endregion

    #region ném xiêng

    // Độ cao tối đa của vật trong chuyển động ném xiêng:
    public static double NemXieng_Max_H(float vo, float degree)
    {
        return vo * vo * Mathf.Sin(Mathf.Deg2Rad * degree) * Mathf.Sin(Mathf.Deg2Rad * degree) / 2 / 9.8;
    }

    // Độ xa tối đa của vật trong chuyển động ném xiêng:
    public static double NemXieng_Max_L(float vo, float degree)
    {
        return vo * vo * Mathf.Sin(2 * Mathf.Deg2Rad * degree) / 9.8;
    }

    // Vật tốc của vật tại độ cao h trong chuyển động ném xiêng:
    public static float NemXieng_v(float vo, float h)
    {
        return Mathf.Sqrt((float)(vo * vo - 2 * 9.8 * h));
    }

    // Tính tọa độ Y theo X trong chuyển động ném xiêng (Gốc tọa độ ở dưới, bên trái - v0 phải khác 0)
    public static double NemXieng_YFromX(float vo, float degree, float x, bool allowVoZero = false)
    {
        if (!allowVoZero && vo == 0)
        {
            Debug.LogError("Lỗi chia cho 0 (NemXieng_YFromX)");
            return 0;
        }

        return vo == 0 ? 0 : -(9.8 / (2 * vo * vo * Mathf.Cos(Mathf.Deg2Rad * degree) * Mathf.Cos(Mathf.Deg2Rad * degree))) * x * x + Mathf.Tan(Mathf.Deg2Rad * degree) * x;
    }

    // Tính tọa độ X theo Y trong chuyển động ném xiêng (Gốc tọa độ ở dưới, bên trái - v0 phải khác 0)
    public static Vector3 NemXieng_XFromY(float vo, float degree, float y, bool allowVoZero = false)
    {
        if (!allowVoZero && vo == 0)
        {
            Debug.LogError("Lỗi chia cho 0 (NemXieng_XFromY)");
            return Vector3.zero;
        }

        double a = -(9.8 / (2 * vo * vo * Mathf.Cos(Mathf.Deg2Rad * degree) * Mathf.Cos(Mathf.Deg2Rad * degree)));
        double b = Mathf.Tan(Mathf.Deg2Rad * degree);

        return vo == 0 ? Vector3.zero : NghiemBacHaiMotAn((float)a, (float)b, -y);
    }

    // tìm Vo thông qua 1 điểm trên quỹ đạo
    // x phải khác 0
    public static float NemXieng_FindVo(float degree, float x, float y, bool allowXZero = false)
    {
        if (!allowXZero && x == 0)
        {
            Debug.LogError("Lỗi chia cho 0 (NemXieng_YFromX)");
            return 0;
        }

        return Mathf.Sqrt((9.8f / ((Mathf.Tan(Mathf.Deg2Rad * degree) * x - y) / (x * x))) / 2 / Mathf.Pow(Mathf.Cos(Mathf.Deg2Rad * degree), 2));
    }

    #endregion

    #region rơi tự do

    // Rơi tự do từ trên xuống	
    public static float RoiTuDo_v(float vo, float d)
    {
        return Mathf.Sqrt((float)(vo * vo + 2 * 9.8 * d));
    }

    #endregion

    #region % and Random

    // random từ các phần trăm (không nhất thiết phải cộng lại 100%)
    // trả về index của cái phần trăm mà giá trị random thuộc về 
    public static int RandomManyPercents(params int[] percents)
    {
        int total = 0;

        foreach (int ele in percents)
            total += ele;

        int pre = 0, rand;
        rand = Random.Range(0, total);

        int index = 0;
        foreach (int ele in percents)
        {
            if (rand <= ele + pre && rand >= pre)
                return index;
            else
                pre += ele;

            index++;
        }

        return index;
    }

    // random bool có phần trăm
    public static bool RandomBool(int percent = 50)
    {
        return (Random.Range(0, 100) < percent ? true : false);
    }

    // random bool trả về 1 hoặc -1
    public static int Random1(int percent = 50)
    {
        return RandomBool(percent) ? 1 : -1;
    }

    // random int có phần trăm
    // để giảm chiều dài land
    public static int RandomInt(int start, int end, int percent)                    
    {
        if (end <= start)
            return start;
        else
        {
            int mid = start + (end - start) / 2;
            if (RandomBool(percent))
                return RandomInt(mid + 1, end, percent);
            else
                return RandomInt(start, mid, percent);
        }
    }

    #endregion

    #region Thay đổi giá trị vector3

    // change value 1 thành phẩn của vector3
    public static Vector3 Vector3Value(Vector3 oldValue, VECTOR3_CHANGE_VALUE index, params float[] value)
    {
        switch (index)
        {
            case VECTOR3_CHANGE_VALUE.CHANGE_X:
                return new Vector3(value[0], oldValue.y, oldValue.z);
            case VECTOR3_CHANGE_VALUE.CHANGE_Y:
                return new Vector3(oldValue.x, value[0], oldValue.z);
            case VECTOR3_CHANGE_VALUE.CHANGE_Z:
                return new Vector3(oldValue.x, oldValue.y, value[0]);
            case VECTOR3_CHANGE_VALUE.CHANGE_XY:
                return new Vector3(value[0], value[1], oldValue.z);
            case VECTOR3_CHANGE_VALUE.CHANGE_YZ:
                return new Vector3(oldValue.x, value[0], value[1]);
            case VECTOR3_CHANGE_VALUE.CHANGE_XZ:
                return new Vector3(value[0], oldValue.y, value[1]);
            case VECTOR3_CHANGE_VALUE.CHANGE_XYZ:
                return new Vector3(value[0], value[1], value[2]);
        }

        return oldValue;
    }

    #endregion

    #region khác

    // thay sprite hình cho một gameobject
    public static void ChangeSprite(GameObject obj, Sprite sprite)
    {
        // sprite đã là sprite hiện tại thì k thay
        if (obj.GetComponent<SpriteRenderer>().sprite.name == sprite.name)
            return;

        obj.GetComponent<SpriteRenderer>().sprite = sprite;
        FixBoxCollider2D(obj);
    }

    // chỉnh kích thước box collider 2D khớp vs sprite hình
    public static void FixBoxCollider2D(GameObject obj)
    {
        BoxCollider2D box = obj.GetComponent<BoxCollider2D>();
        if (box == null)
            return;

        box.size = new Vector2(SpriteWidth(obj) / obj.transform.localScale.x, SpriteHeight(obj) / obj.transform.localScale.y);
    }

    // destroy object khi ra khỏi camera (2D), true nếu destroy    
    public static bool DestroyIfOut(GameObject obj, params NAVIGATION[] list)
    {
        Camera camera = FindObjectOfType<Camera>();
        float hozCamera = camera.orthographicSize * camera.aspect;
        float wid = SpriteWidth(obj) / 2;
        float hei = SpriteHeight(obj) / 2;

        for (int i = 0; i < list.Length; i++)
        {
            if ((list[i] == NAVIGATION.RIGHT && obj.transform.position.x - wid > camera.transform.position.x + hozCamera) ||
                (list[i] == NAVIGATION.LEFT && obj.transform.position.x + wid < camera.transform.position.x - hozCamera) ||
            (list[i] == NAVIGATION.RIGHT && obj.transform.position.x - wid > camera.transform.position.x + hozCamera) ||
            (list[i] == NAVIGATION.TOP && obj.transform.position.y - hei > camera.transform.position.y + camera.orthographicSize) ||
            (list[i] == NAVIGATION.BOTTOM && obj.transform.position.y + hei < camera.transform.position.y - camera.orthographicSize))
            {
                Destroy(obj);
                return true;
            }
        }

        return false;
    }

    // tính width của sprite theo unit
    public static float SpriteWidth(GameObject gameObject)
    {
        Sprite sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
        if (sprite == null)
            return Mathf.Abs(gameObject.transform.localScale.x);

        return Mathf.Abs(sprite.texture.width / sprite.pixelsPerUnit * gameObject.transform.localScale.x);
    }

    // tính height của sprite theo unit
    public static float SpriteHeight(GameObject gameObject)
    {
        Sprite sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
        if (sprite == null)
            return Mathf.Abs(gameObject.transform.localScale.y);

        return Mathf.Abs(sprite.texture.height / sprite.pixelsPerUnit * gameObject.transform.localScale.y);
    }

    // Scale theo tỉ lệ
    // index = 0, 1, 2 thì scale theo x, y, z
    public static void Scale(GameObject gameObject, float newValue, uint index = 0)
    {
        if (index == 0) // scale theo x
            gameObject.transform.localScale = new Vector3(newValue, newValue * gameObject.transform.localScale.y / gameObject.transform.localScale.x, newValue * gameObject.transform.localScale.z / gameObject.transform.localScale.x);
        else if (index == 1) // scale theo y
            gameObject.transform.localScale = new Vector3(newValue * gameObject.transform.localScale.x / gameObject.transform.localScale.y, newValue, newValue * gameObject.transform.localScale.z / gameObject.transform.localScale.y);
        else // scale theo z
            gameObject.transform.localScale = new Vector3(newValue * gameObject.transform.localScale.x / gameObject.transform.localScale.z, newValue * gameObject.transform.localScale.y / gameObject.transform.localScale.z, newValue);
    }

    // lấy hướng va chạm 2D
    public static NAVIGATION CollisionNavi(GameObject examination, Collision2D center)
    {
        float halfHeightCenter = SpriteHeight(examination) / 2;

        Vector2 contactPoint = center.contacts[0].point;
                                        
        if (Mathf.Abs(contactPoint.y - examination.transform.position.y) <= halfHeightCenter)
        {
            if (contactPoint.x < examination.transform.position.x)
                return NAVIGATION.RIGHT;
            return NAVIGATION.LEFT;
        }
        else
        {
            if (contactPoint.y < examination.transform.position.y)
                return NAVIGATION.TOP;
            return NAVIGATION.BOTTOM;
        }
    }

    // scale object (có sprite hay không đều được) theo kích thước chỉ định
    // trả về true nếu scale thành công
    // trả về false nếu k có scale (đã đúng kích thước yêu cầu)
    // truyền vào 0 nếu không muốn thay đổi 
    // keepRatio thì một trong hai cái width và height phải bằng 0
    public static bool FitSize(GameObject obj, float width = 0, float height = 0, bool keepRatio = false)
    {
        if (width != 0)
        {
            float w = SpriteWidth(obj);

            if (w == width)
                return false;

            obj.transform.localScale = new Vector3(width * obj.transform.localScale.x / w, (keepRatio && height == 0) ? (width / w) : obj.transform.localScale.y, obj.transform.localScale.z);

            return true;
        }

        if (height != 0)
        {
            float h = SpriteHeight(obj);

            if (h == height)
                return false;

            obj.transform.localScale = new Vector3((keepRatio && width == 0) ? (height / h) : obj.transform.localScale.x, height * obj.transform.localScale.y / h, obj.transform.localScale.z);

            return true;
        }

        return false;
    }

    // độ dài nửa ngang của camera
    public static float CameraWidthHalf(float multi = 1.0f)
    {
        Camera c = FindObjectOfType<Camera>();
        return c.orthographicSize * c.aspect * multi;
    }

    // độ dài nửa dọc của camera
    public static float CameraHeightHalf(float multi = 1.0f)
    {
        return FindObjectOfType<Camera>().orthographicSize * multi;
    }

    // true khi nhấn enter down
    public static bool EnterDown(KeyCode key = KeyCode.Return)
    {
        return Input.GetKeyDown(key);
    }

    #endregion
}