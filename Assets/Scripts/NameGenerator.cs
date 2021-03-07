using ContentLibraries;

public static class NameGenerator
{
    public static string Generate (Gender gender)
    {
	    string last = ContentLibrary.Instance.Names.GetRandomLastName();
	    string first;

        if (gender == Gender.Male)
        {
	        first = ContentLibrary.Instance.Names.GetRandomWeightedMaleFirstName();
        }
        else
        {
	        first = ContentLibrary.Instance.Names.GetRandomWeightedFemaleFirstName();
        }

        return first + " " + last;
    }
}
