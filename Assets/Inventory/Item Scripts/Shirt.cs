using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="NewShirt", menuName = "Items/Shirt", order = 1)]
public class Shirt : Item {
	[SerializeField] Sprite standDown;
	[SerializeField] Sprite standRight;
	[SerializeField] Sprite standLeft;
	[SerializeField] Sprite standUp;
	[SerializeField] Sprite walkDown1;
	[SerializeField] Sprite walkDown2;
	[SerializeField] Sprite walkUp1;
	[SerializeField] Sprite walkUp2;
	[SerializeField] Sprite walkRight1;
	[SerializeField] Sprite walkRight2;
	[SerializeField] Sprite walkLeft1;
	[SerializeField] Sprite walkLeft2;

	public Sprite[] GetShirtSprites () {
		return new Sprite[] {
			standDown, standRight, standLeft, standUp,
			walkDown1, walkDown2,
			walkUp1, walkUp2,
			walkRight1, walkRight2,
			walkLeft1, walkLeft2
		};
	}
}
