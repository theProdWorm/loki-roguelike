using Entities;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    static Player PLAYER;
    public UnityEvent OnSceneLoaded;

    Fading _fade;

    private void Awake()
    {
        _fade = FindFirstObjectByType<Fading>();
        if(!_fade) Debug.LogWarning("No fade found");
    }

    private void Start()
    {
        PLAYER = FindFirstObjectByType<Player>();
        if(PLAYER) OnSceneLoaded.AddListener(PLAYER.SetDashing);
    }

    public void LoadScene(int sceneIndex)
    {
        OnSceneLoaded.Invoke();
        if (!_fade)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
            return;
        }
        StartCoroutine(AfterFade(_fade.FadeIn(), sceneIndex));
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

    private IEnumerator AfterFade(IEnumerator func, int i)
    {
        if (_fade != null)
            yield return StartCoroutine(func);

        switch (i)
        {
            case 0:
            case 1:
                UnityEngine.SceneManagement.SceneManager.LoadScene(i);
                break;
            case 2:
                Application.Quit();
                break;
            default:
                break;
        }
    }
}
