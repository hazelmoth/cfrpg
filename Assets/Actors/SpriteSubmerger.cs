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
    private bool isFalling;
    private List<SpriteData> spriteDatas;

    class SpriteData
    {
        public SpriteRenderer sprite;
		public SpriteFallAnimator.FallingSprite fallingSpriteObject;
        public float startHeight;
    }

    // Start is called before the first frame update
    void Start()
    {
        spriteDatas = new List<SpriteData>();
		actor = GetComponent<Actor>();
		foreach (SpriteRenderer sprite in sprites) {
			sprite.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            SpriteData data = new SpriteData();
            data.sprite = sprite;
			// We're assuming that normal sprite local y-pos will never change in-game
            data.startHeight = sprite.transform.localPosition.y;
            spriteDatas.Add(data);
		}
    }

    // Update is called once per frame
    void Update()
    {
		SetSubmerged(IsOverWater);
    }

	void SetSubmerged (bool doSubmerge) {
		if (spriteMask == null)
		{
			maskObject = GameObject.Instantiate(new GameObject(), this.transform);
			maskObject.transform.localScale = new Vector3(maskObject.transform.localScale.x, submergeDist, maskObject.transform.localScale.y);
			maskObject.transform.Translate(0, -1f * submergeDist / 2, 0);
			spriteMask = maskObject.AddComponent<SpriteMask>();
			spriteMask.sprite = maskSprite;
			maskObject.SetActive(false);
		}
		if (GetComponent<SortingGroup>() == null)
		{
			SortingGroup group = gameObject.AddComponent<SortingGroup>();
			group.sortingLayerName = "Entities";
		}
		if (doSubmerge)
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
				//maskObject.SetActive(false);
				RaiseSprites();
				SetShadowsVisible(true);
				isSubmerged = false;
			}
		}
	}
	void LowerSprites ()
	{
		foreach (SpriteData sprite in spriteDatas)
		{
			if (sprite.fallingSpriteObject != null)
			{
				SpriteFallAnimator.CancelFall(sprite.fallingSpriteObject);
				sprite.fallingSpriteObject = null;
			}
			sprite.fallingSpriteObject = SpriteFallAnimator.AnimateFall(sprite.sprite, submergeDist - (sprite.startHeight - sprite.sprite.transform.localPosition.y), 2f);
		}
	}
	void RaiseSprites()
	{
		foreach (SpriteData sprite in spriteDatas)
		{
			if (sprite.fallingSpriteObject != null)
			{
				SpriteFallAnimator.CancelFall(sprite.fallingSpriteObject);
				sprite.fallingSpriteObject = null;
			}
			float dist = sprite.startHeight - sprite.sprite.transform.localPosition.y;
			sprite.fallingSpriteObject = SpriteFallAnimator.AnimateFall(sprite.sprite, -1f * dist, -14f);
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
