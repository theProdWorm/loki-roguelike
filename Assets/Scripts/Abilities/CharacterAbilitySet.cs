using UnityEngine;

namespace Abilities
{
    [CreateAssetMenu(fileName = "New Character Ability Set", menuName = "Abilities/Character Ability Set", order = 2)]
    public class CharacterAbilitySet : ScriptableObject
    {
        public Ability Attack;
        public Ability Special;
        
        public Ability Switch;
        public Ability Ultimate;
    }
}