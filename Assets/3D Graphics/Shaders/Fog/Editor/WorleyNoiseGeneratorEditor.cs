using UnityEditor;
using UnityEngine;

namespace _3D_Graphics.Shaders.Fog.Editor
{
    [CustomEditor(typeof(WorleyNoiseGenerator))]
    public class WorleyNoiseGeneratorEditor : UnityEditor.Editor
    {
        private WorleyNoiseGenerator _worleyNoiseGenerator;

        public override void OnInspectorGUI()
        {
            _worleyNoiseGenerator = (WorleyNoiseGenerator) target;
        
            DrawDefaultInspector();
        
            if (GUILayout.Button("Generate"))
            {
                _worleyNoiseGenerator.Generate();
            }
        }
    }
}
