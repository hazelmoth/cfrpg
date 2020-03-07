using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using System.Collections;

public class LightFlicker : MonoBehaviour
{
	public float MaxReduction;
	public float MaxIncrease;
	public float RateDamping;
	public float Strength;
	public bool StopFlickering;

	private Light2D _lightSource;
	private BaseLightIntensity _intensityReference;
	private float _baseIntensity;
	private bool _flickering;

	public void Reset()
	{
		MaxReduction = 0.2f;
		MaxIncrease = 0.2f;
		RateDamping = 0.1f;
		Strength = 300;
	}

	public void Start()
	{
		_lightSource = GetComponent<Light2D>();
		if (_lightSource == null)
		{
			Debug.LogError("Flicker script must have a Light2D Component on the same GameObject.");
			return;
		}
		_intensityReference = GetComponent<BaseLightIntensity>();
		_baseIntensity = _lightSource.intensity;
		StartCoroutine(DoFlicker());
	}

	void Update()
	{
		if (!StopFlickering && !_flickering)
		{
			StartCoroutine(DoFlicker());
		}
	}

	private IEnumerator DoFlicker()
	{
		_flickering = true;
		while (!StopFlickering)
		{
			_lightSource.intensity = Mathf.Lerp(_lightSource.intensity, Random.Range(BaseIntensity - MaxReduction, BaseIntensity + MaxIncrease), Strength * Time.deltaTime);
			yield return new WaitForSeconds(RateDamping);
		}
		_flickering = false;
	}

    private float BaseIntensity
    {
        get
        {
            if (_intensityReference != null)
            {
				return _intensityReference.Intensity;
            }
            else
            {
				return _baseIntensity;
            }
        }
    }
}