using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class AutoSelect : MonoBehaviour
    {
    
        private void OnEnable()
        {
            MenuManager.PreviousSelected = EventSystem.current.currentSelectedGameObject;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }
}
