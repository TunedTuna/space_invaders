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
        mat.SetFloat("_toggleGradient", 1f);
        Debug.Log("Current toggleGradient value: " + mat.GetFloat("toggleGradient"));

        Debug.Log("Gradient enabled!");


    }
    public void DisableGradient()
    {
        mat.SetFloat("_toggleGradient", 0f);
        Debug.Log("Gradient disabled!");

    }
}
