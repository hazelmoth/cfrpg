using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//[Serializable]
[CreateAssetMenu]
public class CustomRuleTile : TileBase {

	[SerializeField] private Sprite defaultSprite;
	[SerializeField] private Sprite surroundingSprite;
	[SerializeField] private Sprite borderAbove, outsideCornerTopRight, insideCornerTopRight, horizontalStrip, onlyOnLeft, isolatedSpot;

	public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
	{
		Sprite[] sprites = null;
		GetNeighboringTileSprites(tilemap, position, ref sprites);
		var iden = Matrix4x4.identity;

		tileData.sprite = defaultSprite;
		tileData.colliderType = Tile.ColliderType.None;
		tileData.flags = TileFlags.LockTransform;
		tileData.transform = iden;

		Matrix4x4 transform = iden;

		bool top = (sprites [1] == surroundingSprite);
		bool left = (sprites [3] == surroundingSprite);
		bool right = (sprites [4] == surroundingSprite);
		bool bottom = (sprites [6] == surroundingSprite);
		bool topLeft = (sprites [0] == surroundingSprite);
		bool topRight = (sprites [2] == surroundingSprite);
		bool bottomLeft = (sprites [5] == surroundingSprite);
		bool bottomRight = (sprites [7] == surroundingSprite);

		if (top && !left && !right && !bottom) // top border
		{
			tileData.sprite = borderAbove;
		}
		else if (!top && !left && right && !bottom) // right border
		{
			tileData.sprite = borderAbove;
			RotateTile (90, ref tileData);
		}
		else if (!top && left && !right && !bottom) // left border
		{
			tileData.sprite = borderAbove;
			RotateTile (-90, ref tileData);
		}
		else if (!top && !left && !right && bottom) // bottom border
		{
			tileData.sprite = borderAbove;
			RotateTile (180, ref tileData);
		}
		else if (top && !left && right && !bottom) // top right corner
		{
			tileData.sprite = outsideCornerTopRight;
		}
		else if (top && left && !right && !bottom) // top left corner
		{
			tileData.sprite = outsideCornerTopRight;
			RotateTile (-90, ref tileData);
		}
		else if (!top && !left && right && bottom) // bottom right corner
		{
			tileData.sprite = outsideCornerTopRight;
			RotateTile (90, ref tileData);
		}
		else if (!top && left && !right && bottom) // bottom left corner
		{
			tileData.sprite = outsideCornerTopRight;
			RotateTile (180, ref tileData);
		}
		else if (!top && !left && !right && !bottom && topRight) // top right inside corner
		{
			tileData.sprite = insideCornerTopRight;
		}
		else if (!top && !left && !right && !bottom && topLeft) // top left inside corner
		{
			tileData.sprite = insideCornerTopRight;
			RotateTile (-90, ref tileData);
		}
		else if (!top && !left && !right && !bottom && bottomRight) // bottom right inside corner
		{
			tileData.sprite = insideCornerTopRight;
			RotateTile (90, ref tileData);
		}
		else if (!top && !left && !right && !bottom && bottomLeft) // bottom left inside corner
		{
			tileData.sprite = insideCornerTopRight;
			RotateTile (180, ref tileData);
		}
		else if (top && left && right && !bottom) // top end
		{
			tileData.sprite = onlyOnLeft;
			RotateTile (-90, ref tileData);
		}
		else if (top && !left && right && bottom) // right end
		{
			tileData.sprite = onlyOnLeft;
		}
		else if (top && left && !right && bottom) // left end
		{
			tileData.sprite = onlyOnLeft;
			RotateTile (180, ref tileData);
		}
		else if (!top && left && right && bottom) // bottom end
		{
			tileData.sprite = onlyOnLeft;
			RotateTile (90, ref tileData);
		}
		else if (!top && left && right && !bottom) // vertical strip
		{
			tileData.sprite = horizontalStrip;
			RotateTile (90, ref tileData);
		}
		else if (top && !left && !right && bottom) // horizontal strip
		{
			tileData.sprite = horizontalStrip;
		}
		else if (top && left && right && bottom) // isolated spot
		{
			tileData.sprite = isolatedSpot;
		}
	}

	public override void RefreshTile(Vector3Int location, ITilemap tileMap)
	{
		for (int y = -1; y <= 1; y++)
		{
			for (int x = -1; x <= 1; x++)
			{
				base.RefreshTile(location + new Vector3Int(x, y, 0), tileMap);
			}
		}
	}

	private void RotateTile(float angle, ref TileData tileData) {
		tileData.transform = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -angle), Vector3.one);
	}

	private void GetNeighboringTileSprites(ITilemap tilemap, Vector3Int position, ref Sprite[] neighboringTileSprites)
	{
		if (neighboringTileSprites != null)
			return;

		Sprite[] m_CachedNeighboringTiles = new Sprite[8];

		int index = 0;
		for (int y = 1; y >= -1; y--)
		{
			for (int x = -1; x <= 1; x++)
			{
				if (x != 0 || y != 0)
				{
					Vector3Int tilePosition = new Vector3Int(position.x + x, position.y + y, position.z);
					m_CachedNeighboringTiles[index++] = tilemap.GetSprite(tilePosition);
				}
			}
		}
		neighboringTileSprites = m_CachedNeighboringTiles;
	}

}
