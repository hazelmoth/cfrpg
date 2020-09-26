using UnityEngine.Tilemaps;

[System.Serializable]
public class GroundMaterial
{
	public GroundMaterial (string name, bool isWater, TileBase tileAsset) {
		this.materialId = name;
		this.isWater = isWater;
		this.tileAsset = tileAsset;
	}
	public string materialId = "new";
	public bool isWater = false;
	public bool isFarmable = false; // Whether this material can be turned over into farmland.
	public bool isFarmland = false; // Whether this material itself is farmland.
	public bool isDiggable = true;
	public TileBase tileAsset;
}
