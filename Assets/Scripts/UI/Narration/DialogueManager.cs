using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace UI.Narration
{
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField] private PlayerInput _playerInput;
        
        [SerializeField] private GameObject _dialoguePanel;
        [SerializeField] private TextMeshProUGUI _dialogueTMP;
        [SerializeField] private TextMeshProUGUI _speakerTMP;
        [SerializeField] private TextMeshProUGUI _nextIndicator;
    
        public UnityEvent OnDialogueFinished;
        
        private int _dialoguePage;
        private DialogueSequence _dialogue;

        public static DialogueManager INSTANCE;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            if (INSTANCE)
            {
                Destroy(gameObject);
                return;
            }

            INSTANCE = this;
            _dialoguePanel.SetActive(false);
        }

        public static void StartDialogue(DialogueSequence dialogue)
        {
            INSTANCE._playerInput.SwitchCurrentActionMap("Dialogue");
            
            INSTANCE._dialogue = dialogue;
            INSTANCE._dialoguePage = 0;
            INSTANCE._dialoguePanel.SetActive(true);
            
            INSTANCE._nextIndicator.enabled = INSTANCE._dialogue.Lines.Length > 1;
            SetDialogue(dialogue.Lines[0]);
        }

        private static void EndDialogue()
        {
            INSTANCE._dialoguePanel.SetActive(false);
            INSTANCE._playerInput.SwitchCurrentActionMap("Player");
            
            INSTANCE.OnDialogueFinished.Invoke();
        }

        public static void AdvanceDialogue(InputAction.CallbackContext context)
        {
            if (!context.started)
                return;
            
            if (INSTANCE._dialoguePage < INSTANCE._dialogue.Lines.Length - 1)
            {
                INSTANCE._nextIndicator.enabled = INSTANCE._dialoguePage < INSTANCE._dialogue.Lines.Length - 2;
                
                var line = INSTANCE._dialogue.Lines[++INSTANCE._dialoguePage];
                SetDialogue(line);
            }
            else
                EndDialogue();
        }

        private static void SetDialogue(DialogueLine line)
        {
            string text = line.Text;
            string speaker = line.Speaker.ToString();
            
            INSTANCE._dialogueTMP.text = text;
            INSTANCE._speakerTMP.text = speaker;
        }
    }
}
