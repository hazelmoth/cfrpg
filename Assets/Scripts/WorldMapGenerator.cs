using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapGenerator : MonoBehaviour
{
    // TODO define plants and generation parameters in a seperate file or object
    public delegate void WorldFinishedEvent(bool success, WorldMap world);

	private const string WorldSceneName = SceneObjectManager.WorldSceneId;
	private const float PlantFrequency = 0.2f;
	private const bool UseLinearGradient = false;
	private const bool UseEllipticalGradient = false;
	private const bool AllDesert = false;

	private const string GrassMaterialId = "dead_grass";
	private const string SandMaterialId = "sand";
	private const string WaterMaterialId = "water";

	// higher frequency is grainier
	private const float noiseFrequencyLayer1 = 0.2f;
	private const float noiseFrequencyLayer2 = 1f;
	private const float noiseFrequencyLayer3 = 1.5f;
	private const float noiseFrequencyLayer4 = 2.5f;
	// how much each level affects the terrain
	private const float noiseDepthLayer1 = 0.9f;
	private const float noiseDepthLayer2 = 0.4f;
	private const float noiseDepthLayer3 = 0.2f;
	private const float noiseDepthLayer4 = 0.2f;
	private const float sandLevel = 0.15f; // Anything below this height is sand or water
	private const float waterLevel = 0.135f; // Anything below this height is water

	private const float biotopeNoiseFreq = 0.9f;

	private const float ShackPlacementRadiusPerRot = 5;
	private const float ShackPlacementDegreesPerAttempt = 20;
	private const int ShackPlacementAttempts = 50; // How many times we'll try to place a shack before giving up.


	public static void StartGeneration (int sizeX, int sizeY, float seed, WorldFinishedEvent callback, MonoBehaviour genObject)
	{
        genObject.StartCoroutine(GenerateCoroutine(sizeX, sizeY, seed, callback));
    }

	private static IEnumerator GenerateCoroutine (int sizeX, int sizeY, float seed, WorldFinishedEvent callback) {
		WorldMap map = new WorldMap ();
		map.mapDict = new Dictionary<string, Dictionary<Vector2Int, MapUnit>> ();
		map.mapDict.Add (WorldSceneName, new Dictionary<Vector2Int, MapUnit> ());

        int tilesPerFrame = 100;
        int tilesDoneSinceFrame = 0;

		// Loop through every tile defined by the size, fill it with grass and maybe add a plant
		for (int y = 0; y < sizeY; y++) {
			for (int x = 0; x < sizeX; x++)
			{
				Vector2Int currentPosition = new Vector2Int(x, y);
				MapUnit mapTile = new MapUnit();

				float h;

				if (UseLinearGradient)
				{
					h = LinearGradient(new Vector2(x, y), sizeX, sizeY / 2, false, false);
				}
				else if (UseEllipticalGradient)
				{
					// Start with a nice height gradient from center to edges
					h = EllipseGradient(new Vector2(x - sizeX / 2, y - sizeY / 2), sizeX, sizeY);
				}
				else
				{
					h = 1;
				}

				// Round off the height with a log function
				if (h > 0)
					h = Mathf.Log(h + 1, 2);

				// Multiply layers of noise so the map is more interesting
				h = h * Mathf.PerlinNoise((noiseFrequencyLayer1 / 10) * x + seed, (noiseFrequencyLayer1 / 10) * y + seed) * noiseDepthLayer1 + h * (1 - noiseDepthLayer1);
				h = h * Mathf.PerlinNoise((noiseFrequencyLayer2 / 10) * x + seed, (noiseFrequencyLayer2 / 10) * y + seed) * noiseDepthLayer2 + h * (1 - noiseDepthLayer2);
				h = h * Mathf.PerlinNoise((noiseFrequencyLayer3 / 10) * x + seed, (noiseFrequencyLayer3 / 10) * y + seed) * noiseDepthLayer3 + h * (1 - noiseDepthLayer3);
				h = h * Mathf.PerlinNoise((noiseFrequencyLayer4 / 10) * x + seed, (noiseFrequencyLayer4 / 10) * y + seed) * noiseDepthLayer4 + h * (1 - noiseDepthLayer4);

				// Assign ground material and vegetation based on height
				bool canHavePlants = false;
				if (h > sandLevel && !AllDesert) // Grass
				{
					mapTile.groundMaterial = ContentLibrary.Instance.GroundMaterials.Get(GrassMaterialId);
					canHavePlants = true;
				}
				else if (h > waterLevel) // Sand
				{
					mapTile.groundMaterial = ContentLibrary.Instance.GroundMaterials.Get(SandMaterialId);
					canHavePlants = false; // no vegetation on sand
				}
				else // Water
				{
					mapTile.groundMaterial = ContentLibrary.Instance.GroundMaterials.Get(WaterMaterialId);
				}


				map.mapDict[WorldSceneName].Add(currentPosition, mapTile);

				// Decide whether to add a plant, and if so choose one randomly
				if (canHavePlants)
				{
					float b = UniformSimplex((biotopeNoiseFreq / 10) * x + seed, (biotopeNoiseFreq / 10) * y + seed, seed);
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

		if (!AttemptPlaceShack(map, sizeX, sizeY))
		{
			callback(false, null);
		}
		callback(true, map);
	}

	private static bool AttemptPlaceShack(WorldMap map, int sizeX, int sizeY)
	{
		for (int i = 0; i < ShackPlacementAttempts; i++)
		{
			float rot = i * ShackPlacementDegreesPerAttempt;
			Vector2 pos = Spiral(ShackPlacementRadiusPerRot, 0f, false, rot);
			int tileX = Mathf.FloorToInt(pos.x);
			int tileY = Mathf.FloorToInt(pos.y);
			tileX += sizeX / 2;
			tileY += sizeY / 2;
			if (tileX > sizeX || tileX < 0 || tileY > sizeY || sizeY < 0)
			{
				Debug.LogWarning("Shack placement out of bounds: (" + tileX + ", " + tileY + ")");
				continue;
			}
			EntityData shack = ContentLibrary.Instance.Entities.Get("shack");

			bool failure = false;
			foreach (Vector2Int basePosition in shack.baseShape)
			{
				Vector2Int absolute = new Vector2Int(basePosition.x + tileX, basePosition.y + tileY);
				if (map.mapDict[WorldSceneName].ContainsKey(absolute))
				{
					MapUnit mapUnit = map.mapDict[WorldSceneName][absolute];
					if (mapUnit.groundMaterial.isWater)
					{
						failure = true;
						break;
					}
				}
				else { failure = true; }
			}
			if (failure) continue;

			// It seems all of the tiles are buildable, so let's actually place the entity
			foreach (Vector2Int basePosition in shack.baseShape)
			{
				Vector2Int absolute = new Vector2Int(basePosition.x + tileX, basePosition.y + tileY);
				MapUnit mapUnit = map.mapDict[WorldSceneName][absolute];
				mapUnit.groundCover = null;
				mapUnit.entityId = shack.entityId;
				mapUnit.relativePosToEntityOrigin = basePosition;
			}
			return true;
		}
		return false;
	}

	// Returns a biotope for a given value between 0 and 1, based off of defined biotope frequencies
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

	// A noise function with a somewhat uniform distribution
	private static float UniformSimplex(float x, float y, float seed)
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

	// Returns a position along a spiral after the given degrees of rotations, moving out at the given number of units
	// per rotation and starting at the given angle.
	private static Vector2 Spiral(float radiusPerRot, float startAngle, bool clockwise, float degrees)
	{
		float rot = degrees / 360;
		float radius = rot * radiusPerRot;
		float angle = startAngle;
		angle += rot * 360 * (clockwise ? -1f : 1f);
		Vector2 vector = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
		vector *= radius;
		return vector;
	}
}
