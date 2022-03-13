using System;
using System.Linq;
using ActorAnim.Animation;
using ContentLibraries;
using Items;
using MyBox;
using UnityEngine;

namespace ActorAnim
{
	/// A sprite controller that uses 4-frame walk and single-frame attack animations,
	/// and can render a full set of clothing sprites.
	public class ClothedAnimatedSpriteController : IActorSpriteController
	{
		/// On these sprite indices we lower the hat, hair, and shirt by a pixel, if
		/// the current race supports that behaviour.
		private static readonly int[] HeadDropIndices = { 4, 6, 8, 10, 12, 14, 16, 18 };

		/// Which sorting layer clothes sprites are on
		private const string ClothesSortingLayer = "Entities";

		/// How long the attack frame is held
		private const float AttackFrameDuration = 0.25f;

		private readonly Actor actor;
		private readonly ActorAnimController animController;
		private readonly GameObject spriteParent;
		private readonly SpriteRenderer bodyRenderer;
		private readonly SpriteRenderer swooshRenderer;
		private readonly SpriteRenderer hairRenderer;
		private readonly SpriteRenderer hatRenderer;
		private readonly SpriteRenderer shirtRenderer;
		private readonly SpriteRenderer pantsRenderer;

		/// Regular positions of clothes sprites
		private readonly Vector2 normalHatPos, normalHairPos, normalShirtPos;

		private Sprite[] bodySprites = null;
		private Sprite[] swooshSprites = null;
		private Sprite[] hairSprites = null;
		private Sprite[] hatSprites = null;
		private Sprite[] shirtSprites = null;
		private Sprite[] pantsSprites = null;

		private bool spritesHaveBeenSet = false;
		private bool bounceUpperSprites; // Lower the upper body during certain frames?

		private bool forceUnconsciousSprite = false;
		private bool forceHoldDirection;
		private Direction heldDirection;
		private int lastWalkFrame = 0;

		public ClothedAnimatedSpriteController(Actor actor, RuntimeAnimatorController animatorController)
		{
			this.actor = actor;
			animController = actor.GetOrAddComponent<ActorAnimController>();
			animController.Init(this, animatorController);

			// Create clothing sprite gameobjects
			spriteParent = new GameObject("Sprites");
			spriteParent.transform.SetParent(actor.transform);
			spriteParent.transform.localPosition = Vector3.zero;

			GameObject bodySprite = new("Body");
			bodySprite.transform.SetParent(spriteParent.transform);
			bodySprite.transform.localPosition = Vector3.zero;
			bodyRenderer = bodySprite.AddComponent<SpriteRenderer>();
			bodyRenderer.sortingLayerName = ClothesSortingLayer;

			GameObject pantsSprite = new("Pants");
			pantsSprite.transform.SetParent(spriteParent.transform);
			pantsSprite.transform.localPosition = Vector3.zero;
			pantsRenderer = pantsSprite.AddComponent<SpriteRenderer>();
			pantsRenderer.sortingLayerName = ClothesSortingLayer;
			pantsRenderer.sortingOrder = 1;

			GameObject shirtSprite = new("Shirt");
			shirtSprite.transform.SetParent(spriteParent.transform);
			shirtSprite.transform.localPosition = Vector3.zero;
			shirtRenderer = shirtSprite.AddComponent<SpriteRenderer>();
			shirtRenderer.sortingLayerName = ClothesSortingLayer;
			shirtRenderer.sortingOrder = 2;

			GameObject hairSprite = new("Hair");
			hairSprite.transform.SetParent(spriteParent.transform);
			hairSprite.transform.localPosition = Vector3.zero;
			hairRenderer = hairSprite.AddComponent<SpriteRenderer>();
			hairRenderer.sortingLayerName = ClothesSortingLayer;
			hairRenderer.sortingOrder = 3;

			GameObject hatSprite = new("Hat");
			hatSprite.transform.SetParent(spriteParent.transform);
			hatSprite.transform.localPosition = Vector3.zero;
			hatRenderer = hatSprite.AddComponent<SpriteRenderer>();
			hatRenderer.sortingLayerName = ClothesSortingLayer;
			hatRenderer.sortingOrder = 4;

			GameObject swooshSprite = new("Swoosh");
			swooshSprite.transform.SetParent(spriteParent.transform);
			swooshSprite.transform.localPosition = Vector3.zero;
			swooshRenderer = swooshSprite.AddComponent<SpriteRenderer>();
			swooshRenderer.sortingLayerName = ClothesSortingLayer;
			swooshRenderer.sortingOrder = 5;

			normalHatPos = hatRenderer.transform.localPosition;
			normalHairPos = hairRenderer.transform.localPosition;
			normalShirtPos = shirtRenderer.transform.localPosition;
		}

		public void UpdateSprites()
		{
			UpdateSprites(null);
		}

		public void UpdateSprites(Direction forced)
		{
			UpdateSprites(new Direction?(forced));
		}

		public void UpdateSprites(Direction? forcedDirection)
		{
			UpdateSpriteArrays();
			bounceUpperSprites = ContentLibrary.Instance.Races.Get(actor.GetData().RaceId).BounceUpperSprites;
			forceHoldDirection = forcedDirection.HasValue;
			heldDirection = forcedDirection.GetValueOrDefault();

			if (actor.GetData().Health.Sleeping || actor.GetData().Health.IsDead)
			{
				SetSpriteUnconscious(true);
				return;
			}

			SetSpriteUnconscious(false);

			if (actor.WalkVector.magnitude != 0)
			{
				animController.SetDirection(actor.WalkVector.ToDirection());
				if (forcedDirection.HasValue) animController.SetDirection(forcedDirection.Value);
				animController.SetWalking(true);
			}
			else
			{
				if (forcedDirection.HasValue) animController.SetDirection(forcedDirection.Value);
				animController.SetWalking(false);
				if (!animController.InPunchAnim) SetFrame(lastWalkFrame);
			}
		}

		/// If true, the actor will render as unconscious, regardless of state.
		/// If given value is false, returns this actor to an upright position.
		private void SetSpriteUnconscious(bool value)
		{
			forceUnconsciousSprite = value;
			if (value)
			{
				float spriteRotation = 90;

				if (actor.GetData().Health.Sleeping)
				{
					IBed bed = actor.GetData().Health.CurrentBed;
					spriteRotation = bed.SpriteRotation;
				}

				SwitchToUnconsciousSprite();
				SetSpriteRotation(spriteRotation);
			}
			else
			{
				SetSpriteRotation(0);
			}
		}

		/// Sets the current sprites to punch sprites unless the actor is unconscious.
		public void ShowPunchSprites()
		{
			if (forceUnconsciousSprite)
			{
				SwitchToUnconsciousSprite();
				return;
			}

			switch (animController.GetPunchDirection())
			{
				case Direction.Down:
					SetCurrentBodySpriteIndex(20);
					break;
				case Direction.Up:
					SetCurrentBodySpriteIndex(21);
					break;
				case Direction.Left:
					SetCurrentBodySpriteIndex(22);
					break;
				case Direction.Right:
					SetCurrentBodySpriteIndex(23);
					break;
			}

			ShowSwooshSprite(animController.GetPunchDirection());
			SetHeadSpritesFromDirection(animController.GetPunchDirection());
		}

		/// Animation frames number 0-4, where 0 indicates standing still, and 1-4 indicate
		/// the respective frames of the walk animation.
		public void SetFrame(int animFrame)
		{
			bool isNewFrame = animFrame != lastWalkFrame;

			lastWalkFrame = animFrame;
			if (!spritesHaveBeenSet)
				return;

			// When the actor is standing still
			if (animFrame == 0)
			{
				SetCurrentBodySpriteIndex((int)CurrentDirection);
			}
			else
			{
				SetCurrentBodySpriteIndex((animFrame + 3) + 4 * (int)CurrentDirection);

				// Play footstep sound if we're on an odd frame and this is a new footstep
				if (isNewFrame && animFrame % 2 == 1)
				{
					FootstepSoundPlayer.PlayRandomFootstep(actor.gameObject);
				}
			}

			// Hair and hats don't change with walking animations
			SetHeadSpritesFromDirection(CurrentDirection);

			HideSwooshSprite();

			if (forceUnconsciousSprite)
			{
				SwitchToUnconsciousSprite();
			}
		}

		/// The direction this Actor's sprites are currently facing.
		public Direction CurrentDirection => forceHoldDirection ? heldDirection : animController.GetDirection();

		public void StartAttackAnim(Vector2 direction)
		{
			animController.PlayPunchAnim(AttackFrameDuration, direction.ToDirection());
		}

		private void ShowSwooshSprite(Direction dir)
		{
			if (swooshSprites == null || swooshSprites.Length < 4) return;
			swooshRenderer.sprite = swooshSprites[(int)dir];
		}

		private void HideSwooshSprite()
		{
			swooshRenderer.sprite = null;
		}

		private void SwitchToUnconsciousSprite()
		{
			SetCurrentBodySpriteIndex(3);
			SetHeadSpritesFromDirection(Direction.Right);
			HideSwooshSprite();
		}

		/// Sets the sprites for body, shirt, and pants based on the provided index,
		/// and adjusts the vertical position of upper sprites on the appropriate frames.
		private void SetCurrentBodySpriteIndex(int spriteIndex)
		{
			if (bodySprites.Length > spriteIndex) bodyRenderer.sprite = bodySprites[spriteIndex];
			if (shirtSprites.Length > spriteIndex) shirtRenderer.sprite = shirtSprites[spriteIndex];
			if (pantsSprites.Length > spriteIndex) pantsRenderer.sprite = pantsSprites[spriteIndex];

			// Bounce the upper body sprites when appropriate
			if (bounceUpperSprites && HeadDropIndices.Contains(spriteIndex))
			{
				float pixelSize = 1f / bodyRenderer.sprite.pixelsPerUnit;
				hatRenderer.transform.localPosition = normalHatPos + Vector2.down * pixelSize;
				hairRenderer.transform.localPosition = normalHairPos + Vector2.down * pixelSize;
				shirtRenderer.transform.localPosition = normalShirtPos + Vector2.down * pixelSize;
			}
			// Otherwise set those sprites to their normal positions
			else
			{
				hatRenderer.transform.localPosition = normalHatPos;
				hairRenderer.transform.localPosition = normalHairPos;
				shirtRenderer.transform.localPosition = normalShirtPos;
			}
		}

		private void SetCurrentHatSpriteIndex(int spriteIndex)
		{
			hatRenderer.sprite = hatSprites[spriteIndex];
		}

		private void SetCurrentHairSpriteIndex(int spriteIndex)
		{
			hairRenderer.sprite = hairSprites[spriteIndex];
		}

		private void SetHeadSpritesFromDirection(Direction dir)
		{
			switch (dir)
			{
				case Direction.Down:
					SetCurrentHatSpriteIndex(0);
					SetCurrentHairSpriteIndex(0);
					break;
				case Direction.Up:
					SetCurrentHatSpriteIndex(1);
					SetCurrentHairSpriteIndex(1);
					break;
				case Direction.Left:
					SetCurrentHatSpriteIndex(2);
					SetCurrentHairSpriteIndex(2);
					break;
				case Direction.Right:
					SetCurrentHatSpriteIndex(3);
					SetCurrentHairSpriteIndex(3);
					break;
				default:
					Debug.LogError("Invalid direction: " + dir);
					break;
			}
		}

		/// Sets the rotation of all of this actor's sprites.
		private void SetSpriteRotation(float degrees)
		{
			Vector3 oldRot = spriteParent.transform.rotation.eulerAngles;
			spriteParent.transform.rotation = Quaternion.Euler(oldRot.x, oldRot.y, degrees);
		}

		private void SetSpriteParentOffset(Vector2 offset)
		{
			// TODO for proper positioning when unconscious
		}

		/// Updates the sprite arrays to match the current actor's appearance.
		public void UpdateSpriteArrays()
		{
			ActorData data = actor.GetData();
			string raceId = data.RaceId;
			string hairId = data.Hair;
			string hatId = data.Inventory?.EquippedHat?.Id;
			string shirtId = data.Inventory?.EquippedShirt?.Id;
			string pantsId = data.Inventory?.EquippedPants?.Id;

			bodySprites = Array.Empty<Sprite>();
			swooshSprites = Array.Empty<Sprite>();
			hairSprites = new Sprite[4];
			hatSprites = new Sprite[4];
			shirtSprites = new Sprite[12];
			pantsSprites = new Sprite[12];

			ActorRace race = ContentLibrary.Instance.Races.Get(raceId);
			if (race != null)
			{
				bodySprites = ContentLibrary.Instance.Races.Get(raceId).BodySprites.ToArray();
				swooshSprites = ContentLibrary.Instance.Races.Get(raceId).SwooshSprites.ToArray();
			}
			else {
				Debug.LogWarning("No race found for race ID " + raceId);
			}

			if (hairId != null)
			{
				Hair hair = ContentLibrary.Instance.Hairs.Get(hairId);
				if (hair != null) hairSprites = hair.sprites;
			}
			if (hatId != null)
			{
				if (ContentLibrary.Instance.Items.Get(hatId) is IHat)
				{
					IHat hat = (IHat)ContentLibrary.Instance.Items.Get(hatId);
					if (hat != null) hatSprites = hat.GetHatSprites();
				}
			}
			if (shirtId != null)
			{
				if (ContentLibrary.Instance.Items.Get(shirtId) is Shirt)
				{
					Shirt shirt = (Shirt)ContentLibrary.Instance.Items.Get(shirtId);
					if (shirt != null) shirtSprites = shirt.GetShirtSprites();
				}
			}
			if (pantsId != null)
			{
				if (ContentLibrary.Instance.Items.Get(pantsId) is Pants)
				{
					Pants pants = (Pants)ContentLibrary.Instance.Items.Get(pantsId);
					if (pants != null) pantsSprites = pants.GetPantsSprites();
				}
			}

			spritesHaveBeenSet = true;
		}
	}
}
