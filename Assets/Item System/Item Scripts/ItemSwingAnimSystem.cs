using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemSwingAnimSystem
{
	const float startAngle = -45;
	const float endAngle = -120;

	// TODO held item offsets should be held in data for body sprite
	static Dictionary<Direction, Vector2> itemSpriteOffsets = new Dictionary<Direction, Vector2>
	{
		{ Direction.Right, new Vector2(0.3f, 1f) },
		{ Direction.Left, new Vector2(-0.3f,  1f) },
		{ Direction.Down, new Vector2(-0.3f,  1f-0.3f) },
		{ Direction.Up, new Vector2(0.3f,  1f+0.3f) }
	};

	static Dictionary<Direction, float> angleOffsets = new Dictionary<Direction, float>
	{
		{ Direction.Right, 0 },
		{ Direction.Left, 180 },
		{ Direction.Down, -90 },
		{ Direction.Up, 90 }
	};

	public static void Animate (Sprite itemSprite, Actor actor, float swingDuration, System.Action<Actor> callback, bool callbackOnMidSwing = true)
	{
		actor.StartCoroutine(AnimateCoroutine(itemSprite, actor, swingDuration, callback, callbackOnMidSwing));
	}

	static IEnumerator AnimateCoroutine(Sprite itemSprite, Actor actor, float swingDuration, System.Action<Actor> callback, bool callbackOnMidSwing)
	{
		GameObject spriteParentObject = new GameObject("swinging item");
		spriteParentObject.transform.SetParent(actor.transform, false);

		SpriteRenderer renderer = spriteParentObject.AddComponent<SpriteRenderer>();
		renderer.sprite = itemSprite;
		bool hasCalledCallback = false;

		float startTime = Time.time;
		while (Time.time - startTime < swingDuration)
		{
			spriteParentObject.transform.localPosition = itemSpriteOffsets[actor.Direction];
			float animProgress = (Time.time - startTime) / swingDuration;
			float localAngle = Mathf.Lerp(startAngle, endAngle, animProgress);
			float angle = angleOffsets[actor.Direction] + localAngle;

			spriteParentObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));

			if (!hasCalledCallback && callbackOnMidSwing && animProgress >= 0.5f)
			{
				callback?.Invoke(actor);
				hasCalledCallback = true;
			}
			yield return null;
		}
		GameObject.Destroy(spriteParentObject);

		if (!callbackOnMidSwing)
			callback?.Invoke(actor);
	}
}
