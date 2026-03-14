using Entities;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Animation
{
    [RequireComponent(typeof(Volume))]
    public class HealthVignette : MonoBehaviour
    {
        [SerializeField] private ParticleSystem.MinMaxCurve _healthIntensityCurve;
        [SerializeField] private float _minIntensity;
        [SerializeField] private float _maxIntensity;
        [SerializeField] private float _pulseSpeed;
        [SerializeField] private float _lowHealthPulseSpeed;
        
        private Volume _volume;
        private Vignette _vignette;

        private float _playerHealthPercent = 1f;

        private void Awake()
        {
            _volume = GetComponent<Volume>();
            _volume.profile.TryGet(out _vignette);
        }
        
        private void Start()
        {
            var player = FindFirstObjectByType<Player>();
            player.OnHealthChanged.AddListener((current, max) => _playerHealthPercent = current / (float) max);
        }
        
        private void Update()
        {
            float pulseT = (Mathf.Sin(_pulseSpeed * Time.realtimeSinceStartup * Mathf.PI * 2) + 1) / 2;
            float intensity = Mathf.Lerp(_minIntensity, _maxIntensity, _healthIntensityCurve.Evaluate(_playerHealthPercent) * pulseT);

            _vignette.intensity.value = intensity;
            print(intensity);
        }
    }
}