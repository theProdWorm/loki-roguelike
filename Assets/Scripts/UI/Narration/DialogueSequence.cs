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
        
        private UnityEvent _onFinishedTemp;

        private void OnValidate()
        {
            OnFinished.RemoveAllListeners();
            OnFinished.AddListener(_onFinishedTemp.Invoke);
        }

        public void AddListener(Action action)
        {
            _onFinishedTemp.AddListener(action.Invoke);
        }
    }
}