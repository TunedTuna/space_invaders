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
    public Animator animator;
    //public EnemyManager em;
    
    
    //everyone shares the same death "animation"
    //public Sprite deathSprite; // Assign the explosion sprite in Inspector
    //private SpriteRenderer spriteRenderer;
    //private Animator animator;


    private void Start()
    {
        setScore();
        //bulletCoolDown = 5f;
        animator = GetComponent<Animator>();
        //spriteRenderer = GetComponent<SpriteRenderer>();
        //animator = GetComponent<Animator>();
        //em = GetComponent<EnemyManager>();
        StartCoroutine(shootCountDown());

        //TODO make an event to ping that an enemy has fired so not everyone fires at once. then IEnumerator would most likely be replaced :0

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
          
            animator.SetBool("isDead", true);
            Destroy(collision.gameObject);
            onEnemyDied?.Invoke(scoreGiven);
            onSpeedDeath?.Invoke();

            bulletCoolDown = 10f;
            StartCoroutine(DestroyAfterDelay(0.5f));

        }
 


    }
    private void setScore()
    {
        switch (gameObject.tag)
        {
            case "Crab": scoreGiven = 20; bulletCoolDown = 5f; break;
            case "Squid": scoreGiven = 30; bulletCoolDown = 3f; break;
            case "Mystery": scoreGiven = 100; bulletCoolDown = 2f; break;
            default: scoreGiven = 10; bulletCoolDown = 4f; break; // Default score
        }
    }

    IEnumerator shootInterval()
    {
        while (true) 
        {
            GameObject shot = Instantiate(bulletPrefab, shottingOffset.position, Quaternion.identity);
            shot.GetComponent<Bullet>().setShooter(gameObject);
            //Debug.Log("Bang!");
            yield return new WaitForSeconds(bulletCoolDown);
        }
    }
    IEnumerator shootCountDown()
    {
        yield return new WaitForSeconds(3f);
        StartCoroutine(shootInterval());
    }
    private IEnumerator DestroyAfterDelay(float delay)
    {
      
        yield return new WaitForSeconds(delay);

        
        gameObject.SetActive(false);
    }
}
