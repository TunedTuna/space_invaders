using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //once this is a prefab, stuff will get funky
    private int scoreGiven;
    public delegate void EnemyDied(int points);
    public static event EnemyDied onEnemyDied;

    public delegate void speedDeath();
    public static event speedDeath onSpeedDeath;

    private void Start()
    {
        setScore();
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
      Debug.Log("Ouch!");
        Destroy(collision.gameObject);
        gameObject.SetActive(false);

        onEnemyDied?.Invoke(scoreGiven);
        onSpeedDeath?.Invoke();
    }
    private void setScore()
    {
        switch (gameObject.tag)
        {
            case "Crab": scoreGiven = 20; break;
            case "Squid": scoreGiven = 30; break;
            case "Mystery": scoreGiven = 100; break;
            default: scoreGiven = 10; break; // Default score
        }
    }
}
