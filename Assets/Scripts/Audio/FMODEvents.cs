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
    
        [Header("Fenrir SFX")]
        [SerializeField] private EventReference _fenrirAttack;
        [SerializeField] private EventReference _fenrirSwitchIn;

        [Header("Hel SFX")]
        [SerializeField] private EventReference _helAttack;
        [SerializeField] private EventReference _helProjectileTravel;
        [SerializeField] private EventReference _helProjectileHit;
        [SerializeField] private EventReference _helSwitchIn;
    
        [Header("Enemy SFX")]
        [SerializeField] private EventReference _draugrDeath;
        [SerializeField] private EventReference _draugrHit;
        [SerializeField] private EventReference _draugrSwing;
    
        [Header("UI SFX")]
        [SerializeField] private EventReference _uiButtonClick;
        [SerializeField] private EventReference _uiButtonHover;
        [SerializeField] private EventReference _gameStart;
    
        [Header("Music")]
        [SerializeField] private EventReference _ambienceMusic;
        [SerializeField] private EventReference _combatMusic;
        [SerializeField] private EventReference _menuMusic;

        private static FMODEvents _instance;

        private void Awake()
        {
            if (_instance != null)
            {
                Debug.LogError("Found more than one FMOD Events instance in the scene");
            
                Destroy(gameObject);
                return;
            }
        
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
