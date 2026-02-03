using Stats;
using UnityEngine;

namespace Abilities.Attacks
{
    public class Attack : MonoBehaviour
    {
        private AttackStats _stats;
        
        public void SetStats(AttackStats stats) => _stats = stats;
    }
}