using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private void LateUpdate()
    {
        gameObject.transform.LookAt(Camera.main.transform);
    }
}
