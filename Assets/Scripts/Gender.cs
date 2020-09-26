using UnityEngine;

public enum Gender { 
    Male,
    Female
}

public static class GenderHelper
{
    public static Gender GenderFromString (string gender)
    {
        if (gender.ToLower() == "female")
            return Gender.Female;
        else
            return Gender.Male;
    }
    public static Gender RandomGender ()
    {
        if (Random.Range(0, 2) == 0)
            return Gender.Female;
        else
            return Gender.Male;
    }
}