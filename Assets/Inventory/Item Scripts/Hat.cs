using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="NewItem", menuName = "Items/Hat", order = 1)]
public class Hat : Item {
	[SerializeField] Sprite spriteDown;
	[SerializeField] Sprite spriteRight;
	[SerializeField] Sprite spriteLeft;
	[SerializeField] Sprite spriteUp;

	public Sprite SpriteForward { get {return spriteDown;} }
	public Sprite SpriteRight { get {return spriteRight;} }
	public Sprite SpriteLeft { get {return spriteLeft;} }
	public Sprite SpriteBack { get {return spriteUp;} }

	public Sprite[] GetHatSprites() {
		return new Sprite[] { spriteDown, spriteRight, spriteLeft, spriteUp };
	}
}
