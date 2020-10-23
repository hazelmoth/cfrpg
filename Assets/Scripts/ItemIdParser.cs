using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemIdParser
{
    private const char ModifierStart = ':';
    private const char ModifierEquals = '=';
    private const char ModifierSeperator = ';';

    public static string ParseBaseId(string id)
    {
        return (id.Split(ModifierStart)[0]);
    }

    // Returns a new dictionary containing the modifiers stored in the given item ID.
    public static IDictionary<string, string> ParseModifiers(string id)
    {
        Dictionary<string, string> modifiers = new Dictionary<string, string>();
        string[] split = id.Split(ModifierStart);
        if (split.Length > 1)
        {
            foreach (string modifier in split[1].Split(ModifierSeperator))
            {
                string[] parts = modifier.Split(ModifierEquals);
                modifiers.Add(parts[0].Trim(), parts[1].Trim());
            }
        }
        return modifiers;
    }

    // Returns a new item ID with the given modifier changed to the given value.
    public static string SetModifier(string id, string modifier, string value)
    {
        IDictionary<string, string> mods = ParseModifiers(id);
        string baseId = ParseBaseId(id);
        mods[modifier] = value;
        return WriteModifiers(baseId, mods);
    }

    private static string WriteModifiers(string baseId, IDictionary<string, string> modifiers)
    {
        string newId = baseId;
        newId += ModifierStart;
        foreach (string key in modifiers.Keys)
        {
            newId += key.Trim() + ModifierEquals + modifiers[key].Trim() + ModifierSeperator;
        }
        if (modifiers.Keys.Count > 0)
        {
            newId = newId.Substring(0, newId.Length - 1); // Trim the last seperator character
        }
        return newId;
    }
}
