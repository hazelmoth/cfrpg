using UnityEngine;

// A component for detecting when things get PUNCHED!.
public interface IImpactReceiver
{
	void OnImpact(float strength, Vector2 direction);
}
