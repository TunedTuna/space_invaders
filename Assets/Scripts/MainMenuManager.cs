using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public AudioSource audioSrc;
    public AudioClip bgMusic;
    void Start()
    {
        Animator anim = GetComponent<Animator>();
        audioSrc = GetComponent<AudioSource>();
        audioSrc.clip = bgMusic;
        audioSrc.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
