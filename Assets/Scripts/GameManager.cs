using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

[System.Serializable]
public class ScoreData
{
    //public List<int> highScores = new List<int>();
    public int highScore;
}


public class GameManager : MonoBehaviour
{

    public GameObject legend;
    public TextMeshProUGUI currentScore_text;
    public TextMeshProUGUI hiscore_text;
    public GameObject enemy;    //demo ver
    public GameObject player;
    //scorestuff
    private int score;
    private string scoreFilePath;
    private ScoreData scoreData= new ScoreData();

    public static GameManager Instance; //idk what this does...
    private bool gameStarted = false; //to show "main menu"
    public bool gameFinished;
    public bool fin;    //stop Update from spamming score change

    public EnemyManager em;

    void Start()
    {
        
        //at start, show the "main menu" until player presses any key
        //then hide, legend.
        //score is reset
        //keep hi score
        scoreFilePath = Application.persistentDataPath + "highscore.json";
        LoadScores();

        legend.SetActive(true);
        currentScore_text.text = "Score\n0000";
        hiscore_text.text ="Hi-Score\n"+ scoreData.highScore.ToString("D4");
        gameFinished = false;
        fin = false;
        enemy.SetActive(false);
        enemyAble();//turn off

    }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) &&!gameStarted) 
        {
            ////hide legend n start the game
            //legend.SetActive(false);
            //gameStarted = true;
            //Debug.Log("Game Started!");
            StartCoroutine(StartGameWithDelay());
            
        }
        if (Input.GetKeyDown(KeyCode.R) && gameFinished)
        {
            restartGame();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            gameFinished = true;
        }
        if (gameFinished && !fin)
        {
            fin = true;
            hiscoreManager();
            player.SetActive(false);
            enemyAble() ; //turn off
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            resetHiScore();
            editHiScore();
            
        }
    }

    IEnumerator StartGameWithDelay()
    {
        // Hide the legend and wait for 3 seconds
        legend.SetActive(false);

        // Wait for 3 seconds
        yield return new WaitForSeconds(0.5f);

        // Now, start the game
        gameStarted = true;
        enemyAble();// turn on
        Debug.Log("Game Started!");
    }

    public bool IsGameStarted()
    {
        return gameStarted;
    }


    void restartGame()
    {
        //reset enemy, current score,player position (0,-3,0)
        score = 0;
        player.SetActive(true);
        gameFinished = false;
        fin = false;
        enemyAble();// turn on
    }

    //score stuff ---------------------------------------------------------------
    public void hiscoreManager()
    {
        //call this when player dies
        
        if (score > scoreData.highScore)
        {
            scoreData.highScore = score;

        }
        editHiScore();
    }
    public void LoadScores()
    {
        if (File.Exists(scoreFilePath))
        {
            string json = File.ReadAllText(scoreFilePath);
            scoreData = JsonUtility.FromJson<ScoreData>(json);

        }
        else
        {
            scoreData.highScore = 0;
        }
        Debug.Log("Loaded Scores: " + string.Join(", ", scoreData.highScore));
    }
    public void SaveScore()
    {
        string json = JsonUtility.ToJson(scoreData, true); // Convert to JSON
        File.WriteAllText(scoreFilePath, json); // Save to file
        Debug.Log("High Score saved to: " + scoreFilePath);
    }
    //text stuff-------------------------------------------------------------------------
    public void editCurrentScore(int x)
    {
        //everytime an enemy dies
        //event?
        score += x;
        currentScore_text.text = "Score\n" + score.ToString("D4");
    }
    void editHiScore()
    {
        string json = JsonUtility.ToJson(scoreData, true); // Convert to JSON with formatting
        File.WriteAllText(scoreFilePath, json); // Save the updated high score
        Debug.Log("High Score Saved: " + scoreData.highScore);
        hiscore_text.text = "Hi-Score\n" + scoreData.highScore.ToString("D4");
    }
    void resetHiScore()
    {
        scoreData.highScore = 0;
    }
    //enemy mangager handler? ----------------------------------------------------------------
    void enemyAble()
    {
        em.enabled = !em.enabled;
        Debug.Log($"Script is: {em.enabled}");
    }
    //a getter just cus-------------------------------------------
    public bool getGameFinished()
    {
        return gameFinished;
    }
}
