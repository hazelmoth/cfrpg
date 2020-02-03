using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class GroundMaterialLibraryObject : ScriptableObject
{
	[SerializeField]
	public List<GroundMaterial> materials;

}
