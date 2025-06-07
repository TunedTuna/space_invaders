using UnityEngine;

public class ShaderController : MonoBehaviour
{
     private Material mat;
    private void Start()
    {
        mat = GetComponent<SpriteRenderer>().material;
    }
    public void EnableGradient()
    {
        if(mat != null)
        {

        mat.SetFloat("_toggleGradient", 1f);

        Debug.Log("Gradient enabled!");

        }

    }
    public void DisableGradient()
    {
        if(mat != null)
        {

        mat.SetFloat("_toggleGradient", 0f);
        Debug.Log("Gradient disabled!");
        }

    }
}
