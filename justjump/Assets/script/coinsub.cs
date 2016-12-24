using UnityEngine;

public class coinsub : MonoBehaviour
{
    public int listIndex, listCount;

    void Update()
    {
        porn.DestroyIfOut(gameObject, NAVIGATION.LEFT);
    }
}
