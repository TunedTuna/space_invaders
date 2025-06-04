using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour,IToggle
{
    private enum EnemyType
    {
        Crab,
        Squid,
        Octopus,
        Mystery
    }

    [Header("Visual")]
    [SerializeField] private EntityVisuals visuals;
    [SerializeField] private EnemyType enemyType;
    [Header("Scores?")]
    //once this is a prefab, stuff will get funky
    private int scoreGiven;
    public delegate void EnemyDied(int points);
    public static event EnemyDied OnEnemyDied;

    public delegate void speedDeath();
    public static event speedDeath OnSpeedDeath;
    public bool isDeado;

    public delegate void shotsFired();
    public static event shotsFired OnShotsFired;

    [Header("Bullet")]
    public GameObject bulletPrefab;
    public Transform shottingOffset;
    public float bulletCoolDown;

    private const int DEAD_LAYER = 11;



    //everyone shares the same death "animation"
    //public Sprite deathSprite; // Assign the explosion sprite in Inspector
    //private SpriteRenderer spriteRenderer;
    //private Animator animator;


    private void Start()
    {
        SetScore();
        //bulletCoolDown = 5f;
        //animator = GetComponent<Animator>();//edited in inspecto bc child has it
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
        
        Destroy(temp); //destroy the bullet
        //do logic then hand keys to Visuals
        if (!isDeado)
        {
            isDeado = true;
            OnEnemyDied?.Invoke(scoreGiven);
            OnSpeedDeath?.Invoke();
            
            //audioSrc.Play();
            gameObject.layer = DEAD_LAYER;
        }
        
        StopAllCoroutines();
        visuals.IsDeadAnimation();

    }
    private void SetScore()
    {
        switch (enemyType)
        {
            case EnemyType.Crab: scoreGiven = 20; bulletCoolDown = 5f; break;
            case EnemyType.Squid: scoreGiven = 30; bulletCoolDown = 3f; break;
            case EnemyType.Mystery: scoreGiven = 100; bulletCoolDown = 6f; break;
            default: scoreGiven = 10; bulletCoolDown = 4f; break; // Default score
        }
    }

    IEnumerator ShootInterval()
    {
        while (true) 
        {
            OnShotsFired?.Invoke();
            visuals.EnterShootFrame();
            GameObject shot = Instantiate(bulletPrefab, shottingOffset.position, Quaternion.identity);
            shot.GetComponent<Bullet>().setShooter(gameObject);
            //Debug.Log("Bang!");
            yield return new WaitForSeconds(bulletCoolDown);
            visuals.ExitShootFrame();
        }
        
    }
    IEnumerator ShootCountDown()
    {
        yield return new WaitForSeconds(bulletCoolDown);
        StartCoroutine(ShootInterval());
    }

    //IToggle contracts
    public void Enable() => enabled = true;
    public void Disable() => enabled = false;

}
