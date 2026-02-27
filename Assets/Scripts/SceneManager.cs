using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public void LoadScene(int sceneIndex) => 
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
    public void LoadScene(string sceneName) =>
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    public void LoadSceneAdditive(int sceneIndex) => 
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex, LoadSceneMode.Additive);
    public void LoadSceneAdditive(string sceneName) =>
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    public void LoadSceneAdditiveAsync(int sceneIndex) =>
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
    public void LoadSceneAdditiveAsync(string sceneName) =>
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    
    public void UnloadSceneAsync(int sceneIndex) =>
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneIndex);
    public void UnloadSceneAsync(string sceneName) =>
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
    
    public void QuitGame() => Application.Quit();
}
