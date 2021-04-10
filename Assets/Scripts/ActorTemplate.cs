using System.Collections.Generic;
using ContentLibraries;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu]
public class ActorTemplate : ScriptableObject, IContentItem
{
	public string templateId;
	public float femaleChance = 0.5f;
	public List<string> races;
	public List<string> hairs;
	public float hatChance = 0.5f;
	public List<string> hats;
	public List<string> shirts;
	public List<string> pants;
	public List<string> personalities;
	public List<string> professions;

	public string Id => templateId;
}
