using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIEnemyHealth : MonoBehaviour
    {
        private static List<UIEnemyHealth> BARS = new List<UIEnemyHealth>();
        private static bool ALLENABLED = true;
        
        Slider _healthSlider;

        private int _storedMax = 1;
        private int _storedCurrent = 1;
        
        private bool _enabled = true;

        public static void SlidersEnabled(bool value)
        {
            foreach (var bar in BARS)
            {
                bar.SetSlider(value);
            }
            ALLENABLED = value;
        }

        private void SetSlider(bool value)
        {
            _healthSlider.gameObject.SetActive(value);
            _enabled = value;
        }

        private void OnEnable()
        {
            _healthSlider = GetComponentInChildren<Slider>();
            BARS.Add(this);
            UpdateHealth(_storedCurrent, _storedMax);
            if (!ALLENABLED)
            {
                SetSlider(false);
            }
        }

        private void OnDisable()
        {
            _storedMax = (int)_healthSlider.maxValue;
            _storedCurrent = (int)_healthSlider.value;
            BARS.Remove(this);
        }

        public void UpdateHealth(int _currentHealth, int _maxHealth)
        {
            //TODO: Consider adding an effect when hit
            if (_healthSlider == null)
                return;
            
            //_healthSlider = GetComponentInChildren<Slider>();

            _healthSlider.maxValue = _maxHealth;
            _healthSlider.value = _currentHealth;

            //TODO: Consider adding a fade out effect instead of just deactivating the game object when health is full. This would make it look smoother and more polished.
            if (_currentHealth >= _maxHealth || _currentHealth <= 0)
            {
                _healthSlider.gameObject.SetActive(false);
            }
            else if(_enabled)
            {
                _healthSlider.gameObject.SetActive(true);
            }
        }
    }
}