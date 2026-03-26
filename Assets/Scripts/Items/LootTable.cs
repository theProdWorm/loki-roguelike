using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "LootTable", menuName = "Items/LootTable")]
    public class LootTable : ScriptableObject
    {
        public WeightedItem<BaseItemStats>[] items;
    }
}
