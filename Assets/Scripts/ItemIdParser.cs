using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

public static class ItemIdParser
{
    private const char ModifierStart = ':';
    private const char ModifierEquals = '=';
    private const char ModifierSeperator = ';';

    public static string ParseBaseId(string id)
    {
        return (id.Split(ModifierStart)[0]);
    }

    /// Returns a new dictionary containing the modifiers stored in the given item ID.
    public static IDictionary<string, string> ParseModifiers(string id)
    {
        Dictionary<string, string> modifiers = new Dictionary<string, string>();
        string[] split = id.Split(ModifierStart);
        if (split.Length > 1)
        {
            foreach (string modifier in split[1].Split(ModifierSeperator))
            {
                string[] parts = modifier.Split(ModifierEquals);
                if (parts.Length != 2) continue;
                modifiers.Add(parts[0].Trim(), parts[1].Trim());
            }
        }
        return modifiers;
    }

    /// Returns a new item ID with the given modifier changed to the given value.
    [Pure]
    public static string SetModifier(string id, string modifier, string value)
    {
        IDictionary<string, string> mods = ParseModifiers(id);
        string baseId = ParseBaseId(id);
        mods[modifier] = value;
        return SetAllModifiers(baseId, mods);
    }

    /// Returns a new item ID with all the given modifiers added, in alphabetical order.
    [Pure]
    public static string SetAllModifiers(string baseId, IDictionary<string, string> modifiers)
    {
        if (modifiers.Count == 0) return baseId;

        string newId = baseId;
        newId += ModifierStart;

        List<string> modifierList = modifiers
            .Select(modifier => modifier.Key.Trim() + ModifierEquals + modifier.Value.Trim())
            .ToList();

        // Sort the modifiers alphabetically.
        modifierList.Sort();

        newId = modifierList.Aggregate(newId, (current, modifier) => current + (modifier + ModifierSeperator));

        // Trim the last separator character
        newId = newId[..^1];

        return newId;
    }
}
