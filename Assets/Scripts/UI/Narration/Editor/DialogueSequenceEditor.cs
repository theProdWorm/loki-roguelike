using UI.Narration;
using UnityEditor;

namespace UI.Narration.Editor
{
    [CustomEditor(typeof(DialogueSequence))]
    public class DialogueSequenceEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var repeatable = serializedObject.FindProperty("Repeatable");
            var lines = serializedObject.FindProperty("Lines");
            var isVoiced = serializedObject.FindProperty("IsVoiced");
            
            EditorGUILayout.PropertyField(repeatable);
            EditorGUILayout.PropertyField(lines);
            EditorGUILayout.PropertyField(isVoiced);
            
            serializedObject.ApplyModifiedProperties();

            if (!isVoiced.boolValue)
                return;

            var voiceEvent = serializedObject.FindProperty("VoiceEvent");
            var voiceParameterName = serializedObject.FindProperty("VoiceParameterName");
            
            EditorGUILayout.PropertyField(voiceEvent);
            EditorGUILayout.PropertyField(voiceParameterName);
        }
    }
}