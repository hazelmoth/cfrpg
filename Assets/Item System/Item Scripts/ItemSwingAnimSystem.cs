using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemSwingAnimSystem
{
	// angles for facing right
	private const float START_ANGLE = -45;
	private const float END_ANGLE = -135;
	// The name of the object containing actors' sprites must be named this
	// for held items to follow those sprites properly
	private const string SPRITE_PARENT_NAME = "Sprites";

	// TODO held item offsets should be held in data for body sprite
	static Dictionary<Direction, Vector2> itemSpriteOffsets = new Dictionary<Direction, Vector2>
	{
		{ Direction.Right, new Vector2(0.3f, 0.8f) },
		{ Direction.Left, new Vector2(-0.3f,  0.8f) },
		{ Direction.Down, new Vector2(-0.3f,  0.8f) },
		{ Direction.Up, new Vector2(0.3f,  0.8f) }
	};
	static Dictionary<Direction, float> angleOffsets = new Dictionary<Direction, float>
	{
		{ Direction.Right, 0 },
		{ Direction.Left, 0 },
		{ Direction.Down, -90 },
		{ Direction.Up, 90 }
	};
	static Dictionary<Direction, bool> flipAngles = new Dictionary<Direction, bool>
	{
		{ Direction.Right, false },
		{ Direction.Left, true },
		{ Direction.Down, false },
		{ Direction.Up, false }
	};
	static Dictionary<Direction, bool> reverseSwing = new Dictionary<Direction, bool>
	{
		{ Direction.Right, false },
		{ Direction.Left, false },
		{ Direction.Down, true },
		{ Direction.Up, true }
	};
	static Dictionary<Direction, int> sortingOrders = new Dictionary<Direction, int>
	{
		{ Direction.Right, 5 },
		{ Direction.Left, 5 },
		{ Direction.Down, 5 },
		{ Direction.Up, -1 }
	};


	public static void Animate (Sprite itemSprite, Actor actor, float swingDuration, System.Action<Actor> callback, bool callbackOnMidSwing = true)
	{
		actor.StartCoroutine(AnimateCoroutine(itemSprite, actor, swingDuration, callback, callbackOnMidSwing));
	}

	// TODO position the swinging item relative to the body sprite (to handle swimming, etc.)
	static IEnumerator AnimateCoroutine(Sprite itemSprite, Actor actor, float swingDuration, System.Action<Actor> callback, bool callbackOnMidSwing)
	{
		GameObject spriteParentObject = actor.transform.Find(SPRITE_PARENT_NAME).gameObject;

		if (spriteParentObject == null)
		{
			spriteParentObject = new GameObject("Sprites");
			spriteParentObject.transform.SetParent(actor.transform, false);
		}

		GameObject itemSpriteObject = new GameObject("Swinging Item");
		itemSpriteObject.transform.SetParent(spriteParentObject.transform, false);

		SpriteRenderer renderer = itemSpriteObject.AddComponent<SpriteRenderer>();
		renderer.sprite = itemSprite;
		// Set sorting layer to sort with actors
		renderer.sortingLayerName = "Entities";
		
		bool hasCalledCallback = false;

		float startTime = Time.time;
		while (Time.time - startTime < swingDuration)
		{
			renderer.sortingOrder = sortingOrders[actor.Direction];
			itemSpriteObject.transform.localPosition = itemSpriteOffsets[actor.Direction];
			
			float startAngle = ItemSwingAnimSystem.START_ANGLE;
			float endAngle = ItemSwingAnimSystem.END_ANGLE;

			if (reverseSwing[actor.Direction])
			{
				// Switch the values of start and end angles
				startAngle += endAngle;
				endAngle = startAngle - endAngle;
				startAngle -= endAngle;

				renderer.flipX = true;
			}
			if  (flipAngles[actor.Direction])
			{
				startAngle = -startAngle;
				endAngle = -endAngle;

				renderer.flipX = true;
			}
			if (!flipAngles[actor.Direction] && !reverseSwing[actor.Direction])
			{
				renderer.flipX = false;
			}


			float animProgress = (Time.time - startTime) / swingDuration;
			float localAngle = Mathf.Lerp(startAngle, endAngle, animProgress);
			float angle = angleOffsets[actor.Direction] + localAngle;

			itemSpriteObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));

			if (!hasCalledCallback && callbackOnMidSwing && animProgress >= 0.5f)
			{
				callback?.Invoke(actor);
				hasCalledCallback = true;
			}
			yield return null;
		}
		GameObject.Destroy(itemSpriteObject);

		if (!callbackOnMidSwing)
			callback?.Invoke(actor);
	}
}
