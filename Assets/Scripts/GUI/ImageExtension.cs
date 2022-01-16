using UnityEngine;
using UnityEngine.UI;

namespace GUI
{
	public static class ImageExtension
	{
		/// Sets the image color to invisible if its sprite is null, or white if it's not.
		public static void SetAlphaIfNullSprite(this Image image)
		{
			if (image.sprite == null)
			{
				image.color = Color.clear;
			}
			else
			{
				image.color = Color.white;
			}
		}
	}
}
