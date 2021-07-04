using ContentLibraries;
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
		Vector2Int tile = target.Vector2.ToVector2Int();

		GroundMaterial ground = RegionMapManager.GetGroundMaterialAtPoint(target.Vector2.ToVector2Int(), target.scene);
		GroundMaterial groundCover = RegionMapManager.GetGroundCoverAtPoint(target.Vector2.ToVector2Int(), target.scene);

		if (ground == null) return;
		string entity = RegionMapManager.GetMapObjectAtPoint(tile, target.scene).entityId;
		EntityData entityData = ContentLibrary.Instance.Entities.Get(entity);

		if (groundCover != null)
		{
			if (groundCover.isFarmland)
			{
				if (entityData != null) return; // Do nothing if an entity is covering this farmland.

				// If there's already farmland here, remove it.
				RegionMapManager.ChangeGroundMaterial(tile, target.scene, TilemapLayer.GroundCover, null);
				return;
			}
			else return; // Do nothing if some other ground cover is already here.
		}
		else if (ground.isFarmable)
		{
			if (entityData != null)
			{
				if (entityData.canBeBuiltOver)
					RegionMapManager.RemoveEntityAtPoint(tile, target.scene);
				else
					return;
			}

			RegionMapManager.ChangeGroundMaterial(tile, target.scene, TilemapLayer.GroundCover, ContentLibrary.Instance.GroundMaterials.Get(farmlandGroundMaterialId));
		}
	}
	bool ITileSelectable.VisibleTileSelector => true;
	float ITileSelectable.TileSelectorRange => range;

	Sprite IAimable.heldItemSprite => heldItemSprite;

	Direction IAimable.pointDirection => spritePointDirection;
}
