using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaylightController : MonoBehaviour
{
	private GameObject lightObject;
	private Light sunLight;
	private const float PEAK_INTENSITY = 1f;
	private const float MIN_INTENSITY = 0.0f;
	private Color color = new Color(0.8901961f, 0.8784314f, 0.8156863f);


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
		lightObject.transform.localRotation = CalculateRotation(TimeKeeper.TimeAsFraction);
    }
	Quaternion CalculateRotation(float timeFraction)
	{
		return Quaternion.Euler(new Vector3(0f, timeFraction * 360f + 180, 0f));
	}
	void CreateSunLightObject ()
	{
		lightObject = new GameObject();
		lightObject.name = "Sun";
		sunLight = lightObject.AddComponent<Light>();
		sunLight.type = LightType.Directional;
		sunLight.intensity = PEAK_INTENSITY;
		sunLight.color = color;
		sunLight.shadows = LightShadows.None;

	}
}
