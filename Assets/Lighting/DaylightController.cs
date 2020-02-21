using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class DaylightController : MonoBehaviour
{
	[SerializeField] private GameObject sunPrefab;
	private GameObject lightObject;
	private Light2D sunLight;
	private const float PEAK_INTENSITY = 0.85f;
	private const float MIN_INTENSITY = 0.4f;
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
		lightObject = GameObject.Instantiate(sunPrefab);
		lightObject.name = "Sun";
		sunLight = lightObject.GetComponent<Light2D>();
		sunLight.lightType = Light2D.LightType.Global;
		sunLight.intensity = PEAK_INTENSITY;
		sunLight.color = color;
		sunLight.shadowIntensity = 0;
	}
}
