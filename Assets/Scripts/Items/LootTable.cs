using System.Collections.Generic;
using Items;
using UnityEngine;

[CreateAssetMenu(fileName = "LootTable", menuName = "Items/LootTable")]
public class LootTable : ScriptableObject
{
    public WeightedItem<BaseItemStats>[] items;
}
