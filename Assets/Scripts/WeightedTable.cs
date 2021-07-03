using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/*
 * A WeightedTable represents a weighted collection of IDs, which can be picked
 * from at random, factoring in the weights.
 */
[System.Serializable]
public class WeightedTable
{
    public WeightedTable(IEnumerable<KeyValuePair<string, float>> entries)
    {
        this.entries = entries.Select(pair => new Entry {id = pair.Key, weight = pair.Value}).ToList();
    }
    
    [System.Serializable]
    public class Entry
    {
        public string id;
        public float weight = 1f;
    }

    [SerializeField]
    private List<Entry> entries;

    public List<Entry> Entries => entries.ToList();

    /**
     * Returns the ID that is at the given position in the table, taking weights
     * into account. Given value must be between 0 and 1.
     */
    public string Get(float proportion)
    {
        if (entries.Count == 0) throw new Exception("This table is empty!");
        if (proportion < 0 || proportion > 1) Debug.LogError("Given value must be between 0 and 1!");
        proportion = Mathf.Clamp(proportion, 0f, 0.9999f);
        float position = proportion * entries.Sum(entry => entry.weight);
        
        float currentPos = entries[0].weight;
        int currentIndex = 0;
        while (currentPos < position)
        {
            currentIndex++;
            currentPos += entries[currentIndex].weight;
        }
        return entries[currentIndex].id;
    }
    
    /**
     * Randomly picks the given number of IDs from this table, where entries with
     * greater weights are more likely to be chosen.
     * Uses UnityEngine.Random in its current state.
     */
    public List<string> PickRandom(int quantity)
    {
        List<string> result = new List<string>();
        for (int i = 0; i < quantity; i++)
        {
            result.Add(PickRandom());
        }
        return result;
    }
    
    /**
     * Returns a random ID from this table, where entries with greater weights
     * are more likely to be chosen.
     * Uses UnityEngine.Random in its current state.
     */
    public string PickRandom()
    {
        if (entries.Count == 0)
        {
            Debug.LogWarning("Tried to pick from empty spawn table!");
            return null;
        }
        List<Entry> shuffledEntries = Entries.Shuffle().ToList();
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