using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClothingManager : MonoBehaviour {

	[SerializeField] SpriteRenderer hatRenderer;
	[SerializeField] SpriteRenderer shirtRenderer;
	[SerializeField] SpriteRenderer pantsRenderer;

	Hat equippedHat;
	Shirt equippedShirt;
	Pants equippedPants;

	Sprite hatDown, hatRight, hatLeft, hatUp;
	Sprite[] shirtFramesDown, shirtFramesRight, shirtFramesLeft, shirtFramesUp;
	Sprite[] pantsFramesDown, pantsFramesRight, pantsFramesLeft, pantsFramesUp;

	int currentFrame;
	Direction currentDir;

	// Use this for initialization
	void Start () {
		PlayerAnimController.OnDirectionChange += ChangeDirection;
		PlayerInventory.OnHatEquipped += EquipHat;
		PlayerInventory.OnShirtEquipped += EquipShirt;
		PlayerInventory.OnPantsEquipped += EquipPants;
		pantsFramesDown = new Sprite[3];
		pantsFramesRight = new Sprite[3];
		pantsFramesLeft = new Sprite[3];
		pantsFramesUp = new Sprite[3];
		shirtFramesDown = new Sprite[3];
		shirtFramesRight = new Sprite[3];
		shirtFramesLeft = new Sprite[3];
		shirtFramesUp = new Sprite[3];
	}

	void SetHatSprites (Hat hat) {
		if (hat == null) {
			hatDown = null;
			hatRight = null;
			hatLeft = null;
			hatUp = null;
			return;
		}
		hatDown = hat.GetHatSprites() [0];
		hatRight = hat.GetHatSprites() [1];
		hatLeft = hat.GetHatSprites() [2];
		hatUp = hat.GetHatSprites() [3];
	}
	void SetPantsSprites (Pants pants) {
		if (pants == null) {
			pantsFramesDown = new Sprite[3];
			pantsFramesRight = new Sprite[3];
			pantsFramesLeft = new Sprite[3];
			pantsFramesUp = new Sprite[3];
			return;
		}
		Sprite[] sprites = pants.GetPantsSprites ();
		pantsFramesDown = new Sprite[] { sprites [0], sprites [4], sprites [5] };
		pantsFramesRight = new Sprite[] { sprites [1], sprites [8], sprites [9] };
		pantsFramesLeft = new Sprite[] { sprites [2], sprites [10], sprites [11] };
		pantsFramesUp = new Sprite[] { sprites [3], sprites [6], sprites [7] };
	}

	void SetShirtSprites (Shirt shirt) {
		if (shirt == null) {
			shirtFramesDown = new Sprite[3];
			shirtFramesRight = new Sprite[3];
			shirtFramesLeft = new Sprite[3];
			shirtFramesUp = new Sprite[3];
			return;
		}
		Sprite[] sprites = shirt.GetShirtSprites ();
		shirtFramesDown = new Sprite[] { sprites [0], sprites [4], sprites [5] };
		shirtFramesRight = new Sprite[] { sprites [1], sprites [8], sprites [9] };
		shirtFramesLeft = new Sprite[] { sprites [2], sprites [10], sprites [11] };
		shirtFramesUp = new Sprite[] { sprites [3], sprites [6], sprites [7] };
	}


	void EquipHat (Hat hat) {
		equippedHat = hat;
		SetHatSprites (hat);
		UpdateSprites();
	}
	void EquipShirt (Shirt shirt) {
		equippedShirt = shirt;
		SetShirtSprites (shirt);
		UpdateSprites ();
	}
	void EquipPants (Pants pants) {
		equippedPants = pants;
		SetPantsSprites (pants);
		UpdateSprites ();
	}

	void ChangeDirection (Direction dir) {
		currentDir = dir;
		UpdateSprites ();
	}

	void UpdateSprites () {
		switch (currentDir) {
		case Direction.Down:
			hatRenderer.sprite = hatDown;
			shirtRenderer.sprite = shirtFramesDown [currentFrame];
			pantsRenderer.sprite = pantsFramesDown [currentFrame];
			break;
		case Direction.Right:
			hatRenderer.sprite = hatRight;
			shirtRenderer.sprite = shirtFramesRight [currentFrame];
			pantsRenderer.sprite = pantsFramesRight [currentFrame];
			break;
		case Direction.Left:
			hatRenderer.sprite = hatLeft;
			shirtRenderer.sprite = shirtFramesLeft [currentFrame];
			pantsRenderer.sprite = pantsFramesLeft [currentFrame];
			break;
		case Direction.Up:
			hatRenderer.sprite = hatUp;
			shirtRenderer.sprite = shirtFramesUp [currentFrame];
			pantsRenderer.sprite = pantsFramesUp [currentFrame];
			break;
		}
	}

	public void SetFrame (int frame) {
		currentFrame = frame;
		UpdateSprites ();
	}

	public void AdvanceFrame () {
		currentFrame++;
	}
}
