using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="NewShirt", menuName = "Items/Shirt", order = 1)]
public class Shirt : ItemData {
	[SerializeField] private Sprite[] sprites;

	public Sprite[] GetShirtSprites () {
		return sprites;
	}
}
