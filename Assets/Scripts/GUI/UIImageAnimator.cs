﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GUI
{
	public class UIImageAnimator : MonoBehaviour
	{
		[SerializeField] private float animationFramerate = 5;
		[SerializeField] private Image image;
		[SerializeField] private List<Sprite> sprites;
		private float lastFrameChange = 0f;
		private int currentSprite = 0;

		// Start is called before the first frame update
		private void Start()
		{
			image.sprite = sprites[0];
		}

		// Update is called once per frame
		private void Update()
		{
			if (animationFramerate <= 0)
				return;
			float secPerSprite = 1f / animationFramerate;
			if (Time.time - lastFrameChange >= secPerSprite)
			{
				lastFrameChange = Time.time;
				NextSprite();
			}
		}

		private void NextSprite ()
		{
			currentSprite++;
			currentSprite = currentSprite % sprites.Count;
			image.sprite = sprites[currentSprite];
		}
	}
}
