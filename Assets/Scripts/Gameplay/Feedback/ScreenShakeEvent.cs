using UnityEngine;

namespace Gameplay.Feedback
{
    [CreateAssetMenu(fileName = "New Screen Shake Event", menuName = "Gameplay/Screen Shake Event")]
    public class ScreenShakeEvent : ScriptableObject
    {
        public float Duration;
        public float IntensityMultiplier = 1f;
        public ParticleSystem.MinMaxCurve IntensityCurve;
    }
}