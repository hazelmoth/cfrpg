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
        if (tables == null) return results;

        foreach (Entry entry in tables)
            for (int i = 0; i < entry.rolls; i++)
            {
                float randShot = Random.value;
                if (randShot < entry.probability) results.Add(entry.table.PickRandom());
            }

        return results;
    }

    /// Adds the given table and returns this object for chaining.
    public CompoundWeightedTable Add(int n, float p, WeightedTable table)
    {
        if (tables == null) tables = new List<Entry>();
        tables.Add(new Entry {table = table, rolls = n, probability = p});
        return this;
    }
}
