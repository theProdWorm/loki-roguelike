using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents2 : MonoBehaviour
{
    //För kategorisering av rubriker och underrubriker i Fmodevent references
    [Header("Player SFX")]
    [SerializeField] private EventReference _playerDeath;
    [SerializeField] private EventReference _playerHit;
    [SerializeField] private EventReference _playerMovement;
    [SerializeField] private EventReference _playerDash;

    [Header("Music")]
    [SerializeField] private EventReference _ambienceMusic;
    [SerializeField] private EventReference _combatMusic;
    [SerializeField] private EventReference _menuMusic;

    [field: Header("Ambience Sounds")]
    [field: SerializeField] public EventReference skog {  get; private set; }

    public static FMODEvents2 instance {  get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one FMODEvents2 instance in the scene");
        }
        instance = this;
    }
}
