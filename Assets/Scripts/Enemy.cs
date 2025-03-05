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

    public GameObject bulletPrefab;
    public Transform shottingOffset;
    public float bulletCoolDown;

    //everyone shares the same death "animation"
    //public Sprite deathSprite; // Assign the explosion sprite in Inspector
    //private SpriteRenderer spriteRenderer;
    //private Animator animator;

    private void Start()
    {
        setScore();
        bulletCoolDown = 5f;
        //spriteRenderer = GetComponent<SpriteRenderer>();
        //animator = GetComponent<Animator>();
        StartCoroutine(shootInterval());
       
    }
    private void Update()
    {
        //GameObject shot = Instantiate(bulletPrefab, shottingOffset.position, Quaternion.identity);
        //shot.GetComponent<Bullet>().setShooter(gameObject);
        //Debug.Log("Bang!");
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.gameObject.CompareTag("PB")))
        {
            //if the player shot
            Debug.Log("Ouch!");
            Destroy(collision.gameObject);
            //animator.enabled = false;
            //spriteRenderer.sprite = deathSprite;
            gameObject.SetActive(false);

            onEnemyDied?.Invoke(scoreGiven);
            onSpeedDeath?.Invoke();
        }
 


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

    IEnumerator shootInterval()
    {
        while (true) 
        {
            GameObject shot = Instantiate(bulletPrefab, shottingOffset.position, Quaternion.identity);
            shot.GetComponent<Bullet>().setShooter(gameObject);
            Debug.Log("Bang!");
            yield return new WaitForSeconds(bulletCoolDown);
        }
    }
}
