using Entities;
using Entities.Player;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIHealthManager : MonoBehaviour
    {
        private static Player _player;

        [SerializeField] private Slider _healthBar;
        [SerializeField] private Slider _potionBar;

        private int oldHealth = -1;

        private void OnEnable()
        {
            _player = FindAnyObjectByType<Player>();
            _player.OnHealthChanged.AddListener(UpdateHealthUI);
            _player.OnPotionChargesChanged.AddListener(UpdateGobletCharge);
        }

        private void OnDisable()
        {
            _player.OnHealthChanged.RemoveListener(UpdateHealthUI);
            _player.OnPotionChargesChanged.RemoveListener(UpdateGobletCharge);
        }

        private void UpdateHealthUI(int currentHealth, int maxHealth)
        {
            if(oldHealth == -1) 
                oldHealth = currentHealth;
            
            _healthBar.maxValue = maxHealth;
            _healthBar.value = currentHealth;

            oldHealth = currentHealth;
        }

        private void UpdateGobletCharge(int currentCharges, int maxCharges)
        {
            _potionBar.maxValue = maxCharges;
            _potionBar.value = currentCharges;
        }
    }
}
