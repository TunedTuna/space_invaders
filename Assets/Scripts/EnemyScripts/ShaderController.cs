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
        mat.SetFloat("toggleGradient", 1f);
        Debug.Log("Current toggleGradient value: " + mat.GetFloat("toggleGradient"));

        Debug.Log("Gradient enabled!");


    }
    public void DisableGradient()
    {
        mat.SetFloat("toggleGradient", 0f);
        Debug.Log("Gradient disabled!");

    }
}
