using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))] //technique for making sure there isn't a null reference during runtime if you are going to use get component
public class Bullet : MonoBehaviour
{
    private Rigidbody2D myRigidbody2D;
    private Renderer myRenderer;
    public GameObject shooter;
    public Material mats;

    public float speed = 5;
    // Start is called before the first frame update
    void Start()
    {
        myRigidbody2D = GetComponent<Rigidbody2D>();
        myRenderer = GetComponent<Renderer>();
        //mats = Resources.Load<Material>("Materials/Red");
        

        Fire();
    }

    // Update is called once per frame
    private void Fire()
    {
        if (shooter.CompareTag("Player"))
        {
            myRigidbody2D.linearVelocity = Vector2.up * speed;
            //Debug.Log("Wwweeeeee");
        }
        else
        {
            myRenderer.material= mats;
            myRigidbody2D.linearVelocity = Vector2.down * speed;
            //Debug.Log("Wwweeeeee");
        }
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
    public void setShooter(GameObject go)
    {
        shooter = go;
        if (shooter.CompareTag("Player"))
        {
            gameObject.layer = LayerMask.NameToLayer("PlayerBullet");
            gameObject.tag = "PB";

        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("EnemyBullet");
            gameObject.tag = "EB";
        }
    }
}
