using System.Collections.Generic;
using System.Collections.Immutable;

public static class Calendar
{
	public class Season
	{
		public Season(string name, int numDays)
		{
			this.Name = name;
			this.NumDays = numDays;
		}

		public string Name { get; }
		public int NumDays { get; }
	}

	public static ImmutableList<Season> Seasons { get; } =
		new List<Season>
		{
			new Season("Spring", 30),
			new Season("Summer", 30),
			new Season("Fall", 30),
			new Season("Winter", 30)
		}.ToImmutableList();

	public static int DaysInYear {
		get
		{
			int result = 0;
			foreach (Season m in Seasons)
			{
				result += m.NumDays;
			}
			return result;
		} 
	}

	public static int GetDayOfSeason (int dayOfYear)
	{
		int remaining = dayOfYear;
		while (true)
		{
			foreach (Season m in Seasons)
			{
				if (remaining > m.NumDays)
				{
					remaining -= m.NumDays;
				}
				else
				{
					return remaining;
				}
			}
		}
	}

	// not zero-indexed
	public static Season GetSeason(int dayOfYear)
	{
		int remaining = dayOfYear;
		while (true)
		{
			foreach (Season m in Seasons)
			{
				if (remaining > m.NumDays)
				{
					remaining -= m.NumDays;
				}
				else
				{
					return m;
				}
			}
		}
	}
	public static Season GetFollowingSeason (Season season)
	{
		int index = -1;
		for (int i = 0; i < Seasons.Count; i++)
		{
			if (Seasons[i].Equals(season))
				index = i;
		}
		index++;
		index %= Seasons.Count;
		return Seasons[index];
	}
}
