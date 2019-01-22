using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Sets the right sprite for the NPC based off of what the animator and Anim Controller are doing
public class NPCSpriteController : MonoBehaviour {

	[SerializeField] SpriteRenderer spriteRenderer = null;
	HumanAnimController animController;
	Sprite[] sprites = null;

	void Start () {
		animController = this.GetComponent<HumanAnimController> ();
	}

	public void SetSpriteArray (Sprite[] sprites) {
		this.sprites = sprites;
	}

	// Called by animation events
	public void SetFrame (int animFrame) {
		if (animFrame == 0) {
			switch (animController.GetDirection ()) {
			case Direction.Down:
				setCurrentSprite (0);
				break;
			case Direction.Right:
				setCurrentSprite (1);
				break;
			case Direction.Up:
				setCurrentSprite (3);
				break;
			case Direction.Left:
				setCurrentSprite (2);
				break;
			}
		}
		else if (animFrame == 1) {
			switch (animController.GetDirection ()) {
			case Direction.Down:
				setCurrentSprite (4);
				break;
			case Direction.Right:
				setCurrentSprite (8);
				break;
			case Direction.Up:
				setCurrentSprite (6);
				break;
			case Direction.Left:
				setCurrentSprite (10);
				break;
			}
		}
		else /*if (animFrame == 2)*/ {
			switch (animController.GetDirection ()) {
			case Direction.Down:
				setCurrentSprite (5);
				break;
			case Direction.Right:
				setCurrentSprite (9);
				break;
			case Direction.Up:
				setCurrentSprite (7);
				break;
			case Direction.Left:
				setCurrentSprite (11);
				break;
			}
		}
	}

	void setCurrentSprite (int spriteIndex) {
		spriteRenderer.sprite = sprites [spriteIndex];
	}
}
