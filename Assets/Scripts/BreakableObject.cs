using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour, IImpactReceiver
{
	public delegate void ObjectBreakEvent();
	public delegate void ObjectDropItemsEvent(List<DroppedItem> items);
	public event ObjectBreakEvent OnObjectBreak;
	public event ObjectDropItemsEvent OnDropItems;

	[System.Serializable]
	public struct ItemDrop
	{
		public string itemId;
		public int maxQuantity;
		public float dropProbability; // the likelihood of a drop for each item in maxQuantity
	}
	[SerializeField] private float maxHealth = 1.0f;
	[SerializeField] private List<ItemDrop> itemDrops = new List<ItemDrop>();

	public bool enableDrops = true;

	private float currentHealth;
	private bool hasBeenHit = false;

	private Vector2 originalPos;
	private bool isShaking = false;
	private const float shakeDuration = 0.1f;
	private const float shakeDistance = 0.13f;

	// So derived classes can raise this event
	protected void RaiseDropItemsEvent (List<DroppedItem> items)
	{
		OnDropItems?.Invoke(items);
	}

	void IImpactReceiver.OnImpact(Vector2 impact)
	{
		if (!enabled)
		{
			return;
		}

		if (!hasBeenHit)
		{
			currentHealth = maxHealth;
			originalPos = transform.position;
		}
		hasBeenHit = true;
		currentHealth -= impact.magnitude;

		// break
		if (currentHealth <= 0) {
			Break();
		} else {
			Shake(shakeDistance, impact.normalized);
		}
	}

	public void Break()
	{
		EntityObject entity = GetComponent<EntityObject>();

		OnObjectBreak?.Invoke();
		if (enableDrops)
		{
			DropItems();
		}

		// If this is an entity, remove it through WorldMapManager; otherwise, just destroy it
		if (entity != null)
		{
			Vector2 localPos = TilemapInterface.WorldPosToScenePos(transform.position, SceneObjectManager.WorldSceneId);
			RegionMapManager.RemoveEntityAtPoint(TilemapInterface.FloorToTilePos(localPos), entity.Scene);
		}
		else
		{
			Debug.LogWarning("destroyed some non-entity thing!");
			Destroy(this.gameObject);
		}
	}

	protected virtual void DropItems ()
	{
		List<DroppedItem> droppedItems = new List<DroppedItem>();
		foreach (ItemDrop drop in itemDrops)
		{
			for (int i = 0; i < drop.maxQuantity; i++)
			{
				if (Random.value < drop.dropProbability)
				{
					float dropHeight = 0.5f;
					Vector2 dropPosition = new Vector2(transform.localPosition.x, transform.localPosition.y + dropHeight);
					DroppedItem item = DroppedItemSpawner.SpawnItem(new ItemStack(drop.itemId, 1), dropPosition, SceneObjectManager.WorldSceneId);
					item.InitiateFakeFall(dropHeight);
					droppedItems.Add(item);
				}
			}
		}
		RaiseDropItemsEvent(droppedItems);
	}

	private void Shake(float distance, Vector2 direction)
	{
		if (!isShaking) {
			originalPos = transform.position;
		} else {
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
		while (Time.time < startTime + shakeDuration / 2)
		{
			transform.position = startPos + direction * (distance * (Time.time - startTime) / (shakeDuration / 2));
			yield return null;
		}
		while (Time.time < startTime + shakeDuration)
		{
			transform.position = startPos + direction * (distance * (1 - (Time.time - (startTime + shakeDuration / 2)) / (shakeDuration / 2)));
			yield return null;
		}
		transform.position = startPos;
		isShaking = false;
		yield break;
	}
}
