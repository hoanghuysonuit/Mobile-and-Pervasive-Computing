using UnityEngine;

public class EditMode : MonoBehaviour
{
    [Range(0.0f, 10.0f)]
    public float timeScale;
    public bool timeScaleMode;

    [Tooltip("Thay đổi mode scale time bằng 1 lần nhấn enter?")]
    public bool enter2TimeScale;

    void Update()
    {
        if (timeScaleMode)
        {
            if (Time.timeScale != timeScale)
                Time.timeScale = timeScale;
        }        

        if (enter2TimeScale && porn.EnterDown())
            timeScaleMode = !timeScaleMode;            
    }
}
