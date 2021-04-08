using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/*
 * A SpawnTable represents a weighted collection of IDs, which can be picked
 * from at random, factoring in the weights.
 */
[System.Serializable]
public class WeightedTable
{
    [System.Serializable]
    public class Entry
    {
        public string id;
        public float weight;
    }

    public List<Entry> entries;

    public string Pick()
    {
        if (entries.Count == 0)
        {
            Debug.LogWarning("Tried to pick from empty spawn table!");
            return null;
        }
        List<Entry> shuffledEntries = entries.Shuffle().ToList();
        Debug.Assert(entries.Count == shuffledEntries.Count);
        float weightSum = (from entry in entries select entry.weight).Sum();
        float target = weightSum * Random.value - 0.00001f;
        
        float currentSum = 0;
        int i = 0;
        currentSum += shuffledEntries[0].weight;
        while (currentSum < target)
        {
            i++;
            currentSum += shuffledEntries[i].weight;
        }

        return shuffledEntries[i].id;
    }
}