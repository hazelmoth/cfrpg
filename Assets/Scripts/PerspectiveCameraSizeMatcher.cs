using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerspectiveCameraSizeMatcher : MonoBehaviour
{
	[SerializeField] private float distance = 10f;
	[SerializeField] private Camera orthographicCam;
	private Camera thisCamera;

    // Start is called before the first frame update
    private void Start()
    {
		thisCamera = GetComponent<Camera>();
		if (thisCamera == null)
			Debug.LogError("No camera component attached to this GameObject!");

		thisCamera.fieldOfView = CalulateFov();
    }

    // Update is called once per frame
    private void Update()
    {
		thisCamera.fieldOfView = CalulateFov();
	}

    private float CalulateFov()
	{
		if (!orthographicCam.orthographic)
		{
			Debug.LogError("Target camera isn't orthographic!");
		}
		float orthSize = orthographicCam.orthographicSize;
		float halfAngle = Mathf.Rad2Deg * Mathf.Atan(orthSize / distance);
		return halfAngle * 2;
	}
}
