using System.Collections.Generic;

public static class Professions
{
    public static string TraderProfessionID => "trader";
    public static string BuilderProfessionID => "builder";

    public static string GetRandomSettlerProfession ()
    {
        return new List<string> { BuilderProfessionID }.PickRandom();
    }
}
