using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName="NewHoe", menuName = "Items/Hoe", order = 1)]
public class Hoe : PointableItem {

	[SerializeField] TileBase farmlandTilePrefab;
	[SerializeField] float range;

	public override void Activate (Vector3Int tile) {
		TilemapInterface.ChangeTile (tile.x, tile.y, farmlandTilePrefab);
	}
	public override bool UseTileSelector => true;
	public override float TileSelectorRange => range;
}
