using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EquipmentRenderer
{
	private const string SpriteParentName = "Sprites";

	private static Dictionary<int, GameObject> currentlyRendered;

	public static void PointItem(Actor actor, Gun item, float angleFromRight, bool flipOnLeft)
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

		renderObject.transform.localPosition = ContentLibrary.Instance.Races.GetById(actor.Race).GetItemPosition(actor.Direction);
		renderObject.transform.rotation = Quaternion.AngleAxis(angleFromRight, Vector3.forward);

		if (actor.Direction == Direction.Up)
		{
			renderComponent.sortingOrder = -10;
		}
		else
		{
			renderComponent.sortingOrder = 10;
		}

		renderComponent.flipY = (actor.Direction != Direction.Right && flipOnLeft);


		Sprite sprite = item.gunSprite;
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

	private static void initIfNotInited()
	{
		if (currentlyRendered == null)
		{
			currentlyRendered = new Dictionary<int, GameObject>();
			SceneChangeActivator.OnSceneExit += () =>
			{
				currentlyRendered = null;
			};
		}
	}
}
