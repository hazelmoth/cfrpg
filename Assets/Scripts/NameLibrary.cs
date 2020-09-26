using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;
using UnityEngine;

public class NameLibrary
{
	private const string LIBRARY_ASSET_PATH = "NameLibrary"; 
	private NameLibraryAsset loadedAsset;

	private const float NameDistributionPower = 3;

	private List<string> femaleFirstNames;
	private List<string> maleFirstNames;
	private List<string> lastNames;

	public void LoadLibrary()
	{
		loadedAsset = (NameLibraryAsset)(Resources.Load(LIBRARY_ASSET_PATH, typeof(ScriptableObject)));

		if (loadedAsset == null)
		{
			Debug.LogError("Library asset not found!");
		}

		femaleFirstNames = new List<string>();
		foreach (JSONNode node in JSON.Parse(loadedAsset.femaleFirstJson.text).AsArray)
		{
			femaleFirstNames.Add(node.Value);
		}
		maleFirstNames = new List<string>();
		foreach (JSONNode node in JSON.Parse(loadedAsset.maleFirstJson.text).AsArray)
		{
			maleFirstNames.Add(node.Value);
		}
		lastNames = new List<string>();
		foreach (JSONNode node in JSON.Parse(loadedAsset.lastJson.text).AsArray)
		{
			lastNames.Add(node.Value);
		}
	}

	public List<string> GetAllMaleFirst() => maleFirstNames;
	public List<string> GetAllFemaleFirst() => femaleFirstNames;
	public List<string> GetAllLast() => lastNames;

	public string GetRandomWeightedMaleFirstName()
	{
		return maleFirstNames[ExponentialRandomRange(maleFirstNames.Count, NameDistributionPower)];
	}
	public string GetRandomWeightedFemaleFirstName()
	{
		return femaleFirstNames[ExponentialRandomRange(femaleFirstNames.Count, NameDistributionPower)];
	}
	public string GetRandomLastName()
	{
		return lastNames.PickRandom();
	}

	// Returns a random integer in the given range, exponentially weighted towards zero
	private int ExponentialRandomRange(int exclusiveMax, float exp)
	{
		float sqrt = Random.Range(0, Mathf.Pow(exclusiveMax, 1/exp)); // Get a random value between 0 and the root of max
		int val = Mathf.FloorToInt(Mathf.Pow(sqrt, exp)); // Square our random square root and floor to int

		return Mathf.Clamp(val, 0, exclusiveMax - 1); // Clamp in case we happen to hit exactly the upper bound
	}
}
