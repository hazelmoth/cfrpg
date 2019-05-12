using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "data", menuName = "Personality Type Asset")]
public class PersonalityData : ScriptableObject
{
	[SerializeField] string personalityId;
	[SerializeField] float frequency = 1f;
	public string PersonalityId => personalityId;
	public float Frequency => frequency;
}
