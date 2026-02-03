using UnityEngine;

namespace Abilities
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "Abilities/Ability", order = 0)]
    public class Ability : ScriptableObject
    {
        public float Cooldown;
        public GameObject Prefab;
    }
}