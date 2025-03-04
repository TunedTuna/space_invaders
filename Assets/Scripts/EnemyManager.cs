using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Enemy layout")]
    public int numEnemiesAcross; //loop through number of enemies using width to seperate same and hight to seperate different types(i)
    public int widthPerEnemy; //x
    public int heightPerEnemy; //y

    [Header("Gameplay setting")]
    public float secondsPerStep;
    public float minShootInterval;

    [Header("GamEnemy Prefab")]
    //octopus, crab, squid
    public GameObject octopus; //a
    public GameObject crab; //b
    public GameObject squid; //c

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //create formation
        formation();

    }

    void Update()
    {
        
    }
    void formation()
    {
        //for  i
        //a (0+x,0,0) *i
        //b (0+x, 0+y,0)*i
        //c (0+x, 0+y,0)*i
  
            for(int j = 0; j < numEnemiesAcross; j++)
            {
            //c
            //vector3(j+widthPerEnemy ,1+ heightPerEnemy,0)
            Instantiate(squid, new Vector3(j * widthPerEnemy, 1* heightPerEnemy, 0), Quaternion.identity);
            }
            for (int j = 0; j < numEnemiesAcross; j++)
            {
            //b
            //vector3(j+widthPerEnemy ,2+ heightPerEnemy,0)
            Instantiate(crab, new Vector3(j * widthPerEnemy, 2* heightPerEnemy, 0), Quaternion.identity);
        }
            for (int j = 0; j < numEnemiesAcross; j++)
            {
            //a
            //vector3(j+widthPerEnemy ,3+ heightPerEnemy,0)
            Instantiate(octopus, new Vector3(j * widthPerEnemy, 3 * heightPerEnemy, 0), Quaternion.identity);
        }
        
    }

}
