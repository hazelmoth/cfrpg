using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JobLibrary
{
	// maps job Ids to names
	private static Dictionary<string, string> jobDict;
	private static bool hasInited;

	public static Dictionary<string, string> GetJobs ()
	{
		if (!hasInited)
		{
			Init();
		}
		return jobDict;
	}

	private static void Init()
	{
		jobDict = new Dictionary<string, string>();
		jobDict.Add("builder", "Builder");
		jobDict.Add("farmer", "Farmer");
		jobDict.Add("guard", "Guard");
	}
}
