using System;
using UnityEngine;
using UnityEngine.Events;

public class SettingsMenu : MonoBehaviour
{

    //[SerializeField] private GameObject[] _panels;

    public static UnityEvent<int> OnmasterVolumeChanged = new();
    public static UnityEvent<int> OnsfxVolumeChanged = new();
    public static UnityEvent<int> OnmusicVolumeChanged = new();
    
    public static UnityEvent<float> OnrumbleChanged = new();
    public static UnityEvent<float> OnscreenShakeChanged = new();
    
    /*public void openTab(int index)
    {
        for (int i = 0; i < _panels.Length; i++)
            _panels[i].SetActive(i == index);
    }*/

    public static void MasterSlider(float value)
    {
        OnmasterVolumeChanged?.Invoke((int)value);
    }
    
    public static void SfxSlider(float value)
    {
        OnmasterVolumeChanged?.Invoke((int)value);
    }
    
    public static void MusicSlider(float value)
    {
        OnmasterVolumeChanged?.Invoke((int)value);
    }
    public static void RumbleSlider(float value)
    {
        OnrumbleChanged?.Invoke(value);
    }
    public static void ScreenShakeSlider(float value)
    {
        OnscreenShakeChanged?.Invoke(value);
    }
}
