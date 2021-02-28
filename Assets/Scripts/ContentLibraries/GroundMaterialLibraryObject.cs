using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GroundMaterialLibraryObject : ScriptableObject
{
	[SerializeField]
	public List<GroundMaterial> materials;

}
