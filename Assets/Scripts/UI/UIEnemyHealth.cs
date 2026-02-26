using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIEnemyHealth : MonoBehaviour
    {
        Slider _healthSlider;

        private void OnEnable()
        {
            _healthSlider = GetComponentInChildren<Slider>();
            UpdateHealth(5, 5);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                UpdateHealth(3, 5);
            }
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
            else
            {
                _healthSlider.gameObject.SetActive(true);
            }
        }
    }
}