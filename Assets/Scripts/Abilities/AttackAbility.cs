using UnityEngine;

namespace Abilities
{
    [CreateAssetMenu(fileName = "New Attack Ability", menuName = "Abilities/Attack Ability", order = 1)]
    public class AttackAbility : Ability
    {
        public GameObject Prefab;
    }
}