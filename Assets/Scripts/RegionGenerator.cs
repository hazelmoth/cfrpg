using System.Collections;
using System.Collections.Generic;
using System.Threading;
using ContentLibraries;
using FeatureGenerators;
using UnityEngine;

public static class RegionGenerator
{
	public delegate void WorldFinishedEvent(bool success, RegionMap world);

	private const string WorldSceneName = SceneObjectManager.WorldSceneId;
	private const bool AllDesert = false;

	private const int CliffBorderThickness = 20; // How many tiles surrounding the region are cliffs.
	private const int CliffBorderRadius = 13;   // For rounded borders
	private const int ExitPathWidth = 4;       // The width of the paths out of the map
	private const int BeachGradientSize = 25; // How many tiles at the edge of the map slope down into the water.
	private const string CliffMaterialId = "cliff";
	private const string SandMaterialId = "sand";
	private const string WaterMaterialId = "water";
	private const string DeepWaterMaterialId = "water_deep";
	private const float SandLevel = 0.15f;         // Anything below this height is sand or water.
	private const float WaterLevel = 0.135f;       // Anything below this height is water.

	// higher frequency is grainier
	private const float NoiseFrequencyLayer1 = 0.6f;
	private const float NoiseFrequencyLayer2 = 1f;
	private const float NoiseFrequencyLayer3 = 1.5f;
	private const float NoiseFrequencyLayer4 = 2.5f;

	// how much each level affects the terrain
	private const float NoiseDepthLayer1 = 0.5f;
	private const float NoiseDepthLayer2 = 0.4f;
	private const float NoiseDepthLayer3 = 0.2f;
	private const float NoiseDepthLayer4 = 0.2f;
	
	private const float BiotopeNoiseFreq = 0.9f;

	private const string StartingShackId = "shack";

	private const float EntityPlacementRadiusPerRot = 10;
	private const float EntityPlacementDegreesPerAttempt = 20;
	private const int ShackPlacementAttempts = 50; // How many times we'll try to place the starting shack before failing world gen.

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

		// Loop through every tile defined by the size (+ cliff borders); populate
		// its ground material and sometimes add natural entities
		for (int y = 0 - CliffBorderThickness; y < sizeY + CliffBorderThickness; y++) {
			for (int x = 0 - CliffBorderThickness; x < sizeX + CliffBorderThickness; x++)
			{
				Vector2Int currentPosition = new Vector2Int(x, y);
				bool isBorder = (y < 0 || x < 0) || (y >= sizeY || x >= sizeX);
				MapUnit mapTile = new MapUnit();

				float h = 1; // height

				if (template.coasts.Count == 4)
				{
					// This is an island region.
					h = GenerationHelper.EllipseGradient(new Vector2(x - sizeX / 2, y - sizeY / 2), sizeX, sizeY);
				}
				else foreach (Direction coastDirection in template.coasts)
				{
					// This is a coastal region; we'll use a linear gradient.
					
					bool horizontal = coastDirection == Direction.Right ||
					                  coastDirection == Direction.Left;
					
					bool flip = coastDirection == Direction.Right ||
					            coastDirection == Direction.Up;

					int gradientStart = flip ? (horizontal ? sizeX : sizeY) : 0;
					int gradientEnd = flip ? (horizontal ? sizeX : sizeY) - BeachGradientSize : BeachGradientSize;
					
					h *= GenerationHelper.LinearGradient(new Vector2(x, y), horizontal, gradientStart, gradientEnd);
				}

				// Multiply layers of noise so the map is more interesting
				
				h = h * Mathf.PerlinNoise((NoiseFrequencyLayer1 / 10) * x + template.seed,
					(NoiseFrequencyLayer1 / 10) * y + template.seed) * NoiseDepthLayer1 + h * (1 - NoiseDepthLayer1);
				
				h = h * Mathf.PerlinNoise((NoiseFrequencyLayer2 / 10) * x + template.seed,
					(NoiseFrequencyLayer2 / 10) * y + template.seed) * NoiseDepthLayer2 + h * (1 - NoiseDepthLayer2);
				
				h = h * Mathf.PerlinNoise((NoiseFrequencyLayer3 / 10) * x + template.seed,
					(NoiseFrequencyLayer3 / 10) * y + template.seed) * NoiseDepthLayer3 + h * (1 - NoiseDepthLayer3);
				
				h = h * Mathf.PerlinNoise((NoiseFrequencyLayer4 / 10) * x + template.seed,
					(NoiseFrequencyLayer4 / 10) * y + template.seed) * NoiseDepthLayer4 + h * (1 - NoiseDepthLayer4);

				
				// ========= Assign ground material based on height ============
				
				bool canHavePlants;
				
				Biome biome = ContentLibrary.Instance.Biomes.Get(template.biome);
				if (biome == null)
				{
					Debug.LogError("Biome not found! ID: " + template.biome);
					callback(false, null);
					yield break;
				}

				GroundMaterial grassMaterial = ContentLibrary.Instance.GroundMaterials.Get(biome.GrassMaterial);
				
				if (h > SandLevel && !AllDesert) // Grass
				{
					mapTile.groundMaterial = grassMaterial;
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
					canHavePlants = false;
				}
				
				
				// ========= Rounded edges =====================================
				
				{
					if ((x < CliffBorderRadius && y < CliffBorderRadius)
							&& Vector2.Distance(
								new Vector2(CliffBorderRadius, CliffBorderRadius), 
								new Vector2(x, y)
							) > CliffBorderRadius 
					    || (x >= sizeX - CliffBorderRadius-1 && y < CliffBorderRadius)
						    && Vector2.Distance(
								new Vector2(sizeX - CliffBorderRadius-1, CliffBorderRadius), 
								new Vector2(x, y)
				            ) > CliffBorderRadius
					    || (x < CliffBorderRadius && y >= sizeY - CliffBorderRadius)
						    && Vector2.Distance(
							    new Vector2(CliffBorderRadius, sizeY - CliffBorderRadius-1), 
							    new Vector2(x, y)
						    ) > CliffBorderRadius
					    || (x >= sizeX - CliffBorderRadius && y >= sizeY - CliffBorderRadius)
						    && Vector2.Distance(
							    new Vector2(sizeX - CliffBorderRadius-1, sizeY - CliffBorderRadius-1), 
							    new Vector2(x, y)
						    ) > CliffBorderRadius)
					{
						isBorder = true;
					}
				}
				
				// ========= Exit paths ========================================

				// Exempt exit paths from being considered borders.
				if (!template.coasts.Contains(Direction.Left) &&
				    x < sizeX / 2 &&
				    y >= (sizeY / 2) - (ExitPathWidth / 2) &&
				    y < (sizeY / 2) + (ExitPathWidth / 2)
				    ||
				    !template.coasts.Contains(Direction.Right) &&
				    x >= sizeX / 2 &&
				    y >= (sizeY / 2) - (ExitPathWidth / 2) &&
				    y < (sizeY / 2) + (ExitPathWidth / 2)
				    ||
				    !template.coasts.Contains(Direction.Up) &&
				    x > (sizeX / 2) - (ExitPathWidth / 2) &&
				    x <= (sizeX / 2) + (ExitPathWidth / 2) &&
				    y >= sizeY / 2
				    ||
				    !template.coasts.Contains(Direction.Down) &&
				    x >= (sizeX / 2) - (ExitPathWidth / 2) &&
				    x < (sizeX / 2) + (ExitPathWidth / 2) &&
				    y < sizeY / 2)
				{
					isBorder = false;
				}

				// Add cliff tiles or deep water to borders
				if (isBorder)
				{
					if (mapTile.groundMaterial.Id == WaterMaterialId)
					{
						mapTile.groundCover = ContentLibrary.Instance.GroundMaterials.Get(DeepWaterMaterialId);
					}
					else
					{
						mapTile.cliffMaterial = ContentLibrary.Instance.GroundMaterials.Get(CliffMaterialId);
						mapTile.groundMaterial = ContentLibrary.Instance.GroundMaterials.Get(SandMaterialId);
					}
					canHavePlants = false;
				}
				
				// Decide whether to add a plant, and if so choose one randomly
				if (canHavePlants)
				{
					float b = GenerationHelper.UniformSimplex((BiotopeNoiseFreq / 10) * x, (BiotopeNoiseFreq / 10) * y, template.seed);
					
					Biotope biotope = GetBiotope(biome, b);

					if (Random.Range(0f, 1f) < biotope.entityFrequency)
					{
						mapTile.entityId = WeightedString.GetWeightedRandom(biotope.entities);
					}

				}
				
				// Actually add the tile to the map
				map.mapDict[WorldSceneName].Add(currentPosition, mapTile);
				
				tilesDoneSinceFrame++;
				if (tilesDoneSinceFrame >= TilesPerFrame)
				{
					tilesDoneSinceFrame = 0;
					yield return null;
				}
			}
		}

		// ========= Player home ===============================================
		
		if (template.playerHome)
		{
			EntityData shackData = ContentLibrary.Instance.Entities.Get(StartingShackId);
			Vector2 mapCenter = new Vector2(sizeX / 2f, sizeY / 2f);
			if (!map.AttemptPlaceEntity(
				shackData,
				ShackPlacementAttempts, 
				mapCenter, 
				new List<string>()))
			{
				callback(false, null);
			}
		}
		
		// ======== Add defining feature =======================================
		
		if (template.feature != null)
		{
			RegionFeatureGenerator featureGenerator = ContentLibrary.Instance.RegionFeatures.Get(template.feature);
			if (!featureGenerator.AttemptApply(map, template, template.seed)) callback(false, null);
		}

		// ======== Spawn actors ===============================================
		
		foreach (string templateId in ContentLibrary.Instance.Biomes.Get(template.biome).PickSpawnTemplates())
		{
			ActorTemplate characterTemplate = ContentLibrary.Instance.ActorTemplates.Get(templateId);
			if (characterTemplate == null)
			{
				Debug.LogError($"Missing character generation template \"{templateId}\".");
				continue;
			}

			ActorData actor = ActorGenerator.Generate(characterTemplate);
			ActorRegistry.Register(actor);
			Vector2 spawnPoint = ActorSpawnpointFinder.FindSpawnPoint(map, SceneObjectManager.WorldSceneId);
			Location spawnLocation = new Location(spawnPoint, SceneObjectManager.WorldSceneId);
			Direction direction = Direction.Down;
			
			map.actors.Add(actor.actorId, new RegionMap.ActorPosition(spawnLocation, direction));
		}

		foreach (string actorId in template.unspawnedActors ?? new List<string>())
		{
			Vector2 spawnPoint = ActorSpawnpointFinder.FindSpawnPoint(map, SceneObjectManager.WorldSceneId);
			Location spawnLocation = new Location(spawnPoint, SceneObjectManager.WorldSceneId);
			Direction direction = Direction.Down;
			
			map.actors.Add(actorId, new RegionMap.ActorPosition(spawnLocation, direction));
		}
		
		callback(true, map);
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
				Biotope tope = ContentLibrary.Instance.Biotopes.Get(biotope.biotopeId);
				
				if (tope != null) return tope;
				else
				{
					Debug.LogError("Biotope not found: " + biotope.biotopeId);
				}
			}
		}

		Debug.LogError("Biotope not found.");
		return null;
	}
}
