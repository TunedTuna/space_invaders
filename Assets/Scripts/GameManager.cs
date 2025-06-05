using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class ScoreData
{
    //public List<int> highScores = new List<int>();
    public int highScore;
}

public class GameManager : MonoBehaviour
{
    //game state
    private enum State
    {
        WaitingToStart,
        CountDownToStart,
        GamePlaying,
        GameOver,

    }
    private State state;
    public EventHandler OnStateChange;
    [Header("Scores text")]
    public TextMeshProUGUI currentScore_text;
    public TextMeshProUGUI hiscore_text;
    //public GameObject enemy;    //demo ver//the doofus off screen in Game_v1
    public GameObject barricade_prefab;

    //scorestuff
    private int score;
    private string scoreFilePath;
    private ScoreData scoreData = new ScoreData();

    [Header("Wannabe state")]
    public static GameManager Instance; //idk what this does...//we have an instance, but veryone has direct access to it...?
    private bool gameStarted = false; //to show "main menu"
    public bool gameFinished;

    [Header("Invasion")]
    public EnemyManager em;
    public TextMeshProUGUI invadedText;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip bg;

    void Start()
    {

        //at start, show the "main menu" until player presses any key
        //then hide, legend.
        //score is reset
        //keep hi score
        scoreFilePath = Application.persistentDataPath + "highscore.json";
        LoadScores();
        audioSource= GetComponent<AudioSource>();
        audioSource.clip = bg;
        audioSource.Play();

        //legend.SetActive(true);
        currentScore_text.text = "Score\n0000";
        hiscore_text.text = "Hi-Score\n" + scoreData.highScore.ToString("D4");
        gameFinished = false;
        
        invadedText.enabled = false;
        //enemy.SetActive(false);
        EnemyAble();//turn off


    }

    void Awake()
    {
        StartCoroutine(StartGameWithDelay());
        if (Instance == null)
            Instance = this;
        invadedText.enabled = false;

    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space) && !gameStarted)
        //{
        //    ////hide legend n start the game
        //    //legend.SetActive(false);
        //    //gameStarted = true;
        //    //Debug.Log("Game Started!");
        //    StartCoroutine(StartGameWithDelay());

        //}
        if (Input.GetKeyDown(KeyCode.R) && gameFinished)
        {
            RestartGame();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            gameFinished = true;
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ResetHiScore();
            EditHiScore();

        }
    }

    public void InvadedGameOver()
    {
            em.StopAllCoroutines();
        em.Invaded();
        invadedText.color = Color.red;
        invadedText.enabled = true;
            HiScoreManager();
            //player.SetActive(false);
            //EnemyAble(); //turn off
            StartCoroutine(CreditsCountdown());
        
    }
    public void WinnerGameOver()
    {
        
        HiScoreManager();
    }
 
    IEnumerator StartGameWithDelay()
    {
        state=State.WaitingToStart;
        OnStateChange?.Invoke(this,EventArgs.Empty);
        // Hide the legend and wait for 3 seconds
        //legend.SetActive(false);

        // Wait for 3 seconds
        yield return new WaitForSeconds(0.5f);

        // Now, start the game
        gameStarted = true;
        EnemyAble();// turn on
        SpawnBarricade();
        Debug.Log("Game Started!");
    }
    IEnumerator CreditsCountdown()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("Credits");
    }


    //score file stuff ---------------------------------------------------------------

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
    void EditHiScore()
    {
        string json = JsonUtility.ToJson(scoreData, true); // Convert to JSON with formatting
        File.WriteAllText(scoreFilePath, json); // Save the updated high score
        Debug.Log("High Score Saved: " + scoreData.highScore);
        hiscore_text.text = "Hi-Score\n" + scoreData.highScore.ToString("D4");
    }
    //text stuff-------------------------------------------------------------------------
    public void HiScoreManager()
    {
        //call this when player dies

        if (score > scoreData.highScore)
        {
            scoreData.highScore = score;

        }
        EditHiScore();
    }
    public void EditCurrentScore(int x)
    {
        //everytime an enemy dies
        //event?
        score += x;
        currentScore_text.text = "Score\n" + score.ToString("D4");
    }

    void ResetHiScore()
    {
        scoreData.highScore = 0;
    }
    //enemy mangager handler? ----------------------------------------------------------------
    void EnemyAble()
    {
        em.enabled = !em.enabled;
        //Debug.Log($"Script is: {em.enabled}");
    }
    //a getter just cus-------------------------------------------
    public bool GetGameFinished()
    {
        return gameFinished;
    }

    //barricade spawner?----------------------------------------------------------------------------------

    void SpawnBarricade()
    {
        float xCoord = -4.5f;
        float yCoord = -2.7f;
        //enemy = Instantiate(squid, new Vector3(j * widthPerEnemy, 1 * heightPerEnemy, 0), Quaternion.identity);
        GameObject cover = null;
        cover = Instantiate(barricade_prefab, new Vector3((xCoord), yCoord, 0), Quaternion.identity);
  
        cover = Instantiate(barricade_prefab, new Vector3(0, yCoord, 0), Quaternion.identity);
        
        cover = Instantiate(barricade_prefab, new Vector3((-xCoord), yCoord, 0), Quaternion.identity);
    }

    public bool IsGamePlaying()
    {
        return state == State.GamePlaying;
    }
    public bool IsGameWaiting()
    {
        return state==State.CountDownToStart;
    }
    public bool IsGameOver()
    {
        return state==State.GameOver;   
    }


    public bool IsGameStarted()
    {
        return gameStarted;
    }


    void RestartGame()
    {
        //just reload the scene?
        //reset enemy, current score,player position (0,-3,0)
        score = 0;
        //player.SetActive(true);
        gameFinished = false;
        
        EnemyAble();// turn on
    }



}
