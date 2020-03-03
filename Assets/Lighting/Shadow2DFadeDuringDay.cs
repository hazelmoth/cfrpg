using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

// Fades out the shadow intensity of an attached Light2d component during the
// day, with a cutoff for max sunlight to produce a shadow.
public class Shadow2DFadeDuringDay : MonoBehaviour
{
    [SerializeField] private float minIntensity = 0f;
    [SerializeField] private float maxIntensity = 0.5f;
    // The maximum sunlight that can produce any shadows
    [SerializeField] private float shadowSunlightCutoff = 0.9f;

    private Light2D light2D;

    // Start is called before the first frame update
    void Start()
    {
        light2D = GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float brightness = DaylightController.IntensityAsFraction;
        float shadowFactor;
        if (brightness >= shadowSunlightCutoff)
        {
            shadowFactor = 0;
        } else
        {
            shadowFactor = 1 - (brightness / shadowSunlightCutoff);
        }
        float shadowIntensity = shadowFactor * (maxIntensity - minIntensity) + minIntensity;
        light2D.shadowIntensity = shadowIntensity;
    }
}
