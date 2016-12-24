using UnityEngine;
using System.Collections;

public class RemoveOutCamera : MonoBehaviour
{
    public bool left;
    public bool right;
    public bool top;
    public bool bottom;

    NAVIGATION[] para = new NAVIGATION[4];

    void Start()
    {
        if (left)
            para[0] = NAVIGATION.LEFT;
        else
            para[0] = NAVIGATION.NAVI_NONE;

        if (right)
            para[1] = NAVIGATION.RIGHT;
        else
            para[1] = NAVIGATION.NAVI_NONE;

        if (top)
            para[2] = NAVIGATION.TOP;
        else
            para[2] = NAVIGATION.NAVI_NONE;

        if (bottom)
            para[3] = NAVIGATION.BOTTOM;
        else
            para[3] = NAVIGATION.NAVI_NONE;
    }

    void Update()
    {
        porn.DestroyIfOut(gameObject, para[0], para[1], para[2], para[3]);
    }
}
