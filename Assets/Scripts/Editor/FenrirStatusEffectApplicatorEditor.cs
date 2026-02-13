using StatusEffects.Applicators;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(FenrirStatusEffectApplicator))]
    public class FenrirStatusEffectApplicatorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var stacksToApply = serializedObject.FindProperty("_stacksToApply");

            var woundDuration = serializedObject.FindProperty("_duration");
            var woundStackable = serializedObject.FindProperty("_stackable");
            var woundRefresh = serializedObject.FindProperty("_refresh");
            
            var maxWounds = serializedObject.FindProperty("_maxWounds");
            
            var vulnerableDuration = serializedObject.FindProperty("_vulnerableDuration");
            var vulnerableStackable = serializedObject.FindProperty("_vulnerableStackable");
            var vulnerableRefresh = serializedObject.FindProperty("_vulnerableRefresh");
            var vulnerableDamageIncrease = serializedObject.FindProperty("_vulnerableDamageIncrease");
            
            // Wounds
            EditorGUILayout.LabelField("Wounds", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(maxWounds,
                new GUIContent("Max Wounds", "Amount of Wounds required to apply Vulnerable"));
            
            EditorGUILayout.PropertyField(stacksToApply, new GUIContent("Stacks to Apply"));
            EditorGUILayout.PropertyField(woundDuration, new GUIContent("Duration"));
            EditorGUILayout.PropertyField(woundStackable, new GUIContent("Stackable"));
            EditorGUILayout.PropertyField(woundRefresh, new GUIContent("Refresh"));
            
            EditorGUILayout.Space();
            
            // Vulnerable
            EditorGUILayout.LabelField("Vulnerable", EditorStyles.boldLabel);
            
            EditorGUILayout.PropertyField(vulnerableDuration, new GUIContent("Duration"));
            EditorGUILayout.PropertyField(vulnerableStackable, new GUIContent("Stackable"));
            EditorGUILayout.PropertyField(vulnerableRefresh, new GUIContent("Refresh"));
            EditorGUILayout.PropertyField(vulnerableDamageIncrease, new GUIContent("Damage Increase"));
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}