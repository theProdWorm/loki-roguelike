using UnityEngine;

public class SettingsMenu : MonoBehaviour
{

    [SerializeField] private GameObject[] _panels;
    
    public void openTab(int index)
    {
        for (int i = 0; i < _panels.Length; i++)
            _panels[i].SetActive(i == index);
    }
}
