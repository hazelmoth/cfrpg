using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A component for detecting when things get PUNCHED!.
public interface IImpactReceiver
{
	void OnImpact(float strength, Vector2 direction);
}
