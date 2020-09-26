using Items;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName="NewHoe", menuName = "Items/Hoe", order = 1)]
public class Hoe : ItemData, ITileSelectable {

	[SerializeField] private TileBase farmlandTilePrefab = null;
	[SerializeField] private float range = 4;

	void ITileSelectable.Use(TileLocation target) {
		Vector2Int tile = target.Position.ToVector2Int();
		TilemapInterface.ChangeTile (tile.x, tile.y, farmlandTilePrefab, SceneObjectManager.WorldSceneId, TilemapLayer.GroundCover);
	}
	bool ITileSelectable.VisibleTileSelector => true;
	float ITileSelectable.TileSelectorRange => range;
}
