using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A universal, systematic class for exerting punches or other sudden forces on actors, entities, and items
public class PunchSystem : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public static void ExertDirectionalPunch(Vector2 originInScene, Direction direction, float distance, float strength, string scene) {
		ExertDirectionalPunch(originInScene, direction.ToVector2(), distance, strength, scene, new List<string>());
	}
	public static void ExertDirectionalPunch (Vector2 originInScene, Direction direction, float distance, float strength, string scene, List<string> layers) {
		ExertDirectionalPunch(originInScene, direction.ToVector2(), distance, strength, scene, layers);
	}
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
		Debug.Log("Punch!");
		if (hit.collider != null)
		{
			PunchReciever[] punchRecievers = hit.collider.gameObject.GetComponents<PunchReciever>();
			Debug.Log("There's a " + hit.collider.gameObject.name);
			if (punchRecievers != null)
			{
				foreach (PunchReciever punchReciever in punchRecievers)
				{
					punchReciever.OnPunch(strength, direction);
					Debug.Log("punching a " + ((Component)punchReciever).gameObject.name);
				}
			}
		}
	}
}
