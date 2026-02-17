using System;
using TMPro;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    public PlayerInput playerInput;
    public GameObject diagloguePanel;
    public TextMeshProUGUI diaglogueText;
    public TextMeshProUGUI nextIndicator;
    
    private static GameObject dPanel;
    private static TextMeshProUGUI dText;
    private static TextMeshProUGUI nIndicator;
    private static PlayerInput _playerInput;
    
    private static bool inDialogue = false;

    private static int _dialoguePage;
    private static string[] _dialogue;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        _playerInput = playerInput;
        dPanel = diagloguePanel;
        dText = diaglogueText;
        nIndicator = nextIndicator;
        dPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
    }

    public static void StartDialogue(string[] dialogue)
    {
        _dialogue = dialogue;
        _playerInput.SwitchCurrentActionMap("Dialogue");
        _dialoguePage = 0;
        inDialogue = true;
        dPanel.SetActive(true);
        dText.text = dialogue[_dialoguePage];
        nIndicator.enabled = _dialoguePage + 1 != _dialogue.Length;
    }

    public static void EndDialogue()
    {
        dPanel.SetActive(false);
        inDialogue = false;
        _playerInput.SwitchCurrentActionMap("Player");
    }

    public static void AdvanceDialogue(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (_dialoguePage < _dialogue.Length-1)
            {
                if (_dialoguePage + 1 == _dialogue.Length - 1) nIndicator.enabled = false;
                dText.text = _dialogue[++_dialoguePage];
            }
            else
                EndDialogue();
        }
        
    }
}
