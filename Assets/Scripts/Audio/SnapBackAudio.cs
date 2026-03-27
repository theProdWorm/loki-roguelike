using UnityEngine;

namespace Audio
{
    public class SnapBackAudio : MonoBehaviour
    {
        private void Start()
        {
            FMODEvents.INSTANCE.SetMasterVolume(SettingsMenu.INSTANCE.MasterVolumeSlider.value);
        }
    }
}