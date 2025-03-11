using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using static Enemy;

public class Player : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameManager gm;

    public float moveDistance = 5f;// maybe move this to game manager since enemy/play should share this

    public Transform shottingOffset;
    // pulling this from pong cus im lazy rn
    public float maxTravelHeight;
    public float minTravelHeight;
    public float speed;
    //public float collisionBallSpeedUp = 1.5f;
    public string inputAxis;
    public bool isDead;

    [Header("noise")]
    public AudioClip noise;
    public Animator animator;

    private void Start()
    {
        //subsribe
        Enemy.onEnemyDied += Enemy_onEnemyDied;
        animator= GetComponent<Animator>();
        isDead = false;
    }

    private void Enemy_onEnemyDied(int points)
    {
        //wanna know about points here
        //Debug.Log($"I know about dead Enemy: points:{points}");
        gm.editCurrentScore(points);
        //throw new System.NotImplementedException();
    }

    void Update()
    {
        if (!GameManager.Instance.IsGameStarted())
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject shot = Instantiate(bulletPrefab, shottingOffset.position, Quaternion.identity);
            shot.GetComponent<Bullet>().setShooter(gameObject);
            //Debug.Log("Bang!");

            //Destroy(shot, 3f);

        }
        if (!isDead)
        {
            float direction = Input.GetAxis(inputAxis);
            Vector3 newPosition = transform.position + new Vector3(direction, 0, 0) * speed * Time.deltaTime;
            newPosition.x = Mathf.Clamp(newPosition.x, minTravelHeight, maxTravelHeight);

            transform.position = newPosition;
        }

 
    }
    private void OnDestroy()
    {
        //unsubscribe when we die
        gm.gameFinished = true;
        Enemy.onEnemyDied -= Enemy_onEnemyDied;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        AudioSource audioSrc =GetComponent<AudioSource>();
        audioSrc.clip = noise;
        audioSrc.Play();

        //if the player shot
        //Debug.Log("oof!");
        isDead = true;
            Destroy(collision.gameObject);
        //animator.enabled = false;
        //spriteRenderer.sprite = deathSprite;
        animator.SetBool("isDead", true);
        
        StartCoroutine(waitToKill());
        


    }
    IEnumerator waitToKill()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);

    }
}
