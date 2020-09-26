using System.Collections.Generic;
using UnityEngine;

// A universal, systematic class for exerting punches or other sudden forces on actors, entities, and items
public class PunchSystem
{

	public static void ExertDirectionalPunch(Vector2 originInScene, Vector2 direction, float distance, float strength, string scene) {
		ExertDirectionalPunch(originInScene, direction, distance, strength, scene, new List<string>());
	}
	public static void ExertDirectionalPunch (Vector2 originInScene, Vector2 direction, float distance, float strength, string scene, List<string> layers)
	{
		Vector2 worldOrigin = TilemapInterface.ScenePosToWorldPos(originInScene, scene);
		int layerMask = LayerMask.GetMask(layers.ToArray());
		if (layers.Count == 0)
			layerMask = ~0;

		RaycastHit2D hit = Physics2D.Raycast(worldOrigin, direction, distance, layerMask);
		Debug.DrawRay(worldOrigin, direction, Color.red, 0.3f);
		if (hit.collider != null)
		{
			IImpactReceiver[] punchRecievers = hit.collider.gameObject.GetComponents<IImpactReceiver>();
			if (punchRecievers != null)
			{
				foreach (IImpactReceiver punchReciever in punchRecievers)
				{
					punchReciever.OnImpact(strength, direction);
				}
			}
		}
	}
}
