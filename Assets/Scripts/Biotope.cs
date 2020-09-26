using UnityEngine;

// a biotope describes a single, small area of uniform environmental conditions;
// e.g., a clearing, a cluster of trees, etc.

[CreateAssetMenu]
public class Biotope : ScriptableObject
{
	// the relative amount of land that should be dedicated to this biotope
	// compared to another typical biotope
	[SerializeField] public float biotopeFrequency;
	// The likelihood of any given tile in this biotype to recieve an entity
	[SerializeField] public float entityFrequency;
	// The entities that make up this biotype and their relative frequencies
	[SerializeField] public WeightedString[] entities;
}