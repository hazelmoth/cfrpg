using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using ContentLibraries;
using MyBox;
using UnityEngine;

/// A plant that can be harvested through an interaction rather than just destroying it.
public class HarvestablePlant : MonoBehaviour, IInteractable
{
    private enum GrowthMode
    {
        HarvestPeriodically,
        HarvestWhenFullyGrown
    }
    //TODO consolidate harvestable plants with breakable plants

    //TODO support for swapping sprites after harvesting

    /// How this plant will handle sprites
    [SerializeField] private GrowthMode growthMode = GrowthMode.HarvestPeriodically;
    [SerializeField] [ConditionalField(nameof(growthMode), false, GrowthMode.HarvestPeriodically)]
    private SpriteRenderer spriteRenderer;
    [SerializeField] [ConditionalField(nameof(growthMode), false, GrowthMode.HarvestPeriodically)]
    private Sprite normalSprite;
    [SerializeField] [ConditionalField(nameof(growthMode), false, GrowthMode.HarvestPeriodically)]
    private Sprite harvestableSprite;
    [SerializeField] [ConditionalField(nameof(growthMode), false, GrowthMode.HarvestPeriodically)]
    private int secondsBetweenHarvests = 300;

    [SerializeField] [ConditionalField(nameof(growthMode), false, GrowthMode.HarvestWhenFullyGrown)]
    private GrowablePlant growablePlantComponent;
    
    [Separator]
    [SerializeField] private bool destroyOnHarvest = false;
    /// How high the items drop from when harvested
    [SerializeField] private float dropHeight = 0.75f;
    [SerializeField] private CompoundWeightedTable dropTable;

    //TODO saveable
    private ulong lastHarvestTick;

    public void OnInteract()
    {
        Harvest(out _);
    }

    private void Update()
    {
        if (growthMode == GrowthMode.HarvestPeriodically)
        {
            ReadyToHarvest = TimeKeeper.CurrentTick - lastHarvestTick
                > (TimeKeeper.TicksPerRealSecond * (ulong)secondsBetweenHarvests);
            
            spriteRenderer.sprite = ReadyToHarvest ? harvestableSprite : normalSprite;
        }

        if (growthMode == GrowthMode.HarvestWhenFullyGrown)
        {
            ReadyToHarvest = growablePlantComponent.FullyGrown;
        }
    }

    public bool ReadyToHarvest { get; private set; }

    public void Harvest(out ImmutableList<DroppedItem> droppedItems)
    {
        droppedItems = ImmutableList<DroppedItem>.Empty;
        if (!ReadyToHarvest) return;

        List<DroppedItem> items = new List<DroppedItem>();
        lastHarvestTick = TimeKeeper.CurrentTick;

        dropTable.Pick()
            .ForEach(
                id =>
                {
                    Vector2 localPosition = transform.localPosition;
                    Vector2 dropPosition = new Vector2(localPosition.x, localPosition.y + dropHeight);
                    DroppedItem item = DroppedItemSpawner.SpawnItem(
                        new ItemStack(id, 1),
                        dropPosition,
                        SceneObjectManager.WorldSceneId);
                    item.InitiateFakeFall(dropHeight);
                    items.Add(item);
                });
        droppedItems = items.ToImmutableList();

        if (destroyOnHarvest)
        {
            Vector2Int tilePos = new Vector2Int((int) transform.position.x, (int) transform.position.y);
            Vector2 localPos = TilemapInterface.WorldPosToScenePos(tilePos, SceneObjectManager.WorldSceneId);
            RegionMapManager.RemoveEntityAtPoint(
                new Vector2Int((int) localPos.x, (int) localPos.y),
                SceneObjectManager.WorldSceneId);
        }
        
        if (growthMode == GrowthMode.HarvestWhenFullyGrown)
            growablePlantComponent.RevertGrowthStage();
    }
}