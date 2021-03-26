using System;
using AI.Behaviours;
using AI.Trees.Nodes;
using SettlementSystem;
using UnityEngine;

// Decides what the Actor should do.
namespace AI
{
	public class ActorBehaviourAi : MonoBehaviour
	{
		private Actor actor;
		private ActorBehaviourExecutor executor;
		private SettlementManager settlement;

		// Update is called once per frame
		private void Update()
		{
			if (actor == null)
			{
				actor = GetComponent<Actor>();
				executor = GetComponent<ActorBehaviourExecutor>();
				Debug.Assert(actor != null);
				Debug.Assert(executor != null);
			}

			if (settlement == null)
			{
				settlement = FindObjectOfType<SettlementManager>();
				if (settlement == null)
				{
					Debug.LogError("SettlementManager object not found");
				}
			}
			if (actor.PlayerControlled)
			{
				executor.ForceCancelBehaviours();
				return;
			}

			Type behaviourType = EvaluateBehaviour(actor, out var args);
			executor.Execute(behaviourType, args);
		}

		private static Type EvaluateBehaviour (Actor actor, out object[] args)
		{
			Debug.Assert(!actor.PlayerControlled, "Tried to evaluate AI for player actor!");
		
			args = new object[] { actor }; // All of these behaviours only take one parameter
		
			// Dead people don't do much
			if (actor.GetData().PhysicalCondition.IsDead)
			{
				return typeof(BeDeadBehaviour);
			}

			// Traders always trade
			if (actor.GetData().Profession == Professions.TraderProfessionID)
			{
				return typeof(TraderBehaviour);
			}

			// Same faction as the player = this is a settler!
			string faction = actor.GetData().FactionStatus.FactionId;
			if (faction != null && faction == ActorRegistry.Get(PlayerController.PlayerActorId).data.FactionStatus.FactionId)
			{
				return typeof(SettlerBehaviour);
			}

			// No faction = violent drifter.
			if (faction == null)
			{
				args = new object[] { actor, new Follow(actor, ActorRegistry.Get(PlayerController.PlayerActorId).actorObject, 5) };
				return typeof(TreeBehaviour);
			}

			// Apparently not a settler or a trader; just a nobody.
			// I guess he'll just walk around like a loser.
			Debug.LogWarning("Couldn't determine a good behaviour tree for this actor: " + actor.ActorId);
			return typeof(GoForWalkBehaviour);
		}
	}
}
