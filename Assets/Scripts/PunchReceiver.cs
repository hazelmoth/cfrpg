using UnityEngine;

// A component for detecting when things get PUNCHED!.
public interface IImpactReceiver
{
	void OnImpact(ImpactInfo impact);
}
