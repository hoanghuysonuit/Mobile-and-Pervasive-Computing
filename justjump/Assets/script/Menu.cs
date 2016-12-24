using UnityEngine;
using UnityEngine.UI;
using DigitalRuby.SoundManagerNamespace;

public class Menu : MonoBehaviour
{
    public GameObject startMenu;
    public GameObject pauseButton;
    public GameObject pauseMenu;
    public GameObject replayMenu;
    public GameObject scoreCanvas;
    public GameObject countdownCanvas;
    public Slider musicVolume, soundVolume;
    public GameObject settingMenu;

    jumper _jumper;
    gamemanager gameManager;
    bool isGameOver;
    float tickWaitGameOver;
    MiniSoundManager miniSoundManager;

    [Tooltip("Thời gian chờ lúc game over"), Space(10)]
    public float timeWaitGameOver;

    void Start()
    {
        Time.timeScale = 0;
        _jumper = FindObjectOfType<jumper>();
        gameManager = FindObjectOfType<gamemanager>();
        if (PlayerPrefs.GetString("start_menu_state") == "off")
        {
            PlayerPrefs.DeleteAll();
            StartButton(false);
        }
        miniSoundManager = FindObjectOfType<MiniSoundManager>();
    }

    void Update()
    {
        if (pauseButton.activeInHierarchy && Input.GetKeyDown(KeyCode.Escape))
            PauseButton();

        // delay để lên màn hình gameover
        if (isGameOver)
        {
            if (Time.realtimeSinceStartup - tickWaitGameOver > timeWaitGameOver)
            {
                ActiveGameOverCanvas();
                isGameOver = false;
            }
        }
    }
    
    public void StartButton(bool haveSound = true)
    {
        startMenu.SetActive(false);
        Time.timeScale = 1;
        pauseButton.SetActive(true);
        scoreCanvas.SetActive(true);

        if (haveSound)
            MiniSoundManager.PlaySound(SOUND_NAME.SOUND_CLICK_BUTTON);
    }

    public void PauseButton()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        pauseButton.SetActive(false);
        scoreCanvas.SetActive(false);
        MiniSoundManager.PlaySound(SOUND_NAME.SOUND_CLICK_BUTTON);
        _jumper.gameObject.SetActive(false);

        // bỏ hết đạn hiện tại
        foreach (shootsub ele in FindObjectsOfType<shootsub>())
            Destroy(ele.gameObject);
    }

    public void ResumeButton()
    {
        pauseMenu.SetActive(false);
        scoreCanvas.SetActive(true);
        MiniSoundManager.PlaySound(SOUND_NAME.SOUND_CLICK_BUTTON);
        countdownCanvas.GetComponent<CountdownCanvas>().StartCountdown();
        _jumper.gameObject.SetActive(true);
    }

    public void SetResumeAfterCountdown()
    {
        Time.timeScale = 1;
        pauseButton.SetActive(true);
    }

    public void TryAgainButton()
    {
        gameManager.ReloadScene();
        PlayerPrefs.SetString("start_menu_state", "off");
        MiniSoundManager.PlaySound(SOUND_NAME.SOUND_CLICK_BUTTON);
    }

    public void SetGameOver()
    {
        isGameOver = true;
        tickWaitGameOver = Time.realtimeSinceStartup;
        Time.timeScale = 0;
        MiniSoundManager.PlaySound(SOUND_NAME.SOUND_GAME_OVER);
    }

    public void ActiveGameOverCanvas()
    {
        scoreCanvas.SetActive(false);
        replayMenu.SetActive(true);
        pauseButton.SetActive(false);
        _jumper.gameObject.SetActive(false);
    }

    public void OkSettingButton()
    {
        settingMenu.SetActive(false);
        startMenu.SetActive(true);
        MiniSoundManager.ChangeSoundVolume(soundVolume.value / 10.0f);
        MiniSoundManager.ChangeMusicVolume(musicVolume.value / 10.0f);
        miniSoundManager.soundVolume = (uint)soundVolume.value;
        miniSoundManager.musicVolume = (uint)musicVolume.value;
    }

    public void SettingButton()
    {
        settingMenu.SetActive(true);
        startMenu.SetActive(false);
        musicVolume.value = (int)(SoundManager.MusicVolume * 10);
        soundVolume.value = miniSoundManager.soundVolume;
        musicVolume.value = miniSoundManager.musicVolume;
    }

    public void MenuButton()
    {
        gameManager.ReloadScene();
    }
}
