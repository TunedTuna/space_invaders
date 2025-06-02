using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsTimer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        StartCoroutine(CreditClock(5f));
    }

    IEnumerator CreditClock(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene("MainMenu");
    }

    //IEnumerator StartGameWithDelay()
    //{
    //    // Hide the legend and wait for 3 seconds
    //    legend.SetActive(false);

    //    // Wait for 3 seconds
    //    yield return new WaitForSeconds(0.5f);

    //    // Now, start the game
    //    gameStarted = true;
    //    enemyAble();// turn on
    //    spawnBarricade();
    //    Debug.Log("Game Started!");
    //}

}
