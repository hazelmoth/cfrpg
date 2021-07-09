using System;
using System.Collections.Generic;
using UnityEngine;

// Used by pathfinding to detect whether an object (e.g. Actor) is blocking a path.
public class ObstacleDetectionSystem : MonoBehaviour
{
	private const float ColliderSize = 0.8f;
	private const int CollisionCheckerLayer = 12;

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

	public static bool CheckForObstacles(Actor actor, Vector2 worldPos, Actor ignored = null)
	{
		RegisterIfUnregistered(actor);

		instance.actors[actor.ActorId].collider.transform.position = worldPos;
		return instance.actors[actor.ActorId].checker.Colliding(ignored != null ? ignored.gameObject : null);
	}

	private static void RegisterIfUnregistered(Actor actor)
	{
		instance.actors ??= new Dictionary<string, RegisteredActor>();

		// Unregister this actor if it's registered but the collider has been destroyed.
		if (instance.actors.ContainsKey(actor.ActorId) && instance.actors[actor.ActorId].collider == null)
		{
			instance.actors.Remove(actor.ActorId);
		}

		if (!instance.actors.ContainsKey(actor.ActorId) || instance.actors[actor.ActorId] == null)
		{
			instance.actors[actor.ActorId] = new RegisteredActor(actor);
		}
	}

	private static void CreateCollider(RegisteredActor actor)
	{
		GameObject colliderObject = new GameObject("Obstacle Check Collider");
		colliderObject.transform.SetParent(actor.actor.gameObject.transform);
		colliderObject.layer = CollisionCheckerLayer;
		BoxCollider2D collider = colliderObject.AddComponent<BoxCollider2D>();
		collider.size = Vector2.one * ColliderSize;
		collider.offset = Vector2.one * 0.5f; // Offset the collider since it will be positioned on tile coordinates
		collider.isTrigger = true;
		actor.collider = collider;
		actor.checker = collider.gameObject.AddComponent<CollisionChecker>();
	}
}