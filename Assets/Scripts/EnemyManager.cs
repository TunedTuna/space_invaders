using System.Collections;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Enemy layout")]
    public int numEnemiesAcross = 1; //loop through number of enemies using width to seperate same and hight to seperate different types(i)
    public int widthPerEnemy = 1; //x
    public int heightPerEnemy = 1; //y

    [Header("Gameplay setting")]
    public float secondsPerStep = 1;
    //public float minShootInterval;

    [Header("GamEnemy Prefab")]
    //octopus, crab, squid
    public GameObject octopus; //a
    public GameObject crab; //b
    public GameObject squid; //c

    [Header("Enemy Parent")]
    public Transform papaTransform;
    public int enemyRemaining;
    private Vector3 startPosition;

    [Header("Movement Settings")]
    public float moveDistance = 5f;  // How far the parent moves left/right per step
    public float moveSpeed = 1f;     // Speed of the choppy movement
    public float speedInc;
    public float yCoord = 0f;
    [Header("Da RUles ")]
    public GameManager gm;
    public GameObject player;
    private bool gameOver; //this script acts weird when trying to get GM's "gameFinished"



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        Enemy.onSpeedDeath += Enemy_onSpeedDeath;
        //create formation
        formation();
        startPosition = papaTransform.position;
        StartCoroutine(MoveParent());
        enemyRemaining = numEnemiesAcross * 3;
        speedInc = 1f / enemyRemaining;
        yCoord = 0f;
        gameOver = false;
    }

    private void Enemy_onSpeedDeath()
    {
        if (secondsPerStep - speedInc < 0f)
        {
            secondsPerStep = 0.1f;
        }
        else
        {
            secondsPerStep -= speedInc;
        }
        Debug.Log($"speed boost!, {speedInc}");
    }

    void Update()
    {
        //move left/right until 7 then move down one. move/sec . the less enmes= smaller sec
        if (Input.GetKeyDown(KeyCode.R))
        {
            gameOver = false;
          
            
            Enemy.onSpeedDeath += Enemy_onSpeedDeath;
            //create formation
            
            papaTransform.position = startPosition;
            //StartCoroutine(MoveParent());
            enemyRemaining = numEnemiesAcross * 3;
            speedInc = 1f / enemyRemaining;
            yCoord = 0f;
            deleteFormation();
            formation();
            
        }


    }

    void deleteFormation()
    {
        for (int i = papaTransform.childCount - 1; i >= 0; i--)
        {
            Transform enemy = papaTransform.GetChild(i);
            Destroy(enemy.gameObject);  // Destroy the child GameObject
        }
    }
    void formation()
    {
        //for  i
        //a (0+x,0,0) *i
        //b (0+x, 0+y,0)*i
        //c (0+x, 0+y,0)*i
        GameObject enemy = null;
        for (int j = 0; j < numEnemiesAcross; j++)
        {
            //c
            //vector3(j+widthPerEnemy ,1+ heightPerEnemy,0)
            enemy = Instantiate(squid, new Vector3(j * widthPerEnemy, 1 * heightPerEnemy, 0), Quaternion.identity);
            enemy.transform.SetParent(papaTransform);


        }
        for (int j = 0; j < numEnemiesAcross; j++)
        {
            //b
            //vector3(j+widthPerEnemy ,2+ heightPerEnemy,0)
            enemy = Instantiate(crab, new Vector3(j * widthPerEnemy, 2 * heightPerEnemy, 0), Quaternion.identity);
            enemy.transform.SetParent(papaTransform);

        }
        for (int j = 0; j < numEnemiesAcross; j++)
        {
            //a
            //vector3(j+widthPerEnemy ,3+ heightPerEnemy,0)
            enemy = Instantiate(octopus, new Vector3(j * widthPerEnemy, 3 * heightPerEnemy, 0), Quaternion.identity);
            enemy.transform.SetParent(papaTransform);

        }

    }
    IEnumerator MoveParent()
    {
        while (!gameOver)
        //move while game start
        {
            if (gameOver) { yield break; }

            for (float x = 0; x < moveDistance; x += moveSpeed)
            {
                papaTransform.position = startPosition + new Vector3(x, yCoord, 0);
                yield return new WaitForSeconds(secondsPerStep);
            }

            yCoord -= 1f;
            papaTransform.position = new Vector3(papaTransform.position.x, yCoord, 0);
            checkInvasion();
            yield return new WaitForSeconds(secondsPerStep);

            for (float x = moveDistance; x > 0; x -= moveSpeed)
            {
                papaTransform.position = startPosition + new Vector3(x, yCoord, 0);
                yield return new WaitForSeconds(secondsPerStep);
            }
            yCoord -= 1f;
            papaTransform.position = new Vector3(papaTransform.position.x, yCoord, 0);
            checkInvasion();
            yield return new WaitForSeconds(secondsPerStep);

        }
    }
    void checkInvasion()
    {
        for (int i = 0; i < papaTransform.childCount; i++)
        {
            Transform enemy = papaTransform.GetChild(i);

            if (enemy != null && enemy.gameObject.activeSelf)
            {
                if (enemy.position.y <= player.transform.position.y)
                {
                    gm.gameFinished = true;
                    Debug.Log("You've been invaded by " + enemy.name + "!");
                    gameOver = true;
                    break;
                }
            }
        }
    }




}
