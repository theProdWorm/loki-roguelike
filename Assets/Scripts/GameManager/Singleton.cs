using UnityEngine;

public class Singleton : MonoBehaviour
{
    void Awake()
    {
        Singleton[] instances = FindObjectsByType<Singleton>(FindObjectsSortMode.None);
        if (instances.Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
