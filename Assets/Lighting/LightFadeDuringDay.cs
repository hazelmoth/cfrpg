using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

// Fades out the brightness of a Light2D when the sun is bright. Uses the initial
// intensity of a BaseLightIntensity script as night intensity, and a editor-
// editable field for daytime intensity. Modifies the BaseLightIntensity instead
// of the actual light intensity.
public class LightFadeDuringDay : MonoBehaviour
{
    [SerializeField] private float minIntensity = 0.2f;
    [SerializeField] private float maxIntensity = 1f;

    private Light2D light2D;
    private BaseLightIntensity intensityReference;

    // Start is called before the first frame update
    void Start()
    {
        light2D = GetComponent<Light2D>();
        intensityReference = GetComponent<BaseLightIntensity>();
    }

    // Update is called once per frame
    void Update()
    {
        float sunBrightness = DaylightController.IntensityAsFraction;
        float darkness = 1 - sunBrightness;
        float intensity = darkness * (maxIntensity - minIntensity) + minIntensity;
        intensityReference.Intensity = intensity;
    }
}
