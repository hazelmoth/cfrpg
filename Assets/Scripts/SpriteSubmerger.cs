using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SpriteSubmerger : MonoBehaviour
{
	private const float MASK_WIDTH = 2.5f;
	private const float SUBMERGE_DIST = 0.625f;

	[SerializeField] private float checkRadius = 0.4f;
	[SerializeField] private Sprite maskSprite;
	
	[SerializeField] private GameObject spriteParentObject;
	[SerializeField] private List<SpriteRenderer> sprites;
	[SerializeField] private List<SpriteRenderer> shadowSprites;

	private Actor actor;
	private SpriteMask spriteMask;
	private GameObject maskObject;
	private bool isSubmerged;
	private FallAnimator.FallingObject fallingObject;
	private float startHeight;

    // Start is called before the first frame update
    private void Start()
    {
		actor = GetComponent<Actor>();

		if (GetComponent<SortingGroup>() == null)
		{
			SortingGroup group = gameObject.AddComponent<SortingGroup>();
			group.sortingLayerName = "Entities";
		}

		foreach (SpriteRenderer sprite in sprites) {
			sprite.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
		}

		startHeight = spriteParentObject.transform.localPosition.y;
    }

    // Update is called once per frame
    private void Update()
    {
		SetSubmerged(IsOverWater);
    }

    private void SetSubmerged (bool doSubmerge) {
		if (spriteMask == null)
		{
			maskObject = Instantiate(new GameObject(), this.transform);
			maskObject.transform.localScale = new Vector3(MASK_WIDTH, SUBMERGE_DIST * 2, 1);
			maskObject.transform.Translate(0, -1f * SUBMERGE_DIST, 0);
			spriteMask = maskObject.AddComponent<SpriteMask>();
			spriteMask.sprite = maskSprite;
			maskObject.SetActive(false);
		}

        if (doSubmerge == isSubmerged)
        {
			return;
        }

		if (doSubmerge)
		{
			LowerSprites();
			SetShadowsVisible(false);
			isSubmerged = true;
		}
		else
		{
			RaiseSprites();
			SetShadowsVisible(true);
			isSubmerged = false;
		}
	}

    private void LowerSprites ()
	{
		if (fallingObject != null)
		{
			FallAnimator.CancelFall(fallingObject);
			fallingObject = null;
		}
		maskObject.SetActive(true);

		// Calculate the distance in case we're already partially submerged
		float dist = SUBMERGE_DIST - (startHeight - spriteParentObject.transform.localPosition.y);
		fallingObject = FallAnimator.AnimateFall(spriteParentObject, dist, 2f, null);
	}

    private void RaiseSprites()
	{
		if (fallingObject != null)
		{
			FallAnimator.CancelFall(fallingObject);
			fallingObject = null;
		}
		float dist = startHeight - spriteParentObject.transform.localPosition.y;
		fallingObject = FallAnimator.AnimateFall(spriteParentObject, -1f * dist, -14f, () => maskObject.SetActive(false));
	}

    private void SetShadowsVisible (bool visible)
	{
		foreach (SpriteRenderer sprite in shadowSprites)
		{
			sprite.enabled = visible;
		}
	}

    private bool IsOverWater
	{
		get
		{
			string scene;
			if (actor != null)
			{
				scene = actor.CurrentScene;
			}
			else
			{
				scene = SceneObjectManager.GetSceneIdForObject(this.gameObject);
			}

			bool WaterAtPoint(Vector2 offsetFromPosition)
			{
				GroundMaterial ground = WorldMapManager.GetGroundMaterialtAtPoint((transform.position.ToVector2() + offsetFromPosition).ToVector2Int(), scene);
				if (ground == null)
				{
					return false;
				}
				return ground.isWater;
			}

			if (!WaterAtPoint(Vector2.zero))
				return false;

			Vector2 pos = transform.position;
			Vector2 posInTile = new Vector2(pos.x % 1, pos.y % 1);


			bool waterTopLeft = WaterAtPoint((Vector2.up + Vector2.left) * checkRadius);
			bool waterTopRight = WaterAtPoint((Vector2.up + Vector2.right) * checkRadius);
			bool waterBottomLeft = WaterAtPoint((Vector2.down + Vector2.left) * checkRadius);
			bool waterBottomRight = WaterAtPoint((Vector2.down + Vector2.right) * checkRadius);

			return waterTopLeft && waterTopRight && waterBottomLeft && waterBottomRight;
		}
	}
}
