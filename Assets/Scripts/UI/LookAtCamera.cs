using UnityEngine;

namespace UI
{
    public class LookAtCamera : MonoBehaviour
    {
        private void LateUpdate()
        {
            gameObject.transform.LookAt(Camera.main.transform);
        }
    }
}
