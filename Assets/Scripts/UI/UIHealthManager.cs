using UnityEngine;
using UnityEngine.UI;

namespace Entities.Player
{
    public class UIHealthManager : MonoBehaviour
    {
        private static Player _player;

        [SerializeField] private Slider _healthBar;
        [SerializeField] private Slider _gobletBar;
        private int gobletCharge;
        

        private int oldHealth = -1;

        private void OnEnable()
        {
            _player = FindAnyObjectByType<Player>();
            _player.OnHealthUpdate.AddListener(UpdateHealthUI);
            _player.OnDamageDealt.AddListener(UpdateGobletCharge);
            _gobletBar.maxValue = _player._gobletCost;
        }

        private void OnDisable()
        {
            _player.OnHealthUpdate.RemoveListener(UpdateHealthUI);
            _player.OnDamageDealt.RemoveListener(UpdateGobletCharge);
        }

        private void UpdateHealthUI(int currentHealth, int maxHealth)
        {
            if(oldHealth == -1) oldHealth = currentHealth;
            _healthBar.maxValue = maxHealth;
            _healthBar.value = currentHealth;

            if (currentHealth > oldHealth)
            {
                gobletCharge = 0;
                _gobletBar.value = gobletCharge;
            }
            oldHealth = currentHealth;
        }

        private void UpdateGobletCharge(Entity _)
        {
            if (_player.GobletReady) return;
            gobletCharge++;
            _gobletBar.value = gobletCharge;

            if (gobletCharge >= _gobletBar.maxValue)
            {
                _player.GobletReady = true;
            }
        }
    }
}
