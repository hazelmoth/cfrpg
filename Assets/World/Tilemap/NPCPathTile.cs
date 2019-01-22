using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class NPCPathTile : TileBase {

	[SerializeField] Sprite sprite;

	public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
	{
		Sprite[] neighbourSprites = null;
		GetNeighboringTileSprites(tilemap, position, ref neighbourSprites);
		var iden = Matrix4x4.identity;

		tileData.sprite = sprite;
		tileData.colliderType = Tile.ColliderType.None;
		tileData.flags = TileFlags.LockTransform;
		tileData.transform = iden;

		Matrix4x4 transform = iden;

		// Don't show this tile if the game is running
		if (Application.isPlaying) {
			tileData.sprite = null;
		}
	}

	public override void RefreshTile(Vector3Int location, ITilemap tileMap)
	{
		base.RefreshTile (location, tileMap);
	}

	private void RotateTile(float angle, ref TileData tileData) {
		tileData.transform = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -angle), Vector3.one);
	}

	private void GetNeighboringTileSprites(ITilemap tilemap, Vector3Int position, ref Sprite[] neighboringTileSprites)
	{
		if (neighboringTileSprites != null)
			return;

		Sprite[] cachedNeighboringTiles = new Sprite[8];

		int index = 0;
		for (int y = 1; y >= -1; y--)
		{
			for (int x = -1; x <= 1; x++)
			{
				if (x != 0 || y != 0)
				{
					Vector3Int tilePosition = new Vector3Int(position.x + x, position.y + y, position.z);
					cachedNeighboringTiles[index++] = tilemap.GetSprite(tilePosition);
				}
			}
		}
		neighboringTileSprites = cachedNeighboringTiles;
	}

}
