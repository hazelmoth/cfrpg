using UnityEngine;
using UnityEngine.UI;

public static class ImageExtension
{
	/// <summary>
	/// Sets the image color to invisible if its sprite is null, or white if it's not.
	/// </summary>
	public static void SetAlphaIfNullSprite (this Image image)
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
