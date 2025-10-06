using UnityEngine;

using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    AsyncOperation asyncLoad;

    void Start()
    {
        asyncLoad = SceneManager.LoadSceneAsync("Merged");
        asyncLoad.allowSceneActivation = false;
	}

	public void LoadMainScene()
    {
        asyncLoad.allowSceneActivation = true;
    }
}
