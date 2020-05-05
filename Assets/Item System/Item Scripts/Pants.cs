using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="NewPants", menuName = "Items/Pants", order = 1)]
public class Pants : Item {

	[SerializeField] private Sprite[] sprites;

	public Sprite[] GetPantsSprites () {
		return sprites;
	}
}
