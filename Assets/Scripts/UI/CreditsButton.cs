using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsButton : MonoBehaviour
{

    public void LoadGameScene()
    {
        SceneManager.LoadScene("Credits");
    }

}
