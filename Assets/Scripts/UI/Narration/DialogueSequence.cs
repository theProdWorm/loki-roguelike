using System;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Narration
{
    [CreateAssetMenu(fileName = "Dialogue Sequence", menuName = "Dialogue/Dialogue Sequence")]
    public class DialogueSequence : ScriptableObject
    {
        [SerializeField] public bool Repeatable;
        [SerializeField] public DialogueLine[] Lines;

        [SerializeField] public UnityEvent OnFinished;
        
        private void OnValidate()
        {
            OnFinished.RemoveAllListeners();
        }
    }
}