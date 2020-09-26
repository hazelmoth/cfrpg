using Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EquipmentRenderer
{
	private const string SpriteParentName = "Sprites";

	private static Dictionary<int, GameObject> currentlyRendered;
	private static Dictionary<int, float> currentThrustOffsets;

	public static void RenderItem(Actor actor, IPointable item, float angleFromRight, bool flipOnLeft)
	{
		initIfNotInited();
		GameObject renderObject;
		SpriteRenderer renderComponent;

		if (currentlyRendered.ContainsKey(actor.GetInstanceID()))
		{
			renderObject = currentlyRendered[actor.GetInstanceID()];
			renderComponent = renderObject.GetComponent<SpriteRenderer>();
		}
		else
		{
			renderObject = new GameObject("Equipment Sprite");

			GameObject spriteParentObject = actor.transform.Find(SpriteParentName).gameObject;
			if (spriteParentObject == null)
			{
				spriteParentObject = new GameObject(SpriteParentName);
				spriteParentObject.transform.SetParent(actor.transform, false);
			}
			renderObject.transform.SetParent(spriteParentObject.transform, false);

			renderComponent = renderObject.AddComponent<SpriteRenderer>();
			renderComponent.sortingLayerName = "Entities";

			currentlyRendered.Add(actor.GetInstanceID(), renderObject);
		}

		renderObject.transform.localPosition = ContentLibrary.Instance.Races.GetById(actor.GetData().Race).GetItemPosition(actor.Direction);
		// Translate the object if it's currently being thrust
		if (currentThrustOffsets.ContainsKey(actor.GetInstanceID()))
		{
			Vector3 translation = (Quaternion.AngleAxis(angleFromRight, Vector3.forward) * Vector3.right) * currentThrustOffsets[actor.GetInstanceID()];
			renderObject.transform.Translate(translation, Space.World);
		}

		renderObject.transform.rotation = Quaternion.AngleAxis(angleFromRight, Vector3.forward);

		renderComponent.flipY = (actor.Direction != Direction.Right && flipOnLeft);


		if (item.pointDirection == Direction.Up)
		{
			renderObject.transform.Rotate(Vector3.forward, -90);
			if (renderComponent.flipY)
			{
				renderObject.transform.Rotate(Vector3.forward, 180);
			}
		} 
		else if (item.pointDirection == Direction.Left)
		{
			renderObject.transform.Rotate(Vector3.forward, -180);
		} 
		else if (item.pointDirection == Direction.Down)
		{
			renderObject.transform.Rotate(Vector3.forward, -270);
			if (renderComponent.flipY)
			{
				renderObject.transform.Rotate(Vector3.forward, 180);
			}
		}

		if (actor.Direction == Direction.Up)
		{
			renderComponent.sortingOrder = -10;
		}
		else
		{
			renderComponent.sortingOrder = 10;
		}


		Sprite sprite = item.heldItemSprite;
		renderComponent.sprite = sprite;
	}

	public static void StopRendering(Actor actor)
	{
		initIfNotInited();
		if (currentlyRendered.ContainsKey(actor.GetInstanceID()))
		{
			GameObject.Destroy(currentlyRendered[actor.GetInstanceID()]);
			currentlyRendered.Remove(actor.GetInstanceID());
		}
	}

	public static void ThrustItem (Actor actor, float distance, float duration)
	{
		if (currentlyRendered.ContainsKey(actor.GetInstanceID()))
		{
			if (!currentThrustOffsets.ContainsKey(actor.GetInstanceID()) || currentThrustOffsets[actor.GetInstanceID()] == 0f)
			{
				actor.StartCoroutine(ThrustItemCoroutine(actor, distance, duration));
			}
		}
	}
	private static IEnumerator ThrustItemCoroutine(Actor actor, float distance, float duration)
	{
		float startTime = Time.time;
		while (Time.time - startTime < duration / 2f)
		{
			currentThrustOffsets[actor.GetInstanceID()] = (Time.time - startTime) / (duration / 2) * distance;
			yield return null;
		}
		while (Time.time - startTime < duration)
		{
			currentThrustOffsets[actor.GetInstanceID()] = (1 - (((Time.time - startTime) - (duration / 2)) / (duration / 2))) * distance;
			yield return null;
		}
		currentThrustOffsets.Remove(actor.GetInstanceID());
	}

	private static void initIfNotInited()
	{
		if (currentlyRendered == null)
		{
			currentlyRendered = new Dictionary<int, GameObject>();
			currentThrustOffsets = new Dictionary<int, float>();

			SceneChangeActivator.OnSceneExit += () =>
			{
				currentlyRendered = null;
			};
		}
	}
}
