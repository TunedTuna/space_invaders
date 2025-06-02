using UnityEngine;

public class EnemyHitBoxHandler : MonoBehaviour
{
    [SerializeField] private Enemy enemyScript;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.gameObject.CompareTag("PB")))
        {
            enemyScript.EnemyHurt(collision.gameObject);
        }



    }
}
