using UnityEngine;

// 5 m/s = 0.1 m / frame

public class jumper : MonoBehaviour
{
    #region biến

    /* các biến sử dụng cho các lớp khác */

    [HideInInspector]
    public float bottomPos; // vị trí đáy của land trong auto

    [HideInInspector]
    public float expectedVo; // Vo lý tưởng để bay tới giữa land tiếp theo

    [HideInInspector]
    public bool haveDirections; // có các loại hỗ trợ directions ?

    [HideInInspector]
    public bool isAuto; // land nhấn chế độ auto

    [HideInInspector]
    public float v0; // vận tốc ban đầu khi bay

    [HideInInspector]
    public bool isNotStillPress; // còn nhấn khi land xuống k

    [HideInInspector]
    public zoom zoomLand;

    [HideInInspector]
    public landmover landMover;

    [HideInInspector]
    public landcore nowLandCore; // land đang xử lý

    [HideInInspector]
    public float flyingVel; // tốc độ bay hiện tại

    public Transform maxMovePos; // vị trí đi xuống tối đa của land

    public Transform maxHeightFly; // giới hạn lại độ cao bay tối đa

    public float firstVeloc; // tốc độ rơi tự do ban đầu

    /* các hệ số */
    public float vel2Dis; // hệ số chuyển từ vận tốc sang độ nhúng của land
    public float speedLandDown; // tốc độ đi xuống của land trong manual
    public float speedLandUp; // tốc độ đi lên của land trong manual
    public float game2realVel; // tốc độ game sang tốc độ thực (để tạo parabol)
    public float angle; // góc bay
    public float devidedVel; // hệ số tốc độ bay (càng lớn càng chậm)
    public float expectedVoApproRight; // sai số giữa expectedVo và Vo nhấn thực tế
    public float expectedVoApproLeft; // sai số giữa expectedVo và Vo nhấn thực tế
    public bool startGame; // có start game hay k?

    /* biến của jumper */
    bool isFlying;
    float preVel; // lưu vận tốc trước khi jumper chạm land
    float halfLandJumper; // để tính vị trí Y tiếp theo của jumper, expectedVo
    Rigidbody2D rig;
    FixedJoint2D fixedJoint;
    Vector2 beforeFlyingPos; // tọa độ Y trước khi bay để tính độ cao    
    camera _camera;
    direction _direction;
    lazedirection _lazeDirection;
    linedirection _lineDirection;

    gamemanager _gamemanager;

    // biến perfect get coin
    int nowIndexCoin, nowCountCoin, countGotCoin;

    #endregion

    void Start()
    {
        fixedJoint = GetComponent<FixedJoint2D>();
        rig = GetComponent<Rigidbody2D>();
        preVel = -firstVeloc;
        if (startGame)
            rig.velocity = new Vector2(0, preVel);
        isFlying = false;
        landMover = FindObjectOfType<landmover>();
        _camera = FindObjectOfType<camera>();
        _direction = GetComponent<direction>();
        _gamemanager = FindObjectOfType<gamemanager>();
        nowCountCoin = -1;
        Random.InitState(1000);
        zoomLand = FindObjectOfType<zoom>();
        _lazeDirection = GetComponent<lazedirection>();
        _lineDirection = GetComponent<linedirection>();
        halfLandJumper = porn.SpriteHeight(FindObjectOfType<landcore>().gameObject) / 2 + porn.SpriteHeight(gameObject) / 2;
    }

    void Update()
    {
        if (isFlying) // đang bay
        {
            // cập nhật bay cho jumper
            Vector2 virNewPos = new Vector2(transform.position.x + flyingVel - beforeFlyingPos.x, 0);
            virNewPos.y = (float)porn.NemXieng_YFromX(v0 * game2realVel, angle, virNewPos.x, true);
            Vector2 newDis = beforeFlyingPos + virNewPos - new Vector2(transform.position.x, transform.position.y);
            rig.velocity = newDis * 50;
            preVel = rig.velocity.y;

            // tốc độ di chuyển land
            if (Mathf.Abs(_camera.transform.position.x - transform.position.x) < 7)
                landMover.nowLandSpeed = rig.velocity.x;
            else if (landMover.nowLandSpeed != 0)
                landMover.nowLandSpeed = 0;
            
            // rớt hụt land thì chết
            if (transform.position.y < -maxHeightFly.position.y && Time.timeScale != 0)
                _gamemanager.SetGameOver();
        }
    }

    // jumper va chạm với bonuses
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "coin") // jumper va chạm với coin
        {
            MiniSoundManager.PlaySound(SOUND_NAME.SOUND_GET_COIN);
            _gamemanager.scoreText.GetComponent<AddScoreEffectScript>().AddScore(_gamemanager.gottaCoinScore);

            // perfect get coin
            coinsub cs = other.GetComponent<coinsub>();
            if (countGotCoin == 0) // ăn coin đầu tiên của list
            {
                nowIndexCoin = cs.listIndex;
                nowCountCoin = cs.listCount;
                countGotCoin++;
            }
            else if (nowIndexCoin == cs.listIndex)
                countGotCoin++;

            Destroy(other.gameObject);
        }
        else if (other.tag == "direction collective") // jumper va chạm với direction collective
        {
            Destroy(other.gameObject);
            _gamemanager.dirTextObject.GetComponent<GradualSizeEffectScript>().StartEffect();

            // active direction
            _direction.ActiveDirection(true);
            _gamemanager.guildAppear.SetLimit(_gamemanager.guildLimit, false);            
        }
        else if (other.tag == "laze direction collective") // jumper va chạm với laze direction collective
        {
            Destroy(other.gameObject);

            // active laze dir
            _lazeDirection.SetActive(true);
            _gamemanager.lazeDirAppear.SetLimit(_gamemanager.lazeDirLimit, false);
        }
        else if (other.tag == "line direction collective") // jumper va chạm với line direction collective
        {
            Destroy(other.gameObject);
            _gamemanager.lineDirTextObject.GetComponent<GradualSizeEffectScript>().StartEffect();

            // active line dir
            _lineDirection.SetActive(true);
            _gamemanager.lineAppear.SetLimit(_gamemanager.lineDirLimit, false);
        }

        if (other.tag != "coin")
            MiniSoundManager.PlaySound(SOUND_NAME.SOUND_GOT_BONUS);
    }

    // enemy, landing 
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "movenemy" ||
               other.gameObject.tag == "enemy" ||
               other.gameObject.tag == "flyenemy")
        {
            _gamemanager.SetGameOver();
        }
        else if (other.gameObject.tag == "land") // jumper va chạm với land
        {
            NAVIGATION navi = porn.CollisionNavi(gameObject, other);

            if (navi == NAVIGATION.LEFT || navi == NAVIGATION.BOTTOM) // va chạm nhưng sai hướng
                _gamemanager.SetGameOver();
            else // va chạm đúng hướng rồi nè ba
            {
                rig.velocity = Vector2.zero;
                nowLandCore = other.gameObject.GetComponent<landcore>();
                nowLandCore.isNowLand = true;
                fixedJoint.connectedBody = nowLandCore.rig;
                fixedJoint.enabled = true;
                isFlying = false;
                landMover.nowLandSpeed = landMover.landSpeed;
                _gamemanager.OnLanding();
                haveDirections = false;

                // perfect get coin
                if (nowCountCoin == countGotCoin)
                    _gamemanager.TakeScoreOnPerfectCoin();

                if (!helper.JumpButtonHolding()) // auto
                {
                    isAuto = true;
                    bottomPos = nowLandCore.transform.position.y - Mathf.Abs(preVel) * vel2Dis;
                    if (bottomPos < maxMovePos.position.y)
                        bottomPos = maxMovePos.position.y;
                    nowLandCore.rig.velocity = new Vector2(0, -speedLandUp);
                    expectedVo = 0;
                }
                else // manual
                {
                    isAuto = false;
                    nowLandCore.rig.velocity = new Vector2(0, -speedLandDown);
                    isNotStillPress = false;
                    expectedVo = porn.NemXieng_FindVo(angle, nowLandCore.nextLand.transform.position.x - transform.position.x, nowLandCore.nextLand.transform.position.y + halfLandJumper - transform.position.y, true);

                    // directions update                    
                    _direction.UpdateParams();
                    _lazeDirection.UpdateParams();
                    _lineDirection.CreateLineDirection();
                }
            }
        }
    }

    // ngay trước lúc bay (land đã lên đỉnh)
    public void SetFly()
    {
        // perfect got coin
        countGotCoin = 0;
        nowCountCoin = -1;

        // zoom 
        zoomLand.zoomLand = nowLandCore.nextLand;

        // hủy nowLandScore
        nowLandCore.rig.velocity = Vector2.zero;
        nowLandCore.isNowLand = false;
        nowLandCore = null;

        // other settings
        beforeFlyingPos = transform.position;
        fixedJoint.enabled = false;
        isFlying = true;
        rig.velocity = Vector2.zero;

        // set Vo lại nếu xài directions gần giữa land
        if (haveDirections)
        {
            v0 = expectedVo;
            //print(Random.value);
        }
        flyingVel = v0 / devidedVel;
    }

    // perfect pressing land ?
    public bool PerfectLandPressing()
    {
        float sub = (nowLandCore.oriPos.y - nowLandCore.transform.position.y) / vel2Dis * game2realVel - expectedVo;

        if (nowLandCore &&
            expectedVo != 0 &&
            sub > expectedVoApproLeft && sub < expectedVoApproRight)
            return true;

        return false;
    }
}
