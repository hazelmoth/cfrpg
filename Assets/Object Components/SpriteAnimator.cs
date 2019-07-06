using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
	[SerializeField] float animationFramerate = 5;
	[SerializeField] SpriteRenderer spriteRenderer;
	[SerializeField] List<Sprite> sprites;
	float lastFrameChange = 0f;
	int currentSprite = 0;

	// Start is called before the first frame update
	void Start()
    {
		spriteRenderer.sprite = sprites[0];
    }

    // Update is called once per frame
    void Update()
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
	void NextSprite ()
	{
		currentSprite++;
		currentSprite = currentSprite % sprites.Count;
		spriteRenderer.sprite = sprites[currentSprite];
	}
}
