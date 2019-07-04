using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SpriteSubmerger : MonoBehaviour
{
	[SerializeField] float checkRadius = 0.5f;
	[SerializeField] Sprite maskSprite;
	[SerializeField] float submergeDist = 0.6f;
	[SerializeField] List<SpriteRenderer> sprites;
	[SerializeField] List<SpriteRenderer> shadowSprites;

	private Actor actor;
	private SpriteMask spriteMask;
	private GameObject maskObject;
	private bool isSubmerged;

    // Start is called before the first frame update
    void Start()
    {
		actor = GetComponent<Actor>();
		foreach (SpriteRenderer sprite in sprites) {
			sprite.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
		}
    }

    // Update is called once per frame
    void Update()
    {
		SetSubmerged(IsOverWater);
    }

	void SetSubmerged (bool submerged) {
		if (spriteMask == null)
		{
			maskObject = GameObject.Instantiate(new GameObject(), this.transform);
			maskObject.transform.localScale = new Vector3(maskObject.transform.localScale.x, submergeDist, maskObject.transform.localScale.y);
			maskObject.transform.Translate(0, -1f * submergeDist / 2, submergeDist);
			spriteMask = maskObject.AddComponent<SpriteMask>();
			spriteMask.sprite = maskSprite;
			maskObject.SetActive(false);
		}
		if (GetComponent<SortingGroup>() == null)
		{
			SortingGroup group = gameObject.AddComponent<SortingGroup>();
			group.sortingLayerName = "Entities";
		}
		if (submerged)
		{
			if (isSubmerged)
				return;
			else
			{
				maskObject.SetActive(true);
				LowerSprites();
				SetShadowsVisible(false);
				isSubmerged = true;
			}
		}
		else
		{
			if (!isSubmerged)
				return;
			else
			{
				maskObject.SetActive(false);
				RaiseSprites();
				SetShadowsVisible(true);
				isSubmerged = false;
			}
		}
	}
	void LowerSprites ()
	{
		foreach (SpriteRenderer sprite in sprites)
		{
			sprite.transform.Translate(0f, -1f * submergeDist, submergeDist);
		}
	}
	void RaiseSprites()
	{
		foreach (SpriteRenderer sprite in sprites)
		{
			sprite.transform.Translate(0f, submergeDist, -1 * submergeDist);
		}
	}
	void SetShadowsVisible (bool visible)
	{
		foreach (SpriteRenderer sprite in shadowSprites)
		{
			sprite.enabled = visible;
		}
	}
	bool IsOverWater
	{
		get
		{
			string scene;
			if (actor != null)
			{
				scene = actor.ActorCurrentScene;
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

			bool leftClear = posInTile.x > checkRadius || WaterAtPoint(Vector2.left);
			bool upClear = posInTile.y < 1 - checkRadius || WaterAtPoint(Vector2.up);
			bool rightClear = posInTile.x < 1 - checkRadius || WaterAtPoint(Vector2.right);
			bool downClear = posInTile.y > checkRadius || WaterAtPoint(Vector2.down);

			return leftClear && upClear && rightClear && downClear;
		}
	}
}
