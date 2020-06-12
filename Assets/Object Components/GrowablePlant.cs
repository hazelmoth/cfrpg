using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowablePlant : MonoBehaviour
{
    // The amount that grow time can randomly vary by
    private const float GrowthTimeVariance = 0.1f;

    [SerializeField] List<Sprite> growthStages;
    [SerializeField] float daysToGrow;

    private float growthProgress;
    private TimeKeeper.DateTime plantTime;
    private SpriteRenderer spriteRenderer;
    private BreakableObject breakable;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        breakable = GetComponent<BreakableObject>();
        plantTime = TimeKeeper.CurrentDateTime;
        daysToGrow += (daysToGrow * Random.Range(-GrowthTimeVariance, GrowthTimeVariance));
    }

    // Update is called once per frame
    void Update()
    {
        if (growthStages == null || growthStages.Count == 0)
        {
            return;
        }
        growthProgress = TimeKeeper.daysBetween(plantTime, TimeKeeper.CurrentDateTime) / daysToGrow;
        growthProgress = Mathf.Clamp01(growthProgress);
        int i = Mathf.FloorToInt(growthProgress * (growthStages.Count - 1));

        spriteRenderer.sprite = growthStages[i];

        if (breakable)
        {
            breakable.enableDrops = growthProgress >= 1;
        }
    }
}
