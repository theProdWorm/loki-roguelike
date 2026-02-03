using UnityEngine;

namespace Abilities
{
    public class CharacterAbilitySet : ScriptableObject
    {
        public Ability Attack;
        public Ability Special;
        public Ability Switch;
        
        [Header("Unused")]
        public Ability Dash;
        public Ability Ultimate;
    }
}