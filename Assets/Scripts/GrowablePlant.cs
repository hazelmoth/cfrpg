using System.Collections.Generic;
using UnityEngine;

public class GrowablePlant : MonoBehaviour, ISaveable
{
    private const string ComponentSaveId = "growable_plant";
    private const string GrowthProgressTag = "growth";
    private const string HydrationTag = "hydration";
    private const string WitherednessTag = "witheredness";
    private const string PlantTimeTag = "plant_time";

    // The amount that grow time can randomly vary by
    private const float GrowthTimeVariance = 0.1f;

    [SerializeField] private string plantName;
    [SerializeField] private List<Sprite> growthStages;
    [SerializeField] private List<Sprite> witheredStages; // A withered variant for every growth stage.
    [SerializeField] private float daysToGrow = 10;
    [SerializeField] private float daysToDry = 1f; // Time without watering until a plant withers.
    [SerializeField] private float daysToWither = 1.1f; // Time for a withering plant to die without water.
    [SerializeField] private float daysToRecover = 0.75f; // Time to rehydrate after being withered.
    [SerializeField] private float witheringGrowthMultiplier = 0.5f; // How fast a withering plant grows compared to a healthy one.

    // Per-tick conversions of the above values
    private float GrowthPerTick => (1 / daysToGrow) / (TimeKeeper.SecondsPerDay * TimeKeeper.TicksPerInGameSecond);
    private float DrynessPerTick => (1 / daysToDry) / (TimeKeeper.SecondsPerDay * TimeKeeper.TicksPerInGameSecond);
    private float WitherPerTick => (1 / daysToWither) / (TimeKeeper.SecondsPerDay * TimeKeeper.TicksPerInGameSecond);
    private float WitherRecoveryPerTick => (1 / daysToRecover) / (TimeKeeper.SecondsPerDay * TimeKeeper.TicksPerInGameSecond);

    private float growthProgress; // How grown this plant is, between 0 and 1.
    private float hydration; // Between 0 and 1. A newly watered plant has hydration = 1. Declines until 0, after which the plant withers.
    private float witheredness; // Increases when a plant is dry. At witheredness = 1, the plant dies.
    private ulong plantTime; // When this plant was planted.

    private SpriteRenderer spriteRenderer;
    private BreakableObject breakable;
    string ISaveable.ComponentId => ComponentSaveId;

    // Start is called before the first frame update
    private void Start()
    {
        if (growthStages.Count != witheredStages.Count)
        {
            Debug.LogError("This " + plantName + " plant has a different number of growth stage sprites and withered sprites. " + 
                "There should be a withered sprite for every growth stage.");
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        breakable = GetComponent<BreakableObject>();
        plantTime = TimeKeeper.CurrentTick;
        daysToGrow += (daysToGrow * Random.Range(-GrowthTimeVariance, GrowthTimeVariance));

        hydration = 1; // A just-planted plant doesn't need to be watered yet
    }

    // Update is called once per frame
    private void Update()
    {
        if (growthStages == null || growthStages.Count == 0)
        {
            return;
        }

        EntityObject thisEntity = GetComponent<EntityObject>();
        if (RegionMapManager.GetMapUnitAtPoint(thisEntity.Location.Vector2Int, thisEntity.Scene).IsMoist)
        {
            // Moist tiles keep their hydration at max
            hydration = 1;
        }

        if (hydration > 0)
        {
            witheredness -= WitherRecoveryPerTick * TimeKeeper.DeltaTicks;
            hydration -= DrynessPerTick * TimeKeeper.DeltaTicks;

            hydration = Mathf.Clamp01(hydration);
            witheredness = Mathf.Clamp01(witheredness);
        }
        else
        {
            // Plant is withering
            witheredness += WitherPerTick * TimeKeeper.DeltaTicks;
        }


        if (witheredness > 0)
        {
            growthProgress += GrowthPerTick * TimeKeeper.DeltaTicks * witheringGrowthMultiplier;
        }
        else
        {
            growthProgress += GrowthPerTick * TimeKeeper.DeltaTicks;
        }
        growthProgress = Mathf.Clamp01(growthProgress);


        int i = Mathf.FloorToInt(growthProgress * (growthStages.Count - 1));
        if (witheredness > 0)
        {
            spriteRenderer.sprite = witheredStages[i];
        }
        else
        {
            spriteRenderer.sprite = growthStages[i];
        }

        if (breakable)
        {
            breakable.enableDrops = growthProgress >= 1 && witheredness < 1;
            
            // The plant dies when 100% withered
            if (witheredness >= 1)
            {
                breakable.Break();
            }
        }
    }

    /// Whether this plant is at its final growth stage.
    public bool FullyGrown => growthProgress > 0.9999f;

    /// Reverts this plant's growth to the beginning of the previous stage.
    public void RevertGrowthStage()
    {
        int currentStage = Mathf.FloorToInt((growthProgress + 0.0001f) * (growthStages.Count - 1));
        growthProgress = (1f / growthStages.Count) * (currentStage - 1) + 0.0001f;
        growthProgress = Mathf.Clamp01(growthProgress);
    }

    IDictionary<string, string> ISaveable.GetTags()
    {
        Dictionary<string, string> tags = new Dictionary<string, string>();
        tags[GrowthProgressTag] = growthProgress.ToString();
        tags[HydrationTag] = hydration.ToString();
        tags[WitherednessTag] = witheredness.ToString();
        tags[PlantTimeTag] = plantTime.ToString();
        return tags;
    }

    void ISaveable.SetTags(IDictionary<string, string> tags)
    {
        growthProgress = float.Parse(tags[GrowthProgressTag]);
        hydration = float.Parse(tags[HydrationTag]);
        witheredness = float.Parse(tags[WitherednessTag]);
        plantTime = ulong.Parse(tags[PlantTimeTag]);
    }
}
