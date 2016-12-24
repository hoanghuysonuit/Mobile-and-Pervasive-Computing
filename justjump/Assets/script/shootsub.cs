using UnityEngine;
using System.Collections;

public class shootsub : MonoBehaviour
{
    public float speed;
    gamemanager gameManager;

    void Start()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(speed, 0);
        gameManager = FindObjectOfType<gamemanager>();
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "movenemy" ||
               other.gameObject.tag == "enemy" ||
               other.gameObject.tag == "flyenemy") // bắn trúng enymies
        {
            gameManager.SetEnemiesDie(other.gameObject, true);
            gameManager.scoreText.GetComponent<AddScoreEffectScript>().AddScore(gameManager.killEnemiesScore);
        }

        Destroy(gameObject);
    }
    
}
