using System.Collections;
using UnityEngine;

public class shoot : MonoBehaviour
{
    jumper _jumper;
    public GameObject bulletPrefab;
    float halfWidthJumper;
    Menu _menu;
    public float maxTime;
    float tick;
    bool pressed;

    void Start()
    {
        _jumper = FindObjectOfType<jumper>();
        halfWidthJumper = porn.SpriteWidth(_jumper.gameObject) / 2;
        _menu = FindObjectOfType<Menu>();
    }

    void Update()
    {
        if (!_jumper.nowLandCore && (!_menu || _menu.pauseButton.activeSelf) && Time.timeScale != 0)
        {
            if (helper.JumpButtonHolding()) 
            {                
                if (!pressed)
                {
                    tick = 0;
                    pressed = true;
                }
            }
            else
            {
                if (pressed && tick <= maxTime)
                    StartCoroutine(Shoot());

                pressed = false;
            }
        }

        if (pressed)
            tick += Time.deltaTime;
    }

    IEnumerator Shoot()
    {
        for (int i = 0; i < 1; i++)
        {
            MiniSoundManager.PlaySound(SOUND_NAME.SOUND_SHOOTING);
            Instantiate(bulletPrefab, transform.position + new Vector3(1 + halfWidthJumper, 0), Quaternion.identity);
            yield return null;
        }
    }
}
