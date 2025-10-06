using UnityEngine;

using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    public void LoadMainScene()
    {
        SceneManager.LoadScene("Merged");
    }
}
