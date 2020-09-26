﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Describes a physical gameobject for a dropped item
public class DroppedItem : MonoBehaviour, IPickuppable
{
	[SerializeField] private SpriteRenderer spriteRenderer = null;

	private const float fallAcceleration = 9.8f;

	private Item item;
	public Item Item { get { return item; } }

	bool IPickuppable.CurrentlyPickuppable => true;

	Item IPickuppable.ItemPickup => item;

	public void SetItem (Item item) {
		this.item = item;
		if (item.GetData() != null)
		{
			spriteRenderer.sprite = item.GetData().ItemIcon;
		}
		else
		{
			Debug.LogError ("someone tried to set a dropped item to a nonexistent item id");
		}
	}

	// Make the item appear to fall down a distance
	public void InitiateFakeFall (float distance) {
		float horizontalVelocity = Random.Range (-1f, 1f);
		StartCoroutine (FakeFallCoroutine (distance, horizontalVelocity));
	}

	private IEnumerator FakeFallCoroutine (float distance, float horizontalVelocity) {
		float targetY = transform.position.y - distance;
		float targetZ = transform.position.z;

		float remainingDist = distance;
		float startTime = Time.time;
		while (remainingDist > 0) {
			float currentVelocity = fallAcceleration * (Time.time - startTime);
			float distToFall = currentVelocity * Time.deltaTime;
			float distHorizontal = horizontalVelocity * Time.deltaTime;
			// Calculate a z pos that gives us the same sprite sort value as where we're falling to
			float newZPos = targetY + targetZ - (transform.position.y - distToFall);

			this.transform.position = new Vector3 (transform.position.x + distHorizontal, transform.position.y - distToFall, newZPos);
			remainingDist -= distToFall;
			yield return null;
		}
		// Set the z pos to the target to avoid z-fighting issues with multiple objects
		transform.position = new Vector3 (transform.position.x, transform.position.y, targetZ);
	}
}