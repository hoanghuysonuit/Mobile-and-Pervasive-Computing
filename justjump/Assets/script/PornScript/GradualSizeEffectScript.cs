using UnityEngine;

public enum STATE_GRADUAL_SIZE_SCRIPT
{
    PROCESSING_GRADUAL_SIZE_SCRIPT, // Đang gradual
    JUST_FINISHED_GRADUAL_SIZE_SCRIPT, // Ngay lúc hoàn tất
    DONE_GRADUAL_SIZE_SCRIPT // Đã sử dụng xong
}

public class GradualSizeEffectScript : MonoBehaviour
{
    [Tooltip("Kích thước bắt đầu")]
    public GameObject start;

    [Tooltip("Kích thước cuối")]
    public GameObject end;

    [Tooltip("Số bước thực hiện. Càng lớn càng lâu")]
    public uint step;

    [Tooltip("Kích thước thêm vào mỗi bước")]
    float addSize;

    [Tooltip("Trạng thái")]
    STATE_GRADUAL_SIZE_SCRIPT state;

    [Tooltip("Start khi bắt đầu")]
    public bool startWhenLoad;

    [Space(10), Tooltip("Disable sau khi finish?")]
    public bool disableAfterFinishing;
    [Tooltip("Thời gian hiển thị sau hiệu ứng (Khi destroyAfterFinish true)")]
    public float timeDisplay;
    float tick;

    void Start()
    {
        if (step == 0)
            step = 1;
        addSize = (end.transform.localScale.x - start.transform.localScale.x) / step;

        if (startWhenLoad)
            StartEffect();
        else
            state = STATE_GRADUAL_SIZE_SCRIPT.PROCESSING_GRADUAL_SIZE_SCRIPT;
    }

    void Update()
    {
        if (state == STATE_GRADUAL_SIZE_SCRIPT.PROCESSING_GRADUAL_SIZE_SCRIPT)
        {
            porn.Scale(gameObject, transform.localScale.x + addSize);
            if (transform.localScale.x >= end.transform.localScale.x)
            {
                state = STATE_GRADUAL_SIZE_SCRIPT.JUST_FINISHED_GRADUAL_SIZE_SCRIPT;
                tick = 0;
            }
        }
        else if (state == STATE_GRADUAL_SIZE_SCRIPT.JUST_FINISHED_GRADUAL_SIZE_SCRIPT)
        {
            if (disableAfterFinishing)
            {
                tick += Time.deltaTime;
                if (tick >= timeDisplay)
                    state = STATE_GRADUAL_SIZE_SCRIPT.DONE_GRADUAL_SIZE_SCRIPT;
            }
            else
                state = STATE_GRADUAL_SIZE_SCRIPT.DONE_GRADUAL_SIZE_SCRIPT;
        }
        else 
        {
            if (disableAfterFinishing)
                gameObject.SetActive(false);
        }
    }

    public void StartEffect()
    {
        gameObject.SetActive(true);
        porn.Scale(gameObject, start.transform.localScale.x);
        state = STATE_GRADUAL_SIZE_SCRIPT.PROCESSING_GRADUAL_SIZE_SCRIPT;
    }

    public void SetDoneState()
    {
        state = STATE_GRADUAL_SIZE_SCRIPT.DONE_GRADUAL_SIZE_SCRIPT;
    }

    public bool IsJustFinished()
    {
        return state == STATE_GRADUAL_SIZE_SCRIPT.JUST_FINISHED_GRADUAL_SIZE_SCRIPT;
    }

    public bool IsProcessing()
    {
        return state == STATE_GRADUAL_SIZE_SCRIPT.PROCESSING_GRADUAL_SIZE_SCRIPT;
    }

    public bool IsDone()
    {
        return state == STATE_GRADUAL_SIZE_SCRIPT.DONE_GRADUAL_SIZE_SCRIPT;
    }
}
