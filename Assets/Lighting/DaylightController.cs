using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class DaylightController : MonoBehaviour
{
	[SerializeField] private GameObject sunPrefab = null;
	[SerializeField] private AnimationCurve brightnessCurve = null;
	[SerializeField] private AnimationCurve redCurve = null;
	[SerializeField] private AnimationCurve greenCurve = null;
	[SerializeField] private AnimationCurve blueCurve = null;
	private GameObject lightObject;
	private Light2D sunLight;
	private const float PEAK_INTENSITY = 1f;
	private const float MIN_INTENSITY = 0.4f;


	// Start is called before the first frame update
	void Start()
    {
        if (sunLight == null)
		{
			CreateSunLightObject();
		}
    }

	// Update is called once per frame
	void Update()
	{
		float time = TimeKeeper.TimeAsFraction;
		sunLight.intensity = brightnessCurve.Evaluate(time) * (PEAK_INTENSITY - MIN_INTENSITY) + MIN_INTENSITY;

		float r = redCurve.Evaluate(time);
		float g = greenCurve.Evaluate(time);
		float b = blueCurve.Evaluate(time);
		sunLight.color = new Color(r, g, b);
    }
	void CreateSunLightObject ()
	{
		lightObject = GameObject.Instantiate(sunPrefab);
		lightObject.name = "Sun";
		sunLight = lightObject.GetComponent<Light2D>();
		sunLight.lightType = Light2D.LightType.Global;
		sunLight.intensity = PEAK_INTENSITY;
		sunLight.shadowIntensity = 0;
	}
}
