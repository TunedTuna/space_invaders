using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [Header("mystery Prefab")]
    public GameObject mystery;//d
    public GameObject tempMystery;
    public bool mysteryExist;
    public Transform mysteryTransform;

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

    public TextMeshProUGUI invadedText;



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
        invadedText.enabled = false;
        mysteryExist = false;
        Debug.Log("Enemyremaining: " + enemyRemaining);
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

        enemyRemaining--;
        if (enemyRemaining <= 0) 
        { enemyRemaining = 0; }

        Debug.Log("Enemyremaining: "+enemyRemaining);
        //Debug.Log($"speed boost!, {speedInc}");
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
            secondsPerStep = 1f;
            yCoord = 0f;
            deleteFormation();
            formation();
            
        }

        if (!mysteryExist)
        {
            spawnMystery();
            StartCoroutine(moveMystery());



        }
        if(enemyRemaining <= 0)
        {
            //gm.gameFinished = true;
            //gameObject.SetActive(false);
            invadedText.text = "Invasion stopped!";
            Debug.Log("U win!");
            invadedText.color = Color.green;
            invadedText.enabled = true;
            StartCoroutine(creditsCountdown());
        }



    }
    IEnumerator creditsCountdown()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Credits");
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
    IEnumerator moveMystery()
    {
        float yCoord = 4;
        float mysteryDist = moveDistance * 2;
        Transform start = mysteryTransform;
        for (int i = 0; i < 3; i++)
        {

            for (float x = start.position.x; x < moveDistance; x += moveSpeed)
            {
                //move rigght
                tempMystery.transform.position = startPosition + new Vector3(x, yCoord, 0);
                yield return new WaitForSeconds(0.5f);

            }
            //chill at end  
            yield return new WaitForSeconds(2f);
            for (float x =  moveDistance; x >-11f; x -= moveSpeed)
            {
                // move left
                tempMystery.transform.position = startPosition + new Vector3(x, yCoord, 0);
                yield return new WaitForSeconds(0.5f);
            }
            //chill at end  
            //mysteryExist = false;
            yield return new WaitForSeconds(2f);
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
                    invadedText.text = $"You've been invaded by {enemy.name}!";
                    invadedText.enabled = true;

                    gameOver = true;
                    break;
                }
            }
        }
    }
    //mystery---------------------------------------------------------------------------------------
    void spawnMystery()
    {
        ///-11.5, 4, 0
        ///
        mysteryExist = true;
        tempMystery = Instantiate(mystery, new Vector3(-11,4, 0), Quaternion.identity);
        mysteryTransform = tempMystery.transform;
        
    }





}
