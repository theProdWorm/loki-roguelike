using UnityEngine;

namespace Abilities
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "Abilities/Ability", order = 0)]
    public class Ability : ScriptableObject
    {
        public int Charges;
        public float RechargeTime;
        public GameObject Prefab;
    }
}