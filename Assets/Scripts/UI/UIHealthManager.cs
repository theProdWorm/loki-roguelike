using UnityEngine;
using UnityEngine.UI;

namespace Entities.Player
{
    public class UIHealthManager : MonoBehaviour
    {
        static Player player;

        [SerializeField] private Slider _healthBar;

        private void OnEnable()
        {
            player = FindAnyObjectByType<Player>();
            player.OnHealthUpdate.AddListener(UpdateHealthUI);
        }

        private void OnDisable()
        {
            player.OnHealthUpdate.RemoveListener(UpdateHealthUI);
        }

        private void UpdateHealthUI(int _currentHealth, int _maxHealth)
        {
            _healthBar.value = (float)_currentHealth / _maxHealth;

        }
    }
}
