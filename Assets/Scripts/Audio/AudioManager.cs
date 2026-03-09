using FMODUnity;
using UnityEngine;

namespace Audio
{
   public class AudioManager : MonoBehaviour
   {
      
      [SerializeField] private float _maxWaterDistance;
      
      private static AudioManager _instance;

      private Vector3 _nextAudioPosition;
   
      private void Awake()
      {
         if (_instance != null)
         {
            Debug.LogError("Found more than one AudioManager in the scene!");
            Destroy(gameObject);
            return;
         }
      
         _instance = this;
         DontDestroyOnLoad(gameObject);
      }

      public void SetNextAudioPosition(Vector3 position) => _nextAudioPosition = position;
      public void PlayOneShot(EventReference sound) => RuntimeManager.PlayOneShot(sound, _nextAudioPosition);
   }
}
