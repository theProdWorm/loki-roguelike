using System.Collections;
using UnityEngine;

namespace Animation
{
    public class Sinker : MonoBehaviour
    {
        [SerializeField] private float _sinkDistance = 1f;
        [SerializeField] private float _sinkDuration = 2f;
        
        public void Sink()
        {
            StartCoroutine(SinkRoutine());
        }

        private IEnumerator SinkRoutine()
        {
            float elapsedTime = 0;
            while (elapsedTime < _sinkDuration)
            {
                float sinkDistance = _sinkDistance * Time.deltaTime / _sinkDuration;
                transform.position -= new Vector3(0, sinkDistance, 0);
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }
}