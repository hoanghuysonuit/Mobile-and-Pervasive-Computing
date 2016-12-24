using UnityEngine;

public enum SOUND_NAME
{
    SOUND_GET_COIN, SOUND_LANDING, SOUND_GOT_PERFECT_COIN, SOUND_GOT_BONUS, SOUND_ENEMY_DIES, SOUND_SHOOTING,
    SOUND_GAME_OVER, SOUND_MAKE_COIN, SOUND_CLICK_BUTTON, SOUND_COUNTDOWN
}

public enum MUSIC_NAME
{
    MUSIC_BKG
}

public class MiniSoundManager : MonoBehaviour
{
    [Range(0, 10)]
    public uint musicVolume, soundVolume;

    [Tooltip("List sounds")]
    public AudioSource[] listSounds;
    static AudioSource[] listSound;
    static float[] oriSoundVolume;

    [Tooltip("List musics")]
    public AudioSource[] listMusics;
    static AudioSource[] listMusic;
    static float[] oriMusicVolume;

    void Start()
    {
        listSound = listSounds;
        listMusic = listMusics;

        oriMusicVolume = new float[listMusic.Length];
        oriSoundVolume = new float[listSound.Length];
        for (int i = 0; i < listMusic.Length; i++)
            oriMusicVolume[i] = listMusic[i].volume;
        for (int i = 0; i < listSound.Length; i++)
            oriSoundVolume[i] = listSound[i].volume;

        ChangeMusicVolume(musicVolume / 10.0f);
        ChangeSoundVolume(soundVolume / 10.0f);
    }

    public static void PlaySound(SOUND_NAME name)
    {
        listSound[(int)name].PlayOneShot(listSound[(int)name].clip);
    }
      
    void PlayMusic(MUSIC_NAME name)
    {
    }

    /// <summary>
    /// Volume truyền vào chuẩn là từ 0 đến 1
    /// </summary>
    public static void ChangeSoundVolume(float volume)
    {
        for (int i = 0; i < listSound.Length; i++)
        {
            listSound[i].volume = volume * oriSoundVolume[i];
        }
    }

    /// <summary>
    /// Volume truyền vào chuẩn là từ 0 đến 1
    /// </summary>
    public static void ChangeMusicVolume(float volume)
    {
        for (int i = 0; i < listMusic.Length; i++)
        {
            listMusic[i].volume = volume * oriMusicVolume[i];
        }
    }
}
