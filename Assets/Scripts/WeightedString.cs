using UnityEngine;

/// Just a string with an associated weight.
[System.Serializable]
public struct WeightedString
{
	public string value;
	public float frequencyWeight;
	public WeightedString(string value, float weight)
	{
		this.value = value;
		this.frequencyWeight = weight;
	}

	public static string GetWeightedRandom(WeightedString[] arr)
	{
		float weightSum = 0f;
		string result = null;
		foreach (WeightedString option in arr)
		{
			weightSum += option.frequencyWeight;
		}
		float throwValue = Random.Range(0f, weightSum);
		foreach (WeightedString option in arr)
		{
			if (throwValue < option.frequencyWeight)
			{
				result = option.value;
				break;
			}
			throwValue -= option.frequencyWeight;
		}
		return result;
	}
}
