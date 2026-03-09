using UnityEngine;

namespace UI.Narration
{
    [CreateAssetMenu(fileName = "Dialogue Sequence", menuName = "Dialogue/Dialogue Sequence")]
    public class DialogueSequence : ScriptableObject
    {
        [SerializeField] public bool Repeatable;
        [SerializeField] public DialogueLine[] Lines;
    }
}