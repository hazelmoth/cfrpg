using System;
using AI.Behaviours;
using AI.Trees;
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
			if (PauseManager.Paused) return;
			
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

			// If we're fighting someone, attack them.
			if (actor.HostileTargets.Count > 0)
			{
				args = new object[] { actor, new MeleeFight(actor, actor.HostileTargets.Peek()) };
				return typeof(TreeBehaviour);
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

			// No faction; probably wildlife. Wander around.
			if (faction == null)
			{
				args = new object[] { actor, new Repeater(new Task(typeof(MoveRandomly), new object[]{actor, 20})) };
				return typeof(TreeBehaviour);
			}
			
			Debug.LogWarning("Couldn't determine a good behaviour tree for this actor: " + actor.ActorId);
			return typeof(GoForWalkBehaviour);
		}
	}
}
