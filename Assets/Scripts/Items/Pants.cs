using UnityEngine;

[CreateAssetMenu(fileName="NewPants", menuName = "Items/Pants", order = 1)]
public class Pants : ItemData {

	[SerializeField] private Sprite[] sprites;

	public Sprite[] GetPantsSprites () {
		return sprites;
	}
}
