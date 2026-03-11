using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public UnityEvent OnSceneLoaded;

    public void LoadScene(int sceneIndex)
    {
        OnSceneLoaded.Invoke();
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
    }

    public void LoadScene(string sceneName)
    {
        OnSceneLoaded.Invoke();
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void LoadSceneAsync(int sceneIndex)
    {
        OnSceneLoaded.Invoke();
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex);
    }

    public void LoadSceneAsync(string sceneName)
    {
        OnSceneLoaded.Invoke();
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
    }

    public void LoadSceneAdditive(int sceneIndex)
    {
        OnSceneLoaded.Invoke();
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex, LoadSceneMode.Additive);
    }

    public void LoadSceneAdditive(string sceneName)
    {
        OnSceneLoaded.Invoke();
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }

    public void LoadSceneAdditiveAsync(int sceneIndex)
    {
        OnSceneLoaded.Invoke();
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
    }

    public void LoadSceneAdditiveAsync(string sceneName)
    {
        OnSceneLoaded.Invoke();
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }

    public void UnloadSceneAsync(int sceneIndex)
    {
        OnSceneLoaded.Invoke();
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneIndex);
    }

    public void UnloadSceneAsync(string sceneName)
    {
        OnSceneLoaded.Invoke();
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
    }

    public void QuitGame() => Application.Quit();
}
