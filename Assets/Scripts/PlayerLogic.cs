using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using static Enemy;

public class PlayerLogic : MonoBehaviour,IToggle
{
    public EventHandler OnPlayerDead;
    [Header("Visual")]
    [SerializeField] private EntityVisuals visuals;


    [Header("Logic")]
    public GameManager gm;
    public bool isDead;
   
    [Header("Gun")]
    public Transform shottingOffset;
    public GameObject bulletPrefab;
    // pulling this from pong cus im lazy rn
    [Header("Movement")]
    public float maxTravelHeight;
    public float minTravelHeight;
    public float speed;
    private float lastDirection;
    //public float collisionBallSpeedUp = 1.5f;
    public string inputAxis;
    public float moveDistance = 5f;// maybe move this to game manager since enemy/play should share this




    private void Start()
    {
        //subsribe
        Enemy.OnEnemyDied += Enemy_onEnemyDied;
        //animator= GetComponent<Animator>();
        isDead = false;
    }

    private void Enemy_onEnemyDied(int points)
    {
        //wanna know about points here
        //Debug.Log($"I know about dead Enemy: points:{points}");
        gm.EditCurrentScore(points);
        //throw new System.NotImplementedException();
    }

    void Update()
    {
        if (!GameManager.Instance.IsGameStarted())
        {
            return;
        }

        ShootAnimation();
        PrimativeMovement();


    }
    private void OnDestroy()
    {
        //unsubscribe when we die
        visuals.StopParticles();
        gm.gameFinished = true;
        Enemy.OnEnemyDied -= Enemy_onEnemyDied;
    }
  

    IEnumerator ShootAni()
    {
        visuals.EnterShootFrame();
        visuals.PlayMuzzleFLash();
        //yield return new WaitForSeconds(1f);//let animaton sync w/ bullet
        GameObject shot = Instantiate(bulletPrefab, shottingOffset.position, Quaternion.identity);
        shot.GetComponent<Bullet>().setShooter(gameObject);
        yield return new WaitForSeconds(4f);
        visuals.ExitShootFrame();
    }

    private void PrimativeMovement()
    {
        if (!isDead)
        {

            float direction = Input.GetAxis(inputAxis);
           
            Vector3 newPosition = transform.position + new Vector3(direction, 0, 0) * speed * Time.deltaTime;
            newPosition.x = Mathf.Clamp(newPosition.x, minTravelHeight, maxTravelHeight);

            transform.position = newPosition;
            float temp= Input.GetAxisRaw(inputAxis);
            visuals.FlipParticles(temp);
            lastDirection = direction;
        }
    }
    private void ShootAnimation()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(ShootAni());
            //Debug.Log("Bang!");

            //Destroy(shot, 3f);
        }
    }

    public void PlayerHurt(GameObject temp)
    {
        visuals.PlayHurtNoise();

       
        isDead = true;
        Destroy(temp);
        //do what script has to do before shutting down
        gm.gameFinished = true;
        Enemy.OnEnemyDied -= Enemy_onEnemyDied;

        //visuals take over
        gm.GameOverStuff();
        visuals.IsDeadAnimation();
    
        


    }

    //IToggle contracts
    public void Enable() => enabled = true;
    public void Disable() => enabled = false;
}
