using UnityEngine;

namespace Gameplay
{
    public class TutorialManager : MonoBehaviour
    {
        private static TutorialManager _instance;

        public static bool SwitchUnlocked { get; private set; }
        
        private void Awake()
        {
            if (!_instance)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public void UnlockSwitch() => SwitchUnlocked = true;
    }
}
