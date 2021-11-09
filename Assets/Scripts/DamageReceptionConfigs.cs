using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

/// A collection of per-type damage multiplier configs.
public enum DamageReceptionConfig
{
    [InspectorName("Default (take all damage)")]
    Default,
    Tree,
    GrassyPlant,
}

public static class DamageReceptionConfigsHelper
{
    // Default receives all damage
    public static readonly Dictionary<ImpactInfo.DamageType, float> Default = new()
    {
        { ImpactInfo.DamageType.Chop, 1f },
        { ImpactInfo.DamageType.Blunt, 1f },
        { ImpactInfo.DamageType.Slash, 1f },
        { ImpactInfo.DamageType.Stab, 1f },
        { ImpactInfo.DamageType.Gunshot, 1f },
        { ImpactInfo.DamageType.Punch, 1f },
    };

    public static readonly Dictionary<ImpactInfo.DamageType, float> Tree = new()
    {
        { ImpactInfo.DamageType.Chop, 1f },
        { ImpactInfo.DamageType.Slash, 0.2f },
        { ImpactInfo.DamageType.Stab, 0.04f },
        { ImpactInfo.DamageType.Gunshot, 0.04f },
    };

    // Grass only responds to slash damage
    public static readonly Dictionary<ImpactInfo.DamageType, float> GrassyPlant = new()
    {
        { ImpactInfo.DamageType.Slash, 1f },
    };


    /// Returns the list of multipliers for this configuration.
    public static List<BreakableObject.DamageMultiplier> GetMultipliers(this DamageReceptionConfig config)
    {
        Dictionary<ImpactInfo.DamageType, float> result = config switch
        {
            DamageReceptionConfig.Tree => Tree,
            DamageReceptionConfig.GrassyPlant => GrassyPlant,
            _ => Default,
        };
        return MultListFromDictionary(result);
    }

    private static List<BreakableObject.DamageMultiplier> MultListFromDictionary(Dictionary<ImpactInfo.DamageType, float> dict)
    {
        List<BreakableObject.DamageMultiplier> list = new();
        foreach ((ImpactInfo.DamageType damageType, float multiplier) in dict)
        {
            list.Add(new BreakableObject.DamageMultiplier(){damageType = damageType, damageMultiplier = multiplier});
        }
        return list;
    }
}
