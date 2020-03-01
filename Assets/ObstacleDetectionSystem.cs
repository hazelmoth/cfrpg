using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDetectionSystem : MonoBehaviour
{
	private const float COLLIDER_SIZE = 0.8f;
	private const int COLLISION_CHECKER_LAYER = 12;

	private Dictionary<string, RegisteredActor> actors;
	private static ObstacleDetectionSystem instance;

	private class RegisteredActor
	{
		public RegisteredActor (Actor actor)
		{
			this.actor = actor;
			CreateCollider(this);
		}
		public Actor actor;
		public BoxCollider2D collider;
		public CollisionChecker checker;
	}

	private void Start()
	{
		instance = this;
	}
	public static bool CheckForObstacles(Actor actor, Vector2 worldPos)
	{

		RegisterIfUnregistered(actor);

		instance.actors[actor.ActorId].collider.transform.position = worldPos;
		return (instance.actors[actor.ActorId].checker.Colliding);
	}

	private static void RegisterIfUnregistered(Actor actor)
	{
		if (instance.actors == null)
		{
			instance.actors = new Dictionary<string, RegisteredActor>();
		}

		if (!instance.actors.ContainsKey(actor.ActorId))
		{
			instance.actors.Add(actor.ActorId, new RegisteredActor(actor));
		}
	}

	private static void CreateCollider(RegisteredActor actor)
	{
		GameObject colliderObject = new GameObject("Obstacle Check Collider");
		colliderObject.transform.SetParent(actor.actor.gameObject.transform);
		colliderObject.layer = COLLISION_CHECKER_LAYER;
		BoxCollider2D collider = colliderObject.AddComponent<BoxCollider2D>();
		collider.size = Vector2.one * COLLIDER_SIZE;
		collider.offset = Vector2.one * 0.5f; // Offset the collider since it will be positioned on tile coordinates
		collider.isTrigger = true;
		actor.collider = collider;
		actor.checker = collider.gameObject.AddComponent<CollisionChecker>();
	}
}