using System.Collections.Generic;
using UnityEngine;

/// A universal, systematic class for exerting punches or other sudden forces on actors,
/// entities, and items
public static class ImpactSystem
{
	public static void ExertDirectionalForce(
		Actor source,
		Vector2 originInScene,
		Vector2 direction,
		float distance,
		float strength,
		ImpactInfo.DamageType impactDamageType,
		string scene)
	{
		ExertDirectionalForce(source, originInScene, direction, distance, strength, impactDamageType, scene, new List<string>());
	}

	public static void ExertDirectionalForce(
		Actor source,
		Vector2 originInScene,
		Vector2 direction,
		float distance,
		float strength,
		ImpactInfo.DamageType impactDamageType,
		string scene,
		List<string> layers)
	{
		Vector2 worldOrigin = TilemapInterface.ScenePosToWorldPos(originInScene, scene);
		int layerMask = LayerMask.GetMask(layers.ToArray());
		if (layers.Count == 0)
			layerMask = ~0;

		RaycastHit2D hit = Physics2D.Raycast(worldOrigin, direction, distance, layerMask);
		Debug.DrawRay(worldOrigin, direction, Color.red, 0.3f);
		if (hit.collider == null) return;

		IImpactReceiver[] impactReceivers = hit.collider.gameObject.GetComponents<IImpactReceiver>();
		if (impactReceivers == null) return;

		foreach (IImpactReceiver impactReceiver in impactReceivers)
		{
			Vector2 force = direction.normalized * strength;
			impactReceiver.OnImpact(new ImpactInfo(impactDamageType, source, force));
		}
	}
}
