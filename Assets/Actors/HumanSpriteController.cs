using UnityEngine;

// Sets the right sprite for the NPC based off of what the animator and Anim Controller are doing
public class HumanSpriteController : MonoBehaviour {

	[SerializeField] SpriteRenderer bodyRenderer = null;
	[SerializeField] SpriteRenderer swooshRenderer = null;
    [SerializeField] SpriteRenderer hairRenderer = null;
	[SerializeField] SpriteRenderer hatRenderer = null;
	[SerializeField] SpriteRenderer shirtRenderer = null;
	[SerializeField] SpriteRenderer pantsRenderer = null;
	HumanAnimController animController;
	Sprite[] bodySprites = null;
	Sprite[] swooshSprites = null;
    Sprite[] hairSprites = null;
	Sprite[] hatSprites = null;
	Sprite[] shirtSprites = null;
	Sprite[] pantsSprites = null;

    bool spritesHaveBeenSet = false;
	bool forceUnconsciousSprite = false;
    int lastWalkFrame = 0;

	void Awake ()
	{
		animController = GetComponent<HumanAnimController> ();
	}


	public Sprite CurrentBodySprite => bodyRenderer.sprite;
	public Sprite CurrentSwooshSprite => swooshRenderer.sprite;
	public Sprite CurrentHairSprite => hairRenderer.sprite;
	public Sprite CurrentHatSprite => hatRenderer.sprite;
	public Sprite CurrentShirtSprite => shirtRenderer.sprite;
	public Sprite CurrentPantsSprite => pantsRenderer.sprite;
	public bool ForceUnconsciousSprite
	{
		get
		{
			return forceUnconsciousSprite;
		}
		set
		{
			forceUnconsciousSprite = value;
			if (value)
				SwitchToUnconsciousSprite();
		}
	}

	public void SetBodySpriteArray (Sprite[] sprites)
	{
		this.bodySprites = sprites;
	}
	// This needs to be updated whenever the NPC's clothes change
	public void SetSpriteArrays (Sprite[] bodySprites, Sprite[] swooshSprites, Sprite[] hairSprites, Sprite[] hatSprites, Sprite[] shirtSprites, Sprite[] pantsSprites)
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
				SetCurrentBodySpriteIndex(12);
				break;
			case Direction.Right:
				SetCurrentBodySpriteIndex(13);
				break;
			case Direction.Up:
				SetCurrentBodySpriteIndex(15);
				break;
			case Direction.Left:
				SetCurrentBodySpriteIndex(14);
				break;
		}

		ShowSwooshSprite(animController.GetPunchDirection());
		SetHeadSpritesFromDirection(animController.GetPunchDirection());
	}

	// Called by animation events
	public void SetFrame (int animFrame)
	{
		lastWalkFrame = animFrame;
        if (!spritesHaveBeenSet)
            return;
		// When the actor is standing still
		if (animFrame == 0) { 
			switch (animController.GetDirection ()) {
			case Direction.Down:
				SetCurrentBodySpriteIndex (0);
				break;
			case Direction.Right:
				SetCurrentBodySpriteIndex (1);
				break;
			case Direction.Up:
				SetCurrentBodySpriteIndex (3);
				break;
			case Direction.Left:
				SetCurrentBodySpriteIndex (2);
				break;
			}
		}
		// First walking frame
		else if (animFrame == 1) {
			switch (animController.GetDirection ()) {
			case Direction.Down:
				SetCurrentBodySpriteIndex (4);
				break;
			case Direction.Right:
				SetCurrentBodySpriteIndex (8);
				break;
			case Direction.Up:
				SetCurrentBodySpriteIndex (6);
				break;
			case Direction.Left:
				SetCurrentBodySpriteIndex (10);
				break;
			}
		}
		// Second walking frame
		else /*if (animFrame == 2)*/ {
			switch (animController.GetDirection ()) {
			case Direction.Down:
				SetCurrentBodySpriteIndex (5);
				break;
			case Direction.Right:
				SetCurrentBodySpriteIndex (9);
				break;
			case Direction.Up:
				SetCurrentBodySpriteIndex (7);
				break;
			case Direction.Left:
				SetCurrentBodySpriteIndex (11);
				break;
			}
		}
        // Hair and hats don't change with walking animations
		SetHeadSpritesFromDirection (animController.GetDirection ());

		HideSwooshSprite();

		if (forceUnconsciousSprite)
		{
			SwitchToUnconsciousSprite();
		}
	}

	void ShowSwooshSprite (Direction dir)
	{
		switch (animController.GetDirection())
		{
			case Direction.Down:
				swooshRenderer.sprite = swooshSprites[0];
				break;
			case Direction.Right:
				swooshRenderer.sprite = swooshSprites[1];
				break;
			case Direction.Up:
				swooshRenderer.sprite = swooshSprites[3];
				break;
			case Direction.Left:
				swooshRenderer.sprite = swooshSprites[2];
				break;
		}
	}

	void HideSwooshSprite ()
	{
		swooshRenderer.sprite = null;
	}

	void SwitchToUnconsciousSprite()
	{
		SetCurrentBodySpriteIndex(16);
		hatRenderer.sprite = null;
		hairRenderer.sprite = null;
		shirtRenderer.sprite = null;
		pantsRenderer.sprite = null;
		HideSwooshSprite();
	}
	void SetCurrentBodySpriteIndex (int spriteIndex)
	{
		if (bodySprites.Length > spriteIndex)
			bodyRenderer.sprite = bodySprites [spriteIndex];
		if (shirtSprites.Length > spriteIndex)
			shirtRenderer.sprite = shirtSprites [spriteIndex];
		if (pantsSprites.Length > spriteIndex)
			pantsRenderer.sprite = pantsSprites [spriteIndex];
	}
	void SetCurrentHatSpriteIndex (int spriteIndex)
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
    void SetCurrentHairSpriteIndex (int spriteIndex)
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
    void SetHeadSpritesFromDirection (Direction dir) {
		if (dir == Direction.Down) {
			SetCurrentHatSpriteIndex (0);
            SetCurrentHairSpriteIndex (0);
		}
		else if (dir == Direction.Right) {
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
}