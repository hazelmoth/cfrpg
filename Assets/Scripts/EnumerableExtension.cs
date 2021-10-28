using System;
using System.Collections.Generic;
using System.Linq;

public static class EnumerableExtension
{
    /// Returns a random element from the given sequence.
    /// If the sequence has no elements, throws an exception.
    public static T PickRandom<T>(this IEnumerable<T> source)
    {
        return source.PickRandom(1).Single();
    }

    public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count)
    {
        return source.Shuffle().Take(count);
    }
    
    public static T SeededPickRandom<T>(this IEnumerable<T> source, int seed)
    {
        return source.Shuffle().SeededPickRandom(1, seed).Single();
    }
    
    public static IEnumerable<T> SeededPickRandom<T>(this IEnumerable<T> source, int count, int seed)
    {
        return source.Shuffle().Take(count);
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        return source.OrderBy(x => Guid.NewGuid());
    }
    
    public static IEnumerable<T> SeededShuffle<T>(this IEnumerable<T> source, int seed)
    {
        Random rand = new Random(seed);
        return source.OrderBy(x => rand.Next());
    }
}
