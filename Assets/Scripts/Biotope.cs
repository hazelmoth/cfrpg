using ContentLibraries;
using UnityEngine;

// a biotope describes a single, small area of uniform environmental conditions;
// e.g., a clearing, a cluster of trees, etc.

[CreateAssetMenu]
public class Biotope : ScriptableObject, IContentItem
{
	[SerializeField] public string id;
	// The likelihood of any given tile in this biotope to receive an entity
	[SerializeField] public float entityFrequency;
	// The entities that make up this biotope and their relative frequencies
	[SerializeField] public WeightedString[] entities;
	
	public string Id => id;
}