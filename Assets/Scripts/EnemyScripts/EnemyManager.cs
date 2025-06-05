using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyManager : MonoBehaviour
{
    public EventHandler OnInvaded;
    [Header("Enemy layout")]
    public int numEnemiesAcross = 1; //loop through number of enemies using width to seperate same and hight to seperate different types(i)
    public float widthPerEnemy = 1f; //x
    public float heightPerEnemy = 1f; //y

    [Header("Gameplay setting")]
    public float secondsPerStep = 1;
    //public float minShootInterval;

    [Header("GamEnemy Prefab")]
    //octopus, crab, squid
    public GameObject octopus; //a
    public GameObject crab; //b
    public GameObject squid; //c
    [Header("mystery Prefab")]
    public GameObject mysteryTemp;//d
    public GameObject mysteryPrefab;//d
    public bool mysteryExist;
    public Transform mysteryTransform;
    private float mysteryRespawnTimer;
    private Coroutine mysteryCoroutine;

    [Header("Enemy Parent")]
    public Transform papaTransform;
    public int enemyRemaining;
    [SerializeField] private Vector3 startPosition;
    //these two should change based on number of enemies, otherwise they'd go offsreen
    [SerializeField] private float maxLeft=-7f;
    [SerializeField] private float maxRight = 7f;

    [SerializeField] private float internalLeft;
    [SerializeField] private float internalRight;

    [Header("Movement Settings")]
    public float moveDistance; // How far the parent moves left/right per step 5f //dog water
    public float moveSpeed = 1f;     // Speed of the choppy movement
    public float speedInc;
    public float yCoord;
    [Header("Da RUles ")]
    public GameManager gm;
    public GameObject player;
    private bool gameOver; //this script acts weird when trying to get GM's "gameFinished" //this script handles the game flow, gm has no ideas, thats why...
    

    public TextMeshProUGUI invadedText;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        Enemy.OnSpeedDeath += Enemy_onSpeedDeath;
        Enemy.OnMysteryDied += Enemy_OnMysteryDied;
        //create formation
        Formation();
        startPosition = papaTransform.position;
        StartCoroutine(MoveParent());
        enemyRemaining = numEnemiesAcross * 3;
        speedInc = 1f / enemyRemaining;
        
        gameOver = false;
        invadedText.enabled = false;
        mysteryExist = false;
        Debug.Log("Enemyremaining: " + enemyRemaining);
        gm.OnStateChange += GameManager_onStateChange; //gm tells this script that player died
        

    }
    void Update()
    {
        ManualGameReset();
        HandleMystery();
    }

    private void Enemy_OnMysteryDied()
    {
        mysteryExist = false;
        StopCoroutine(mysteryCoroutine);
    }

    private void GameManager_onStateChange(object sender, EventArgs e)
    {
        gameOver = gm.IsGameOver();
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
        OuttaEnemies();

        Debug.Log("Enemyremaining: "+enemyRemaining);
        //Debug.Log($"speed boost!, {speedInc}");
    }


    private void ManualGameReset()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            gameOver = false;

            Enemy.OnSpeedDeath += Enemy_onSpeedDeath;
            //create formation

            papaTransform.position = startPosition;//what?
            //StartCoroutine(MoveParent());
            enemyRemaining = numEnemiesAcross * 3;
            speedInc = 1f / enemyRemaining;
            secondsPerStep = 1f;
            yCoord = 0f;
            DeleteFormation();
            Formation();

        }
    }
    IEnumerator CreditsCountdown()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("Credits");
    }

    void DeleteFormation()
    {
        for (int i = papaTransform.childCount - 1; i >= 0; i--)
        {
            Transform enemy = papaTransform.GetChild(i);
            Destroy(enemy.gameObject);  // Destroy the child GameObject
        }
    }
    private void AlignEnemyBlock()
    {
        //take num enmies+1
        //get half = center
        //max left= center - getHalf
        //max right= center +getHalf
        float center = (numEnemiesAcross+1) / 2; //e.g. 5 enemies:  0 - 6, then center= 3;
        internalLeft = center * -1;
        internalRight = center;
        float theRealEdge = 8f;
        float max = theRealEdge - numEnemiesAcross;
        maxLeft = max * -1;
        maxRight = max;
        
    }

    private void SyncAlignment(GameObject enemy,int j)
    {
        Transform t = enemy.transform;
        Vector3 pos = t.position;
        Vector3 alignX;
        alignX = new Vector3((internalLeft) + (j*widthPerEnemy), pos.y, pos.z);
        
        enemy.transform.localPosition = alignX;
    }
    void Formation()
    {
        AlignEnemyBlock();
        //for  i
        //a (0+x,0,0) *i
        //b (0+x, 0+y,0)*i
        //c (0+x, 0+y,0)*i
        GameObject enemy;
        for (int j = 0; j < numEnemiesAcross; j++)
        {
            //c
            //vector3(j+widthPerEnemy ,1+ heightPerEnemy,0)
            enemy = Instantiate(squid, new Vector3((j * widthPerEnemy) , 1 * heightPerEnemy, 0), Quaternion.identity);
            enemy.transform.SetParent(papaTransform);
            //x = maxLeft+i
            SyncAlignment(enemy,j);
            


        }
        for (int j = 0; j < numEnemiesAcross; j++)
        {
            //b
            //vector3(j+widthPerEnemy ,2+ heightPerEnemy,0)
            enemy = Instantiate(crab, new Vector3(j * widthPerEnemy, 2 * heightPerEnemy, 0), Quaternion.identity);
            enemy.transform.SetParent(papaTransform);
            //x = maxLeft+i
            SyncAlignment(enemy, j);
        }
        for (int j = 0; j < numEnemiesAcross; j++)
        {
            //a
            //vector3(j+widthPerEnemy ,3+ heightPerEnemy,0)
            enemy = Instantiate(octopus, new Vector3(j * widthPerEnemy, 3 * heightPerEnemy, 0), Quaternion.identity);
            enemy.transform.SetParent(papaTransform);
            //x = maxLeft+i
            SyncAlignment(enemy,j);
        }

    }
    IEnumerator MoveParent()
    {
        //take current position
        //sweep right
        //move down
        //sweep left
        //move down
        while (!gameOver)
        //move while game start
        {
            //move right
            if (gameOver) { yield break; }

            for (float x = papaTransform.position.x; x <= maxRight; x += moveSpeed)
            {
                papaTransform.position = startPosition + new Vector3(x, yCoord, 0);
                yield return new WaitForSeconds(secondsPerStep);
            }
            yield return new WaitForSeconds(secondsPerStep);

            yCoord -= 1f;
            papaTransform.position = new Vector3(papaTransform.position.x, yCoord, 0);
            if (CheckInvasion()) { break; }


            //move left
            for (float x = papaTransform.position.x; x >= maxLeft; x -= moveSpeed)
            {
                papaTransform.position = startPosition + new Vector3(x, yCoord, 0);
                yield return new WaitForSeconds(secondsPerStep);
            }
            yield return new WaitForSeconds(secondsPerStep);
            yCoord -= 1f;
            papaTransform.position = new Vector3(papaTransform.position.x, yCoord, 0);
            if (CheckInvasion()) { break; }

        }
    }
    IEnumerator MoveMystery()
    {
        float yCoord = mysteryTemp.transform.position.y;
        for (int i = 0; i < 3; i++)
        {
            for (float x = mysteryTemp.transform.position.x; x < maxRight+3f; x += moveSpeed)
            {
                //move rigght
                mysteryTemp.transform.position = startPosition + new Vector3(x, yCoord, 0);
                yield return new WaitForSeconds(0.5f);

            }
            //chill at end  
            yield return new WaitForSeconds(2f);
            mysteryTemp.GetComponentInChildren<EntityVisuals>().ToggleFlip();
            for (float x = mysteryTemp.transform.position.x; x >maxLeft-3f; x -= moveSpeed)
            {
                // move left
                mysteryTemp.transform.position = startPosition + new Vector3(x, yCoord, 0);
                yield return new WaitForSeconds(0.5f);
            }
            //chill at end  
            //mysteryExist = false;
            yield return new WaitForSeconds(2f);
            mysteryTemp.GetComponentInChildren<EntityVisuals>().ToggleFlip();
        }
        
        
    }
    private bool CheckInvasion()
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
                    invadedText.text = $"You've been invaded\n by {enemy.name}!";
                    invadedText.enabled = true;

                    gameOver = true;
                    gm.GameOverStuff();
                    return true;
                }
            }
        }
        return false;
    }
    private void OuttaEnemies()
    {
        if (enemyRemaining <= 0)
        {
            //gm.gameFinished = true;
            //gameObject.SetActive(false);
            gameOver = true;
            invadedText.text = "Invasion stopped!";
            Debug.Log("U win!");
            invadedText.color = Color.green;
            invadedText.enabled = true;
            StartCoroutine(CreditsCountdown());
        }
    }
    //mystery---------------------------------------------------------------------------------------
    private void SpawnMystery()
    {
        float rng = UnityEngine.Random.Range(8f, 15f);
        mysteryRespawnTimer = rng;
        ///-11.5, 4, 0
        ///
        mysteryExist = true;
        mysteryTemp = Instantiate(mysteryPrefab, new Vector3(-9,6, 0), Quaternion.identity);
        mysteryTransform = mysteryTemp.transform;
        mysteryCoroutine= StartCoroutine(MoveMystery());

    }
    private void HandleMystery()
    {
        
        if (!mysteryExist )
        {
            mysteryRespawnTimer -= Time.deltaTime;
            if ( mysteryRespawnTimer < 0)
            {
                SpawnMystery();
                
            }
            
        }
    }


    private void OnDestroy()
    {
        Enemy.OnSpeedDeath -= Enemy_onSpeedDeath;
    }





}
