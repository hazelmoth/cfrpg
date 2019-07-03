using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapGenerator : MonoBehaviour
{
	// TODO define plants and generation parameters in a seperate file or object
	const string WorldSceneName = SceneObjectManager.WorldSceneId;
	const float PlantFrequency = 0.2f;
	static readonly WeightedString[] plantBank = 
	{
		new WeightedString("plant_fern", 1.5f),
		new WeightedString("plant_tomato", 1f),
		new WeightedString("tree_deciduous", 0.7f),
		new WeightedString("plant_pineapple", 0.2f)
	};
	const string GrassMaterialId = "grass";
	const string SandMaterialId = "sand";
	const string WaterMaterialId = "water";

	const float noiseScale = 1.5f;
	const float sandLevel = 0.3f;
	const float waterLevel = 0.2f;

	public static WorldMap Generate (int sizeX, int sizeY)
	{
		float seed = Random.value * 1000;
		return Generate(sizeX, sizeY, seed);
	}
	public static WorldMap Generate (int sizeX, int sizeY, float seed) {
		WorldMap map = new WorldMap ();
		map.mapDict = new Dictionary<string, Dictionary<Vector2Int, MapUnit>> ();
		map.mapDict.Add (WorldSceneName, new Dictionary<Vector2Int, MapUnit> ());

		// Loop through every tile defined by the size, fill it with grass and maybe add a plant
		for (int y = 0; y < sizeY; y++) {
			for (int x = 0; x < sizeX; x++) {
				Vector2Int currentPosition = new Vector2Int (x, y);
				MapUnit mapTile = new MapUnit();

				// Start with a nice height gradient from center to edges
				float h = EllipseGradient(new Vector2(x - sizeX / 2, y - sizeY / 2), sizeX, sizeY);
				// Round off the height with a log function
				if (h > 0)
					h = Mathf.Log(h + 1, 2); 

				// Multiply a layer of noise so the map is more interesting
				h = h * Mathf.PerlinNoise((noiseScale / 10) * x + seed, (noiseScale / 10) * y + seed);

				// Assign ground material based on height
				bool canHavePlants = false;
				if (h > sandLevel)
				{
					mapTile.groundMaterial = GroundMaterialLibrary.GetGroundMaterialById(GrassMaterialId);
					canHavePlants = true;
				}
				else if (h > waterLevel)
					mapTile.groundMaterial = GroundMaterialLibrary.GetGroundMaterialById(SandMaterialId);
				else
					mapTile.groundMaterial = GroundMaterialLibrary.GetGroundMaterialById(WaterMaterialId);

				map.mapDict [WorldSceneName].Add (currentPosition, mapTile);

				// Decide whether to add a plant, and if so choose one randomly
				if (canHavePlants && Random.Range (0f, 1f) < PlantFrequency) {
					map.mapDict[WorldSceneName][currentPosition].entityId = GetWeightedRandomString(plantBank);
				}
			}
		}
		return map;
	}

	// Returns a value between 0 and 1 based on where a point is between the origin and a surrounding ellipse.
	// 1 is the center of the ellipse, 0 is the outside.
	static float EllipseGradient(Vector2 point, float width, float height)
	{
		// diameters to radii
		width = width / 2;
		height = height / 2;
		// Find point on ellipse that is on the line between the origin and input point
		float x = Mathf.Sqrt( Mathf.Pow(width * height * point.x, 2f) / (Mathf.Pow(point.x * height, 2) + Mathf.Pow(point.y * width, 2)) );
		float y = height * Mathf.Sqrt(1 - Mathf.Pow(x / width, 2));
		if (point.x < 0)
			x *= -1;
		if (point.y < 0)
			y *= -1;
		Vector2 ellipsePoint = new Vector2(x, y);

		float z = Vector2.Distance(Vector2.zero, point) / Vector2.Distance(Vector2.zero, ellipsePoint);
		z = Mathf.Clamp01(z);
		// Invert so 1 is the center
		z = 1 - z;
		return z;
	}
		
	// For randomly selecting plants
	struct WeightedString {
		public string value;
		public float frequencyWeight;
		public WeightedString (string id, float freqMult) {
			this.value = id;
			this.frequencyWeight = freqMult;
		}
	}
	static string GetWeightedRandomString (WeightedString[] arr) {
		float weightSum = 0f;
		string result = null;
		foreach (WeightedString option in arr) {
			weightSum += option.frequencyWeight;
		}
		float throwValue = Random.Range(0f, weightSum);
		foreach (WeightedString option in arr) {
			if (throwValue < option.frequencyWeight) {
				result = option.value;
				break;
			}
			throwValue -= option.frequencyWeight;
		}
		return result;
	}
}
