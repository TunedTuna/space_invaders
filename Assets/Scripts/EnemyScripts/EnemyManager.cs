using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI.Table;

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
    [SerializeField] private float maxLeft=-6f;
    [SerializeField] private float maxRight = 6f;

    [SerializeField] private float internalLeft;
    [SerializeField] private float internalRight;
    private Coroutine moveParentThingy;

    [Header("Movement Settings")]
    public float moveDistance; // How far the parent moves left/right per step 5f //dog water
    public float moveSpeed = 0.25f;     // Speed of the choppy movement//look at the inspector
    public float speedInc;
    public float yCoord;
    [Header("Da RUles ")]
    public GameManager gm;
    public GameObject player;
    private bool gameOver; //this script acts weird when trying to get GM's "gameFinished" //this script handles the game flow, gm has no ideas, thats why...
    

    public TextMeshProUGUI invadedText;
    [Header("identification")]
    [SerializeField] List<EnemyID> listEnemyID;//might be reduandant and removed later... TODO
    private List<List<Enemy>> enemyColumns;

    [Header("Debug")]
    [SerializeField] private bool debug_stopEnemyMove = false;

    void Start()
    {
        
        Enemy.OnSpeedDeath += Enemy_onSpeedDeath;
        Enemy.OnMysteryDied += Enemy_OnMysteryDied;
        //create formation
        Formation();
        startPosition = papaTransform.position;
        if (!debug_stopEnemyMove)
        {

        moveParentThingy= StartCoroutine(MoveParent());
        }
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
        SeeListList();
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


    private void Enemy_onSpeedDeath(int col, int row)
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

        //if its not null, it is now
        Debug.Log($"ID: {enemyColumns[col][row].GetComponent<Enemy>().GetID()}, col:{col}, row:{row}");
        enemyColumns[col][row] = null;

        

        TrimEdgeColumns();
        OuttaEnemies();
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
    private void SeeListList()
    {
        //when enemy dies do this but check if start/end are null
        //if start==null, maxLeft--, enemyColumn.RemoveAt(0);
        //if end ==null, maxRight++,enemyColumn.RemoveAt(enemyColumn.Count-1);
        if (Input.GetKeyDown(KeyCode.S))
        {
            for (int col = 0; col < enemyColumns.Count; col++)
            {
                Debug.Log($"Column {col} has {enemyColumns[col].Count} enemies:");
                for (int row = 0; row < enemyColumns[col].Count; row++)
                {
                    Enemy dd = enemyColumns[col][row];
                    string enemyName = dd != null ? dd.name : "null";
                    Debug.Log($"  - Row {row}: {enemyName}");
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            for (int i = 0; i < enemyColumns[0].Count; i++)
            {
                Debug.Log($"In Z: {i}");
                Enemy dd = enemyColumns[0][i];
                string enemyName = dd != null ? dd.name : "null";
                if (enemyName == null)
                {
                    Debug.Log($"{enemyColumns[0][i]} is: {enemyName}");
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            int lastIndex = enemyColumns.Count - 1;
            for (int i = 0; i < enemyColumns[lastIndex].Count; i++)
            {
                Debug.Log($"In X: {i}");
                Enemy dd = enemyColumns[lastIndex][i];
                string enemyName = dd != null ? dd.name : "null";
                Debug.Log($"  - Row {lastIndex}: {enemyName}");
            }
        }
    }

    private void TrimEdgeColumns()
    {
        //left side(first index of enemyColumn)
        int emptyColCounter;
        bool isEmpty = true;
        while (isEmpty && enemyRemaining>0)
        {


            emptyColCounter = 0;
            for (int i = 0; i < enemyColumns[0].Count; i++)
            {

                if (enemyColumns[0][i] == null)
                {
                    emptyColCounter++;
                }
            }
            if (emptyColCounter == 3)
            {
                //left col empty, remove
                Debug.Log("lefy");
                maxLeft--;
                enemyColumns.RemoveAt(0);
                isEmpty = true;
                DecrementAllCol();

            }
            else
            {
                isEmpty = false;
            }

        }
        isEmpty = true;
        while( isEmpty&& enemyRemaining > 0)
        {
            //right side (last index of enemyColumn)
            emptyColCounter = 0;
            int lastIndex = enemyColumns.Count - 1;
            for (int i = 0; i < enemyColumns[lastIndex].Count; i++)
            {

                if (enemyColumns[lastIndex][i] == null)
                {
                    emptyColCounter++;
                }
            }
            if (emptyColCounter == 3)
            {
                //left col empty, remove
                Debug.Log("righty");
                maxRight++;
                enemyColumns.RemoveAt(lastIndex);
                isEmpty = true;
            }
            else
            {
                isEmpty = false;
            }
        }

    }
    IEnumerator CreditsCountdown()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("Credits");
    }
    private void DecrementAllCol()
    {
        for (int col = 0; col < enemyColumns.Count; col++)
        {
            for (int row = 0; row < enemyColumns[col].Count; row++)
            {
                Enemy enemy = enemyColumns[col][row];
                if (enemy != null)
                {
                    enemy.DecrementCol();
                }
            }
        }

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
        if(numEnemiesAcross > 1)
        {
        float center = 0.75f*(numEnemiesAcross-1); //did math on paper (pg48-moleskin book 2 [for me])
        internalLeft = center * -1;
        internalRight = center;
        float theRealEdge = 9f;
        float max = theRealEdge - numEnemiesAcross;
        maxLeft = max * -1;
        maxRight = max;

        }
        enemyColumns = new List<List<Enemy>>();
        for (int i = 0; i < numEnemiesAcross; i++)
        {
            enemyColumns.Add(new List<Enemy>());
        }
        
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
        if (numEnemiesAcross > 1)
        {

        AlignEnemyBlock();
        }
        //for  i
        //a (0+x,0,0) *i
        //b (0+x, 0+y,0)*i
        //c (0+x, 0+y,0)*i
        GameObject enemy;
        int row;
        int col;
        for (int j = 0; j < numEnemiesAcross; j++)
        {
            //c
            //vector3(j+widthPerEnemy ,1+ heightPerEnemy,0)
            enemy = Instantiate(squid, new Vector3((j * widthPerEnemy) , 1 * heightPerEnemy, 0), Quaternion.identity);
            enemy.transform.SetParent(papaTransform);
            //x = maxLeft+i
            SyncAlignment(enemy,j);
            enemyColumns[j].Add(enemy.GetComponent<Enemy>());
            col = j;
            row = enemyColumns[j].Count - 1;
            enemy.GetComponent<Enemy>().SetEnemyID(listEnemyID[j],col, row);


        }
        for (int j = 0; j < numEnemiesAcross; j++)
        {
            //b
            //vector3(j+widthPerEnemy ,2+ heightPerEnemy,0)
            enemy = Instantiate(crab, new Vector3(j * widthPerEnemy, 2 * heightPerEnemy, 0), Quaternion.identity);
            enemy.transform.SetParent(papaTransform);
            //x = maxLeft+i
            SyncAlignment(enemy, j);
            enemyColumns[j].Add(enemy.GetComponent<Enemy>());
            col = j;
            row = enemyColumns[j].Count - 1;
            enemy.GetComponent<Enemy>().SetEnemyID(listEnemyID[j], col, row);
        }
        for (int j = 0; j < numEnemiesAcross; j++)
        {
            //a
            //vector3(j+widthPerEnemy ,3+ heightPerEnemy,0)
            enemy = Instantiate(octopus, new Vector3(j * widthPerEnemy, 3 * heightPerEnemy, 0), Quaternion.identity);
            enemy.transform.SetParent(papaTransform);
            //x = maxLeft+i
            SyncAlignment(enemy,j);
            enemyColumns[j].Add(enemy.GetComponent<Enemy>());
            col = j;
            row = enemyColumns[j].Count - 1;
            enemy.GetComponent<Enemy>().SetEnemyID(listEnemyID[j], col, row);
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
     IEnumerator DropParent()
    {
        //GameOver animation
        while (!gameOver)
        {
            mysteryTemp.transform.position = new Vector3(mysteryTemp.transform.position.x, mysteryTemp.transform.position.y - 1, mysteryTemp.transform.position.z);
            papaTransform.position = new Vector3(papaTransform.position.x, papaTransform.position.y - 1, papaTransform.position.z);
            yield return new WaitForSeconds(0.5f);
        }
    }
    public void Invaded()
    {
        StartCoroutine(DropParent());   
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
                    gm.InvadedGameOver();
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
            gm.WinnerGameOver();
            if (moveParentThingy != null)
            {

            StopCoroutine(moveParentThingy);    
            }
            //gm.gameFinished = true;
            //gameObject.SetActive(false);
            gameOver = true;
            invadedText.text = "Invasion stopped!";
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
        Enemy.OnMysteryDied-= Enemy_OnMysteryDied;
    }





}
