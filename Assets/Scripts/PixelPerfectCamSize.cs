using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PixelPerfectCamSize : MonoBehaviour
{
	// The ideal camera orthographic size, which we'll try to find values close to
	[SerializeField] private float referenceSize = 8.4375f;

	private const int PIXELS_PER_UNIT = 16;

	private Camera thisCam;
	private float lastRecordedResolution;

    // Start is called before the first frame update
    private void Start()
    {
		thisCam = GetComponent<Camera>();
		if (thisCam == null)
		{
			Debug.LogError("This script needs a camera!");
		}
		thisCam.orthographicSize = FindBestSize(Screen.height);
		lastRecordedResolution = Screen.height;
    }

    // Update is called once per frame
    private void Update()
    {
		// If the screen dimensions have changed, recalculate the camera size
        if (Screen.height != lastRecordedResolution)
		{
			thisCam.orthographicSize = FindBestSize(Screen.height);
		}
		lastRecordedResolution = Screen.height;
    }

	// Finds an orthographic camera size close to the reference that provides an
	// exact integer pixels-per-unit
	private float FindBestSize (int resY)
	{
		float approxPixelsPerPixel = (resY / 2) / (referenceSize * PIXELS_PER_UNIT);
		float newPixelsPerPixel = Mathf.RoundToInt(approxPixelsPerPixel);
		if (newPixelsPerPixel < 1)
		{
			// If rounding would take us to zero, just keep the original pixel density
			newPixelsPerPixel = approxPixelsPerPixel;
		}
		return (resY / 2) / (newPixelsPerPixel * PIXELS_PER_UNIT);
	}
}
