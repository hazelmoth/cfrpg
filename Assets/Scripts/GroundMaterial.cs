using UnityEngine.Tilemaps;

[System.Serializable]
public class GroundMaterial
{
	public GroundMaterial (string name, bool isWater, TileBase tileAsset) {
		this.materialId = name;
		this.isWater = isWater;
		this.tileAsset = tileAsset;
	}
	public string materialId = "New Material";
	public bool isWater = false;
	public bool isFarmable = false;
	public bool isDiggable = true;
	public TileBase tileAsset;
}
