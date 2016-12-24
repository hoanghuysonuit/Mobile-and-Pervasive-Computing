using UnityEngine;
using UnityEngine.UI;

public class AddScoreEffectScript : MonoBehaviour
{
    [Tooltip("Text cho effect")]
    Text text;

    [Tooltip("Hệ số trừ mỗi lần update")]
    public uint sub;

    [Tooltip("Max font size cộng thêm")]
    public uint maxBonusSize;
    
    [Tooltip("Có đang effect hay không?")]
    bool isUpdate;

    [Tooltip("Điểm")]
    uint score;

    [Tooltip("Số điểm còn chưa cộng")]
    uint nowScore;

    [Tooltip("Font size gốc")]
    int oriSize;

    void Start()
    {
        isUpdate = false;
        score = 0;
        text = GetComponent<Text>();
        text.text = score.ToString();
        oriSize = text.fontSize;
    }

    public void AddScore(uint add)
    {
        nowScore += add;
        isUpdate = true;
    }

    void Update()
    {
        if (isUpdate) // còn cập nhật
        {
            if (nowScore > 0) // còn điểm để cộng
            {
                if (nowScore > sub)
                {
                    nowScore -= sub;
                    score += sub;
                }
                else
                {
                    score += nowScore;
                    nowScore = 0;
                }

                text.text = score.ToString();
                int randSizeToBonus = Random.Range((int)0, (int)maxBonusSize);
                text.fontSize = oriSize + randSizeToBonus;
            }
            else // bắt đầu hết điểm để cộng
            {
                isUpdate = false;
                text.fontSize = oriSize;
            }
        }
    }
}
