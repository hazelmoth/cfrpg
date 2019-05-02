using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A component for detecting when things get PUNCHED!.
public class PunchReciever : MonoBehaviour
{
	public delegate void PunchRecieverEvent(float strength, Vector2 direction);
	public event PunchRecieverEvent OnPunched;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void RecievePunch (float strength, Vector2 direction)
	{
		OnPunched?.Invoke(strength, direction);
		Debug.Log(gameObject.name + " just got PUNCHED!");
	}
}
