using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITileSelectable
{
	float TileSelectorRange { get; }

	bool VisibleTileSelector { get; }

	void Use(TileLocation target);
}
