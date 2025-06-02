using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    [SerializeField] private PlayerLogic logic;

    public GameObject particles;
    [Header("noise")]
    public AudioClip deathBoom;
    public AudioClip pew;
    public Animator animator;

    public void PlayParticles()
    {

    }
    public void StopParticles()
    {
        particles.SetActive(false);
    }
    public void EnterShootFrame()
    {
        animator.SetBool("isShoot", true);
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

    public void PlayHurtNoise()
    {
        AudioSource audioSrc = GetComponent<AudioSource>();
        audioSrc.clip = deathBoom;
        audioSrc.Play();
    }

    public void IsDeadAnimation()
    {
        logic.enabled = false;
        animator.SetBool("isDead", true);
        PlayHurtNoise();
        StopParticles();

        //stop particles
        //change game state
    }


}
