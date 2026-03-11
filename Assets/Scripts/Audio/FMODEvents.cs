using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace Audio
{
    public class FMODEvents : MonoBehaviour
    {
        [Header("Player SFX")]
        [SerializeField] private EventReference _playerDeath;
        [SerializeField] private EventReference _playerHit;
        [SerializeField] private EventReference _playerMovement;
        [SerializeField] private EventReference _playerDash;
    
        [SerializeField] private EventReference _playerAttack;
        [SerializeField] private EventReference _playerSwitchIn;

        [Header("Hel SFX")]
        [SerializeField] private EventReference _helProjectileTravel;
        [SerializeField] private EventReference _helProjectileHit;
    
        [Header("Enemy SFX")]
        [SerializeField] private EventReference _draugrDeath;
        [SerializeField] private EventReference _draugrHit;
        [SerializeField] private EventReference _draugrSwing;
        
        [Header("Misc")]
        [SerializeField] private EventReference _potionConsume;
        [SerializeField] private EventReference _runestoneInteract;
    
        [Header("UI SFX")]
        [SerializeField] private EventReference _uiButtonClick;
        [SerializeField] private EventReference _uiButtonHover;
        [SerializeField] private EventReference _gameStart;
    
        [Header("Music")]
        [SerializeField] private EventReference _ambienceMusic;
        [SerializeField] private EventReference _combatMusic;
        [SerializeField] private EventReference _menuMusic;

        private bool _isPlayerHel;
        private bool _isPlayerLowHealth;

        private Vector3 _nextPosition;
        private readonly List<EventInstance> _eventInstances = new();
        private Dictionary<string, EventInstance> _eventInstancesByName = new();
        
        
        private static FMODEvents _instance;

        private void Awake()
        {
            // if (_instance != null)
            // {
            //     Debug.LogError("Found more than one FMOD Events instance in the scene");
            //
            //     Destroy(gameObject);
            //     return;
            // }
            //
            // _instance = this;
            // DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            for (int i = _eventInstances.Count - 1; i >= 0; i--)
            {
                var instance = _eventInstances[i];
                if (!instance.isValid())
                {
                    _eventInstances.RemoveAt(i);
                    continue;
                }
                
                instance.setParameterByName("Player_Form", _isPlayerHel ? 1 : 0);
                instance.setParameterByName("Player_LowHealth", _isPlayerLowHealth ? 1 : 0);
            }
        }

        public static void SetCharacter(bool isPlayerHel) => _instance._isPlayerHel = isPlayerHel;
        public static void SetLowHealth(bool isLowHealth) => _instance._isPlayerLowHealth = isLowHealth;
        
        public void SetNextPosition(Transform reference) => _nextPosition = reference.position;
        public void SetNextPosition(Vector3 position) => _nextPosition = position;
        public void PlayEvent(string eventName)
        {
            var instance = RuntimeManager.CreateInstance(eventName);
            
            _eventInstances.Add(instance);
            _eventInstancesByName[eventName] = instance;
            
            instance.start();
        }

        public void StopEvent(string eventName)
        {
            var reference = _eventInstancesByName[eventName];
            
            if (reference.isValid())
                reference.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }
}
