using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Items
{
    public class LootUtils
    {
        public static T GetItem<T>(WeightedItem<T>[] items)
        {
            int totalWeight = 0;
            foreach (var item in items)
            {
                totalWeight += item.weight;
            }
        
            int sample = Random.Range(0, totalWeight);

            foreach (var item in items)
            {
                if (sample < item.weight)
                {
                    return item.item;
                }
                else sample -= item.weight;
            }
            return default;
        }

        public static T[] GetItems<T>(WeightedItem<T>[] items, int amount)
        {
            if(items.Length < amount)
            {
                return items.Select(x => x.item).ToArray();
            }
            List<WeightedItem<T>> pool = new List<WeightedItem<T>>(items);
            T[] result = new T[amount];

            for (int k = 0; k < amount; k++)
            {
                int totalWeight = pool.Sum(i => i.weight);
                int sample = Random.Range(0, totalWeight);

                foreach (var i in pool)
                {
                    if (sample < i.weight)
                    {
                        result[k] = i.item;
                        pool.Remove(i);
                        break;
                    }
                    sample -= i.weight;
                }
            }
            return result;
        }
    }

    [Serializable]
    public struct WeightedItem<T>
    {
        public T item;
        public int weight;
    }
}