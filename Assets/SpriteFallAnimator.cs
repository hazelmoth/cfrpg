using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFallAnimator : MonoBehaviour
{
    const float gravConstant = 12f;

    static SpriteFallAnimator instance;
    public class FallingSprite 
    { 
        public SpriteRenderer sprite; 
        public float distance;
        public float gravMultiplier;
        public float startY;
		public float startTime;
    }
    static List<FallingSprite> spritesToAnimate = null;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
		
        for (int i = spritesToAnimate.Count - 1; i >= 0; i--)
        {
			float elapsedTime = Time.time - spritesToAnimate[i].startTime;
			float distToFall = gravConstant * spritesToAnimate[i].gravMultiplier * elapsedTime * Time.deltaTime;
			float targetHeight = spritesToAnimate[i].startY - spritesToAnimate[i].distance;
			float newYHeight = spritesToAnimate[i].sprite.transform.localPosition.y - distToFall;

			if (spritesToAnimate[i].distance >= 0)
				newYHeight = Mathf.Max(newYHeight, targetHeight);
			else
				newYHeight = Mathf.Min(newYHeight, targetHeight);

            spritesToAnimate[i].sprite.transform.localPosition = new Vector3
            (
                spritesToAnimate[i].sprite.transform.localPosition.x,
                newYHeight,
                spritesToAnimate[i].sprite.transform.localPosition.z
            );

			// Check if the fall is completed, whether we're falling up or down
            if ((spritesToAnimate[i].distance >= 0 && spritesToAnimate[i].sprite.transform.localPosition.y <= targetHeight) || 
				(spritesToAnimate[i].distance < 0 && spritesToAnimate[i].sprite.transform.localPosition.y >= targetHeight))
			{
				spritesToAnimate[i] = null;
                spritesToAnimate.RemoveAt(i);
                Debug.Log("removed");
            }
        }
    }
    public static FallingSprite AnimateFall(SpriteRenderer sprite, float distance, float gravMultiplier)
    {
        if (Equals(gravMultiplier, 0f))
            return null;

        if (instance == null)
        {
            GameObject gameObject = new GameObject();
            instance = gameObject.AddComponent<SpriteFallAnimator>();
            spritesToAnimate = new List<FallingSprite>();
			return AnimateFall(sprite, distance, gravMultiplier);
        }
		// Remove any existing FallingSprite objects for this sprite
		for (int i = spritesToAnimate.Count - 1; i >= 0; i--)
		{
			if (spritesToAnimate[i].sprite.GetInstanceID() == sprite.GetInstanceID())
			{
				spritesToAnimate.RemoveAt(i);
			}
		}

		FallingSprite spriteData = new FallingSprite();
        spriteData.sprite = sprite;
        spriteData.distance = distance;
        spriteData.gravMultiplier = gravMultiplier;
        spriteData.startY = sprite.transform.localPosition.y;
		spriteData.startTime = Time.time;
        spritesToAnimate.Add(spriteData);
        return spriteData;
    }
	public static void CancelFall (FallingSprite fallingSpriteObject)
	{
		if (fallingSpriteObject == null)
		{
			return;
		}
		if (spritesToAnimate.Contains(fallingSpriteObject))
		{
			spritesToAnimate.Remove(fallingSpriteObject);
		}
		else
		{
			Debug.LogWarning("Tried to cancel a falling sprite that isn't registered with SpriteFallAnimator.");
		}
	}
}
