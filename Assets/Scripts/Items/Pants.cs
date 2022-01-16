using UnityEngine;

namespace Items
{
	[CreateAssetMenu(fileName="NewPants", menuName = "Items/Pants")]
	public class Pants : ItemData {

		[SerializeField] private Sprite[] sprites;

		public Sprite[] GetPantsSprites () {
			return sprites;
		}
	}
}
