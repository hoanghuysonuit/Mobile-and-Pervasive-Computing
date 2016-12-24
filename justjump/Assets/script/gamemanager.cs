using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#region defines

// gồm direction, direction coll và expected
public enum DIRECTION_TYPE
{
    SIMPLE_WHITE_DIRECITON
}

// gồm này, expected, coll
public enum LAZE_DIRECTION_TYPE
{
    SIMPLE_YELLOW_LINE
}

// gồm này, expected, coll
public enum LINE_DIRECTION_TYPE
{
    SIMPLE_RECT_LINE_DIR
}

public enum BULLET_TYPE
{
    BASIC_BULLET
}

public enum BACKGROUND_TYPE
{
    MONOCHROME_BACKGROUND
}

public enum LAND_TYPE
{
    MONOCHROME_LAND
}

public enum COIN_TYPE
{
    STAR_COIN, COIN_COIN
}

public enum JUMPER_TYPE
{
    BUNNY_JUMPER
}

public enum ENEMY_TYPE
{
    GAI_ENEMY
}

public enum MOVENEMY_TYPE
{
    GAI_MOVENEMY
}

public enum FLYENEMY_TYPE
{
    SIMPLE_FLYENEMY
}

// CÁI NÀY PHẢI THEO THỨ TỰ TRONG UNITY !!!
public enum OBJECT_TYPE
{
    JUMPER_OBJECT, BACKGROUND_OBJECT, LAND_OBJECT, COIN_OBJECT, DIRECTION_OBJECT, DIRECTION_COLL__OBJECT, ENEMY_OBJECT, MOVENEMY_OBJECT, FLYENEMY_OBJECT, LAZE_DIRECTION_OBJECT, DIRECTION_EXPECTED_VO_OBJECT, LAZE_EXPECTED_VO_OBJECT, LAZE_DIR_COLL_OBJECT, LINE_DIRECTION_OBJECT, LINE_DIR_COLL_OBJECT, BULLET_OBJECT
}

[System.Serializable]
public struct SCeneStructure
{
    public JUMPER_TYPE jumper;
    public BACKGROUND_TYPE bgr;
    public LAND_TYPE land;
    public COIN_TYPE coin;
    public DIRECTION_TYPE direction;
    public ENEMY_TYPE enemy;
    public MOVENEMY_TYPE movenemy;
    public FLYENEMY_TYPE flyenemy;
    public LAZE_DIRECTION_TYPE lazedirection;
    public LINE_DIRECTION_TYPE linedirection;
    public BULLET_TYPE bullet;

    public int GetIndex(string tag)
    {
        if (tag == "Player")
            return (int)jumper;
        else if (tag == "background")
            return (int)bgr;
        else if (tag == "coin")
            return (int)coin;
        else if (tag == "direction" || tag == "direction collective" || tag == "direction expectedVo")
            return (int)direction;
        else if (tag == "enemy")
            return (int)enemy;
        else if (tag == "bullet")
            return (int)bullet;
        else if (tag == "movenemy")
            return (int)movenemy;
        else if (tag == "flyenemy")
            return (int)flyenemy;
        else if (tag == "land")
            return (int)land;
        else if (tag == "lazedirection" || tag == "laze direction collective" || tag == "laze expected")
            return (int)lazedirection;
        else if (tag == "linedirection" || tag == "line direction collective" || tag == "line expected")
            return (int)linedirection;

        return 0;
    }
}

[System.Serializable]
public struct ObjectStruct
{
    public GameObject obj;
    public Sprite[] spr;
}

#endregion

public class gamemanager : MonoBehaviour
{
    #region biến (không phải đuổi ngaaaa)

    [Tooltip("điểm tối đa có được khi hạ cánh ở giữa land")]
    public int maxLandingScore;
    [Tooltip("điểm khi ăn một coin")]
    public uint gottaCoinScore;
    [Tooltip("điểm khi bắn 1 enemies")]
    public uint killEnemiesScore;
    [Tooltip("vị trí cao tối đa của direction collective")]
    public float maxDirCollPos;

    // effect text
    [Header("effect text"), Tooltip("perfect text")]
    public GameObject perfectTextObject;
    [Tooltip("line direction text")]
    public GameObject lineDirTextObject;
    [Tooltip("direction text")]
    public GameObject dirTextObject;

    [Header("thông số thưởng")]
    [Tooltip("middle ratio nhỏ hơn số này thì được xem là ở giữa")]
    public int middleIs;
    [Tooltip("số lần nhảy ngay giữa để có direction"), Space(10)]
    public int guildMiddleTimes;
    [Tooltip("số land hiệu lực của direction")]
    public int guildLimit;
    [Tooltip("số lần nhảy ngay giữa để có line direction"), Space(5)]
    public int lineDirMidTimes;
    [Tooltip("số land hiệu lực của line direction")]
    public int lineDirLimit;
    [Tooltip("số lần nhảy ngay giữa để có laze direction"), Space(5)]
    public int lazeDirMidTimes;
    [Tooltip("số land hiệu lực của laze direction")]
    public int lazeDirLimit;

    [Header("giảm độ dài land")]
    [Tooltip("số giây giảm độ dài land")]
    public int time2ShortLand;
    [Tooltip("phần trăm land giảm một lần là bao nhiêu"), Range(1, 50)]
    public int decPercent;
    [Tooltip("độ dài land giảm một lần là bao nhiêu")]
    public float decLength;
    [Tooltip("phần trăm nhỏ nhất")]
    public int minPercent;
    [Tooltip("độ dài land ngắn nhất có thể"), Range(1, 5)]
    public int minLandLength;
    float time2ShortLandTick;
    bool reachedLimit; // đã đạt giới hạn nhỏ nhất chưa

    [Header("enemy")]
    [Tooltip("thời gian bắt đầu xuất hiện enemy")]
    public uint enemyAppearTime;
    [Tooltip("phần trăm xuất hiện the enemy"), Range(1, 99)]
    public uint enemyAppearPercent;
    [Tooltip("phần trăm để xuất hiện enemy"), Range(1, 99), Space(10)]
    public uint enemyPercent;
    [Tooltip("phần trăm để xuất hiện movenemy"), Range(1, 99), Space(5)]
    public uint movenemyPercent;
    [Tooltip("phần trăm để xuất hiện flyenemy"), Range(1, 99), Space(5)]
    public uint flyenemyPercent;

    [Header("các text")]
    public Text scoreText;
    public Text landCountText;

    // các biến khác
    jumper _jumper;
    landmover _landmover;
    coin _coin;
    direction _direction;
    linedirection lineDirection;
    lazedirection lazeDirection;
    enemy _enemy;
    movenemy _movenemy;
    flyenemy _flyenemy;
    [HideInInspector]
    public Vector3 preOriPosLand; // dùng để kiểm tra xem land này đã xài chưa
    int nowScene; // index màn hiện tại
    float directionCollHalfHeight; // để tính vị trí direction coll    
    Menu _menu;

    // hệ thống đếm directions collective
    [HideInInspector]
    public CountSystem guildAppear; // normal direction
    [HideInInspector]
    public CountSystem lineAppear;
    [HideInInspector]
    public CountSystem lazeDirAppear;

    [Header("các objects struct")]
    public ObjectStruct[] objects;

    [Header("sprite mỗi scene")]
    public SCeneStructure[] sceneStructure;

    #endregion biến

    void Start()
    {
        nowScene = 0;
        _jumper = FindObjectOfType<jumper>();
        _landmover = FindObjectOfType<landmover>();
        _coin = FindObjectOfType<coin>();
        _direction = FindObjectOfType<direction>();
        SetSceneSprite(nowScene);
        reachedLimit = false;
        guildAppear = new CountSystem(guildMiddleTimes);
        directionCollHalfHeight = porn.SpriteHeight(objects[(int)OBJECT_TYPE.DIRECTION_COLL__OBJECT].obj) / 2;
        maxDirCollPos = FindObjectOfType<Camera>().orthographicSize - maxDirCollPos;
        _enemy = FindObjectOfType<enemy>();
        _movenemy = FindObjectOfType<movenemy>();
        _flyenemy = FindObjectOfType<flyenemy>();
        lineAppear = new CountSystem(lineDirMidTimes);
        lazeDirAppear = new CountSystem(lazeDirMidTimes);
        lineDirection = FindObjectOfType<linedirection>();
        lazeDirection = FindObjectOfType<lazedirection>();
        _menu = FindObjectOfType<Menu>(); 
    }

    void Update()
    {
        // giảm độ dài của land
        if (!reachedLimit && time2ShortLandTick > time2ShortLand)
        {
            if (_landmover.lenghtLandPercent >= minPercent + decPercent)
                _landmover.lenghtLandPercent -= decPercent;
            else
            {
                if (_landmover.minLandWidthNow.localScale.x >= minLandLength + decLength)
                {
                    _landmover.lenghtLandPercent = 100;
                    _landmover.minLandWidthNow.localScale -= new Vector3(decLength, 0);
                }
                else
                    reachedLimit = true;
            }

            time2ShortLandTick = 0; 
        }
        else
            time2ShortLandTick += Time.deltaTime;

        // nháp
        if (porn.EnterDown())
            Time.timeScale = 0;
    }

    // thiết lập sprite cho mỗi scene
    void SetSceneSprite(int sceneIndex)
    {
        if (sceneIndex >= sceneStructure.Length)
            return;

        nowScene = sceneIndex;
        
        foreach (ObjectStruct ele in objects)
            porn.ChangeSprite(ele.obj, ele.spr[sceneStructure[nowScene].GetIndex(ele.obj.tag)]);
    }

    // khi được perfect coin
    public void TakeScoreOnPerfectCoin()
    {
        perfectTextObject.GetComponent<GradualSizeEffectScript>().StartEffect();
        MiniSoundManager.PlaySound(SOUND_NAME.SOUND_GOT_PERFECT_COIN);
    }

    // xử lý việc tạo collective
    void CollectiveHandle(int middleRatio)
    {
        StartCoroutine(_coin.MakeCoin(new Vector3(_jumper.transform.position.x, _jumper.nowLandCore.transform.position.y), _jumper.nowLandCore.nextLand.transform.position));
        return;

        if (middleRatio <= middleIs) // trường hợp đáp land ngay giữa
        {
            bool haveDirColl = false, haveLineDirColl = false, haveLazeDirColl = false;

            // kiểm tra đã đủ số lần middle chưa

            if (!_direction.active && !lineDirection.active && !lazeDirection.active)
            {
                if (guildAppear.Count()) // direction
                    haveDirColl = true;

                if (lineAppear.Count()) // line dir
                    haveLineDirColl = true;

                if (lazeDirAppear.Count()) // laze dir
                    haveLazeDirColl = true;
            }

            // CÁI NÀO MIDDLE TIMES CÀNG LỚN THÌ BỎ IF CÀNG CAO NHA!

            //if (haveLazeDirColl) // laze dir
            //{
            //    _jumper.nowLandCore.bonusList.Add((GameObject)Instantiate(objects[(int)OBJECT_TYPE.LAZE_DIR_COLL_OBJECT].obj, FindCollectivePos(), Quaternion.identity));
            //}
            //else if (haveLineDirColl) // line direction
            //{
            //    _jumper.nowLandCore.bonusList.Add((GameObject)Instantiate(objects[(int)OBJECT_TYPE.LINE_DIR_COLL_OBJECT].obj, FindCollectivePos(), Quaternion.identity));
            //}
            //else if (haveDirColl) // direction
            //{
            //    _jumper.nowLandCore.bonusList.Add((GameObject)Instantiate(objects[(int)OBJECT_TYPE.DIRECTION_COLL__OBJECT].obj, FindCollectivePos(), Quaternion.identity));
            //}
            //else // just coin
            //    StartCoroutine(_coin.MakeCoin(new Vector3(_jumper.transform.position.x, _jumper.nowLandCore.transform.position.y), _jumper.nowLandCore.nextLand.transform.position));
        }
        else // trường hợp đáp land k ngay giữa
        {
            // reset các bonus (khi nhảy không ngay giữa)

            if (!_direction.active) // direction 
                guildAppear.Reset();

            if (!lineDirection.active) // line direction 
                lineAppear.Reset();

            if (!lazeDirection.active) // laze direction 
                lazeDirAppear.Reset();
        }

        // hủy các bonus khi hết lần ở giữa

        if (_direction.active) // hủy direction
        {
            if (guildAppear.Count())
            {
                _direction.ActiveDirection(false);
                guildAppear.SetLimit(guildMiddleTimes);
            }
        }

        if (lineDirection.active) // hủy line direction
        {
            if (lineAppear.Count())
            {
                lineDirection.SetActive(false);
                lineAppear.SetLimit(lineDirMidTimes);
            }
        }

        if (lazeDirection.active) // hủy laze direction
        {
            if (lazeDirAppear.Count())
            {
                lazeDirection.SetActive(false);
                lazeDirAppear.SetLimit(lazeDirMidTimes);
            }
        }
    }

    // khi jumper bắt đầu đáp xuống land
    public void OnLanding()
    {
        // kiểm tra land này đã xài chưa
        if (preOriPosLand == _jumper.nowLandCore.oriPos)
            return;
        else
            preOriPosLand = _jumper.nowLandCore.oriPos;

        MiniSoundManager.PlaySound(SOUND_NAME.SOUND_LANDING);

        // số land đã đi được
        landCountText.GetComponent<AddScoreEffectScript>().AddScore(1);        
        
        // chỉ số phần rơi xuống (càng giữa càng 0)
        int middleRatio = Mathf.FloorToInt(Mathf.Abs(_jumper.transform.position.x - _jumper.nowLandCore.transform.position.x) / 0.5f);

        // xử lý collective
        CollectiveHandle(middleRatio);

        // tính score càng giữa càng cao
        int nowScore = maxLandingScore - (int)(maxLandingScore / _jumper.nowLandCore.transform.localScale.x) * middleRatio;
        if (nowScore < 0)
            nowScore = 0;

        //score += nowScore;
        //scoreText.text = score.ToString();
        scoreText.GetComponent<AddScoreEffectScript>().AddScore((uint)nowScore);

        // bỏ enemy ra khỏi land
        if (_jumper.nowLandCore.thenemy)
            SetEnemiesDie(_jumper.nowLandCore.thenemy, _jumper.nowLandCore.thenemy.transform.position.x > _jumper.transform.position.x);
    }
   
    Vector3 FindCollectivePos()
    {
        // tìm vị trí tạo collective

        float v0 = porn.NemXieng_FindVo(_jumper.angle, _jumper.nowLandCore.nextLand.transform.position.x - _jumper.nowLandCore.transform.position.x, _jumper.nowLandCore.nextLand.transform.position.y - _jumper.nowLandCore.transform.position.y, true);
        float x = (_jumper.nowLandCore.nextLand.transform.position.x - _jumper.nowLandCore.transform.position.x) / 2;
        double y = porn.NemXieng_YFromX(v0, _jumper.angle, x, true);
        float step = _jumper.transform.position.y > _jumper.nowLandCore.nextLand.transform.position.y ? _coin.step : -_coin.step;
        for (; _jumper.transform.position.y + y + directionCollHalfHeight > maxDirCollPos; x += step)
            y = porn.NemXieng_YFromX(v0, _jumper.angle, x, true);

        return new Vector3(x, (float)y) + _jumper.nowLandCore.transform.position;
    }

    public void SetEnemiesDie(GameObject enemies, bool rightDie)
    {
        if (enemies.GetComponent<enemysub>())
            enemies.GetComponent<enemysub>().landFollow = null;
        else if (enemies.GetComponent<movenemysub>())
            enemies.GetComponent<movenemysub>().landFollow = null;

        enemies.GetComponent<ParabolMoveScript>().SetFly(rightDie);
        enemies.GetComponent<Collider2D>().enabled = false;
        enemies.GetComponent<RotationScript>().enabled = true;

        MiniSoundManager.PlaySound(SOUND_NAME.SOUND_ENEMY_DIES);
    }

    public void SetGameOver()
    {
        _menu.SetGameOver();
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene("main");        
    }    

    public void CreateEnemy(GameObject land)
    {
        if (enemyAppearTime < Time.realtimeSinceStartup && porn.RandomBool((int)enemyAppearPercent))
        {
            // CÓ BAO NHIÊU LOẠI ENEMIES THÌ BỎ HẾT VÀO RANDOM 
            // IF SẮP XẾP THEO THỨ TỰ !

            int type = porn.RandomManyPercents((int)enemyPercent, (int)movenemyPercent, (int)flyenemyPercent);

            if (type == 0)
                _enemy.Create(land);
            else if (type == 1)
                _movenemy.Create(land);
            else if (type == 2)
                _flyenemy.Create(land);
        }
    }
}
