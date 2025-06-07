using System.Collections;
using UnityEngine;
/// <summary>
/// this script handls the animations of the entities
/// 
/// </summary>
public class EntityVisuals : MonoBehaviour
{
    [SerializeField] private MonoBehaviour logicBehavior;
    private IToggle Logic => logicBehavior as IToggle;
    private BoxCollider2D bc2d;
    [SerializeField] private SpriteRenderer bodySpriteRenderer;
    [SerializeField] private SpriteRenderer faceSpriteRenderer;
    [SerializeField] private Sprite genericDeath;
    [SerializeField] private GameObject papa;
    [SerializeField] private Animator faceAnimator;
    [SerializeField] private Animator bodyAnimator;
    [SerializeField] private float syncFaceDelay;

    [Header("Particles")]
    public GameObject particles;
    [SerializeField] private ParticleSystem muzzleFlash;

    [Header("noise")]
    public AudioClip deathBoom;
    public AudioClip pew;
    public Animator animator;//does nothing for 3/4 enemeies, tied to mstery rn
    private void Start()
    {
        //since this script and animator should be in same object...
        animator=GetComponent<Animator>();
        bc2d = GetComponent<BoxCollider2D>();
    }

    public void PlayMuzzleFLash()
    {
        if (muzzleFlash != null)
        {
            //muzzleFlashes, 
            muzzleFlash.Play();
        }
        
    }
    public void StopParticles()
    {
        //the boosters
        particles.SetActive(false);
    }
    public void EnterShootFrame()
    {
        animator.SetTrigger("ShotFired");
        AudioSource audioSrc = gameObject.GetComponent<AudioSource>();
        audioSrc.clip = pew;
        audioSrc.Play();
    }
    public void ExitShootFrame()
    {
     
        animator.SetBool("isShoot", false);
    }
    public void FlipParticles(float position)
    {
        switch (position)
        {
            case -1:
                particles.transform.rotation = Quaternion.Euler(150, -90, -90);
                break;
            case 1:
                
                particles.transform.rotation = Quaternion.Euler(30, -90, -90);
                break;
            default:
                particles.transform.rotation = Quaternion.Euler(90, -90, -90);
                break;
        }
  
    }
    public void ToggleFlip()
    {
        bodySpriteRenderer.flipX = !bodySpriteRenderer.flipX;
    }
    public void PlayHurtNoise()
    {
        AudioSource audioSrc = GetComponent<AudioSource>();
        audioSrc.clip = deathBoom;
        audioSrc.Play();
    }

    public void IsDeadAnimation()
    {
        Logic.Disable();
        bc2d.enabled = false;
        animator.SetBool("isDead", true);
        
        bodyAnimator.SetBool("isDead", true);
        DisableFace();
        PlayHurtNoise();
        if (particles != null)
        {
            StopParticles();
        }
        //play death animation
        StartCoroutine(DestroyAfterAnimation(2.5f));
        
        
        
    }
    public void DisableFace()
    {
        //temporary, animator will handles this and the timing later
        
        if(faceSpriteRenderer != null)
        {
        faceSpriteRenderer.enabled = false;

        }
    }
    public void ShockTheHomies()
    {
        faceAnimator.SetTrigger("isShocked");
    }
 
    private IEnumerator DestroyAfterAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(papa);
    }


}
