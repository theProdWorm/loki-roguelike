using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay
{
    public class RumbleManager : MonoBehaviour
    {
        private static RumbleManager _instance;

        private void Awake()
        {
            if (_instance)
                Destroy(gameObject);
            else
                _instance = this;
        }

        public void Rumble(RumbleEvent rumbleEvent)
        {
            if (Gamepad.current == null)
                return;
            
            StartCoroutine(RumbleCoroutine(rumbleEvent));
        }

        private IEnumerator RumbleCoroutine(RumbleEvent rumbleEvent)
        {
            float elapsedTime = 0;

            while (elapsedTime < rumbleEvent.Duration)
            {
                float t = Mathf.Clamp01(elapsedTime / rumbleEvent.Duration);
                
                float low = rumbleEvent.LowFrequency.Evaluate(t);
                float high = rumbleEvent.HighFrequency.Evaluate(t);
                
                Gamepad.current.SetMotorSpeeds(low, high);
                
                Debug.Log($"Rumble: ({low}, {high})");
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            Gamepad.current.SetMotorSpeeds(0, 0);
        }
    }
}
