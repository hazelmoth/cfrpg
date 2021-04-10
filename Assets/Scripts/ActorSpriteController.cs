using System.Linq;
using ContentLibraries;
using UnityEngine;

// Sets the right sprite for the Actor based off of what the animator and Anim Controller are doing
public class ActorSpriteController : MonoBehaviour {
	
	// On these sprite indices we lower the hat, hair, and shirt by a pixel, if
	// the current race supports that behaviour.
	private static readonly int[] HeadDropIndices = {4, 6, 8, 10, 12, 14, 16, 18};

	[SerializeField] private GameObject spriteParent = null;
	[SerializeField] private SpriteRenderer bodyRenderer = null;
	[SerializeField] private SpriteRenderer swooshRenderer = null;
    [SerializeField] private SpriteRenderer hairRenderer = null;
	[SerializeField] private SpriteRenderer hatRenderer = null;
	[SerializeField] private SpriteRenderer shirtRenderer = null;
	[SerializeField] private SpriteRenderer pantsRenderer = null;
	
	// Regular positions of sprites
	private Vector2 normalHatPos, normalHairPos, normalShirtPos;

	private Actor actor;
	private ActorAnimController animController;
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
	private int lastWalkFrame = 0;
	private Direction heldDirection;

	public Sprite CurrentBodySprite => bodyRenderer.sprite;
	public Sprite CurrentSwooshSprite => swooshRenderer.sprite;
	public Sprite CurrentHairSprite => hairRenderer.sprite;
	public Sprite CurrentHatSprite => hatRenderer.sprite;
	public Sprite CurrentShirtSprite => shirtRenderer.sprite;
	public Sprite CurrentPantsSprite => pantsRenderer.sprite;
	
	private bool FaceTowardsMouse { get; set; }
	
	private void Awake ()
	{
		actor = GetComponent<Actor>();
		animController = GetComponent<ActorAnimController> ();
		bounceUpperSprites = ContentLibrary.Instance.Races.GetById(actor.GetData().Race).BounceUpperSprites;
		
		normalHatPos = hatRenderer.transform.localPosition;
		normalHairPos = hairRenderer.transform.localPosition;
		normalShirtPos = shirtRenderer.transform.localPosition;
	}

	private void Update()
	{
		ForceUnconsciousSprite = actor.GetData().PhysicalCondition.IsDead || actor.GetData().PhysicalCondition.Sleeping;

		if (forceHoldDirection || FaceTowardsMouse)
		{
			SetFrame(lastWalkFrame);
		}
	}

	public bool ForceUnconsciousSprite
	{
		set
		{
			forceUnconsciousSprite = value;
			if (value)
			{
				float spriteRotation = 90;

				if (actor.GetData().PhysicalCondition.Sleeping)
				{
					IBed bed = actor.GetData().PhysicalCondition.CurrentBed;
					spriteRotation = bed.SpriteRotation;
				}

				SwitchToUnconsciousSprite();
				SetSpriteRotation(spriteRotation);
			} 
			else
			{
				SetSpriteRotation(0);
				SetFrame(lastWalkFrame);
			}
		}
	}

	// Sets the Sprites for this actor. This needs to be updated whenever the
	// Actor's clothes change.
	public void SetSpriteArrays(Sprite[] bodySprites, Sprite[] swooshSprites, Sprite[] hairSprites,
								Sprite[] hatSprites, Sprite[] shirtSprites, Sprite[] pantsSprites)
	{
		this.bodySprites = bodySprites;
		this.swooshSprites = swooshSprites;
        this.hairSprites = hairSprites;
		this.hatSprites = hatSprites;
		this.shirtSprites = shirtSprites;
		this.pantsSprites = pantsSprites;
        spritesHaveBeenSet = true;

        SetFrame(lastWalkFrame);
	}

	// Called by animation events
	public void StartPunch ()
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

	// Called by animation events. Animation frames number 0-4, where 0 indicates
	// standing still, and 1-4 indicate the respective frames of the walk animation.
	public void SetFrame (int animFrame)
	{
		bool isNewFrame = animFrame != lastWalkFrame;

		lastWalkFrame = animFrame;
        if (!spritesHaveBeenSet)
            return;

		// When the actor is standing still
		if (animFrame == 0) {
			SetCurrentBodySpriteIndex ((int)CurrentDirection);
		}
		else
		{
			SetCurrentBodySpriteIndex((animFrame + 3) + 4 * (int)CurrentDirection);

			// Play footstep sound if we're on an odd frame and this is a new footstep
			if (isNewFrame && animFrame % 2 == 1)
			{
				FootstepSoundPlayer.PlayRandomFootstep(this.gameObject);
			}
		}

		// Hair and hats don't change with walking animations
		SetHeadSpritesFromDirection (CurrentDirection);

		HideSwooshSprite();

		if (forceUnconsciousSprite)
		{
			SwitchToUnconsciousSprite();
		}
	}

	public Direction CurrentDirection
	{
		get
		{
			if (forceHoldDirection)
			{
				return heldDirection;
			}
			else if (FaceTowardsMouse)
			{
				return DirectionTowardsMouse();
			}
			else
			{
				return animController.GetDirection();
			}
		}
	}

	public void HoldDirection(Direction dir)
	{
		forceHoldDirection = true;
		heldDirection = dir;
	}

	public void StopHoldingDirection()
	{
		forceHoldDirection = false;
	}

	private void ShowSwooshSprite (Direction dir)
	{
		if (swooshSprites == null || swooshSprites.Length < 4) return;
		swooshRenderer.sprite = swooshSprites[(int) dir];
	}

	private void HideSwooshSprite ()
	{
		swooshRenderer.sprite = null;
	}

	private void SwitchToUnconsciousSprite()
	{
		SetCurrentBodySpriteIndex(3);
		SetHeadSpritesFromDirection(Direction.Right);
		HideSwooshSprite();
	}

	private void SetCurrentBodySpriteIndex (int spriteIndex)
	{
		if (bodySprites.Length > spriteIndex) bodyRenderer.sprite = bodySprites [spriteIndex];
		if (shirtSprites.Length > spriteIndex) shirtRenderer.sprite = shirtSprites [spriteIndex];
		if (pantsSprites.Length > spriteIndex) pantsRenderer.sprite = pantsSprites [spriteIndex];

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

	private void SetCurrentHatSpriteIndex (int spriteIndex)
	{
		if (hatSprites[spriteIndex] != null)
		{
			hatRenderer.sprite = hatSprites[spriteIndex];
		}
		else
		{
			hatRenderer.sprite = null;
		}
    }

	private void SetCurrentHairSpriteIndex (int spriteIndex)
    {
        if (hairSprites[spriteIndex] != null)
        {
            hairRenderer.sprite = hairSprites[spriteIndex];
        }
        else
        {
            hairRenderer.sprite = null;
        }
    }

	private void SetHeadSpritesFromDirection (Direction dir) {
		if (dir == Direction.Down) {
			SetCurrentHatSpriteIndex (0);
            SetCurrentHairSpriteIndex (0);
		}
		else if (dir == Direction.Up) {
			SetCurrentHatSpriteIndex (1);
            SetCurrentHairSpriteIndex(1);
        }
		else if (dir == Direction.Left) {
			SetCurrentHatSpriteIndex (2);
            SetCurrentHairSpriteIndex(2);
        }
		else {
			SetCurrentHatSpriteIndex (3);
            SetCurrentHairSpriteIndex(3);
        }
	}

	private void SetSpriteRotation(float degrees)
	{
		Vector3 oldRot = spriteParent.transform.rotation.eulerAngles;
		spriteParent.transform.rotation = Quaternion.Euler(oldRot.x, oldRot.y, degrees);
	}

    private Direction DirectionTowardsMouse()
    {
	    Vector2 vector = MousePositionHelper.GetMouseWorldPos() - (Vector2)transform.position;
	    return vector.ToDirection();
    }
}