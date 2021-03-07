using System.Collections.Generic;
using UnityEngine;

namespace ContentLibraries
{
	[CreateAssetMenu]
	public class GroundMaterialLibraryObject : ScriptableObject
	{
		[SerializeField]
		public List<GroundMaterial> materials;

	}
}
