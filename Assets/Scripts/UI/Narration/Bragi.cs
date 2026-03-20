using System.Collections.Generic;
using UnityEngine;

namespace UI.Narration
{
    public class Bragi : DialogueObject
    {
        [SerializeField] private List<DialogueSequence> _firstMeeting;
        [SerializeField] private List<DialogueSequence> _afterFirstBranch;
        [SerializeField] private List<DialogueSequence> _afterSecondBranch;
        
        private void Start()
        {
            var progressPersistence = FindFirstObjectByType<ProgressPersistence>();
            
            if (progressPersistence.BragerDialogue2)
                _dialogueQueue = _afterSecondBranch;
            else if (progressPersistence.BragerDialogue1)
                _dialogueQueue = _afterFirstBranch;
            else
                _dialogueQueue = _firstMeeting;
        }
    }
}