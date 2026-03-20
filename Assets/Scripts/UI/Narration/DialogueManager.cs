using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using FMOD.Studio;
using FMODUnity;
using Audio;

namespace UI.Narration
{
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField] private PlayerInput _playerInput;
        
        [SerializeField] private GameObject _dialoguePanel;
        [SerializeField] private TextMeshProUGUI _dialogueTMP;
        [SerializeField] private TextMeshProUGUI _speakerTMP;
        [SerializeField] private Image _speakerBackground;
        [SerializeField] private TextMeshProUGUI _nextIndicator;
	[SerializeField] private FMODEvents _fmodEvents;

        [SerializeField] private GameObject _hud;
    
        [SerializeField] private float _defaultLetterDelay;
        [SerializeField] private LetterDelay[] _customLetterDelays;

        private int _dialoguePage;
        private DialogueSequence _dialogue;

	private EventInstance _dialogueEventInstance;

        private Coroutine _slowWriteCoroutine;
        
        private DialogueLine CurrentLine => _dialogue.Lines[_dialoguePage];
        
        public static DialogueManager INSTANCE;
        
        private void Awake()
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
            
            if (INSTANCE._hud) 
                INSTANCE._hud.SetActive(false);
            
            INSTANCE._nextIndicator.enabled = INSTANCE._dialogue.Lines.Length > 1;
            INSTANCE._slowWriteCoroutine = INSTANCE.StartCoroutine(SlowWriteText(dialogue.Lines[0]));

            if (!INSTANCE._dialogue.IsVoiced)
                return;

	    INSTANCE._dialogueEventInstance = INSTANCE._fmodEvents.PlayEvent(INSTANCE._dialogue.VoiceEventName);
	    INSTANCE._dialogueEventInstance.setParameterByName(INSTANCE._dialogue.VoiceParameterName, INSTANCE._dialoguePage);
        }

        private static void EndDialogue()
        {
            INSTANCE._playerInput.SwitchCurrentActionMap("Player");
            
            INSTANCE._dialoguePanel.SetActive(false);
            
            if (INSTANCE._hud)
                INSTANCE._hud.SetActive(true);

            INSTANCE._dialogue.OnFinished?.Invoke();
        }

        public static void AdvanceDialogue(InputAction.CallbackContext context)
        {
            if (!context.started)
                return;
            
            if (INSTANCE._slowWriteCoroutine != null)
            { // Skip slow write
                INSTANCE.StopCoroutine(INSTANCE._slowWriteCoroutine);
                INSTANCE._slowWriteCoroutine = null;
                
                INSTANCE._dialogueTMP.text = INSTANCE.CurrentLine.Text;
            }
            else if (INSTANCE._dialoguePage < INSTANCE._dialogue.Lines.Length - 1)
            { // Advance to next line
                INSTANCE._nextIndicator.enabled = INSTANCE._dialoguePage < INSTANCE._dialogue.Lines.Length - 2;
                
                var line = INSTANCE._dialogue.Lines[++INSTANCE._dialoguePage];
                INSTANCE._slowWriteCoroutine = INSTANCE.StartCoroutine(SlowWriteText(line));

		INSTANCE._dialogueEventInstance.setParameterByName(INSTANCE._dialogue.VoiceParameterName, INSTANCE._dialoguePage);
            }
            else
                EndDialogue();
        }

        private static void SetSpeaker(DialogueSpeaker speaker)
        {
            var backgroundColor = speaker.BackgroundColor;
            var textColor = speaker.TextColor;
            backgroundColor.a = 0.5f;
            textColor.a = 1f;
            
            INSTANCE._speakerBackground.color = backgroundColor;
            INSTANCE._speakerTMP.color = textColor;
            INSTANCE._speakerTMP.text = speaker.name;
        }

        private static float GetLetterDelay(char targetChar)
        {
            foreach (var c in INSTANCE._customLetterDelays)
            {
                if (c.Letter == targetChar)
                    return c.Delay;
            }

            return INSTANCE._defaultLetterDelay;
        }
        
        private static IEnumerator SlowWriteText(DialogueLine line)
        {
            SetSpeaker(line.Speaker);
            
            string text = "";

            foreach (var c in line.Text)
            {
                float delay = GetLetterDelay(c);

                text += c;
                INSTANCE._dialogueTMP.text = text;
                yield return new WaitForSeconds(delay);
            }
            
            INSTANCE._slowWriteCoroutine = null;
        }
    }
}
