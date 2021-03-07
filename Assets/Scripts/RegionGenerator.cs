using System.Collections;
using System.Collections.Generic;
using ContentLibraries;
using UnityEngine;

public static class RegionGenerator
{
    // TODO define plants and generation parameters in a seperate file or object
    public delegate void WorldFinishedEvent(bool success, RegionMap world);

	private const string WorldSceneName = SceneObjectManager.WorldSceneId;
	private const bool AllDesert = false;

	private const string GrassMaterialId = "dead_grass";
	private const string SandMaterialId = "sand";
	private const string WaterMaterialId = "water";
	private const float SandLevel = 0.15f;         // Anything below this height is sand or water.
	private const float WaterLevel = 0.135f;       // Anything below this height is water.

	// higher frequency is grainier
	private const float NoiseFrequencyLayer1 = 0.2f;
	private const float NoiseFrequencyLayer2 = 1f;
	private const float NoiseFrequencyLayer3 = 1.5f;
	private const float NoiseFrequencyLayer4 = 2.5f;

	// how much each level affects the terrain
	private const float NoiseDepthLayer1 = 0.9f;
	private const float NoiseDepthLayer2 = 0.4f;
	private const float NoiseDepthLayer3 = 0.2f;
	private const float NoiseDepthLayer4 = 0.2f;
	
	private const float BiotopeNoiseFreq = 0.9f;

	private const string StartingShackId = "shack";
	private const string StartingWagonId = "wagon";

	private const float EntityPlacementRadiusPerRot = 10;
	private const float EntityPlacementDegreesPerAttempt = 20;
	private const int ShackPlacementAttempts = 50; // How many times we'll try to place the starting shack before failing world gen.
	private const float WagonDistance = 15; // Distance of wagon from starting shack
	
	private const int TilesPerFrame = 100; // How many tiles will be generated each frame.


	public static void StartGeneration(
		int sizeX,
		int sizeY,
		RegionInfo template,
		WorldFinishedEvent callback,
		MonoBehaviour genObject)
	{
        genObject.StartCoroutine(GenerateCoroutine(sizeX, sizeY, template, callback));
    }

	private static IEnumerator GenerateCoroutine (int sizeX, int sizeY, RegionInfo template, WorldFinishedEvent callback) {
		RegionMap map = new RegionMap ();
		map.mapDict = new Dictionary<string, Dictionary<Vector2Int, MapUnit>> ();
		map.mapDict.Add (WorldSceneName, new Dictionary<Vector2Int, MapUnit> ());

		int tilesDoneSinceFrame = 0;

		// Loop through every tile defined by the size, fill it with grass and maybe add a plant
		for (int y = 0; y < sizeY; y++) {
			for (int x = 0; x < sizeX; x++)
			{
				Vector2Int currentPosition = new Vector2Int(x, y);
				MapUnit mapTile = new MapUnit();

				float h; // height

				// Initialize the height for the tile based on this region's topography.
				if (template.topography == RegionTopography.EastCoast ||
				    template.topography == RegionTopography.WestCoast ||
				    template.topography == RegionTopography.NorthCoast ||
				    template.topography == RegionTopography.SouthCoast)
				{
					// This is a coastal region; we'll use a linear gradient.
					
					bool horizontal = template.topography == RegionTopography.EastCoast ||
					                  template.topography == RegionTopography.WestCoast;
					
					bool flip = template.topography == RegionTopography.EastCoast ||
					            template.topography == RegionTopography.NorthCoast;
					
					h = GenerationHelper.LinearGradient(new Vector2(x, y), sizeX, sizeY / 2f, horizontal, flip);
				}
				else if (false) // this is what an island topography would be, if we had islands
				{
					// Start with a nice height gradient from center to edges
					h = GenerationHelper.EllipseGradient(new Vector2(x - sizeX / 2, y - sizeY / 2), sizeX, sizeY);
				}
				else if (template.topography == RegionTopography.Water)
				{
					h = 0;
				}
				else
				{
					// Anything else is normal flat land.
					h = 1;
				} 

				// Round off the height with a log function, so coasts are mostly land.
				if (h > 0)
					h = Mathf.Log(2*h + 1, 3);

				
				// Multiply layers of noise so the map is more interesting
				
				h = h * Mathf.PerlinNoise((NoiseFrequencyLayer1 / 10) * x + template.seed,
					(NoiseFrequencyLayer1 / 10) * y + template.seed) * NoiseDepthLayer1 + h * (1 - NoiseDepthLayer1);
				
				h = h * Mathf.PerlinNoise((NoiseFrequencyLayer2 / 10) * x + template.seed,
					(NoiseFrequencyLayer2 / 10) * y + template.seed) * NoiseDepthLayer2 + h * (1 - NoiseDepthLayer2);
				
				h = h * Mathf.PerlinNoise((NoiseFrequencyLayer3 / 10) * x + template.seed,
					(NoiseFrequencyLayer3 / 10) * y + template.seed) * NoiseDepthLayer3 + h * (1 - NoiseDepthLayer3);
				
				h = h * Mathf.PerlinNoise((NoiseFrequencyLayer4 / 10) * x + template.seed,
					(NoiseFrequencyLayer4 / 10) * y + template.seed) * NoiseDepthLayer4 + h * (1 - NoiseDepthLayer4);

				
				// Assign ground material and vegetation based on height
				bool canHavePlants = false;
				if (h > SandLevel && !AllDesert) // Grass
				{
					mapTile.groundMaterial = ContentLibrary.Instance.GroundMaterials.Get(GrassMaterialId);
					canHavePlants = true;
				}
				else if (h > WaterLevel) // Sand
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
					float b = GenerationHelper.UniformSimplex((BiotopeNoiseFreq / 10) * x, (BiotopeNoiseFreq / 10) * y, template.seed);
					Biotope biotope = GetBiotope(ContentLibrary.Instance.Biomes.Get(template.biome), b);

					if (Random.Range(0f, 1f) < biotope.entityFrequency)
					{
						map.mapDict[WorldSceneName][currentPosition].entityId = WeightedString.GetWeightedRandom(biotope.entities);
					}

				}
				tilesDoneSinceFrame++;
				if (tilesDoneSinceFrame >= TilesPerFrame)
				{
					tilesDoneSinceFrame = 0;
					yield return null;
				}
			}
		}

		// Place player home stuff
		if (template.playerHome)
		{
			EntityData shackData = ContentLibrary.Instance.Entities.Get(StartingShackId);
			EntityData wagonData = ContentLibrary.Instance.Entities.Get(StartingWagonId);
			Vector2 mapCenter = new Vector2(sizeX / 2, sizeY / 2);
			Vector2 wagonOffset = Vector2.right * WagonDistance;
			wagonOffset *= (template.seed % 2f < 1f) ? -1 : 1; // Randomize which side of the house the wagon is on
			if (!AttemptPlaceEntity(shackData, ShackPlacementAttempts, mapCenter, new List<string>(), map, sizeX,
				sizeY))
			{
				callback(false, null);
			}

			if (!AttemptPlaceEntity(wagonData, ShackPlacementAttempts, mapCenter + wagonOffset,
				new List<string> {shackData.entityId}, map, sizeX, sizeY))
			{
				callback(false, null);
			}
		}
		
		// If this region has a defining feature, add it to the map
		if (template.feature != null)
		{
			if (!template.feature.AttemptApply(map, template.seed)) callback(false, null);
		}

		callback(true, map);
	}

	// Attempts to place the given entity on the map somewhere near the given target position over the
	// given number of attempts, moving farther from that position for each failed attempt. Returns false
	// if all attempts fail. Will not be placed over entities whose ID is contained in the given list.
	public static bool AttemptPlaceEntity(EntityData entity, int attempts, Vector2 targetPos, List<string> entityBlacklist, RegionMap map, int sizeX, int sizeY)
	{
		for (int i = 0; i < attempts; i++)
		{
			float rot = i * EntityPlacementDegreesPerAttempt;
			Vector2 pos = GenerationHelper.Spiral(EntityPlacementRadiusPerRot, 0f, false, rot);
			pos += targetPos;
			int tileX = Mathf.FloorToInt(pos.x);
			int tileY = Mathf.FloorToInt(pos.y);
			if (tileX > sizeX || tileX < 0 || tileY > sizeY || sizeY < 0)
			{
				Debug.LogWarning("Placement out of bounds: (" + tileX + ", " + tileY + ")");
				continue;
			}

			bool failure = false;
			foreach (Vector2Int basePosition in entity.baseShape)
			{
				Vector2Int absolute = new Vector2Int(basePosition.x + tileX, basePosition.y + tileY);
				if (map.mapDict[WorldSceneName].ContainsKey(absolute))
				{
					MapUnit mapUnit = map.mapDict[WorldSceneName][absolute];
					if (mapUnit.groundMaterial.isWater || entityBlacklist.Contains(mapUnit.entityId))
					{
						failure = true;
						break;
					}
				}
				else { failure = true; }
			}
			if (failure) continue;

			// It seems all of the tiles are buildable, so let's actually place the entity
			foreach (Vector2Int basePosition in entity.baseShape)
			{
				Vector2Int absolute = new Vector2Int(basePosition.x + tileX, basePosition.y + tileY);
				MapUnit mapUnit = map.mapDict[WorldSceneName][absolute];
				mapUnit.groundCover = null;
				mapUnit.entityId = entity.entityId;
				mapUnit.relativePosToEntityOrigin = basePosition;
			}
			return true;
		}
		return false;
	}

	// Returns a biotope for a given value between 0 and 1, based off of defined
	// biotope frequencies in the given biome.
	private static Biotope GetBiotope(Biome biome, float value)
	{
		if (value < -0.1 || value > 1.1)
		{
			Debug.LogError("Biotope input value isn't between 0 and 1!");
		}
		value = Mathf.Clamp(value, 0f, 0.99999f);
		List<Biome.BiotopeInfo> biotopes = biome.Biotopes;

		float weightSum = 0;
		foreach (Biome.BiotopeInfo biotope in biotopes)
		{
			weightSum += biotope.frequency;
		}

		float target = value * weightSum;

		float currentSum = 0;
		foreach (Biome.BiotopeInfo biotope in biotopes)
		{
			currentSum += biotope.frequency;
			if (currentSum >= target)
			{
				return ContentLibrary.Instance.Biotopes.Get(biotope.biotopeId);
			}
		}

		Debug.LogError("Biotope not found.");
		return null;
	}
}
