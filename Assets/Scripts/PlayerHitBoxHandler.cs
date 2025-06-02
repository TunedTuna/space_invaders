using UnityEngine;

public class PlayerHitBoxHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Player playerScript;

    void OnCollisionEnter2D(Collision2D collision)
    {
       playerScript.PlayerHurt(collision.gameObject);


    }
}
