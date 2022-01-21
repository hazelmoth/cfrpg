using AI.Trees;
using AI.Trees.Nodes;
using ContinentMaps;
using SettlementSystem;
using UnityEngine;

namespace AI
{
	/// Decides what the Actor should do.
	public class ActorBehaviourAi : MonoBehaviour
	{
		private Actor actor;
		private ActorBehaviourExecutor executor;
		private SettlementManager settlementManager;

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

			if (settlementManager == null)
			{
				settlementManager = FindObjectOfType<SettlementManager>();
				if (settlementManager == null)
				{
					Debug.LogError("SettlementManager object not found");
				}
			}
			if (actor.PlayerControlled)
			{
				executor.CancelTasks();
				return;
			}

			Task behaviour = EvaluateBehaviour();
			executor.Execute(behaviour);
		}

		private Task EvaluateBehaviour ()
		{
			Debug.Assert(!actor.PlayerControlled, "Tried to evaluate AI for player actor!");

			// Dead people do nothing.
			if (actor.GetData().Health.IsDead)
			{
				return new Task(typeof(Wait), new object[] { 1 });
			}

            // While speaking, actors look at the player (assumes all dialogue involves the player)
            if (actor.InDialogue)
            {
				return new Task(typeof(StareAtActor), new object[] { actor, PlayerController.GetPlayerActor() });
            }
			
			// Slug people are aggressive
			if (actor.GetData().RaceId == "slug_person")
			{
				return new Task(typeof(AggroAnimalBehaviour), new object[] { actor });
			}

			// If we're fighting someone, attack them.
			if (actor.HostileTargets.Count > 0)
			{
				return new Task(typeof(MeleeFight), new object[] { actor, actor.HostileTargets.Peek() });
			}

			// Traders always trade
			if (actor.GetData().Profession == Professions.TraderProfessionID)
			{
				// TODO: rewrite TraderBehaviour as behaviour tree
				//return typeof(TraderBehaviour);
			}

			// If the actor has a house in this region, they'll act as a settler.
			if (settlementManager.GetHomeScene(actor.ActorId, ContinentManager.CurrentRegionId) != null)
			{
				return new Task(typeof(SettlerBehaviour), new object[] { actor });
			}

			string faction = actor.GetData().FactionStatus.FactionId;

			// No faction; probably wildlife. Wander around.
			if (faction == null)
			{
				return Wander.MakeTask(actor);
			}
			
			Debug.LogWarning("Couldn't determine a good behaviour tree for this actor: " + actor.ActorId);
			return new Task(typeof(Wait), new object[] { 1 });
		}
	}
}
