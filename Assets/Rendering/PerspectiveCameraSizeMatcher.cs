using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerspectiveCameraSizeMatcher : MonoBehaviour
{
	[SerializeField] float distance = 10f;
	[SerializeField] Camera orthographicCam;
	Camera thisCamera;

    // Start is called before the first frame update
    void Start()
    {
		thisCamera = GetComponent<Camera>();
		if (thisCamera == null)
			Debug.LogError("No camera component attached to this GameObject!");

		thisCamera.fieldOfView = CalulateFov();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	float CalulateFov()
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
