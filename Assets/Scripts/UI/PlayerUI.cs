using Entities;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerUI : MonoBehaviour
    {
        private static Player _player;

        [SerializeField] private Slider _healthBar;
        [SerializeField] private Slider _healthLagBar;
        
        [SerializeField] private Slider _potionBar1;
        [SerializeField] private Slider _potionBar2;
        
        [SerializeField] private Slider _switchCooldownBar;
        
        private int oldHealth = -1;

        private void OnEnable()
        {
            _player = FindAnyObjectByType<Player>();
            _player.OnHealthChanged.AddListener(UpdateHealthUI);
            _player.OnPotionChargesChanged.AddListener(UpdatePotionCharge);
        }

        private void OnDisable()
        {
            _player.OnHealthChanged.RemoveListener(UpdateHealthUI);
            _player.OnPotionChargesChanged.RemoveListener(UpdatePotionCharge);
        }

        // private void Update()
        // {
        //     float switchCooldownPercent = _player.GetSwitchCooldown();
        //     
        //     _switchCooldownBar.value = switchCooldownPercent;
        // }

        private void UpdateHealthUI(int currentHealth, int maxHealth)
        {
            if(oldHealth == -1) 
                oldHealth = currentHealth;
            
            _healthBar.maxValue = maxHealth;
            _healthBar.value = currentHealth;

            oldHealth = currentHealth;
        }

        private void UpdatePotionCharge(int currentCharges, int maxCharges)
        {
            _potionBar1.maxValue = maxCharges;
            _potionBar1.value = currentCharges;
        }
    }
}
