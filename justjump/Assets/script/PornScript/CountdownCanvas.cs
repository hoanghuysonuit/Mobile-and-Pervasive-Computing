using UnityEngine;

public class CountdownCanvas : MonoBehaviour
{
    [Tooltip("Số giây cho 1 lần đếm")]
    public float time;
    float tick;

    [Tooltip("3 hình countdown")]
    public GameObject image1, image2, image3;

    Menu _menu;
    bool active;

    void Start()
    {
        _menu = FindObjectOfType<Menu>();
    }

    void Update()
    {
        if (!active)
            return;

        if (Time.realtimeSinceStartup - tick > time)
        {
            if (image3.activeSelf)
            {
                image2.SetActive(true);
                image3.SetActive(false);
            }
            else if (image2.activeSelf)
            {
                image1.SetActive(true);
                image2.SetActive(false);
            }
            else if (image1.activeSelf)
            {
                image1.SetActive(false);
                _menu.SetResumeAfterCountdown();
                active = false;
            }

            tick = Time.realtimeSinceStartup;

            if (active)
                MiniSoundManager.PlaySound(SOUND_NAME.SOUND_COUNTDOWN);
        }
    }

    public void StartCountdown()
    {
        tick = Time.realtimeSinceStartup;
        image3.SetActive(true);
        MiniSoundManager.PlaySound(SOUND_NAME.SOUND_COUNTDOWN);
        active = true;
    }
}
