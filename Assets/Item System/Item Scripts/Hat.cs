using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="NewItem", menuName = "Items/Hat", order = 1)]
public class Hat : Item {
	[SerializeField] Sprite spriteDown = null;
	[SerializeField] Sprite spriteRight = null;
	[SerializeField] Sprite spriteLeft = null;
	[SerializeField] Sprite spriteUp = null;

	public Sprite SpriteForward { get {return spriteDown;} }
	public Sprite SpriteRight { get {return spriteRight;} }
	public Sprite SpriteLeft { get {return spriteLeft;} }
	public Sprite SpriteBack { get {return spriteUp;} }

	public Sprite[] GetHatSprites() {
		return new Sprite[] { spriteDown, spriteRight, spriteLeft, spriteUp };
	}

    // A hat to represent no hat
    public static Hat GetEmptyHatObject ()
    {
        return ScriptableObject.CreateInstance<Hat>();
    }
}
