using Items;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName="NewHoe", menuName = "Items/Hoe", order = 1)]
public class Hoe : ItemData, ITileSelectable, IAimable {

	[SerializeField] private string farmlandGroundMaterialId = "farmland";
	[SerializeField] private float range = 4;
	[SerializeField] private Sprite heldItemSprite;
	[SerializeField] private Direction spritePointDirection = Direction.Right;

	void ITileSelectable.Use(TileLocation target) 
	{
		Vector2Int tile = target.Position.ToVector2Int();

		GroundMaterial ground = WorldMapManager.GetGroundMaterialtAtPoint(target.Position.ToVector2Int(), target.Scene);
		GroundMaterial groundCover = WorldMapManager.GetGroundCoverAtPoint(target.Position.ToVector2Int(), target.Scene);

		if (ground == null) return;
		string entity = WorldMapManager.GetMapObjectAtPoint(tile, target.Scene).entityId;
		EntityData entityData = ContentLibrary.Instance.Entities.Get(entity);

		if (groundCover != null)
		{
			if (groundCover.isFarmland)
			{
				if (entityData != null) return; // Do nothing if an entity is covering this farmland.

				// If there's already farmland here, remove it.
				WorldMapManager.ChangeGroundMaterial(tile, target.Scene, TilemapLayer.GroundCover, null);
				return;
			}
			else return; // Do nothing if some other ground cover is already here.
		}
		else if (ground.isFarmable)
		{
			if (entityData != null)
			{
				if (entityData.canBeBuiltOver)
					WorldMapManager.RemoveEntityAtPoint(tile, target.Scene);
				else
					return;
			}

			WorldMapManager.ChangeGroundMaterial(tile, target.Scene, TilemapLayer.GroundCover, ContentLibrary.Instance.GroundMaterials.Get(farmlandGroundMaterialId));
		}
	}
	bool ITileSelectable.VisibleTileSelector => true;
	float ITileSelectable.TileSelectorRange => range;

	Sprite IAimable.heldItemSprite => heldItemSprite;

	Direction IAimable.pointDirection => spritePointDirection;
}
