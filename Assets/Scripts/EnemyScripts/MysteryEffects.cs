using System.Collections;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class MysteryEffects : MonoBehaviour
{

    public GameObject effects;
    private Vector3 lastPosition;
    public SpriteRenderer sr;
    public Animator animator;

    private void Start()
    {
        lastPosition = transform.position;
        sr = GetComponent<SpriteRenderer>();
        Enemy.OnShotsFired += Enemy_onShotsFired;
        animator= GetComponent<Animator>();
        
    }

    private void Enemy_onShotsFired()
    {
        StartCoroutine(shootClip());
    }

    // Update is called once per frame
    void Update()
    {
       Vector3 velocity= transform.position - lastPosition;

        if (velocity.x > 0)
        {
            effects.transform.rotation = Quaternion.Euler(0, -90, -90);
            sr.flipX = true;
            Debug.Log("Flipping Right");
        }
            

        else if (velocity.x < 0)
        {
            effects.transform.rotation = Quaternion.Euler(180, -90, -90);
            sr.flipX = false;
            Debug.Log("Flipping Left");
        }

        lastPosition = transform.position; // Update lastPosition

    }
    IEnumerator shootClip()
    {
        animator.SetBool("isShoot", true);
        yield return new WaitForSeconds(4f);
        animator.SetBool("isShoot", false);
    }
    private void OnDestroy()
    {
        Enemy.OnShotsFired -= Enemy_onShotsFired;
    }
}
