using System.Collections;
using UnityEngine;

public class MysteryHandler : MonoBehaviour
{
    [Header("mystery Prefab")]
    public GameObject mysteryTemp;//d
    public GameObject mysteryPrefab;//d
    public bool mysteryExist;
    public Transform mysteryTransform;
    private float mysteryRespawnTimer;
    private Coroutine mysteryCoroutine;

    [SerializeField] private Vector3 startPosition;
    public Transform papaTransform;

    [SerializeField] private float moveSpeed = 1.25f;
    void Start()
    {
        mysteryExist = false;
        Enemy.OnMysteryDied += Enemy_OnMysteryDied;
        startPosition=papaTransform.position;
    }

    void Update()
    {
        HandleMystery();
    }
    private void Enemy_OnMysteryDied()
    {
        mysteryExist = false;
        StopCoroutine(mysteryCoroutine);
    }

    IEnumerator MoveMystery()
    {
    
        float realMaxRight = 8f;
        float realMaxLeft = -8f;
        float yCoord = mysteryTemp.transform.position.y;

        for (int i = 0; i < 3; i++)
        {
            for (float x = mysteryTemp.transform.position.x; x < realMaxRight; x += moveSpeed)
            {
                //move rigght
                mysteryTemp.transform.position = startPosition + new Vector3(x, yCoord, 0);
                yield return new WaitForSeconds(0.5f);

            }
            //chill at end  
            yield return new WaitForSeconds(2f);
            mysteryTemp.GetComponentInChildren<EntityVisuals>().ToggleFlip();
            for (float x = mysteryTemp.transform.position.x; x > realMaxLeft; x -= moveSpeed)
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
    private void SpawnMystery()
    {
        float rng = UnityEngine.Random.Range(8f, 15f);
        mysteryRespawnTimer = rng;
        mysteryExist = true;
        mysteryTemp = Instantiate(mysteryPrefab, new Vector3(-9, 6, 0), Quaternion.identity);
        mysteryTransform = mysteryTemp.transform;
        mysteryCoroutine = StartCoroutine(MoveMystery());

    }
    private void HandleMystery()
    {

        if (!mysteryExist)
        {
            mysteryRespawnTimer -= Time.deltaTime;
            if (mysteryRespawnTimer < 0)
            {
                SpawnMystery();

            }

        }
    }

    private void OnDestroy()
    {
        Enemy.OnMysteryDied -= Enemy_OnMysteryDied;
    }

    IEnumerator DropMystery()
    {
        while (GameManager.Instance.GetGameFinished())
        {
            mysteryTemp.transform.position = new Vector3(mysteryTemp.transform.position.x, mysteryTemp.transform.position.y - 1, mysteryTemp.transform.position.z);
            yield return new WaitForSeconds(0.5f);
        }
    }

}
