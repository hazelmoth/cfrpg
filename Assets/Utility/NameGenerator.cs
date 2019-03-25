using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NameGenerator
{
    public static string Generate (Gender gender)
    {
        if (gender == Gender.Male)
        {
            return "Harold";
        }
        else
        {
            return "Melinda";
        }
    }
}
