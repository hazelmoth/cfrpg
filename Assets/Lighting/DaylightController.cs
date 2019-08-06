using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaylightController : MonoBehaviour
{
	GameObject lightObject;
	Light sunLight;
	const float peakIntensity = 0.8f;
	const float minIntensity = 0.0f;


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
		SetIntensity(TimeKeeper.TimeAsFraction);
    }
	void SetIntensity(float timeFraction)
	{
		//float intensityFraction = -Mathf.Cos(timeFraction * 2 * Mathf.PI) / 2 + 0.5f;
		//sunLight.intensity = intensityFraction * (peakIntensity - minIntensity) + minIntensity;

		lightObject.transform.rotation = Quaternion.Euler(new Vector3(0f, timeFraction * 360f + 180, 0f));
	}
	void CreateSunLightObject ()
	{
		lightObject = new GameObject();
		lightObject.name = "Sun";
		sunLight = lightObject.AddComponent<Light>();
		sunLight.type = LightType.Directional;
		sunLight.intensity = peakIntensity;
		sunLight.shadows = LightShadows.Soft;
	}
}
