using UnityEngine;

namespace Items
{
	[CreateAssetMenu(fileName="NewShirt", menuName = "Items/Shirt")]
	public class Shirt : ItemData {
		[SerializeField] private Sprite[] sprites;

		public Sprite[] GetShirtSprites () {
			return sprites;
		}
	}
}
