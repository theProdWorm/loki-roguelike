using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public void LoadScene(int sceneIndex) => 
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
    public void LoadSceneAdditive(int sceneIndex) => 
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex, LoadSceneMode.Additive);
    public void LoadSceneAdditiveAsync(int sceneIndex) =>
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
    
    public void UnloadSceneAsync(int sceneIndex) =>
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneIndex);
    
    public void QuitGame() => Application.Quit();
}
