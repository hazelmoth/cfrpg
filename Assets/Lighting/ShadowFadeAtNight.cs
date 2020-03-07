using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowFadeAtNight : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float maxAlpha;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        maxAlpha = spriteRenderer.color.a;
    }

    // Update is called once per frame
    void Update()
    {
        float sunIntensity = DaylightController.IntensityAsFraction;
        float alpha = sunIntensity * maxAlpha;
        Color current = spriteRenderer.color;
        Color newColor = new Color(current.r, current.g, current.b, alpha);
        spriteRenderer.color = newColor;
    }
}