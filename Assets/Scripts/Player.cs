using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameManager gm;

    public Transform shottingOffset;
    // Update is called once per frame

    private void Start()
    {
        //subsribe
        Enemy.onEnemyDied += Enemy_onEnemyDied;
    }

    private void Enemy_onEnemyDied(int points)
    {
        //wanna know about points here
        Debug.Log($"I know about dead Enemy: points:{points}");
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
            Debug.Log("Bang!");

            //Destroy(shot, 3f);

        }
    }
    private void OnDestroy()
    {
        //unsubscribe when we die
        gm.gameFinished = true;
        Enemy.onEnemyDied -= Enemy_onEnemyDied;
    }
}
