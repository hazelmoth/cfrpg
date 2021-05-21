using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

/**
 * A CompoundWeightedTable is a collection of WeightedTables, with the probability
 * distribution for # of results from each table configurable individually, for
 * the purpose of allowing more complex rules for choosing multiple items (for
 * example, have a certain item always included exactly once).
 */
[Serializable]
public class CompoundWeightedTable
{
    [Serializable]
    public class Entry
    {
        public int rolls;
        public float probability = 0.5f;
        public WeightedTable table;
    }
    
    public List<Entry> tables;

    public List<string> Pick()
    {
        List<string> results = new List<string>();
        
        foreach (Entry entry in tables)
            for (int i = 0; i < entry.rolls; i++)
            {
                float randShot = Random.value;
                if (randShot < entry.probability) results.Add(entry.table.Pick());
            }

        return results;
    }
}
