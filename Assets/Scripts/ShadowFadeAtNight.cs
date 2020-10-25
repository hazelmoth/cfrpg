using UnityEngine;

public class ShadowFadeAtNight : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float maxAlpha;
    private const float minAlpha = 0.1f;

    // Start is called before the first frame update
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        maxAlpha = spriteRenderer.color.a;
    }

    // Update is called once per frame
    private void Update()
    {
        float sunIntensity = DaylightController.IntensityAsFraction;
        float alpha = sunIntensity * (maxAlpha - minAlpha) + minAlpha;
        Color current = spriteRenderer.color;
        Color newColor = new Color(current.r, current.g, current.b, alpha);
        spriteRenderer.color = newColor;
    }
}