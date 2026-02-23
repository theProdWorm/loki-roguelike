using UnityEngine;
using UnityEngine.UI;

namespace Entities.Player
{
    public class UIHealthManager : MonoBehaviour
    {
        private static Player _player;

        [SerializeField] private Slider _healthBar;

        private void OnEnable()
        {
            _player = FindAnyObjectByType<Player>();
            _player.OnHealthUpdate.AddListener(UpdateHealthUI);
        }

        private void OnDisable()
        {
            _player.OnHealthUpdate.RemoveListener(UpdateHealthUI);
        }

        private void UpdateHealthUI(int currentHealth, int maxHealth)
        {
            _healthBar.maxValue = maxHealth;
            _healthBar.value = currentHealth;
        }
    }
}
