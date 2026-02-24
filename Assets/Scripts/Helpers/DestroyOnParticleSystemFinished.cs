using UnityEngine;

namespace Helpers
{
    public class DestroyOnParticleSystemFinished : MonoBehaviour
    {
        private ParticleSystem _particleSystem;
    
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        // Update is called once per frame
        private void Update()
        {
            bool finished = _particleSystem.particleCount == 0;
            
            if (finished)
                Destroy(gameObject);
        }
    }
}
