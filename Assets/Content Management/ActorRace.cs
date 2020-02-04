using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ActorRace : ScriptableObject
{
	[SerializeField] private string id;
	public string Id => id;

	[SerializeField] private string name;
	public string Name => name;

	[SerializeField] private List<Sprite> bodySprites;
	public List<Sprite> BodySprites => bodySprites;
}
