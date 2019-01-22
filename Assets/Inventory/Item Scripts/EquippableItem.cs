using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EquippableItem : Item {

	public abstract void Activate (Vector3Int tile);

	public abstract bool UseTileSelector { get; }
	public abstract float TileSelectorRange { get; }
}
