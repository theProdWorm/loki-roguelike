using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay
{
    public class RumbleManager : MonoBehaviour
    {
        public static bool PLAYER_MOVING_IN_WATER;
        public static RumbleManager INSTANCE;

        [SerializeField] private float _waterLowFrequency;
        [SerializeField] private float _waterHighFrequency;
        
        private Coroutine _rumbleCoroutine;
        
        private void Awake()
        {
            if (INSTANCE)
                Destroy(gameObject);
            else
                INSTANCE = this;
        }

        private void Update()
        {
            if (Gamepad.current == null || _rumbleCoroutine != null) 
                return;
            
            if (PLAYER_MOVING_IN_WATER)
                Gamepad.current?.SetMotorSpeeds(_waterLowFrequency, _waterHighFrequency);
            else
                Gamepad.current?.SetMotorSpeeds(0, 0);
        }
        
        public void StopRumble()
        {
            StopAllCoroutines();
            Gamepad.current?.SetMotorSpeeds(0, 0);
        }

        public void Rumble(RumbleEvent rumbleEvent)
        {
            if (Gamepad.current == null)
                return;
            
            if (_rumbleCoroutine != null)
                StopCoroutine(_rumbleCoroutine);
            
            _rumbleCoroutine = StartCoroutine(RumbleCoroutine(rumbleEvent));
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
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            Gamepad.current.SetMotorSpeeds(0, 0);
        }

        private void OnDestroy()
        {
            StopRumble();
        }
    }
}
