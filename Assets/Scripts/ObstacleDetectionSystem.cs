using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using UnityEngine;

/// Used by pathfinding to detect whether an object (e.g. Actor) is blocking a path.
public class ObstacleDetectionSystem : MonoBehaviour
{
	private const float ColliderSize = 0.8f;
	private const int CollisionCheckerLayer = 12;

	private Dictionary<string, RegisteredActor> actors;

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

	public bool CheckForObstacles(Actor actor, Vector2 worldPos, Actor ignored = null)
	{
		RegisterIfUnregistered(actor);
		actors[actor.ActorId].collider.transform.position = worldPos;
		ISet<Collider2D> ignoredColliders = ImmutableHashSet.Create(
			actor.GetComponent<Collider2D>(),
			ignored != null ? ignored.GetComponent<Collider2D>() : null);

		return actors[actor.ActorId].checker.Colliding(ignoredColliders);
	}

	private void RegisterIfUnregistered(Actor actor)
	{
		actors ??= new Dictionary<string, RegisteredActor>();

		// Unregister this actor if it's registered but the object has been destroyed.
		if (actors.ContainsKey(actor.ActorId) && actors[actor.ActorId].actor == null)
		{
			actors.Remove(actor.ActorId);
		}

		if (!actors.ContainsKey(actor.ActorId) || actors[actor.ActorId] == null)
		{
			actors[actor.ActorId] = new RegisteredActor(actor);
		}
	}

	private static void CreateCollider(RegisteredActor actor)
	{
		GameObject colliderObject = new GameObject("Obstacle Check Collider");
		colliderObject.layer = CollisionCheckerLayer;
		BoxCollider2D collider = colliderObject.AddComponent<BoxCollider2D>();
		collider.size = Vector2.one * ColliderSize;
		collider.isTrigger = true;
		actor.collider = collider;
		actor.checker = collider.gameObject.AddComponent<CollisionChecker>();
	}
}
