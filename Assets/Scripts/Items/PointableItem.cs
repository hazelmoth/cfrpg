using UnityEngine;

namespace Items
{

	// Base class for items that can select tiles
	public abstract class PointableItem : ItemData
	{

		public abstract void Activate(Vector3Int tile);

		public abstract bool UseTileSelector { get; }
		public abstract float TileSelectorRange { get; }
	}
}
