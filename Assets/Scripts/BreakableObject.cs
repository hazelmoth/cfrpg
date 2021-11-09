using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;
using Random = UnityEngine.Random;

public class BreakableObject : MonoBehaviour, IImpactReceiver
{
    public delegate void ObjectDropItemsEvent(List<DroppedItem> items);

    private const float ShakeDuration = 0.1f;
    private const float ShakeDistance = 0.13f;

    [SerializeField] private float maxHealth = 1.0f;
    [SerializeField] private List<ItemDrop> itemDrops = new();
    [SerializeField] private bool useCustomDamageMultipliers = false;

    [SerializeField]
    [ConditionalField(nameof(useCustomDamageMultipliers), true)]
    private DamageReceptionConfig damageMultiplierConfig;

    [SerializeField]
    [ConditionalField(nameof(useCustomDamageMultipliers))]
    private CollectionWrapper<DamageMultiplier> customDamageMultipliers = new();

    public bool enableDrops = true;

    private float currentHealth;
    private bool hasBeenHit = false;
    private bool isShaking = false;

    private Vector2 originalPos;

    void IImpactReceiver.OnImpact(ImpactInfo impact)
    {
        if (!enabled) return;

        if (!hasBeenHit)
        {
            currentHealth = maxHealth;
            originalPos = transform.position;
        }

        List<DamageMultiplier> damageMultipliers = useCustomDamageMultipliers
            ? customDamageMultipliers.Value.ToList()
            : damageMultiplierConfig.GetMultipliers();

        float hitDamage = damageMultipliers == null || !damageMultipliers.Any()
            ? impact.force.magnitude
            : damageMultipliers.Where(x => x.damageType == impact.damageType)
                .Select(x => x.damageMultiplier * impact.force.magnitude)
                .FirstOrDefault();

        hasBeenHit = true;
        currentHealth -= hitDamage;

        // Break the object if health is at 0
        if (currentHealth <= 0)
            Break();
        else if (hitDamage > 7) // For nontrivial amounts of damage, shake the object
            Shake(ShakeDistance, impact.force.normalized);
    }

    public event ObjectDropItemsEvent OnDropItems;

    // So derived classes can raise this event
    protected void RaiseDropItemsEvent(List<DroppedItem> items)
    {
        OnDropItems?.Invoke(items);
    }

    public void Break()
    {
        EntityObject entity = GetComponent<EntityObject>();

        if (enableDrops) DropItems();

        // If this is an entity, remove it through WorldMapManager; otherwise, just destroy it
        if (entity != null)
        {
            Vector2 localPos = TilemapInterface.WorldPosToScenePos(transform.position, SceneObjectManager.WorldSceneId);
            RegionMapManager.RemoveEntityAtPoint(TilemapInterface.FloorToTilePos(localPos), entity.Scene);
        }
        else
        {
            Debug.LogWarning("destroyed some non-entity thing!");
            Destroy(gameObject);
        }
    }

    protected virtual void DropItems()
    {
        List<DroppedItem> droppedItems = new();
        foreach (ItemDrop drop in itemDrops)
            for (int i = 0; i < drop.maxQuantity; i++)
                if (Random.value < drop.dropProbability)
                {
                    const float dropHeight = 0.5f;
                    Vector2 dropPosition = new(transform.localPosition.x, transform.localPosition.y + dropHeight);
                    DroppedItem item = DroppedItemSpawner.SpawnItem(
                        new ItemStack(drop.itemId, 1),
                        dropPosition,
                        SceneObjectManager.WorldSceneId);
                    item.InitiateFakeFall(dropHeight);
                    droppedItems.Add(item);
                }

        RaiseDropItemsEvent(droppedItems);
    }

    private void Shake(float distance, Vector2 direction)
    {
        if (!isShaking)
        {
            originalPos = transform.position;
        }
        else
        {
            StopAllCoroutines();
            transform.position = originalPos;
        }

        StartCoroutine(ShakeCoroutine(distance, direction));
    }

    private IEnumerator ShakeCoroutine(float distance, Vector2 direction)
    {
        isShaking = true;
        direction = direction.normalized;
        float startTime = Time.time;
        Vector2 startPos = transform.position;
        while (Time.time < startTime + ShakeDuration / 2)
        {
            transform.position = startPos + direction * (distance * (Time.time - startTime) / (ShakeDuration / 2));
            yield return null;
        }

        while (Time.time < startTime + ShakeDuration)
        {
            transform.position = startPos
                + direction * (distance * (1 - (Time.time - (startTime + ShakeDuration / 2)) / (ShakeDuration / 2)));
            yield return null;
        }

        transform.position = startPos;
        isShaking = false;
    }

    [Serializable]
    public struct ItemDrop
    {
        public string itemId;
        public int maxQuantity;
        public float dropProbability; // the likelihood of a drop for each item in maxQuantity
    }

    [Serializable]
    public struct DamageMultiplier
    {
        public ImpactInfo.DamageType damageType;
        public float damageMultiplier;
    }
}
