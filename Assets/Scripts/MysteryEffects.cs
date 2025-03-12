using UnityEngine;
using static UnityEngine.ParticleSystem;

public class MysteryEffects : MonoBehaviour
{

    public GameObject effects;
    private Vector3 lastPosition;

    private void Start()
    {
        lastPosition = transform.position;
    }
    // Update is called once per frame
    void Update()
    {
       Vector3 velocity= transform.position - lastPosition;

        if (velocity.x > 0)
            effects.transform.rotation = Quaternion.Euler(0, -90, -90);
        else if (velocity.x < 0)
            effects.transform.rotation = Quaternion.Euler(180, -90, -90);
       
    }
}
