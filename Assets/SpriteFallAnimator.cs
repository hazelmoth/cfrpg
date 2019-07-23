using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFallAnimator : MonoBehaviour
{
    const float gravConstant = 9.8f;

    static SpriteFallAnimator instance;
    public class FallingSprite 
    { 
        public SpriteRenderer sprite; 
        public float distance;
        public float gravMultiplier;
        public float startY;
    }
    static List<FallingSprite> spritesToAnimate = null;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        spritesToAnimate = new List<FallingSprite>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = spritesToAnimate.Count - 1; i >= 0; i--)
        {
            float distToFall = gravConstant * Time.deltaTime;
            float newYHeight = spritesToAnimate[i].sprite.transform.position.y - distToFall;
            newYHeight = Mathf.Max(newYHeight, spritesToAnimate[i].startY - spritesToAnimate[i].distance);

            spritesToAnimate[i].sprite.transform.position = new Vector3
            (
                spritesToAnimate[i].sprite.transform.position.x,
                newYHeight,
                spritesToAnimate[i].sprite.transform.position.z
            );
            //BUG it seems startY is continually being changed because the fall keeps being restarted
            Debug.Log(spritesToAnimate[i].startY - spritesToAnimate[i].distance);
            Debug.Log(spritesToAnimate[i].sprite.transform.position.y);
            if (newYHeight <= spritesToAnimate[i].startY - spritesToAnimate[i].distance)
            {
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
        }
        FallingSprite spriteData = new FallingSprite();
        spriteData.sprite = sprite;
        spriteData.distance = distance;
        spriteData.gravMultiplier = gravMultiplier;
        spriteData.startY = sprite.transform.position.y;
        spritesToAnimate.Add(spriteData);
        return spriteData;
    }
}
