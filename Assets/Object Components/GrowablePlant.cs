using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowablePlant : MonoBehaviour
{
    // The amount that grow time can randomly vary by
    private const float GrowthTimeVariance = 0.1f;

    [SerializeField] string plantName;
    [SerializeField] List<Sprite> growthStages;
    [SerializeField] List<Sprite> witheredStages; // A withered variant for every growth stage.
    [SerializeField] float daysToGrow = 10;
    [SerializeField] float daysToDry = 1.1f; // Time without watering until a plant withers.
    [SerializeField] float daysToWither = 1.1f; // Time for a withering plant to die without water.
    [SerializeField] float daysToRecover = 0.75f; // Time to rehydrate after being withered.
    [SerializeField] float witheringGrowthMultiplier = 0.5f; // How fast a withering plant grows compared to a healthy one.

    // Per-tick conversions of the above values
    private float growthPerTick => (1 / daysToGrow) / (TimeKeeper.secondsPerDay * TimeKeeper.TicksPerIngameSecond);
    private float drynessPerTick => (1 / daysToDry) / (TimeKeeper.secondsPerDay * TimeKeeper.TicksPerIngameSecond);
    private float witherPerTick => (1 / daysToWither) / (TimeKeeper.secondsPerDay * TimeKeeper.TicksPerIngameSecond);
    private float witherRecoveryPerTick => (1 / daysToRecover) / (TimeKeeper.secondsPerDay * TimeKeeper.TicksPerIngameSecond);

    private float growthProgress; // How grown this plant is, between 0 and 1.
    private float hydration; // Between 0 and 1. A newly watered plant has hydration = 1. Declines until 0, after which the plant withers.
    private float witheredness; // Increases when a plant is dry. At witheredness = 1, the plant dies.
    private ulong plantTime; // When this plant was planted.

    private SpriteRenderer spriteRenderer;
    private BreakableObject breakable;

    // Start is called before the first frame update
    void Start()
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
    void Update()
    {
        if (growthStages == null || growthStages.Count == 0)
        {
            return;
        }
        if (hydration > 0)
        {
            witheredness -= witherRecoveryPerTick * TimeKeeper.DeltaTicks;
            hydration -= drynessPerTick * TimeKeeper.DeltaTicks;

            hydration = Mathf.Clamp01(hydration);
            witheredness = Mathf.Clamp01(witheredness);
        }
        else
        {
            // Plant is withering
            witheredness += witherPerTick * TimeKeeper.DeltaTicks;
        }


        if (witheredness > 0)
        {
            growthProgress += growthPerTick * TimeKeeper.DeltaTicks * witheringGrowthMultiplier;
        }
        else
        {
            growthProgress += growthPerTick * TimeKeeper.DeltaTicks;
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
        }

        // The plant dies when 100% withered
        if (witheredness >= 1)
        {
            breakable.Break();
        }
    }

    public void Water()
    {
        hydration = 1;
    }
}
