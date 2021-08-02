using System;
using ContentLibraries;
using MyBox;
using UnityEngine;

/// A plant that can be harvested through an interaction rather than just destroying it.
public class HarvestablePlant : MonoBehaviour, IInteractable
{
    //TODO consolidate harvestable plants with breakable plants

    //TODO support for swapping sprites after harvesting

    [SerializeField] private bool swapSprites = true;
    [SerializeField] [ConditionalField(nameof(swapSprites))]
    private SpriteRenderer spriteRenderer;
    [SerializeField] [ConditionalField(nameof(swapSprites))]
    private Sprite normalSprite;
    [SerializeField] [ConditionalField(nameof(swapSprites))]
    private Sprite harvestableSprite;
    [Separator]
    [SerializeField] private string droppedItemId; // TODO support for dropping multiple, different items
    [SerializeField] private bool destroyOnHarvest = false;
    /// How high the items drop from when harvested
    [SerializeField] private float dropHeight = 0.75f;
    [SerializeField] private int dropNumber = 1;
    [SerializeField] private int secondsBetweenHarvests = 300;

    //TODO saveable
    private ulong lastHarvestTick;

    public void OnInteract()
    {
        Harvest(out _);
    }

    private void Update()
    {
        ReadyToHarvest = TimeKeeper.CurrentTick - lastHarvestTick
            > (TimeKeeper.TicksPerRealSecond * (ulong)secondsBetweenHarvests);

        if (swapSprites)
            spriteRenderer.sprite = ReadyToHarvest ? harvestableSprite : normalSprite;
    }

    public bool ReadyToHarvest { get; private set; }

    public void Harvest(out DroppedItem droppedItem)
    {
        droppedItem = null;
        if (!ReadyToHarvest) return;

        lastHarvestTick = TimeKeeper.CurrentTick;
        ReadyToHarvest = false;

        for (int i = 0; i < dropNumber; i++)
        {
            Vector2 dropPosition = new Vector2(transform.localPosition.x, transform.localPosition.y + dropHeight);
            DroppedItem item = DroppedItemSpawner.SpawnItem(
                new ItemStack(droppedItemId, 1),
                dropPosition,
                SceneObjectManager.WorldSceneId);
            droppedItem = item;
            item.InitiateFakeFall(dropHeight);
        }

        if (destroyOnHarvest)
        {
            Vector2Int tilePos = new Vector2Int((int) transform.position.x, (int) transform.position.y);
            Vector2 localPos = TilemapInterface.WorldPosToScenePos(tilePos, SceneObjectManager.WorldSceneId);
            RegionMapManager.RemoveEntityAtPoint(
                new Vector2Int((int) localPos.x, (int) localPos.y),
                SceneObjectManager.WorldSceneId);
        }
    }
}