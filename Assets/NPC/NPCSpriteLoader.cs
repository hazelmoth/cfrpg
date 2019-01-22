using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Literally just loads the sprites for the NPC
public class NPCSpriteLoader : MonoBehaviour {

	public void LoadSprites (string spriteName) {
		Sprite[] sprites = Resources.LoadAll<Sprite> ("Sprites/" + spriteName);
		this.GetComponent<NPCSpriteController> ().SetSpriteArray (sprites);
	}
}
