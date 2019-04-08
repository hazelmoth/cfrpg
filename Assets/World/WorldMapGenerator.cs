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


	public static WorldMap Generate (int sizeX, int sizeY) {
		WorldMap map = new WorldMap ();
		map.mapDict = new Dictionary<string, Dictionary<Vector2Int, MapUnit>> ();
		map.mapDict.Add (WorldSceneName, new Dictionary<Vector2Int, MapUnit> ());

		// Loop through every tile defined by the size, fill it with grass and maybe add a plant
		for (int y = 0; y < sizeY; y++) {
			for (int x = 0; x < sizeX; x++) {
				Vector2Int currentPosition = new Vector2Int (x, y);
				MapUnit mapTile = new MapUnit();

				mapTile.groundMaterial = GroundMaterialLibrary.GetGroundMaterialById (GrassMaterialId);
				map.mapDict [WorldSceneName].Add (currentPosition, mapTile);
				// Decide whether to add a plant, and if so choose one randomly
				if (Random.Range (0f, 1f) < PlantFrequency) {
					map.mapDict[WorldSceneName][currentPosition].entityId = GetWeightedRandomString(plantBank);
				}
			}
		}
		return map;
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
