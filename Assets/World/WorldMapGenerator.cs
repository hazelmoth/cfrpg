using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapGenerator : MonoBehaviour
{
    // TODO define plants and generation parameters in a seperate file or object
    public delegate void WorldFinishedEvent(WorldMap world);

	private const string WorldSceneName = SceneObjectManager.WorldSceneId;
	private const float PlantFrequency = 0.2f;
	private const bool UseLinearGradient = true;
	private const bool AllDesert = true;

	private static readonly WeightedString[] plantBank = 
	{
		new WeightedString("tree_deciduous", 0.5f),
		new WeightedString("tree_western_hemlock", 0.7f),
		new WeightedString("plant_fern", 1.5f),
		new WeightedString("plant_tomato", 1f),
		new WeightedString("plant_pineapple", 0.2f)
	};
	private const string GrassMaterialId = "grass";
	private const string SandMaterialId = "sand";
	private const string WaterMaterialId = "water";

	// higher frequency is grainier
	private const float noiseFrequencyLayer1 = 0.15f;
	private const float noiseFrequencyLayer2 = 1f;
	private const float noiseFrequencyLayer3 = 1.5f;
	private const float noiseFrequencyLayer4 = 2.5f;
	// how much each level affects the terrain
	private const float noiseDepthLayer1 = 1.0f;
	private const float noiseDepthLayer2 = 0.4f;
	private const float noiseDepthLayer3 = 0.2f;
	private const float noiseDepthLayer4 = 0.2f;
	private const float sandLevel = 0.175f;
	private const float waterLevel = 0.16f;

	private const float biotopeNoiseFreq = 0.7f;


	public static void StartGeneration (int sizeX, int sizeY, float seed, WorldFinishedEvent callback, MonoBehaviour genObject)
	{
		//float seed = Random.value * 1000;
		void OnFinished(WorldMap world)
        {
            callback(world);
        }
        genObject.StartCoroutine(GenerateCoroutine(sizeX, sizeY, seed, OnFinished));
    }

	private static IEnumerator GenerateCoroutine (int sizeX, int sizeY, float seed, WorldFinishedEvent callback) {
		WorldMap map = new WorldMap ();
		map.mapDict = new Dictionary<string, Dictionary<Vector2Int, MapUnit>> ();
		map.mapDict.Add (WorldSceneName, new Dictionary<Vector2Int, MapUnit> ());

        int tilesPerFrame = 100;
        int tilesDoneSinceFrame = 0;

		// Loop through every tile defined by the size, fill it with grass and maybe add a plant
		for (int y = 0; y < sizeY; y++) {
			for (int x = 0; x < sizeX; x++) {
				Vector2Int currentPosition = new Vector2Int (x, y);
				MapUnit mapTile = new MapUnit();

				float h;

				if (UseLinearGradient)
				{
					h = LinearGradient(new Vector2(x, y), sizeX, sizeY/2, false, false);
				}
				else
				{
					// Start with a nice height gradient from center to edges
					h = EllipseGradient(new Vector2(x - sizeX / 2, y - sizeY / 2), sizeX, sizeY);
				}

				// Round off the height with a log function
				if (h > 0)
					h = Mathf.Log(h + 1, 2);
				
				// Multiply layers of noise so the map is more interesting
				h = h * Mathf.PerlinNoise((noiseFrequencyLayer1 / 10) * x + seed, (noiseFrequencyLayer1 / 10) * y + seed) * noiseDepthLayer1 + h * (1 - noiseDepthLayer1);
				h = h * Mathf.PerlinNoise((noiseFrequencyLayer2 / 10) * x + seed, (noiseFrequencyLayer2 / 10) * y + seed) * noiseDepthLayer2 + h * (1 - noiseDepthLayer2);
				h = h * Mathf.PerlinNoise((noiseFrequencyLayer3 / 10) * x + seed, (noiseFrequencyLayer3 / 10) * y + seed) * noiseDepthLayer3 + h * (1 - noiseDepthLayer3);
				h = h * Mathf.PerlinNoise((noiseFrequencyLayer4 / 10) * x + seed, (noiseFrequencyLayer4 / 10) * y + seed) * noiseDepthLayer4 + h * (1 - noiseDepthLayer4);

				// Assign ground material based on height
				bool canHavePlants = false;
				if (h > sandLevel && !AllDesert)
				{
					mapTile.groundMaterial = ContentLibrary.Instance.GroundMaterials.GetGroundMaterialById(GrassMaterialId);
					canHavePlants = true;
				}
				else if (h > waterLevel)
					mapTile.groundMaterial = ContentLibrary.Instance.GroundMaterials.GetGroundMaterialById(SandMaterialId);
				else
					mapTile.groundMaterial = ContentLibrary.Instance.GroundMaterials.GetGroundMaterialById(WaterMaterialId);

				map.mapDict [WorldSceneName].Add (currentPosition, mapTile);

				// Decide whether to add a plant, and if so choose one randomly
				if (canHavePlants)
				{
					float b = EvenNoise((biotopeNoiseFreq / 10) * x + seed, (biotopeNoiseFreq / 10) * y + seed, seed);
					Biotope biotope = GetBiotope(b);

					if (Random.Range(0f, 1f) < biotope.entityFrequency)
					{
						map.mapDict[WorldSceneName][currentPosition].entityId = WeightedString.GetWeightedRandom(biotope.entities);
					}
					
				}
                tilesDoneSinceFrame++;
                if (tilesDoneSinceFrame >= tilesPerFrame)
                {
                    tilesDoneSinceFrame = 0;
                    yield return null;
                }
            }
		}
        callback(map);
	}

	// Returns a biotope for a given value between 0 and 1
	private static Biotope GetBiotope(float value)
	{
		if (value < -0.1 || value > 1.1)
		{
			Debug.LogError("Biotope input value isn't between 0 and 1!");
		}
		value = Mathf.Clamp(value, 0f, 0.999f);
		List<Biotope> biotopes = ContentLibrary.Instance.Biotopes.Biotopes;

		float weightSum = 0;
		foreach (Biotope biotope in biotopes)
		{
			weightSum += biotope.biotopeFrequency;
		}

		float target = value * weightSum;

		float currentSum = 0;
		foreach (Biotope biotope in biotopes)
		{
			currentSum += biotope.biotopeFrequency;
			if (currentSum >= target)
			{
				return biotope;
			}
		}

		Debug.LogError("Biotope not found.");
		int i = Mathf.FloorToInt(value * (ContentLibrary.Instance.Biotopes.Biotopes.Count));
		return ContentLibrary.Instance.Biotopes.Biotopes[i];
	}

	// A noise function with a somewhat even distribution
	private static float EvenNoise(float x, float y, float seed)
	{
		SimplexNoiseGenerator noise = new SimplexNoiseGenerator(seed.ToString());

		float n = noise.noise(x, y, 0);
		// Rescale the noise so we're only sampling a third of its original range
		n *= 3f;
		n += 0.5f;
		n = Mathf.Clamp01(n);
		return n;
	}

	// Returns a value based on the location of a point on a linear gradient. Gradient rises bottom to top or left to right if not flipped.
	// Origin is the lower right corner.
	private static float LinearGradient(Vector2 point, float width, float height, bool horizontal, bool flip)
	{
		float result;

		if (horizontal)
		{
			result = point.x / width;
		}
		else
		{
			result = point.y / height;
		}

		if (flip)
		{
			result = 1f - result;
		}

		return result;
	}

	// Returns a value between 0 and 1 based on where a point is between the origin and a surrounding ellipse.
	// 1 is the center of the ellipse, 0 is the outside. Origin is the center of the ellipse.
	private static float EllipseGradient(Vector2 point, float width, float height)
	{
		// diameters to radii
		width = width / 2;
		height = height / 2;

		if (point.x == 0 && point.y == 0)
		{
			return 1;
		}
		// Find point on ellipse that is on the line between the origin and input point
		float x = Mathf.Sqrt(Mathf.Pow(width * height * point.x, 2f) / (Mathf.Pow(point.x * height, 2) + Mathf.Pow(point.y * width, 2)));
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
}
