using UnityEngine;

namespace Items
{
	[CreateAssetMenu(fileName = "NewItem", menuName = "Items/Hat", order = 1)]
	public class Hat : ItemData, IHat
	{
		[SerializeField] private Sprite spriteDown = null;
		[SerializeField] private Sprite spriteRight = null;
		[SerializeField] private Sprite spriteLeft = null;
		[SerializeField] private Sprite spriteUp = null;

		Sprite[] IHat.GetHatSprites()
		{
			return new Sprite[] { spriteDown, spriteRight, spriteLeft, spriteUp };
		}
	}
}