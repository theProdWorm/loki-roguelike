using UnityEngine;

namespace Helpers
{
    public class DestroyOnParticleSystemFinished : MonoBehaviour
    {
        private ParticleSystem _particleSystem;

        private float _elapsedTime;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        // Update is called once per frame
        private void Update()
        {
            _elapsedTime += Time.deltaTime;
            
            if (_elapsedTime < _particleSystem.main.startLifetime.constantMax)
                return;
            
            bool finished = _particleSystem.particleCount == 0;
            
            if (finished)
                Destroy(gameObject);
        }
    }
}
