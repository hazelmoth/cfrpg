using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Sets the right sprite for the NPC based off of what the animator and Anim Controller are doing
public class HumanSpriteController : MonoBehaviour {

	[SerializeField] SpriteRenderer spriteRenderer = null;
    [SerializeField] SpriteRenderer hairRenderer = null;
	[SerializeField] SpriteRenderer hatRenderer = null;
	[SerializeField] SpriteRenderer shirtRenderer = null;
	[SerializeField] SpriteRenderer pantsRenderer = null;
	HumanAnimController animController;
	Sprite[] bodySprites = null;
    Sprite[] hairSprites = null;
	Sprite[] hatSprites = null;
	Sprite[] shirtSprites = null;
	Sprite[] pantsSprites = null;

	void Start () {
		animController = this.GetComponent<HumanAnimController> ();
	}

	public void SetBodySpriteArray (Sprite[] sprites) {
		this.bodySprites = sprites;
	}
	// This needs to be updated whenever the NPC's clothes change
	public void SetSpriteArrays (Sprite[] bodySprites, Sprite[] hairSprites, Sprite[] hatSprites, Sprite[] shirtSprites, Sprite[] pantsSprites) {
		this.bodySprites = bodySprites;
        this.hairSprites = hairSprites;
		this.hatSprites = hatSprites;
		this.shirtSprites = shirtSprites;
		this.pantsSprites = pantsSprites;
	}

	// Called by animation events
	public void SetFrame (int animFrame) {
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
		SetHeadSpritesFromDirection (animController.GetDirection ());
	}

	void SetCurrentBodySpriteIndex (int spriteIndex) {
		spriteRenderer.sprite = bodySprites [spriteIndex];
		if (shirtSprites[spriteIndex] != null)
			shirtRenderer.sprite = shirtSprites [spriteIndex];
		if (pantsSprites[spriteIndex] != null)
			pantsRenderer.sprite = pantsSprites [spriteIndex];
	}
	void SetCurrentHatSpriteIndex (int spriteIndex) {
		if (hatSprites [spriteIndex] != null)
			hatRenderer.sprite = hatSprites [spriteIndex];
	}
    void SetCurrentHairSpriteIndex (int spriteIndex)
    {
        if (hairSprites[spriteIndex] != null)
        {
            Debug.Log(hairRenderer);
            hairRenderer.sprite = hairSprites[spriteIndex];
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
