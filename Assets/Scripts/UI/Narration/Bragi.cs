using System.Collections.Generic;
using GameManager;
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
            if (ProgressPersistence.SecondBranchDone)
                _dialogueQueue = _afterSecondBranch;
            else if (ProgressPersistence.FirstBranchDone)
                _dialogueQueue = _afterFirstBranch;
            else
                _dialogueQueue = _firstMeeting;
        }
    }
}