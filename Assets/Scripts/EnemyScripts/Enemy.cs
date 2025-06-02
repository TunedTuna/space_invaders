using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //once this is a prefab, stuff will get funky
    private int scoreGiven;
    public delegate void EnemyDied(int points);
    public static event EnemyDied OnEnemyDied;

    public delegate void speedDeath();
    public static event speedDeath OnSpeedDeath;
    public bool isDeado;

    public delegate void shotsFired();
    public static event shotsFired OnShotsFired;

    public GameObject bulletPrefab;
    public Transform shottingOffset;
    public float bulletCoolDown;
    public Animator animator;
    //public EnemyManager em;

    public AudioClip deathBoom;
    public AudioClip pew;
    AudioSource audioSrc;


    //everyone shares the same death "animation"
    //public Sprite deathSprite; // Assign the explosion sprite in Inspector
    //private SpriteRenderer spriteRenderer;
    //private Animator animator;


    private void Start()
    {
        SetScore();
        //bulletCoolDown = 5f;
        //animator = GetComponent<Animator>();//edited in inspecto bc child has it
        audioSrc = GetComponent<AudioSource>();
        //spriteRenderer = GetComponent<SpriteRenderer>();
        //animator = GetComponent<Animator>();
        //em = GetComponent<EnemyManager>();
        isDeado = false;
        StartCoroutine(ShootCountDown());
        
        

        //TODO make an event to ping that an enemy has fired so not everyone fires at once. then IEnumerator would most likely be replaced :0

    }
    private void Update()
    {
       
    }

    public void EnemyHurt(GameObject temp)
    {
        animator.SetBool("isDead", true);
        Destroy(temp); //destroy the bullet
        if (!isDeado)
        {
            isDeado = true;
            OnEnemyDied?.Invoke(scoreGiven);
            OnSpeedDeath?.Invoke();
            audioSrc.PlayOneShot(deathBoom);
            //audioSrc.Play();
            gameObject.layer = 11;
        }
        
        StopAllCoroutines();
        StartCoroutine(DestroyAfterDelay(1f));
    }
    private void SetScore()
    {
        switch (gameObject.tag)
        {
            case "Crab": scoreGiven = 20; bulletCoolDown = 5f; break;
            case "Squid": scoreGiven = 30; bulletCoolDown = 3f; break;
            case "Mystery": scoreGiven = 100; bulletCoolDown = 6f; break;
            default: scoreGiven = 10; bulletCoolDown = 4f; break; // Default score
        }
    }

    IEnumerator ShootInterval()
    {
        while (true) 
        {
            OnShotsFired?.Invoke();
            audioSrc.clip = pew;
            audioSrc.Play();
            GameObject shot = Instantiate(bulletPrefab, shottingOffset.position, Quaternion.identity);
            shot.GetComponent<Bullet>().setShooter(gameObject);
            //Debug.Log("Bang!");
            yield return new WaitForSeconds(bulletCoolDown);
        }
    }
    IEnumerator ShootCountDown()
    {
        yield return new WaitForSeconds(3f);
        StartCoroutine(ShootInterval());
    }
    private IEnumerator DestroyAfterDelay(float delay)
    {

        yield return new WaitForSeconds(delay);

        
        gameObject.SetActive(false);
    }
}
